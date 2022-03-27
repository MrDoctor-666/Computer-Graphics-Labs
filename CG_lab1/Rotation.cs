using System;
using OpenTK;

namespace CG_lab1_try2
{

    class Rotation
    {
        public Vector2[] figure;
        public Vector2[] newFigure = null;

        public Rotation()
        {
            figure = new Vector2[4];
            figure[0].X = 0.1f;
            figure[0].Y = 0.1f;
            figure[1].X = 0.1f;
            figure[1].Y = 0.5f;
            figure[2].X = 0.5f;
            figure[2].Y = 0.5f;
            figure[3].X = 0.5f;
            figure[3].Y = 0.1f;


        }

        public Vector2[] Rotate(Vector2 rotateAroundPoint, int angle)
        {
            newFigure = new Vector2[4];
            double radAngle = angle * Math.PI / 180;

            double[,] first = new double[3, 3]
            {
                {1, 0, 0 },
                {0, 1, 0 },
                {-rotateAroundPoint.X, -rotateAroundPoint.Y, 1 }
            };
            double[,] second = new double[3, 3]
            {
                {Math.Cos(radAngle), Math.Sin(radAngle), 0 },
                {-Math.Sin(radAngle), Math.Cos(radAngle), 0 },
                {0, 0, 1 }
            };
            double[,] third = new double[3, 3]
            {
                {1, 0, 0 },
                {0, 1, 0 },
                {rotateAroundPoint.X, rotateAroundPoint.Y, 1 }
            };

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Point " + i);
                double[] point = new double[3] { figure[i].X, figure[i].Y, 1 };
                point = Multiply(point, first);
                point = Multiply(point, second);
                point = Multiply(point, third);
                newFigure[i].X = (float)point[0];
                newFigure[i].Y = (float)point[1];
            }

            return newFigure;
        }

        private double[] Multiply(double[] one, double[,] two)
        {
            double[] answer = new double[3];
            for(int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    answer[i] += one[j] * two[j, i];
                }
                Console.WriteLine(answer[i]);
            }

            Console.WriteLine();

            return answer;
        }
    }
}
