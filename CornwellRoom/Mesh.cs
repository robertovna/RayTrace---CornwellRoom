using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornwellRoom
{
    class Mesh
    {
        // points of polyhedron
        public List<Vertex> geometric_vertices = new List<Vertex>();

        // faces of polyhedron
        public List<Triangle> triangles = new List<Triangle>();

        // central point of polyhedron (weights)
        public Vertex center_point = new Vertex();

        public Mesh(List<Vertex> vert, List<Triangle> triangles_, Material mat)
        {
            geometric_vertices = vert;
            triangles = triangles_;
            center_point = SetCenter();
            foreach (var t in triangles)
            {
                t.Normal = t.normal(geometric_vertices);
                t.Material = new Material(mat);
            }
                
        }

        public Mesh() { }

        public void Clear()
        {
            geometric_vertices.Clear();
            triangles.Clear();
        }

        public void FromFile(string filename, Material mat)
        {
            Clear();

            StreamReader sr = File.OpenText(filename);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == "")
                    continue;
                line = line.Replace(',', '.');
                string[] ss = line.Split();
                ss = ss.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string type = ss[0];
                switch (type)
                {
                    case "v":
                        geometric_vertices.Add(new Vertex(double.Parse(ss[1], CultureInfo.InvariantCulture),
                            double.Parse(ss[2], CultureInfo.InvariantCulture),
                            double.Parse(ss[3], CultureInfo.InvariantCulture),
                            1));
                        break;

                    case "f":
                        triangles.Add(new Triangle(ss[1], ss[2], ss[3]));
                        break;

                    default:
                        break;
                }
            }

            sr.Close();
            center_point = SetCenter();
            foreach (var t in triangles)
            { 
                t.Normal = t.normal(geometric_vertices);
                t.Material = new Material(mat);
            }
        }


        public virtual (double, Vertex,Material) Intersect(Ray ray)
        {
            double near_t = -1;
            Vertex norm = null;
            Material m = null;

            foreach(var triangle in triangles)
            {
                (var t, var n) = triangle.Intersect(ray, geometric_vertices);
                if (t!=-1  && (t < near_t || near_t == -1))
                {
                    near_t = t;
                    norm = n;
                    m = triangle.Material;
                }
            }
            return (near_t,norm, m);
        }

        public Vertex SetCenter()
        {
            Vertex p = new Vertex();
            foreach (Vertex p1 in geometric_vertices)
                p += p1;
            return p / geometric_vertices.Count();
        }

         // Affine transformations
        public void Move(double dx, double dy, double dz)
        {
            Matrix matrix = Matrix.Move(dx, dy, dz);
            foreach (Vertex v in geometric_vertices)
                v.Apply(matrix);
            center_point.Apply(matrix);
        }

        public void Rotate(double angleX, double angleY, double angleZ)
        {
            Matrix matrix = Matrix.Rotate(angleX, angleY, angleZ);
            foreach (Vertex point in geometric_vertices)
                point.Apply(matrix);
            center_point.Apply(matrix);
        }

        public void Scale(double fx, double fy, double fz)
        {
            Matrix matrix = Matrix.Scale(fx, fy, fz);
            foreach (Vertex point in geometric_vertices)
                point.Apply(matrix);
            center_point.Apply(matrix);
        }

    }
}
