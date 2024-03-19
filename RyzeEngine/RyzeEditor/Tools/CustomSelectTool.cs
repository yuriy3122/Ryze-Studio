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
        GameObject _nearestToCameraObject;
        int? _subMeshIndex;

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
            return true;
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

        public override bool OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return false;
            }

            return true;
        }

        public override bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                return false;
            }

            var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);

            float distance = float.MaxValue;

            var intersects = GetGeometryIntersections(ray);

            foreach (var intersect in intersects)
            {
                var dist = (intersect.Point - _world.Camera.Position).Length();

                if (dist < distance)
                {
                    distance = dist;
                    _subMeshIndex = intersect.SubMeshIndex;
                    _nearestToCameraObject = _world.Entities.OfType<GameObject>().Where(x => x.Id == intersect.GameObjectId).FirstOrDefault();
                }
            }

            return true;
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
