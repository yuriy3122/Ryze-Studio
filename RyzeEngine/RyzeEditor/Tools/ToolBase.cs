using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX.RawInput;
using SharpDX;
using RyzeEditor.GameWorld;

namespace RyzeEditor.Tools
{
	public abstract class ToolBase : ITool
	{
		protected WorldMap _world;

		protected Selection _selection;

		public Guid Id { get; set; }

		public bool IsActive { get; set; }

		public WorldMap WorldMap
		{
			get
			{
				return _world;
			}
			set
			{
				_world = value;
			}
		}

		public virtual bool IsAlwaysActive()
		{
			return false;
		}

		public virtual bool OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
		{
            return false;
		}

		public virtual bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
            return false;
        }

		public virtual bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
		{
            return false;
        }

        public virtual bool OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            return false;
        }

        public virtual bool OnKeyboardInput(object sender, KeyboardInputEventArgs keyboardInputEventArgs)
		{
            return false;
        }

		protected ToolBase(WorldMap world, Selection selection)
		{
			Id = Guid.NewGuid();
			_world = world;
			_selection = selection;
		}

        protected List<RayPickData> GetGeometryIntersections(Ray ray)
        {
            var intersects = new List<RayPickData>();

            foreach (var gameObject in _world.Entities.OfType<GameObject>())
            {
                if (gameObject.GeometryMeshes == null || gameObject.GeometryMeshes.Count == 0)
                {
                    continue;
                }

                if (gameObject.Intersects(ray, out RayPickData data))
                {
                    intersects.Add(data);
                }
            }

            return intersects;
        }
    }
}
