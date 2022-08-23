using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace RyzeEditor.Renderer
{
	public abstract class RenderD3dBase : IRenderer
	{
		protected IntPtr _handle;
		protected Camera _camera;
		protected Device _device;
		protected SwapChain _swapChain;
		protected DeviceContext _context;
		protected Texture2D _backBuffer;
        protected Texture2D _proxyBackBuffer;
        protected RenderTargetView _renderView;
        protected RenderTargetView _proxyRenderView;
        protected ShaderResourceView _proxyShaderResourceView;
        protected Texture2D _depthBuffer;
		protected DepthStencilView _depthView;
        protected DepthStencilState _defaultStentilState;
        protected DepthStencilState _disabledDepthStencilState;
        protected SwapChainDescription _desc;
        protected RasterizerState _rasterState;
        protected RasterizerState _cullDisabledRasterizerState;
        protected Factory _factory;
		protected Buffer _vertBuffer;
        protected Buffer _fxaaVertBuffer;
        protected Buffer _instanceBuffer;
		protected Buffer _indexBuffer;
        protected Buffer _fxaaIndexBuffer;
        protected EffectManager _effectManager;
		protected ShaderResourceManager _shaderResourceManager;
        protected const int IndexBufferCapacity = 1000000;
		protected const int VertexBufferCapacity = 150000;
		protected const int InstanceMaxValue = 10000;
        private float _widthScale;
        private float _heigthScale;

        public void Initialize(IntPtr handle, Camera camera)
		{
			_handle = handle;
			_camera = camera;

            var resolution = Screen.PrimaryScreen.Bounds;
            _widthScale = 1.0f;
            _heigthScale = 1.0f;

            _desc = new SwapChainDescription
			{
				BufferCount = 1,
				ModeDescription = new ModeDescription((int)(_widthScale * _camera.ClientWndSize.Width), (int)(_heigthScale * _camera.ClientWndSize.Height), new Rational(30, 1), Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = _handle,
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};

			FeatureLevel[] levels =
			{
				FeatureLevel.Level_11_0,
				FeatureLevel.Level_10_1,
				FeatureLevel.Level_10_0
			};

			Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, levels, _desc, out _device, out _swapChain);
			_context = _device.ImmediateContext;

			_factory = _swapChain.GetParent<Factory>();
			_factory.MakeWindowAssociation(_handle, WindowAssociationFlags.IgnoreAll);

			var desc = new BufferDescription(IndexBufferCapacity * sizeof(uint), ResourceUsage.Dynamic, BindFlags.IndexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
			_indexBuffer = Buffer.Create(_device, new uint[IndexBufferCapacity], desc);

			int vertSize = 4 * Utilities.SizeOf<Vector3>() + Utilities.SizeOf<Vector2>();
			desc = new BufferDescription(VertexBufferCapacity * vertSize, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
			_vertBuffer = Buffer.Create(_device, new Vertex[VertexBufferCapacity], desc);

            VertexPositionTex[] quadVertices =
            {
                new VertexPositionTex(new Vector4(-1.0f, -1.0f, 0.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new VertexPositionTex(new Vector4(-1.0f, 1.0f, 0.0f, 1.0f),  new Vector2(0.0f, 0.0f)),
                new VertexPositionTex(new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                new VertexPositionTex(new Vector4(1.0f, -1.0f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f))
            };

            int fxaaVertSize = Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector2>();
            var fxaaDesc = new BufferDescription(quadVertices.Length * fxaaVertSize, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _fxaaVertBuffer = Buffer.Create(_device, quadVertices, fxaaDesc);

            fxaaDesc = new BufferDescription(6 * sizeof(uint), ResourceUsage.Dynamic, BindFlags.IndexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _fxaaIndexBuffer = Buffer.Create(_device, new uint[] {0, 1, 2, 0, 2, 3}, fxaaDesc);

            desc = new BufferDescription(InstanceMaxValue * Utilities.SizeOf<Matrix>(), ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
			_instanceBuffer = Buffer.Create(_device, new Matrix[InstanceMaxValue], desc);

            var rasterDesc = RasterizerStateDescription.Default();
            rasterDesc.CullMode = CullMode.None;
            _cullDisabledRasterizerState = new RasterizerState(_device, rasterDesc);

            rasterDesc = RasterizerStateDescription.Default();
            _rasterState = new RasterizerState(_device, rasterDesc);

            var depthStencilDesc = DepthStencilStateDescription.Default();
            _defaultStentilState = new DepthStencilState(_device, depthStencilDesc);

            depthStencilDesc = DepthStencilStateDescription.Default();
            depthStencilDesc.IsDepthEnabled = false;
            _disabledDepthStencilState = new DepthStencilState(_device, depthStencilDesc);

            _effectManager = new EffectManager(_device);
            _shaderResourceManager = new ShaderResourceManager(_device);
        }

		public Camera Camera
		{
			get { return _camera; }
			set { _camera = value; }
		}

		public void PreRender()
		{
            _shaderResourceManager.CleanupUnusedResources();

            _context.ClearRenderTargetView(_proxyRenderView, Color.Black);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            _context.OutputMerger.SetTargets(_depthView, _proxyRenderView);
            _context.OutputMerger.DepthStencilState = _defaultStentilState;
            _context.Rasterizer.State = _rasterState;
        }

        public void PostRender()
        {
            _context.ClearRenderTargetView(_renderView, Color.Black);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            _context.OutputMerger.SetTargets(_depthView, _renderView);
            _context.OutputMerger.DepthStencilState = _disabledDepthStencilState;
            _context.Rasterizer.State = _cullDisabledRasterizerState;

            var effect = _effectManager.GetEffect("FXAA");

            _context.InputAssembler.InputLayout = effect.Layout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.VertexShader.SetConstantBuffer(0, effect.ContantBuffer);
            _context.VertexShader.Set(effect.VertexShader);
            _context.PixelShader.Set(effect.PixelShader);

            _context.InputAssembler.SetIndexBuffer(_fxaaIndexBuffer, Format.R32_UInt, 0);

            var binding = new VertexBufferBinding(_fxaaVertBuffer, Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector2>(), 0);
            _context.InputAssembler.SetVertexBuffers(0, binding);

            var data = new List<float>
            {
                _widthScale * _camera.ClientWndSize.Width,
                _heigthScale * _camera.ClientWndSize.Height
            };
            _context.UpdateSubresource(data.ToArray(), effect.ContantBuffer);

            _context.PixelShader.SetShaderResource(0, _proxyShaderResourceView);
            _context.PixelShader.SetSampler(0, effect.SamplerState);

            _context.DrawIndexed(6, 0, 0);
        }

        public void ResizeWindow(Size wndSize)
		{
			CreateWindowSizeDependentResources(wndSize);
		}

		public void ClearDepthStencilView()
		{
			_context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);			
		}

		public void Present()
		{
			_swapChain.Present(0, PresentFlags.None);
		}

		public void Dispose()
		{
			_effectManager.UnloadEffects();

			Utilities.Dispose(ref _depthBuffer);
			Utilities.Dispose(ref _vertBuffer);
            Utilities.Dispose(ref _fxaaVertBuffer);
			Utilities.Dispose(ref _instanceBuffer);
			Utilities.Dispose(ref _indexBuffer);
            Utilities.Dispose(ref _fxaaIndexBuffer);
            Utilities.Dispose(ref _depthView);
			Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref _proxyRenderView);
            Utilities.Dispose(ref _proxyShaderResourceView);
            Utilities.Dispose(ref _backBuffer);
            Utilities.Dispose(ref _proxyBackBuffer);
            Utilities.Dispose(ref _rasterState);
            Utilities.Dispose(ref _cullDisabledRasterizerState);
            Utilities.Dispose(ref _defaultStentilState);
            Utilities.Dispose(ref _disabledDepthStencilState);
            Utilities.Dispose(ref _device);
			Utilities.Dispose(ref _context);
			Utilities.Dispose(ref _swapChain);
			Utilities.Dispose(ref _factory);
		}

		public virtual void DrawLineStrip(IEnumerable<Point3> points, RenderMode mode)
		{
		}

		public virtual void DrawMeshInstanced(IMesh mesh, Matrix[] matrices, RenderMode mode)
		{
		}

		private void CreateWindowSizeDependentResources(Size wndSize)
		{
			Utilities.Dispose(ref _backBuffer);
            Utilities.Dispose(ref _proxyBackBuffer);
            Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref _proxyRenderView);
            Utilities.Dispose(ref _proxyShaderResourceView);
            Utilities.Dispose(ref _depthBuffer);
			Utilities.Dispose(ref _depthView);

            _camera.AspectRatio = (float)wndSize.Width / wndSize.Height;
			_camera.ClientWndSize = wndSize;

            int width = (int)(_widthScale * wndSize.Width);
            int height = (int)(_heigthScale * wndSize.Height);

            _swapChain.ResizeBuffers(_desc.BufferCount, width, height, Format.Unknown, SwapChainFlags.None);

			_backBuffer = Resource.FromSwapChain<Texture2D>(_swapChain, 0);
			
			_depthBuffer = new Texture2D(_device, new Texture2DDescription
			{
				Format = Format.D32_Float_S8X24_UInt,
				ArraySize = 1,
				MipLevels = 1,
				Width = width,
				Height = height,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None
			});

            _renderView = new RenderTargetView(_device, _backBuffer);

            _proxyBackBuffer = new Texture2D(_device, new Texture2DDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _proxyRenderView = new RenderTargetView(_device, _proxyBackBuffer);
            _proxyShaderResourceView = new ShaderResourceView(_device, _proxyBackBuffer);

            _depthView = new DepthStencilView(_device, _depthBuffer);
			_context.Rasterizer.SetViewport(new Viewport(0, 0, width, height, 0.0f, 1.0f));
		}
	}
}
