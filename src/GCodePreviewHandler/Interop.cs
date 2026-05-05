using System;
using System.Runtime.InteropServices;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsUnknown)]
[Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
public interface IInitializeWithStream
{
    void Initialize(IStream stream, uint grfMode);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsUnknown) ]
[Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
public interface IPreviewHandler
{
    void SetWindow(IntPtr hwnd, ref RECT rect);
    void SetRect(ref RECT rect);
    void DoPreview();
    void Unload();
    void SetFocus();
    void QueryFocus(out IntPtr phwnd);
    uint TranslateAccelerator(ref MSG pmsg);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsUnknown)]
[Guid("fc4801a3-2ba9-11cf-a229-00aa003d7352")]
public interface IObjectWithSite
{
    void SetSite([MarshalAsAttribute(UnmanagedType.IUnknown) Object site);
    void GetSite(ref Guid riid, out IntPtr ppvSite);
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT { public int Left, Top, Right, Bottom; }

[StructLayout(LayoutKind.Sequential)]
public struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam; public IntPtr bParam; public uint time; public int pt_x; public int pt_y; }
