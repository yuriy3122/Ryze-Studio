
using log4net;

namespace RyzeEditor.Packer
{
    public class PackerOptions
    {
        public ILog Logger { get; set; }

        public string TextureFormat { get; set; }

        public string OutputFilePath { get; set; }

        public int PlatformAlignment { get; set; }

        public bool PackTextures { get; set; }

        public bool PackMaterials { get; set; }

        public bool PackPointLights { get; set; }

        public bool PackAccelerationStructures { get; set; }

        public bool PackMeshData { get; set; }

        public bool PackWorldChunkData { get; set; }

        public bool PackHiddenObjects { get; set; }

        public bool ConsoleOutput { get; set; }
    }
}
