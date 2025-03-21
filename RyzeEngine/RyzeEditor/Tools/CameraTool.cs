﻿using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.RawInput;
using RyzeEditor.GameWorld;
using RyzeEditor.Renderer;

namespace RyzeEditor.Tools
{
	public class CameraTool:  ToolBase, IVisualElement
	{
        private bool _rightMouseButtonPressed;
        private Point? _lastPoint;

        private const int MouseMoveDelta = 2;

        public CameraTool(WorldMap world, Selection selection) : base(world, selection)
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

        public override bool IsAlwaysActive()
		{
			return true;
		}

        public override bool OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            _rightMouseButtonPressed = false;
            _lastPoint = null;

            return true;
        }

        public override bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == MouseButtons.Right)
            {
                _rightMouseButtonPressed = true;
            }

            return true;
        }

        public override bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_rightMouseButtonPressed == false)
            {
                return true;
            }

            if (_lastPoint == null)
            {
                _lastPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);
            }

            if ((Math.Abs(_lastPoint.Value.X - mouseEventArgs.X) <= MouseMoveDelta) &&
                 Math.Abs(_lastPoint.Value.Y - mouseEventArgs.Y) <= MouseMoveDelta)
            {
                return true;
            }

            var point = new Point(mouseEventArgs.X, mouseEventArgs.Y);
            var deltaX = point.X - _lastPoint.Value.X;
            var deltaY = point.Y - _lastPoint.Value.Y;

            const float step = 0.001f;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                _world.Camera.RotateY(step * deltaX);
            }
            else
            {
                _world.Camera.RotateX(step * deltaY);
            }

            _lastPoint = point;

            return true;
        }

        public override bool OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            _world.Camera.Zoom(mouseEventArgs.Delta / 100.0f);

            return true;
        }

        public override bool OnKeyboardInput(object sender, KeyboardInputEventArgs arg)
		{
			switch (arg.Key)
			{
				case Keys.A:
					_world.Camera.Strafe(0.125f);
					break;
				case Keys.D:
					_world.Camera.Strafe(-0.125f);
					break;
				case Keys.W:
					_world.Camera.Walk(0.125f);
					break;
				case Keys.S:
					_world.Camera.Walk(-0.125f);
					break;
			}

            return true;
		}

		public void Render3d(IRenderer renderer, RenderMode mode)
		{
		}
	}
}
