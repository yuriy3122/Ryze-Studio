using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.Renderer
{
	public class PlaneGrid
	{
		public List<QuardPatch> Patches { private set; get; }

		public PlaneGrid(int patchNum, float patchSize)
		{
			Patches = new List<QuardPatch>();

			for (int i = -1 * patchNum; i < patchNum; i++)
			{
				for (int j = -1 * patchNum; j < patchNum; j++)
				{
					var p1 = new Vector3((j + 0) * patchSize, 0.0f, (i + 0) * patchSize);
					var p2 = new Vector3((j + 0) * patchSize, 0.0f, (i + 1) * patchSize);
					var p3 = new Vector3((j + 1) * patchSize, 0.0f, (i + 1) * patchSize);
					var p4 = new Vector3((j + 1) * patchSize, 0.0f, (i + 0) * patchSize);

					Patches.Add(new QuardPatch(p1, p2, p3, p4));
				}
			}
		}
	}
}
