using SharpDX;
using System;

namespace RyzeEditor.Helpers
{
    public class MatrixHelper
    {
        public static Matrix MatrixRightHandedRotationX(float angle)
        {
            var matrix = Matrix.Identity;

            matrix.M22 = (float)Math.Cos(angle);
            matrix.M23 = -(float)Math.Sin(angle);
            matrix.M32 = (float)Math.Sin(angle);
            matrix.M33 = (float)Math.Cos(angle);

            return matrix;
        }

        public static Matrix MatrixRightHandedRotationY(float angle)
        {
            var matrix = Matrix.Identity;

            matrix.M11 = (float)Math.Cos(angle);
            matrix.M13 = (float)Math.Sin(angle);
            matrix.M31 = -(float)Math.Sin(angle);
            matrix.M33 = (float)Math.Cos(angle);

            return matrix;
        }

        public static Matrix MatrixRightHandedRotationZ(float angle)
        {
            var matrix = Matrix.Identity;

            matrix.M11 = (float)Math.Cos(angle);
            matrix.M12 = -(float)Math.Sin(angle);
            matrix.M21= (float)Math.Sin(angle);
            matrix.M22 = (float)Math.Cos(angle);

            return matrix;
        }

        public static BulletSharp.Matrix ConvertMatrix(Matrix matrix)
        {
            var output = new BulletSharp.Matrix
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,

                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,

                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,

                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44
            };

            return output;
        }

        public static SharpDX.Matrix ExtractMatrix(BulletSharp.Matrix matrix)
        {
            var output = new SharpDX.Matrix
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,

                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,

                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,

                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44
            };

            return output;
        }
    }
}
