using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;

namespace RyzeEditor.Tools
{
	public class PlacementTool : ToolBase, IVisualElement
	{
		public string SelectedMeshId { get; set; }

		public PlacementTool(WorldMap world, Selection selection) : base(world, selection)
		{
		}

		public Vector3 Position
		{
			get { return new Vector3(); }
		}

		public BoundingBox BoundingBox
		{
			get { return new BoundingBox(); }
		}

        public RenderOptions Options { get; set; }

        public override bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
		{
            var result = false;

			_selection.Clear();

            if (_world == null || string.IsNullOrEmpty(SelectedMeshId))
            {
                return result;
            }

            var mesh = ResourceManagment.ResourceManager.Instance.GetMesh(SelectedMeshId);
            var center = (mesh.BoundingBox.Maximum - mesh.BoundingBox.Minimum) / 2.0f;

            var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);

            Vector3 point = Vector3.Zero;
            float distance = float.MaxValue;

            var intersects = GetGeometryIntersections(ray);

            if (intersects.Count == 0)
            {
                var plane = new Plane(Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f));

                if (!plane.Intersects(ref ray, out point))
                {
                    return result;
                }
            }
            else
            {
                foreach (var intersect in intersects)
                {
                    var dist = (intersect.Point - _world.Camera.Position).Length();
                    if (dist < distance)
                    {
                        distance = dist;
                        var dir = Vector3.Normalize(_world.Camera.Position - intersect.Point);
                        var dot = Vector3.Dot(Vector3.UnitY, dir);
                        point = intersect.Point + (Math.Abs(center.Y) / dot) * dir;
                    }
                }
            }

            var meshes = new List<string> { SelectedMeshId };

            var gameObject = new GameObject(meshes, meshes)
            {
                Rotation = Quaternion.Identity,
                RotationRH = Quaternion.Identity,
                Scale = Vector3.One,
                Position = point
			};

			_world.AddEntity(gameObject);

            return result;
		}

		public void Render3d(IRenderer renderer, RenderMode mode)
		{
		}
	}
}
