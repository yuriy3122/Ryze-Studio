using System.IO;

namespace RyzeEditor.ResourceManagment
{
	public enum ResourceType
	{
		Texture,
		Mesh
	}

	public interface IResourceStorage
	{
		byte[] GetData(ResourceType type, string path);

		Stream GetStream(ResourceType type, string path);
	}
}
