using System;
using System.Windows.Media.Media3D;

namespace CG_lab3
{
    public class BezierCurvesCustom
    {
        public Point3D[] initialPoints;
        Point3D[] toDrawPoints;
        float MaxStepDraw = 100f;

        public BezierCurvesCustom(Point3D[] curvePoints)
        {
            toDrawPoints = new Point3D[curvePoints.Length];
            initialPoints = curvePoints;

            initialPoints.CopyTo(toDrawPoints, 0);

            //2-3
            if (curvePoints.Length > 4) toDrawPoints[3] = FindLineMiddle(toDrawPoints[2], toDrawPoints[3]);
            //5-6
            if (curvePoints.Length > 7) toDrawPoints[6] = FindLineMiddle(toDrawPoints[5], toDrawPoints[6]);

        }

        public Point3D[] PointsToDraw()
        {
            Point3D[] points = new Point3D[(int)(MaxStepDraw + 1)*(toDrawPoints.Length /3)];
            int j = 0;

            for (int i = 0; i < toDrawPoints.Length - 1; i += 3)
            {
                for (float t = 0; t <= 1; t += 1.0f / MaxStepDraw)
                {
                    points[j] = PointOnCurve(t, toDrawPoints[i], toDrawPoints[i+1], toDrawPoints[i+2], toDrawPoints[i+3]);
                    j++;
                }
            }

            return points;
        }

        Point3D PointOnCurve(double t, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            double var1 = 1 - t;
            Point3D vPoint = new Point3D(0, 0, 0);

            // B(t) = P1 * ( 1 — t )^3 + P2 * 3 * t * ( 1 — t )^2 + P3 * 3 * t^2 * ( 1 — t ) + P4 * t^3
            
            vPoint.X = Math.Pow(var1, 3) * p1.X + 3 * t * Math.Pow(var1, 2) * p2.X + 3 * Math.Pow(t, 2) * var1 * p3.X + Math.Pow(t, 3) * p4.X;
            vPoint.Y = Math.Pow(var1, 3) * p1.Y + 3 * t * Math.Pow(var1, 2) * p2.Y + 3 * Math.Pow(t, 2) * var1 * p3.Y + Math.Pow(t, 3) * p4.Y;
            vPoint.Z = Math.Pow(var1, 3) * p1.Z + 3 * t * Math.Pow(var1, 2) * p2.Z + 3 * Math.Pow(t, 2) * var1 * p3.Z + Math.Pow(t, 3) * p4.Z;

            return (vPoint);
        }

        public Point3D PointOnCurve4(double t)
        {
            return PointOnCurve(t, toDrawPoints[0], toDrawPoints[1], toDrawPoints[2], toDrawPoints[3]);
        }

        Point3D FindLineMiddle(Point3D start, Point3D end) => new Point3D((start.X + end.X) / 2, (start.Y + end.Y) / 2, (start.Z + end.Z) / 2);
    }
}
