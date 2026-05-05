using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GCodePreviewControl;

namespace GCodePreviewHandler
{
    [ComVisible(true)]
    [Guid("D0A1F2C1-ABCD-4F00-9A11-1234567890AB")]
    public class PreviewHandler : UserControl
    {
        private readonly GCodePreviewControl.GCodePreviewControl _viewer =
            new GCodePreviewControl.GCodePreviewControl();

        public PreviewHandler()
        {
            Controls.Add(_viewer);
            _viewer.Dock = DockStyle.Fill;
        }

        public void Load(string path)
        {
            var text = System.IO.File.ReadAllText(path);
            _viewer.LoadGCode(text);
        }
    }
}
