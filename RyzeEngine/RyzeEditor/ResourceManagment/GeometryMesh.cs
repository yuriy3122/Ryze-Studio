﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX;

namespace RyzeEditor.ResourceManagment
{
    [Serializable]
    public struct Vertex
    {
        public Vector3 Pos;
        public Vector3 Norm;
        public Vector3 Tangent;
        public Vector3 Bitangent;
        public Vector2 Tex;

        public Vertex(Vector3 pos, Vector3 norm, Vector3 tangent, Vector3 bitangent, Vector2 tex)
        {
            Pos = pos;
            Norm = norm;
            Tangent = tangent;
            Bitangent = bitangent;
            Tex = tex;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var vertex = (Vertex)obj;
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

    [Serializable]
    public class SubMesh
    {
        public uint Id { get; set; }

        public uint ParentId { get; set; }

        public List<Vertex> VertexData { get; set; }

        public List<Material> Materials { get; set; }

        public Dictionary<int, List<uint>> Indices { get; set; }

        public Vector3 Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public Quaternion RotationRH { get; set; }

        public Vector3 Position { get; set; }

        public int TessellationFactor { get; set; }

        public BoundingBox BoundingBox
        {
            get
            {
                var vertices = new List<Vector3>();

                foreach (var vertex in VertexData)
                {
                    Vector3 vertPosition = vertex.Pos;
                    var matrix = Matrix.Scaling(Scale.X, Scale.Y, Scale.Z) *
                                 Matrix.RotationQuaternion(Rotation) *
                                 Matrix.Translation(Position.X, Position.Y, Position.Z);

                    Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);

                    vertices.Add(position);
                }

                return BoundingBox.FromPoints(vertices.ToArray());
            }
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                var vertices = new List<Vector3>();

                foreach (var vertex in VertexData)
                {
                    Vector3 vertPosition = vertex.Pos;
                    var matrix = Matrix.Scaling(Scale.X, Scale.Y, Scale.Z) *
                                 Matrix.RotationQuaternion(Rotation) *
                                 Matrix.Translation(Position.X, Position.Y, Position.Z);

                    Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);

                    vertices.Add(position);
                }

                return BoundingSphere.FromPoints(vertices.ToArray());
            }
        }

        public SubMesh(uint nodeId, uint parentNodeId)
        {
            Id = nodeId;
            ParentId = parentNodeId == 0 ? uint.MaxValue : parentNodeId;
            Scale = Vector3.One;
        }

