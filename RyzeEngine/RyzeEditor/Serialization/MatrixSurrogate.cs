using System;
using System.Globalization;
using System.Runtime.Serialization;
using SharpDX;

namespace RyzeEditor.Serialization
{
    public class MatrixSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var matrix = (Matrix)obj;
            info.AddValue("M11", matrix.M11);
            info.AddValue("M12", matrix.M12);
            info.AddValue("M13", matrix.M13);
            info.AddValue("M14", matrix.M14);
            info.AddValue("M21", matrix.M21);
            info.AddValue("M22", matrix.M22);
            info.AddValue("M23", matrix.M23);
            info.AddValue("M24", matrix.M24);
            info.AddValue("M31", matrix.M31);
            info.AddValue("M32", matrix.M32);
            info.AddValue("M33", matrix.M33);
            info.AddValue("M34", matrix.M34);
            info.AddValue("M41", matrix.M41);
            info.AddValue("M42", matrix.M42);
            info.AddValue("M43", matrix.M43);
            info.AddValue("M44", matrix.M44);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var matrix = (Matrix)obj;
            matrix.M11 = Convert.ToSingle(info.GetString("M11"), CultureInfo.InvariantCulture);
            matrix.M12 = Convert.ToSingle(info.GetString("M12"), CultureInfo.InvariantCulture);
            matrix.M13 = Convert.ToSingle(info.GetString("M13"), CultureInfo.InvariantCulture);
            matrix.M14 = Convert.ToSingle(info.GetString("M14"), CultureInfo.InvariantCulture);
            matrix.M21 = Convert.ToSingle(info.GetString("M21"), CultureInfo.InvariantCulture);
            matrix.M22 = Convert.ToSingle(info.GetString("M22"), CultureInfo.InvariantCulture);
            matrix.M23 = Convert.ToSingle(info.GetString("M23"), CultureInfo.InvariantCulture);
            matrix.M24 = Convert.ToSingle(info.GetString("M24"), CultureInfo.InvariantCulture);
            matrix.M31 = Convert.ToSingle(info.GetString("M31"), CultureInfo.InvariantCulture);
            matrix.M32 = Convert.ToSingle(info.GetString("M32"), CultureInfo.InvariantCulture);
            matrix.M33 = Convert.ToSingle(info.GetString("M33"), CultureInfo.InvariantCulture);
            matrix.M34 = Convert.ToSingle(info.GetString("M34"), CultureInfo.InvariantCulture);
            matrix.M41 = Convert.ToSingle(info.GetString("M41"), CultureInfo.InvariantCulture);
            matrix.M42 = Convert.ToSingle(info.GetString("M42"), CultureInfo.InvariantCulture);
            matrix.M43 = Convert.ToSingle(info.GetString("M43"), CultureInfo.InvariantCulture);
            matrix.M44 = Convert.ToSingle(info.GetString("M44"), CultureInfo.InvariantCulture);

            return matrix;
        }
    }
}
