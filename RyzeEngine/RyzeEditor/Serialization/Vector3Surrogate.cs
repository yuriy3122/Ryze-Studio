using System;
using System.Globalization;
using System.Runtime.Serialization;
using SharpDX;

namespace RyzeEditor.Serialization
{
	public class Vector3Surrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var vector = (Vector3)obj;
			info.AddValue("X", vector.X);
			info.AddValue("Y", vector.Y);
			info.AddValue("Z", vector.Z);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var vector = (Vector3)obj;
			vector.X = Convert.ToSingle(info.GetString("X"), CultureInfo.InvariantCulture);
			vector.Y = Convert.ToSingle(info.GetString("Y"), CultureInfo.InvariantCulture);
			vector.Z = Convert.ToSingle(info.GetString("Z"), CultureInfo.InvariantCulture);

			return vector;
		}
	}
}
