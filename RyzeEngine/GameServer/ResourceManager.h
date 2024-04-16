#ifndef ResourceManager_h
#define ResourceManager_h
#include <string>
#include "GameObject.h"
#include "CollisionShape.h"
#include "RigidBody.h"
#include "Wheel.h"
#include "Vehicle.h"
#include <Windows.h>

class ResourceManager
{
public:
	ResourceManager();

	int LoadResourcesFromFile(std::string filePath);

	~ResourceManager();

	const GameObjectList& GetGameObjects() const;

	const CollisionShapeList& GetCollisionShapes() const;

	const RigidBodyList& GetRigidBodies() const;

	const VehicleList& GetVehicles() const;

private:
	DWORD ReadGameObjectData(const char* buffer, DWORD offset);

	DWORD ReadCollisionData(const char* buffer, DWORD offset);

	DWORD ReadCollisionShapeData(const char* buffer, DWORD pos);

	DWORD ReadRigidBodyData(const char* buffer, DWORD pos);

	wheel_t* ReadWheelData(const char* buffer, DWORD pos);

	DWORD ReadVehicleData(const char* buffer, DWORD pos);

	GameObjectList m_gameObjects;

	CollisionShapeList m_collisionShapeList;

	RigidBodyList m_rigidBodyList;

	VehicleList m_vehicleList;

	char* m_data;
};

#endif /* ResourceManager_h */