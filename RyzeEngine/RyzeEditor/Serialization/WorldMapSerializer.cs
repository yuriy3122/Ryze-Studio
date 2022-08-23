using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RyzeEditor.GameWorld;
using SharpDX;

namespace RyzeEditor.Serialization
{
	public static class WorldMapSerializer
	{
		public static void Serialize(WorldMap mapWorld, Stream stream)
		{
			if (mapWorld == null)
			{
				throw new ArgumentException("WorldMap reference is null");
			}

			if (stream == null)
			{
				throw new ArgumentException("Stream is null");
			}

            IFormatter formatter = new BinaryFormatter { SurrogateSelector = GetSelector() };
            formatter.Serialize(stream, mapWorld);
        }

		public static WorldMap Deserialize(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentException("Stream is null");
			}

			IFormatter formatter = new BinaryFormatter();
			formatter.SurrogateSelector = GetSelector();

			stream.Position = 0;

			return (WorldMap)formatter.Deserialize(stream);
		}

		private static SurrogateSelector GetSelector()
		{
			var surrogateSelector = new SurrogateSelector();
			surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new Vector2Surrogate());
			surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
            surrogateSelector.AddSurrogate(typeof(Matrix3x3), new StreamingContext(StreamingContextStates.All), new Matrix3x3Surrogate());
            surrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSurrogate());
            surrogateSelector.AddSurrogate(typeof(Matrix), new StreamingContext(StreamingContextStates.All), new MatrixSurrogate());
            surrogateSelector.AddSurrogate(typeof(BoundingBox), new StreamingContext(StreamingContextStates.All), new BoundingBoxSurrogate());
            surrogateSelector.AddSurrogate(typeof(BoundingSphere), new StreamingContext(StreamingContextStates.All), new BoundingSphereSurrogate());

            return surrogateSelector;
		}
	}
}