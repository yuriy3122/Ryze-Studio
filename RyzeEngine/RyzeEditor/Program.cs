using System;

namespace RyzeEditor
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
            var editorApp = new EditorApp();
            editorApp.Run();
		}
    }
}