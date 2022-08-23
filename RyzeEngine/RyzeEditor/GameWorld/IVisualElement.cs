using System;
using System.Collections.Generic;
using SharpDX;
using RyzeEditor.Renderer;

namespace RyzeEditor.GameWorld
{
	public interface IVisualElement
	{
		Vector3 Position { get; }

		BoundingBox BoundingBox { get; }

        RenderOptions Options { get; set; }

        void Render3d(IRenderer renderer, RenderMode mode);
	}

    [Serializable]
    public class RenderOptions
    {
        public RenderOptions()
        {
            SubMeshIds = new List<int>();
        }

        public Guid? GameObjectId { get; set; }

        public List<int> SubMeshIds { get; set; }

        public ShapeType ShapeType { get; set; }

        public Vector4 Color { get; set; }
    }

    public enum ShapeType
    {
        GeometryMesh,
        CollisionMesh,
        WheelMesh
    }
}