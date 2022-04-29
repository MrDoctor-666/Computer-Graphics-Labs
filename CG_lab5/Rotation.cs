using System;
using System.Windows.Media.Media3D;

namespace CG_lab5
{
    public enum Axes
    {
        X, Y, Z
    }

    public class Rotation
    {

        public Point3D RotateArounAxis(Point3D point, Axes axis, int angleDegrees)
        {
            double radAngle = angleDegrees * Math.PI / 180;

            double[] coordsToChange = new double[2];
            switch (axis)
            {
                case Axes.X:
                    coordsToChange[0] = point.Y;
                    coordsToChange[1] = point.Z;
                    break;
                case Axes.Y:
                    coordsToChange[0] = point.X;
                    coordsToChange[1] = point.Z;
                    break;
                case Axes.Z:
                    coordsToChange[0] = point.X;
                    coordsToChange[1] = point.Y;
                    break;
            }

            double[,] muliplMatrix = new double[2, 2]
            {
                {Math.Cos(radAngle), Math.Sin(radAngle)},
                {-Math.Sin(radAngle), Math.Cos(radAngle) }
            };

            coordsToChange = Multiply(coordsToChange, muliplMatrix);

            switch (axis)
            {
                case Axes.X:
                    point.Y = coordsToChange[0];
                    point.Z = coordsToChange[1];
                    break;
                case Axes.Y:
                    point.X = coordsToChange[0];
                    point.Z = coordsToChange[1];
                    break;
                case Axes.Z:
                    point.X = coordsToChange[0];
                    point.Y = coordsToChange[1];
                    break;
            }

            return point;
        }

        private double[] Multiply(double[] first, double[,] second)
        {
            double[] answer = new double[2] { 0, 0 };

            for (int i = 0; i < answer.Length; i++)
            {
                for (int j = 0; j < first.Length; j++)
                {
                    answer[i] += first[j] * second[j, i];
                }
            }

            return answer;
        }
    }
}
