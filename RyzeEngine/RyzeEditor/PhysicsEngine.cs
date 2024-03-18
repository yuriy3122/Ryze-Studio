using System;
using System.Collections.Generic;
using BulletSharp;
using RyzeEditor.GameWorld;
using RyzeEditor.Helpers;

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

            _discreteDynamicsWorld.StepSimulation(deltaTime, 10, 1.0f / 100.0f);

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

            int i = 0;

            foreach (var entity in entities)
            {
                var rigidBody = entity as GameWorld.RigidBody;

                if (rigidBody != null)
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

                    switch (rigidBody.ShapeType)
                    {
                        case CollisionShapeType.Box:
                            var collisionShape = new CompoundShape();

                            var max = rigidBody.BoundingBox.Maximum;
                            var min = rigidBody.BoundingBox.Minimum;
                            var center = (max + min) / 2.0f;

                            var boxHalfExtends = max - center;
                            boxHalfExtends.X = Math.Abs(boxHalfExtends.X);
                            boxHalfExtends.Y = Math.Abs(boxHalfExtends.Y);
                            boxHalfExtends.Z = Math.Abs(boxHalfExtends.Z);

                            var boxShape = new BoxShape(boxHalfExtends.X, boxHalfExtends.Y, boxHalfExtends.Z);
                            var localInertia = boxShape.CalculateLocalInertia(rigidBody.Mass);

                            var id = rigidBody.SubMeshId ?? 0;

                            var subMeshPosition = rigidBody.Mesh.GetSubMesh(id).Position + center;
                            subMeshPosition.Z *= -1.0f;

                            var subMeshScale = rigidBody.Mesh.GetSubMesh(id).Scale;
                            var subMeshRotation = rigidBody.Mesh.GetSubMesh(id).RotationRH;
                            var subMeshMatrix = SharpDX.Matrix.Scaling(subMeshScale) *
                                                SharpDX.Matrix.RotationQuaternion(subMeshRotation) *
                                                SharpDX.Matrix.Translation(subMeshPosition);

                            collisionShape.AddChildShape(MatrixHelper.ConvertMatrix(subMeshMatrix), boxShape);

                            var motionState = new DefaultMotionState(MatrixHelper.ConvertMatrix(startTransform));
                            var rbInfo = new RigidBodyConstructionInfo(rigidBody.Mass, motionState, collisionShape, localInertia);
                            var newRigidBody = new BulletSharp.RigidBody(rbInfo) { UserIndex = i++ };
                            _discreteDynamicsWorld.AddRigidBody(newRigidBody);

                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public void Dispose()
        {
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
