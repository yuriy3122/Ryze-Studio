using System;
using System.Drawing;
using PropertyChanged;
using SharpDX;

namespace RyzeEditor.GameWorld
{
	[Serializable]
    [ImplementPropertyChanged]
    public class Camera : EntityBase
	{
		/// <summary>
		/// Position of the camera
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Looking at direction of the camera
		/// </summary>
		public Vector3 LookAtDir { get; set; }

        /// <summary>
        /// Up direction
        /// </summary>
        [InspectorVisible(false)]
        public Vector3 UpDir { get; set; }

		/// <summary>
		/// Field of view
		/// </summary>
		public float FOV { get; set; }

		/// <summary>
		/// Z near distance
		/// </summary>
		public float ZNear { get; set; }

		/// <summary>
		/// Z far distance
		/// </summary>
		public float ZFar { get; set; }

		/// <summary>
		/// Aspect ratio
		/// </summary>
		public float AspectRatio { get; set; }

        /// <summary>
        /// Active camera
        /// </summary>
        [InspectorVisible(false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// Render window
        /// </summary>
        [InspectorVisible(false)]
        public Size ClientWndSize { get; set; }

        [InspectorVisible(false)]
        public bool IsOrthoProj { get; set; }

		public Ray GetPickRay(int x, int y)
		{
			var viewPort = new ViewportF(0, 0, ClientWndSize.Width, ClientWndSize.Height, 0.0f, 1.0f);
			var view = Matrix.LookAtLH(Position, LookAtDir, UpDir);
			var proj = Matrix.PerspectiveFovLH(FOV, AspectRatio, ZNear, ZFar);
            var matrix = view * proj;

            return Ray.GetPickRay(x, y, viewPort, matrix);
		}

		public void RotateY(float delta)
		{
            var matrix = Matrix.RotationY(delta);
			var rot = new Vector3(Position.X, 0.0f, Position.Z) - new Vector3(LookAtDir.X, 0.0f, LookAtDir.Z);

            Vector3.TransformCoordinate(ref rot, ref matrix, out Vector3 position);

            Position = position + new Vector3(LookAtDir.X, Position.Y, LookAtDir.Z);
        }

		public void RotateX(float delta)
		{
            var dir = Position - LookAtDir;
            dir.Normalize();

            var axis = Vector3.Cross(dir, UpDir);
            axis.Normalize();

            var rotation = Quaternion.RotationAxis(axis, delta);

            var matrix = Matrix.RotationQuaternion(rotation);
            var rot = new Vector3(Position.X, Position.Y, Position.Z) - new Vector3(LookAtDir.X, LookAtDir.Y, LookAtDir.Z);

            Vector3.TransformCoordinate(ref rot, ref matrix, out Vector3 position);

            Position = position + new Vector3(LookAtDir.X, LookAtDir.Y, LookAtDir.Z);
        }

		public void Strafe(float delta)
		{
			var n = Vector3.Cross(UpDir, LookAtDir - Position);
			n.Normalize();

			Position -= delta * n;
			LookAtDir -= delta * n;
		}

		public void Walk(float delta)
		{
			var vec = LookAtDir - Position;
			var n = new Vector3(vec.X, 0.0f, vec.Z);
			n.Normalize();

			Position += delta * n;
			LookAtDir += delta * n;	
		}

        public void Zoom(float delta)
        {
            var signDelta = delta > 0.0f ? 1.0f : -1.0f;
            var absDelta = Math.Abs(delta);
            var deltaValue = signDelta * Math.Max(absDelta, 0.5f);

            var eyeDir = (Position - LookAtDir);
            var distance = eyeDir.Length();
            eyeDir.Normalize();

            Position = LookAtDir + eyeDir * (distance + deltaValue);
        }
	}
}