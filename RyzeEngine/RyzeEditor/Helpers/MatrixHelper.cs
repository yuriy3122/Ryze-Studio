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
    }
}
