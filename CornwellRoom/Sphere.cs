using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CornwellRoom
{
    class Sphere : Mesh
    {
        double r;
        Material material;
        public Sphere(Vertex cent, double radius,Material m)
        {
            center_point = cent;
            r = radius;
            material = m;
        }
        
        public override (double,Vertex,Material) Intersect(Ray ray)
        {
            Vertex oc = ray.pos - center_point;//вектор из луча в центр
            //коорд квадратного уравнения(получено из уравн точек на луче и на поверхн сферы)
            (double k1, double k2, double k3) = (Vertex.scalar(ray.dir,ray.dir),2 * Vertex.scalar(oc, ray.dir), Vertex.scalar(oc,oc) -r*r);

            double discr = k2 * k2 - 4 * k1 * k3;
            if (discr < 0)
                return (-1, null,null);
            var t = (-k2 + Math.Sqrt(discr)) / (2 * k1);
            var t1 = (-k2 - Math.Sqrt(discr)) / (2 * k1);
            if (t < t1 && t > 0.0001)
                return (t, Normal(ray,t),material);
            if (t1 < t && t1 > 0.0001)
                return (t1, Normal(ray,t1),material);
            return (-1, null,null);
        }

        public Vertex Normal(Ray r,double t)
        {
            var p = r.pos + r.dir * t;
            var n = p - center_point;
            return new Vertex(n / n.Distance());
        }

    }
}
