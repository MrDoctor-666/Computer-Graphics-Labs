using System;
using OpenTK;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Windows.Media.Media3D;

namespace CG_lab6
{
    public partial class Form1 : Form
    {
        GLControl renderCanvas1;
        Rotation rot = new Rotation();
        int winX, winY, winW, winH;
        int angleX = 0, angleY = 0, angleZ = 0;
        Point3D[] curNormals = new Point3D[6];
        Cube cube = new Cube();
        Cube cubeNew;
        bool pause = false;
        bool isLight = true;
        bool isColor = false;
        Vector3 viewPoint = new Vector3(3, 3, 5);

        float[] lightAmbient = new float[] { 0.1f, 0.1f, 0.1f, 1f };
        float[] lightPosition = new float[] { 3.0f, 3.0f, 5.0f, 1.0f };
        float[] lightDiffuse = new float[] { 1f, 1f, 1f, 1.0f };
        float[] lightSpecular;
        float[] lightDirection;

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
                    if (control.Name.Contains("0"))
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
                    else
                    {
                        char num = control.Name[control.Name.Length - 1];
                        switch (num)
                        {
                            case '1':
                                ((NumericUpDown)control).Value = (decimal)lightPosition[0];
                                break;
                            case '2':
                                ((NumericUpDown)control).Value = (decimal)lightPosition[1];
                                break;
                            case '3':
                                ((NumericUpDown)control).Value = (decimal)lightPosition[2];
                                break;
                        }
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
                    if (control.Name.Contains("0"))
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
                    else
                    {
                        char num = control.Name[control.Name.Length - 1];
                        switch (num)
                        {
                            case '1':
                                lightPosition[0] = (float)((NumericUpDown)control).Value;
                                break;
                            case '2':
                                lightPosition[1] = (float)((NumericUpDown)control).Value;
                                break;
                            case '3':
                                lightPosition[2] = (float)((NumericUpDown)control).Value;
                                break;
                        }
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

        protected void SetupLight()
        {
            if (isLight == true)
            {
                
                lightSpecular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };
                lightDirection = new float[] {-11f, -11f, -11f, 0.0f };
            }

            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.Lighting);
            GL.LightModel(LightModelParameter.LightModelAmbient, lightAmbient);
            GL.Light(LightName.Light0, LightParameter.Position, lightPosition);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse);
            GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, lightDirection);
            GL.Light(LightName.Light0, LightParameter.Specular, lightSpecular);

            GL.Enable(EnableCap.Light0);
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
            if (isLight == true) SetupLight();
            else GL.Disable(EnableCap.Lighting);
            if (isColor) GL.Enable(EnableCap.ColorMaterial);
            else GL.Disable(EnableCap.ColorMaterial);

            //Drawing

            Point3D[] pointsNew = new Point3D[24];
            Point3D[] normalsNew = new Point3D[6];
            for (int i = 0; i < 24; i++)
            {
                pointsNew[i] = rot.RotateArounAxis(cube.cubeVertex[i], Axes.X, angleX);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Y, angleY);
                pointsNew[i] = rot.RotateArounAxis(pointsNew[i], Axes.Z, angleZ);
            }

            for (int i = 0; i < 6; i++)
            {
                normalsNew[i] = rot.RotateArounAxis(cube.normals[i], Axes.X, angleX);
                normalsNew[i] = rot.RotateArounAxis(normalsNew[i], Axes.Y, angleY);
                normalsNew[i] = rot.RotateArounAxis(normalsNew[i], Axes.Z, angleZ);

            }

            cubeNew.cubeVertex = pointsNew;
            cubeNew.normals = normalsNew;
            drawRefLines();
            if (!pause)
            {
                cubeNew = new Cube(cube);
                cubeNew.cubeVertex = pointsNew;
                cubeNew.normals = normalsNew;
                cubeNew.Draw();
                curNormals = normalsNew;
            }
            else
            {
                cubeNew.normals = curNormals;
                cubeNew.Draw();
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

        private void button3_Click(object sender, EventArgs e)
        {
            isLight = !isLight;
            RePaint();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            isColor = !isColor;
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
