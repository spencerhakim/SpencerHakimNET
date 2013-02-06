using System;
using System.ComponentModel;
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
        /// <summary>
        /// Gets and sets the handle of the window to display the thumbnail of
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr SourceWindow
        {
            get { return hwndSource; }
            set
            {
                if( this.thumbId != IntPtr.Zero )
                    Marshal.ThrowExceptionForHR( DwmUnregisterThumbnail(this.thumbId) );

                //reset privates
                this.thumbId = IntPtr.Zero;
                this.hwndSource = value;

                if( this.hwndSource == IntPtr.Zero )
                    return;

                Marshal.ThrowExceptionForHR( DwmRegisterThumbnail(this.FindForm().Handle, this.hwndSource, out this.thumbId) );
                this.UpdateThumbProps();
            }
        }

        /// <summary>
        /// Gets or sets whether the thumbnail should scale above the native resolution of the source window
        /// </summary>
        [Category("Appearance"), Description("Gets or sets whether the thumbnail should scale above the native resolution of the source window")]
        [DefaultValue(true)]
        public bool ScaleAboveNativeSize
        {
            get { return this.scaleAboveNativeSize; }
            set
            {
                this.scaleAboveNativeSize = value;
                this.UpdateThumbProps();
            }
        }
        private bool scaleAboveNativeSize = true;

        /// <summary>
        /// Gets or sets the opacity of the thumbnail
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the opacity of the thumbnail")]
        [DefaultValue((byte)255)]
        public byte Opacity
        {
            get { return this.opacity; }
            set
            {
                this.opacity = value;
                this.UpdateThumbProps();
            }
        }
        private byte opacity = 255;

        /// <summary>
        /// Gets or sets whether the thumbnail should include the native Windows border (Aero glass, titlebar, titlebar buttons, etc.) in the thumbnail.
        /// NOTE: Windows that draw their own border are unaffected by this setting.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets whether the thumbnail should include the native Windows border (Aero glass, titlebar, titlebar buttons, etc.) in the thumbnail")]
        [DefaultValue(false)]
        public bool SourceClientAreaOnly
        {
            get { return this.sourceClientAreaOnly; }
            set
            {
                this.sourceClientAreaOnly = value;
                this.UpdateThumbProps();
            }
        }
        private bool sourceClientAreaOnly = false;

        /// <summary>
        /// Gets the absolute position of the control relative to its form
        /// </summary>
        protected Point AbsoluteLocation
        {
            get { return this.FindForm().PointToClient( this.Parent.PointToScreen(this.Location) ); }
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

        #region Overrides
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.UpdateThumbProps();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.UpdateThumbProps();
        }
        #endregion

        private void UpdateThumbProps()
        {
            if( thumbId != IntPtr.Zero )
            {
                Size sourceSize;
                Marshal.ThrowExceptionForHR( DwmQueryThumbnailSourceSize(this.thumbId, out sourceSize) );

                //TODO - create properties to control these
                DWM_THUMBNAIL_PROPERTIES dwmProps = new DWM_THUMBNAIL_PROPERTIES();
                dwmProps.dwFlags = DWM_TNP.VISIBLE | DWM_TNP.OPACITY | DWM_TNP.RECTDESTINATION | DWM_TNP.SOURCECLIENTAREAONLY;
                dwmProps.fVisible = this.Visible;
                dwmProps.opacity = this.Opacity;
                dwmProps.fSourceClientAreaOnly = this.SourceClientAreaOnly;
                dwmProps.rcDestination = new RECT(
                    this.AbsoluteLocation.X, this.AbsoluteLocation.Y,
                    this.AbsoluteLocation.X + this.Width, this.AbsoluteLocation.Y + this.Height
                );

                if( !this.ScaleAboveNativeSize )
                {
                    if( sourceSize.Width < this.Width )
                        dwmProps.rcDestination.Right = dwmProps.rcDestination.Left + sourceSize.Width;
                    if( sourceSize.Height < this.Height )
                        dwmProps.rcDestination.Bottom = dwmProps.rcDestination.Top + sourceSize.Height;
                }

                Marshal.ThrowExceptionForHR( DwmUpdateThumbnailProperties(this.thumbId, ref dwmProps) );
            }
        }
    }
}
