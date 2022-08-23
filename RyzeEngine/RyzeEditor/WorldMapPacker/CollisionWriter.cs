using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BulletSharp;
using BulletSharp.Math;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Packer
{
    public class BoxShapeEx : BoxShape
    {
        public Vector3 Center { get; set; }

        public BoxShapeEx(Vector3 boxHalfExtents) :
            base(boxHalfExtents)
        {
        }

        public BoxShapeEx(float boxHalfExtent) :
            base(boxHalfExtent)
        {
        }

        public BoxShapeEx(float boxHalfExtentX, float boxHalfExtentY, float boxHalfExtentZ) :
            base(boxHalfExtentX,  boxHalfExtentY, boxHalfExtentZ)
        {
        }
    }

    public class SphereShapeEx : SphereShape
    {
        public Vector3 Center { get; set; }

        public SphereShapeEx(float radius) :
            base(radius)
        {
        }
    }

    public class ConvexHullShapeEx: ConvexHullShape
    {
        public Vector3 Center { get; set; }

        public ConvexHullShapeEx(float[] points) :
            base(points)
        {
        }
    }

    public class HeightfieldTerrainShapeEx: HeightfieldTerrainShape
    {
        public GameWorld.RigidBody RigidBody { get; private set; }

        public SharpDX.Vector3 Center { get; private set; }

        public int HeightStickX { get; private set; } 
            
        public int HeightStickZ { get; private set; }

        public float MinHeight { get; private set; }

        public float MaxHeight { get; private set; }

        public float GridSpacing { get; set; }

        public float[] HeightfieldData { get; private set; }

        public HeightfieldTerrainShapeEx(GameWorld.RigidBody rigidBody) : 
            base(0, 0, IntPtr.Zero, 0, 0, 0, 0, PhyScalarType.Single, false)
        {
            RigidBody = rigidBody;

            var max = rigidBody.BoundingBox.Maximum;
            var min = rigidBody.BoundingBox.Minimum;

            var center = (max + min) / 2.0f;

            var point = center;
            point.Z *= -1.0f;
            Center = point;

            MinHeight = min.Y;
            MaxHeight = max.Y;

            PrepareHeightfieldData();
        }

        private void PrepareHeightfieldData()
        {
            var mesh = RigidBody.Mesh;

            var ray = new SharpDX.Ray
            {
                Position = new SharpDX.Vector3(0.0f, 1000.0f, 0.0f),
                Direction = new SharpDX.Vector3(0.0f, -1.0f, 0.0f)
            };

            var max = RigidBody.BoundingBox.Maximum;
            var min = RigidBody.BoundingBox.Minimum;

            const float Eplison = 0.001f;
            var dx = Math.Abs(max.X - min.X) - 2 * Eplison;
            var dz = Math.Abs(max.Z - min.Z) - 2 * Eplison;

            HeightStickX = (int)Math.Ceiling(dx / Math.Max(RigidBody.GridSpacing, Eplison));
            HeightStickZ = (int)Math.Ceiling(dz / Math.Max(RigidBody.GridSpacing, Eplison));

            var gridSpacingX = dx / HeightStickX;
            var gridSpacingZ = dz / HeightStickZ;

            var heightfieldData = new List<float>();

            for (int i = -HeightStickX / 2; i < HeightStickX / 2; i++)
            {
                for (int j = -HeightStickZ / 2; j < HeightStickZ / 2; j++)
                {
                    ray.Position.X = i * gridSpacingX + Eplison;
                    ray.Position.Z = j * gridSpacingZ + Eplison;

                    RigidBody.GameObject.Intersects(ray, out RayPickData data);

                    if (data != null)
                    {
                        heightfieldData.Add(data.Point.Y - RigidBody.GameObject.Position.Y);
                    }
                    else
                    {
                        heightfieldData.Add(-1000.0f);
                    }
                }
            }

            HeightfieldData = heightfieldData.ToArray();
        }
    }

    public class CollisionWriter : IDisposable
    {
        private readonly WorldMapData _worldMapData;
        private readonly PackerOptions _options;

        private DefaultCollisionConfiguration _collisionConfiguration;
        private CollisionDispatcher _dispatcher;
        private BroadphaseInterface _broadphaseInterface;
        private SequentialImpulseConstraintSolver _solver;
        private DiscreteDynamicsWorld _dynamicsWorld;

        private const ushort ID_COLLISION_BLOCK_CHUNK = 0x0050;
        private const ushort ID_COLLISION_SHAPE_BLOCK_CHUNK = 0x0060;
        private const ushort ID_COLLISION_RIGID_BLOCK_CHUNK = 0x0070;

        private int _collisionId;
        private List<int> _collisionIds = new List<int>();

        public CollisionWriter(WorldMapData worldMapData, PackerOptions options)
        {
            _worldMapData = worldMapData;
            _options = options;

            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _broadphaseInterface = new DbvtBroadphase();
            _solver = new SequentialImpulseConstraintSolver();
            _dynamicsWorld = new DiscreteDynamicsWorld(_dispatcher, _broadphaseInterface, _solver, _collisionConfiguration);
        }

        public void WriteData(Stream stream)
        {
            UpdateDynamicsWorld();

            stream.Write(BitConverter.GetBytes(ID_COLLISION_BLOCK_CHUNK), 0, sizeof(ushort));

            long tmpPos = stream.Position;
            stream.Write(BitConverter.GetBytes(0L), 0, sizeof(int));//Collision Chunk size
            stream.Write(BitConverter.GetBytes(0L), 0, sizeof(int));//Number of Collision Shapes
            stream.Write(BitConverter.GetBytes(0L), 0, sizeof(int));//Number of Rigid Bodies

            WriteCollisionShapeData(stream);

            WriteVehicleCollisionShapeData(stream);

            WriteRigidBodyData(stream);

            long pos = stream.Position;
            stream.Position = tmpPos;

            long blockSize = pos - tmpPos;
            stream.Write(BitConverter.GetBytes((int)blockSize), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(_collisionIds.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(_dynamicsWorld.NumCollisionObjects), 0, sizeof(int));
            stream.Position = pos;
        }

        private void WriteVehicleCollisionShapeData(Stream stream)
        {
            var shapes = new Dictionary<string, CollisionShape>();
            foreach (var gameObject in _worldMapData.GameObjects)
            {
                var vehicle = gameObject.Key as Vehicle;

                if (vehicle == null) continue;

                if (!shapes.ContainsKey(vehicle.ChassisMeshId))
                {
                    var convexHullMeshId = vehicle.GetChassisConvexHullMesh();
                    var mesh = ResourceManager.Instance.GetMesh(convexHullMeshId);

                    var subMeshIds = new List<uint>();
                    var wheelSubMeshIds = new List<string>();

                    foreach (var wheel in vehicle.Wheels)
                    {
                        wheelSubMeshIds.AddRange(wheel.SubMeshIds);
                    }

                    foreach (var submesh in mesh.SubMeshes)
                    {
                        if (!wheelSubMeshIds.Contains(submesh.Id.ToString()))
                        {
                            subMeshIds.Add(submesh.Id);
                        }
                    }

                    var rigidBody = new GameWorld.RigidBody
                    {
                        ShapeType = CollisionShapeType.ConvexHull,
                        Mesh = mesh,
                        GameObject = vehicle,
                        SubMeshIds = subMeshIds,
                        Mass = vehicle.Mass
                    };

                    shapes[vehicle.ChassisMeshId] = GetCollisionShape(rigidBody);
                    _collisionIds.Add(shapes[vehicle.ChassisMeshId].UserIndex);

                    WriteCollisionShapeBinaryData(stream, shapes[vehicle.ChassisMeshId]);
                }

                vehicle.CollisionShapeId = shapes[vehicle.ChassisMeshId].UserIndex;
            }
        }

        private void WriteRigidBodyData(Stream stream)
        {
            for (int i = 0; i < _dynamicsWorld.NumCollisionObjects; i++)
            {
                stream.Write(BitConverter.GetBytes(ID_COLLISION_RIGID_BLOCK_CHUNK), 0, sizeof(ushort));

                var rigidBody = (BulletSharp.RigidBody)_dynamicsWorld.CollisionObjectArray[i];

                stream.Write(BitConverter.GetBytes(rigidBody.UserIndex), 0, sizeof(int));                      //Game Object id
                stream.Write(BitConverter.GetBytes(rigidBody.CollisionShape.UserIndex), 0, sizeof(int));       //Shape index
                stream.Write(BitConverter.GetBytes(rigidBody.InvMass), 0, sizeof(float));                      //Inv mass
            }
        }

        private void WriteCollisionShapeBinaryData(Stream stream, CollisionShape shape)
        {
            stream.Write(BitConverter.GetBytes(ID_COLLISION_SHAPE_BLOCK_CHUNK), 0, sizeof(ushort));  //CHUNK ID
            stream.Write(BitConverter.GetBytes(shape.UserIndex), 0, sizeof(int));       //User index
            stream.Write(BitConverter.GetBytes((int)shape.ShapeType), 0, sizeof(int));  //Shape type

            switch (shape.ShapeType)
            {
                case BroadphaseNativeType.ConvexHullShape:
                    WriteConvexHullShapeData(shape, stream);
                    break;
                case BroadphaseNativeType.BoxShape:
                    WriteBoxShapeData(shape, stream);
                    break;
                case BroadphaseNativeType.TerrainShape:
                    WriteHeightfieldTerrainShapeData(shape, stream);
                    break;
            }
        }

        private void WriteCollisionShapeData(Stream stream)
        {
            for (int i = 0; i < _dynamicsWorld.NumCollisionObjects; i++)
            {
                if (!(_dynamicsWorld.CollisionObjectArray[i] is BulletSharp.RigidBody obj) || _collisionIds.IndexOf(obj.CollisionShape.UserIndex) >= 0)
                {
                    continue;
                }

                _collisionIds.Add(obj.CollisionShape.UserIndex);

                WriteCollisionShapeBinaryData(stream, obj.CollisionShape);
            }
        }

        private void WriteBoxShapeData(CollisionShape shape, Stream stream)
        {
            if (shape is BoxShapeEx boxShape)
            {
                stream.Write(BitConverter.GetBytes(boxShape.HalfExtentsWithMargin.X), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(boxShape.HalfExtentsWithMargin.Y), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(boxShape.HalfExtentsWithMargin.Z), 0, sizeof(float));

                stream.Write(BitConverter.GetBytes(boxShape.Center.X), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(boxShape.Center.Y), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(boxShape.Center.Z), 0, sizeof(float));
            }
        }

        private void WriteConvexHullShapeData(CollisionShape shape, Stream stream)
        {
            if (shape is ConvexHullShapeEx convexHullShape)
            {
                //Write Center
                stream.Write(BitConverter.GetBytes(convexHullShape.Center.X), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(convexHullShape.Center.Y), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(convexHullShape.Center.Z), 0, sizeof(float));

                //Number of Vertices
                stream.Write(BitConverter.GetBytes(convexHullShape.NumVertices), 0, sizeof(int));

                //Alignment
                int alignment = _options.PlatformAlignment;
                int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
                stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));                        

                stream.Position += offset;

                foreach (var point in convexHullShape.UnscaledPoints)
                {
                    stream.Write(BitConverter.GetBytes(point.X), 0, sizeof(float));
                    stream.Write(BitConverter.GetBytes(point.Y), 0, sizeof(float));
                    stream.Write(BitConverter.GetBytes(point.Z), 0, sizeof(float));
                    stream.Write(BitConverter.GetBytes(0L), 0, sizeof(float));
                }
            }
        }

        private void WriteHeightfieldTerrainShapeData(CollisionShape shape, Stream stream)
        {
            if (shape is HeightfieldTerrainShapeEx heightfieldTerrainShape)
            {
                //Write Center
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.Center.X), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.Center.Y), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.Center.Z), 0, sizeof(float));

                //HeightStick Wwdth
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.HeightStickX * 2), 0, sizeof(int));

                //HeightStick length
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.HeightStickZ * 2), 0, sizeof(int));

                //Grid spacing
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.GridSpacing), 0, sizeof(float));

                //HeightfieldData length
                stream.Write(BitConverter.GetBytes(heightfieldTerrainShape.HeightfieldData.Length), 0, sizeof(int));

                //Alignment
                int alignment = _options.PlatformAlignment;
                int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
                stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

                stream.Position += offset;

                foreach (var point in heightfieldTerrainShape.HeightfieldData)
                {
                    stream.Write(BitConverter.GetBytes(point), 0, sizeof(float));
                }
            }
        }

        private void UpdateDynamicsWorld()
        {
            _collisionIds.Clear();
            _collisionId = 1;
            var groups = new Dictionary<GameObject, List<GameWorld.RigidBody>>();
            var collisionShapesCache = new Dictionary<Tuple<string, uint?>, CollisionShape>();

            foreach (var rigidBody in _worldMapData.RigidBodies)
            {
                if (!groups.ContainsKey(rigidBody.GameObject))
                {
                    groups[rigidBody.GameObject] = new List<GameWorld.RigidBody>();
                }

                var items = groups[rigidBody.GameObject];
                items.Add(rigidBody);
            }

            foreach (var group in groups)
            {
                var matrix = SharpDX.Matrix.Scaling(group.Key.Scale) *
                             SharpDX.Matrix.RotationQuaternion(group.Key.RotationRH) *
                             SharpDX.Matrix.Translation(group.Key.Position);

                if (group.Value.Count > 1)
                {
                    var compoundShape = new CompoundShape();
                    var rigidBodies = group.Value;
                    var mass = 0.0f;

                    foreach (var rigidBody in rigidBodies)
                    {
                        var key = new Tuple<string, uint?>(rigidBody.Mesh.Id, rigidBody.SubMeshId);
                        CollisionShape shape;

                        if (!collisionShapesCache.ContainsKey(key))
                        {
                            collisionShapesCache[key] = GetCollisionShape(rigidBody);
                        }

                        shape = collisionShapesCache[key];

                        var subMeshScale = rigidBody.Mesh.GetSubMesh(rigidBody.SubMeshId.Value).Scale;
                        var subMeshRotation = rigidBody.Mesh.GetSubMesh(rigidBody.SubMeshId.Value).RotationRH;
                        var subMeshPosition = rigidBody.Mesh.GetSubMesh(rigidBody.SubMeshId.Value).Position;
                        var subMeshMatrix = SharpDX.Matrix.Scaling(subMeshScale) *
                                            SharpDX.Matrix.RotationQuaternion(subMeshRotation) *
                                            SharpDX.Matrix.Translation(subMeshPosition);

                        mass += rigidBody.Mass;
                        compoundShape.AddChildShape(new Matrix(subMeshMatrix.ToArray()), shape);
                    }

                    var motionState = new DefaultMotionState(new Matrix(matrix.ToArray()));
                    var constructionInfo = new RigidBodyConstructionInfo(mass, motionState, compoundShape);
                    var newRigidBody = new BulletSharp.RigidBody(constructionInfo) { UserIndex = _worldMapData.GameObjects[group.Key] };
                    _dynamicsWorld.AddRigidBody(newRigidBody);
                }
                else
                {
                    var rigidBody = group.Value.FirstOrDefault();
                    var motionState = new DefaultMotionState(new Matrix(matrix.ToArray()));

                    var key = new Tuple<string, uint?>(rigidBody.Mesh.Id, null);
                    CollisionShape shape;

                    if (!collisionShapesCache.ContainsKey(key))
                    {
                        collisionShapesCache[key] = GetCollisionShape(rigidBody);
                    }

                    shape = collisionShapesCache[key];
                    var constructionInfo = new RigidBodyConstructionInfo(rigidBody.Mass, motionState, shape);
                    var newRigidBody = new BulletSharp.RigidBody(constructionInfo) { UserIndex = _worldMapData.GameObjects[group.Key] };
                    _dynamicsWorld.AddRigidBody(newRigidBody);
                }
            }
        }

        private CollisionShape GetCollisionShape(GameWorld.RigidBody rigidBody)
        {
            CollisionShape shape = null;

            var max = rigidBody.BoundingBox.Maximum;
            var min = rigidBody.BoundingBox.Minimum;
            var center = (max + min) / 2.0f;

            switch (rigidBody.ShapeType)
            {
                case CollisionShapeType.Box:
                    var boxHalfExtends = max - center;
                    boxHalfExtends.X = Math.Abs(boxHalfExtends.X);
                    boxHalfExtends.Y = Math.Abs(boxHalfExtends.Y);
                    boxHalfExtends.Z = Math.Abs(boxHalfExtends.Z);
                    center.Z *= -1.0f;
                    shape = new BoxShapeEx(new Vector3(boxHalfExtends.ToArray())) { Center = new Vector3(center.ToArray()) };
                    break;
                case CollisionShapeType.Sphere:
                    var shpereCenter = rigidBody.BoundingSphere.Center;
                    shpereCenter.Z *= -1.0f;
                    shape = new SphereShapeEx(rigidBody.BoundingSphere.Radius) { Center = new Vector3(shpereCenter.ToArray()) };
                    break;
                case CollisionShapeType.ConvexHull:
                    var vertices = rigidBody.GetMeshVertices();
                    var convexHullShape = new ConvexHullShapeEx(vertices) { Center = new Vector3(center.ToArray()) };
                    shape = convexHullShape;
                    break;
                case CollisionShapeType.Heightfield:
                    var heightFieldShape = new HeightfieldTerrainShapeEx(rigidBody)
                    {
                        GridSpacing = rigidBody.GridSpacing
                    };
                    shape = heightFieldShape;

                    break;
            }

            shape.CalculateLocalInertia(rigidBody.Mass);
            shape.UserIndex = _collisionId++;

            return shape;
        }

        public void Dispose()
        {
            if (_dynamicsWorld != null)
            {
                _dynamicsWorld.Dispose();
            }
            if (_solver != null)
            {
                _solver.Dispose();
            }
            if (_broadphaseInterface != null)
            {
                _broadphaseInterface.Dispose();
            }
            if (_dispatcher != null)
            {
                _dispatcher.Dispose();
            }
            if (_collisionConfiguration != null)
            {
                _collisionConfiguration.Dispose();
            }
        }
    }
}
