using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Windows.Media.Media3D;

namespace CG_lab5
{
    public class Cube
    {
		public Point3D[] cubeVertex;
		public Color4[] color;
		public bool[] vertexVisibility = new bool[24];
		public bool[] ribsVisibility = new bool[24];


		public Cube()
        {
			cubeVertex = new Point3D[24];
			color = new Color4[8];

			//Upper facet
			cubeVertex[0] = new Point3D(1.0f, -1.0f, -1.0f);
			cubeVertex[1] = new Point3D(1.0f, -1.0f, 1.0f);
			cubeVertex[2] = new Point3D(1.0f, 1.0f, 1.0f);
			cubeVertex[3] = new Point3D(1.0f, 1.0f, -1.0f);

			//Down facet
			cubeVertex[4] = new Point3D(-1.0f, -1.0f, -1.0f);
			cubeVertex[5] = new Point3D(-1.0f, -1.0f, 1.0f);
			cubeVertex[6] = new Point3D(-1.0f, 1.0f, 1.0f);
			cubeVertex[7] = new Point3D(-1.0f, 1.0f, -1.0f);

			//Back facet
			cubeVertex[8] = new Point3D(-1.0f, -1.0f, -1.0f);
			cubeVertex[9] = new Point3D(1.0f, -1.0f, -1.0f);
			cubeVertex[10] = new Point3D(1.0f, -1.0f, 1.0f);
			cubeVertex[11] = new Point3D(-1.0f, -1.0f, 1.0f);

			//Left
			cubeVertex[12] = new Point3D(1.0f, 1.0f, -1.0f);
			cubeVertex[13] = new Point3D(1.0f, -1.0f, -1.0f);
			cubeVertex[14] = new Point3D(-1.0f, -1.0f, -1.0f);
			cubeVertex[15] = new Point3D(-1.0f, 1.0f, -1.0f);

			//Right
			cubeVertex[16] = new Point3D(-1.0f, -1.0f, 1.0f);
			cubeVertex[17] = new Point3D(-1.0f, 1.0f, 1.0f);
			cubeVertex[18] = new Point3D(1.0f, 1.0f, 1.0f);
			cubeVertex[19] = new Point3D(1.0f, -1.0f, 1.0f);

			//Front 
			cubeVertex[20] = new Point3D(1.0f, 1.0f, 1.0f);
			cubeVertex[21] = new Point3D(-1.0f, 1.0f, 1.0f);
			cubeVertex[22] = new Point3D(-1.0f, 1.0f, -1.0f);
			cubeVertex[23] = new Point3D(1.0f, 1.0f, -1.0f);

			initColors();
		}

		public Cube(Point3D[] cubeArray)
        {
			cubeVertex = cubeArray;
			initColors();
		}

		public Cube(Cube cube)
        {
			//cube.cubeVertex.CopyTo(cubeVertex, 0);
			cube.vertexVisibility.CopyTo(vertexVisibility, 0);
			cube.ribsVisibility.CopyTo(ribsVisibility, 0);
			initColors();
        }

		private void initColors()
		{
			color = new Color4[6];
			color[0] = Color4.Red;
			color[1] = Color4.DeepPink;
			color[2] = Color4.Blue;
			color[3] = Color4.Green;
			color[4] = Color4.Yellow;
			color[5] = Color4.Purple;
		}

		public void Draw()
        {
			GL.Begin(BeginMode.Quads);

			for (int i = 0; i < 6; i++)
            {
				GL.Color4(color[i]);
				for (int j = 0; j < 4; j++)
					GL.Vertex3(cubeVertex[4*i + j].X, cubeVertex[4 * i + j].Y, cubeVertex[4 * i + j].Z);
			}
			GL.End();
		}

		public void CountVisible(Point3D pointView)
		{
			double distance = 0;
			for (int i = 0; i < 24; ++i)
			{
				if (FindDistance(pointView, cubeVertex[i]) > distance)
				{
					distance = FindDistance(pointView, cubeVertex[i]);
				}
			}
			for (int i = 0, vertex = 0; i < 24; i += 4, ++vertex)
			{
				if (ApproxEqual(FindDistance(pointView, cubeVertex[i]), distance)
					|| ApproxEqual(FindDistance(pointView, cubeVertex[i + 1]), distance)
					|| ApproxEqual(FindDistance(pointView, cubeVertex[i + 2]), distance)
					|| ApproxEqual(FindDistance(pointView, cubeVertex[i + 3]), distance))
					vertexVisibility[vertex] = false;
				else
					vertexVisibility[vertex] = true;
			}
			for (int i = 0, ribs = 0; i < 24; i += 2, ++ribs)
			{
				if (ApproxEqual(FindDistance(pointView, cubeVertex[i]), distance) 
					|| ApproxEqual(FindDistance(pointView, cubeVertex[i + 1]), distance))
					ribsVisibility[ribs] = false;
				else
					ribsVisibility[ribs] = true;
			}

		}

		double FindDistance(Point3D from, Point3D to)
        {
			return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2) + Math.Pow(from.Z - to.Z, 2));
        }

		public void DrawVisible(bool showVisible = true)
        {
			GL.LineWidth(4);
			GL.Begin(BeginMode.Quads);

			for (int i = 0; i < 6; i++)
			{
				GL.Color4(color[i]);
				if (vertexVisibility[i] == showVisible)
					for (int j = 0; j < 4; j++)
						GL.Vertex3(cubeVertex[4 * i + j].X, cubeVertex[4 * i + j].Y, cubeVertex[4 * i + j].Z);
			}
			GL.End();

			GL.Color4(Color4.Black);
			for (int i = 0, ribs = 0; i < 24; i += 2, ribs++)
			{
				if (ribsVisibility[ribs] == showVisible)
				{
					GL.Begin(BeginMode.Lines);
					GL.Vertex3(cubeVertex[i].X - 0.01, cubeVertex[i].Y - 0.01, cubeVertex[i].Z - 0.01);
					GL.Vertex3(cubeVertex[i+1].X - 0.01, cubeVertex[i+1].Y - 0.01, cubeVertex[i+1].Z - 0.01);
					GL.Vertex3(cubeVertex[i].X + 0.01, cubeVertex[i].Y + 0.01, cubeVertex[i].Z + 0.01);
					GL.Vertex3(cubeVertex[i+1].X + 0.01, cubeVertex[i+1].Y + 0.01, cubeVertex[i+1].Z + 0.01);
					GL.End();
				}
			}
			GL.LineWidth(1);
		}
		bool ApproxEqual(double double1, double double2)
		{
			double difference = Math.Abs(double1 * .00001);
			if (Math.Abs(double1 - double2) <= difference)
				return true;
			else return false;
		}
	}
}
