using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GCodePreviewHandler
{
    [ComVisible(true)]
    [Guid("D0A1F2C1-ABCD-4F00-9A11-1234567890AB")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PreviewHandler :
        IPreviewHandler,
        IInitializeWithFile,
        IOleWindow
    {
        private IntPtr _parentHwnd = IntPtr.Zero;
        private RECT _rect;
        private string? _filePath;
        private bool _initialized;

        private Form? _hostForm;
        private TextBox? _textBox;

        #region IInitializeWithFile

        public void Initialize(string pszFilePath, STGM grfMode)
        {
            _filePath = pszFilePath;
            _initialized = true;
        }

        #endregion

        #region IPreviewHandler

        public void SetWindow(IntPtr hwnd, ref RECT rect)
        {
            _parentHwnd = hwnd;
            _rect = rect;

            if (_hostForm == null)
            {
                _hostForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    TopLevel = false,
                    ShowInTaskbar = false
                };

                _textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Dock = DockStyle.Fill,
                    WordWrap = false
                };

                _hostForm.Controls.Add(_textBox);
                _hostForm.CreateControl();

                NativeMethods.SetParent(_hostForm.Handle, _parentHwnd);
                UpdateBounds();
                _hostForm.Show();
            }
            else
            {
                UpdateBounds();
            }
        }

        public void SetRect(ref RECT rect)
        {
            _rect = rect;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            if (_hostForm == null)
                return;

            int width = _rect.Right - _rect.Left;
            int height = _rect.Bottom - _rect.Top;

            _hostForm.Bounds = new System.Drawing.Rectangle(
                _rect.Left,
                _rect.Top,
                width,
                height);
        }

        public void DoPreview()
        {
            if (!_initialized || string.IsNullOrEmpty(_filePath) || _textBox == null)
                return;

            try
            {
                string text = File.ReadAllText(_filePath);
                _textBox.Text = text;
            }
            catch (Exception ex)
            {
                _textBox.Text = "Error loading file:\r\n" + ex;
            }
        }

        public void Unload()
        {
            _filePath = null;
            _initialized = false;

            if (_hostForm != null)
            {
                _hostForm.Close();
                _hostForm.Dispose();
                _hostForm = null;
            }

            _textBox = null;
            _parentHwnd = IntPtr.Zero;
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
            return 0;
        }

        #endregion

        #region IOleWindow

        public void GetWindow(out IntPtr phwnd)
        {
            phwnd = _hostForm?.Handle ?? IntPtr.Zero;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
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

    #endregion
}
