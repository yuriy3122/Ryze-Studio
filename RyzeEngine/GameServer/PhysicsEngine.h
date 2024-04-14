#include "GameObject.h"
#include "CollisionShape.h"
#include "RigidBody.h"
#include "ResourceManager.h"
#include "btBulletDynamicsCommon.h"
#include <map>

class PhysicsEngine
{
public:
	PhysicsEngine(ResourceManager* resourceManager);

	void Initialize();

	void StepSimulation(float deltaTime);

	~PhysicsEngine();

private:
	btCollisionShape* CreateBoxShape(collision_shape_t* shape);
	btRigidBody* CreateRigidBody(rigid_body_t* rigidBody, btCollisionShape* collisionShape);

	ResourceManager* m_resourceManager;

	btDefaultCollisionConfiguration* m_collisionConfiguration;
	btCollisionDispatcher* m_dispatcher;
	btBroadphaseInterface* m_overlappingPairCache;
	btSequentialImpulseConstraintSolver* m_solver;
	btDiscreteDynamicsWorld* m_dynamicsWorld;

	std::map<int, btCollisionShape*> m_collisionShapes;
};
