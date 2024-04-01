using SharpDX;
using System;

namespace RyzeEditor.SkinnedAnimation
{
    [Serializable]
    public struct SkinnedVertex
    {
        public Vector3 Pos;
        public Vector3 Norm;
        public Vector3 Tangent;
        public Vector3 Bitangent;
        public Vector2 Tex;
        public ushort[] JointIndices;
        public float[] JointWeights;

        public SkinnedVertex(Vector3 pos, Vector3 norm, Vector3 tangent, Vector3 bitangent, Vector2 tex)
        {
            Pos = pos;
            Norm = norm;
            Tangent = tangent;
            Bitangent = bitangent;
            Tex = tex;
            JointIndices = new ushort[4];
            JointWeights = new float[3];
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var vertex = (SkinnedVertex)obj;
            var epsilon = new Vector3(1e-8f, 1e-8f, 1e-8f);

            if (!Vector3.NearEqual(vertex.Pos, Pos, epsilon))
            {
                return false;
            }

            if (!Vector3.NearEqual(vertex.Norm, Norm, epsilon))
            {
                return false;
            }

            if (!Vector3.NearEqual(vertex.Tangent, Tangent, epsilon))
            {
                return false;
            }

            if (!Vector3.NearEqual(vertex.Bitangent, Bitangent, epsilon))
            {
                return false;
            }

            if (Vector2.Distance(vertex.Tex, Tex) > 0.00001f)
            {
                return false;
            }

            for (int i = 0; i < 4; i++)
            {
                if (JointIndices[i] != vertex.JointIndices[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (JointWeights[i] != vertex.JointWeights[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Pos.GetHashCode();
            hash = hash * 23 + Norm.GetHashCode();
            hash = hash * 23 + Tex.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"Pos: [{Pos.X}, {Pos.Y}, {Pos.Z}] Tex: [{Tex.X}, {Tex.Y}] Norm: [{Norm.X}, {Norm.Y}, {Norm.Z}] Tangent: [{Tangent.X}, {Tangent.Y}, {Tangent.Z}] Bitangent: [{Bitangent.X}, {Bitangent.Y}, {Bitangent.Z}]";
        }
    }
}
