using System;
using System.Collections.Generic;
using SharpDX;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Helpers
{
    public static class GeometryShape
    {
        public const string Cone = "coneShape";
        public const string СonvexPlaneShape = "convexPlaneShape";
        public const string Sphere = "sphereShape";

        public static GeometryMesh FindShape(string meshId)
        {
            switch (meshId)
            {
                case Cone:
                    var coneMesh = BuildConeShape();
                    coneMesh.Id = meshId;
                    return coneMesh;

                case СonvexPlaneShape:
                    var convexPlanemesh = BuildСonvexPlaneShape();
                    convexPlanemesh.Id = meshId;
                    return convexPlanemesh;

                case Sphere:
                    var color = new Vector3(1.0f, 1.0f, 0.0f);
                    var sphereMesh = BuildSphereGeometry(color);
                    sphereMesh.Id = meshId;
                    return sphereMesh;

                default:
                    return null;
            };
        }

        public static GeometryMesh BuildСonvexPlaneShape(float size = 100.0f)
        {
            var vertexData = new List<Vertex>();
            var indices = new List<uint>();

            var p0 = new Vector3(-size, 0.0f, -size);
            var p1 = new Vector3(-size, 0.0f, size);
            var p2 = new Vector3(size,  0.0f, size);
            var p3 = new Vector3(size,  0.0f, -size);

            var n0 = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
            var n1 = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
            var n2 = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
            var n3 = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));

            vertexData.Add(new Vertex { Pos = p0, Norm = n0, Tex = new Vector2(0.0f, 1.0f) });
            vertexData.Add(new Vertex { Pos = p1, Norm = n1, Tex = new Vector2(0.0f, 0.0f) });
            vertexData.Add(new Vertex { Pos = p2, Norm = n2, Tex = new Vector2(1.0f, 0.0f) });
            vertexData.Add(new Vertex { Pos = p3, Norm = n3, Tex = new Vector2(1.0f, 1.0f) });

            indices.Add(0);
            indices.Add(1);
            indices.Add(2);

            indices.Add(0);
            indices.Add(2);
            indices.Add(3);

            var material = new Material(Vector3.One, Vector3.One, Vector3.One, 0.0f, 0)
            {
                DiffuseTexture = ResourceManager.Instance.GetTexture("crate_1.jpg")
            };

            var mesh = new GeometryMesh() { Id = "ConvexPlaneShape" };
            var subMesh = new SubMesh(1, 0)
            {
                VertexData = vertexData,
                Indices = new Dictionary<int, List<uint>> { [0] = indices },
                Materials = new List<Material>() { material },
                TessellationFactor = 4
            };
            mesh.SubMeshes.Add(subMesh);
            mesh.ComputeBoundBox();

            return mesh;
        }

        public static GeometryMesh BuildConeShape(float radius = 5.0f, float length = 30.0f, uint segments = 12)
        {
            var vertexData = new List<Vertex>();
            var indices = new List<uint>();
            var vector = new Vector4(0.0f, radius, 0.0f, 0.0f);
            var top = new Vector3(length, 0.0f, 0.0f);
            var center = Vector3.Zero;
            uint index = 0;
  
            for (uint i = 0; i < segments; i++)
            {
                var matrix = Matrix.RotationX((float)Math.PI * -i / 6.0f);
                Vector4.Transform(ref vector, ref matrix, out Vector4 pt);

                var point = new Vector3(pt.X, pt.Y, pt.Z);
                var dir1 = top - point;
                var dir2 = Vector3.Zero - point;
                var dir3 = Vector3.Cross(dir1, dir2);
                var normal = Vector3.Cross(dir1, dir3);
                normal.Normalize();

                vertexData.Add(new Vertex { Pos = point, Norm = normal });
                vertexData.Add(new Vertex { Pos = top, Norm = normal });

                indices.Add(index);
                indices.Add(++index);
                indices.Add(++index);
            }
            indices[3 * (int)segments - 1] = 0;

            for (uint i = 0; i < segments; i++)
            {
                var matrix = Matrix.RotationX((float)Math.PI * i / 6.0f);
                Vector4.Transform(ref vector, ref matrix, out Vector4 pt);

                var point = new Vector3(pt.X, pt.Y, pt.Z);
                var normal = new Vector3(-1.0f, 0.0f, 0.0f);

                vertexData.Add(new Vertex { Pos = point, Norm = normal });
                vertexData.Add(new Vertex { Pos = center, Norm = normal });

                indices.Add(index);
                indices.Add(++index);
                indices.Add(++index);
            }
            indices[6 * (int)segments - 1] = indices[3 * (int)segments];

            var mesh = new GeometryMesh() { Id = "ConeMesh" };
            mesh.SubMeshes.Add(new SubMesh(1, 0));
            mesh.SubMeshes[0].VertexData = vertexData;
            mesh.SubMeshes[0].Indices = new Dictionary<int, List<uint>> {[0] = indices};
            mesh.SubMeshes[0].Materials = new List<Material>() { new Material(Vector3.Zero, Vector3.Zero, Vector3.Zero, 0.0f, 0) };
            mesh.ComputeBoundBox();

            return mesh;
        }

        public static GeometryMesh BuildSphereGeometry(Vector3 color, float diameter = 1.0f, int tessellation = 16)
        {
            if (tessellation < 3) throw new ArgumentOutOfRangeException("tessellation", "Must be >= 3");

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            float radius = diameter / 2;

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i <= verticalSegments; i++)
            {
                float v = 1.0f - (float)i / verticalSegments;

                var latitude = (float)((i * Math.PI / verticalSegments) - Math.PI / 2.0);
                var dy = (float)Math.Sin(latitude);
                var dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    float u = (float)j / horizontalSegments;

                    var longitude = (float)(j * 2.0 * Math.PI / horizontalSegments);
                    var dx = (float)Math.Sin(longitude);
                    var dz = (float)Math.Cos(longitude);

                    dx *= dxz;
                    dz *= dxz;

                    var normal = new Vector3(dx, dy, dz);
                    var textureCoordinate = new Vector2(u, v);

                    var vertex = new Vertex
                    {
                        Pos = normal * radius,
                        Norm = normal,
                        Tex = textureCoordinate
                    };
                    vertices.Add(vertex);
                }
            }

            // Fill the index buffer with triangles joining each pair of latitude rings.
            int stride = horizontalSegments + 1;

            for (int i = 0; i < verticalSegments; i++)
            {
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % stride;

                    indices.Add((uint)(i * stride + j));
                    indices.Add((uint)(nextI * stride + j));
                    indices.Add((uint)(i * stride + nextJ));

                    indices.Add((uint)(i * stride + nextJ));
                    indices.Add((uint)(nextI * stride + j));
                    indices.Add((uint)(nextI * stride + nextJ));
                }
            }

            var mesh = new GeometryMesh() { Id = "SphereMesh" };
            mesh.SubMeshes.Add(new SubMesh(1, 0));
            mesh.SubMeshes[0].VertexData = vertices;
            mesh.SubMeshes[0].Indices = new Dictionary<int, List<uint>> { [0] = indices };
            mesh.SubMeshes[0].Materials = new List<Material>() { new Material(color, color, Vector3.Zero, 0.0f, 0) };
            mesh.ComputeBoundBox();
            mesh.ComputeBoundingSphere();

            return mesh;
        }
    }
}