using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace RyzeEditor.Renderer
{
	public class EffectManager
	{
		private readonly Dictionary<string, Effect> _effects;

		public EffectManager(Device device)
		{
			if (_effects != null)
			{
				return;	
			}

			_effects = new Dictionary<string, Effect>
			{
				{"Mesh", LoadMeshEffect(device)},
				{"Primitive", LoadPrimitiveEffect(device)},
                {"FXAA", LoadFxaaEffect(device) }
			};
		}

        public Effect GetEffect(String effectName)
		{
			if (_effects.ContainsKey(effectName) == false)
			{
				throw new ArgumentException(String.Format("Effect {0} not found", effectName));				
			}

			return _effects[effectName];
		}

		public void UnloadEffects()
		{
			if (_effects == null)
			{
				return;
			}

			foreach (var effect in _effects.Where(effect => effect.Value != null))
			{
				effect.Value.Dispose();
			}
		}

		private static Effect LoadPrimitiveEffect(Device device)
		{
            var samplerStateDesc = SamplerStateDescription.Default();

            var elements = new[]
			{
				new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
			};

			var constBufferSize = Utilities.SizeOf<Matrix>();

			var effect = new Effect("Shaders\\Primitive.fx", elements, constBufferSize, samplerStateDesc, "4_0");

			effect.Initialize(device);

			return effect;
		}

        private Effect LoadFxaaEffect(Device device)
        {
            var samplerStateDesc = new SamplerStateDescription
            {
                Filter = Filter.MinPointMagLinearMipPoint,
                AddressU = TextureAddressMode.Mirror,
                AddressV = TextureAddressMode.Mirror,
                AddressW = TextureAddressMode.Mirror,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 1,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = 1
            };

            var elements = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0),
            };

            var constBufferSize = Utilities.SizeOf<Matrix>();

            var effect = new Effect("Shaders\\FXAA.fx", elements, constBufferSize, samplerStateDesc, "4_0");

            effect.Initialize(device);

            return effect;
        }

        private static Effect LoadMeshEffect(Device device)
		{
			var samplerStateDesc = new SamplerStateDescription
			{
				Filter = Filter.MinLinearMagPointMipLinear,
				AddressU = TextureAddressMode.Mirror,
				AddressV = TextureAddressMode.Mirror,
				AddressW = TextureAddressMode.Mirror,
				BorderColor = Color.Black,
				ComparisonFunction = Comparison.Never,
				MaximumAnisotropy = 1,
				MipLodBias = 0,
				MinimumLod = 0,
				MaximumLod = 1
			};

			var elements = new[]
			{
				new InputElement("POSITION", 0, Format.R32G32B32_Float,     0, 0),
				new InputElement("NORMAL",   0, Format.R32G32B32_Float,    12, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    24, 0),
                new InputElement("TANGENT",  1, Format.R32G32B32_Float,    36, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       64, 0),

                new InputElement("INSTANCE", 0, Format.R32G32B32A32_Float,  0, 1, InputClassification.PerInstanceData, 1),
 				new InputElement("INSTANCE", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1), 
 				new InputElement("INSTANCE", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
 				new InputElement("INSTANCE", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1)
			};

			var constBufferSize = (RenderMode.ShadowMapCascadeNumber + 3) * Utilities.SizeOf<Matrix>() + 2 * Utilities.SizeOf<Vector4>();

			var effect = new Effect("Shaders\\Mesh.fx", elements, constBufferSize, samplerStateDesc, "4_0");

			effect.Initialize(device);

			return effect;
		}
	}
}
