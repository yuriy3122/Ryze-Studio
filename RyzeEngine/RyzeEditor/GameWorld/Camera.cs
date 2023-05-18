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
		public bool IsActive { get; set; }

        /// <summary>
        /// Render window
        /// </summary>
        [InspectorVisible(false)]
        public Size ClientWndSize { get; set; }

		public bool IsOrthoProj { get; set; }

		public Ray GetPickRay(int x, int y)
		{
			var viewPort = new ViewportF(0, 0, ClientWndSize.Width, ClientWndSize.Height, 0.0f, 1.0f);
			var view = Matrix.LookAtLH(Position, LookAtDir, UpDir);
			var proj = Matrix.PerspectiveFovLH(FOV, AspectRatio, ZNear, ZFar);

			return Ray.GetPickRay(x, y, viewPort, view * proj);
		}

		public void RotateY(float delta)
		{
			var matrix = Matrix.RotationY(delta);
			var posOrigin = Position;

            Vector3.TransformCoordinate(ref posOrigin, ref matrix, out Vector3 position);

            Position = position;
		}

		public void RotateX(float delta)
		{
			var matrix = Matrix.RotationX(delta);
			var posOrigin = Position;

            Vector3.TransformCoordinate(ref posOrigin, ref matrix, out Vector3 position);

            Position = position;
		}

		public void Strafe(float delta)
		{
			var n = Vector3.Cross(UpDir, LookAtDir - Position);
			n.Normalize();

			Position = Position - delta * n;
			LookAtDir = LookAtDir - delta * n;
		}

		public void Walk(float delta)
		{
			var vec = LookAtDir - Position;
			var n = new Vector3(vec.X, 0.0f, vec.Z);
			n.Normalize();

			Position = Position + delta * n;
			LookAtDir = LookAtDir + delta * n;			
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