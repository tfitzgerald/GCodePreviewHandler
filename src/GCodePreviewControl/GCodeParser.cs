using System;
using System.Collections.Generic;
using System.Globalization;
using SharpDX;

namespace GCodePreviewControl
{
    public enum MoveType { Rapid, Cut }

    public struct GCodeMove
    {
        public Vector3 From;
        public Vector3 To;
        public MoveType Type;
    }

    public static class GCodeParser
    {
        public static List<GCodeMove> Parse(string gcode)
        {
            var moves = new List<GCodeMove>();
            float x = 0, y = 0, z = 0;
            var modalMove = MoveType.Rapid; // G0 is the power-on default

            foreach (var rawLine in gcode.Split('\n'))
            {
                // Strip inline comments and normalize
                var line = rawLine;
                var semi = line.IndexOf(';');
                if (semi >= 0) line = line.Substring(0, semi);
                line = line.Trim().ToUpperInvariant();
                if (string.IsNullOrEmpty(line)) continue;

                float nx = x, ny = y, nz = z;
                bool hasCoord = false;
                MoveType? lineMove = null;

                foreach (var tok in line.Split(new[] { ' ', '\t' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    switch (tok)
                    {
                        case "G0": case "G00":
                            lineMove = MoveType.Rapid;
                            modalMove = MoveType.Rapid;
                            break;
                        case "G1": case "G01":
                            lineMove = MoveType.Cut;
                            modalMove = MoveType.Cut;
                            break;
                        default:
                            if (tok.Length < 2) break;
                            if (tok[0] == 'X' && TryParse(tok, 1, out float vx))
                            { nx = vx; hasCoord = true; }
                            else if (tok[0] == 'Y' && TryParse(tok, 1, out float vy))
                            { ny = vy; hasCoord = true; }
                            else if (tok[0] == 'Z' && TryParse(tok, 1, out float vz))
                            { nz = vz; hasCoord = true; }
                            break;
                    }
                }

                // Only emit a move when the position actually changes
                if (hasCoord && (nx != x || ny != y || nz != z))
                {
                    moves.Add(new GCodeMove
                    {
                        // Remap: GCode(X,Y,Z) → SharpDX(X, Z, Y)
                        // so GCode's Z (layer height) becomes the Y (up) axis
                        From = new Vector3(x,  z,  y),
                        To   = new Vector3(nx, nz, ny),
                        Type = lineMove ?? modalMove
                    });
                }

                if (hasCoord) { x = nx; y = ny; z = nz; }
            }

            return moves;
        }

        private static bool TryParse(string tok, int start, out float value) =>
            float.TryParse(tok.Substring(start),
                           NumberStyles.Float,
                           CultureInfo.InvariantCulture,
                           out value);
    }
}
