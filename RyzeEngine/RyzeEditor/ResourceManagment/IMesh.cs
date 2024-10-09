using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.ResourceManagment
{
	public interface IMesh
	{
		string Id { get; set; }

        string Name { get; set; }

		BoundingBox BoundingBox { get; set; }

        BoundingSphere BoundingSphere { get; set; }

        List<SubMesh> SubMeshes { get; }

        long SubMeshMask { get; set; }

        SubMesh GetSubMesh(uint subMeshId);

        void SetDiffuseColor(Color3 color);

        BoundingBox GetBoundBox(List<uint> subMeshes);

        Quaternion GetRotationRH(uint submeshId);
    }
}