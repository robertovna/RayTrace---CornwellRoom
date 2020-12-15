using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CornwellRoom
{
    class Ray
    {
        public Vertex pos;
        public Vertex dir;
        public Material material;

        public Ray(Vertex position, Vertex direction)
        {
            pos = position;
            dir = direction / direction.Distance();
        }

        //луч отражения
        public Ray ReflectRay(Vertex ver,Vertex normal) => new Ray(ver, dir - normal * Vertex.scalar(normal, dir) * 2);

        //луч преломления
        public Ray TransparencyRay(Vertex ver,Vertex normal, double n1_n2 = 1)
        {
            var cos_I_N = Vertex.scalar(dir, normal);
            var cos = cosTransparencyRay(cos_I_N, n1_n2);
            if (cos >= 0)
                return new Ray(ver, dir * n1_n2 - normal * (cos + n1_n2 * cos_I_N));
            return null;
        }

        //угол преломления
        private static double cosTransparencyRay(double cos_I_N, double n1_n2) => Math.Sqrt(1 - n1_n2* n1_n2 * (1 - cos_I_N * cos_I_N));

        //Теневой луч
        public Vertex ShadowRay(Light l, Vertex ver, Vertex normal) => material.color * material.diffusion * Vertex.cos(normal, l.position - ver) * l.diffusion;

    }
}
