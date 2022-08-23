using System;
using System.Collections.Generic;

namespace RyzeEditor.ResourceManagment
{
	public interface IResourceManager
	{
		ITexture GetTexture(string textureId);

		IMesh GetMesh(string meshId);

		IEnumerable<string> GetMeshIdList();

        IEnumerable<string> GetTextureIdList();

        bool IsReclaimed(string resourceId);
    }
}
