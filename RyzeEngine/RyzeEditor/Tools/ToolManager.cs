using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SharpDX.RawInput;
using RyzeEditor.GameWorld;
using RyzeEditor.Helpers;

namespace RyzeEditor.Tools
{
	public class ToolManager : IToolManager
	{
        private readonly Selection _selection;
		private readonly WorldMap _world;

		public ToolManager(WorldMap world, Selection selection)
		{
			_world = world;
			_selection = selection;

            foreach (Tool tool in (Tool[])Enum.GetValues(typeof(Tool)))
            {
                AddTool(tool);
            }
        }

		public WorldMap WorldMap
		{
			get
			{
				return _world;
			}
			set
			{
				foreach (var item in Tools.OfType<ToolBase>())
				{
					item.WorldMap = value;
				}
			}
		}

		public ITool GetFirstActiveTool()
		{
			return Tools.FirstOrDefault(tool => tool.IsActive && !tool.IsAlwaysActive());
		}

		public void SetActiveTool(Tool id)
		{
			foreach (var tool in Tools.Where(tool => !tool.IsAlwaysActive()))
			{
				tool.IsActive = false;
			}

            var toolBase = Tools.OrderBy(type => LevenshteinDistance.Compute(type.GetType().Name, id.ToString())).FirstOrDefault();

            if (toolBase != null)
            {
                toolBase.IsActive = true;
            }
		}

        public void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            var processed = false;

            foreach (var tool in Tools.Where(tool => tool.IsActive && !tool.IsAlwaysActive()))
            {
                processed = tool.OnMouseDown(sender, mouseEventArgs);
            }

            if (!processed)
            {
                var tools = Tools.Where(x => x.IsAlwaysActive());

                if (tools.Any())
                {
                    var tool = tools.FirstOrDefault();
                    tool.OnMouseDown(sender, mouseEventArgs);
                }
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
		{
            var processed = false;

            foreach (var tool in Tools.Where(tool => tool.IsActive && !tool.IsAlwaysActive()))
			{
                processed = tool.OnMouseUp(sender, mouseEventArgs);
            }

            if (!processed)
            {
                var tools = Tools.Where(x => x.IsAlwaysActive());

                if (tools.Any())
                {
                    var tool = tools.FirstOrDefault();
                    tool.OnMouseUp(sender, mouseEventArgs);
                }
            }
        }

		public void OnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
            var processed = false;

            foreach (var tool in Tools.Where(tool => tool.IsActive && !tool.IsAlwaysActive()))
			{
                processed = tool.OnMouseMove(sender, mouseEventArgs);
            }

            if (!processed)
            {
                var tools = Tools.Where(x => x.IsAlwaysActive());

                if (tools.Any())
                {
                    var tool = tools.FirstOrDefault();
                    tool.OnMouseMove(sender, mouseEventArgs);
                }
            }
        }

        public void OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            var processed = false;

            foreach (var tool in Tools.Where(tool => tool.IsActive && !tool.IsAlwaysActive()))
            {
                processed = tool.OnMouseWheel(sender, mouseEventArgs);
            }

            if (!processed)
            {
                var tools = Tools.Where(x => x.IsAlwaysActive());

                if (tools.Any())
                {
                    var tool = tools.FirstOrDefault();
                    tool.OnMouseWheel(sender, mouseEventArgs);
                }
            }
        }

        public void OnKeyboardInput(object sender, KeyboardInputEventArgs keyboardInputEventArgs)
		{
            var processed = false;

            foreach (var tool in Tools.Where(tool => tool.IsActive && !tool.IsAlwaysActive()))
			{
                processed = tool.OnKeyboardInput(sender, keyboardInputEventArgs);
			}

            if (!processed)
            {
                var tools = Tools.Where(x => x.IsAlwaysActive());

                if (tools.Any())
                {
                    var tool = tools.FirstOrDefault();
                    tool.OnKeyboardInput(sender, keyboardInputEventArgs);
                }
            }
        }

        public List<ITool> Tools { get; } = new List<ITool>();

        private void AddTool(Tool id)
        {
            var derivedType = typeof(ToolBase);

            var toolType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetExportedTypes())
                .Where(type => type.IsClass && derivedType.IsAssignableFrom(type))
                .OrderBy(type => LevenshteinDistance.Compute(type.Name, id.ToString()))
                .FirstOrDefault();

            var obj = Activator.CreateInstance(toolType, _world, _selection);

            if (obj is ToolBase tool)
            {
                tool.IsActive = tool.IsAlwaysActive();
                Tools.Add(tool);
            }
        }
    }

	public enum Tool
	{
		Rotation,
		Translation,
		Select,
        CustomSelect,
        Collision,
        Placement,
		Camera,
        PointLight,
        Vehicle
	}

	public interface IToolManager
	{
		List<ITool> Tools { get; }
	}
}