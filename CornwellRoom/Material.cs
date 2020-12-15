using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CornwellRoom
{
    class Material
    {
        public Vertex color;

        public double ambient = 0;
        public double diffusion = 0;

        public double reflection = 0;
        public double refraction = 0;

        public Material(Material m)
        {
            color = m.color;
            ambient = m.ambient;
            diffusion = m.diffusion;
            reflection = m.reflection;
            refraction = m.refraction;
        }

        public Material(Color col,  double amb, double diff, double refl, double refr)
        {
            color = Vertex.ToVertex(col);
            ambient = amb;
            diffusion = diff;
            reflection = refl;
            refraction = refr;
        }

    }
}
