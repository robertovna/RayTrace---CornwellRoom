using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornwellRoom
{
    class Matrix
    {
        public double[,] data;
        public Matrix()
        {
            data =
                new double[,] {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
        }

        public Matrix(double[,] matrix)
        {
            this.data = matrix;
        }

        public static Matrix Move(double dx, double dy, double dz)
        {
            return new Matrix(
                new double[,]
                {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 1, 0 },
                    { dx, dy, dz, 1 },
                });
        }

        public static Matrix Rotate(double angleX, double angleY, double angleZ)
        {
            double cosX = Math.Cos(angleX * Math.PI / 180);
            double sinX = Math.Sin(angleX * Math.PI / 180);
            double cosY = Math.Cos(angleY * Math.PI / 180);
            double sinY = Math.Sin(angleY * Math.PI / 180);
            double cosZ = Math.Cos(angleZ * Math.PI / 180);
            double sinZ = Math.Sin(angleZ * Math.PI / 180);
            double[,] x =
                {
                    { 1, 0, 0, 0 },
                    { 0, cosX, -sinX, 0 },
                    { 0, sinX, cosX, 0 },
                    { 0, 0, 0, 1 }
                };
            double[,] y =
                {
                    { cosY, 0, sinY, 0 },
                    { 0, 1, 0, 0 },
                    { -sinY, 0, cosY, 0 },
                    { 0, 0, 0, 1 }
                };
            double[,] z =
                {
                    { cosZ, -sinZ, 0, 0 },
                    { sinZ, cosZ, 0, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 0, 1 }
                };
            if (angleX == 0 && angleY == 0)
                return new Matrix(z);
            if (angleX == 0 && angleZ == 0)
                return new Matrix(y);
            if (angleY == 0 && angleZ == 0)
                return new Matrix(x);

            if (angleX == 0)
                return new Matrix(mult_matr(y, z));
            if (angleY == 0)
                return new Matrix(mult_matr(x, z));
            if (angleZ == 0)
                return new Matrix(mult_matr(x, y));

            return new Matrix(mult_matr(mult_matr(x, y), z));
        }

        public static Matrix Scale(double fx, double fy, double fz)
        {
            return new Matrix(
                new double[,] {
                    { fx, 0, 0, 0 },
                    { 0, fy, 0, 0 },
                    { 0, 0, fz, 0 },
                    { 0, 0, 0, 1 }
                });
        }
        public static Matrix ReflectX() => Scale(1, -1, -1);

        public static Matrix ReflectY() => Scale(-1, 1, -1);

        public static Matrix ReflectZ() => Scale(-1, -1, 1);

        public static Matrix PERSP(double dist, double x_angle, double y_angle)
        {
            
            double cosx = Math.Cos(x_angle);
            double sinx = Math.Sin(x_angle);
            double cosy = Math.Cos(y_angle);
            double siny = Math.Sin(y_angle);

            double[,] rotate_y =
                {
                    { cosy, 0, siny, 0 },
                    { 0, 1, 0, 0 },
                    { -siny, 0, cosy, 0 },
                    { 0, 0, 0, 1 }
                };

            double[,] rotate_x =
                {
                    { 1, 0, 0, 0 },
                    { 0, cosx, -sinx, 0 },
                    { 0, sinx, cosx, 0 },
                    { 0, 0, 0, 1 }
                };

            double[,] persp_matr =
                {
                    { 1, 0, 0, 0},
                    { 0, 1, 0, 0},
                    { 0, 0, 0, -1 / dist},
                    { 0, 0, 0, 1} 
                };

            return new Matrix(mult_matr(mult_matr(rotate_y, rotate_x), persp_matr));
        }

        public static double[,] mult_matr(double[,] a, double[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0))
            {
                throw new Exception("Matrix 1 has " + a.GetLength(1) + " cols, and Matrix 2 has " + b.GetLength(0) + " rows" + '\n'
                    + "Multiplication Impossible");
            }

            var c = new double[a.GetLength(0), b.GetLength(1)];
            for (var i = 0; i < a.GetLength(0); i++)
                for (var j = 0; j < b.GetLength(1); j++)
                {
                    c[i, j] = 0;
                    for (var k = 0; k < a.GetLength(1); k++)
                        c[i, j] += a[i, k] * b[k, j];
                }
            return c;
        }

    }
}
