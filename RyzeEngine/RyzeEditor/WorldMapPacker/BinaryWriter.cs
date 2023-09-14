using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using SharpDX;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Extentions;
using RyzeEditor.Helpers;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Packer
{
    public class BinaryWriter
    {
        private readonly WorldMapData _worldMapData;
        private PackerOptions _options;
        private CollisionWriter _collisionWriter;
        private VehicleWriter _vehicleWriter;
        private WorldChunkWriter _worldChunkWriter;

        public event EventHandler<PackerEventArgs> NewMessage;

        private long _tmpPos;

        #region contants

        private const ushort ID_HEADER = 0x1FF1;
        private const ushort ID_VERSION = 0x0001;
        private const ushort ID_TEXTURE_CHUNK = 0x0001;
        private const ushort ID_MATERIAL_CHUNK = 0x0002;
        private const ushort ID_MESH_CHUNK = 0x0003;
        private const ushort ID_GAME_OBJ_CHUNK = 0x0006;
        private const ushort ID_POINT_LIGHT_CHUNK = 0x0007;
        private const ushort ID_INDEX_BUFFER_CHUNK = 0x0008;
        private const ushort ID_TEXTURE_FORMAT_PVR = 0x0010;
        private const ushort ID_TEXTURE_FORMAT_JPG = 0x0020;
        private const ushort ID_TEXTURE_BLOCK_CHUNK = 0x0030;
        private const ushort ID_MESH_PATCH_INDEX_CHUNK = 0x0031;
        private const ushort ID_MESH_PATCH_DATA_CHUNK = 0x0032;
        private const ushort ID_VERTEX_POSITION_BUFFER_CHUNK = 0x0033;
        private const ushort ID_VERTEX_TEXNORM_BUFFER_CHUNK = 0x0034;
        private const ushort ID_VERTEX_TANGENT_BUFFER_CHUNK = 0x0035;
        private const ushort ID_FONT_TEXTURE_ATLAS_CHUNK = 0x0036;
        private const ushort ID_SKYBOX_TEXTURE_CHUNK = 0x0037;
        private const ushort ID_SKYBOX_MESH_CHUNK = 0x0038;
        private const ushort ID_ACCELERATION_STRUCTURE = 0x0039;
        private const ushort ID_HIDDEN_OBJ_STRUCTURE = 0x0040;

        #endregion

        public BinaryWriter(WorldMapData worldMapData)
        {
            _worldMapData = worldMapData;
        }

        protected virtual void OnNewMessage(PackerEventArgs e)
        {
            NewMessage?.Invoke(this, e);
        }

        public void WriteData(PackerOptions options)
        {
            OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === scene packing started"));

            _options = options;

            _collisionWriter = new CollisionWriter(_worldMapData, _options);
            _vehicleWriter = new VehicleWriter(_worldMapData, _options);
            _worldChunkWriter = new WorldChunkWriter(_worldMapData);

            using (var stream = File.Open(options.OutputFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(BitConverter.GetBytes(ID_HEADER), 0, sizeof(ushort));
                stream.Write(BitConverter.GetBytes(ID_VERSION), 0, sizeof(ushort));

                //texture block ptr
                _tmpPos = stream.Position;
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(int));

                stream.Write(BitConverter.GetBytes(_worldMapData.Textures.Count), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_worldMapData.Materials.Count), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_worldMapData.Meshes.Count), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(_worldMapData.GameObjects.Count), 0, sizeof(int));

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing material data...complete."));
                WriteMaterialData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing mesh data...complete."));
                WriteMeshData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing scene object data...complete."));
                WriteGameObjectData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing point light data...complete."));
                WritePointLightData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing collision data...complete."));
                _collisionWriter.WriteData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing vehicle data...complete."));
                _vehicleWriter.WriteData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing world map chunk data...complete."));
                _worldChunkWriter.WriteData(stream);

                WriteFontTextureAtlasData(stream);

                WriteSkyboxCubeTextureData(stream);

                WriteSkyboxMeshData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing acceleration structure data"));
                WriteAccelerationStructureData(stream);

                WriteHiddenObjectsData(stream);

                OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === writing texture data"));
                WriteTextureData(stream);
            }

            _collisionWriter.Dispose();

            OnNewMessage(new PackerEventArgs($"{DateTime.UtcNow:mm:ss} === scene packing complete"));
        }

        private void WriteAccelerationStructureData(FileStream stream)
        {
            var gameObjects = _worldMapData.GameObjects.Where(x => x.Key.AccelerationStructure).Select(x => x.Key).ToList();

            if (gameObjects.Any())
            {
                var ids = new List<int>();

                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.UserData != null && int.TryParse(gameObject.UserData.ToString(), out int id))
                    {
                        ids.Add(id);
                    }
                }

                if (ids.Count > 0)
                {
                    stream.Write(BitConverter.GetBytes(ID_ACCELERATION_STRUCTURE), 0, sizeof(ushort));
                    stream.Write(BitConverter.GetBytes(ids.Count), 0, sizeof(int));

                    foreach (var id in ids)
                    {
                        stream.Write(BitConverter.GetBytes(id), 0, sizeof(int));
                    }
                }
            }
        }

        private void WriteHiddenObjectsData(FileStream stream)
        {
            var gameObjects = _worldMapData.GameObjects.Where(x => x.Key.IsHidden).Select(x => x.Key).ToList();

            if (gameObjects.Any())
            {
                var ids = new List<int>();

                foreach (var gameObject in gameObjects)
                {
                    if (gameObject.UserData != null && int.TryParse(gameObject.UserData.ToString(), out int id))
                    {
                        ids.Add(id);
                    }
                }

                if (ids.Count > 0)
                {
                    stream.Write(BitConverter.GetBytes(ID_HIDDEN_OBJ_STRUCTURE), 0, sizeof(ushort));
                    stream.Write(BitConverter.GetBytes(ids.Count), 0, sizeof(int));

                    foreach (var id in ids)
                    {
                        stream.Write(BitConverter.GetBytes(id), 0, sizeof(int));
                    }
                }
            }
        }

        private void WriteSkyboxMeshData(FileStream stream)
        {
            if (_worldMapData.SkyboxMesh != null)
            {
                stream.Write(BitConverter.GetBytes(ID_SKYBOX_MESH_CHUNK), 0, sizeof(ushort));

                int index = _worldMapData.Meshes[_worldMapData.SkyboxMesh];

                stream.Write(BitConverter.GetBytes(index), 0, sizeof(int));
            }
        }

        private void WriteSkyboxCubeTextureData(FileStream stream)
        {
            if (_worldMapData.SkyboxTextures == null || _worldMapData.SkyboxTextures.Count == 0)
            {
                return;
            }

            stream.Write(BitConverter.GetBytes(ID_SKYBOX_TEXTURE_CHUNK), 0, sizeof(ushort));

            foreach (var skyboxTexture in _worldMapData.SkyboxTextures)
            {
                int index = _worldMapData.Textures[skyboxTexture];

                stream.Write(BitConverter.GetBytes(index), 0, sizeof(int));
            }
        }

        private void WriteFontTextureAtlasData(FileStream stream)
        {
            foreach (var fontTextureAtlas in _worldMapData.FontTextureAtlases)
            {
                stream.Write(BitConverter.GetBytes(ID_FONT_TEXTURE_ATLAS_CHUNK), 0, sizeof(ushort));

                const int bytesCount = 64;
                var bytes = Encoding.UTF8.GetBytes(fontTextureAtlas.FontName);

                if (bytesCount - bytes.Length > 0)
                {
                    stream.Write(bytes, 0, bytes.Length);
                    var buffer = new byte[bytesCount - bytes.Length];
                    stream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    throw new ArgumentException("Exceeded font name len: 50 char maximum");
                }

                stream.Write(BitConverter.GetBytes((int)fontTextureAtlas.TextureId), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)fontTextureAtlas.Columns), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)fontTextureAtlas.Rows), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)fontTextureAtlas.DeviceResolution.Height), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes((int)fontTextureAtlas.DeviceResolution.Width), 0, sizeof(int));
            }
        }

        private void WriteGameObjectData(FileStream stream)
        {
            int i = 1;

            foreach (var gameObject in _worldMapData.GameObjects)
            {
                var obj = gameObject.Key as GameObject;

                if (obj == null)
                {
                    continue;
                }

                obj.UserData = i++;

                stream.Write(BitConverter.GetBytes(ID_GAME_OBJ_CHUNK), 0, sizeof(ushort));    //Chunk Header

                var position = new Vector3(obj.Position.X, obj.Position.Y, obj.Position.Z);
                position.Z = -1.0f * position.Z;

                var rotation = obj.Rotation;
                rotation.X = -rotation.X;
                rotation.Y = -rotation.Y;

                stream.Write(rotation.GetBytes(), 0, 4 * sizeof(float));                       //Rotation
                stream.Write(obj.Scale.GetBytes(), 0, 3 * sizeof(float));                      //Scale
                stream.Write(position.GetBytes(), 0, 3 * sizeof(float));                       //Position

                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));                      //Reserve for 64-bit pointer
                stream.Write(BitConverter.GetBytes(obj.GeometryMeshes.Count), 0, sizeof(int)); //Geometry mesh count
                stream.Write(BitConverter.GetBytes((int)obj.UserData), 0, sizeof(int));        //GameObject Id

                foreach (var mesh in obj.GeometryMeshes)
                {
                    int meshId = _worldMapData.Meshes[mesh];
                    stream.Write(BitConverter.GetBytes(meshId), 0, sizeof(int));
                }
            }
        }

        private void WritePointLightData(FileStream stream)
        {
            stream.Write(BitConverter.GetBytes(ID_POINT_LIGHT_CHUNK), 0, sizeof(ushort)); //Chunk Header

            var pointLights = new List<PointLight>();

            foreach (var gameObject in _worldMapData.GameObjects)
            {
                if (gameObject.Key.GetType() == typeof(PointLight))
                {
                    pointLights.Add((PointLight)gameObject.Key);
                }
            }

            int alignment = _options.PlatformAlignment;

            const int pointLightStructSizeInBytes = 112;
            int pointLightBufferSize = pointLights.Count * pointLightStructSizeInBytes;
            pointLightBufferSize += alignment - pointLightBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(pointLights.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(pointLightBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

            stream.Position += offset;

            foreach (var pointLight in pointLights)
            {
                byte[] bytes = new byte[16 * 4];
                stream.Write(bytes, 0, bytes.Length); // reserve for matrix_float4x4 view_projection_matrix

                stream.Write(pointLight.Color.GetBytes(), 0, 3 * sizeof(float));
                stream.Write(BitConverter.GetBytes(0.0f), 0, sizeof(float));

                stream.Write(pointLight.Position.GetBytes(), 0, 3 * sizeof(float));
                stream.Write(BitConverter.GetBytes(0.0f), 0, sizeof(float));

                stream.Write(pointLight.Direction.GetBytes(), 0, 3 * sizeof(float));
                stream.Write(BitConverter.GetBytes(0.0f), 0, sizeof(float));

                stream.Write(BitConverter.GetBytes(pointLight.Radius), 0, sizeof(float));
                stream.Write(BitConverter.GetBytes(pointLight.Intensity), 0, sizeof(float));

                var scale = pointLight.Radius / pointLight.GeometryMeshes[0].BoundingSphere.Radius;
                stream.Write(BitConverter.GetBytes(scale), 0, sizeof(float));

                var mesh = pointLight.GeometryMeshes[0];
                int meshId = _worldMapData.Meshes[mesh];
                stream.Write(BitConverter.GetBytes(meshId), 0, sizeof(int));
            }

            stream.Position += pointLightBufferSize - pointLights.Count * pointLightStructSizeInBytes;
        }

        private void WriteVertexPositionBufferData(FileStream stream, List<Vertex> vertexBuffer)
        {
            stream.Write(BitConverter.GetBytes(ID_VERTEX_POSITION_BUFFER_CHUNK), 0, sizeof(ushort));

            int alignment = _options.PlatformAlignment;

            int vertexPositionSize = 4 * sizeof(float);
            int vertexBufferSize = vertexBuffer.Count * vertexPositionSize;
            vertexBufferSize += alignment - vertexBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(vertexBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(vertexBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

            stream.Position += offset;

            foreach (var vertex in vertexBuffer)
            {
                stream.Write(vertex.GetPositionBytes(), 0, vertexPositionSize);
            }

            stream.Position += vertexBufferSize - vertexBuffer.Count * vertexPositionSize;
        }

        private void WriteVertexTexNormBufferData(FileStream stream, List<Vertex> vertexBuffer)
        {
            stream.Write(BitConverter.GetBytes(ID_VERTEX_TEXNORM_BUFFER_CHUNK), 0, sizeof(ushort));

            int alignment = _options.PlatformAlignment;

            int vertexTexNormSize = 2 * sizeof(float) + 4 * sizeof(ushort);
            int vertexBufferSize = vertexBuffer.Count * vertexTexNormSize;
            vertexBufferSize += alignment - vertexBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(vertexBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(vertexBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

            stream.Position += offset;

            foreach (var vertex in vertexBuffer)
            {
                stream.Write(vertex.GetTexNormBytes(), 0, vertexTexNormSize);
            }

            stream.Position += vertexBufferSize - vertexBuffer.Count * vertexTexNormSize;
        }

        private void WriteVertexTangentBufferData(FileStream stream, List<Vertex> vertexBuffer)
        {
            stream.Write(BitConverter.GetBytes(ID_VERTEX_TANGENT_BUFFER_CHUNK), 0, sizeof(ushort));

            int alignment = _options.PlatformAlignment;

            int vertexTangentSize = 4 * sizeof(ushort) + 4 * sizeof(ushort);
            int vertexBufferSize = vertexBuffer.Count * vertexTangentSize;
            vertexBufferSize += alignment - vertexBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(vertexBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(vertexBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

            stream.Position += offset;

            foreach (var vertex in vertexBuffer)
            {
                stream.Write(vertex.GetTangentBytes(), 0, vertexTangentSize);
            }

            stream.Position += vertexBufferSize - vertexBuffer.Count * vertexTangentSize;
        }

        private void WriteIndexBufferData(FileStream stream, List<uint> indexBuffer)
        {
            stream.Write(BitConverter.GetBytes(ID_INDEX_BUFFER_CHUNK), 0, sizeof(ushort));

            int alignment = _options.PlatformAlignment;
            int indexBufferSize = indexBuffer.Count * sizeof(uint);
            indexBufferSize += alignment - indexBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(indexBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(indexBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

            stream.Position += offset;

            foreach (uint index in indexBuffer)
            {
                stream.Write(BitConverter.GetBytes(index), 0, sizeof(uint));
            }

            stream.Position += indexBufferSize - indexBuffer.Count * sizeof(uint);
        }

        private void WriteSubMeshData(FileStream stream, Dictionary<IMesh, List<SubMeshData>> subMeshes)
        {
            var patchIndex = 0;

            foreach (var mesh in _worldMapData.Meshes)
            {
                stream.Write(BitConverter.GetBytes(ID_MESH_CHUNK), 0, sizeof(ushort));
                stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long)); //reserve for 64-bit pointer

                Vector3 min = mesh.Key.BoundingBox.Minimum;
                min.Z = -1.0f * min.Z;

                Vector3 max = mesh.Key.BoundingBox.Maximum;
                max.Z = -1.0f * max.Z;

                stream.Write(min.GetBytes(), 0, 3 * sizeof(float));
                stream.Write(max.GetBytes(), 0, 3 * sizeof(float));
                stream.Write(BitConverter.GetBytes(subMeshes[mesh.Key].Count), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(mesh.Value), 0, sizeof(int));

                foreach (var subMesh in subMeshes[mesh.Key])
                {
                    var position = new Vector3(subMesh.Position.X, subMesh.Position.Y, -1.0f * subMesh.Position.Z);
                    var scale = subMesh.Scale;
                    var rotation = subMesh.Rotation;

                    subMesh.PatchIndexBufferOffset = patchIndex;

                    stream.Write(rotation.GetBytes(), 0, 4 * sizeof(float));
                    stream.Write(scale.GetBytes(), 0, 3 * sizeof(float));
                    stream.Write(position.GetBytes(), 0, 3 * sizeof(float));
                    stream.Write(BitConverter.GetBytes(0L), 0, sizeof(long));
                    stream.Write(BitConverter.GetBytes(subMesh.Id), 0, sizeof(uint));
                    stream.Write(BitConverter.GetBytes(subMesh.ParentId), 0, sizeof(uint));
                    stream.Write(BitConverter.GetBytes(subMesh.IndexCount), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(subMesh.IndexBufferOffset), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(subMesh.PatchIndexBufferOffset), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(subMesh.MaterialId), 0, sizeof(int));

                    var tessFactor = new Half(subMesh.TessellationFactor);
                    stream.Write(BitConverter.GetBytes(tessFactor.RawValue), 0, sizeof(ushort));

                    //Struct padding - 6 bytes
                    stream.Write(BitConverter.GetBytes(0), 0, sizeof(int));
                    ushort padding = 0;
                    stream.Write(BitConverter.GetBytes(padding), 0, sizeof(ushort));

                    if (subMesh.TessellationFactor > 0)
                    {
                        patchIndex += (subMesh.IndexCount / 3);
                    }
                }
            }
        }

        private void WriteSubMeshPatchData(FileStream stream, Dictionary<IMesh, List<SubMeshData>> subMeshes, List<uint> indexBuffer, List<Vertex> vertexBuffer)
        {
            var patchIndexBuffer = new List<int>();
            var patchDataBuffer = new List<PatchData>();

            foreach (var mesh in _worldMapData.Meshes)
            {
                foreach (var subMesh in subMeshes[mesh.Key])
                {
                    if (subMesh.TessellationFactor > 0)
                    {
                        for (int i = 0; i < subMesh.IndexCount / 3; i++)
                        {
                            patchIndexBuffer.Add(subMesh.PatchIndexBufferOffset + i);

                            uint i0 = indexBuffer[subMesh.IndexBufferOffset + 3 * i + 0];
                            uint i1 = indexBuffer[subMesh.IndexBufferOffset + 3 * i + 1];
                            uint i2 = indexBuffer[subMesh.IndexBufferOffset + 3 * i + 2];

                            Vertex v0 = vertexBuffer[(int)i0];
                            Vertex v1 = vertexBuffer[(int)i1];
                            Vertex v2 = vertexBuffer[(int)i2];

                            var patchData = PatchHelper.CalculatePatchDataForTriangle(v0, v1, v2);

                            //Optimization: prevent multiplication at runtime, shaders code.
                            patchData.Positions[0] *= 3.0f;
                            patchData.Positions[1] *= 3.0f;
                            patchData.Positions[2] *= 3.0f;
                            patchData.Positions[3] *= 3.0f;
                            patchData.Positions[4] *= 3.0f;
                            patchData.Positions[5] *= 3.0f;
                            patchData.Positions[6] *= 6.0f;

                            //PatchData Test
                            //var vertex4 = PatchHelper.TestPatchData(v0, v1, v2, patchData, new Vector3(1.0f/3.0f, 1.0f / 3.0f, 1.0f / 3.0f));

                            patchDataBuffer.Add(patchData);
                        }
                    }
                }
            }

            //Write patchIndexBuffer data
            stream.Write(BitConverter.GetBytes(ID_MESH_PATCH_INDEX_CHUNK), 0, sizeof(ushort));

            int alignment = _options.PlatformAlignment;
            int patchIndexBufferSize = patchIndexBuffer.Count * sizeof(int);
            patchIndexBufferSize += alignment - patchIndexBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(patchIndexBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(patchIndexBufferSize), 0, sizeof(int));

            int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));
            stream.Position += offset;

            foreach (int patchIndex in patchIndexBuffer)
            {
                stream.Write(BitConverter.GetBytes(patchIndex), 0, sizeof(int));
            }

            stream.Position += patchIndexBufferSize - patchIndexBuffer.Count * sizeof(int);

            //Write patchIndexDataBuffer data
            stream.Write(BitConverter.GetBytes(ID_MESH_PATCH_DATA_CHUNK), 0, sizeof(ushort));

            int patchDataBufferSize = patchDataBuffer.Count * PatchData.SizeInBytes;
            patchDataBufferSize += alignment - patchDataBufferSize % alignment;

            stream.Write(BitConverter.GetBytes(patchDataBuffer.Count), 0, sizeof(int));
            stream.Write(BitConverter.GetBytes(patchDataBufferSize), 0, sizeof(int));

            offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);
            stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));
            stream.Position += offset;

            foreach (var patchData in patchDataBuffer)
            {
                stream.Write(patchData.GetBytes(), 0, PatchData.SizeInBytes);
            }

            stream.Position += patchDataBufferSize - patchDataBuffer.Count * PatchData.SizeInBytes;
        }

        private void WriteMeshData(FileStream stream)
        {
            var vertexBuffer = new List<Vertex>();
            var indexBuffer = new List<uint>();
            var subMeshes = new Dictionary<IMesh, List<SubMeshData>>();
            var uidList = new Dictionary<Guid, List<SubMeshData>>();

            foreach (var mesh in _worldMapData.Meshes)
            {
                var subMeshDataList = new List<SubMeshData>();

                subMeshes[mesh.Key] = subMeshDataList;

                foreach (var subMesh in mesh.Key.SubMeshes)
                {
                    var subMeshGeometry = _worldMapData.SubMeshGeometryList[subMesh];
                    uint offset = Convert.ToUInt32(vertexBuffer.Count);

                    if (!uidList.ContainsKey(subMeshGeometry.Uid))
                    {
                        uidList[subMeshGeometry.Uid] = new List<SubMeshData>();
                        vertexBuffer.AddRange(subMeshGeometry.VertexData);

                        for (int i = 0; i < subMesh.Materials.Count; i++)
                        {
                            if (!subMeshGeometry.Indices.ContainsKey(i))
                            {
                                continue;
                            }

                            var indices = new List<uint>(subMeshGeometry.Indices[i]);

                            for (int j = 0; j < indices.Count; j++)
                            {
                                indices[j] += offset;
                            }

                            int indexOffset = indexBuffer.Count;

                            indexBuffer.AddRange(indices);

                            var subMeshData = new SubMeshData
                            {
                                Id = subMesh.Id,
                                ParentId = subMesh.ParentId,
                                IndexBufferOffset = indexOffset,
                                IndexCount = indices.Count,
                                MaterialId = _worldMapData.Materials[subMesh.Materials[i]],
                                Scale = subMesh.Scale,
                                Rotation = subMesh.RotationRH,
                                Position = subMesh.Position,
                                TessellationFactor = subMesh.TessellationFactor
                            };

                            subMeshDataList.Add(subMeshData);
                            uidList[subMeshGeometry.Uid].Add(subMeshData);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < subMesh.Materials.Count; i++)
                        {
                            var subMeshData = new SubMeshData
                            {
                                Id = subMesh.Id,
                                ParentId = subMesh.ParentId,
                                IndexBufferOffset = uidList[subMeshGeometry.Uid][i].IndexBufferOffset,
                                IndexCount = uidList[subMeshGeometry.Uid][i].IndexCount,
                                MaterialId = _worldMapData.Materials[subMesh.Materials[i]],
                                Scale = subMesh.Scale,
                                Rotation = subMesh.RotationRH,
                                Position = subMesh.Position,
                                TessellationFactor = subMesh.TessellationFactor
                            };

                            subMeshDataList.Add(subMeshData);
                        }
                    }

                    if (subMesh.Materials.Count == 0)
                    {
                        var subMeshData = new SubMeshData
                        {
                            Id = subMesh.Id,
                            ParentId = subMesh.ParentId,
                            Scale = subMesh.Scale,
                            Rotation = subMesh.RotationRH,
                            Position = subMesh.Position
                        };

                        subMeshDataList.Add(subMeshData);
                    }
                }
            }

            for (int i = 0; i < vertexBuffer.Count; i++)
            {
                //Convert to right handed coordinate system
                var pos = vertexBuffer[i].Pos;
                pos.Z = -1.0f * pos.Z;

                var norm = vertexBuffer[i].Norm;
                norm.Z = -1.0f * norm.Z;

                var tex = vertexBuffer[i].Tex;

                var tangent = vertexBuffer[i].Tangent;
                tangent.Z = -1.0f * tangent.Z;

                var bitangent = vertexBuffer[i].Bitangent;
                bitangent.Z = -1.0f * bitangent.Z;

                vertexBuffer[i] = new Vertex(pos, norm, tangent, bitangent, tex);
            }

            WriteVertexPositionBufferData(stream, vertexBuffer);

            WriteVertexTexNormBufferData(stream, vertexBuffer);

            WriteVertexTangentBufferData(stream, vertexBuffer);

            WriteIndexBufferData(stream, indexBuffer);

            WriteSubMeshData(stream, subMeshes);

            WriteSubMeshPatchData(stream, subMeshes, indexBuffer, vertexBuffer);
        }

        private void WriteMaterialData(FileStream stream)
        {
            foreach (var material in _worldMapData.Materials)
            {                
                stream.Write(BitConverter.GetBytes(ID_MATERIAL_CHUNK), 0, sizeof(ushort));//Chunk Header                           
                stream.Write(BitConverter.GetBytes(material.Value), 0, sizeof(int));      //Id 
                
                long tmp = stream.Position;                                               //Data Lenght
                stream.Write(BitConverter.GetBytes(0), 0, sizeof(int));
                var mtl = (Material)material.Key;
                   
                stream.Write(mtl.Ambient.GetBytes(), 0, 3 * sizeof(float));               //Ambient                      
                stream.Write(mtl.Diffuse.GetBytes(), 0, 3 * sizeof(float));               //Diffuse                    
                stream.Write(mtl.Specular.GetBytes(), 0, 3 * sizeof(float));              //Specular                 
                stream.Write(BitConverter.GetBytes(mtl.Shininess), 0, sizeof(float));     //Shininess

                float transparency = 1.0f - mtl.Transparency;

                ITexture diffuseTexture = ((Material)material.Key).DiffuseTexture;        //DiffuseTexture

                if (diffuseTexture != null)
                {
                    var texture = ResourceManager.Instance.GetTexture(diffuseTexture.Id);

                    byte alpha = 255;

                    using (var memStream = new MemoryStream(texture.Data))
                    {
                        using (var bitmap = new Bitmap(memStream))
                        {
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                for (int y = 0; y < bitmap.Height; y++)
                                {
                                    var pixelColor = bitmap.GetPixel(x, y);
                                    alpha = Math.Min(pixelColor.A, alpha);
                                }
                            }
                        }
                    }

                    if (alpha < 255)
                    {
                        transparency = alpha / 255.0f;
                    }
                }

                stream.Write(BitConverter.GetBytes(transparency), 0, sizeof(float));      //Transparency          

                int diffuseTextureId = 0;
                if (diffuseTexture != null && _worldMapData.Textures.ContainsKey(diffuseTexture))
                {
                    diffuseTextureId = _worldMapData.Textures[diffuseTexture];
                }

                ITexture normalTexture = ((Material)material.Key).NormalTexture;          //NormalTexture

                int normalTextureId = 0;
                if (normalTexture != null && _worldMapData.Textures.ContainsKey(normalTexture))
                {
                    normalTextureId = _worldMapData.Textures[normalTexture];
                }

                ITexture specularTexture = ((Material)material.Key).SpecularTexture;      //SpecularTexture

                int specularTextureId = 0;
                if (specularTexture != null && _worldMapData.Textures.ContainsKey(specularTexture))
                {
                    specularTextureId = _worldMapData.Textures[specularTexture];
                }

                stream.Write(BitConverter.GetBytes(diffuseTextureId), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(normalTextureId), 0, sizeof(int));
                stream.Write(BitConverter.GetBytes(specularTextureId), 0, sizeof(int));
                long pos = stream.Position;
                stream.Position = tmp;

                stream.Write(BitConverter.GetBytes((int)(pos - tmp)), 0, sizeof(int));

                stream.Position = pos;
            }
        }

        private void WriteTextureData(Stream stream)
        {
            long pos = stream.Position;
            stream.Position = _tmpPos;
            stream.Write(BitConverter.GetBytes((int)pos), 0, sizeof(int));
            stream.Position = pos;

            stream.Write(BitConverter.GetBytes(ID_TEXTURE_BLOCK_CHUNK), 0, sizeof(ushort));//Chunk Header   

            foreach (var texture in _worldMapData.Textures)
            {
                var binaryData = texture.Key.Data;
                var format = (int)ID_TEXTURE_FORMAT_JPG;

                var tex = ResourceManager.Instance.GetTexture(string.Format("{0}.pvr", texture.Key.Id.Split('.')[0]));

                if (tex != null)
                {
                    binaryData = tex.Data;
                    format = ID_TEXTURE_FORMAT_PVR;
                }
                
                stream.Write(BitConverter.GetBytes(ID_TEXTURE_CHUNK), 0, sizeof(ushort));   //Chunk Header                
                stream.Write(BitConverter.GetBytes(texture.Value), 0, sizeof(int));         //Id
                stream.Write(BitConverter.GetBytes(binaryData.Length), 0, sizeof(int));     //Data Lenght                      
                stream.Write(BitConverter.GetBytes(format), 0, sizeof(int));                //Texture Format

                int alignment = _options.PlatformAlignment;
                int offset = (int)(alignment - (stream.Position + sizeof(int)) % alignment);//Offset
                stream.Write(BitConverter.GetBytes(offset), 0, sizeof(int));

                stream.Position += offset;

                stream.Write(binaryData, 0, binaryData.Length);                              //Data
            }
        }

        internal class SubMeshData
        {
            public uint Id { get; set; }

            public uint ParentId { get; set; }

            public int IndexCount { get; set; }

            public int IndexBufferOffset { get; set; }

            public int PatchIndexBufferOffset { get; set; }

            public int MaterialId { get; set; }

            public Vector3 Scale { get; set; }

            public Quaternion Rotation { get; set; }

            public Vector3 Position { get; set; }

            public int TessellationFactor { get; set; }
        }
    }
}