using System;
using System.Linq;
using System.Collections.Generic;
using SharpDX;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Renderer;
using RyzeEditor.Helpers;
using PropertyChanged;

namespace RyzeEditor.GameWorld
{
    [Serializable]
    [ImplementPropertyChanged]
    public class RigidBody : EntityBase, IVisualElement
    {
        public RigidBody()
        {
            Id = Guid.NewGuid();
        }

        private string _meshId;

        [InspectorVisible(false)]
        public new bool IsHidden { get; set; }

        [InspectorVisible(false)]
        public GameObject GameObject { get; set; }

        [InspectorVisible(false)]
        public List<uint> SubMeshIds { get; set; }

        [InspectorVisible(false)]
        public IMesh Mesh
        {
            get
            {
                return _meshId != null ? ResourceManager.Instance.GetMesh(_meshId) : null;
            }
            set
            {
                _meshId = value?.Id;
            }
        }

        [InspectorVisible(false)]
        public uint? SubMeshId
        {
            get
            {
                return SubMeshIds?.FirstOrDefault();
            }
            set
            {
                if (SubMeshIds != null)
                {
                    SubMeshIds.Clear();
                }

                uint? id = value;

                if (id.HasValue)
                {
                    if (SubMeshIds == null)
                    {
                        SubMeshIds = new List<uint>();
                    }

                    SubMeshIds.Add(id.Value);
                }
            }
        }

        public CollisionShapeType ShapeType { get; set; }

        public float Mass { get; set; }

        //For Heightfield shape
        public float GridSpacing { get; set; }

        public Vector3 Position => GameObject.Position;

        public float[] GetMeshVertices()
        {
            var result = new List<float>();

            foreach (var subMesh in Mesh.SubMeshes)
            {
                if (SubMeshIds != null && !SubMeshIds.Contains(subMesh.Id))
                {
                    continue;
                }

                result.AddRange(GetVertexData(Mesh, subMesh));
            }

            return result.ToArray();
        }

        public BoundingBox BoundingBox
        {
            get
            {
                var boundingBox = new BoundingBox();

                if (Mesh == null)
                {
                    return boundingBox;
                }

                var vertices = new List<Vector3>();

                foreach (var subMesh in Mesh.SubMeshes)
                {
                    if (SubMeshIds != null && !SubMeshIds.Contains(subMesh.Id))
                    {
                        continue;
                    }

                    foreach (var vertex in subMesh.VertexData)
                    {
                        Vector3 vertPosition = vertex.Pos;
                        var matrix = subMesh.GetMatrix(Mesh);

                        Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);

                        vertices.Add(position);
                    }
                }

                return BoundingBox.FromPoints(vertices.ToArray());
            }
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                var boundingSphere = new BoundingSphere();

                if (Mesh == null)
                {
                    return boundingSphere;
                }

                var vertices = new List<Vector3>();

                foreach (var subMesh in Mesh.SubMeshes)
                {
                    if (SubMeshIds != null && !SubMeshIds.Contains(subMesh.Id))
                    {
                        continue;
                    }

                    foreach (var vertex in subMesh.VertexData)
                    {
                        Vector3 vertPosition = vertex.Pos;
                        var matrix = subMesh.GetMatrix(Mesh);
                        Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);

                        vertices.Add(position);
                    }
                }

                return BoundingSphere.FromPoints(vertices.ToArray());
            }
        }

        [InspectorVisible(false)]
        public RenderOptions Options { get; set; }

        public void Render3d(IRenderer renderer, RenderMode mode)
        {
        }

        public bool Intersects(Ray ray, out RayPickData data)
        {
            data = null;

            if (Mesh == null)
            {
                return false;
            }

            CollisionTests.MeshRayIntersects(Mesh, ray, GameObject.WorldMatrix, out data);

            if (data != null)
            {
                data.GameObjectId = Id;
            }

            return data != null;
        }

        private List<float> GetVertexData(IMesh Mesh, SubMesh subMesh)
        {
            var vertices = new List<float>();

            foreach (var vertex in subMesh.VertexData)
            {
                var matrix = subMesh.GetMatrix(Mesh) * Matrix.Scaling(GameObject.Scale);

                Vector3 vertPosition = vertex.Pos;
                Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);
                vertices.Add(position.X);
                vertices.Add(position.Y);
                vertices.Add(position.Z * -1.0f);
            }

            return vertices;
        }
    }

    public enum RigidBodyType
    {
        Static = 0,
        Dynamic = 1,
        Kinematic = 3
    }

    public enum CollisionShapeType
    {
        Box = 0,
        Sphere = 1,
        ConvexHull = 2,
        Heightfield = 3
    }
}