using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;
using RyzeEditor.Helpers;

namespace RyzeEditor.Tools
{
	public class TranslationTool : ToolBase, IVisualElement
	{
		private const float ScaleDist = 0.002f;
		private const float ScalePos = 0.11f;
		private const int MouseMoveDelta = 5;

		private bool _leftMouseButtonPressed;
		private readonly GameObject _arrowAxisGameObject;
		private readonly bool[] _axisSelected = {false, false, false};
		private Point? _lastPoint;
		private Vector3? _lastIntersectPoint;
		private bool _dataChanged;
        private Vector4 ColorAxis = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

        private readonly List<Point3> _axisLinePoints = new List<Point3>() { new Point3(), new Point3() };

		public TranslationTool(WorldMap world, Selection selection) : base(world, selection)
		{
			var arrowMesh = new List<string> { GeometryShape.Cone };

            _arrowAxisGameObject = new GameObject(arrowMesh, arrowMesh)
			{
				Scale = new Vector3(1.0f, 1.0f, 1.0f),
				Position = new Vector3(0.0f, 0.0f, 0.0f),
				Rotation = Quaternion.Identity
			};
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
			_leftMouseButtonPressed = true;
			_dataChanged = false;

            return true;
		}

		public override bool OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
		{
			_leftMouseButtonPressed = false;
			_lastPoint = null;
			_lastIntersectPoint = null;

            if (_dataChanged)
			{
				WorldMap.CommitChanges();
			}

            Cursor.Current = Cursors.Default;

            return true;
		}

		public override bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
            _axisSelected[0] = _axisSelected[1] = _axisSelected[2] = false;

            var gameObjects = _selection.Get().OfType<GameObject>().ToList();

            if (gameObjects == null || gameObjects.Count == 0)
            {
                return true;
            }

            var positions = gameObjects.Select(x => x.Position).ToList();
            var average = positions.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / positions.Count;
            var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);
			var distance = (average - _world.Camera.Position).Length();

            _arrowAxisGameObject.Position = new Vector3(average.X + ScalePos * distance, average.Y, average.Z);
            _arrowAxisGameObject.Scale = new Vector3(ScaleDist * distance, ScaleDist * distance, ScaleDist * distance);
            _arrowAxisGameObject.Rotation = Quaternion.Identity;

            if (_arrowAxisGameObject.Intersects(ray, out RayPickData intersectData))
			{
				_axisSelected[0] = true;
			}

			if (_axisSelected[0] == false)
			{
                _arrowAxisGameObject.Position = new Vector3(average.X, average.Y + ScalePos * distance, average.Z);
                _arrowAxisGameObject.Rotation = Quaternion.RotationYawPitchRoll(0.0f, 0.0f, 1.0f * (float)Math.PI / 2.0f);

				if (_arrowAxisGameObject.Intersects(ray, out intersectData))
				{
					_axisSelected[1] = true;
				}
			}

			if (_axisSelected[0] == false && _axisSelected[1] == false)
			{
                _arrowAxisGameObject.Position = new Vector3(average.X, average.Y, average.Z + ScalePos * distance);
                _arrowAxisGameObject.Rotation = Quaternion.RotationYawPitchRoll(-1.0f * (float)Math.PI / 2.0f, 0.0f, 0.0f);

                if (_arrowAxisGameObject.Intersects(ray, out intersectData))
				{
					_axisSelected[2] = true;
				}				
			}

            if (intersectData == null)
			{
                Cursor.Current = Cursors.Default;
                return true;
            }

            Cursor.Current = Cursors.Hand;

            if (!_leftMouseButtonPressed)
            {
                return true;
            }

