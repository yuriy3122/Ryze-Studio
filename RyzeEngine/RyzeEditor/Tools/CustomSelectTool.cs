using System;
using System.Linq;
using System.Windows.Forms;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using SharpDX;
using SharpDX.RawInput;

//https://ameye.dev/notes/rendering-outlines/
namespace RyzeEditor.Tools
{
    public class CustomSelectTool : ToolBase, IVisualElement
    {
        bool _controlKeyPressed;

        public CustomSelectTool(WorldMap world, Selection selection) : base(world, selection)
        {
        }

        public Vector3 Position
        {
            get { return Vector3.Zero; }
        }

        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(); }
        }

        public RenderOptions Options { get; set; }

        public override bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            var result = false;

            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return result;
            }

            if (_world == null)
            {
                return result;
            }

            if (!_controlKeyPressed)
            {
                _selection.Clear();
            }

            var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);

            GameObject nearestToCameraObject = null;
            float distance = float.MaxValue;
            int subMeshIndex = 0;

            var intersects = GetGeometryIntersections(ray);

            foreach (var intersect in intersects)
            {
                var dist = (intersect.Point - _world.Camera.Position).Length();
                if (dist < distance)
                {
                    distance = dist;
                    subMeshIndex = intersect.SubMeshIndex;
                    nearestToCameraObject = _world.Entities.OfType<GameObject>().Where(x => x.Id == intersect.GameObjectId).FirstOrDefault();
                }
            }

            if (nearestToCameraObject != null)
            {
                if (_controlKeyPressed && _selection.Get().OfType<GameObject>().Contains(nearestToCameraObject))
                {
                    _selection.RemoveEntity(nearestToCameraObject.Id);
                }
                else
                {
                    _selection.AddEntity(nearestToCameraObject);
                }

                result = true;
            }

            return result;
        }

        public override bool OnKeyboardInput(object sender, KeyboardInputEventArgs arg)
        {
            _controlKeyPressed = false;

            switch (arg.Key)
            {
                case Keys.ControlKey:
                    _controlKeyPressed = arg.State == KeyState.KeyDown;

                    break;
            }

            return _controlKeyPressed;
        }

        public void Render3d(IRenderer renderer, RenderMode mode)
        {

            foreach (var gameObject in _selection.Get().OfType<GameObject>())
            {
                //Highlight submesh BoundBox
            }
        }
    }
}
