using System;
using OpenTK;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Windows.Media.Media3D;

namespace CG_lab4
{
    public partial class Form1 : Form
    {
        GLControl renderCanvas1;
        int winX, winY, winW, winH;
        bool drawCutting = false;
        int toCutVetexNum = 5, cutterVertexNum = 4;
        Point3D[] point3Ds = new Point3D[5]
        {
            new Point3D(-0.5,-0.5,0),
            new Point3D(-0.25,1,0),
            new Point3D(0.1,0.5,0),
            new Point3D(0.5,0.7,0),
            new Point3D(0.8,-0.7,0)
        };


        Point3D[] cutterArray = new Point3D[4]
        {
            new Point3D(0, 0, 0),
            new Point3D(0, 1, 0),
            new Point3D(1, 1,0),
            new Point3D(1, 0 ,0)

        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            glControl_Resize(renderCanvas1, EventArgs.Empty);
        }
        private void glControl_Resize(object sender, EventArgs e)
        {
            renderCanvas1.MakeCurrent();

            if (renderCanvas1.ClientSize.Height == 0)
                renderCanvas1.ClientSize = new System.Drawing.Size(renderCanvas1.ClientSize.Width, 1);

            GL.Viewport(0, 0, renderCanvas1.ClientSize.Width, renderCanvas1.ClientSize.Height);

            float aspect_ratio = Math.Max(renderCanvas1.ClientSize.Width, 1) / (float)Math.Max(renderCanvas1.ClientSize.Height, 1);
            Matrix4 perpective = Matrix4.CreateOrthographicOffCenter(-2f, 2f, -2f, 2f, 1.0f, 10f);
            //Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
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
                    int num; string name = control.Name;
                    char letter = control.Name[control.Name.Length - 1];
                    name = control.Name.Remove(control.Name.Length - 1);
                    Int32.TryParse(name.Remove(0, 13), out num);
                    Console.WriteLine(control.Name + " " + num + num % 8);
                    if (num < toCutVetexNum)
                        switch (letter)
                        {
                            case 'x':
                                ((NumericUpDown)control).Visible = true;
                                ((NumericUpDown)control).Value = (decimal)point3Ds[num % 8].X;
                                break;
                            case 'y':
                                ((NumericUpDown)control).Visible = true;
                                ((NumericUpDown)control).Value = (decimal)point3Ds[num % 8].Y;
                                break;
                        }
                    else if (num > 7 && num % 8 < cutterVertexNum)
                        switch (letter)
                        {
                            case 'x':
                                ((NumericUpDown)control).Visible = true;
                                ((NumericUpDown)control).Value = (decimal)cutterArray[num % 8].X;
                                break;
                            case 'y':
                                ((NumericUpDown)control).Visible = true;
                                ((NumericUpDown)control).Value = (decimal)cutterArray[num % 8].Y;
                                break;
                        }
                    else ((NumericUpDown)control).Visible = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    int num; string name = control.Name;
                    char letter = control.Name[control.Name.Length - 1];
                    name = control.Name.Remove(control.Name.Length - 1);
                    Int32.TryParse(name.Remove(0, 13), out num);
                    Console.WriteLine(control.Name + " " + num);
                    if (num < toCutVetexNum)
                        switch (letter)
                        {
                            case 'x':
                                point3Ds[num % 8].X = (double)((NumericUpDown)control).Value;
                                break;
                            case 'y':
                                point3Ds[num % 8].Y = (double)((NumericUpDown)control).Value;
                                break;
                        }
                    else if (num > 7 && num % 8 < cutterVertexNum)
                        switch (letter)
                        {
                            case 'x':
                                cutterArray[num % 8].X = (double)((NumericUpDown)control).Value;
                                break;
                            case 'y':
                                cutterArray[num % 8].Y = (double)((NumericUpDown)control).Value;
                                break;
                        }
                }
            }
            RePaint();
        }

        protected void SetupCamera()
        {
            Matrix4 lookat = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
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

            drawRefLines();

            Polygon2D pol = new Polygon2D(new System.Collections.Generic.List<Point3D>(cutterArray));
            pol.DrawPolygon(Color4.Purple, 1);

            Polygon2D toCut = new Polygon2D(new System.Collections.Generic.List<Point3D>(point3Ds));
            toCut.DrawPolygon(Color4.Red);

            if (drawCutting)
            {
                System.Collections.Generic.List<Polygon2D> l = Clipping.Clip(pol, toCut);
                foreach (Polygon2D polygon in l)
                    polygon.DrawPolygon(Color4.Yellow, 2);
            }

            GL.Flush();
            renderCanvas1.SwapBuffers();
            
        }

        void drawRefLines()
        {
            //void glColor3d(GLdouble red, GLdouble green, GLdouble blue); 
            GL.Color4(Color4.Black);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(+1000.0, 0.0, -1.0);
            GL.Vertex3(-1000.0, 0.0, -1.0);
            GL.End();
            //GL.Color3(0.0, 1.0, 0.0);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0.0, +1000.0, -1.0);
            GL.Vertex3(0.0, -1000.0, -1.0);
            GL.End();
            //GL.Color3(0.0, 0.0, 1.0);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(0.0, 0.0, +1000.0);
            GL.Vertex3(0.0, 0.0, -1000.0);
            GL.End();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (toCutVetexNum > 3)
            {
                toCutVetexNum--;
                Point3D[] newPoints = new Point3D[toCutVetexNum];
                for (int i = 0; i < toCutVetexNum; i++)
                    newPoints[i] = point3Ds[i];
                point3Ds = newPoints;
                SetInitValues();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (cutterVertexNum > 3)
            {
                cutterVertexNum--;
                Point3D[] newPoints = new Point3D[cutterVertexNum];
                for (int i = 0; i < cutterVertexNum; i++)
                    newPoints[i] = cutterArray[i];
                cutterArray = newPoints;
                SetInitValues();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (toCutVetexNum < 8)
            {
                toCutVetexNum++;
                Point3D[] newPoints = new Point3D[toCutVetexNum];
                for (int i = 0; i < toCutVetexNum - 1; i++)
                    newPoints[i] = point3Ds[i];
                newPoints[toCutVetexNum - 1] = new Point3D(0, 0, 0);
                point3Ds = newPoints;
                SetInitValues();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cutterVertexNum < 8)
            {
                cutterVertexNum++;
                Point3D[] newPoints = new Point3D[cutterVertexNum];
                for (int i = 0; i < cutterVertexNum - 1; i++)
                    newPoints[i] = cutterArray[i];
                newPoints[cutterVertexNum - 1] = new Point3D(0, 0, 0);
                cutterArray = newPoints;
                SetInitValues();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            drawCutting = !drawCutting;
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
