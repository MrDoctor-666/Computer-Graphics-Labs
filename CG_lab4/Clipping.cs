using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CG_lab4
{
    public static class Clipping
    {

        static List<Point3D> pCut;
        static List<Point3D> pToCut;
        static List<Point3D> intersections;

        public static List<Polygon2D> Clip(Polygon2D cutter, Polygon2D toCut)
        {
            pCut = new List<Point3D>(cutter.vertices); 
            pToCut = new List<Point3D>(toCut.vertices);
            intersections = new List<Point3D>();
            Point3D? point; int i1 = 0, i2 = 1;

            //find all intersections and add to lists
            do
            {
                for (int j = 1; j < toCut.vertices.Count; j++)
                {
                    point = SegmentCross(cutter.vertices[i1], cutter.vertices[i2], toCut.vertices[j - 1], toCut.vertices[j]);
                    if (point.HasValue)
                        //todo change all to approxEqual
                        if (!(point.Value.Equals(cutter.vertices[i1]) ||
                            point.Value.Equals(cutter.vertices[i2]) ||
                            point.Value.Equals(toCut.vertices[j - 1]) ||
                            ApproxEqualPoints(toCut.vertices[j-1], point.Value) ||
                            point.Value.Equals(toCut.vertices[j]))
                            )
                            AddTo(point.Value, pCut.IndexOf(cutter.vertices[i1]), pToCut.IndexOf(toCut.vertices[j - 1]));
                }
                point = SegmentCross(cutter.vertices[i1], cutter.vertices[i2], toCut.vertices[toCut.vertices.Count - 1], toCut.vertices[0]);
                if (point.HasValue)
                    //todo change all to approxEqual
                    if (!(point.Value.Equals(cutter.vertices[i1]) ||
                            point.Value.Equals(cutter.vertices[i2]) ||
                            ApproxEqualPoints(point.Value, toCut.vertices[toCut.vertices.Count - 1]) ||
                            point.Value.Equals(toCut.vertices[0]))
                            )
                        AddTo(point.Value, pCut.IndexOf(cutter.vertices[i1]), pToCut.Count - 1);

                i1++; i2++;
                if (i2 == cutter.vertices.Count) i2 = 0;
            }
            while (i2 != 1);

            CheckCorrectOrder(pToCut);
            CheckCorrectOrder(pCut);

            List<Point3D>[] arrClip = FindClipping();
            List<Polygon2D> polygons = new List<Polygon2D>();
            foreach (List<Point3D> point3Ds in arrClip)
                if (point3Ds!= null && point3Ds.Count != 0) polygons.Add(new Polygon2D(point3Ds));

            return polygons;
        }

        static List<Point3D>[] FindClipping()
        {
            List<Point3D>[] arrList = new List<Point3D>[10]; int arrLC = 0;
            Point3D? question = FirstInterception();
            Point3D startPoint;
            if (question.HasValue) startPoint = question.Value;
            else return new List<Point3D>[1] { new List<Point3D>() }; 
            Point3D curPoint;
            bool position;
            int i, j;
            while (arrLC < 10 && intersections.Count > 0)
            {
                List<Point3D> result = new List<Point3D>();
                result.Add(startPoint);
                intersections.Remove(startPoint);
                position = true;
                i = pToCut.IndexOf(startPoint) + 1;
                j = 0;
                while (true)
                {
                    if (i >= pToCut.Count) i = 0;
                    if (j >= pCut.Count) j = 0;
                    if (position) curPoint = pToCut[i];
                    else curPoint = pCut[j];

                    if (intersections.Contains(curPoint))
                    {
                        intersections.Remove(curPoint);
                        position = !position;
                        i = pToCut.IndexOf(curPoint);
                        j = pCut.IndexOf(curPoint);
                    }


                    if (curPoint.Equals(startPoint)) break;
                    if (ApproxEqual(curPoint.X, startPoint.X) && ApproxEqual(curPoint.Y, startPoint.Y)) break;
                    result.Add(curPoint);
                    i++; j++;
                }
                arrList[arrLC] = result;
                arrLC++;
                Print(result);
                if (FirstInterception().HasValue)
                    startPoint = FirstInterception().Value;
                else break;
            }
            return arrList;
        }

        static Point3D? FirstInterception()
        {
            foreach (Point3D p in pToCut)
                if (intersections.Contains(p)) return p;

            return null;
        }

        static bool IsPointFirst(Point3D p1, Point3D p2, Point3D line1, Point3D line2)
        {
            if (line1.X != line2.X)
            {
                if (Math.Abs(line1.X - p1.X) < Math.Abs(line1.X - p2.X)) return true;
                else return false;
            }
            else if (Math.Abs(line1.Y - p1.Y) < Math.Abs(line1.Y - p2.Y)) return true;
            else return false;
        }

        static void AddTo(Point3D point, int i, int j)
        {
            if (Contains(intersections, point)) return;
            intersections.Add(point);
            if (!pCut.Contains(point)) pCut.Insert(i + 1, point);
            if (!pToCut.Contains(point)) pToCut.Insert(j + 1, point);
        }

        static bool Contains(List<Point3D> points, Point3D p )
        {
            foreach (Point3D point in points)
                if (ApproxEqual(p.X, point.X) && ApproxEqual(p.Y, point.Y)) return true;
            return false;
        }

        static bool ApproxEqualPoints(Point3D point1, Point3D point2)
        {
            if (ApproxEqual(point1.X, point2.X) && ApproxEqual(point1.Y, point2.Y)) return true;
            else return false;
        }

        static bool ApproxEqual(double double1, double double2)
        {
            double difference = Math.Abs(double1 * .00001);
            if (Math.Abs(double1 - double2) <= difference)
                return true;
            else return false;
        }

        static Point3D? SegmentCross(Point3D p1In, Point3D p2In, Point3D p3In, Point3D p4In)
        {
            Point3D p1 = p1In, p2 = p2In, p3 = p3In, p4 = p4In;
            double Xa, A1, A2, b1, b2, Ya;
            if (p2.X < p1.X) {
                Point3D tmp = p1; p1 = p2; p2 = tmp;
            }

            if (p4.X < p3.X) {
                Point3D tmp = p3; p3 = p4; p4 = tmp;
            }
            
            if (p2.X < p3.X) return null;
            if (p1.Equals(p3) || p1.Equals(p4) || p2.Equals(p3) || p2.Equals(p4)) return null;

            //if first segment is vertical
            if (p1.X - p2.X == 0)
            {
                Xa = p1.X;
                A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                b2 = p3.Y - A2 * p3.X;
                Ya = A2 * Xa + b2;

                if (p3.X <= Xa && p4.X >= Xa && Math.Min(p1.Y, p2.Y) <= Ya &&
                        Math.Max(p1.Y, p2.Y) >= Ya)
                    return new Point3D(Xa, Ya, 0);

                return null;
            }

            //if second segment is vertical
            if (p3.X - p4.X == 0)
            {
                Xa = p3.X;
                A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                b1 = p1.Y - A1 * p1.X;
                Ya = A1 * Xa + b1;

                if (p1.X <= Xa && p2.X >= Xa && Math.Min(p3.Y, p4.Y) <= Ya &&
                        Math.Max(p3.Y, p4.Y) >= Ya)
                    return new Point3D(Xa, Ya, 0);

                return null;
            }

            A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
            A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
            b1 = p1.Y - A1 * p1.X;
            b2 = p3.Y - A2 * p3.X;

            if (A1 == A2) return null; //parallel

            Xa = (b2 - b1) / (A1 - A2);
            Ya = A1 * Xa + b1;

            if ((Xa < Math.Max(p1.X, p3.X)) || (Xa > Math.Min(p2.X, p4.X))) return null; 
            else return new Point3D(Xa, Ya, 0);
        }

        static void SortPart(List<Point3D> list)
        {
            if (list.Count < 4) return;
            int start = 0, end = list.Count - 1;
            bool flag;
            while (true)
            {
                flag = true;
                for (int i = 1; i < list.Count - 2; i++)
                {
                    if (!IsPointFirst(list[i], list[i + 1], list[start], list[end]))
                    {
                        Point3D temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                        flag = false;
                    }
                }
                if (flag) return;
            }
        }

        static void CheckCorrectOrder(List<Point3D> points)
        {
            List<Point3D> toSort;

            for (int i = 1; i < points.Count; i++)
            {
                if (intersections.Contains(points[i]))
                {
                    int j = i;
                    int endLine = j + 2;
                    toSort = new List<Point3D>();
                    if (j + 1 < points.Count && intersections.Contains(points[j + 1]))
                    {
                        toSort.Add(points[j - 1]);
                        toSort.Add(points[j]);
                        toSort.Add(points[j + 1]);
                        while (true)
                        {
                            if (endLine >= points.Count) endLine = 0;
                            toSort.Add(points[endLine]);
                            if (!intersections.Contains(points[endLine])) break;
                            endLine++;
                        }
                        SortPart(toSort);
                        points[j] = toSort[1];
                        points[j + 1] = toSort[2];
                        endLine = j + 2;
                        int l = 3;
                        while (l < toSort.Count)
                        {
                            if (endLine >= points.Count) endLine = 0;
                            points[endLine] = toSort[l]; l++;
                            if (!intersections.Contains(points[endLine])) break;
                            endLine++;
                        }
                    }
                }
            }
        }

        static void Print(List<Point3D> point3Ds)
        {
            foreach (Point3D point in point3Ds)
                Console.WriteLine(point.ToString());
            Console.WriteLine("\n");
        }
    }
}
