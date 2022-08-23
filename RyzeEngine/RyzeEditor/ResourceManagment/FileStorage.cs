using System.IO;
using RyzeEditor.Properties;

namespace RyzeEditor.ResourceManagment
{
	public class FileStorage : IResourceStorage
	{
		public byte[] GetData(ResourceType type, string path)
		{
			switch (type)
			{
				case ResourceType.Texture:
                    var texture = Path.Combine(Settings.Default.StoragePath, "Textures", path);

                    if (!File.Exists(texture))
                    {
                        return null;
                    }

                    return File.ReadAllBytes(texture);

				case ResourceType.Mesh:
                    var mesh = Path.Combine(Settings.Default.StoragePath, "Meshes", path);

                    if (!File.Exists(mesh))
                    {
                        return null;
                    }

                    return File.ReadAllBytes(mesh);

				default:
					return null;
			}
		}

		public Stream GetStream(ResourceType type, string path)
		{
			switch (type)
			{
				case ResourceType.Mesh:
					return File.OpenRead(Path.Combine(Settings.Default.StoragePath, "Meshes", path));

				case ResourceType.Texture:
					return File.OpenRead(Path.Combine(Settings.Default.StoragePath, "Textures", path));

				default:
					return null;
			}
		}
	}
}