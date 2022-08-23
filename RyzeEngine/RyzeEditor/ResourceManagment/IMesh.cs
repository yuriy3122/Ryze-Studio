using System.Collections.Generic;
using SharpDX;

namespace RyzeEditor.ResourceManagment
{
	public interface IMesh
	{
		string Id { get; set; }

		BoundingBox BoundingBox { get; set; }

        BoundingSphere BoundingSphere { get; set; }

        List<SubMesh> SubMeshes { get; }

        SubMesh GetSubMesh(uint subMeshId);

        void SetDiffuseColor(Color3 color);

        BoundingBox GetBoundBox(List<uint> subMeshes);

        Quaternion GetRotationRH(uint submeshId);
    }
}