using System;
using SharpDX;
using RyzeEditor.GameWorld;
using RyzeEditor.ResourceManagment;

namespace RyzeEditor.Helpers
{
    public static class CollisionTests
    {
        public static bool MeshRayIntersects(IMesh mesh, Ray ray, Matrix worldMatrix, out RayPickData data)
        {
            data = null;

            var toLocal = Matrix.Invert(worldMatrix);

            Vector3.TransformNormal(ref ray.Direction, ref toLocal, out Vector3 dirObject);
            dirObject.Normalize();

            Vector3.TransformCoordinate(ref ray.Position, ref toLocal, out Vector3 posObject);
            var rayToObject = new Ray(posObject, dirObject);

            if (!rayToObject.Intersects(mesh.BoundingBox))
            {
                return false;
            }

            float distance = float.MaxValue;

            for (int i = 0; i < mesh.SubMeshes.Count; i++)
            {
                var subMesh = mesh.SubMeshes[i];
                var matrix = Matrix.Invert(subMesh.GetMatrix(mesh));

                Vector3.TransformNormal(ref dirObject, ref matrix, out Vector3 dirLocal);
                dirLocal.Normalize();

                Vector3.TransformCoordinate(ref posObject, ref matrix, out Vector3 posLocal);
                var rayToLocal = new Ray(posLocal, dirLocal);

                for (int j = 0; j < subMesh.Materials.Count; j++)
                {
                    if (!subMesh.Indices.ContainsKey(j))
                    {
                        continue;
                    }

                    var indices = subMesh.Indices[j];

                    for (int k = 0; k < indices.Count; k += 3)
                    {
                        var v1 = subMesh.VertexData[(int)indices[k + 0]];
                        var v2 = subMesh.VertexData[(int)indices[k + 1]];
                        var v3 = subMesh.VertexData[(int)indices[k + 2]];

                        if (rayToLocal.Intersects(ref v1.Pos, ref v2.Pos, ref v3.Pos, out Vector3 point) == false)
                        {
                            continue;
                        }

                        Matrix matrixTM = subMesh.GetMatrix(mesh) * worldMatrix;
                        Vector3.TransformCoordinate(ref point, ref matrixTM, out Vector3 pointWT);

                        var dist = (pointWT - ray.Position).Length();
                        if (dist < distance)
                        {
                            distance = dist;
                            data = new RayPickData((int)subMesh.Id, pointWT, Guid.NewGuid());
                        }
                    }
                }
            }

            return data != null;
        }
    }
}
