using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Packer;

namespace RyzeEditor
{
    public class ServerClient
    {
        private const int UdpPort = 11000;

        public ServerClient()
        {
        }

        public void Suspend()
        {
            //TODO: Send command to Server to Suspend
        }

        public void ProcessMessages(WorldMap worldMap)
        {
            var task = Task.Run(async () =>
            {
                PackWorldData(worldMap);

                //TODO: Run Server Process

                var gameObjectMap = new Dictionary<int, GameObject>();
                var gameObjects = worldMap.Entities.OfType<GameObject>().ToList();

                foreach (var gameObject in gameObjects)
                {
                    gameObjectMap[(int)gameObject.UserData] = gameObject;
                }

                byte[] buffer = new byte[1024];

                using (var udpClient = new UdpClient(UdpPort))
                {
                    while (true)
                    {
                        var receivedData = await udpClient.ReceiveAsync();

                        using (var memoryStream = new MemoryStream(receivedData.Buffer))
                        {
                            memoryStream.Read(buffer, 0, sizeof(ushort));
                            ushort header = BitConverter.ToUInt16(buffer, 0);

                            if (header == 1)
                            {
                                memoryStream.Read(buffer, 0, 32);
                                var objectId = BitConverter.ToInt32(buffer, 0);

                                if (gameObjectMap.ContainsKey(objectId))
                                {
                                    float px = BitConverter.ToSingle(buffer, 4);
                                    float py = BitConverter.ToSingle(buffer, 8);
                                    float pz = -1.0f * BitConverter.ToSingle(buffer, 12);
                                    gameObjectMap[objectId].Position = new Vector3(px, py, pz);

                                    float rx = -1.0f * BitConverter.ToSingle(buffer, 16);
                                    float ry = -1.0f * BitConverter.ToSingle(buffer, 20);
                                    float rz = BitConverter.ToSingle(buffer, 24);
                                    float rw = BitConverter.ToSingle(buffer, 28);
                                    gameObjectMap[objectId].Rotation = new Quaternion(rx, ry, rz, rw);
                                }
                            }
                        }
                    }
                }
            });
        }

        private void PackWorldData(WorldMap worldMap)
        {
            var options = new PackerOptions();
            var packer = new WorldMapPacker(worldMap, options);
            packer.Execute();
        }
    }

    public class GameObjectMessage
    {
        public int ObjectId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}
