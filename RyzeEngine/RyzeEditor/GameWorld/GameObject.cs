using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SharpDX;
using PropertyChanged;
using RyzeEditor.Renderer;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Helpers;

namespace RyzeEditor.GameWorld
{
	[Serializable]
	public enum Axis
	{
		None,
		X,
		Y,
		Z
	}

	public class RayPickData
	{
		public RayPickData(int subMeshIndex, Vector3 point, Guid gameObjectId)
		{
            SubMeshIndex = subMeshIndex;
			Point = point;
            GameObjectId = gameObjectId;
		}

		public int SubMeshIndex { get; private set; }

        /// <summary>
        /// Intersection point in World Space
        /// </summary>
		public Vector3 Point { get; private set; }

        public Guid GameObjectId { get; set; }
    }

	[Serializable]
	[ImplementPropertyChanged]
	public class GameObject : EntityBase, IVisualElement
	{
		public GameObject(List<string> geometryMeshIds, List<string> collisionMeshIds)
		{
			Id = Guid.NewGuid();
            _geometryMeshIds = geometryMeshIds;
        }

        [InspectorVisible(false)]
        public RenderOptions Options { get; set; }

        private readonly List<string> _geometryMeshIds = new List<string>();

        [field: NonSerialized]
        private List<IMesh> _geometryMeshes;

        [InspectorVisible(false)]
        public ConcurrentDictionary<uint, SubMeshTransform> SubMeshTransforms { get; set; }

        [RelativeChangeable(true)]
        public Vector3 Position { get; set; }

        [RelativeChangeable(true)]
        public Vector3 Scale { get; set; }

        public Quaternion Rotation { get; set; }

        [InspectorVisible(false)]
        public object UserData { get; set; }

        public bool Static { get; set; }

        [InspectorVisible(true)]
        public bool RenderNormals { get; set; }

        [InspectorVisible(false)]
        public bool RenderTangents { get; set; }

        [InspectorVisible(false)]
        public bool RenderBitangents { get; set; }

        [InspectorVisible(false)]
        public long SubMeshMask { get; set; } = -1;

        /// <summary>
        /// Include in Ray Traced acceleration stucture
        /// </summary>
        public bool AccelerationStructure { get; set; }

        public List<IMesh> GeometryMeshes
        {
            get
            {
                if (_geometryMeshes != null)
                {
                    return _geometryMeshes;
                }

                if (_geometryMeshIds == null)
                {
                    return null;
                }

                _geometryMeshes = new List<IMesh>();

                foreach (var geometryMeshId in _geometryMeshIds)
                {
                    _geometryMeshes.Add(ResourceManager.Instance.GetMesh(geometryMeshId));
                }

                return _geometryMeshes;
            }
        }

        public override string ToString()
        {
            var name = $"{GetType().Name}";

            if (_geometryMeshes != null)
            {
                name = $"{GetType().Name}, Mesh={_geometryMeshes.FirstOrDefault()?.Name}";
            }

            return name;
        }

        [DoNotNotify]
		public Matrix WorldMatrix => Matrix.Scaling(Scale.X, Scale.Y, Scale.Z) * 
		                             Matrix.RotationQuaternion(Rotation) *
		                             Matrix.Translation(Position.X, Position.Y, Position.Z);

	    [DoNotNotify]
		public BoundingBox BoundingBox => (GeometryMeshes != null && GeometryMeshes.Count > 0) ? GeometryMeshes[0].BoundingBox : new BoundingBox();

        public void Render3d(IRenderer renderer, RenderMode mode)
		{
			if (GeometryMeshes != null && GeometryMeshes.Count > 0)
			{
				renderer.DrawMeshInstanced(GeometryMeshes[0], new[] { this }, mode);
			}
		}

		public void Rotate(Axis rotationAxis, Vector3 dir, float angle)
		{
			switch (rotationAxis)
			{
				case Axis.X:
                    float rotX = dir.X > 0 ? angle : -angle;
                    Rotation = Quaternion.RotationYawPitchRoll(0.0f, rotX, 0.0f) * Rotation;
                    break;
				case Axis.Y:
                    float rotY = dir.Y > 0 ? angle : -angle;
                    Rotation = Quaternion.RotationYawPitchRoll(rotY, 0.0f, 0.0f) * Rotation;
                    break;
				case Axis.Z:
                    float rotZ = dir.Z > 0 ? angle : -angle;
                    Rotation = Quaternion.RotationYawPitchRoll(0.0f, 0.0f, rotZ) * Rotation;
                    break;
			}
		}

		public void Translate(Axis axis, Vector3 vec, Vector3? lastIntersectPoint)
		{
			if (lastIntersectPoint == null) return;

			switch (axis)
			{
				case Axis.X:
				{
					float delta = vec.X - lastIntersectPoint.Value.X;
					Position = new Vector3(Position.X + delta, Position.Y, Position.Z);
				}
				break;
				case Axis.Y:
				{
					float delta = vec.Y - lastIntersectPoint.Value.Y;
					Position = new Vector3(Position.X, Position.Y + delta, Position.Z);
				}
				break;
				case Axis.Z:
				{
					float delta = vec.Z - lastIntersectPoint.Value.Z;
					Position = new Vector3(Position.X, Position.Y, Position.Z + delta);
				}
				break;
			}
		}

		public bool Intersects(Ray ray, out RayPickData data)
		{
			data = null;

            if (GeometryMeshes == null || GeometryMeshes.Count == 0)
            {
                return false;
            }

            CollisionTests.MeshRayIntersects(GeometryMeshes[0], ray, WorldMatrix, out data);

            if (data != null)
            {
                data.GameObjectId = Id;
            }

            return data != null;
		}
	}
}