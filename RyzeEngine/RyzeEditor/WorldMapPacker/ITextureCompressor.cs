
namespace RyzeEditor.Packer
{
    public interface ITextureCompressor
    {
        void Compress(string file, int mipmaps);
    }

    public static class TextureCompressor
    {
        public static ITextureCompressor Create(string txtFormat)
        {
            switch (txtFormat.ToUpperInvariant())
            {
                case "PVR":
                    return new PVRTextureCompressor();

                default:
                    return null;
            }
        }
    }
}
