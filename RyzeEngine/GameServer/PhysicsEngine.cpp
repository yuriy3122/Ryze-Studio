#include "PhysicsEngine.h"
#include <iostream>

PhysicsEngine::PhysicsEngine(ResourceManager* resourceManager)
{
	m_resourceManager = resourceManager;

	m_collisionConfiguration = new btDefaultCollisionConfiguration();
	m_dispatcher = new btCollisionDispatcher(m_collisionConfiguration);

	btVector3 worldMin(-1000, -1000, -1000);
	btVector3 worldMax(1000, 1000, 1000);
	m_overlappingPairCache = new btAxisSweep3(worldMin, worldMax);
	m_solver = new btSequentialImpulseConstraintSolver;

	m_dynamicsWorld = new btDiscreteDynamicsWorld(m_dispatcher, m_overlappingPairCache, m_solver, m_collisionConfiguration);
	m_dynamicsWorld->getSolverInfo().m_minimumSolverBatchSize = 128;
	m_dynamicsWorld->getSolverInfo().m_globalCfm = 0.00001;
	m_dynamicsWorld->setGravity(btVector3(0, -9.8, 0));
}

void PhysicsEngine::Initialize()
{
	for (int i = 0; i < m_resourceManager->GetRigidBodies().count; i++)
	{
		auto rigidBody = m_resourceManager->GetRigidBodies().data[i];
		collision_shape_t* shape = NULL;

		for (int j = 0; j < m_resourceManager->GetCollisionShapes().count; j++)
		{
			auto ptr = m_resourceManager->GetCollisionShapes().data[j];

			if (ptr->userIndex == rigidBody->collisionShapeIndex)
			{
				shape = m_resourceManager->GetCollisionShapes().data[j];
				break;
			}
		}

		if (shape != NULL)
		{
			btCollisionShape* collisionShape = NULL;

			auto shapeIter = m_collisionShapes.find(shape->userIndex);

			if (shapeIter != m_collisionShapes.end())
			{
				collisionShape = shapeIter->second;
			}

			if (collisionShape == NULL)
			{
				switch (shape->shapeType)
				{
					case 0:
						collisionShape = CreateBoxShape(shape);
						break;
				}

				m_collisionShapes[shape->userIndex] = collisionShape;
			}

			auto btRigidBody = CreateRigidBody(rigidBody, collisionShape);
			m_dynamicsWorld->addRigidBody(btRigidBody);
		}
	}
}

void PhysicsEngine::StepSimulation(float deltaTime)
{
	m_dynamicsWorld->stepSimulation(deltaTime, 10, 1.0f / 100.0f);

	for (int j = m_dynamicsWorld->getNumCollisionObjects() - 1; j >= 0; j--)
	{
		btCollisionObject* obj = m_dynamicsWorld->getCollisionObjectArray()[j];
		btRigidBody* body = btRigidBody::upcast(obj);
		btTransform transform;

		if (body && body->getMotionState())
		{
			body->getMotionState()->getWorldTransform(transform);
		}
		else
		{
			transform = obj->getWorldTransform();
		}

		if (body != NULL)
		{
			void* ptr = body->getUserPointer();

			if (ptr != NULL)
			{
				game_object_t* gameObject = (game_object_t*)ptr;

				gameObject->position.x = transform.getOrigin().getX();
				gameObject->position.y = transform.getOrigin().getY();
				gameObject->position.z = transform.getOrigin().getZ();

				btQuaternion btQuat = transform.getRotation();
				quaternion_t quat{};
				quat.x = btQuat.getX();
				quat.y = btQuat.getY();
				quat.z = btQuat.getZ();
				quat.w = btQuat.getW();
				gameObject->rotation = quat;
			}
		}
	}
}

btCollisionShape* PhysicsEngine::CreateBoxShape(collision_shape_t* shape)
{
	btCompoundShape* collisionShape = new btCompoundShape();

	btTransform localTransform;
	localTransform.setIdentity();
	localTransform.setOrigin(btVector3(shape->center.x, shape->center.y, shape->center.z));

	btBoxShape* boxShape = new btBoxShape(btVector3(shape->boundBox.x, shape->boundBox.y, shape->boundBox.z));

	collisionShape->addChildShape(localTransform, boxShape);

	return collisionShape;
}

btRigidBody* PhysicsEngine::CreateRigidBody(rigid_body_t* rigidBody, btCollisionShape* collisionShape)
{
	game_object_t* gameObject = NULL;

	for (int i = 0; i < m_resourceManager->GetGameObjects().count; i++)
	{
		if (m_resourceManager->GetGameObjects().data[i]->objectId == rigidBody->gameObjectId)
		{
			gameObject = m_resourceManager->GetGameObjects().data[i];
			break;
		}
	}

	btScalar mass = rigidBody->invMass;
	btVector3 localInertia(0.0f, 0.0f, 0.0f);
	collisionShape->calculateLocalInertia(mass, localInertia);

	btTransform startTransform;
	startTransform.setIdentity();

	if (gameObject != NULL)
	{
		startTransform.setOrigin(btVector3(gameObject->position.x, gameObject->position.y, gameObject->position.z));
		startTransform.setRotation(btQuaternion(gameObject->rotation.x, gameObject->rotation.y, gameObject->rotation.z, gameObject->rotation.w));
	}

	btDefaultMotionState* motionState = new btDefaultMotionState(startTransform);
	btRigidBody::btRigidBodyConstructionInfo rbInfo(mass, motionState, collisionShape, localInertia);
	btRigidBody* body = new btRigidBody(rbInfo);
	body->setUserPointer(gameObject);

	return body;
}

PhysicsEngine::~PhysicsEngine()
{
	delete m_collisionConfiguration;
	delete m_dispatcher;
	delete m_overlappingPairCache;
	delete m_solver;
	delete m_dynamicsWorld;
}
