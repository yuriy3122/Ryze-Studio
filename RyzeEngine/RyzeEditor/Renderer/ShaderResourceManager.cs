using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX.Direct3D11;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Renderer
{
    public class ShaderResourceManager
	{
		private readonly Dictionary<string, ShaderResourceView> _shaderResourceViews = new Dictionary<string, ShaderResourceView>();
		private readonly Device _device;

		public ShaderResourceManager(Device device)
		{
			_device = device;
		}

		public ShaderResourceView GetShaderResource(ITexture texture)
		{
			if (_shaderResourceViews.ContainsKey(texture.Id))
			{
				return _shaderResourceViews[texture.Id];
			}

            Bitmap bitmap;
            using (var stream = new MemoryStream(texture.Data))
            {
                bitmap = new Bitmap(stream);
            }

            Bitmap clone = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(clone))
            {
                graphics.DrawImage(bitmap, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            bitmap.Dispose();

            var data = clone.LockBits(new Rectangle(0, 0, clone.Width, clone.Height), ImageLockMode.ReadOnly, clone.PixelFormat);

            var texture2d = new Texture2D(_device, new Texture2DDescription()
            {
                Width = clone.Width,
                Height = clone.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Immutable,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            }, new SharpDX.DataRectangle(data.Scan0, data.Stride));
            clone.UnlockBits(data);

            _shaderResourceViews[texture.Id] = new ShaderResourceView(_device, texture2d);

            return _shaderResourceViews[texture.Id];
		}

        public void CleanupUnusedResources()
        {
            var resourceManager = ResourceManager.Instance;
            var reclaimedTextures = new List<string>();

            foreach (var reclaimedTexture in reclaimedTextures)
            {
                if (_shaderResourceViews.ContainsKey(reclaimedTexture))
                {
                    _shaderResourceViews[reclaimedTexture].Dispose();
                    _shaderResourceViews.Remove(reclaimedTexture);
                }
            }
        }
	}
}
