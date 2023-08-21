using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RyzeEditor.Helpers;
using RyzeEditor.Properties;

namespace RyzeEditor.ResourceManagment
{
	public class ResourceManager : IResourceManager
	{
		private readonly Dictionary<string, WeakReference> _texDictionary = new Dictionary<string, WeakReference>();
		private readonly Dictionary<string, WeakReference> _meshDictionary = new Dictionary<string, WeakReference>();

		private static volatile ResourceManager _instance;
		private static readonly object _syncRoot = new object();

		private ResourceManager()
		{
		}

		public static ResourceManager Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				lock (_syncRoot)
				{
					if (_instance == null)
					{
						_instance = new ResourceManager();
					}
				}

				return _instance;
			}
		}

		public ITexture GetTexture(string textureId)
		{
            ITexture loadTexture(string id)
            {
                var storage = StorageFactory.GetStorage(Settings.Default.StorageType);

                var data = storage.GetData(ResourceType.Texture, id);

                if (data == null)
                {
                    return null;
                }

                var texture = new Texture { Id = id, Data = data };

                _texDictionary[id] = new WeakReference(texture);

                return texture;
            }

            if (_texDictionary.ContainsKey(textureId) == false)
			{
				loadTexture(textureId);
			}

            if (!_texDictionary.ContainsKey(textureId))
            {
                return null;
            }

			var target = _texDictionary[textureId].Target;

			if (target == null)
			{
                target = loadTexture(textureId);
			}

			return (ITexture)target;
		}

		public IMesh GetMesh(string meshId)
		{
            IMesh loadMesh(string id)
            {
                var mesh = GeometryShape.FindShape(id);

                if (mesh == null)
                {
                    var storage = StorageFactory.GetStorage(Settings.Default.StorageType);

                    using (var stream = storage.GetStream(ResourceType.Mesh, meshId))
                    {
                        mesh = GeometryMesh.FromStream(stream);
                        mesh.Id = meshId;
                        mesh.Name = id.Split('.').FirstOrDefault();
                    }
                }

                _meshDictionary[id] = new WeakReference(mesh);

                return mesh;
            }

            if (_meshDictionary.ContainsKey(meshId) == false)
			{
				loadMesh(meshId);
			}

            var target = _meshDictionary[meshId].Target;

			if (target == null)
			{
                target = loadMesh(meshId);
			}

			return (IMesh)target;
		}

		public IEnumerable<string> GetMeshIdList()
		{
			var files = Directory.GetFiles(Path.Combine(Settings.Default.StoragePath, "Meshes"), "*.rz").ToList();

			return files.Select(Path.GetFileName);
		}

        public IEnumerable<string> GetTextureIdList()
        {
            var files = Directory.GetFiles(Path.Combine(Settings.Default.StoragePath, "Textures"), "*.pvr").ToList();

            return files.Select(Path.GetFileName);
        }

        public bool IsReclaimed(string resourceId)
        {
            if (_texDictionary.ContainsKey(resourceId) && _texDictionary[resourceId].IsAlive == false)
            {
                return true;
            }

            if (_meshDictionary.ContainsKey(resourceId) && _meshDictionary[resourceId].IsAlive == false)
            {
                return true;
            }

            return false;
        }
    }
}
