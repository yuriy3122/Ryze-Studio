using System.Collections.Generic;
using System.Windows.Forms;
using RyzeEditor.GameWorld;
using RyzeEditor.Helpers;
using RyzeEditor.Renderer;
using SharpDX;

namespace RyzeEditor.Tools
{
    public class PointLightTool : ToolBase, IVisualElement
    {
        public PointLightTool(WorldMap world, Selection selection) : base(world, selection)
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
                        point = intersect.Point;
                    }
                }
            }

            var sphereMesh = new List<string> { "light.rz" };

            var pointLight = new PointLight(sphereMesh, sphereMesh)
            {
                Rotation = Quaternion.Identity,
                Scale = Vector3.One,
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                Position = point,
                Intensity = 1.0f,
                Radius = 50.0f
            };

            _world.AddEntity(pointLight);

            return result;
        }

        public void Render3d(IRenderer renderer, RenderMode mode)
        {
            foreach (var entity in _world.Entities)
            {
                if (entity.GetType() == typeof(PointLight))
                {
                    var pointLight = (PointLight)entity;

                    if (pointLight.GeometryMeshes != null &&
                        pointLight.GeometryMeshes.Count > 0 &&
                        pointLight.GeometryMeshes[0].SubMeshes != null &&
                        pointLight.GeometryMeshes[0].SubMeshes.Count > 0)
                    {
                        var color = new Vector3(1.0f, 1.0f, 0.0f);
                        pointLight.GeometryMeshes[0].SubMeshes[0].Materials[0].Ambient = color;
                        pointLight.GeometryMeshes[0].SubMeshes[0].Materials[0].Diffuse = color;
                    }
                }
            }
        }
    }
}
