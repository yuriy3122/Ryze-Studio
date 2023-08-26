using System;
using PropertyChanged;
using SharpDX;

namespace RyzeEditor.GameWorld
{
    [Serializable]
    [ImplementPropertyChanged]
    public class SunLight : EntityBase
    {
        public Vector3 LightDir { get; set; }

        public float Intensity { get; set; }

        public SunLight()
        {
            Id = Guid.NewGuid();
            LightDir = new Vector3(1.0f, 1.0f, 1.0f);
            Intensity = 1.0f;
        }
    }
}
