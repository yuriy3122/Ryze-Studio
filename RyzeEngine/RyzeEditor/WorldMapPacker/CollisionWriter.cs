using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BulletSharp;
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

        public ConvexHullShapeEx(IEnumerable<Vector3> points) :
            base(points)
        {
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
                var rotation = group.Key.Rotation;
                rotation.X = -rotation.X;
                rotation.Y = -rotation.Y;

                var matrix = SharpDX.Matrix.Scaling(group.Key.Scale) *
                             SharpDX.Matrix.RotationQuaternion(rotation) *
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

                        var arr = subMeshMatrix.ToArray();



                        compoundShape.AddChildShape(ConvertMatrix(subMeshMatrix), shape);
                    }

                    var motionState = new DefaultMotionState(ConvertMatrix(matrix));
                    var constructionInfo = new RigidBodyConstructionInfo(mass, motionState, compoundShape);
                    var newRigidBody = new BulletSharp.RigidBody(constructionInfo) { UserIndex = _worldMapData.GameObjects[group.Key] };
                    _dynamicsWorld.AddRigidBody(newRigidBody);
                }
                else
                {
                    var rigidBody = group.Value.FirstOrDefault();
                    var motionState = new DefaultMotionState(ConvertMatrix(matrix));

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
                    shape = new BoxShapeEx(new Vector3(boxHalfExtends.X, boxHalfExtends.Y, boxHalfExtends.Z)) 
                        { Center = new Vector3(center.X, center.Y, center.Z) };
                    break;
                case CollisionShapeType.Sphere:
                    var shpereCenter = rigidBody.BoundingSphere.Center;
                    shpereCenter.Z *= -1.0f;
                    shape = new SphereShapeEx(rigidBody.BoundingSphere.Radius) 
                        { Center = new Vector3(shpereCenter.X, shpereCenter.Y, shpereCenter.Z) };
                    break;
                case CollisionShapeType.ConvexHull:
                    var meshVertices = rigidBody.GetMeshVertices();
                    var primitiveCount = meshVertices.Length / 3;
                    var vertices = new List<Vector3>();

                    for (int i = 0; i < primitiveCount; i++)
                    {
                        vertices.Add(new Vector3(meshVertices[i], meshVertices[i + 1], meshVertices[i + 2]));
                    }

                    var convexHullShape = new ConvexHullShapeEx(vertices) { Center = new Vector3(center.X, center.Y, center.Z) };
                    shape = convexHullShape;
                    break;
            }

            shape.CalculateLocalInertia(rigidBody.Mass);
            shape.UserIndex = _collisionId++;

            return shape;
        }

        private Matrix ConvertMatrix(SharpDX.Matrix matrix)
        {
            var output = new Matrix
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,

                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,

                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,

                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44
            };

            return output;
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
