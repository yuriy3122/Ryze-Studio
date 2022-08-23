using System;
using System.Globalization;
using System.Runtime.Serialization;
using SharpDX;

namespace RyzeEditor.Serialization
{
    public class QuaternionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var vector = (Quaternion)obj;
            info.AddValue("X", vector.X);
            info.AddValue("Y", vector.Y);
            info.AddValue("Z", vector.Z);
            info.AddValue("W", vector.W);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var quaternion = (Quaternion)obj;
            quaternion.X = Convert.ToSingle(info.GetString("X"), CultureInfo.InvariantCulture);
            quaternion.Y = Convert.ToSingle(info.GetString("Y"), CultureInfo.InvariantCulture);
            quaternion.Z = Convert.ToSingle(info.GetString("Z"), CultureInfo.InvariantCulture);
            quaternion.W = Convert.ToSingle(info.GetString("W"), CultureInfo.InvariantCulture);

            return quaternion;
        }
    }
}