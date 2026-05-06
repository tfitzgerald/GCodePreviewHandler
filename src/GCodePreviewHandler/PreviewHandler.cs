using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GCodePreviewControl;

namespace GCodePreviewHandler
{
    [ComVisible(true)]
    [Guid("33333333-3333-3333-3333-333333333333")]
    public class PreviewHandler : IPreviewHandler
    {
        private ViewerControl _viewer;

        public void DoPreview(string filePath, IntPtr hwnd)
        {
            _viewer = new ViewerControl();
            _viewer.LoadGCode(filePath);

            var host = new ElementHost { Child = _viewer };
            host.Dock = DockStyle.Fill;

            var parent = Control.FromHandle(hwnd);
            parent.Controls.Add(host);
        }
    }
}
