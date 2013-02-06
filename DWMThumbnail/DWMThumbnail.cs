using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpencerHakim.Windows.Forms
{
    public partial class DWMThumbnail : Control
    {
        #region Interop stuff
        [DllImport("dwmapi.dll")]
        private static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        private static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out Size size);

        [Flags]
        private enum DWM_TNP
        {
            RECTDESTINATION = 0x1,
            RECTSOURCE = 0x2,
            OPACITY = 0x4,
            VISIBLE = 0x8,
            SOURCECLIENTAREAONLY = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_THUMBNAIL_PROPERTIES
        {
            public DWM_TNP dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);
        #endregion

        #region Privates
        private IntPtr thumbId = IntPtr.Zero;
        private IntPtr hwndSource = IntPtr.Zero;
        #endregion

        #region Properties
        public IntPtr SourceWindow
        {
            get { return hwndSource; }
            set
            {
                if( thumbId != IntPtr.Zero )
                    Marshal.ThrowExceptionForHR( DwmUnregisterThumbnail(thumbId) );

                //reset privates
                thumbId = IntPtr.Zero;
                hwndSource = value;

                if( hwndSource == IntPtr.Zero )
                    return;

                Marshal.ThrowExceptionForHR( DwmRegisterThumbnail(this.Handle, this.hwndSource, out thumbId) );
                UpdateThumbProps();
            }
        }
        #endregion

        public DWMThumbnail()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            //unmanaged, will unregister thumb
            this.SourceWindow = IntPtr.Zero;

            if( disposing && components != null )
                components.Dispose();

            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateThumbProps();
        }

        private void UpdateThumbProps()
        {
            if( thumbId != IntPtr.Zero )
            {
                Size sourceSize;
                Marshal.ThrowExceptionForHR( DwmQueryThumbnailSourceSize(thumbId, out sourceSize) );

                //TODO - create properties to control these
                DWM_THUMBNAIL_PROPERTIES dwmProps = new DWM_THUMBNAIL_PROPERTIES();
                dwmProps.dwFlags = DWM_TNP.VISIBLE | DWM_TNP.RECTDESTINATION | DWM_TNP.SOURCECLIENTAREAONLY;
                dwmProps.fVisible = true;
                dwmProps.fSourceClientAreaOnly = true;
                dwmProps.rcDestination = new RECT(this.Left, this.Top, this.Right, this.Bottom);

                //don't scale up
                if( sourceSize.Width < this.Width )
                    dwmProps.rcDestination.Right = dwmProps.rcDestination.Left + sourceSize.Width;
                if( sourceSize.Height < this.Height )
                    dwmProps.rcDestination.Bottom = dwmProps.rcDestination.Top + sourceSize.Height;

                Marshal.ThrowExceptionForHR( DwmUpdateThumbnailProperties(thumbId, ref dwmProps) );
            }
        }
    }
}
