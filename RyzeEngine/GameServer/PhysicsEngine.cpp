#include "PhysicsEngine.h"
#include <iostream>

const float wheelFriction = 1000.0f;
const float suspensionStiffness = 200.0f;
const float suspensionDamping = 2.3f;
const float suspensionCompression = 4.4f;
const float rollInfluence = 0.1f;

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

btCollisionShape* PhysicsEngine::GetBulletCollisionShape(collision_shape_t* shape)
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
			case 4:
				collisionShape = CreateConvexHullShape(shape);
				break;
		}

		m_collisionShapes[shape->userIndex] = collisionShape;
	}

	return collisionShape;
}

void PhysicsEngine::InitializeRigidBodies()
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
			auto collisionShape = GetBulletCollisionShape(shape);
			auto btRigidBody = CreateRigidBody(rigidBody, collisionShape);
			m_dynamicsWorld->addRigidBody(btRigidBody);
		}
	}
}

void PhysicsEngine::Initialize()
{
	InitializeRigidBodies();
	InitializeVehicles();
}

void PhysicsEngine::InitializeVehicles()
{
	for (int i = 0; i < m_resourceManager->GetVehicles().count; i++)
	{
		vehicle_t* vehicle = m_resourceManager->GetVehicles().data[i];

		if (vehicle == NULL)
		{
			continue;
		}

		collision_shape_t* shape = NULL;

		for (int j = 0; j < m_resourceManager->GetCollisionShapes().count; j++)
		{
			collision_shape_t* ptr = m_resourceManager->GetCollisionShapes().data[j];

			if (ptr != NULL && ptr->userIndex == vehicle->chassisCollisionShapeId)
			{
				shape = ptr;
				break;
			}
		}

		if (shape == NULL)
		{
			return;
		}

		btCompoundShape* chassisShape = new btCompoundShape();
		const btScalar* vertexData = (const btScalar*)shape->vertexData;
		btConvexShape* convexShape = new btConvexHullShape(vertexData, shape->numberOfVertices);

		btTransform localTransform;
		localTransform.setIdentity();
		btVector3 center(shape->center.x, shape->center.y, shape->center.z);
		localTransform.setOrigin(center);

		chassisShape->addChildShape(localTransform, convexShape);

		btVector3 chassisLocalInertia(0.0f, 0.0f, 0.0f);
		chassisShape->calculateLocalInertia(vehicle->mass, chassisLocalInertia);

		btTransform chassisStartTransform;
		chassisStartTransform.setIdentity();
		chassisStartTransform.setOrigin(btVector3(0.0f, 0.0f, 0.0f));

		btDefaultMotionState* chassisMotionState = new btDefaultMotionState(chassisStartTransform);
		btRigidBody::btRigidBodyConstructionInfo chassisRbInfo(vehicle->mass, chassisMotionState, chassisShape, chassisLocalInertia);

		game_object_t* object = vehicle->gameObject;
		btVector3 origin(object->position.x, object->position.y, object->position.z);
		btQuaternion rotation(object->rotation.x, object->rotation.y, object->rotation.z, object->rotation.w);

		btTransform transform;
		transform.setIdentity();
		transform.setOrigin(origin);
		transform.setRotation(rotation);

		btRigidBody* chassis = new btRigidBody(chassisRbInfo);
		chassis->setCenterOfMassTransform(transform);
		chassis->setUserPointer(vehicle->gameObject);
		chassis->setLinearVelocity(btVector3(0.0f, 0.0f, 0.0f));
		chassis->setAngularVelocity(btVector3(0.0f, 0.0f, 0.0f));
		chassis->setActivationState(DISABLE_DEACTIVATION);

		m_dynamicsWorld->addRigidBody(chassis);

		btRaycastVehicle::btVehicleTuning tuning;
		auto vehicleRayCaster = new btDefaultVehicleRaycaster(m_dynamicsWorld);
		auto raycastVehicle = new btRaycastVehicle(tuning, chassis, vehicleRayCaster);
		raycastVehicle->setCoordinateSystem(0, 1, 2);

		m_dynamicsWorld->addVehicle(raycastVehicle);

		bool isFrontWheel = true;

		btVector3 connectionPointCS0 = btVector3(vehicle->frontLeftSideWheel->chassisConnectionPointCS.x,
												 vehicle->frontLeftSideWheel->chassisConnectionPointCS.y,
												 vehicle->frontLeftSideWheel->chassisConnectionPointCS.z);

		btVector3 wheelDirectionCS0 = btVector3(vehicle->frontLeftSideWheel->wheelDirectionCS.x,
												vehicle->frontLeftSideWheel->wheelDirectionCS.y,
												vehicle->frontLeftSideWheel->wheelDirectionCS.z);

		btVector3 wheelAxleCS0 = btVector3(vehicle->frontLeftSideWheel->axleCS.x,
										   vehicle->frontLeftSideWheel->axleCS.y,
										   vehicle->frontLeftSideWheel->axleCS.z);

		float suspensionRestLength = vehicle->frontLeftSideWheel->suspensionRestLength;
		float radius = vehicle->frontLeftSideWheel->radius;

		//0: front left-side wheel
		raycastVehicle->addWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS0, suspensionRestLength, radius, tuning, isFrontWheel);

		connectionPointCS0 = btVector3(vehicle->frontRightSideWheel->chassisConnectionPointCS.x,
									   vehicle->frontRightSideWheel->chassisConnectionPointCS.y,
									   vehicle->frontRightSideWheel->chassisConnectionPointCS.z);

		suspensionRestLength = vehicle->frontRightSideWheel->suspensionRestLength;

		//1: front right-side wheel
		raycastVehicle->addWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS0, suspensionRestLength, radius, tuning, isFrontWheel);

		isFrontWheel = false;

		connectionPointCS0 = btVector3(vehicle->rearLeftSideWheel->chassisConnectionPointCS.x,
									   vehicle->rearLeftSideWheel->chassisConnectionPointCS.y,
									   vehicle->rearLeftSideWheel->chassisConnectionPointCS.z);

		suspensionRestLength = vehicle->rearLeftSideWheel->suspensionRestLength;

		//2: rear left-side wheel
		raycastVehicle->addWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS0, suspensionRestLength, radius, tuning, isFrontWheel);

		connectionPointCS0 = btVector3(vehicle->rearRightSideWheel->chassisConnectionPointCS.x,
									   vehicle->rearRightSideWheel->chassisConnectionPointCS.y,
									   vehicle->rearRightSideWheel->chassisConnectionPointCS.z);

		suspensionRestLength = vehicle->rearRightSideWheel->suspensionRestLength;

		//3: rear right-side wheel
		raycastVehicle->addWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS0, suspensionRestLength, radius, tuning, isFrontWheel);

		for (int i = 0; i < raycastVehicle->getNumWheels(); i++)
		{
			btWheelInfo& wheel = raycastVehicle->getWheelInfo(i);
			wheel.m_suspensionStiffness = suspensionStiffness;
			wheel.m_wheelsDampingRelaxation = suspensionDamping;
			wheel.m_wheelsDampingCompression = suspensionCompression;
			wheel.m_frictionSlip = wheelFriction;
			wheel.m_rollInfluence = rollInfluence;
		}

		m_vehicles[vehicle->vehicleId] = raycastVehicle;

		m_dynamicsWorld->getBroadphase()->getOverlappingPairCache()->cleanProxyFromPairs(chassis->getBroadphaseHandle(), m_dynamicsWorld->getDispatcher());

		raycastVehicle->resetSuspension();

		for (int i = 0; i < raycastVehicle->getNumWheels(); i++)
		{
			raycastVehicle->updateWheelTransform(i, true);
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

btCollisionShape* PhysicsEngine::CreateConvexHullShape(collision_shape_t* shape)
{
	btCompoundShape* collisionShape = new btCompoundShape();

	btTransform localTransform;
	localTransform.setIdentity();
	localTransform.setOrigin(btVector3(shape->center.x, shape->center.y, shape->center.z));

	btConvexShape* convexShape = new btConvexHullShape((const btScalar*)shape->vertexData, (int)shape->numberOfVertices);

	collisionShape->addChildShape(localTransform, convexShape);

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
