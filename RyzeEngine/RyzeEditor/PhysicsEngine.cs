using System;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using RyzeEditor.GameWorld;

namespace RyzeEditor
{
    public class PhysicsEngine
    {
        private readonly Dictionary<int, GameObject> _gameObjectMap;
        private readonly ConcurrentStack<string> _stack;
        private const int UdpPort = 11000;

        private bool _needUpdate;

        public PhysicsEngine()
        {
            _gameObjectMap = new Dictionary<int, GameObject>();
            _stack = new ConcurrentStack<string>();

            ReceiveMessage();

            _needUpdate = true;
        }

        public void Update()
        {
            _needUpdate = true;
        }

        public void SuspendSimulation()
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

        public void StepSimulation(IEnumerable<EntityBase> entities)
        {
            if (_needUpdate)
            {
                CleanUpDynamicsWorldData();
                InitDynamicsWorldData(entities);
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

        private void InitDynamicsWorldData(IEnumerable<EntityBase> entities)
        {
            _gameObjectMap?.Clear();
        }

        public void Dispose()
        {
            CleanUpDynamicsWorldData();
        }
    }
}
