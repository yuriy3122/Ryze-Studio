using System;
using SharpDX;

namespace RyzeEditor.ResourceManagment
{
	[Serializable]
	public class Material : IMaterial
	{
		public Vector3 Ambient;

		public Vector3 Diffuse;

		public Vector3 Specular;

		public float Shininess;

		public float Transparency;

		public ITexture DiffuseTexture;

        public ITexture NormalTexture;

        public ITexture SpecularTexture;

        private Vector3 _eplison = new Vector3(0.0001f, 0.0001f, 0.0001f);

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float shiness, float transparency)
		{
			Ambient = ambient;
			Diffuse = diffuse;
			Specular = specular;
			Shininess = shiness;
			Transparency = transparency;
        }
    }
}
