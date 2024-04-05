using System;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using RyzeEditor.GameWorld;
using RyzeEditor.Packer;

namespace RyzeEditor
{
    public class ServerClient
    {
        private readonly Dictionary<int, GameObject> _gameObjectMap;
        private readonly ConcurrentStack<string> _stack;
        private const int UdpPort = 11000;
        protected WorldMap _worldMap;

        private bool _needUpdate;

        public ServerClient(WorldMap world)
        {
            _worldMap = world;
            _gameObjectMap = new Dictionary<int, GameObject>();
            _stack = new ConcurrentStack<string>();

            ReceiveMessage();

            _needUpdate = true;
        }

        public void Update()
        {
            _needUpdate = true;
        }

        public void Suspend()
        {
            //Send command to the Server to suspend simulation
        }

        private void ReceiveMessage()
        {
            Task.Run(async () =>
            {
                using (var udpClient = new UdpClient(UdpPort))
                {
                    while (true)
                    {
                        try
                        {
                            var receivedResult = await udpClient.ReceiveAsync();
                            _stack.Push(Encoding.ASCII.GetString(receivedResult.Buffer));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            });
        }

        public void UpdateState()
        {
            if (_needUpdate)
            {
                CleanUpDynamicsWorldData();
                InitDynamicsWorldData();
                _needUpdate = false;
            }

            int count = Math.Min(1000, _stack.Count);

            for (int i = 0; i < count; i++)
            {
                if (_stack.TryPeek(out string message))
                {
                    Console.WriteLine(message);
                }
            }

            _stack.Clear();
        }

        private void CleanUpDynamicsWorldData()
        {
        }

        private void InitDynamicsWorldData()
        {
            _gameObjectMap?.Clear();

            var options = new PackerOptions();
            var packer = new WorldMapPacker(_worldMap, options);
            packer.Execute();
        }

        public void Dispose()
        {
            CleanUpDynamicsWorldData();
        }
    }
}
