using System;
using System.Windows.Forms;
using log4net;
using RyzeEditor.GameWorld;
using RyzeEditor.Properties;

namespace RyzeEditor.Packer
{
    public class WorldMapPacker
    {
        private readonly WorldMap _worldMap;

        private readonly ILog _logger;

        private readonly PackerOptions _options;

        private readonly PreProcessor _preProcessor;

        private readonly WorldMapData _worldMapData;

        private readonly BinaryWriter _binaryWriter;

        public event EventHandler<PackerEventArgs> NewMessage;

        public event EventHandler<PackerEventArgs> OnComplete;

        public WorldMapPacker(WorldMap worldMap, PackerOptions options)
        {
            _worldMap = worldMap;
            _options = options;
            _logger = options.Logger;

            CreateDefaultOptions();

            _preProcessor = new PreProcessor(options);
            _worldMapData = new WorldMapData(_worldMap);
            _binaryWriter = new BinaryWriter(_worldMapData);

            _binaryWriter.NewMessage += BinaryWriterNewMessage;
            _binaryWriter.OnComplete += BinaryWriterOnComplete;
        }

        private void BinaryWriterOnComplete(object sender, PackerEventArgs e)
        {
            OnComplete?.Invoke(this, e);
        }

        private void BinaryWriterNewMessage(object sender, PackerEventArgs e)
        {
            NewMessage?.Invoke(this, e);
        }

        public void Execute()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _preProcessor.Run();

                _worldMapData.Prepare();

                _binaryWriter.WriteData(_options);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                BinaryWriterNewMessage(this, new PackerEventArgs($"ERROR: {ex.Message}"));
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public void CreateDefaultOptions()
        {
            if (string.IsNullOrEmpty(_options.OutputFilePath))
            {
                _options.OutputFilePath = Settings.Default.OutputFilePath;
            }

            _options.TextureFormat = Settings.Default.TextureFormat;
            _options.PlatformAlignment = Settings.Default.PlatformAlignment;
        }
    }

    public class PackerEventArgs
    {
        public string Message { get; set; }

        public PackerEventArgs(string message)
        {
            Message = message;
        }
    }
}
