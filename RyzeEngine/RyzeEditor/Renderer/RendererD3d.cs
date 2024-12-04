using System.Collections.Generic;
using System.Linq;
using BulletSharp.SoftBody;
using RyzeEditor.Extentions;
using RyzeEditor.GameWorld;
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

			_context.InputAssembler.SetVertexBuffers(0, _vertexBufferLineStripBinding);

			var view = Matrix.LookAtLH(_camera.Position, _camera.LookAtDir, _camera.UpDir);
			var proj = Matrix.PerspectiveFovLH(_camera.FOV, _camera.AspectRatio, _camera.ZNear, _camera.ZFar);

			var viewProj = view * proj;
			viewProj.Transpose();

			var data = new List<float>();
			data.AddRange(viewProj.ToArray());

			_context.UpdateSubresource(data.ToArray(), effect.ContantBuffer);

			_context.Draw(pointList.Count, 0);
		}

		public override void DrawMeshInstanced(IMesh mesh, GameObject[] gameObjects, RenderMode mode)
		{
			var effect = _effectManager.GetEffect("Mesh");

			_context.InputAssembler.InputLayout = effect.Layout;
			_context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			_context.VertexShader.SetConstantBuffer(0, effect.ContantBuffer);
			_context.VertexShader.Set(effect.VertexShader);
            _context.PixelShader.Set(mode.ShadowMap ? null : effect.PixelShader);

			for (int i = 0; i < mesh.SubMeshes.Count; i++)
			{
                var subMesh = mesh.SubMeshes[i];

                int objectCount = 0;

                foreach (var gameObject in gameObjects)
                {
                    if ((gameObject.SubmeshVisibleMask & (1 << i)) != 0)
                    {
                        objectCount++;
                    }
                }

                bool isHidden = objectCount == 0;

                if (subMesh.VertexData.Count == 0 || isHidden)
                {
                    continue;
                }

                _context.MapSubresource(_instanceBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);

                var matrices = new List<Matrix>();

                foreach (var gameObject in gameObjects)
                {
                    if ((gameObject.SubmeshVisibleMask & (1 << i)) == 0)
                    {
                        continue;
                    }

                    SubMeshTransform subMeshTransform = null;

                    if (gameObject.SubMeshTransforms != null && gameObject.SubMeshTransforms.ContainsKey(subMesh.Id))
                    {
                        subMeshTransform = gameObject.SubMeshTransforms[subMesh.Id];
                    }

                    if (subMeshTransform == null)
                    {
                        matrices.Add(gameObject.WorldMatrix);
                    }
                    else
                    {
                        var scaleMatrix = subMesh.GetScaleMatrix(mesh);
                        var scale = scaleMatrix.ScaleVector;

                        var subMeshMatrix = subMesh.GetMatrix(mesh);
                        var subMeshMatrixInv = Matrix.Invert(subMeshMatrix);

                        var subMeshWS = Matrix.Scaling(scale) *
                                        Matrix.RotationQuaternion(subMeshTransform.Rotation) *
                                        Matrix.Translation(subMeshTransform.Position);

                        matrices.Add(subMeshMatrixInv * subMeshWS);
                    }
                }

                stream.WriteRange(matrices.ToArray());
                _context.UnmapSubresource(_instanceBuffer, 0);

                _context.MapSubresource(_vertBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
				stream.WriteRange(subMesh.VertexData.ToArray());
				_context.UnmapSubresource(_vertBuffer, 0);

				_context.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding, _instanceBufferBinding);
				
				for (int j = 0; j < subMesh.Materials.Count; j++)
				{
                    if (!subMesh.Indices.ContainsKey(j))
                    {
                        continue;
                    }

					_context.MapSubresource(_indexBuffer, MapMode.WriteDiscard, MapFlags.None, out stream);
					stream.WriteRange(subMesh.Indices[j].ToArray());
					_context.UnmapSubresource(_indexBuffer, 0);

					var material = subMesh.Materials[j];

					if (!mode.ShadowMap)
					{
                        if (material.DiffuseTexture != null)
                        {
                            var textureView = _shaderResourceManager.GetShaderResource(material.DiffuseTexture);

                            _context.PixelShader.SetShaderResource(0, textureView);
                            _context.PixelShader.SetSampler(0, effect.SamplerState);

                            material.Diffuse = new Vector3(0.0f, 0.0f, 0.0f);
                        }

                        for (int k = 0; k < RenderMode.ShadowMapCascadeNumber; k++)
                        {
                            _context.PixelShader.SetShaderResource(1 + k, GetDepthMapShaderResourceView(k));
                        }
                    }

                    var lightViewProj = new Matrix[RenderMode.ShadowMapCascadeNumber];
                    Vector3 ligthPos = mode.DirectLightDir * 10.0f + _camera.LookAtDir;

                    for (int k = 0; k < RenderMode.ShadowMapCascadeNumber; k++)
                    {
                        float size = Vector3.Distance(_camera.LookAtDir, _camera.Position) * (k * 5.5f + 2.5f);
                        lightViewProj[k] = Matrix.LookAtLH(ligthPos, _camera.LookAtDir, _camera.UpDir) * Matrix.OrthoLH(size, size, -size, size);
                        lightViewProj[k].Transpose();
                    }

                    var view = Matrix.LookAtLH(_camera.Position, _camera.LookAtDir, _camera.UpDir);
                    Matrix viewProj;

                    if (mode.ShadowMap)
                    {
                        viewProj = lightViewProj[mode.ShadowMapCascadeIndex];
                    }
                    else
                    {
                        var proj = Matrix.PerspectiveFovLH(_camera.FOV, _camera.AspectRatio, _camera.ZNear, _camera.ZFar);
                        viewProj = view * proj;
                        viewProj.Transpose();
                    }

                    var posMatrix = subMesh.GetMatrix(mesh);
                    posMatrix.Transpose();

                    var normMatrix = subMesh.GetNormalMatrix(mesh);
                    normMatrix.Transpose();

                    var dir = _camera.Position - _camera.LookAtDir;
                    var lightDir = Vector4.Normalize(new Vector4(dir, 0));
                    var diffuseColor = (mode.SubMeshIds != null && mode.SubMeshIds.Contains((int)subMesh.Id)) ? mode.Color : new Vector4(material.Diffuse, 0);
                    diffuseColor.W = (_camera.ClientWndSize.Width / 1024) * 1024.0f;

                    var data = new List<float>();
                    data.AddRange(posMatrix.ToArray());
                    data.AddRange(normMatrix.ToArray());
                    data.AddRange(viewProj.ToArray());

                    for (int k = 0; k < RenderMode.ShadowMapCascadeNumber; k++)
                    {
                        data.AddRange(lightViewProj[k].ToArray());
                    }

                    data.AddRange(diffuseColor.ToArray());

                    lightDir.W = mode.RenderShadows ? 1.0f : 0.0f;

                    data.AddRange(lightDir.ToArray());

                    _context.UpdateSubresource(data.ToArray(), effect.ContantBuffer);
					_context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
					_context.DrawIndexedInstanced(subMesh.Indices[j].Count, objectCount, 0, 0, 0);
				}
			}
		}
	}
}