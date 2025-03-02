#include "GameObject.h"
#include "CollisionShape.h"
#include "RigidBody.h"
#include "SubMeshTransform.h"
#include "ResourceManager.h"
#include "btBulletDynamicsCommon.h"
#include <BulletCollision/CollisionShapes/btHeightfieldTerrainShape.h>
#include <BulletSoftBody/btSoftRigidDynamicsWorld.h>
#include <BulletSoftBody/btSoftBodyRigidBodyCollisionConfiguration.h>
#include <map>
#include <vector>

using namespace std;

class PhysicsEngine
{
public:
	PhysicsEngine(ResourceManager* resourceManager);

	void Initialize();

	void StepSimulation(float deltaTime);

	const SubMeshTransformList& GetSubMeshTransforms(int objectId);

	~PhysicsEngine();

private:
	btCollisionShape* CreateBoxShape(const collision_shape_t* shape);
	btCollisionShape* CreateConvexHullShape(const collision_shape_t* shape);
	btCollisionShape* CreateHeightfieldTerrainShape(const collision_shape_t* shape);

	btRigidBody* CreateRigidBody(const rigid_body_t* rigidBody, btCollisionShape* collisionShape);

	void InitializeRigidBodies();
	void InitializeVehicles();

	btCollisionShape* GetBulletCollisionShape(collision_shape_t* shape);
	void SetWheelTransform(submesh_transform_t* transform, const btRaycastVehicle* vehicle, const wheel_t* wheel, int index);

	ResourceManager* m_resourceManager;

	btDefaultCollisionConfiguration* m_collisionConfiguration;
	btCollisionDispatcher* m_dispatcher;
	btBroadphaseInterface* m_overlappingPairCache;
	btSequentialImpulseConstraintSolver* m_solver;
	btSoftRigidDynamicsWorld* m_dynamicsWorld;

	map<int, btCollisionShape*> m_collisionShapes;
	map<int, btRaycastVehicle*> m_vehicles;
	map<int, SubMeshTransformList> m_subMeshTransforms;
};
