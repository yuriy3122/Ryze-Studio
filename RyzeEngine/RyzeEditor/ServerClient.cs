using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Packer;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor
{
    public class ServerClient
    {
        public WorldMap WorldMap { get; set; }

        private int _udpPort = 11000;

        private int _isSuspended = 1;

        const string App = "GameServer";

        private readonly string _outputFile;

        private readonly ConcurrentQueue<GameObjectMessage> _messages;

        private readonly Dictionary<int, GameObject> _gameObjectMap;

        public ServerClient()
        {
            _messages = new ConcurrentQueue<GameObjectMessage>();
            _gameObjectMap = new Dictionary<int, GameObject>();

            var dir = Path.GetDirectoryName(Application.ExecutablePath); ;
            _outputFile = Path.Combine(dir, "collision_data.bin");
        }

        public void Update()
        {
            _gameObjectMap.Clear();

            var gameObjects = WorldMap.Entities.OfType<GameObject>().ToList();

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.UserData != null)
                {
                    _gameObjectMap[(int)gameObject.UserData] = gameObject;
                }
            }

            int count = gameObjects.Count * 4;

            for (int i = 0; i < count; i++)
            {
                if (_messages.TryDequeue(out GameObjectMessage message))
                {
                    if (IsSuspended || !_gameObjectMap.ContainsKey(message.ObjectId))
                    {
                        continue;
                    }

                    var gameObject = _gameObjectMap[message.ObjectId];

                    if (message.SubmeshId < 0)
                    {
                        gameObject.Position = message.Position;
                        gameObject.Rotation = message.Rotation;
                    }
                    else
                    {
                        SubMeshTransform transform;
                        uint submeshId = (uint)message.SubmeshId;

                        if (gameObject.SubMeshTransforms == null)
                        {
                            gameObject.SubMeshTransforms = new ConcurrentDictionary<uint, SubMeshTransform>();
                        }

                        if (!gameObject.SubMeshTransforms.ContainsKey(submeshId))
                        {
                            transform = new SubMeshTransform();
                            gameObject.SubMeshTransforms[submeshId] = transform;
                        }
                        else
                        {
                            transform = gameObject.SubMeshTransforms[submeshId];
                        }

                        transform.Position = message.Position;
                        transform.Rotation = message.Rotation;
                    }
                }
            }
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
                                PackWorldData();

                                RestartServerProcess();

                                gameWordId = WorldMap.Id;

                                udpClient = new UdpClient(_udpPort);
                            }

                            var receivedData = await udpClient.ReceiveAsync();

                            using (var memoryStream = new MemoryStream(receivedData.Buffer))
                            {
                                memoryStream.Read(buffer, 0, sizeof(ushort));
                                ushort header = BitConverter.ToUInt16(buffer, 0);

                                switch (header)
                                {
                                    case 1:
                                        ReadGameObjectData(buffer, memoryStream);
                                        break;
                                    case 2:
                                        ReadSubMeshData(buffer, memoryStream);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        udpClient?.Close();
                        KillServerProcess();
                    }
                }
            });
        }

        private static int GetAvailablePort(IPAddress address, int startingPort)
        {
            IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                               where (n.LocalEndPoint.Port >= startingPort && n.LocalEndPoint.Address == address)
                               select n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            endPoints = properties.GetActiveTcpListeners();
            portArray.AddRange(from n in endPoints
                               where (n.Port >= startingPort && n.Address == address)
                               select n.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                               where (n.Port >= startingPort && n.Address == address)
                               select n.Port);

            portArray.Sort();

            for (int i = startingPort; i < ushort.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }

        private void ReadGameObjectData(byte[] buffer, MemoryStream memoryStream)
        {
            memoryStream.Read(buffer, 0, 32);
            var objectId = BitConverter.ToInt32(buffer, 0);

            float px = BitConverter.ToSingle(buffer, 4);
            float py = BitConverter.ToSingle(buffer, 8);
            float pz = -1.0f * BitConverter.ToSingle(buffer, 12);

            float rx = -1.0f * BitConverter.ToSingle(buffer, 16);
            float ry = -1.0f * BitConverter.ToSingle(buffer, 20);
            float rz = BitConverter.ToSingle(buffer, 24);
            float rw = BitConverter.ToSingle(buffer, 28);

            var message = new GameObjectMessage() 
            { 
                ObjectId = objectId,
                SubmeshId = -1,
                Position = new Vector3(px, py, pz),
                Rotation = new Quaternion(rx, ry, rz, rw)
            };

            _messages.Enqueue(message);
        }

        private void ReadSubMeshData(byte[] buffer, MemoryStream memoryStream)
        {
            memoryStream.Read(buffer, 0, 36);
            int objectId = BitConverter.ToInt32(buffer, 0);
            int submeshId = BitConverter.ToInt32(buffer, 4);

            float px = BitConverter.ToSingle(buffer, 8);
            float py = BitConverter.ToSingle(buffer, 12);
            float pz = -1.0f * BitConverter.ToSingle(buffer, 16);

            float rx = -1.0f * BitConverter.ToSingle(buffer, 20);
            float ry = -1.0f * BitConverter.ToSingle(buffer, 24);
            float rz = BitConverter.ToSingle(buffer, 28);
            float rw = BitConverter.ToSingle(buffer, 32);

            var message = new GameObjectMessage()
            {
                ObjectId = objectId,
                SubmeshId = submeshId,
                Position = new Vector3(px, py, pz),
                Rotation = new Quaternion(rx, ry, rz, rw)
            };

            _messages.Enqueue(message);
        }

        private void PackWorldData()
        {
            var options = new PackerOptions() { OutputFilePath = _outputFile };
            var packer = new WorldMapPacker(WorldMap, options);
            packer.Execute();
        }

        private void KillServerProcess()
        {
            try
            {
                var processes = Process.GetProcessesByName(App);

                foreach (Process proc in processes)
                {
                    proc.Kill();
                    proc.WaitForExit(1000);
                }
            }
            catch (NullReferenceException) {/* no instance running */}
        }

        private void RestartServerProcess()
        {
            KillServerProcess();

            Task.Delay(100);

            var address = IPAddress.Parse("127.0.0.5");
            var _udpPort = GetAvailablePort(address, 11000);

            try
            {
                var proc = new ProcessStartInfo($"{App}.exe")
                {
                    Arguments = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)} {_udpPort}",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process.Start(proc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(App + " shutdown exception: " + ex.Message);
            }
        }
    }

    public class GameObjectMessage
    {
        public int ObjectId { get; set; }

        public int SubmeshId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}