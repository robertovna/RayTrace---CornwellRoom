using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CornwellRoom
{
    public partial class Form1 : Form
    {
        private Bitmap bmp;
        private int width, height;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bmp.Dispose();
            bmp = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vertex p = GetPoint(x, y);
                    Vertex cam_pos = new Vertex(0, 0, 10);
                    Ray cam_ray = new Ray(p, p - cam_pos);
                    Color c = TraceRay(cam_ray, 10).ToColor();
                    bmp.SetPixel(x, height - 1 - y, c);
                }
            pictureBox1.Image = bmp;
            pictureBox1.Invalidate();
        }

        private List<Mesh> meshes = new List<Mesh>();
        private List<Light> lights = new List<Light>();

        private Mesh room;

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateScene();
            InitSteps();
        }

        private Vertex TraceRay(Ray r,int rec, double accuracy = 1)
        {
            Vertex i = new Vertex();
            if (rec <= 0 || accuracy < 0.0001)
                return i;

            double closet_t = -1;
            Vertex n = new Vertex();
            Material mat = null;
            foreach (var mesh in meshes)
            {
                (var t, var n1, var mat1) = mesh.Intersect(r);
                if (t!=-1 && (t < closet_t|| closet_t == -1))
                {
                    closet_t = t;
                    n = n1;
                    mat = mat1;
                }
            }
            if (mat == null)
                return i;
            r.material = mat;
            if (Vertex.scalar(r.dir, n) > 0)
                n *= -1;

            Vertex p = r.pos + r.dir * closet_t;
            foreach (var l in lights)
            {
                i += mat.color * l.ambient * mat.ambient;
                (bool is_shadowpace, double refr_coef) = ShadowPlace(l.position, p);
                if (is_shadowpace || refr_coef>0)
                    i += r.ShadowRay(l, p, n) * refr_coef;
            }

            if (mat.reflection > 0)
            {
                Ray reflRay = r.ReflectRay(p, n);
                i +=  TraceRay(reflRay, rec - 1,  accuracy * mat.reflection) * mat.reflection;
            }

            if (mat.refraction > 0)
            {
                Ray refrRay = r.TransparencyRay(p, n);
                i += TraceRay(refrRay, rec - 1, accuracy * mat.refraction) * mat.refraction;
            }
            return i;
        }

        private (bool,double) ShadowPlace(Vertex light, Vertex ver)
        {
            double max_t = (light - ver).Distance();
            Ray r = new Ray(ver, light - ver);

            foreach (Mesh m in meshes)
            {
                (double t,var v,var mat) = m.Intersect(r);
                if (t!=-1 && t < max_t && t > 0.00001f)
                    return (false, mat.refraction);
            }
            return (true,1);
        }
        private void CreateScene()
        {
            room = new Mesh();
            room.FromFile("cube2.obj",new Material(Color.White, 0.4, 0.5, 0, 0));
            room.Scale(5, 5, 5);
            room.triangles[0].Material.color = Vertex.ToVertex(Color.Yellow);//слева
            room.triangles[6].Material.color = Vertex.ToVertex(Color.Yellow);
            room.triangles[1].Material.color = Vertex.ToVertex(Color.Yellow);//спереди
            room.triangles[7].Material.color = Vertex.ToVertex(Color.Yellow);
            room.triangles[2].Material.color = Vertex.ToVertex(Color.Yellow);//справа
            room.triangles[8].Material.color = Vertex.ToVertex(Color.Yellow);
            room.triangles[3].Material.color = Vertex.ToVertex(Color.Yellow);//сзади
            room.triangles[9].Material.color = Vertex.ToVertex(Color.Yellow);
            room.triangles[4].Material.color = Vertex.ToVertex(Color.Yellow);//снизу
            room.triangles[10].Material.color = Vertex.ToVertex(Color.Yellow);
            room.triangles[5].Material.color = Vertex.ToVertex(Color.Yellow);//сверху
            room.triangles[11].Material.color = Vertex.ToVertex(Color.Yellow);
            meshes.Add(room);

            Mesh cube1 = new Mesh();
            cube1.FromFile("cube2.obj", new Material(Color.Silver, 0.1, 0.5, 0.6, 0));
            cube1.Move(-3, -4, 0);
            meshes.Add(cube1);
            cube1 = new Mesh();
            cube1.FromFile("cube2.obj", new Material(Color.Red,  0.7, 0.3, 0, 0));
            cube1.Move(-4, -2.5, -4);
            meshes.Add(cube1);
            cube1 = new Mesh();
            cube1.FromFile("cube2.obj", new Material(Color.White,  0.2, 0.05, 0, 0.9));
            cube1.Move(3, -3, 4);
            meshes.Add(cube1);

            meshes.Add(new Sphere(new Vertex(3, -3, -2), 2, new Material(Color.Red, 0.7, 0.3, 0, 0)));
            meshes.Add(new Sphere(new Vertex(0, -4, -4), 1, new Material(Color.Silver,  0.4, 0.5, 0.6, 0)));
            meshes.Add(new Sphere(new Vertex(-3, -4, 3), 1, new Material(Color.White,  0.2, 0.05, 0, 0.9)));

            lights.Add(new Light(new Vertex(-4, 4, -4), 0.8,0.3));
            lights.Add(new Light(new Vertex(4, 4.5, -4.5), 0.4,0.3));
        }



        void InitSteps()
        {
            (width, height) = (bmp.Width, bmp.Height);
            //front face steps - if cam on OZ
            step_width = (room.geometric_vertices[5] - room.geometric_vertices[1]) / width;
            down = room.geometric_vertices[0];
            up = room.geometric_vertices[1];
        }

        private Vertex down, up, step_width;

        private Vertex GetPoint(int x, int y)
        {
            var u = up + step_width * x;
            var d = down + step_width * x;
            var step_height = (u - d) / height;
            return d + step_height * y;
        }

        //left
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            foreach(var t in room.triangles)
                t.Material.reflection = 0;
            room.triangles[0].Material.reflection = 0.9;
            room.triangles[6].Material.reflection = 0.9;
        }

        //right
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var t in room.triangles)
                t.Material.reflection = 0;
            room.triangles[2].Material.reflection = 0.9;
            room.triangles[8].Material.reflection = 0.9;
        }

        //front
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var t in room.triangles)
                t.Material.reflection = 0;
            room.triangles[1].Material.reflection = 0.9;
            room.triangles[7].Material.reflection = 0.9;
        }

        //none
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var t in room.triangles)
                t.Material.reflection = 0;
        }
    }
}
