using System.Collections.Generic;
using System.Windows.Media.Media3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace CG_lab4
{
    public class Polygon2D
    {
        public List<Point3D> vertices;


        public Polygon2D(List<Point3D> vertices)
        {
            this.vertices = vertices; 
        }

        public Polygon2D()
        {
            vertices = new List<Point3D>();
        }

        public void MakeRectangle()
        {
            vertices.Add(new Point3D(0, 0, 0));
            vertices.Add(new Point3D(0, 1, 0));
            vertices.Add(new Point3D(1, 1, 0));
            vertices.Add(new Point3D(1, 0, 0));
        }

        public void DrawPolygonFill(Color4 color, int order = 0)
        {
            GL.Begin(PrimitiveType.Polygon);
            GL.Color4(color);

            for (int i = 0; i < vertices.Count; i++)
            {
                GL.Vertex3(vertices[i].X, vertices[i].Y, vertices[i].Z + order);
            }

            GL.End();
        }

        public void DrawPolygon(Color4 color, int order = 0)
        {
            if (vertices.Count == 0) return;

            GL.Begin(BeginMode.Lines);
            GL.Color4(color);
            for (int i = 1; i < vertices.Count; i++)
            {
                GL.Vertex3(vertices[i - 1].X, vertices[i - 1].Y, vertices[i - 1].Z + order);
                GL.Vertex3(vertices[i].X, vertices[i].Y, vertices[i].Z + order);
            }

            GL.Vertex3(vertices[vertices.Count - 1].X, vertices[vertices.Count - 1].Y, vertices[vertices.Count - 1].Z + order);
            GL.Vertex3(vertices[0].X, vertices[0].Y, vertices[0].Z + order);

            GL.End();
        }
    }
}
