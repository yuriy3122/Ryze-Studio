namespace RyzeEditor.ResourceManagment
{
	public interface ITexture
	{
		string Id { get; set; }

		byte[] Data { get; set; }
	}
}
