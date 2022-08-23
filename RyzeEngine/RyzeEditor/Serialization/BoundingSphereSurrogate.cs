using System;
using System.Globalization;
using System.Runtime.Serialization;
using SharpDX;

namespace RyzeEditor.Serialization
{
    public class BoundingSphereSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var boundingSphere = (BoundingSphere)obj;

            info.AddValue("X", boundingSphere.Center.X);
            info.AddValue("Y", boundingSphere.Center.Y);
            info.AddValue("Z", boundingSphere.Center.Z);
            info.AddValue("R", boundingSphere.Radius);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var boundingSphere = (BoundingSphere)obj;

            var center = new Vector3
            {
                X = Convert.ToSingle(info.GetString("X"), CultureInfo.InvariantCulture),
                Y = Convert.ToSingle(info.GetString("Y"), CultureInfo.InvariantCulture),
                Z = Convert.ToSingle(info.GetString("Z"), CultureInfo.InvariantCulture)
            };

            var radius = Convert.ToSingle(info.GetString("R"), CultureInfo.InvariantCulture);

            boundingSphere.Center = center;
            boundingSphere.Radius = radius;

            return boundingSphere;
        }
    }
}

