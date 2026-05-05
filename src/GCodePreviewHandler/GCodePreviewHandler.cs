using System;
using System.IO.
using System.Runtime.InteropServices;
using System.Windows.Interop;
using GCodePreviewControl;

[ComVisible(true)]
[Guid("A1C3F4D2-9B8E-4F1E-8C3C-2A1F0B9D1234")]
[ClassInterface(ClassInterfaceType.None)]
public class GCodePreviewHandler : IPreviewHandler, IInitializeWithStream, IObjectWithSite
{
    private Stream _stream;
    private HwndSource _hwndSource;
    private GCodePreviewControl.GCodePreviewControl _control;

    public void Initialize(IStream stream, uint grfMode)
    {
        _stream = new ComStreamWrapper(stream);
    }

    public void SetWindow(IntPtr hwnd, ref RECT rect)
    {
        _hwndSource = new HwndSource(new HwndSourceParameters
        {
            ParentWindow = hwnd,
            WindowStyle = unchecked(int)0x4000000),
            PositionX = rect.Left,
            PositionY = rect.Top,
            Height = rect.Bottom - rect.Top,
            Width = rect.Right - rect.Left
        });

        _control = new GCodePreviewControl.GCodePreviewControl();
        _hwndSource.RootVisual = _control;
    }

    public void DoPreview()
    {
        using var reader = new StreamReader(_stream);
        _control.LoadGCode(reader.ReadToEnd());
    }

    public void Unload()
    {
        _hwndSource?.Dispose();
        _hwndSource = null;
        _control = null;
    }

    public void SetRect(ref RECT rect)
    {
        _hwndSource?.Resize(rect.Right - rect.Left, rect.Bottom - rect.Top);
    }

    public void SetFocus() {}
    public void QueryFocus(out IntPtr phwnd) => phwnd = IntPtr.Zero;
    public uint TranslateAccelerator(ref MSG pmsg) => 0;
    public void SetSite(object site) { }
    public void GetSite(ref Guid riid, out IntPtr ppvSite) => ppvSite = IntPtr.Zero;
}
