using System;
using SharpDX;
using RyzeEditor.Extentions;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Helpers
{
    public static class PatchHelper
    {
        public static PatchData CalculatePatchDataForTriangle(Vertex v0, Vertex v1, Vertex v2)
        {
            PatchData patchData = new PatchData();

            var p1 = v0.Pos;
            var p2 = v1.Pos;
            var p3 = v2.Pos;
            var n1 = v0.Norm;
            var n2 = v1.Norm;
            var n3 = v2.Norm;

            //Geometry
            var w12 = Vector3.Dot((p2 - p1), n1);
            var b210 = (2 * p1 + p2 - w12 * n1) / 3.0f;

            var w21 = Vector3.Dot((p1 - p2), n2);
            var b120 = (2 * p2 + p1 - w21 * n2) / 3.0f;

            var w13 = Vector3.Dot((p3 - p1), n1);
            var b201 = (2 * p1 + p3 - w13 * n1) / 3.0f;

            var w31 = Vector3.Dot((p1 - p3), n3);
            var b102 = (2 * p3 + p1 - w31 * n3) / 3.0f;

            var w23 = Vector3.Dot((p3 - p2), n2);
            var b021 = (2 * p2 + p3 - w23 * n2) / 3.0f;

            var w32 = Vector3.Dot((p2 - p3), n3);
            var b012 = (2 * p3 + p2 - w32 * n3) / 3.0f;

            var e = (b210 + b120 + b201 + b102 + b021 + b012) / 6.0f;
            var v = (p1 + p2 + p3) / 3.0f;

            var b111 = e + (e - v) / 2.0f;

            patchData.Positions.Add(b210);
            patchData.Positions.Add(b120);
    
            patchData.Positions.Add(b201);
            patchData.Positions.Add(b102);

            patchData.Positions.Add(b021);
            patchData.Positions.Add(b012);

            patchData.Positions.Add(b111);

            //Normals
            var v12 = Vector3.Dot(p2 - p1, n1 + n2) / Vector3.Dot(p2 - p1, p2 - p1);
            var h110 = n1 + n2 - 2.0f * v12 * (p2 - p1);
            var n110 = Vector3.Normalize(h110);

            var v23 = Vector3.Dot(p3 - p2, n2 + n3) / Vector3.Dot(p3 - p2, p3 - p2);
            var h011 = n2 + n3 - 2.0f * v23 * (p3 - p2);
            var n011 = Vector3.Normalize(h011);

            var v31 = Vector3.Dot(p3 - p1, n1 + n3) / Vector3.Dot(p3 - p1, p3 - p1);
            var h101 = n3 + n1 - 2.0f * v31 * (p1 - p3);
            var n101 = Vector3.Normalize(h101);

            patchData.Normals.Add(n110);
            patchData.Normals.Add(n011);
            patchData.Normals.Add(n101);

            return patchData;
        }

        public static Vertex TestPatchData(Vertex v0, Vertex v1, Vertex v2, PatchData patch, Vector3 uvw)
        {
            Vertex vertex = new Vertex();

            float u = uvw.X;
            float v = uvw.Y;
            float w = uvw.Z;

            Vector3 b300 = v0.Pos;
            Vector3 b030 = v1.Pos;
            Vector3 b003 = v2.Pos;

            Vector3 n200 = v0.Norm;
            Vector3 n020 = v1.Norm;
            Vector3 n002 = v2.Norm;

            Vector3 b210 = patch.Positions[0];
            Vector3 b120 = patch.Positions[1];
            Vector3 b201 = patch.Positions[2];
            Vector3 b102 = patch.Positions[3];
            Vector3 b021 = patch.Positions[4];
            Vector3 b012 = patch.Positions[5];
            Vector3 b111 = patch.Positions[6];

            Vector3 n110 = patch.Normals[0];
            Vector3 n011 = patch.Normals[1];
            Vector3 n101 = patch.Normals[2];

            vertex.Pos = b300 * (float)Math.Pow(w, 3.0f) + b030 * (float)Math.Pow(u, 3) + b003 * (float)Math.Pow(v, 3) +
            b210 * (float)Math.Pow(w, 2) * u + b120 * w * (float)Math.Pow(u, 2) + b201 * (float)Math.Pow(w, 2) * v +
            b021 * (float)Math.Pow(u, 2) * v + b102 * w * (float)Math.Pow(v, 2) + b012 * u * (float)Math.Pow(v, 2) +
            b111 * w * u * v;

            vertex.Norm = n200 * (float)Math.Pow(w, 2) + n020 * (float)Math.Pow(u, 2) + n002 * (float)Math.Pow(v, 2) + n110 * w * u + n011 * u * v + n101 * w * v;

            return vertex;
        }
    }
}
