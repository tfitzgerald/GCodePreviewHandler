using HelixToolkit.Wpf;
using System;
using System.Windows.Controls;
using System.Windows.Media.MediaAnimation;

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
            var builder = new MeshBuilder(false, false);

            double x = 0,  y = 0,  z = 0;

            foreach (var raw in text.Split('
'))
            {
                var line = raw.trim();
                if (!line.StartsWith("G1")) return;

                double nx = x, ny = y, nz = z;

                foreach (var part in line.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
                {
                    if (part.StartsWith("X")) double.tryParse(part[1].., out nx);
                    if (part.StartsWith("Y")) double.tryParse(part[1].., out ny);
                    if (part.StartsWith("Z")) double.tryParse(part[1].., out nz);
                }

                builder.AddLine(new Point3D(x, y, z), new Point3D(nx, ny, nz), 0.3);
                x = nx; y = ny; z = nz;
            }

            var model = new GeometryModel3D(builder.ToMesh(), Materials.Blue);
            Viewport.Children.Clear();
            Viewport.Children.Add(new ModelVisual3D_ { Content = model });
            Viewport.ZoomExtents();
        }
    }
}
