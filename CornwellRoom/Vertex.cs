using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CornwellRoom
{
    class Vertex
    {
        public double X;
        public double Y;
        public double Z;
        public double W;


        public Vertex() { new Vertex(0, 0, 0, 1); }

        public Vertex(double x, double y, double z, double w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        
        public Vertex(Vertex copy)
        {
            this.X = copy.X;
            this.Y = copy.Y;
            this.Z = copy.Z;
            this.W = copy.W;
        }

        public void Apply(Matrix t)
        {
            double[] coords = { X, Y, Z, W };
            double[] newCoords = new double[4];
            for (int i = 0; i < 4; ++i)
            {
                newCoords[i] = 0;
                for (int j = 0; j < 4; ++j)
                    newCoords[i] += coords[j] * t.data[j, i];
            }
            X = newCoords[0];
            Y = newCoords[1];
            Z = newCoords[2];
            W = newCoords[3];
        }
        static public bool operator ==(Vertex p1, Vertex p2) => !(p1 != p2);
        static public bool operator !=(Vertex p1, Vertex p2) => p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z;
        static public Vertex operator -(Vertex p1, Vertex p2) => new Vertex(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        static public Vertex operator +(Vertex p1, Vertex p2) => new Vertex(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        static public Vertex operator -(Vertex p) => new Vertex(-p.X, -p.Y, -p.Z);
        static public Vertex operator *(Vertex p, double x) => new Vertex(x * p.X, x * p.Y, x * p.Z);
        static public Vertex operator *(Vertex p1, Vertex p2) => new Vertex(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
        static public Vertex operator /(Vertex p, double x) => new Vertex(p.X / x, p.Y / x, p.Z / x);
        public double Distance() => Math.Sqrt(X * X + Y * Y + Z * Z);

        static public double scalar(Vertex vec1, Vertex vec2)
        {
            var mult_vec = vec1 * vec2;
            return mult_vec.X + mult_vec.Y + mult_vec.Z;
        }
        static public Vertex vector(Vertex vec1, Vertex vec2) => new Vertex(vec1.Y * vec2.Z - vec1.Z * vec2.Y,
            vec1.Z * vec2.X - vec1.X * vec2.Z,
            vec1.X * vec2.Y - vec1.Y * vec2.X);
        static public double cos(Vertex v1, Vertex v2) => scalar(v1, v2) / (v1.Distance() * v2.Distance());


        public Color ToColor()
        {
            var red =(int)(( X < 0 ? 0 : X)*255);
            var green = (int)((Y < 0 ? 0 : Y) * 255);
            var blue = (int)((Z < 0 ? 0 : Z) * 255);
            return Color.FromArgb(red>255?255:red,green > 255 ? 255 :green, blue > 255 ? 255 :blue);
        }

        public static Vertex ToVertex(Color c) => new Vertex(c.R/255,c.G/255,c.B/255);

    }
}
