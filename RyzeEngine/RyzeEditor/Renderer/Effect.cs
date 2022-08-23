using System;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace RyzeEditor.Renderer
{
	public class Effect
	{
		public InputLayout Layout
		{
			get { return _layout; }
		}

		public Buffer ContantBuffer
		{
			get { return _contantBuffer; }
		}

		public SamplerState SamplerState
		{
			get { return _samplerState; }	
		}

		public VertexShader VertexShader
		{
			get { return _vertexShader; }
		}

		public PixelShader PixelShader
		{
			get { return _pixelShader; }
		}

		private readonly String _fileName;
		private readonly InputElement[] _elements;
		private readonly SamplerStateDescription _samplerStateDesc;
		private readonly int _constBufferSize;
		private readonly String _shaderFlags;
		private ShaderSignature _signature;
		private CompilationResult _vertexShaderByteCode;
		private CompilationResult _pixelShaderByteCode;
		private VertexShader _vertexShader;
		private PixelShader _pixelShader;
		private InputLayout _layout;
		private Buffer _contantBuffer;
		private SamplerState _samplerState;

		public Effect(String fileName, InputElement[] elements, int constBufferSize, SamplerStateDescription samplerStateDesc, String shaderFlags)
		{
			_elements = elements;
			_constBufferSize = constBufferSize;
			_samplerStateDesc = samplerStateDesc;
			_shaderFlags = shaderFlags;
			_fileName = fileName;
		}

		public void Initialize(Device device)
		{
			_contantBuffer = new Buffer(device, _constBufferSize, ResourceUsage.Default, BindFlags.ConstantBuffer,
				CpuAccessFlags.None, ResourceOptionFlags.None, 0);

			_vertexShaderByteCode = ShaderBytecode.CompileFromFile(_fileName, "VS", String.Format("vs_{0}", _shaderFlags));
			_vertexShader = new VertexShader(device, _vertexShaderByteCode);

			_pixelShaderByteCode = ShaderBytecode.CompileFromFile(_fileName, "PS", String.Format("ps_{0}", _shaderFlags));
			_pixelShader = new PixelShader(device, _pixelShaderByteCode);

			_signature = ShaderSignature.GetInputSignature(_vertexShaderByteCode);

			_layout = new InputLayout(device, _signature, _elements);

			_samplerState = new SamplerState(device, _samplerStateDesc);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref _samplerState);		
			Utilities.Dispose(ref _layout);
			Utilities.Dispose(ref _signature);
			Utilities.Dispose(ref _pixelShader);
			Utilities.Dispose(ref _pixelShaderByteCode);
			Utilities.Dispose(ref _vertexShader);
			Utilities.Dispose(ref _vertexShaderByteCode);
			Utilities.Dispose(ref _contantBuffer);
		}
	}
}