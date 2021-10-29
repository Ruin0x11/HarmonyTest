using System;
using System.Collections.Generic;

namespace OpenNefia.Core.Util
{
    public static class PosUtils
    {
        public static double Dist(int x1, int y1, int x2, int y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Direction GetDirectionTowards(int x, int y, int newX, int newY)
        {
            var dx = newX - x;
            var dy = newY - y;

            if (dx > 0)
            {
                if (dy > 0)
                {
                    return Direction.SouthEast;
                }
                else if (dy < 0)
                {
                    return Direction.NorthEast;
                }
                else
                {
                    return Direction.East;
                }
            }
            else if (dx < 0)
            {
                if (dy > 0)
                {
                    return Direction.SouthWest;
                }
                else if (dy < 0)
                {
                    return Direction.NorthWest;
                }
                else
                {
                    return Direction.West;
                }
            }
            else
            {
                if (dy > 0)
                {
                    return Direction.South;
                }
                else if (dy < 0)
                {
                    return Direction.North;
                }
                else
                {
                    return Direction.Center;
                }
            }
        }

        /// <summary>
        /// From http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static IEnumerable<Point2i> EnumerateLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Point2i((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }
    }
}
