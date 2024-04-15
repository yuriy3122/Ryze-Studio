using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
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
        
        public WorldMap WorldMap { get; set; }

        private int _isSuspended = 1;

        private readonly string _outputFile;

        public ServerClient()
        {
            var dir = Path.GetDirectoryName(Application.ExecutablePath); ;
            _outputFile = Path.Combine(dir, "collision_data.bin");
        }

        public bool IsSuspended
        { 
            get
            {
                return Interlocked.CompareExchange(ref _isSuspended, 0, 0) > 0;
            }

            set
            {
                if (value)
                {
                    if (Interlocked.CompareExchange(ref _isSuspended, 0, 0) == 0)
                    {
                        Interlocked.Increment(ref _isSuspended);
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _isSuspended);
                }
            }
        }

        public void ProcessMessages()
        {
            var task = Task.Run(async () =>
            {
                string gameWordId = string.Empty;
                Dictionary<int, GameObject> gameObjectMap = null;

                UdpClient udpClient = null;
                byte[] buffer = new byte[1024];

                while (true)
                {
                    if (!IsSuspended)
                    {
                        try
                        {
                            if (gameWordId != WorldMap.Id)
                            {
                                udpClient?.Close();

                                PackWorldData();

                                RestartServerProcess();

                                gameObjectMap = new Dictionary<int, GameObject>();
                                var gameObjects = WorldMap.Entities.OfType<GameObject>().ToList();

                                foreach (var gameObject in gameObjects)
                                {
                                    if (gameObject.UserData != null)
                                    {
                                        gameObjectMap[(int)gameObject.UserData] = gameObject;
                                    }
                                }

                                gameWordId = WorldMap.Id;

                                udpClient = new UdpClient(UdpPort);
                            }

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
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            });
        }

        private void PackWorldData()
        {
            var options = new PackerOptions() { OutputFilePath = _outputFile };
            var packer = new WorldMapPacker(WorldMap, options);
            packer.Execute();
        }

        private void RestartServerProcess()
        {
            const string app = "GameServer";

            try
            {
                var processes = Process.GetProcessesByName(app);

                foreach (Process proc in processes)
                {
                    proc.Kill();
                    proc.WaitForExit(1000);
                }
            }
            catch (NullReferenceException) {/* no instance running */}

            try
            {
                var proc = new ProcessStartInfo($"{app}.exe")
                {
                    Arguments = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)} {_outputFile}",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(proc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(app + " shutdown exception: " + ex.Message);
            }
        }
    }

    public class GameObjectMessage
    {
        public int ObjectId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}
