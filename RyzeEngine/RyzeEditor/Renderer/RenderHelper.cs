using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.Renderer
{
	public static class RenderHelper
	{
		public static void RenderBox(IRenderer renderer, Vector3 min, Vector3 max, Vector4 color, Matrix matrix)
		{
			RenderTopPlane(renderer, min, max, color, matrix);

			RenderButtonPlane(renderer, min, max, color, matrix);

			RenderLeftPlane(renderer, min, max, color, matrix);

			RenderRightPlane(renderer, min, max, color, matrix);		
		}

		public static void RenderGridPatch(IRenderer renderer, QuardPatch patch)
		{
			var color = new Vector4(0.5f, 0.5f, 0.5f, 0.0f);

			var points = new List<Point3>
			{
				new Point3(new Vector4(patch.Points[0], 1.0f), color),
				new Point3(new Vector4(patch.Points[1], 1.0f), color),
				new Point3(new Vector4(patch.Points[2], 1.0f), color),
				new Point3(new Vector4(patch.Points[3], 1.0f), color)
			};

			renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}

		private static void RenderTopPlane(IRenderer renderer, Vector3 min, Vector3 max, Vector4 color, Matrix matrix)
		{
			var points = new List<Point3>
			{
				new Point3(new Vector4(min.X, max.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(min.X, max.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(max.X, max.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(max.X, max.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(min.X, max.Y, max.Z, 1.0f), color)
			};

			foreach (var point in points)
			{
				Vector4.Transform(ref point.Position, ref matrix, out point.Position);
			}

			renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}

		private static void RenderButtonPlane(IRenderer renderer, Vector3 min, Vector3 max, Vector4 color, Matrix matrix)
		{
			var points = new List<Point3>
			{
				new Point3(new Vector4(min.X, min.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(max.X, min.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(max.X, min.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(min.X, min.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(min.X, min.Y, max.Z, 1.0f), color)
			};

			foreach (var point in points)
			{
				Vector4.Transform(ref point.Position, ref matrix, out point.Position);
			}

			renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}

		private static void RenderLeftPlane(IRenderer renderer, Vector3 min, Vector3 max, Vector4 color, Matrix matrix)
		{
			var points = new List<Point3>
			{
				new Point3(new Vector4(min.X, min.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(min.X, max.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(min.X, max.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(min.X, min.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(min.X, min.Y, min.Z, 1.0f), color)
			};

			foreach (var point in points)
			{
				Vector4.Transform(ref point.Position, ref matrix, out point.Position);
			}

			renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}

		private static void RenderRightPlane(IRenderer renderer, Vector3 min, Vector3 max, Vector4 color, Matrix matrix)
		{
			var points = new List<Point3>
			{
				new Point3(new Vector4(max.X, min.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(max.X, max.Y, min.Z, 1.0f), color),
				new Point3(new Vector4(max.X, max.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(max.X, min.Y, max.Z, 1.0f), color),
				new Point3(new Vector4(max.X, min.Y, min.Z, 1.0f), color)
			};

			foreach (var point in points)
			{
				Vector4.Transform(ref point.Position, ref matrix, out point.Position);
			}

			renderer.DrawLineStrip(points, new RenderMode { BoundBox = false, IsDepthClipEnabled = false });
		}
	}
}
