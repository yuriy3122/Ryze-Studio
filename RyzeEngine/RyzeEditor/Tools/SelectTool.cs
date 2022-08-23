using System.Linq;
using System.Windows.Forms;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using SharpDX;
using SharpDX.RawInput;

namespace RyzeEditor.Tools
{
	public class SelectTool : ToolBase, IVisualElement
	{
        bool _controlKeyPressed;

        public SelectTool(WorldMap world, Selection selection) : base(world, selection)
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

            var intersects = GetGeometryIntersections(ray);

            foreach (var intersect in intersects)
            {
                var dist = (intersect.Point - _world.Camera.Position).Length();
                if (dist < distance)
                {
                    distance = dist;
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
            var result = false;
            _controlKeyPressed = false;

            switch (arg.Key)
			{
				case Keys.Delete:
					var gameObject = _selection.Get().FirstOrDefault() as GameObject;

					if (gameObject != null)
					{
						_world.RemoveEntity(gameObject);
                        result = true;
					}

					break;

                case Keys.ControlKey:
                    _controlKeyPressed = arg.State == KeyState.KeyDown;
                    result = _controlKeyPressed;
                    break;
            }

            return result;
		}

		public void Render3d(IRenderer renderer, RenderMode mode)
		{
            foreach (var gameObject in _selection.Get().OfType<GameObject>())
            {
                var min = gameObject.BoundingBox.Minimum;
                var max = gameObject.BoundingBox.Maximum;
                var color = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

                RenderHelper.RenderBox(renderer, min, max, color, gameObject.WorldMatrix);
            }
		}
	}
}
