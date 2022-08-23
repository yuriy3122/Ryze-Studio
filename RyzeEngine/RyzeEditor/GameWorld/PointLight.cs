using System;
using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.GameWorld
{
    [Serializable]
    public class PointLight : GameObject
    {
        public PointLight(List<string> geometryMeshIds, List<string> collisionMeshIds) : base(geometryMeshIds, collisionMeshIds)
        {
        }

        public Vector3 Color { get; set; }

        public Vector3 Direction { get; set; }

        public float Radius { get; set; }

        public float Intensity { get; set; }
    }
}