        public bool IsEqualGeometry(SubMesh subMesh)
        {
            if (VertexData.Count != subMesh.VertexData.Count)
            {
                return false;
            }

            if (Indices.Keys.Count != subMesh.Indices.Count)
            {
                return false;
            }

            var keys = Indices.Keys.ToList();
            var subMeshKeys = subMesh.Indices.Keys.ToList();

            for (int i = 0; i < Indices.Keys.Count; i++)
            {
                var indices = Indices[keys[i]];
                var subIndices = subMesh.Indices[subMeshKeys[i]];

                for (int j = 0; j < indices.Count; j++)
                {
                    var v1 = VertexData[(int)indices[j]];
                    var v2 = subMesh.VertexData[(int)subIndices[j]];

                    if (!v1.Equals(v2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Matrix GetNormalMatrix(IMesh mesh)
        {
            var matrix = Matrix.RotationQuaternion(Rotation);

            uint parentId = ParentId;

            do
            {
                var subMesh = mesh.SubMeshes.Where(x => x.Id == parentId).SingleOrDefault();

                if (subMesh != null)
                {
                    var rotation = subMesh.Rotation;

                    matrix = matrix * Matrix.RotationQuaternion(rotation);
                    parentId = subMesh.ParentId;
                }
            }
            while (parentId < uint.MaxValue);

            return matrix;
        }

        public Matrix GetScaleMatrix(IMesh mesh)
        {
            var matrix = Matrix.Scaling(Scale.X, Scale.Y, Scale.Z);

            uint parentId = ParentId;

            do
            {
                var subMesh = mesh.SubMeshes.Where(x => x.Id == parentId).SingleOrDefault();

                if (subMesh != null)
                {
                    var scale = Matrix.Scaling(subMesh.Scale.X, subMesh.Scale.Y, subMesh.Scale.Z);

                    matrix *= scale;
                    parentId = subMesh.ParentId;
                }
            }
            while (parentId < uint.MaxValue);

            return matrix;
        }

        public Matrix GetMatrix(IMesh mesh)
        {
            var matrix = Matrix.Scaling(Scale.X, Scale.Y, Scale.Z) *
                Matrix.RotationQuaternion(Rotation) *
                Matrix.Translation(Position.X, Position.Y, Position.Z);

            uint parentId = ParentId;

            do
            {
                var subMesh = mesh.SubMeshes.Where(x => x.Id == parentId).SingleOrDefault();

                if (subMesh != null)
                {
                    var scale = Matrix.Scaling(subMesh.Scale.X, subMesh.Scale.Y, subMesh.Scale.Z);
                    var rotation = Matrix.RotationQuaternion(subMesh.Rotation);
                    var transform = Matrix.Translation(subMesh.Position.X, subMesh.Position.Y, subMesh.Position.Z);

                    matrix = matrix * scale * rotation * transform;
                    parentId = subMesh.ParentId;
                }
            }
            while (parentId < uint.MaxValue);

            return matrix;
        }
    }

    [Serializable]
    public class GeometryMesh : IMesh
    {
        private const short IdMaterialHeader = 0x1FF1;

        public string Id { get; set; }
        public string Name { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public BoundingSphere BoundingSphere { get; set; }

        public List<SubMesh> SubMeshes { get; } = new List<SubMesh>();

        public SubMesh GetSubMesh(uint subMeshId)
        {
            return SubMeshes.Where(x => x.Id == subMeshId).FirstOrDefault();
        }

        public static GeometryMesh FromStream(Stream stream)
        {
            var mesh = new GeometryMesh();
            mesh.LoadFromStream(stream);

            return mesh;
        }

        public static GeometryMesh FromFile(string filePath)
        {
            if (!(new FileInfo(filePath)).Exists)
            {
                throw new FileNotFoundException();
            }

            var mesh = new GeometryMesh();
            mesh.LoadFromFile(filePath);

            return mesh;
        }

        private void LoadFromFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                LoadFromStream(stream);
            }
        }

        public void SetDiffuseColor(Color3 color)
        {
            foreach (var material in SubMeshes.SelectMany(subMesh => subMesh.Materials))
            {
                material.Diffuse = new Vector3(color.Red, color.Green, color.Blue);
            }
        }

        private void LoadFromStream(Stream stream)
        {
            byte[] buffer = new byte[66];

            while (stream.Read(buffer, 0, 2) > 0)
            {
                stream.Read(buffer, 0, buffer.Length);

                uint id = BitConverter.ToUInt32(buffer, 0);
                uint parentId = BitConverter.ToUInt32(buffer, 4);

                if (parentId == 0)
                {
                    parentId = uint.MaxValue;
                }

                int vertNum = BitConverter.ToInt32(buffer, 8);
                int faceNum = BitConverter.ToInt32(buffer, 12);

                Vector3 scale;
                scale.X = BitConverter.ToSingle(buffer, 16);
                scale.Y = BitConverter.ToSingle(buffer, 20);
                scale.Z = BitConverter.ToSingle(buffer, 24);

                Vector3 rotation;
                rotation.X = BitConverter.ToSingle(buffer, 28);
                rotation.Y = BitConverter.ToSingle(buffer, 32);
                rotation.Z = BitConverter.ToSingle(buffer, 36);

                Vector3 position;
                position.X = BitConverter.ToSingle(buffer, 40);
                position.Y = BitConverter.ToSingle(buffer, 44);
                position.Z = BitConverter.ToSingle(buffer, 48);

                int tessellationFactor = BitConverter.ToInt32(buffer, 52);
                int nextNode = BitConverter.ToInt32(buffer, 56);

                if (vertNum > 0)
                {
                    short hdr = BitConverter.ToInt16(buffer, 60);
                    int vertPtr = BitConverter.ToInt32(buffer, 62);

                    List<Material> mtls = new List<Material>();

                    if (hdr == IdMaterialHeader)
                    {
                        mtls = ReadMaterialData(stream);
                    }

                    stream.Position = vertPtr + 2;
                    stream.Read(buffer, 0, 4); //facePtr

                    var vertices = ReadVertexData(stream, vertNum);

                    // Skip Face data block header
                    stream.Read(buffer, 0, 2);

                    var mtrlIndices = new Dictionary<int, List<uint>>();

                    for (int i = 0; i < faceNum * 3; i++)
                    {
                        stream.Read(buffer, 0, 4);
                        uint index = BitConverter.ToUInt32(buffer, 0);

                        stream.Read(buffer, 0, 4);
                        int mtrlIndex = BitConverter.ToInt32(buffer, 0);

                        if (!mtrlIndices.ContainsKey(mtrlIndex))
                        {
                            mtrlIndices[mtrlIndex] = new List<uint>();
                        }

                        mtrlIndices[mtrlIndex].Add(index);
                    }

                    var subMesh = new SubMesh(id, parentId)
                    {
                        VertexData = vertices,
                        Scale = scale,
                        Rotation = Quaternion.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z),
                        RotationRH = Quaternion.RotationYawPitchRoll(-rotation.Y, -rotation.X, rotation.Z),
                        Position = position,
                        Materials = mtls,
                        Indices = mtrlIndices,
                        TessellationFactor = tessellationFactor
                    };

                    SubMeshes.Add(subMesh);
                }
                else
                {
                    var subMesh = new SubMesh(id, parentId)
                    {
                        VertexData = new List<Vertex>(),
                        Scale = scale,
                        Rotation = Quaternion.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z),
                        RotationRH = Quaternion.RotationYawPitchRoll(-rotation.Y, -rotation.X, rotation.Z),
                        Position = position,
                        Materials = new List<Material>(),
                        Indices = new Dictionary<int, List<uint>>(),
                        TessellationFactor = tessellationFactor
                    };

                    SubMeshes.Add(subMesh);
                }

                stream.Position = nextNode;
            }

            CompactSubMeshIds();
            ComputeBoundBox();
            ComputeBoundingSphere();
        }

        public void ComputeBoundingSphere()
        {
            var vertices = new List<Vector3>();

            foreach (var subMesh in SubMeshes)
            {
                foreach (var vertex in subMesh.VertexData)
                {
                    Vector3 vertPosition = vertex.Pos;
                    Matrix matrix = subMesh.GetMatrix(this);
                    Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);
                    vertices.Add(position);
                }
            }

            BoundingSphere = BoundingSphere.FromPoints(vertices.ToArray());
        }

        private void CompactSubMeshIds()
        {
            List<uint> ids = new List<uint>();

            foreach (var subMesh in SubMeshes)
            {
                ids.Add(subMesh.Id);
            }

            ids.Sort();

            for (int i = 0; i < ids.Count; i++)
            {
                foreach (var subMesh in SubMeshes)
                {
                    if (subMesh.Id == ids[i])
                    {
                        subMesh.Id = (uint)i;
                    }
                    if (subMesh.ParentId == ids[i])
                    {
                        subMesh.ParentId = (uint)i;
                    }
                }
            }
        }

        public void ComputeBoundBox()
        {
            var vertices = new List<Vector3>();

            foreach (var subMesh in SubMeshes)
            {
                Matrix matrix = subMesh.GetMatrix(this);

                foreach (var vertex in subMesh.VertexData)
                {
                    Vector3 vertPosition = vertex.Pos;
                    Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);
                    vertices.Add(position);
                }
            }

            BoundingBox = BoundingBox.FromPoints(vertices.ToArray());
        }

