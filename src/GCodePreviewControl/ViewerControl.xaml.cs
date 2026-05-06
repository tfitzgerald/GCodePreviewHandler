using System.Windows.Controls;
using HelixToolkit.Wpf;

namespace GCodePreviewControl
{
    public partial class ViewerControl : UserControl
    {
        public ViewerControl()
        {
            InitializeComponent();
        }

        public void LoadGCode(string path)
        {
            var model = GCodeRenderer.Render(path);
            Viewport.Children.Clear();
            Viewport.Children.Add(model);
        }
    }
}
