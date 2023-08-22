using System.Collections.Generic;
using System.Linq;
using RyzeEditor.Extentions;
using RyzeEditor.ResourceManagment;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace RyzeEditor.Renderer
{
	public class RendererD3d : RenderD3dBase
	{
		public override void DrawLineStrip(IEnumerable<Point3> points, RenderMode mode)
		{
            var effect = _effectManager.GetEffect("Primitive");

			_context.InputAssembler.InputLayout = effect.Layout;
			_context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineStrip;
			_context.VertexShader.SetConstantBuffer(0, effect.ContantBuffer);
			_context.VertexShader.Set(effect.VertexShader);
			_context.PixelShader.Set(effect.PixelShader);

			var pointList = points.ToList();
			_context.MapSubresource(_vertBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);

            foreach (var point in pointList)
            {
                stream.WriteRange(point.GetBytes());
            }

            _context.UnmapSubresource(_vertBuffer, 0);

			int stride = 2 * Utilities.SizeOf<Vector4>();
			var vertexBufferBinding = new VertexBufferBinding(_vertBuffer, stride, 0);

			_context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);

			var view = Matrix.LookAtLH(_camera.Position, _camera.LookAtDir, _camera.UpDir);
			var proj = Matrix.PerspectiveFovLH(_camera.FOV, _camera.AspectRatio, _camera.ZNear, _camera.ZFar);

			var viewProj = view * proj;
			viewProj.Transpose();

			var data = new List<float>();
			data.AddRange(viewProj.ToArray());

			_context.UpdateSubresource(data.ToArray(), effect.ContantBuffer);

			_context.Draw(pointList.Count, 0);
		}

		public override void DrawMeshInstanced(IMesh mesh, Matrix[] matrices, RenderMode mode)
		{
			var effect = _effectManager.GetEffect("Mesh");

			_context.InputAssembler.InputLayout = effect.Layout;
			_context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			_context.VertexShader.SetConstantBuffer(0, effect.ContantBuffer);
			_context.VertexShader.Set(effect.VertexShader);

            if (!mode.ShadowMap)
            {
                _context.PixelShader.Set(effect.PixelShader);
            }

            _context.MapSubresource(_instanceBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
            stream.WriteRange(matrices.ToArray());
			_context.UnmapSubresource(_instanceBuffer, 0);

			var instanceBufferBinding = new VertexBufferBinding(_instanceBuffer, Utilities.SizeOf<Matrix>(), 0);

			foreach (var subMesh in mesh.SubMeshes)
			{
                if (subMesh.VertexData.Count == 0)
                {
                    continue;
                }

                _context.MapSubresource(_vertBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
				stream.WriteRange(subMesh.VertexData.ToArray());
				_context.UnmapSubresource(_vertBuffer, 0);

				int stride = 4 * Utilities.SizeOf<Vector3>() + Utilities.SizeOf<Vector2>();
				var vertexBufferBinding = new VertexBufferBinding(_vertBuffer, stride, 0);

				_context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding, instanceBufferBinding);
				
				for (int i = 0; i < subMesh.Materials.Count; i++)
				{
                    if (!subMesh.Indices.ContainsKey(i))
                    {
                        continue;
                    }

					_context.MapSubresource(_indexBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
					stream.WriteRange(subMesh.Indices[i].ToArray());
					_context.UnmapSubresource(_indexBuffer, 0);

					var material = subMesh.Materials[i];

					if (!mode.ShadowMap && material.DiffuseTexture != null)
					{
						var textureView = _shaderResourceManager.GetShaderResource(material.DiffuseTexture);

						_context.PixelShader.SetShaderResource(0, textureView);
						_context.PixelShader.SetSampler(0, effect.SamplerState);

						material.Diffuse = new Vector3(0.0f, 0.0f, 0.0f);
					}

                    Matrix view;
                    Matrix proj;

                    if (mode.ShadowMap)
                    {
                        view = Matrix.LookAtLH(mode.SunLightDir * -1000.0f, Vector3.Zero, _camera.UpDir);
                        proj = Matrix.Identity;
                    }
                    else
                    {
                        view = Matrix.LookAtLH(_camera.Position, _camera.LookAtDir, _camera.UpDir);
                        proj = Matrix.PerspectiveFovLH(_camera.FOV, _camera.AspectRatio, _camera.ZNear, _camera.ZFar);
                    }

					var viewProj = view * proj;
                    viewProj.Transpose();

                    var posMatrix = subMesh.GetMatrix(mesh);
                    posMatrix.Transpose();

                    var normMatrix = subMesh.GetNormalMatrix(mesh);
                    normMatrix.Transpose();

                    var lightDir = Vector4.Normalize((new Vector4(mode.SunLightDir, 0)));
                    var diffuseColor = (mode.SubMeshIds != null && mode.SubMeshIds.Contains((int)subMesh.Id)) ? mode.Color : new Vector4(material.Diffuse, 0);

                    var data = new List<float>();
                    data.AddRange(posMatrix.ToArray());
                    data.AddRange(normMatrix.ToArray());
                    data.AddRange(viewProj.ToArray());
					data.AddRange(diffuseColor.ToArray());
                    data.AddRange(lightDir.ToArray());

                    _context.UpdateSubresource(data.ToArray(), effect.ContantBuffer);
					_context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
					_context.DrawIndexedInstanced(subMesh.Indices[i].Count, matrices.Count(), 0, 0, 0);
				}
			}
		}
	}
}