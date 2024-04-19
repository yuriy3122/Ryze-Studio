#include <stdio.h>
#include <tchar.h>
#include <chrono>
#include <thread>
#include "iostream"
#include "winsock2.h"
#include "ResourceManager.h";
#include "PhysicsEngine.h";
#include "Wheel.h"
#include "Vehicle.h"
#pragma comment(lib,"WS2_32")

#pragma warning(disable:6387)

using namespace std;

static void pack_object_data(char* data, game_object_t* object)
{
	uint32_t offset = 0;
	uint16_t header = 1;
	memcpy(data + offset, (void*)&header, sizeof(uint16_t));
	offset += sizeof(uint16_t);

	int32_t id = object->objectId;
	memcpy(data + offset, (void*)&id, sizeof(int32_t));
	offset += sizeof(int32_t);

	float px = object->position.x;
	memcpy(data + offset, (void*)&px, sizeof(float));
	offset += sizeof(float);

	float py = object->position.y;
	memcpy(data + offset, (void*)&py, sizeof(float));
	offset += sizeof(float);

	float pz = object->position.z;
	memcpy(data + offset, (void*)&pz, sizeof(float));
	offset += sizeof(float);

	float rx = object->rotation.x;
	memcpy(data + offset, (void*)&rx, sizeof(float));
	offset += sizeof(float);

	float ry = object->rotation.y;
	memcpy(data + offset, (void*)&ry, sizeof(float));
	offset += sizeof(float);

	float rz = object->rotation.z;
	memcpy(data + offset, (void*)&rz, sizeof(float));
	offset += sizeof(float);

	float rw = object->rotation.w;
	memcpy(data + offset, (void*)&rw, sizeof(float));
}

static void pack_submesh_data(char* data, const game_object_t* object, const submesh_transform_t* transform)
{
	uint32_t offset = 0;
	uint16_t header = 2;
	memcpy(data + offset, (void*)&header, sizeof(uint16_t));
	offset += sizeof(uint16_t);

	int32_t objectId = object->objectId;
	memcpy(data + offset, (void*)&objectId, sizeof(int32_t));
	offset += sizeof(int32_t);

	int32_t submeshId = transform->subMeshId;
	memcpy(data + offset, (void*)&submeshId, sizeof(int32_t));
	offset += sizeof(int32_t);

	float px = transform->position.x;
	memcpy(data + offset, (void*)&px, sizeof(float));
	offset += sizeof(float);

	float py = transform->position.y;
	memcpy(data + offset, (void*)&py, sizeof(float));
	offset += sizeof(float);

	float pz = transform->position.z;
	memcpy(data + offset, (void*)&pz, sizeof(float));
	offset += sizeof(float);

	float rx = transform->rotation.x;
	memcpy(data + offset, (void*)&rx, sizeof(float));
	offset += sizeof(float);

	float ry = transform->rotation.y;
	memcpy(data + offset, (void*)&ry, sizeof(float));
	offset += sizeof(float);

	float rz = transform->rotation.z;
	memcpy(data + offset, (void*)&rz, sizeof(float));
	offset += sizeof(float);

	float rw = transform->rotation.w;
	memcpy(data + offset, (void*)&rw, sizeof(float));
}

static string ExePath()
{
	CHAR buffer[MAX_PATH] = { 0 };
	GetModuleFileNameA(NULL, buffer, MAX_PATH);
	std::string::size_type pos = std::string(buffer).find_last_of("\\/");
	return std::string(buffer).substr(0, pos);
}

int main()
{
	using namespace std::chrono_literals;

	string path = ExePath() + "\\" + "collision_data.bin";

	auto resourceManager = new ResourceManager();
	resourceManager->LoadResourcesFromFile(path);

	auto physicsEngine = new PhysicsEngine(resourceManager);
	physicsEngine->Initialize();

	size_t size = 38;
	char* buffer = (char*)malloc(size);

	int Port = 11001;
	char* IP_ADDRESS_S = "127.0.0.1";
	WSADATA wsaData;
	SOCKET sendSocket;
	sockaddr_in recvAddr{};
	recvAddr.sin_family = AF_INET;
	recvAddr.sin_port = htons(Port);
	recvAddr.sin_addr.s_addr = inet_addr(IP_ADDRESS_S);

	int res = WSAStartup(MAKEWORD(2, 2), &wsaData);

	if (res != 0)
	{
		return 0;
	}

	sendSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

	recvAddr.sin_family = AF_INET;
	recvAddr.sin_port = htons(Port);
	recvAddr.sin_addr.s_addr = inet_addr(IP_ADDRESS_S);

	do
	{
		physicsEngine->StepSimulation(1.0f / 100.0f);

		for (int i = 0; i < resourceManager->GetGameObjects().count; i++)
		{
			auto gameObject = resourceManager->GetGameObjects().data[i];

			if (gameObject != NULL)
			{
				pack_object_data(buffer, gameObject);

				sendto(sendSocket, buffer, 34, 0, (SOCKADDR*)&recvAddr, sizeof(recvAddr));

				SubMeshTransformList transforms = physicsEngine->GetSubMeshTransforms(gameObject->objectId);

				for (int j = 0; j < transforms.count; j++)
				{
					auto transform = &transforms.data[j];

					pack_submesh_data(buffer, gameObject, transform);

					sendto(sendSocket, buffer, 38, 0, (SOCKADDR*)&recvAddr, sizeof(recvAddr));
				}
			}
		}

		std::this_thread::sleep_for(10ms);
	}
	while (true);

	closesocket(sendSocket);

	WSACleanup();

	free(buffer);

	delete resourceManager;

	delete physicsEngine;

    return 0;
}