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

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float shiness, float transparency)
		{
			Ambient = ambient;
			Diffuse = diffuse;
			Specular = specular;
			Shininess = shiness;
			Transparency = transparency;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var material = (Material)obj;

            var epsilon = new Vector3(1e-6f, 1e-6f, 1e-6f);

            if (!Vector3.NearEqual(Ambient, material.Ambient, epsilon))
            {
                return false;
            }
            if (!Vector3.NearEqual(Diffuse, material.Diffuse, epsilon))
            {
                return false;
            }
            if (!Vector3.NearEqual(Specular, material.Specular, epsilon))
            {
                return false;
            }
            if (Shininess != material.Shininess)
            {
                return false;
            }
            if (Transparency != material.Transparency)
            {
                return false;
            }
            if (DiffuseTexture?.Id != material.DiffuseTexture?.Id)
            {
                return false;
            }
            if (NormalTexture?.Id != material.NormalTexture?.Id)
            {
                return false;
            }
            if (SpecularTexture?.Id != material.SpecularTexture?.Id)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + Ambient.GetHashCode();
            hash = hash * 23 + Diffuse.GetHashCode();
            hash = hash * 23 + Specular.GetHashCode();
            hash = hash * 23 + Shininess.GetHashCode();
            hash = hash * 23 + Transparency.GetHashCode();
            hash = hash * 23 + Transparency.GetHashCode();

            return hash;
        }
    }
}
