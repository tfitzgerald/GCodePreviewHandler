using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GCodePreviewControl
{
    public static class GCodeRenderer
    {
        public static ModelVisual3D Render(string path)
        {
            var lines = GCodeParser.Parse(path);
            var group = new Model3DGroup();

            foreach (var segment in lines)
            {
                var builder = new MeshBuilder();
                builder.AddCylinder(segment.Start, segment.End, 0.2, 8);
                var mesh = builder.ToMesh();

                group.Children.Add(new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = Materials.Blue
                });
            }

            return new ModelVisual3D { Content = group };
        }
    }
}
