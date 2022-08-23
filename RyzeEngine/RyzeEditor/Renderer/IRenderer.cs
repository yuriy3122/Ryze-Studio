using System;
using System.Collections.Generic;
using System.Drawing;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;
using SharpDX;

namespace RyzeEditor.Renderer
{
    public class Point3
    {
        public Vector4 Position;
        public Vector4 Color;

        public Point3()
        {
            Position = new Vector4();
            Color = new Vector4();
        }

        public Point3(Vector4 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }
    }

    public struct VertexPositionTex
	{
        public Vector4 Position;
        public Vector2 Tex;

        public VertexPositionTex(Vector4 position, Vector2 tex)
        {
            Position = position;
            Tex = tex;
        }
    }

    public struct RenderMode
    {
        public bool BoundBox;
		public bool IsDepthClipEnabled;
        public List<int> SubMeshIds;
        public Vector4 Color;
	}

	public interface IRenderer
	{
		void Initialize(IntPtr handle, Camera camera);

		void PreRender();

        void PostRender();

		void ResizeWindow(Size wndSize);

		void ClearDepthStencilView();

		void Present();

		void Dispose();

		//drawing primitives
		void DrawLineStrip(IEnumerable<Point3> points, RenderMode mode);

		//drawing complex objects
		void DrawMeshInstanced(IMesh mesh, Matrix[] matrices, RenderMode mode);
	}
}