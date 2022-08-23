using System;

namespace RyzeEditor.ResourceManagment
{
	[Serializable]
	public class Texture : ITexture
	{
		public string Id { get; set; }

		public byte[] Data { get; set; }
	}
}
