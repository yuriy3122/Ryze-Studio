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

    public class RenderMode
    {
        public static int ShadowMapCascadeNumber = 2;

        public bool ShadowMap { get; set; }
        public int ShadowMapCascadeIndex { get; set; }
        public bool BoundBox { get; set; }
        public bool IsDepthClipEnabled { get; set; }
        public List<int> SubMeshIds { get; set; }
        public Vector4 Color { get; set; }
        public Vector3 DirectLightDir { get; set; }
    }

	public interface IRenderer
	{
		void Initialize(IntPtr handle, Camera camera);

        void PreRenderShadowMap(int cascadeNumber);

        void PreRender();

        void PostRender();

		void ResizeWindow(Size wndSize);

		void ClearDepthStencilView();

		void Present();

		void Dispose();

		void DrawLineStrip(IEnumerable<Point3> points, RenderMode mode);

		void DrawMeshInstanced(IMesh mesh, Matrix[] matrices, RenderMode mode);
	}
}