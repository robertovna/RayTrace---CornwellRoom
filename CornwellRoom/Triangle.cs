using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornwellRoom
{
    class Triangle
    {
        public int v1, v2, v3;

        public Material Material;
        public Vertex Normal;

        public Triangle(string vertex1, string vertex2, string vertex3)
        {
            string []coords1 = vertex1.Split('/');
            string []coords2 = vertex2.Split('/');
            string []coords3 = vertex3.Split('/');

            v1 = int.Parse(coords1[0]) - 1;
            v2 = int.Parse(coords2[0]) - 1;
            v3 = int.Parse(coords3[0]) - 1;
        }

        public Vertex normal(List<Vertex> vertices) => Vertex.vector(vertices[v2] - vertices[v1], vertices[v2] - vertices[v3]);
        public (double,Vertex) Intersect(Ray ray, List<Vertex> vertices)
        {
            Vertex edge1 = vertices[v2] - vertices[v1];
            Vertex edge2 = vertices[v3] - vertices[v1];
            Vertex P = Vertex.vector(ray.dir, edge2);
            double det = Vertex.scalar(edge1, P);
            if (det > -0.0001 && det < 0.0001)//луч лежит в плоскости треуг
                return (-1, null);
            
            double koef = 1 / det;
            Vertex T = ray.pos - vertices[v1]; //растояние от верш до нач луча
            double u = Vertex.scalar(T, P) * koef;
            if (u < 0 || u > 1)//проверка, что параметр в границе
                return (-1, null);
            Vertex Q = Vertex.vector(T , edge1);
            double v = Vertex.scalar(ray.dir, Q) * koef;
            if (v < 0 || u + v > 1)
                return (-1, null);
            double t = Vertex.scalar(edge2, Q) * koef;

            if (t > 0.0001) 
                return (t,Normal);
            return (-1, null);
        }

        //Пересечение луча и треугольника основано на равенстве уравнения луча и барицентрического уравнения точек треугольника.
        //С помощью правила крамера
    }
}
