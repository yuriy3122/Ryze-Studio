using RyzeEditor.GameWorld;
using RyzeEditor.Properties;

namespace RyzeEditor.Packer
{
    public class WorldMapPacker
    {
        private readonly WorldMap _worldMap;

        private readonly PackerOptions _options;

        private readonly PreProcessor _preProcessor;

        private readonly WorldMapData _worldMapData;

        private readonly BinaryWriter _binaryWriter;

        public WorldMapPacker(WorldMap worldMap, PackerOptions options)
        {
            _worldMap = worldMap;
            _options = options;

            CreateDefaultOptions();

            _preProcessor = new PreProcessor(options);
            _worldMapData = new WorldMapData(_worldMap);
            _binaryWriter = new BinaryWriter(_worldMapData);
        }

        public void Execute()
        {
            _preProcessor.Run();

            _worldMapData.Prepare();

            _binaryWriter.WriteData(_options);
        }

        public void CreateDefaultOptions()
        {
            _options.TextureFormat = Settings.Default.TextureFormat;
            _options.OutputFilePath = Settings.Default.OutputFilePath;
            _options.PlatformAlignment = Settings.Default.PlatformAlignment;
        }
    }
}
