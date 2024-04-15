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

using namespace std;

static int send_data(const char* data, size_t size)
{
	WSADATA wsaData;
	SOCKET sendSocket;
	sockaddr_in recvAddr{};
	int Port = 11000;
	char* IP_ADDRESS_S = "127.0.0.1";

	int res = WSAStartup(MAKEWORD(2, 2), &wsaData);

	if (res == 0)
	{
		sendSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

		recvAddr.sin_family = AF_INET;
		recvAddr.sin_port = htons(Port);
		recvAddr.sin_addr.s_addr = inet_addr(IP_ADDRESS_S);

		sendto(sendSocket, data, size, 0, (SOCKADDR*)&recvAddr, sizeof(recvAddr));

		closesocket(sendSocket);
	}

	WSACleanup();

	return 0;
}

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

static string ExePath()
{
	CHAR buffer[MAX_PATH] = { 0 };
	GetModuleFileNameA(NULL, buffer, MAX_PATH);
	std::string::size_type pos = std::string(buffer).find_last_of("\\/");
	return std::string(buffer).substr(0, pos);
}

int main()
{
	size_t s1 = sizeof(vehicle_t);
	size_t s2 = sizeof(wheel_t);

	using namespace std::chrono_literals;

	string path = ExePath() + "\\" + "collision_data.bin";

	auto resourceManager = new ResourceManager();
	resourceManager->LoadResourcesFromFile(path);

	auto physicsEngine = new PhysicsEngine(resourceManager);
	physicsEngine->Initialize();

	const size_t size = 36;
	char* buffer = (char*)malloc(size);

	do
	{
		physicsEngine->StepSimulation(1.0f / 100.0f);

		for (int i = 0; i < resourceManager->GetGameObjects().count; i++)
		{
			auto gameObject = resourceManager->GetGameObjects().data[i];

			if (gameObject != NULL)
			{
				pack_object_data(buffer, gameObject);

				send_data(buffer, size);
			}
		}

		std::this_thread::sleep_for(1ms);
	}
	while (true);

	free(buffer);

	delete resourceManager;

	delete physicsEngine;

    return 0;
}