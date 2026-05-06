using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            // Parse on a background thread so the UI stays responsive
            LoadingText.Visibility = Visibility.Visible;
            StatusText.Text = string.Empty;
            Viewport.Items.Clear();

            Task.Run(() => GCodeParser.Parse(text))
                .ContinueWith(t => RenderMoves(t.Result),
                              System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RenderMoves(System.Collections.Generic.List<GCodeMove> moves)
        {
            LoadingText.Visibility = Visibility.Collapsed;

            if (moves.Count == 0)
            {
                StatusText.Text = "No toolpath moves found.";
                return;
            }

            // --- Build separate line sets for rapids and cuts ---
            var rapidBuilder = new LineBuilder();
            var cutBuilder   = new LineBuilder();

            foreach (var m in moves)
            {
                if (m.Type == MoveType.Rapid)
                    rapidBuilder.AddLine(m.From, m.To);
                else
                    cutBuilder.AddLine(m.From, m.To);
            }

            Viewport.Items.Clear();

            // Cuts — bright blue
            var cutGeo = cutBuilder.ToLineGeometry3D();
            if (cutGeo?.Positions?.Count > 0)
            {
                Viewport.Items.Add(new LineGeometryModel3D
                {
                    Geometry  = cutGeo,
                    Color     = new Color4(0.20f, 0.65f, 1.00f, 1.0f),
                    Thickness = 1.5f
                });
            }

            // Rapids — dim grey, thinner
            var rapidGeo = rapidBuilder.ToLineGeometry3D();
            if (rapidGeo?.Positions?.Count > 0)
            {
                Viewport.Items.Add(new LineGeometryModel3D
                {
                    Geometry  = rapidGeo,
                    Color     = new Color4(0.45f, 0.45f, 0.45f, 0.6f),
                    Thickness = 0.8f
                });
            }

            Viewport.ZoomExtents(animationTime: 0);

            int cuts   = moves.Count(m => m.Type == MoveType.Cut);
            int rapids = moves.Count(m => m.Type == MoveType.Rapid);
            StatusText.Text =
                $"{moves.Count:N0} moves  ·  {cuts:N0} cuts (blue)  ·  {rapids:N0} rapids (grey)" +
                "    |  Drag to orbit  ·  Scroll to zoom  ·  Right-drag to pan";
        }
    }
}