            if (_lastPoint == null)
			{
				_lastPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);
			}

			if ((Math.Abs(_lastPoint.Value.X - mouseEventArgs.X) <= MouseMoveDelta) && Math.Abs(_lastPoint.Value.Y - mouseEventArgs.Y) <= MouseMoveDelta)
			{
                return true;
            }

            var plane = new Plane(Vector3.Zero, Vector3.UnitY);

            if (_axisSelected[0] || _axisSelected[1])
            {
                plane = new Plane(Vector3.Zero, Vector3.UnitZ);
            }

            var intersectPoint = Vector3.Zero;
            if (!ray.Intersects(ref plane, out intersectPoint))
            {
                return true;
            }

            if (_lastIntersectPoint == null)
			{
				_lastIntersectPoint = intersectPoint;
			}

			if (_lastIntersectPoint == Vector3.Zero)
            {
                return true;
            }
            
            foreach (var gameObject in gameObjects)
            {
                gameObject.Translate(GetSelectedAxis(), intersectPoint, _lastIntersectPoint);
            }
			
			_dataChanged = true;
			_lastIntersectPoint = intersectPoint;

            return true;
		}

        public override bool OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            return true;
        }

        public void Render3d(IRenderer renderer, RenderMode mode)
		{
            var gameObjects = _selection.Get().OfType<GameObject>().ToList();

            if (gameObjects == null || gameObjects.Count == 0)
            {
                return;
            }

            var positions = gameObjects.Select(x => x.Position).ToList();
            var average = positions.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / positions.Count;

            var distance = (average - _world.Camera.Position).Length();

            _axisLinePoints[0] = new Point3(new Vector4(average, 0), ColorAxis);

            //Draw X Axis
            _arrowAxisGameObject.Position = new Vector3(average.X + ScalePos * distance, average.Y, average.Z);
            _arrowAxisGameObject.Rotation = Quaternion.Identity;
            _arrowAxisGameObject.Scale = new Vector3(ScaleDist * distance, ScaleDist * distance, ScaleDist * distance);
            _arrowAxisGameObject.GeometryMeshes[0].SetDiffuseColor(_axisSelected[0] ? new Color3(1.0f, 1.0f, 0.0f) : new Color3(1.0f, 0.0f, 0.0f));
            _arrowAxisGameObject.Render3d(renderer, mode);

            _axisLinePoints[1] = new Point3(new Vector4(average.X + ScalePos * distance, average.Y, average.Z, 0), ColorAxis);
            renderer.DrawLineStrip(_axisLinePoints, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });

            //Draw Y Axis
            _arrowAxisGameObject.Position = new Vector3(average.X, average.Y + ScalePos * distance, average.Z);
            _arrowAxisGameObject.Rotation = Quaternion.RotationYawPitchRoll(0.0f, 0.0f, 1.0f * (float)Math.PI / 2.0f);
            _arrowAxisGameObject.GeometryMeshes[0].SetDiffuseColor(_axisSelected[1] ? new Color3(1.0f, 1.0f, 0.0f) : new Color3(0.0f, 1.0f, 0.0f));
            _arrowAxisGameObject.Render3d(renderer, mode);

            _axisLinePoints[1] = new Point3(new Vector4(average.X, average.Y + ScalePos * distance, average.Z, 0), ColorAxis);
            renderer.DrawLineStrip(_axisLinePoints, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });

            //Draw Z Axis
            _arrowAxisGameObject.Position = new Vector3(average.X, average.Y, average.Z + ScalePos * distance);
            _arrowAxisGameObject.Rotation = Quaternion.RotationYawPitchRoll(-1.0f * (float)Math.PI / 2.0f, 0.0f, 0.0f);
            _arrowAxisGameObject.GeometryMeshes[0].SetDiffuseColor(_axisSelected[2] ? new Color3(1.0f, 1.0f, 0.0f) : new Color3(0.0f, 0.0f, 1.0f));
            _arrowAxisGameObject.Render3d(renderer, mode);

            _axisLinePoints[1] = new Point3(new Vector4(average.X, average.Y, average.Z + ScalePos * distance, 0), ColorAxis);
            renderer.DrawLineStrip(_axisLinePoints, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
        }

		private Axis GetSelectedAxis()
		{
			var axis = Axis.None;

			if (_axisSelected[0])
			{
				axis = Axis.X;
			}
			else if (_axisSelected[1])
			{
				axis = Axis.Y;
			}
			else if (_axisSelected[2])
			{
				axis = Axis.Z;
			}

			return axis;
		}
	}
}