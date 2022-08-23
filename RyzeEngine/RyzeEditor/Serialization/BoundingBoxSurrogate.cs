using System;
using System.Globalization;
using System.Runtime.Serialization;
using SharpDX;

namespace RyzeEditor.Serialization
{
	public class BoundingBoxSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var boundingBox = (BoundingBox)obj;

			info.AddValue("X1", boundingBox.Minimum.X);
			info.AddValue("Y1", boundingBox.Minimum.Y);
			info.AddValue("Z1", boundingBox.Minimum.Z);

			info.AddValue("X2", boundingBox.Maximum.X);
			info.AddValue("Y2", boundingBox.Maximum.Y);
			info.AddValue("Z2", boundingBox.Maximum.Z);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			var boundingBox = (BoundingBox)obj;

			var min = new Vector3
			{
				X = Convert.ToSingle(info.GetString("X1"), CultureInfo.InvariantCulture),
				Y = Convert.ToSingle(info.GetString("Y1"), CultureInfo.InvariantCulture),
				Z = Convert.ToSingle(info.GetString("Z1"), CultureInfo.InvariantCulture)
			};

			var max = new Vector3
			{
				X = Convert.ToSingle(info.GetString("X2"), CultureInfo.InvariantCulture),
				Y = Convert.ToSingle(info.GetString("Y2"), CultureInfo.InvariantCulture),
				Z = Convert.ToSingle(info.GetString("Z2"), CultureInfo.InvariantCulture)
			};

			boundingBox.Minimum = min;
			boundingBox.Maximum = max;

			return boundingBox;
		}
	}
}
