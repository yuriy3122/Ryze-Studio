using BulletSharp;
using RyzeEditor.GameWorld;
using System;

namespace RyzeEditor
{
    public class PhysicsEngine
    {
        private WorldMap _worldMap;
        private DefaultCollisionConfiguration _collisionConfiguration;
        private CollisionDispatcher _dispatcher;
        private BroadphaseInterface _overlappingPairCache;
        private SequentialImpulseConstraintSolver _solver;
        private DiscreteDynamicsWorld _discreteDynamicsWorld;

        public PhysicsEngine(WorldMap worldMap)
        {
            _worldMap = worldMap;
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
        }

        public void StepSimulation(float deltaTime)
        {
            //TODO: verify list (cache) of Rigid Bodies from WorldMap

            _discreteDynamicsWorld.StepSimulation(deltaTime, 10, 1.0f / 100.0f);
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
