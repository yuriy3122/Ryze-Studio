#include <windows.h>
#include <stdio.h>
#include "ResourceManager.h"
#include "SharedStructures.h"

#pragma warning(disable:6385);
#pragma warning(disable:6387);

ResourceManager::ResourceManager()
{
	m_gameObjects.count = 0;
	m_collisionShapeList.count = 0;
	m_rigidBodyList.count = 0;
	m_vehicleList.count = 0;
	m_data = NULL;
}

DWORD ResourceManager::ReadGameObjectData(const char* buffer, DWORD pos)
{
	game_object_t* objPtr = (game_object_t*)(buffer + pos);
	m_gameObjects.data[m_gameObjects.count++] = objPtr;

	DWORD offset = sizeof(game_object_t);

	return offset;
}

DWORD ResourceManager::ReadCollisionShapeData(const char* buffer, DWORD pos)
{
	collision_shape_t* shape = (collision_shape_t*)malloc(sizeof(collision_shape_t));
	memset(shape, 0, sizeof(collision_shape_t));

	DWORD offset = 0;

	if (shape != NULL)
	{
		shape->userIndex = *((int*)((char*)buffer + pos + offset));
		offset += sizeof(int);

		shape->shapeType = *((int*)((char*)buffer + pos + offset));
		offset += sizeof(int);

		int alligment = 0;
		vector3_t box = { 0.0f, 0.0f, 0.0f };
		shape->boundBox = box;

		switch (shape->shapeType)
		{
			case 0://BoxShape
				shape->numberOfVertices = 0;
				shape->vertexData = NULL;

				shape->boundBox = *((vector3_t*)((char*)buffer + pos + offset));
				offset += sizeof(vector3_t);

				shape->center = *((vector3_t*)((char*)buffer + pos + offset));
				offset += sizeof(vector3_t);

				break;

			case 4://ConvexHullShape
				shape->center = *((vector3_t*)((char*)buffer + pos + offset));
				offset += sizeof(vector3_t);

				shape->numberOfVertices = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int);

				alligment = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int) + alligment;

				shape->vertexData = (float*)((char*)buffer + pos + offset);
				offset += static_cast<unsigned long long>(shape->numberOfVertices) * 4 * sizeof(float);

				break;

			case 24://Heightfield Terrain
				shape->center = *((vector3_t*)((char*)buffer + pos + offset));
				offset += sizeof(vector3_t);

				shape->heightStickWidth = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int);

				shape->heightStickLength = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int);

				shape->gridSpacing = *((float*)((char*)buffer + pos + offset));
				offset += sizeof(float);

				shape->minHeight = *((float*)((char*)buffer + pos + offset));
				offset += sizeof(float);

				shape->maxHeight = *((float*)((char*)buffer + pos + offset));
				offset += sizeof(float);

				shape->numberOfVertices = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int);

				alligment = *((int*)((char*)buffer + pos + offset));
				offset += sizeof(int) + alligment;

				shape->vertexData = (float*)((char*)buffer + pos + offset);
				offset += static_cast<unsigned long long>(shape->numberOfVertices) * sizeof(float);

				break;
		}

		m_collisionShapeList.data[m_collisionShapeList.count++] = shape;
	}

	return offset;
}

DWORD ResourceManager::ReadRigidBodyData(const char* buffer, DWORD pos)
{
	rigid_body_t* body = (rigid_body_t*)((char*)buffer + pos);

	DWORD offset = sizeof(rigid_body_t);

	m_rigidBodyList.data[m_rigidBodyList.count++] = body;

	return offset;
}

DWORD ResourceManager::ReadCollisionData(const char* buffer, DWORD pos)
{
	int blockSize = *((int*)((char*)buffer + pos));
	long offset = 0;
	offset += sizeof(int);

	int collisionShapeCount = *((int*)((char*)buffer + pos + offset));
	offset += sizeof(int);

	size_t collisionShapeDataSize = collisionShapeCount * sizeof(collision_shape_t*);
	m_collisionShapeList.data = (collision_shape_t**)malloc(collisionShapeDataSize);
	memset((void*)m_collisionShapeList.data, 0, collisionShapeDataSize);

	int rigidBodyCount = *((int*)((char*)buffer + pos + offset));
	offset += sizeof(int);

	size_t rigidBodyDataSize = rigidBodyCount * sizeof(rigid_body_t*);
	m_rigidBodyList.data = (rigid_body_t**)malloc(rigidBodyDataSize);
	memset((void*)m_rigidBodyList.data, 0, rigidBodyDataSize);

	while (offset < blockSize)
	{
		uint16_t chunk = (((buffer)[pos + offset + 1] << 8) & 0xFF00) | ((buffer)[pos + offset] & 0xFF);

		offset += sizeof(uint16_t);

		switch (chunk) {
			case ID_COLLISION_SHAPE_BLOCK_CHUNK:
				offset += ReadCollisionShapeData(buffer, pos + offset);
				break;
			case ID_COLLISION_RIGID_BLOCK_CHUNK:
				offset += ReadRigidBodyData(buffer, pos + offset);
				break;
			default:
				break;
		}
	}

	return offset;
}

