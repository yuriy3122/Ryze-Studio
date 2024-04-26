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
        const string App = "GameServer";

        private IPEndPoint _endpoint = null;
        private UdpClient _udpClient = null;
        private int _udpPort = 11000;

        private int _isSuspended = 1;
        private string _gameWordId;
        private readonly string _outputFile;

        public WorldMap WorldMap { get; set; }

        private readonly Dictionary<int, GameObject> _gameObjectMap;

        public ServerClient()
        {
            _gameObjectMap = new Dictionary<int, GameObject>();

            var dir = Path.GetDirectoryName(Application.ExecutablePath); ;
            _outputFile = Path.Combine(dir, "collision_data.bin");
        }

        public void Update()
        {
            if (!IsSuspended)
            {
                if (_gameWordId != WorldMap.Id)
                {
                    _gameObjectMap.Clear();

                    PackWorldData();

                    var gameObjects = WorldMap.Entities.OfType<GameObject>().ToList();

                    foreach (var gameObject in gameObjects)
                    {
                        if (gameObject.UserData != null)
                        {
                            _gameObjectMap[(int)gameObject.UserData] = gameObject;
                        }
                    }

                    RestartServerProcess();

                    var address = IPAddress.Parse("127.0.0.5");
                    var _udpPort = GetAvailablePort(address, 11000);
                    _endpoint = new IPEndPoint(address, _udpPort);
                    _udpClient = new UdpClient(_udpPort);

                    _gameWordId = WorldMap.Id;
                }
            }
            else
            {
                if (_udpClient != null)
                {
                    _udpClient.Close();
                    KillServerProcess();
                    _udpClient = null;
                }
            }

            var availableToRead = _udpClient?.Available ?? 0;

            while (availableToRead > 0)
            {
                var receivedData = _udpClient?.Receive(ref _endpoint);
                availableToRead -= receivedData.Length;

                using (var memoryStream = new MemoryStream(receivedData))
                {
                    memoryStream.Read(receivedData, 0, sizeof(ushort));
                    ushort header = BitConverter.ToUInt16(receivedData, 0);
                    var message = ReadGameObjectData(receivedData, memoryStream);

                    if (message == null || !_gameObjectMap.ContainsKey(message.ObjectId))
                    {
                        continue;
                    }

                    var gameObject = _gameObjectMap[message.ObjectId];

                    switch (header)
                    {
                        case 1:
                            gameObject.Position = message.Position;
                            gameObject.Rotation = message.Rotation;

                            break;
                        case 2:
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

                            break;
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

        private GameObjectData ReadGameObjectData(byte[] buffer, MemoryStream memoryStream)
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

            var message = new GameObjectData()
            {
                ObjectId = objectId,
                SubmeshId = submeshId,
                Position = new Vector3(px, py, pz),
                Rotation = new Quaternion(rx, ry, rz, rw)
            };

            return message;
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

    public class GameObjectData
    {
        public int ObjectId { get; set; }

        public int SubmeshId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}