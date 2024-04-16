#include "GameObject.h"
#include "CollisionShape.h"
#include "RigidBody.h"
#include "ResourceManager.h"
#include "btBulletDynamicsCommon.h"
#include <map>

using namespace std;

class PhysicsEngine
{
public:
	PhysicsEngine(ResourceManager* resourceManager);

	void Initialize();

	void StepSimulation(float deltaTime);

	~PhysicsEngine();

private:
	btCollisionShape* CreateBoxShape(collision_shape_t* shape);
	btCollisionShape* CreateConvexHullShape(collision_shape_t* shape);

	btRigidBody* CreateRigidBody(rigid_body_t* rigidBody, btCollisionShape* collisionShape);

	void InitializeRigidBodies();
	void InitializeVehicles();

	btCollisionShape* GetBulletCollisionShape(collision_shape_t* shape);

	ResourceManager* m_resourceManager;

	btDefaultCollisionConfiguration* m_collisionConfiguration;
	btCollisionDispatcher* m_dispatcher;
	btBroadphaseInterface* m_overlappingPairCache;
	btSequentialImpulseConstraintSolver* m_solver;
	btDiscreteDynamicsWorld* m_dynamicsWorld;

	map<int, btCollisionShape*> m_collisionShapes;
	map<int, btRaycastVehicle*> m_vehicles;
};
