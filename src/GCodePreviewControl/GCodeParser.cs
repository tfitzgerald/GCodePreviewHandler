using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;

namespace GCodePreviewControl
{
    public static class GCodeParser
    {
        public struct Segment { public Point3D Start; public Point3D End; }

        public static IEnumerable<Segment> Parse(string path)
        {
            double x = 0, y = 0, z = 0;
            double lastX = 0, lastY = 0, lastZ = 0;

            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(' ');
                foreach (var p in parts)
                {
                    if (p.StartsWith("X")) x = double.Parse(p.Substring(1));
                    if (p.StartsWith("Y")) y = double.Parse(p.Substring(1));
                    if (p.StartsWith("Z")) z = double.Parse(p.Substring(1));
                }

                yield return new Segment
                {
                    Start = new Point3D(lastX, lastY, lastZ),
                    End = new Point3D(x, y, z)
                };

                lastX = x; lastY = y; lastZ = z;
            }
        }
    }
}
