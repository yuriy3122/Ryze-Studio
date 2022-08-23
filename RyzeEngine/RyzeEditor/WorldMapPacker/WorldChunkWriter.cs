using System;
using SharpDX;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using RyzeEditor.GameWorld;
using RyzeEditor.Extentions;

namespace RyzeEditor.Packer
{
    public class WorldChunkWriter
    {
        private readonly WorldMapData _worldMapData;

        private const ushort ID_WORLD_SECTOR_CHUNK = 0x0090;

        private Vector3 GameWorldHalfExtents = new Vector3(1000.0f, 1000.0f, 1000.0f);

        private Vector3 GameWorldChunkExtents = new Vector3(100.0f, 100.0f, 100.0f);

        private readonly Dictionary<Tuple<int, int, int>, List<int>> _chunkObjects = new Dictionary<Tuple<int, int, int>, List<int>>();

        public WorldChunkWriter(WorldMapData worldMapData)
        {
            _worldMapData = worldMapData;
        }

        public void WriteData(Stream stream)
        {
            _chunkObjects.Clear();

            var dynamicObjects = _worldMapData.RigidBodies.Where(x => x.Mass > 0).Select(x => x.GameObject.Id).ToList();

            foreach (var gameObject in _worldMapData.GameObjects)
            {
                if (gameObject.Key is Vehicle vehicle)
                {
                    dynamicObjects.Add(gameObject.Key.Id);
                }
            }

            var staticObjects = _worldMapData.GameObjects.Where(x => !dynamicObjects.Contains(x.Key.Id)).ToList();

            foreach (var staticObject in staticObjects)
            {
                var pos = staticObject.Key.Position / GameWorldChunkExtents;

                var key = new Tuple<int, int, int>((int)pos.X, (int)pos.Y, (int)pos.Z);

                if (!_chunkObjects.ContainsKey(key))
                {
                    _chunkObjects[key] = new List<int>();
                }

                _chunkObjects[key].Add((int)staticObject.Key.UserData);

                for (int i = (key.Item1 - 1); i <= (key.Item1 + 1); i++)
                {
                    for (int j = (key.Item2 - 1); j <= (key.Item2 + 1); j++)
                    {
                        for (int k = (key.Item3 - 1); k <= (key.Item3 + 1); k++)
                        {
                            if (i == key.Item1 && j == key.Item2 && k == key.Item3)
                            {
                                continue;
                            }

                            var minX = i * GameWorldChunkExtents.X;
                            var minY = j * GameWorldChunkExtents.Y;
                            var minZ = k * GameWorldChunkExtents.Z;

                            var maxX = minX + GameWorldChunkExtents.X;
                            var maxY = minY + GameWorldChunkExtents.Y;
                            var maxZ = minZ + GameWorldChunkExtents.Z;

                            BoundingBox bb = new BoundingBox
                            {
                                Minimum = new Vector3(minX, minY, minZ),
                                Maximum = new Vector3(maxX, maxY, maxZ)
                            };

                            if (bb.Contains(staticObject.Key.BoundingBox) != ContainmentType.Disjoint)
                            {
                                var tmp = new Tuple<int, int, int>(i, j, k);

                                if (!_chunkObjects.ContainsKey(tmp))
                                {
                                    _chunkObjects[tmp] = new List<int>();
                                }

                                _chunkObjects[tmp].Add((int)staticObject.Key.UserData);
                            }
                        }
                    }
                }
            }

            stream.Write(BitConverter.GetBytes(ID_WORLD_SECTOR_CHUNK), 0, sizeof(ushort));  //Header
            stream.Write(GameWorldHalfExtents.GetBytes(), 0, 3 * sizeof(float));            //GameWorld dimentions
            stream.Write(GameWorldChunkExtents.GetBytes(), 0, 3 * sizeof(float));           //GameWorld chunk dimentions
            stream.Write(BitConverter.GetBytes(_chunkObjects.Count), 0, sizeof(int));       //Number of chunks

            foreach (var keyValuePair in _chunkObjects)
            {
                stream.Write(BitConverter.GetBytes(keyValuePair.Key.Item1), 0, sizeof(int));    //X index
                stream.Write(BitConverter.GetBytes(keyValuePair.Key.Item2), 0, sizeof(int));    //Y index
                stream.Write(BitConverter.GetBytes(keyValuePair.Key.Item3), 0, sizeof(int));    //Z index
                stream.Write(BitConverter.GetBytes(keyValuePair.Value.Count), 0, sizeof(int));  //Number of objects in chunk

                foreach (var id in keyValuePair.Value)
                {
                    stream.Write(BitConverter.GetBytes(id), 0, sizeof(int));    //Object Id
                }
            }
        }
    }
}
