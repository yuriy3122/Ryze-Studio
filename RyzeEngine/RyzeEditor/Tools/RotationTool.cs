using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;

namespace RyzeEditor.Tools
{
	public class RotationTool : ToolBase, IVisualElement
	{
		private const int Segments = 180;
		private const int MouseMoveDelta = 2;
		private const float RadiusDelta = 1.0f;

		private float _toolRadius;
		readonly List<Point3> _pointsRotationX = new List<Point3>();
		readonly List<Point3> _pointsRotationY = new List<Point3>();
		readonly List<Point3> _pointsRotationZ = new List<Point3>();
		private bool _leftMouseButtonPressed;
		private Point? _lastPoint;
		private Vector3? _prevRotationVector;
		private Axis _axis;
		private bool _dataChanged;

		public RotationTool(WorldMap world, Selection selection) : base(world, selection)
		{
			for (int i = 0; i < Segments; i++)
			{
				_pointsRotationX.Add(new Point3());
				_pointsRotationY.Add(new Point3());
				_pointsRotationZ.Add(new Point3());
			}
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
			_prevRotationVector = null;
			_axis = Axis.None;

			if (_dataChanged)
			{
				WorldMap.CommitChanges();
				_dataChanged = false;
			}

            Cursor.Current = Cursors.Default;

            return true;
		}

		public override bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
            var gameObjects = _selection.Get().OfType<GameObject>().ToList();

            if (gameObjects == null || gameObjects.Count == 0)
            {
                return true;
            }

            var positions = gameObjects.Select(x => x.Position).ToList();
            var average = positions.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / positions.Count;

            if (_lastPoint == null)
			{
				_lastPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);
			}

			if ((Math.Abs(_lastPoint.Value.X - mouseEventArgs.X) <= MouseMoveDelta) && 
                 Math.Abs(_lastPoint.Value.Y - mouseEventArgs.Y) <= MouseMoveDelta)
			{
				return true;
			}

            _axis = Axis.None;

            _lastPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);

			var ray = _world.Camera.GetPickRay(mouseEventArgs.X, mouseEventArgs.Y);
			var sphere = new BoundingSphere(average, _toolRadius + RadiusDelta);

            if (!Collision.RayIntersectsSphere(ref ray, ref sphere, out Vector3 point))
            {
                _prevRotationVector = null;
                return true;
            }

            float d1 = 1.0f;
			float d2 = 1.0f;
			float d3 = 1.0f;

			var p1 = CalcNearestPoint(_pointsRotationX, ray, ref d1);
			var p2 = CalcNearestPoint(_pointsRotationY, ray, ref d2);
			var p3 = CalcNearestPoint(_pointsRotationZ, ray, ref d3);

			var rot = Axis.None;
			var vec = Vector3.Zero;

			if (d1 < d2 && d1 < d3)
			{
				rot = Axis.X;
				vec = Vector3.Normalize(p1 - average);
			}
			else if (d2 < d1 && d2 < d3)
			{
				rot = Axis.Y;
				vec = Vector3.Normalize(p2 - average);			
			}
			else if (d3 < d1 && d3 < d2)
			{
				rot = Axis.Z;
				vec = Vector3.Normalize(p3 - average);
			}

            _axis = rot;

            if (_prevRotationVector == null)
			{
				_prevRotationVector = vec;
			}
			else
			{
				if ((vec - _prevRotationVector.Value).Length() < 0.0000001f)
				{
					return true;
				}

                if (_leftMouseButtonPressed)
                {
                    Cursor.Current = Cursors.Hand;

                    var angle = Convert.ToSingle(Math.Acos(Vector3.Dot(vec, _prevRotationVector.Value)));

                    var dir = Vector3.Cross(_prevRotationVector.Value, vec);

                    if (angle < 0.000001f || angle > Math.PI / 4.0f)
                    {
                        return true;
                    }

                    foreach (var gameObject in gameObjects)
                    {
                        gameObject.Rotate(_axis, dir, angle);
                    }

                    _dataChanged = true;
                }

				_prevRotationVector = vec;
			}

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
            var rotations = gameObjects.Select(x => x.Rotation).ToList();
            var averagePos = positions.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / positions.Count;

            var distance = (averagePos - _world.Camera.Position).Length();
			_toolRadius = 0.1f * distance;

			var vz = new Vector4(0.0f, 0.0f, _toolRadius, 1.0f);

			for (int i = 0; i < Segments; i++)
			{
                var norm = Vector3.Normalize(_world.Camera.LookAtDir - _world.Camera.Position);

                var matrix = Matrix.RotationX((float)Math.PI * i / 90.0f) * Matrix.Translation(averagePos);
                Vector4.Transform(ref vz, ref matrix, out _pointsRotationX[i].Position);
                var pos = _pointsRotationX[i].Position;
                var d = Vector3.Dot((new Vector3(pos.X, pos.Y, pos.Z) - _world.Camera.Position), norm);

                _pointsRotationX[i].Color = d > distance ? new Vector4(0.2f, 0.1f, 0.1f, 0.0f) :
                    _axis == Axis.X ? new Vector4(1.0f, 1.0f, 0.0f, 0.0f) : new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

                matrix = Matrix.RotationY((float)Math.PI * i / 90.0f) * Matrix.Translation(averagePos);
                Vector4.Transform(ref vz, ref matrix, out _pointsRotationY[i].Position);
                pos = _pointsRotationY[i].Position;
                d = Vector3.Dot((new Vector3(pos.X, pos.Y, pos.Z) - _world.Camera.Position), norm);

                _pointsRotationY[i].Color = d > distance ? new Vector4(0.1f, 0.2f, 0.1f, 0.0f) :
                    _axis == Axis.Y ? new Vector4(1.0f, 1.0f, 0.0f, 0.0f) : new Vector4(0.0f, 1.0f, 0.0f, 0.0f);

                var vy = new Vector4(0.0f, _toolRadius, 0.0f, 1.0f);
                matrix = Matrix.RotationZ((float)Math.PI * i / 90.0f) * Matrix.Translation(averagePos);
                Vector4.Transform(ref vy, ref matrix, out _pointsRotationZ[i].Position);
                pos = _pointsRotationZ[i].Position;
                d = Vector3.Dot((new Vector3(pos.X, pos.Y, pos.Z) - _world.Camera.Position), norm);

                _pointsRotationZ[i].Color = d > distance ? new Vector4(0.1f, 0.1f, 0.2f, 0.0f) :
                    _axis == Axis.Z ? new Vector4(1.0f, 1.0f, 0.0f, 0.0f) : new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            }

			renderer.DrawLineStrip(_pointsRotationX, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
			renderer.DrawLineStrip(_pointsRotationY, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
			renderer.DrawLineStrip(_pointsRotationZ, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}

		private static Vector3 CalcNearestPoint(List<Point3> points, Ray ray, ref float distance)
		{
			var vec = Vector3.Zero;
			foreach (var pt in points)
			{
				if (pt.Color.Length() < 1.0f) continue;

				var p = new Vector3(pt.Position.X, pt.Position.Y, pt.Position.Z);
				var d = Vector3.Cross(ray.Direction, p - ray.Position).Length();

				if (d > distance) continue;

				distance = d;
				vec = p;
			}

			return vec;
		}
	}
}