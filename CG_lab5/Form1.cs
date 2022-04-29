using System;
using OpenTK;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Windows.Media.Media3D;

namespace CG_lab5
{
    public partial class Form1 : Form
    {
        GLControl renderCanvas1;
        Rotation rot = new Rotation();
        int winX, winY, winW, winH;
        int angleX = 0, angleY = 0, angleZ = 0;
        Point3D[] points = new Point3D[16];
        Cube cube = new Cube();
        Cube cubeNew;
        bool pause = false;
        Vector3 viewPoint = new Vector3(3, 3, 5);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cube = new Cube();
            cubeNew = new Cube(cube);
            this.renderCanvas1 = new GLControl();

            winX = this.Location.X; winY = this.Location.Y;
            winW = this.Width; winH = this.Height;

            this.renderCanvas1.BackColor = Color.CadetBlue;
            this.renderCanvas1.Location =
                new System.Drawing.Point(winX, winY);
            this.renderCanvas1.Name = "renderCanvas1";
            this.renderCanvas1.Size = new Size((int)(5* winH / 6), 5*winH/6);
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
                    char letter = control.Name[control.Name.Length - 1];
                    switch (letter)
                    {
                        case 'x':
                            ((NumericUpDown)control).Value = (decimal)viewPoint.X;
                            break;
                        case 'y':
                            ((NumericUpDown)control).Value = (decimal)viewPoint.Y;
                            break;
                        case 'z':
                            ((NumericUpDown)control).Value = (decimal)viewPoint.Z;
                            break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    char letter = control.Name[control.Name.Length - 1];
                    switch (letter)
                    {
                        case 'x':
                            viewPoint.X = (float)((NumericUpDown)control).Value;
                            break;
                        case 'y':
                            viewPoint.Y = (float)((NumericUpDown)control).Value;
                            break;
                        case 'z':
                            viewPoint.Z = (float)((NumericUpDown)control).Value;
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

        protected void SetupCamera()
        {
            Matrix4 lookat = Matrix4.LookAt(viewPoint, Vector3.Zero, Vector3.UnitY);
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

            Point3D[] pointsNew = new Point3D[24];
            for (int i = 0; i < 24; i++)
            {
                pointsNew[i] = rot.RotateArounAxis(cube.cubeVertex[i], Axes.X, angleX);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Y, angleY);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Z, angleZ);
            }

            cubeNew.cubeVertex = pointsNew;
            drawRefLines();
            if (!pause)
            {
                cubeNew = new Cube(cube);
                cubeNew.cubeVertex = pointsNew;
                cubeNew.Draw();
                cubeNew.CountVisible(new Point3D(viewPoint.X, viewPoint.Y, viewPoint.Z));
                cube.CountVisible(new Point3D(viewPoint.X, viewPoint.Y, viewPoint.Z));
                //cubeNew.Draw();
                ///cubeNew.CountVisible();
            }
            else
            {
                cubeNew.DrawVisible();
                //cubeNew.DrawVisible();
            }

            GL.Flush();
            renderCanvas1.SwapBuffers();
            
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

        private void button2_Click(object sender, EventArgs e)
        {
            pause = !pause;
            RePaint();
        }

        private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {
            RePaint();

        }
        private void renderCanvas_Load(object sender, EventArgs e)
        {
            // Specify the color for clearing
            GL.ClearColor(Color.SkyBlue);
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
    }
}
