using System;
using System.Collections.Generic;
using BulletSharp;
using RyzeEditor.GameWorld;
using RyzeEditor.Helpers;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor
{
    public class PhysicsEngine
    {
        private readonly DefaultCollisionConfiguration _collisionConfiguration;
        private readonly CollisionDispatcher _dispatcher;
        private readonly BroadphaseInterface _overlappingPairCache;
        private readonly SequentialImpulseConstraintSolver _solver;
        private readonly DiscreteDynamicsWorld _discreteDynamicsWorld;
        private readonly Dictionary<int, GameObject> _gameObjectMap;
        private readonly Dictionary<string, CollisionShape> _collisionShapeMap;
        private readonly Dictionary<Vehicle, RaycastVehicle> _raycastVehicles;

        private bool _needUpdate;

        public PhysicsEngine()
        {
            _collisionConfiguration = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfiguration);
            _solver = new SequentialImpulseConstraintSolver();

            Vector3 worldMin = new Vector3(-1000.0f, -1000.0f, -1000.0f);
            Vector3 worldMax = new Vector3(1000.0f, 1000.0f, 1000.0f);
            _overlappingPairCache = new AxisSweep3(worldMin, worldMax);

            _discreteDynamicsWorld = new DiscreteDynamicsWorld(_dispatcher, _overlappingPairCache, _solver, _collisionConfiguration);

            _discreteDynamicsWorld.SolverInfo.MinimumSolverBatchSize = 128;
            _discreteDynamicsWorld.SolverInfo.GlobalCfm = 0.00001f;
            _discreteDynamicsWorld.Gravity = new Vector3(0.0f, -9.8f, 0.0f);

            _gameObjectMap = new Dictionary<int, GameObject>();
            _collisionShapeMap = new Dictionary<string, CollisionShape>();
            _raycastVehicles = new Dictionary<Vehicle, RaycastVehicle>();

            _needUpdate = true;
        }

        public void Update()
        {
            _needUpdate = true;
        }

        public void StepSimulation(IEnumerable<EntityBase> entities, float deltaTime)
        {
            if (_needUpdate)
            {
                CleanUpDynamicsWorldData();
                InitDynamicsWorldData(entities);
                _needUpdate = false;
            }

            _discreteDynamicsWorld.StepSimulation(deltaTime, 10, 1.0f / 60.0f);

            for (int j = _discreteDynamicsWorld.NumCollisionObjects - 1; j >= 0; j--)
            {
                var obj = _discreteDynamicsWorld.CollisionObjectArray[j];

                if (!(obj is BulletSharp.RigidBody body)) continue;

                var motionState = body.MotionState;

                var matrix = MatrixHelper.ExtractMatrix(motionState.WorldTransform);
                matrix.Decompose(out SharpDX.Vector3 scale, out SharpDX.Quaternion rotation, out SharpDX.Vector3 position);

                rotation.X = -rotation.X;
                rotation.Y = -rotation.Y;
                position.Z *= -1.0f;

                var gameObject = _gameObjectMap[body.UserIndex];
                gameObject.Rotation = rotation;
                gameObject.Position = position;
                gameObject.Scale = scale;
            }

            //Sync Vehicles (Game objects) params (Steering, BreakingForce, EngineForce) with Bullet Vehicles

            //Get Bullet Vehicle chassic params to update game objects, wheel submeshes
        }

        private void CleanUpDynamicsWorldData()
        {
            var objToDelete = new List<CollisionObject>();

            for (int i = 0; i < _discreteDynamicsWorld.NumCollisionObjects; i++)
            {
                var obj = _discreteDynamicsWorld.CollisionObjectArray[i];

                if (obj is BulletSharp.RigidBody body && body.MotionState != null)
                {
                    body.MotionState = null;
                }

                objToDelete.Add(obj);
            }

            foreach (var obj in objToDelete)
            {
                _discreteDynamicsWorld.RemoveCollisionObject(obj);
            }
        }

        private void InitDynamicsWorldData(IEnumerable<EntityBase> entities)
        {
            if (_gameObjectMap != null)
            {
                _gameObjectMap.Clear();
            }

            if (_collisionShapeMap != null)
            {
                _collisionShapeMap.Clear();
            }

            int i = 0;

            foreach (var entity in entities)
            {
                var rigidBody = entity as GameWorld.RigidBody;

                if (rigidBody != null)
                {
                    AddRigidBody(rigidBody, ref i);
                }

                var vehicle = entity as Vehicle;

                if (vehicle != null)
                {
                    //AddRaycastVehicle(vehicle, ref i);
                }
            }
        }

        private void AddRaycastVehicle(Vehicle vehicle, ref int i)
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

            _gameObjectMap[i] = rigidBody.GameObject;

            var chassisShape = new CompoundShape();
            var convexShape = CreateConvexHullRigidBody(rigidBody);

            var max = vehicle.BoundingBox.Maximum;
            var min = vehicle.BoundingBox.Minimum;
            var center = (max + min) / 2.0f;
            var localTransform = SharpDX.Matrix.Translation(center);

            chassisShape.AddChildShape(MatrixHelper.ConvertMatrix(localTransform), convexShape);

            var chassisLocalInertia = chassisShape.CalculateLocalInertia(rigidBody.Mass);
            var chassisStartTransform = Matrix.Identity;
            var chassisMotionState = new DefaultMotionState(chassisStartTransform);
            var chassisRbInfo = new RigidBodyConstructionInfo(rigidBody.Mass, chassisMotionState, chassisShape, chassisLocalInertia);
            var chassis = new BulletSharp.RigidBody(chassisRbInfo);

            var rotation = vehicle.Rotation;
            rotation.X = -rotation.X;
            rotation.Y = -rotation.Y;

            var position = vehicle.Position;
            position.Z *= -1.0f;

            var startTransform = SharpDX.Matrix.RotationQuaternion(rotation) * SharpDX.Matrix.Translation(position);
            chassis.CenterOfMassTransform = MatrixHelper.ConvertMatrix(startTransform);
            chassis.UserIndex = i++;
            chassis.AngularVelocity = Vector3.Zero;
            chassis.LinearVelocity = Vector3.Zero;
            chassis.ActivationState = ActivationState.DisableDeactivation;

            _discreteDynamicsWorld.AddRigidBody(chassis);

            var vehicleRayCaster = new DefaultVehicleRaycaster(_discreteDynamicsWorld);
            var tuning = new RaycastVehicle.VehicleTuning();
            var raycastVehicle = new RaycastVehicle(tuning, chassis, vehicleRayCaster);

            _discreteDynamicsWorld.AddAction(raycastVehicle);

            for (int j = 0; j < vehicle.Wheels.Count; j++)
            {
                SetupWheel(vehicle.Wheels[j], raycastVehicle);
            }

            const float wheelFriction = 1000.0f;
            const float rollInfluence = 0.1f;

            for (int j = 0; j < raycastVehicle.NumWheels; j++)
            {
                WheelInfo wheelInfo = raycastVehicle.GetWheelInfo(j);
                wheelInfo.SuspensionStiffness = vehicle.Wheels[j].SuspensionStiffness;
                wheelInfo.WheelsDampingRelaxation = vehicle.Wheels[j].SuspensionDamping;
                wheelInfo.WheelsDampingCompression = vehicle.Wheels[j].SuspensionCompression;
                wheelInfo.FrictionSlip = wheelFriction;
                wheelInfo.RollInfluence = rollInfluence;
            }

            _discreteDynamicsWorld.Broadphase.OverlappingPairCache.CleanProxyFromPairs(chassis.BroadphaseHandle, _discreteDynamicsWorld.Dispatcher);

            raycastVehicle.ResetSuspension();

            for (int j = 0; j < raycastVehicle.NumWheels; j++)
            {
                raycastVehicle.UpdateWheelTransform(j, true);
            }
        }

        private void SetupWheel(Wheel wheel, RaycastVehicle raycastVehicle)
        {
            wheel.ComputeParams();

            var connectionPointCS0 = new Vector3(wheel.ChassisConnectionPointCS.X, wheel.ChassisConnectionPointCS.Y, wheel.ChassisConnectionPointCS.Z);
            var wheelDirectionCS0 = new Vector3(wheel.WheelDirectionCS.X, wheel.WheelDirectionCS.Y, wheel.WheelDirectionCS.Z);
            var wheelAxleCS0 = new Vector3(wheel.AxleCS.X, wheel.AxleCS.Y, wheel.AxleCS.Z);

            raycastVehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS0, wheel.SuspensionRestLength,
                wheel.Radius, new RaycastVehicle.VehicleTuning(), wheel.IsFrontWheel);
        }

        private void AddRigidBody(GameWorld.RigidBody rigidBody, ref int i)
        {
            _gameObjectMap[i] = rigidBody.GameObject;

            var rotation = rigidBody.GameObject.Rotation;
            rotation.X = -rotation.X;
            rotation.Y = -rotation.Y;

            var position = rigidBody.GameObject.Position;
            position.Z *= -1.0f;

            var startTransform = SharpDX.Matrix.Scaling(rigidBody.GameObject.Scale) *
                                 SharpDX.Matrix.RotationQuaternion(rotation) *
                                 SharpDX.Matrix.Translation(position);

            CollisionShape collisionShape = null;

            switch (rigidBody.ShapeType)
            {
                case CollisionShapeType.Box:
                    collisionShape = CreateBoxCollisionShape(rigidBody);
                    break;
                case CollisionShapeType.ConvexHull:
                    collisionShape = CreateConvexHullRigidBody(rigidBody);
                    break;
            }

            var localInertia = collisionShape.CalculateLocalInertia(rigidBody.Mass);
            var motionState = new DefaultMotionState(MatrixHelper.ConvertMatrix(startTransform));
            var rbInfo = new RigidBodyConstructionInfo(rigidBody.Mass, motionState, collisionShape, localInertia);
            var newRigidBody = new BulletSharp.RigidBody(rbInfo) { UserIndex = i++ };

            _discreteDynamicsWorld.AddRigidBody(newRigidBody);
        }

        private ConvexHullShape CreateConvexHullRigidBody(GameWorld.RigidBody rigidBody)
        {
            ConvexHullShape collisionShape = null;

            if (_collisionShapeMap.ContainsKey(rigidBody.Mesh.Id))
            {
                collisionShape = _collisionShapeMap[rigidBody.Mesh.Id] as ConvexHullShape;
            }

            if (collisionShape == null)
            {
                var meshVertices = rigidBody.GetMeshVertices();
                var primitiveCount = meshVertices.Length / 3;
                var vertices = new List<Vector3>();

                for (int j = 0; j < primitiveCount; j++)
                {
                    vertices.Add(new Vector3(meshVertices[j], meshVertices[j + 1], meshVertices[j + 2]));
                }

                collisionShape = new ConvexHullShape(vertices);

                _collisionShapeMap[rigidBody.Mesh.Id] = collisionShape;
            }

            return collisionShape;
        }

        private CompoundShape CreateBoxCollisionShape(GameWorld.RigidBody rigidBody)
        {
            var collisionShape = new CompoundShape();

            var max = rigidBody.BoundingBox.Maximum;
            var min = rigidBody.BoundingBox.Minimum;
            var center = (max + min) / 2.0f;

            var boxHalfExtends = max - center;
            boxHalfExtends.X = Math.Abs(boxHalfExtends.X);
            boxHalfExtends.Y = Math.Abs(boxHalfExtends.Y);
            boxHalfExtends.Z = Math.Abs(boxHalfExtends.Z);

            var boxShape = new BoxShape(boxHalfExtends.X, boxHalfExtends.Y, boxHalfExtends.Z);
            var localTransform = SharpDX.Matrix.Translation(center);

            collisionShape.AddChildShape(MatrixHelper.ConvertMatrix(localTransform), boxShape);

            return collisionShape;
        }

        public void Dispose()
        {
            CleanUpDynamicsWorldData();

            if (_discreteDynamicsWorld != null && !_discreteDynamicsWorld.IsDisposed)
            {
                _discreteDynamicsWorld.Dispose();
            }

            if (_solver != null && !_solver.IsDisposed)
            {
                _discreteDynamicsWorld.Dispose();
            }

            if (_overlappingPairCache != null && !_overlappingPairCache.IsDisposed)
            {
                _overlappingPairCache.Dispose();
            }

            if (_dispatcher != null && !_dispatcher.IsDisposed)
            {
                _dispatcher.Dispose();
            }

            if (_collisionConfiguration != null && !_collisionConfiguration.IsDisposed)
            {
                _collisionConfiguration.Dispose();
            }
        }
    }
}
