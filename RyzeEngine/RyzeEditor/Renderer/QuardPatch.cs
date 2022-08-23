using SharpDX;

namespace RyzeEditor.Renderer
{
	public class QuardPatch
	{
		private readonly Vector3 _p1;

		private readonly Vector3 _p2;

		private readonly Vector3 _p3;

		private readonly Vector3 _p4;

		public QuardPatch(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
		{
			_p1 = p1;
			_p2 = p2;
			_p3 = p3;
			_p4 = p4;
		}

		public Vector3[] Points
		{
			get { return new [] {_p1, _p2, _p3, _p4};}
		}
	}
}