        public Quaternion GetRotationRH(uint submeshId)
        {
            var subMesh = SubMeshes.Where(x => x.Id == submeshId).SingleOrDefault();
            var rotation = subMesh.RotationRH;

            uint parentId = subMesh.ParentId;

            do
            {
                subMesh = SubMeshes.Where(x => x.Id == parentId).SingleOrDefault();

                if (subMesh != null)
                {
                    rotation = rotation * subMesh.RotationRH;
                    parentId = subMesh.ParentId;
                }
            }
            while (parentId < uint.MaxValue);

            return rotation;
        }

        public BoundingBox GetBoundBox(List<uint> subMeshes)
        {
            var vertices = new List<Vector3>();

            foreach (var subMesh in SubMeshes)
            {
                if (subMeshes == null || !subMeshes.Contains(subMesh.Id))
                {
                    continue;
                }

                Matrix matrix = subMesh.GetMatrix(this);

                foreach (var vertex in subMesh.VertexData)
                {
                    Vector3 vertPosition = vertex.Pos;
                    Vector3.TransformCoordinate(ref vertPosition, ref matrix, out Vector3 position);
                    vertices.Add(position);
                }
            }

            return BoundingBox.FromPoints(vertices.ToArray());
        }

        private static List<Vertex> ReadVertexData(Stream fileStream, int vertNum)
        {
            byte[] buffer = new byte[56];
            var vertices = new List<Vertex>();

            for (int i = 0; i < vertNum; i++)
            {
                var vert = new Vertex();
                fileStream.Read(buffer, 0, 56);
                vert.Pos.X = BitConverter.ToSingle(buffer, 0);
                vert.Pos.Y = BitConverter.ToSingle(buffer, 4);
                vert.Pos.Z = BitConverter.ToSingle(buffer, 8);

                vert.Norm.X = BitConverter.ToSingle(buffer, 12);
                vert.Norm.Y = BitConverter.ToSingle(buffer, 16);
                vert.Norm.Z = BitConverter.ToSingle(buffer, 20);

                vert.Tangent.X = BitConverter.ToSingle(buffer, 24);
                vert.Tangent.Y = BitConverter.ToSingle(buffer, 28);
                vert.Tangent.Z = BitConverter.ToSingle(buffer, 32);

                vert.Bitangent.X = BitConverter.ToSingle(buffer, 36);
                vert.Bitangent.Y = BitConverter.ToSingle(buffer, 40);
                vert.Bitangent.Z = BitConverter.ToSingle(buffer, 44);

                vert.Tex.X = BitConverter.ToSingle(buffer, 48);
                vert.Tex.Y = BitConverter.ToSingle(buffer, 52);

                vertices.Add(vert);
            }

            return vertices;
        }

