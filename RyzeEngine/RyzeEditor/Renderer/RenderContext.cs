using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Tools;
using SharpDX;

namespace RyzeEditor.Renderer
{
	public class RenderContext
	{
		private readonly IRenderer _renderer;

		private readonly IToolManager _toolManager;

		private readonly RenderMode _renderMode;

		private readonly Dictionary<string, List<Matrix>> _meshGroup;

		private readonly List<EntityBase> _entities;

		private readonly PlaneGrid _planeGrid;

		public RenderContext(IRenderer renderer, IToolManager toolManager)
		{
			_renderer = renderer;
			_toolManager = toolManager;
			_renderMode = new RenderMode();
			_meshGroup = new Dictionary<string, List<Matrix>>();
			_entities = new List<EntityBase>();
			_planeGrid = new PlaneGrid(25, 2.0f);
		}

		public void Dispose()
		{
			if (_renderer != null)
			{
				_renderer.Dispose();
			}
		}

		public void ResizeWindow(Size wndSize)
		{
			if (_renderer != null)
			{
				_renderer.ResizeWindow(wndSize);
			}
		}

		public void RenderWorld(WorldMap worldMap)
		{
			if (worldMap == null)
			{
				return;
			}

			var camera = worldMap.Camera;
			var lookAtDir = worldMap.Camera.LookAtDir - worldMap.Camera.Position;
			lookAtDir.Normalize();

			var frustum = BoundingFrustum.FromCamera(camera.Position, lookAtDir, camera.UpDir, camera.FOV, camera.ZNear, camera.ZFar, camera.AspectRatio);

			_entities.Clear();

			foreach (var entity in worldMap.Entities)
			{
                if (!(entity is IVisualElement visualElement) || entity.IsHidden)
                {
                    continue;
                }

                var gameObject = visualElement as GameObject;

				if (gameObject != null)
				{
					var boundingBox = GetGameObjectBoundingBox(gameObject);

					if (frustum.Contains(boundingBox) != ContainmentType.Disjoint)
					{
						_entities.Add(entity);
					}
				}
				else
				{
					if (frustum.Contains(visualElement.BoundingBox) != ContainmentType.Disjoint)
					{
						_entities.Add(entity);
					}
				}
			}

            _renderer.RenderShadowMap();

			_renderer.PreRender();

			//RenderPlaneGrid(frustum, camera);

			var gameObjects = _entities.OfType<GameObject>().ToList();
            var options = GetRenderOptions();
            var objectIds = options.Where(x => x.ShapeType != ShapeType.GeometryMesh).Select(x => x.GameObjectId);

            foreach (var objectId in objectIds)
            {
                var gameObject = gameObjects.Where(x => x.Id == objectId).FirstOrDefault();

                if (gameObject != null)
                {
                    gameObjects.Remove(gameObject);
                }
            }

            RenderGameObjects(gameObjects);

            var objects = _entities.OfType<GameObject>().Where(x => objectIds.Contains(x.Id)).ToList();

            RenderGameObjectsWithOptions(objects, options);

            _renderer.ClearDepthStencilView();

			RenderTools();

            _renderer.PostRender();

            _renderer.Present();
		}

        private void RenderGameObjectsWithOptions(List<GameObject> objects, List<RenderOptions> options)
        {
            foreach (var obj in objects)
            {
                var option = options.Where(x => x.GameObjectId == obj.Id).SingleOrDefault();

                string meshId = null;

                switch (option.ShapeType)
                {
                    case ShapeType.CollisionMesh:
                        {
                            var rigidBoby = _entities.OfType<RigidBody>().Where(x => x.GameObject == obj).FirstOrDefault();

                            if (rigidBoby == null)
                            {
                                continue;
                            }

                            meshId = rigidBoby.Mesh.Id;
                            break;
                        }

                    case ShapeType.WheelMesh:
                        {
                            var vehicle = _entities.OfType<Vehicle>().Where(x => x.Id == obj.Id).FirstOrDefault();

                            if (vehicle == null)
                            {
                                continue;
                            }

                            meshId = vehicle.ChassisMeshId;
                            break;
                        }
                }

                if (!string.IsNullOrEmpty(meshId))
                {
                    var mesh = ResourceManager.Instance.GetMesh(meshId);
                    var renderMode = new RenderMode
                    {
                        IsDepthClipEnabled = _renderMode.IsDepthClipEnabled,
                        BoundBox = _renderMode.BoundBox,
                        SubMeshIds = option.SubMeshIds,
                        Color = option.Color
                    };

                    if (mesh != null)
                    {
                        var matrices = new List<Matrix> { obj.WorldMatrix };
                        _renderer.DrawMeshInstanced(mesh, matrices.ToArray(), renderMode);
                    }
                }
            }
        }

        private BoundingBox GetGameObjectBoundingBox(GameObject gameObject)
        {
            var matrix = gameObject.WorldMatrix;
            var min = gameObject.BoundingBox.Minimum;
            var max = gameObject.BoundingBox.Maximum;

            Vector3.TransformCoordinate(ref min, ref matrix, out Vector3 minT);
            Vector3.TransformCoordinate(ref max, ref matrix, out Vector3 maxT);

            return new BoundingBox(minT, maxT);
        }

        private void RenderPlaneGrid(BoundingFrustum frustum, Camera camera)
		{
			foreach (var patch in _planeGrid.Patches)
			{
				int visiblePoints = 0;

				var dist = (patch.Points[0] - camera.Position).Length();

				if (dist > 2000.0f)
				{
					continue;
				}

				foreach (var point in patch.Points)
				{
					var pt = point;
					if (frustum.Contains(ref pt) != ContainmentType.Disjoint)
					{
						visiblePoints++;
					}
				}

				if (visiblePoints != 0)
				{
					RenderHelper.RenderGridPatch(_renderer, patch);
				}
			}
		}

