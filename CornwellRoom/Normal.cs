using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornwellRoom
{
    class Normal
    {
        public double X;
        public double Y;
        public double Z;


        public Normal() { new Normal(0, 0, 0); }

        public Normal(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
