using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using RyzeEditor.ResourceManagment;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Packer
{
    public class WorldMapData
    {
        public Dictionary<ITexture, int> Textures { get; private set; }

        public Dictionary<IMaterial, int> Materials { get; private set; }

        public Dictionary<IMesh, int> Meshes { get; private set; }

        public Dictionary<GameObject, int> GameObjects { get; private set; }

        public List<RigidBody> RigidBodies { get; private set; }

        public List<FontTextureAtlas> FontTextureAtlases { get; private set; }

        public Dictionary<SubMesh, SubMeshGeometry> SubMeshGeometryList { get; private set; }

        public IMesh SkyboxMesh { get; private set; }

        public List<ITexture> SkyboxTextures { get; private set; }

        private readonly WorldMap _worldMap;

        public WorldMapData(WorldMap worldMap)
        {
            Textures = new Dictionary<ITexture, int>();

            Materials = new Dictionary<IMaterial, int>();

            Meshes = new Dictionary<IMesh, int>();

            GameObjects = new Dictionary<GameObject, int>();

            SubMeshGeometryList = new Dictionary<SubMesh, SubMeshGeometry>();

            RigidBodies = new List<RigidBody>();

            FontTextureAtlases = new List<FontTextureAtlas>();

            SkyboxTextures = new List<ITexture>();

            _worldMap = worldMap;
        }

        public void Prepare()
        {
            int gameObjectId = 1;
            int meshId = 1;
            int materialId = 1;
            int textureId = 1;

            foreach (var entity in _worldMap.Entities)
            {
                var rigidBody = entity as RigidBody;

                if (rigidBody != null)
                {
                    RigidBodies.Add(rigidBody);
                }

                var gameObject = entity as GameObject;

                if (gameObject == null)
                {
                    continue;
                }

                if (!GameObjects.ContainsKey(gameObject))
                {
                    GameObjects.Add(gameObject, gameObjectId++);
                }

                foreach (var mesh in gameObject.GeometryMeshes)
                {
                    if (!Meshes.ContainsKey(mesh))
                    {
                        Meshes.Add(mesh, meshId++);
                    }

                    foreach (var subMesh in mesh.SubMeshes)
                    {
                        foreach (var material in subMesh.Materials)
                        {
                            if (!Materials.ContainsKey(material))
                            {
                                Materials.Add(material, materialId++);
                            }

                            if (material.DiffuseTexture != null && !Textures.ContainsKey(material.DiffuseTexture))
                            {
                                Textures.Add(material.DiffuseTexture, textureId++);
                            }

                            if (material.NormalTexture != null && !Textures.ContainsKey(material.NormalTexture))
                            {
                                Textures.Add(material.NormalTexture, textureId++);
                            }

                            if (material.SpecularTexture != null && !Textures.ContainsKey(material.SpecularTexture))
                            {
                                Textures.Add(material.SpecularTexture, textureId++);
                            }
                        }

                        ProcessSubMeshGeometry(subMesh);                        
                    }
                }
            }

            ProcessFontTextures(ref textureId);

            ProcessSkyboxTextures(ref textureId);

            ProcessSkyboxMesh(meshId, materialId);
        }

        private void ProcessSkyboxMesh(int meshId, int materialId)
        {
            var meshes = ResourceManager.Instance.GetMeshIdList();
            var skyboxMeshId = meshes.FirstOrDefault(x => x.ToLowerInvariant().Contains("skybox.rz"));

            if (!string.IsNullOrEmpty(skyboxMeshId))
            {
                SkyboxMesh = ResourceManager.Instance.GetMesh(skyboxMeshId);

                if (SkyboxMesh != null)
                {
                    Meshes.Add(SkyboxMesh, meshId++);

                    foreach (var subMesh in SkyboxMesh.SubMeshes)
                    {
                        foreach (var material in subMesh.Materials)
                        {
                            if (!Materials.ContainsKey(material))
                            {
                                Materials.Add(material, materialId++);
                            }
                        }
                    }

                    foreach (var subMesh in SkyboxMesh.SubMeshes)
                    {
                        ProcessSubMeshGeometry(subMesh);
                    }
                }
            }
        }

        private void ProcessSkyboxTextures(ref int textureId)
        {
            var map = new List<string>() { "skybox_px.pvr", "skybox_nx.pvr", "skybox_py.pvr", "skybox_ny.pvr", "skybox_pz.pvr", "skybox_nz.pvr" };

            var textures = ResourceManager.Instance.GetTextureIdList();

            if (textures.Count(x => map.Contains(x)) < map.Count)
            {
                return;
            }

            foreach (var item in map)
            {
                var texture = ResourceManager.Instance.GetTexture(item);

                if (texture != null)
                {
                    SkyboxTextures.Add(texture);
                    Textures.Add(texture, textureId++);
                }
            }
        }

        private void ProcessFontTextures(ref int textureId)
        {
            var textures = ResourceManager.Instance.GetTextureIdList();

            //format: [font name]_fontTextureAtlas_[columns,rows]_[resolutionWidth,resolutionHeight].pvr
            var fontTextureAtlases = textures.Where(x => x.Contains("fontTextureAtlas")).ToList();

            foreach (var fontTextureAtlas in fontTextureAtlases)
            {
                var parts = Path.GetFileNameWithoutExtension(fontTextureAtlas).Split('_');
                var columnsAndRows = parts[2].Split('x');
                var resolution = parts[3].Split('x');

                var atlas = new FontTextureAtlas
                {
                    FontName = parts.First(),
                    TextureId = textureId,
                    Columns = int.Parse(columnsAndRows[0]),
                    Rows = int.Parse(columnsAndRows[1]),
                    DeviceResolution = new Size(int.Parse(resolution[0]), int.Parse(resolution[1]))
                };

                FontTextureAtlases.Add(atlas);

                var texture = ResourceManager.Instance.GetTexture(fontTextureAtlas);

                Textures.Add(texture, textureId++);
            }
        }

        private void ProcessSubMeshGeometry(SubMesh subMesh)
        {
            SubMeshGeometry data = null;

            foreach (var key in SubMeshGeometryList.Keys)
            {
                if (subMesh.VertexData.Count != 0 && key.IsEqualGeometry(subMesh))
                {
                    data = SubMeshGeometryList[key];
                }
            }

            if (data == null)
            {
                data = new SubMeshGeometry()
                {
                    Uid = Guid.NewGuid(),
                    Id = subMesh.Id,
                    ParentId = subMesh.ParentId,
                    VertexData = subMesh.VertexData,
                    Indices = subMesh.Indices,
                    TessellationFactor = subMesh.TessellationFactor
                };
            }

            if (!SubMeshGeometryList.ContainsKey(subMesh))
            {
                SubMeshGeometryList.Add(subMesh, data);
            }
        }

        public void Clear()
        {
            Textures.Clear();

            Materials.Clear();

            Meshes.Clear();

            GameObjects.Clear();
        }
    }

    public class SubMeshGeometry
    {
        public Guid Uid { get; set; }

        public uint Id { get; set; }

        public uint ParentId { get; set; }

        public List<Vertex> VertexData { get; set; }

        public Dictionary<int, List<uint>> Indices { get; set; }

        public int TessellationFactor { get; set; }
    }

    public class FontTextureAtlas
    {
        public string FontName { get; set; }

        public int TextureId { get; set; }

        public int Columns { get; set; }

        public int Rows { get; set; }

        public Size DeviceResolution { get; set; }
    }
}