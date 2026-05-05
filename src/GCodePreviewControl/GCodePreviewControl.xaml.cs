using System.Windows.Controls;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace GCodePreviewControl
{
    public partial class GCodePreviewControl : UserControl
    {
        public GCodePreviewControl()
        {
            InitializeComponent();
        }

        public void LoadGCode(string text)
        {
            var builder = new LineBuilder();

            float x = 0, y = 0, z = 0;

            foreach (var raw in text.Split('\n'))
            {
                var line = raw.Trim();
                if (!line.StartsWith("G1"))
                    continue;

                float nx = x, ny = y, nz = z;

                foreach (var part in line.Split(' '))
                {
                    if (part.StartsWith("X") && float.TryParse(part.Substring(1), out float vx)) nx = vx;
                    if (part.StartsWith("Y") && float.TryParse(part.Substring(1), out float vy)) ny = vy;
                    if (part.StartsWith("Z") && float.TryParse(part.Substring(1), out float vz)) nz = vz;
                }

                builder.AddLine(new Vector3(x, y, z), new Vector3(nx, ny, nz));

                x = nx; y = ny; z = nz;
            }

            var geometry = builder.ToLineGeometry3D();

            var model = new LineGeometryModel3D
            {
                Geometry = geometry,
                Color = new Color4(0f, 0f, 1f, 1f),
                Thickness = 2.0f
            };

            Viewport.Items.Clear();
            Viewport.Items.Add(model);

            Viewport.ZoomExtents();
        }
    }
}
