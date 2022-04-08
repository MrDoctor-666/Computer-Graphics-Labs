using System.Windows.Media.Media3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace CG_lab3
{
    public class BezierPatchCustom
    {
        public Point3D[,] points = new Point3D[4, 4];
        Point3D[] prev;
        int divs = 20;
        public BezierCurvesCustom[,] curves = new BezierCurvesCustom[2, 4];

        public BezierPatchCustom()
        {
            initPoints();
        }

        public BezierPatchCustom(Point3D[,] point3Ds)
        {
            points = point3Ds;
        }

        void setUp()
        {
            for (int i = 0; i < 4; i++)
            {
                Point3D[] toDraw, toDraw2;
                toDraw = new Point3D[4] { points[i, 0], points[i, 1], points[i, 2], points[i, 3] };
                toDraw2 = new Point3D[4] { points[0, i], points[1, i], points[2, i], points[3, i] };
                curves[0, i] = new BezierCurvesCustom(toDraw);
                curves[1, i] = new BezierCurvesCustom(toDraw2);
            }
        }

        void initPoints()
        {
            points[0,0] = new Point3D(-0.75, -0.75, -0.50); 
            points[0,1] = new Point3D(-0.25, -0.75, 0.00);
            points[0,2] = new Point3D(0.25, -0.75, 0.00);
            points[0,3] = new Point3D(0.75, -0.75, -0.50);
            points[1,0] = new Point3D(-0.75, -0.25, -0.75);
            points[1,1] = new Point3D(-0.25, -0.25, 0.50);
            points[1,2] = new Point3D(0.25, -0.25, 0.50);
            points[1,3] = new Point3D(0.75, -0.25, -0.75);
            points[2,0] = new Point3D(-0.75, 0.25, 0.00);
            points[2,1] = new Point3D(-0.25, 0.25, -0.50);
            points[2,2] = new Point3D(0.25, 0.25, -0.50);
            points[2,3] = new Point3D(0.75, 0.25, 0.00);
            points[3,0] = new Point3D(-0.75, 0.75, -0.50);
            points[3,1] = new Point3D(-0.25, 0.75, -1.00);
            points[3,2] = new Point3D(0.25, 0.75, -1.00);
            points[3,3] = new Point3D(0.75, 0.75, -0.50);
        }

        public void Draw()
        {
            setUp();
            float px, py;
            Point3D[] temp = new Point3D[4] {points[0,0], points[1,0],points[2,0], points[3,0] };
            Point3D[] toDraw = new Point3D[4] { points[0, 0], points[0, 1], points[0, 2], points[0, 3] };
            BezierCurvesCustom curve = new BezierCurvesCustom(temp);

            prev = curve.PointsToDraw();
            GL.Begin(BeginMode.TriangleStrip);

            for (int u = 1; u <= divs; u++)
            {
                py = ((float)u) / ((float)divs);         

                curve = curves[0, 0];
                temp[0] = curve.PointOnCurve4(py);

                curve = curves[0, 1];
                temp[1] = curve.PointOnCurve4(py);

                curve = curves[0, 2];
                temp[2] = curve.PointOnCurve4(py);

                curve = curves[0, 3];
                temp[3] = curve.PointOnCurve4(py);


                GL.Begin(BeginMode.TriangleStrip);
                for (int v = 0; v <= divs; v++)
                {
                    GL.Color4(Color4.Red);
                    px = ((float)v) / ((float)divs);   
                    GL.Vertex3(prev[v].X, prev[v].Y, prev[v].Z); 

                    GL.Color4(Color4.Yellow);
                    curve = new BezierCurvesCustom(temp);
                    prev[v] = curve.PointOnCurve4(px);  
                    GL.Vertex3(prev[v].X, prev[v].Y, prev[v].Z);  

                }
                GL.End();
            }
        }
    }
}
