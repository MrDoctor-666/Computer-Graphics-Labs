using System;
using OpenTK;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Windows.Media.Media3D;

namespace CG_lab1_try2
{
    public partial class Form1 : Form
    {
        GLControl renderCanvas1;
        Rotation rot = new Rotation();
        int winX, winY, winW, winH;
        int angleX = 0, angleY = 0, angleZ = 0;
        Point3D[] points = new Point3D[] { 
            new Point3D(-1, -1, 0),
            new Point3D(0.2, -1, 0),
            new Point3D(0.2, 0.2, 0),
            new Point3D(0.4, 0, 0),
            new Point3D(0.5, 0.5, 0),
            new Point3D(0.5, 0.5, 0.5),
            new Point3D(0.7, 0.6, 0.5),
            new Point3D(0.8, 0, 0.5),
            new Point3D(0.8, 1, 0),
            new Point3D(1, 1, -1),
        };


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.renderCanvas1 = new GLControl();
            //this.SuspendLayout();

            winX = this.Location.X; winY = this.Location.Y;
            winW = this.Width; winH = this.Height;

            this.renderCanvas1.BackColor = Color.CadetBlue;
            this.renderCanvas1.Location =
                new System.Drawing.Point(winX, winY);
            this.renderCanvas1.Name = "renderCanvas1";
            this.renderCanvas1.Size = new Size((int)(5* winH / 6), 5*winH/6);
            /*this.renderCanvas1.Location = new System.Drawing.Point(200, 200);
            this.renderCanvas1.Name = "glControl1";
            this.renderCanvas1.Size = new System.Drawing.Size(400, 400);*/
            this.renderCanvas1.TabIndex = 1;
            this.renderCanvas1.VSync = false;
            this.renderCanvas1.Load +=
                new System.EventHandler(this.renderCanvas_Load);
            this.renderCanvas1.Paint +=
                new System.Windows.Forms.PaintEventHandler(
                    this.renderCanvas_Paint);
            this.Controls.Add(this.renderCanvas1);
            textBox3.Text = angleX.ToString();
            textBox2.Text = angleY.ToString();
            textBox1.Text = angleZ.ToString();
            glControl_Resize(renderCanvas1, EventArgs.Empty);
        }
        private void glControl_Resize(object sender, EventArgs e)
        {
            renderCanvas1.MakeCurrent();

            if (renderCanvas1.ClientSize.Height == 0)
                renderCanvas1.ClientSize = new System.Drawing.Size(renderCanvas1.ClientSize.Width, 1);

            GL.Viewport(0, 0, renderCanvas1.ClientSize.Width, renderCanvas1.ClientSize.Height);

            float aspect_ratio = Math.Max(renderCanvas1.ClientSize.Width, 1) / (float)Math.Max(renderCanvas1.ClientSize.Height, 1);
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
            SetInitValues();
        }

        void SetInitValues()
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    int num;
                    Int32.TryParse(control.Name.Remove(0, 13), out num);
                    Console.WriteLine(control.Name + " " + num);
                    switch (num % 3)
                    {
                        case 0:
                            ((NumericUpDown)control).Value = (decimal)points[num / 3].X;
                            break;
                        case 1:
                            ((NumericUpDown)control).Value = (decimal)points[num / 3].Y;
                            break;
                        case 2:
                            ((NumericUpDown)control).Value = (decimal)points[num / 3].Z;
                            break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    int num;
                    Int32.TryParse(control.Name.Remove(0, 13), out num);
                    Console.WriteLine(control.Name + " " + num);
                    switch (num % 3)
                    {
                        case 0:
                            points[num / 3].X = (double)((NumericUpDown)control).Value;
                            break;
                        case 1:
                            points[num / 3].Y = (double)((NumericUpDown)control).Value;
                            break;
                        case 2:
                            points[num / 3].Z = (double)((NumericUpDown)control).Value;
                            break;
                    }
                }
            }

            //angleX = 0; angleY = 0; angleZ = 0;
            //trackBar1.Value = 0; textBox1.Text = "0";
            //trackBar2.Value = 0; textBox2.Text = "0";
            //trackBar3.Value = 0; textBox3.Text = "0";
            RePaint();
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int value;
                if (Int32.TryParse(textBox3.Text, out value))
                {
                    if (value > 360 || value < 0) value %= 360;
                    textBox3.Text = value.ToString();
                    angleX = value;
                    trackBar1.Value = angleX;
                }
                else
                    textBox3.Text = angleX.ToString();
            }
            RePaint();
                
        }

        private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {
            //GL.Viewport(winX - 25, winY - 25, 5*Width/6, 5*Height/6);
            RePaint();
           
        }

        protected void SetupCamera()
        {
            Matrix4 lookat = Matrix4.LookAt(new Vector3(3, 3, 5), Vector3.Zero, Vector3.UnitY);
            //Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
        }

        private void RePaint()
        {
            //Technical set up
            renderCanvas1.MakeCurrent();
            GL.Enable(EnableCap.DepthTest);
            SetupCamera();
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            //Drawing
            //GL.Rotate(angleX, Vector3d.UnitX);
            //GL.Rotate(angleY, Vector3d.UnitY);

            Point3D[] pointsNew = new Point3D[points.Length];
            for (int i = 0; i < pointsNew.Length; i++)
            {
                pointsNew[i] = rot.RotateArounAxis(points[i], Axes.X, angleX);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Y, angleY);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Z, angleZ);
            }

            drawRefLines();
            //drawCube();
            DrawLines(pointsNew, Color4.Red);
            BezierCurvesCustom curve = new BezierCurvesCustom(pointsNew);
            DrawLines(curve.PointsToDraw(), Color4.Green);

            GL.Flush();
            renderCanvas1.SwapBuffers();
            
        }

        private void renderCanvas_Load(object sender, EventArgs e)
        {
            // Specify the color for clearing
            GL.ClearColor(Color.SkyBlue);
        }

        void DrawLines(Point3D[] toDraw, Color4 color)
        {
            for (int i = 1; i < toDraw.Length; i++)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color4(color);

                GL.Vertex3(toDraw[i - 1].X, toDraw[i - 1].Y, toDraw[i - 1].Z);
                GL.Vertex3(toDraw[i].X, toDraw[i].Y, toDraw[i].Z);

                GL.End();
            }
        }

        void drawCube()
        {
            GL.Begin(BeginMode.Quads);

            GL.Color4(Color4.Silver);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color4(Color4.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color4(Color4.Moccasin);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color4(Color4.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color4(Color4.BlueViolet);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color4(Color4.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();
        }

        void drawRefLines()
        {
            //void glColor3d(GLdouble red, GLdouble green, GLdouble blue); 
            GL.Color4(Color4.Black);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(+1000.0, 0.0, 0.0);
            GL.Vertex3(-1000.0, 0.0, 0.0);
            GL.End();
            //GL.Color3(0.0, 1.0, 0.0);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0.0, +1000.0, 0.0);
            GL.Vertex3(0.0, -1000.0, 0.0);
            GL.End();
            //GL.Color3(0.0, 0.0, 1.0);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0.0, 0.0, +1000.0);
            GL.Vertex3(0.0, 0.0, -1000.0);
            GL.End();

        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            angleX = trackBar1.Value;
            textBox3.Text = angleX.ToString();
            RePaint();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            angleY = trackBar2.Value;
            textBox2.Text = angleY.ToString();
            RePaint();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            angleZ = trackBar3.Value;
            textBox1.Text = angleZ.ToString();
            RePaint();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
