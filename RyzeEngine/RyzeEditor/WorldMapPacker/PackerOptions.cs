
using log4net;

namespace RyzeEditor.Packer
{
    public class PackerOptions
    {
        public ILog Logger { get; set; }

        public string TextureFormat { get; set; }

        public string OutputFilePath { get; set; }

        public int PlatformAlignment { get; set; }
    }
}
