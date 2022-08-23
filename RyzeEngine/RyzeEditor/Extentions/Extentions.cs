using SharpDX;
using System;
using System.Collections.Generic;
using RyzeEditor.ResourceManagment;
using RyzeEditor.Renderer;

namespace RyzeEditor.Extentions
{
    public static class Extensions
    {
        public static byte[] GetBytes(this Vector2 vector)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(vector.X));
            bytes.AddRange(BitConverter.GetBytes(vector.Y));

            return bytes.ToArray();
        }

        public static byte[] GetBytes(this Vector3 vector)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(vector.X));
            bytes.AddRange(BitConverter.GetBytes(vector.Y));
            bytes.AddRange(BitConverter.GetBytes(vector.Z));

            return bytes.ToArray();
        }

        public static byte[] GetPositionBytes(this Vertex vertex)
        {
            var bytes = new List<byte>();

            float padding = 0;

            bytes.AddRange(vertex.Pos.GetBytes());
            bytes.AddRange(BitConverter.GetBytes(padding));

            return bytes.ToArray();
        }

        public static byte[] GetTexNormBytes(this Vertex vertex)
        {
            var bytes = new List<byte>();

            bytes.AddRange(vertex.Tex.GetBytes());

            ushort padding = 0;

            var norm = new Half3(vertex.Norm.X, vertex.Norm.Y, vertex.Norm.Z);
            bytes.AddRange(BitConverter.GetBytes(norm.X.RawValue));
            bytes.AddRange(BitConverter.GetBytes(norm.Y.RawValue));
            bytes.AddRange(BitConverter.GetBytes(norm.Z.RawValue));
            bytes.AddRange(BitConverter.GetBytes(padding));

            return bytes.ToArray();
        }

        public static byte[] GetTangentBytes(this Vertex vertex)
        {
            var bytes = new List<byte>();

            ushort padding = 0;

            var tangent = new Half3(vertex.Tangent.X, vertex.Tangent.Y, vertex.Tangent.Z);
            bytes.AddRange(BitConverter.GetBytes(tangent.X.RawValue));
            bytes.AddRange(BitConverter.GetBytes(tangent.Y.RawValue));
            bytes.AddRange(BitConverter.GetBytes(tangent.Z.RawValue));
            bytes.AddRange(BitConverter.GetBytes(padding));

            var bitangent = new Half3(vertex.Bitangent.X, vertex.Bitangent.Y, vertex.Bitangent.Z);
            bytes.AddRange(BitConverter.GetBytes(bitangent.X.RawValue));
            bytes.AddRange(BitConverter.GetBytes(bitangent.Y.RawValue));
            bytes.AddRange(BitConverter.GetBytes(bitangent.Z.RawValue));
            bytes.AddRange(BitConverter.GetBytes(padding));

            return bytes.ToArray();
        }

        public static byte[] GetBytes(this PatchData patchData)
        {
            float posPadding = 0;
            ushort normPadding = 0;
            long structPadding = 0;
            var bytes = new List<byte>();

            for (int i = 0; i < patchData.Positions.Count; i++)
            {
                bytes.AddRange(patchData.Positions[i].GetBytes());
                bytes.AddRange(BitConverter.GetBytes(posPadding));
            }
            for (int i = 0; i < patchData.Normals.Count; i++)
            {
                var norm = new Half3(patchData.Normals[i].X, patchData.Normals[i].Y, patchData.Normals[i].Z);
                bytes.AddRange(BitConverter.GetBytes(norm.X.RawValue));
                bytes.AddRange(BitConverter.GetBytes(norm.Y.RawValue));
                bytes.AddRange(BitConverter.GetBytes(norm.Z.RawValue));
                bytes.AddRange(BitConverter.GetBytes(normPadding));
            }

            //Padding: 8 bytes:
            bytes.AddRange(BitConverter.GetBytes(structPadding));

            return bytes.ToArray();
        }

        public static byte[] GetBytes(this Quaternion quaternion)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(quaternion.X));
            bytes.AddRange(BitConverter.GetBytes(quaternion.Y));
            bytes.AddRange(BitConverter.GetBytes(quaternion.Z));
            bytes.AddRange(BitConverter.GetBytes(quaternion.W));

            return bytes.ToArray();
        }

        public static byte[] GetBytes(this Point3 point)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(point.Position.X));
            bytes.AddRange(BitConverter.GetBytes(point.Position.Y));
            bytes.AddRange(BitConverter.GetBytes(point.Position.Z));
            bytes.AddRange(BitConverter.GetBytes(point.Position.W));

            bytes.AddRange(BitConverter.GetBytes(point.Color.X));
            bytes.AddRange(BitConverter.GetBytes(point.Color.Y));
            bytes.AddRange(BitConverter.GetBytes(point.Color.Z));
            bytes.AddRange(BitConverter.GetBytes(point.Color.W));
            return bytes.ToArray();
        }

        public static decimal ToDecimal(this float argument)
        {
            decimal decValue;

            try
            {
                decValue = (decimal)argument;
            }
            catch (Exception)
            {
                decValue = 0;
            }

            return decValue;
        }

        public static Matrix3x3 GetMatrix3x3(this Matrix matrix)
        {
            Matrix3x3 result = Matrix3x3.Zero;

            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;

            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;

            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;

            return result;
        }

        /// <summary>
        /// Get array of bytes (Column major order)
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Matrix3x3 matrix)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(matrix.M11));
            bytes.AddRange(BitConverter.GetBytes(matrix.M21));
            bytes.AddRange(BitConverter.GetBytes(matrix.M31));
            bytes.AddRange(BitConverter.GetBytes(0.0f));

            bytes.AddRange(BitConverter.GetBytes(matrix.M12));
            bytes.AddRange(BitConverter.GetBytes(matrix.M22));
            bytes.AddRange(BitConverter.GetBytes(matrix.M32));
            bytes.AddRange(BitConverter.GetBytes(0.0f));

            bytes.AddRange(BitConverter.GetBytes(matrix.M13));
            bytes.AddRange(BitConverter.GetBytes(matrix.M23));
            bytes.AddRange(BitConverter.GetBytes(matrix.M33));
            bytes.AddRange(BitConverter.GetBytes(0.0f));

            return bytes.ToArray();
        }

        public static Vector3 ToEuler(this Quaternion quaternion)
        {
            const float Singularity = 0.499f;

            float ww = quaternion.W * quaternion.W;
            float xx = quaternion.X * quaternion.X;
            float yy = quaternion.Y * quaternion.Y;
            float zz = quaternion.Z * quaternion.Z;
            float lengthSqd = xx + yy + zz + ww;
            float singularityTest = quaternion.Y * quaternion.W - quaternion.X * quaternion.Z;
            float singularityValue = Singularity * lengthSqd;
			    return singularityTest > singularityValue
                    ? new Vector3(-2.0f * (float)Math.Atan2(quaternion.Z, quaternion.W), 90.0f, 0.0f)
				    : singularityTest < -singularityValue
                        ? new Vector3(2.0f * (float)Math.Atan2(quaternion.Z, quaternion.W), -90.0f, 0.0f)
					    : new Vector3((float)Math.Atan2(2.0f * (quaternion.Y * quaternion.Z + quaternion.X * quaternion.W), 1.0f - 2.0f * (xx + yy)),
                            (float)Math.Asin(2.0f * singularityTest / lengthSqd),
                            (float)Math.Atan2(2.0f * (quaternion.X * quaternion.Y + quaternion.Z * quaternion.W), 1.0f - 2.0f * (yy + zz)));
        }
    }

    public class PatchData
    {
        public PatchData()
        {
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
        }

        public static readonly int SizeInBytes = 7 * 4 * sizeof(float) + // Positions : 7 control points //112 bytes
                                                 3 * 4 * sizeof(short) + // Normals   : 3 control points //24  bytes
                                                     2 * sizeof(float);  // Padding   :                  //8   bytes

        public int PatchId { get; set; }

        public List<Vector3> Positions { get; set; }

        public List<Vector3> Normals { get; set; }
    }
}