        private void RenderGameObjects(IEnumerable<GameObject> gameObjects)
        {
            _meshGroup.Clear();

            var gameObjectList = gameObjects.Where(obj => obj.GeometryMeshes != null && obj.GeometryMeshes.Count > 0).ToList();

            foreach (var gameObject in gameObjectList)
            {
                if (!_meshGroup.ContainsKey(gameObject.GeometryMeshes[0].Id))
                {
                    _meshGroup.Add(gameObject.GeometryMeshes[0].Id, new List<Matrix>());
                }

                _meshGroup[gameObject.GeometryMeshes[0].Id].Add(gameObject.WorldMatrix);
            }

            foreach (var item in _meshGroup)
            {
                var mesh = ResourceManager.Instance.GetMesh(item.Key);

                if (mesh != null)
                {
                    _renderer.DrawMeshInstanced(mesh, item.Value.ToArray(), _renderMode);
                }
            }

            foreach (var gameObject in gameObjectList)
            {
                RenderNormals(gameObject);

                RenderTangents(gameObject);

                RenderBitangents(gameObject);
            }
        }

        private void RenderBitangents(GameObject gameObject)
        {
            if (!gameObject.RenderBitangents)
            {
                return;
            }

            var mesh = gameObject.GeometryMeshes.FirstOrDefault();
            var subMeshes = mesh.SubMeshes;

            foreach (var subMesh in subMeshes)
            {
                foreach (var vertex in subMesh.VertexData)
                {
                    var position = new Vector4();
                    var matrix = subMesh.GetMatrix(mesh) * gameObject.WorldMatrix;
                    var pos = new Vector4(vertex.Pos, 1.0f);
                    Vector4.Transform(ref pos, ref matrix, out position);

                    var point1 = new Point3
                    {
                        Position = position,
                        Color = Vector4.UnitZ
                    };

                    var bitangent = new Vector4();
                    var normalMatrix = subMesh.GetNormalMatrix(mesh) * Matrix.RotationQuaternion(gameObject.Rotation);
                    var tangentVector = new Vector4(vertex.Bitangent, 0.0f);
                    Vector4.Transform(ref tangentVector, ref normalMatrix, out bitangent);

                    var point2 = new Point3
                    {
                        Position = position + bitangent * 0.2f,
                        Color = Vector4.UnitZ
                    };

                    var points = new List<Point3>() { point1, point2 };

                    _renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
                }
            }
        }

        private void RenderTangents(GameObject gameObject)
        {
            if (!gameObject.RenderTangents)
            {
                return;
            }

            var mesh = gameObject.GeometryMeshes.FirstOrDefault();
            var subMeshes = mesh.SubMeshes;

            foreach (var subMesh in subMeshes)
            {
                foreach (var vertex in subMesh.VertexData)
                {
                    var position = new Vector4();
                    var matrix = subMesh.GetMatrix(mesh) * gameObject.WorldMatrix;
                    var pos = new Vector4(vertex.Pos, 1.0f);
                    Vector4.Transform(ref pos, ref matrix, out position);

                    var point1 = new Point3
                    {
                        Position = position,
                        Color = Vector4.UnitY
                    };

                    var tangent = new Vector4();
                    var normalMatrix = subMesh.GetNormalMatrix(mesh) * Matrix.RotationQuaternion(gameObject.Rotation);
                    var tangentVector = new Vector4(vertex.Tangent, 0.0f);
                    Vector4.Transform(ref tangentVector, ref normalMatrix, out tangent);

                    var point2 = new Point3
                    {
                        Position = position + tangent * 0.2f,
                        Color = Vector4.UnitY
                    };

                    var points = new List<Point3>() { point1, point2 };

                    _renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
                }
            }
        }

        private void RenderNormals(GameObject gameObject)
        {
            if (!gameObject.RenderNormals)
            {
                return;
            }

            var mesh = gameObject.GeometryMeshes.FirstOrDefault();
            var subMeshes = mesh.SubMeshes;

            foreach (var subMesh in subMeshes)
            {
                foreach (var vertex in subMesh.VertexData)
                {
                    var position = new Vector4();
                    var matrix = subMesh.GetMatrix(mesh) * gameObject.WorldMatrix;
                    var pos = new Vector4(vertex.Pos, 1.0f);
                    Vector4.Transform(ref pos, ref matrix, out position);

                    var point1 = new Point3
                    {
                        Position = position,
                        Color = Vector4.UnitX
                    };

                    var normal = new Vector4();
                    var normalMatrix = subMesh.GetNormalMatrix(mesh) * Matrix.RotationQuaternion(gameObject.Rotation);
                    var normalVector = new Vector4(vertex.Norm, 0.0f);
                    Vector4.Transform(ref normalVector, ref normalMatrix, out normal);

                    var point2 = new Point3
                    {
                        Position = position + normal * 0.2f,
                        Color = Vector4.UnitX
                    };

                    var points = new List<Point3>() { point1, point2 };

                    _renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
                }
            }
        }

        private void RenderTools()
		{
			foreach (var visualTool in from tool in _toolManager.Tools let visualTool = tool as IVisualElement
                                       where visualTool != null && tool.IsActive select visualTool)
			{
				visualTool.Render3d(_renderer, _renderMode);
			}
		}

        private List<RenderOptions> GetRenderOptions()
        {
            var optionsList = new List<RenderOptions>();

            foreach (var visualTool in from tool in _toolManager.Tools
                                       let visualTool = tool as IVisualElement
                                       where visualTool != null && tool.IsActive
                                       select visualTool)
            {
                if (visualTool.Options != null)
                {
                    optionsList.Add(visualTool.Options);
                }
            }

            return optionsList;
        }
	}
}