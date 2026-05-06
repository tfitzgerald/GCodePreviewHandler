using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using GCodePreviewControl;

namespace GCodePreviewHandler
{
    // CLSID for the preview handler (keep this stable)
    [ComVisible(true)]
    [Guid("D0A1F2C1-ABCD-4F00-9A11-1234567890AB")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PreviewHandler :
        IPreviewHandler,
        IInitializeWithFile,
        IOleWindow
    {
        private IntPtr _parentHwnd = IntPtr.Zero;
        private Rectangle _bounds;
        private bool _isInitialized;
        private string? _filePath;

        private Form? _hostForm;
        private ElementHost? _elementHost;
        private GCodePreviewControl.GCodePreviewControl? _viewer;

        #region IInitializeWithFile

        public void Initialize(string pszFilePath, STGM grfMode)
        {
            _filePath = pszFilePath;
            _isInitialized = true;
        }

        #endregion

        #region IPreviewHandler

        public void SetWindow(IntPtr hwnd, ref RECT rect)
        {
            _parentHwnd = hwnd;
            _bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            if (_hostForm == null)
            {
                _hostForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    TopLevel = false,
                    ShowInTaskbar = false
                };

                _elementHost = new ElementHost
                {
                    Dock = DockStyle.Fill
                };

                _viewer = new GCodePreviewControl.GCodePreviewControl();
                _elementHost.Child = _viewer;

                _hostForm.Controls.Add(_elementHost);
                _hostForm.CreateControl();

                NativeMethods.SetParent(_hostForm.Handle, _parentHwnd);
                _hostForm.Bounds = _bounds;
                _hostForm.Show();
            }
            else
            {
                _hostForm.Bounds = _bounds;
            }
        }

        public void SetRect(ref RECT rect)
        {
            _bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            if (_hostForm != null)
            {
                _hostForm.Bounds = _bounds;
            }
        }

		public void DoPreview()
		{
			try
			{
				File.AppendAllText(@"C:\Temp\GCodePreview.log", $"DoPreview start: {_filePath}{Environment.NewLine}");

				if (!_isInitialized || string.IsNullOrEmpty(_filePath) || _viewer == null)
				{
					File.AppendAllText(@"C:\Temp\GCodePreview.log", "DoPreview: not initialized or no viewer" + Environment.NewLine);
					return;
				}

				string text = File.ReadAllText(_filePath);
				File.AppendAllText(@"C:\Temp\GCodePreview.log", $"DoPreview: read {text.Length} chars{Environment.NewLine}");

				_viewer.LoadGCode(text);

				File.AppendAllText(@"C:\Temp\GCodePreview.log", "DoPreview: LoadGCode completed" + Environment.NewLine);
			}
			catch (Exception ex)
			{
				File.AppendAllText(@"C:\Temp\GCodePreview.log", "DoPreview ERROR: " + ex + Environment.NewLine);
			}
		}


        public void Unload()
        {
            _viewer = null;

            if (_elementHost != null)
            {
                _elementHost.Dispose();
                _elementHost = null;
            }

            if (_hostForm != null)
            {
                _hostForm.Close();
                _hostForm.Dispose();
                _hostForm = null;
            }

            _parentHwnd = IntPtr.Zero;
            _isInitialized = false;
            _filePath = null;
        }

        public void SetFocus()
        {
            _hostForm?.Focus();
        }

        public void QueryFocus(out IntPtr phwnd)
        {
            phwnd = _hostForm?.Handle ?? IntPtr.Zero;
        }

        public uint TranslateAccelerator(ref MSG pmsg)
        {
            return 0; // no keyboard handling
        }

        #endregion

        #region IOleWindow

        public void GetWindow(out IntPtr phwnd)
        {
            phwnd = _hostForm?.Handle ?? IntPtr.Zero;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
            // not used
        }

        #endregion
    }

    #region Interop

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int pt_x;
        public int pt_y;
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
    public interface IInitializeWithFile
    {
        void Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, STGM grfMode);
    }

    [Flags]
    public enum STGM : uint
    {
        READ = 0x00000000,
        WRITE = 0x00000001,
        READWRITE = 0x00000002
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
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
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000114-0000-0000-C000-000000000046")]
    public interface IOleWindow
    {
        void GetWindow(out IntPtr phwnd);
        void ContextSensitiveHelp(bool fEnterMode);
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }

    // WinForms/WPF bridge
    public class ElementHost : System.Windows.Forms.Integration.ElementHost { }

    #endregion
}
