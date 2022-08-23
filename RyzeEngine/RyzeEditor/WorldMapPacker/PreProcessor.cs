using System.IO;
using RyzeEditor.Properties;

namespace RyzeEditor.Packer
{
    public class PreProcessor
    {
        private readonly PackerOptions _options;

        public PreProcessor(PackerOptions options)
        {
            _options = options;
        }

        public void Run()
        {
            ProcessTextures();
        }

        private void ProcessTextures()
        {
            var textureCompressor = TextureCompressor.Create(_options.TextureFormat);

            var path = Path.Combine(Settings.Default.StoragePath, "Textures");

            var di = new DirectoryInfo(path);

            foreach (var file in di.GetFiles())
            {
                var texFile = Path.ChangeExtension(file.FullName, string.Format(".{0}", Settings.Default.TextureFormat.ToLower()));

                if (File.Exists(texFile))
                {
                    continue;
                }

                int mipmapLevel = texFile.ToLowerInvariant().Contains("skybox") ? 1 : 4;

                textureCompressor.Compress(file.FullName, mipmapLevel);
            }
        }
    }
}