        private List<Material> ReadMaterialData(Stream fileStream)
        {
            var mtls = new List<Material>();

            byte[] buffer = new byte[48];

            fileStream.Read(buffer, 0, 4);
            int mtlsNum = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < mtlsNum; i++)
            {
                Vector3 ambient;
                fileStream.Read(buffer, 0, 48);
                ambient.X = BitConverter.ToSingle(buffer, 0);
                ambient.Y = BitConverter.ToSingle(buffer, 4);
                ambient.Z = BitConverter.ToSingle(buffer, 8);

                Vector3 diffuse;
                diffuse.X = BitConverter.ToSingle(buffer, 12);
                diffuse.Y = BitConverter.ToSingle(buffer, 16);
                diffuse.Z = BitConverter.ToSingle(buffer, 20);

                Vector3 specular;
                specular.X = BitConverter.ToSingle(buffer, 24);
                specular.Y = BitConverter.ToSingle(buffer, 28);
                specular.Z = BitConverter.ToSingle(buffer, 32);

                float shininess = BitConverter.ToSingle(buffer, 36);

                float transparency = BitConverter.ToSingle(buffer, 40);

                int fileNameLen = BitConverter.ToInt32(buffer, 44);
                string diffuseTextureName = string.Empty;
                string normalTextureName = string.Empty;
                string specularTextureName = string.Empty;

                if (fileNameLen > 0)
                {
                    byte[] tmp = new byte[fileNameLen * 2];
                    fileStream.Read(tmp, 0, tmp.Length);
                    diffuseTextureName = (new FileInfo(Encoding.Unicode.GetString(tmp))).Name;
                    normalTextureName = Path.GetFileNameWithoutExtension(diffuseTextureName) + "_normal" + Path.GetExtension(diffuseTextureName);
                    specularTextureName = Path.GetFileNameWithoutExtension(diffuseTextureName) + "_specular" + Path.GetExtension(diffuseTextureName);
                }

                var material = new Material(ambient, diffuse, specular, shininess, transparency)
                {
                    DiffuseTexture = ResourceManager.Instance.GetTexture(diffuseTextureName),
                    NormalTexture = ResourceManager.Instance.GetTexture(normalTextureName),
                    SpecularTexture = ResourceManager.Instance.GetTexture(specularTextureName),
                };

                mtls.Add(material);
            }

            return mtls;
        }
    }

    public class SubMeshTransform
    {
        public int SubMeshId { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Position { get; set; }
    }
}
