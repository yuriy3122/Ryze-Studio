using System;
using System.Windows.Forms;
using SharpDX.RawInput;

namespace RyzeEditor.Tools
{
	public interface ITool
	{
		Guid Id { get; set; }

		bool IsActive { get; set; }

		bool IsAlwaysActive();

		bool OnMouseUp(object sender, MouseEventArgs mouseEventArgs);

		bool OnMouseMove(object sender, MouseEventArgs mouseEventArgs);

		bool OnMouseDown(object sender, MouseEventArgs mouseEventArgs);

        bool OnMouseWheel(object sender, MouseEventArgs mouseEventArgs);

        bool OnKeyboardInput(object sender, KeyboardInputEventArgs keyboardInputEventArgs);
    }
}