wheel_t* ResourceManager::ReadWheelData(const char* buffer, DWORD pos)
{
	int offset = 0;
	int subMeshIdCount = *((int*)((char*)buffer + pos + offset));
	offset += sizeof(int);
	int* subMeshIds = (int*)((char*)buffer + pos + offset);
	offset += subMeshIdCount * sizeof(int);
	wheel_t* wheelPtr = (wheel_t*)((char*)buffer + pos + offset);
	wheelPtr->subMeshIds = subMeshIds;
	wheelPtr->subMeshIdCount = subMeshIdCount;

	return wheelPtr;
}

DWORD ResourceManager::ReadVehicleData(const char* buffer, DWORD pos)
{
	int offset = 0;
	wheel_t* frontLeftWheelPtr = ReadWheelData(buffer, pos + offset);
	offset += sizeof(int) + frontLeftWheelPtr->subMeshIdCount * sizeof(int) + sizeof(wheel_t);

	wheel_t* frontRightWheelPtr = ReadWheelData(buffer, pos + offset);
	offset += sizeof(int) + frontRightWheelPtr->subMeshIdCount * sizeof(int) + sizeof(wheel_t);

	wheel_t* rearLeftWheelPtr = ReadWheelData(buffer, pos + offset);
	offset += sizeof(int) + rearLeftWheelPtr->subMeshIdCount * sizeof(int) + sizeof(wheel_t);

	wheel_t* rearRightWheelPtr = ReadWheelData(buffer, pos + offset);
	offset += sizeof(int) + rearRightWheelPtr->subMeshIdCount * sizeof(int) + sizeof(wheel_t);

	vehicle_t* vehicle = (vehicle_t*)((char*)buffer + pos + offset);
	vehicle->frontLeftSideWheel = frontLeftWheelPtr;
	vehicle->frontRightSideWheel = frontRightWheelPtr;
	vehicle->rearLeftSideWheel = rearLeftWheelPtr;
	vehicle->rearRightSideWheel = rearRightWheelPtr;

	for (int i = 0; i < m_gameObjects.count; i++)
	{
		game_object_t* gameObject = m_gameObjects.data[i];

		if (gameObject != NULL && gameObject->objectId == vehicle->gameObjectId)
		{
			vehicle->gameObject = gameObject;
			break;
		}
	}

	m_vehicleList.data[m_vehicleList.count++] = vehicle;

	offset += sizeof(vehicle_t);

	return offset;
}

int ResourceManager::LoadResourcesFromFile(std::string filePath)
{
	void* handle = CreateFileA(filePath.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);

	if (handle == INVALID_HANDLE_VALUE)
	{
		return 0;
	}

	DWORD size = 2 * sizeof(unsigned short) + sizeof(int);
	char* initData = (char*)malloc(size);

	DWORD nRead = 0;
	BOOL result = ReadFile(handle, initData, size, &nRead, NULL);

	if (result == FALSE)
	{
		return 0;
	}

	int textureBlockPtr = *((int*)((char*)initData + 2 * sizeof(unsigned short)));
	size = textureBlockPtr + sizeof(unsigned short);

	free(initData);

	nRead = 0;
	m_data = (char*)malloc(size);
	result = ReadFile(handle, m_data, size, &nRead, NULL);

	if (result == FALSE)
	{
		return 0;
	}

	long pos = 3 * sizeof(int);

	if (m_data != NULL)
	{
		int gameObjectsCount = *((int*)(m_data + pos));

		if (gameObjectsCount > 0)
		{
			m_gameObjects.data = (game_object_t**)malloc(gameObjectsCount * sizeof(game_object_t*));
		}

		m_vehicleList.data = (vehicle_t**)malloc(64 * sizeof(vehicle_t*));

		pos += sizeof(int);

		while (pos < nRead)
		{
			uint16_t chunk = (((m_data)[pos + 1] << 8) & 0xFF00) | ((m_data)[pos] & 0xFF);

			pos += sizeof(uint16_t);

			switch (chunk)
			{
				case ID_GAME_OBJ_CHUNK:
					pos += ReadGameObjectData(m_data, pos);
					break;
				case ID_COLLISION_BLOCK_CHUNK:
					pos += ReadCollisionData(m_data, pos);
					break;
				case ID_VEHICLE_BLOCK_CHUNK:
					pos += ReadVehicleData(m_data, pos);
					break;
				default:
					break;
			}
		}
	}

	result = CloseHandle(handle);

	return result;
}

const GameObjectList& ResourceManager::GetGameObjects() const 
{
	return m_gameObjects;
}

const CollisionShapeList& ResourceManager::GetCollisionShapes() const
{
	return m_collisionShapeList;
}

const RigidBodyList& ResourceManager::GetRigidBodies() const
{
	return m_rigidBodyList;
}

const VehicleList& ResourceManager::GetVehicles() const
{
	return m_vehicleList;
}

ResourceManager::~ResourceManager()
{
	if (m_collisionShapeList.count > 0)
	{
		for (int i = 0; i < m_collisionShapeList.count; i++)
		{
			auto ptr = m_collisionShapeList.data[i];

			if (ptr != NULL)
			{
				delete ptr;
			}
		}
		free(m_collisionShapeList.data);
	}

	if (m_rigidBodyList.count > 0)
	{
		free(m_rigidBodyList.data);
	}
	if (m_gameObjects.count > 0)
	{
		free(m_gameObjects.data);
	}
	if (m_vehicleList.count > 0)
	{
		free(m_vehicleList.data);
	}

	free(m_data);
}
