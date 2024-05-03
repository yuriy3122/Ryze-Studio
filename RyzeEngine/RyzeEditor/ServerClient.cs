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
        private readonly Dictionary<long, GameObjectData> _objectData;
        private readonly Dictionary<long, Queue<GameObjectTimeState>> _states;
        private readonly Dictionary<long, float> _velocityValues;

        private const int QueueSize = 3;

        public ServerClient()
        {
            _gameObjectMap = new Dictionary<int, GameObject>();
            _objectData = new Dictionary<long, GameObjectData>();
            _states = new Dictionary<long, Queue<GameObjectTimeState>>();
            _velocityValues = new Dictionary<long, float>();

            var dir = Path.GetDirectoryName(Application.ExecutablePath);
            _outputFile = Path.Combine(dir, "collision_data.bin");
        }

        public long MakeKey(int left, int right, ushort header)
        {
            //implicit conversion of left to a long
            long res = left;

            //shift the bits creating an empty space on the right
            // ex: 0x0000CFFF becomes 0xCFFF0000
            res <<= 32;

            //combine the bits on the right with the previous value
            // ex: 0xCFFF0000 | 0x0000ABCD becomes 0xCFFFABCD
            res |= (uint)(right + header); //uint first to prevent loss of signed bit

            //return the combined result
            return res;
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
                _states.Clear();
                _velocityValues.Clear();

                if (_udpClient != null)
                {
                    _udpClient.Close();
                    KillServerProcess();
                    _udpClient = null;
                }
            }

            var availableToRead = _udpClient?.Available ?? 0;

            _objectData.Clear();

            while (availableToRead > 0)
            {
                var receivedData = _udpClient?.Receive(ref _endpoint);
                availableToRead -= receivedData.Length;

                using (var memoryStream = new MemoryStream(receivedData))
                {
                    memoryStream.Read(receivedData, 0, 46);
                    var message = ReadGameObjectData(receivedData);

                    if (message == null || !_gameObjectMap.ContainsKey(message.ObjectId))
                    {
                        continue;
                    }

                    var key = MakeKey(message.ObjectId, message.SubmeshId, message.Header);

                    if (!_objectData.ContainsKey(key))
                    {
                        _objectData[key] = message;
                    }
                    else
                    {
                        if (_objectData[key].Time < message.Time)
                        {
                            _objectData[key] = message;
                        }
                    }
                }
            }

            foreach (var kv in _objectData)
            {
                var gameObject = _gameObjectMap[kv.Value.ObjectId];

                var key = MakeKey(kv.Value.ObjectId, kv.Value.SubmeshId, kv.Value.Header);

                if (!_states.ContainsKey(key))
                {
                    _states[key] = new Queue<GameObjectTimeState>();
                }

                var queue = _states[key];
                var newState = new GameObjectTimeState(kv.Value.Position, kv.Value.Rotation, kv.Value.Time);

                if (queue.Count < QueueSize)
                {
                    if (queue.Count == 0 || Vector3.Distance(queue.Last().Position, newState.Position) > 0.01f)
                    {
                        queue.Enqueue(newState);
                    }
                }

                if (queue.Count == QueueSize)
                {
                    float dist = 0.0f;
                    var items = queue.ToList();

                    for (int i = 0; i < (QueueSize - 1); i++)
                    {
                        var prev = items[i];
                        var next = items[i + 1];
                        dist += Vector3.Distance(prev.Position, next.Position);
                    }

                    if (!_velocityValues.ContainsKey(key))
                    {
                        _velocityValues[key] = 0.0f;
                    }

                    var firstState = queue.Dequeue();
                    var lastState = queue.Last();

                    if (dist > 0.0001f)
                    {
                        var deltaTime = lastState.Time - firstState.Time;
                        var velocity = dist / deltaTime;
                        var initialVelocity = _velocityValues[key];
                        var deltaVelocity = velocity - initialVelocity;
                        var acceleration = deltaVelocity / deltaTime;
                        var time = (float)(newState.Time - lastState.Time);

                        var norm = items[QueueSize - 1].Position - items[QueueSize - 2].Position;
                        norm.Normalize();
                        var extPos = lastState.Position + 
                            norm * (initialVelocity * time + 0.5f * acceleration * (float)Math.Pow(time, 2.0f));

                        var newPos = kv.Value.Position;
                        var newRot = kv.Value.Rotation;

                        newPos = new Vector3(0.5f * extPos.X + 0.5f * newPos.X,
                                             0.5f * extPos.Y + 0.5f * newPos.Y,
                                             0.5f * extPos.Z + 0.5f * newPos.Z);

                        switch (kv.Value.Header)
                        {
                            case 1:
                                gameObject.Position = newPos;
                                gameObject.Rotation = newRot;

                                break;
                            case 2:
                                SubMeshTransform transform;
                                uint submeshId = (uint)kv.Value.SubmeshId;

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

                                transform.Position = newPos;
                                transform.Rotation = newRot;

                                break;
                        }

                        _velocityValues[key] = velocity;
                    }

                    queue.Enqueue(newState);
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

        private GameObjectData ReadGameObjectData(byte[] buffer)
        {
            ushort header = BitConverter.ToUInt16(buffer, 0);
            long time = BitConverter.ToInt64(buffer, 2);
            int objectId = BitConverter.ToInt32(buffer, 10);
            int submeshId = BitConverter.ToInt32(buffer, 14);

            float px = BitConverter.ToSingle(buffer, 18);
            float py = BitConverter.ToSingle(buffer, 22);
            float pz = -1.0f * BitConverter.ToSingle(buffer, 26);

            float rx = -1.0f * BitConverter.ToSingle(buffer, 30);
            float ry = -1.0f * BitConverter.ToSingle(buffer, 34);
            float rz = BitConverter.ToSingle(buffer, 38);
            float rw = BitConverter.ToSingle(buffer, 42);

            var message = new GameObjectData()
            {
                Header = header,
                Time = time,
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
        public ushort Header { get; set; }

        public long Time { get; set; }

        public int ObjectId { get; set; }

        public int SubmeshId { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }

    public class GameObjectTimeState
    {
        public GameObjectTimeState(Vector3 position, Quaternion rotation, long time)
        {
            Time = time;
            Position = position;
            Rotation = rotation;
        }

        public long Time { get; set; }

        public float Velocity { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
}