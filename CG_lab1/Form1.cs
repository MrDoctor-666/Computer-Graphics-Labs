using System;
using OpenTK;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace CG_lab1_try2
{
    public partial class Form1 : Form
    {
        GLControl renderCanvas1;
        int winX, winY, winW, winH;
        Rotation rotation;
        int angle = 0;
        Vector2 rotPoint = new Vector2(0, 0);
        public Form1()
        {
            InitializeComponent();
            rotation = new Rotation();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            angle = trackBar1.Value;
            textBox3.Text = angle.ToString();
            RePaint();
            Console.WriteLine(trackBar1.Value.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.renderCanvas1 = new GLControl();
            //this.SuspendLayout();

            winX = this.Location.X; winY = this.Location.Y;
            winW = this.Width; winH = this.Height;

            this.renderCanvas1.BackColor = System.Drawing.Color.CadetBlue;
            this.renderCanvas1.Location =
                new System.Drawing.Point(winX, winY);
            this.renderCanvas1.Name = "renderCanvas1";
            this.renderCanvas1.Size = new System.Drawing.Size((int)(5* winH / 6), 5*winH/6);
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
            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox3.Text = angle.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                float x;
                if (float.TryParse(textBox1.Text, out x))
                    rotPoint.X = x;
            }
            if (textBox2.Text != null)
            {
                float y;
                if (float.TryParse(textBox2.Text, out y))
                    rotPoint.Y = y;
            }
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
                    angle = value;
                    trackBar1.Value = angle;
                }
                else
                    textBox3.Text = angle.ToString();
            }
            RePaint();
                
        }

        private void renderCanvas_Paint(object sender, PaintEventArgs e)
        {
            //GL.Viewport(winX - 25, winY - 25, 5*Width/6, 5*Height/6);
            RePaint();
           
        }

        private void RePaint()
        {
            // Clear the render canvas with the current color
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Black);
            GL.Vertex2(0, Height / 2);
            GL.Vertex2(0, -Height / 2);
            GL.Vertex2(-Width / 2, 0);
            GL.Vertex2(Width / 2, 0);
            GL.End();
            Console.WriteLine(Height);

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            for (int i = 0; i < 3; i++)
            {
                GL.Vertex2(rotation.figure[i]);
                GL.Vertex2(rotation.figure[i + 1]);
            }
            GL.Vertex2(rotation.figure[3]);
            GL.Vertex2(rotation.figure[0]);
            GL.End();

            rotation.Rotate(rotPoint, angle);

            if (rotation.newFigure != null)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(Color.Yellow);
                for (int i = 0; i < 3; i++)
                {
                    GL.Vertex2(rotation.newFigure[i]);
                    GL.Vertex2(rotation.newFigure[i + 1]);
                }
                GL.Vertex2(rotation.newFigure[3]);
                GL.Vertex2(rotation.newFigure[0]);
                GL.End();
            }

            GL.Enable(EnableCap.PointSmooth);
            GL.PointSize(10f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(Color.White);
            GL.Vertex2(rotPoint);
            GL.End();

            GL.Flush();
            renderCanvas1.SwapBuffers();
        }

        private void renderCanvas_Load(object sender, EventArgs e)
        {
            // Specify the color for clearing
            GL.ClearColor(Color.SkyBlue);
        }
    }
}
