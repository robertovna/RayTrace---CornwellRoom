using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CornwellRoom
{
    class Light
    {
        public Vertex position;
        public Vertex diffusion;
        public Vertex ambient;

        public Light(Vertex pos, double dif,double amb)
        {
            position = pos;
            diffusion = new Vertex(dif, dif, dif);
            ambient = new Vertex(amb, amb, amb);
        }

        public Vertex IntensityDiffusion(double koeff, Vertex normal,Vertex p)
        {
            return diffusion * Vertex.cos(normal, position - p) * koeff;
        }

    }
}
