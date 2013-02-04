using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PushButtonState = System.Windows.Forms.VisualStyles.PushButtonState;

namespace SpencerHakim.Windows.Forms
{
    public partial class ImageMapButton : Control, IEnumerable<ImageMapButtonArea>
    {
        #region Properties and Events
        /// <summary>
        /// Specifies the algorithm used when images are scaled or rotated
        /// </summary>
        [Category("Appearance"), DefaultValue(InterpolationMode.Default)]
        [Description("Specifies the algorithm used when images are scaled or rotated")]
        public InterpolationMode InterpolationMode { get; set; }

        /// <summary>
        /// Specifies how pixels are offset during rendering
        /// </summary>
        [Category("Appearance"), DefaultValue(PixelOffsetMode.Default)]
        [Description("Specifies how pixels are offset during rendering")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        /// <summary>
        /// Default foreground image for the control
        /// </summary>
        [Category("Appearance"), DefaultValue(null)]
        [Description("Default foreground image for the control")]
        public Image Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.image = value;

                if( this.image != null )
                {
                    this.Width = this.image.Width;
                    this.Height = this.image.Height;
                }

                this.Invalidate();
            }
        }
        private Image image;

        /// <summary>
        /// Image for the "hot/over" state
        /// </summary>
        [Category("Appearance"), DefaultValue(null)]
        [Description("Image for the \"hot/over\" state")]
        public Image MouseOverImage
        {
            get
            {
                return this.mouseOverImage;
            }

            set
            {
                this.mouseOverImage = value;
                this.Invalidate();
            }
        }
        private Image mouseOverImage;

        /// <summary>
        /// Image for the "pressed/down" state
        /// </summary>
        [Category("Appearance"), DefaultValue(null)]
        [Description("Image for the \"pressed/down\" state")]
        public Image MouseDownImage
        {
            get
            {
                return this.mouseDownImage;
            }

            set
            {
                this.mouseDownImage = value;
                this.Invalidate();
            }
        }
        private Image mouseDownImage;

        /// <summary>
        /// Image for the disabled state
        /// </summary>
        [Category("Appearance"), DefaultValue(null)]
        [Description("Image for the disabled state")]
        public Image DisabledImage
        {
            get
            {
                return this.disabledImage;
            }

            set
            {
                this.disabledImage = value;
                this.Invalidate();
            }
        }
        private Image disabledImage;

        /// <summary>
        /// Image containing the chromakeys for the map areas
        /// </summary>
        [Category("Behavior"), DefaultValue(null)]
        [Description("Image containing the chromakeys for the map areas")]
        public Image ChromaKeyImage
        {
            get
            {
                return this.chromaKeyImage;
            }

            set
            {
                this.chromaKeyImage = value;
                this.chromaKeyBitmap.SafeDispose();
                this.chromaKeyBitmap = new Bitmap(this.chromaKeyImage);

                this.Invalidate();
            }
        }
        private Image chromaKeyImage;
        private Bitmap chromaKeyBitmap;

        /// <summary>
        /// Controls whether mouse press events will fire repeatedly while the button is held down
        /// </summary>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Controls whether mouse press events will fire repeatedly while the button is held down")]
        public bool MultiClick { get; set; }

        /// <summary>
        /// Occurs when a map area is clicked
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a map area is clicked")]
        public event EventHandler<ButtonClickedEventArgs> MapAreaClicked = delegate{};

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        #endregion

        private Dictionary<int, ImageMapButtonArea> mapAreas = new Dictionary<int,ImageMapButtonArea>(); //can't use Color as TKey because named colors != unnamed colors, even if ARGB is equal
        private Color currentChromaKey = Color.Empty;
        private PushButtonState currentAreaState = PushButtonState.Normal; //state of whatever button the mouse is currently hovering over/clicking

        private float scaleWX = 1;
        private float scaleHY = 1;

        public ImageMapButton()
        {
            InitializeComponent();
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw,
                true
            );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
                //user is reponsible for the Images in the public properties, but chromaKeyBitmap is private and created by us
                this.chromaKeyBitmap.SafeDispose();

                components.SafeDispose();
            }
            base.Dispose(disposing);
        }

        #region Public methods
        /// <summary>
        /// Adds a map area to the button
        /// </summary>
        /// <param name="area">The map area to add</param>
        public void AddMap(ImageMapButtonArea area)
        {
            if( area == null )
                throw new ArgumentNullException("area");

            this.mapAreas.Add(area.ChromaKey.ToArgb(), area);
        }

        /// <summary>
        /// Removes all map areas from the button
        /// </summary>
        public void ClearMaps()
        {
            this.mapAreas.Clear();
        }

        /// <summary>
        /// Removes a map area identified by it's ChromaKey
        /// </summary>
        /// <param name="chromaKey">The ChromaKey of the map area to remove</param>
        public void RemoveMap(Color chromaKey)
        {
            this.mapAreas.Remove(chromaKey.ToArgb());
        }

        public ImageMapButtonArea this[Color chromaKey]
        {
            get { return this.mapAreas[chromaKey.ToArgb()]; }
        }
        #endregion

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //set scaling algos
            e.Graphics.InterpolationMode = this.InterpolationMode;
            e.Graphics.PixelOffsetMode = this.PixelOffsetMode;

            //calculate scale and draw foreground image
            if( this.Image != null )
            {
                GraphicsUnit gu = GraphicsUnit.Pixel;
                RectangleF rF = this.Image.GetBounds(ref gu); //why the fuck is this a ref?
                scaleHY = (float)this.Height / rF.Height;
                scaleWX = (float)this.Width / rF.Width;

                e.Graphics.DrawImage(this.Image, 0, 0, this.Width, this.Height);
            }

            //draw disabled areas, or draw all areas as disabled if control is disabled
            var disabledAreas = this.Where( area => !this.Enabled || !area.Enabled );
            foreach(var area in disabledAreas)
            {
                if( this.DisabledImage != null )
                    e.Graphics.DrawImage(this.DisabledImage, area.DestinationRect.Scale(scaleWX, scaleHY), area.SourceRect, GraphicsUnit.Pixel);
            }

            //don't draw anything else if control is disabled
            if( !this.Enabled )
                return;

            //draw toggled buttons
            var toggledAreas = this.Where( area => area.ToggleMode && area.Pressed );
            foreach(var area in toggledAreas)
            {
                if( this.MouseDownImage != null )
                    e.Graphics.DrawImage(this.MouseDownImage, area.DestinationRect.Scale(scaleWX, scaleHY), area.SourceRect, GraphicsUnit.Pixel);
            }

            //draw the current button
            switch( this.currentAreaState )
            {
                case PushButtonState.Hot:
                    //draw MouseOverImage image
                    if( this.currentChromaKey != Color.Empty && this.MouseOverImage != null )
                    {
                        ImageMapButtonArea area = this[this.currentChromaKey];
                        e.Graphics.DrawImage(this.MouseOverImage, area.DestinationRect.Scale(scaleWX, scaleHY), area.SourceRect, GraphicsUnit.Pixel);
                    }
                    break;

                case PushButtonState.Pressed:
                    //draw MouseDownImage image
                    if( this.currentChromaKey != Color.Empty && this.MouseDownImage != null )
                    {
                        ImageMapButtonArea area = this[this.currentChromaKey];
                        e.Graphics.DrawImage(this.MouseDownImage, area.DestinationRect.Scale(scaleWX, scaleHY), area.SourceRect, GraphicsUnit.Pixel);
                    }
                    break;

                default:
                    //nothing to do otherwise
                    break;
            }
        }

        protected virtual void OnButtonClicked(ButtonClickedEventArgs e)
        {
            if( this.MapAreaClicked != null )
                this.MapAreaClicked(this, e);
        }
        #endregion

        #region Event Handlers
        private void ImageMapButton_EnabledChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void ImageMapButton_MouseMove(object sender, MouseEventArgs e)
        {
            //we need the chromaKey bitmap to check if we're hovering something or not
            if( this.chromaKeyBitmap == null )
                return;

            //unscale mouse XY when interacting with chromaKeyBitmap
            int eX = (int)(e.X * (1/this.scaleWX));
            int eY = (int)(e.Y * (1/this.scaleHY));

            //grab chromaKey pixel at mouse location
            Color pixel = Color.Empty;
            if( (eX >= 0 && eX < this.chromaKeyBitmap.Width) &&
                (eY >= 0 && eY < this.chromaKeyBitmap.Height)
            )
                pixel = this.chromaKeyBitmap.GetPixel(eX, eY);

            //if chromaKey is valid and area is enabled, change state
            if( this.mapAreas.ContainsKey(pixel.ToArgb()) )
            {
                if( this[pixel].Enabled )
                {
                    //don't change back to Hot when already Pressed (unless moving to another area), mouse is allowed to move while down
                    if( this.currentAreaState != PushButtonState.Pressed || this.currentChromaKey.ToArgb() != pixel.ToArgb() )
                        this.currentAreaState = PushButtonState.Hot;
                    this.currentChromaKey = pixel;
                    this.Cursor = Cursors.Hand;
                }

                //set tooltip regardless of whether the area is active
                this.toolTip.Active = true;
                if( this.toolTip.GetToolTip(this) != this[pixel].Text ) //prevents tooltip flicker
                    this.toolTip.SetToolTip(this, this[pixel].Text);
            }
            else //if not over an ImageMapButtonArea, reset
            {
                this.currentAreaState = PushButtonState.Normal;
                this.currentChromaKey = Color.Empty;
                this.Cursor = Cursors.Default;
                this.toolTip.Active = false;
            }

            this.Invalidate();
        }

        private void ImageMapButton_MouseLeave(object sender, EventArgs e)
        {
            //reset mouse down/over
            this.currentAreaState = PushButtonState.Normal;
            this.currentChromaKey = Color.Empty;
            this.Cursor = Cursors.Default;

            this.Invalidate();
        }

        private void ImageMapButton_MouseDown(object sender, MouseEventArgs e)
        {
            //if chromaKey is valid, change state
            if( this.mapAreas.ContainsKey(this.currentChromaKey.ToArgb()) && this[this.currentChromaKey].Enabled )
            {
                this.currentAreaState = PushButtonState.Pressed;
                this.Cursor = Cursors.Hand;

                this.Invalidate();

                //toggle buttons don't support multiclick
                ImageMapButtonArea area = this[this.currentChromaKey];
                if( this.MultiClick && !area.ToggleMode )
                {
                    int shrinkingInterval = 500; //ms
                    do
                    {
                        //click, then idle for shrinkingInterval ms
                        this.OnButtonClicked( new ButtonClickedEventArgs(e, area) );
                        for(int i=0; i < shrinkingInterval/10 && this.currentAreaState == PushButtonState.Pressed; i++)
                        {
                            Thread.Sleep(10);
                            Application.DoEvents();
                        }

                        //slow decrease interval, but don't go below 10
                        if( shrinkingInterval > 10 )
                            shrinkingInterval -= (shrinkingInterval >= 30 ? 20 : shrinkingInterval - 10);
                    }
                    while( this.currentAreaState == PushButtonState.Pressed );
                }
            }
        }

        private void ImageMapButton_MouseUp(object sender, MouseEventArgs e)
        {
            //if chromaKey is valid, change state
            if( this.mapAreas.ContainsKey(this.currentChromaKey.ToArgb()) )
            {
                //non-multiclick is raised on MouseUp, allowing user to move mouse off area to "cancel" click
                ImageMapButtonArea area = this[this.currentChromaKey];
                if( (!this.MultiClick || area.ToggleMode) && this.currentChromaKey != Color.Empty )
                {
                    if( area.ToggleMode )
                        area.Pressed = !area.Pressed;
                
                    this.OnButtonClicked( new ButtonClickedEventArgs(e, area) );
                }
            }

            this.currentAreaState = PushButtonState.Hot; //will either be left as Hot or changed to Normal, but can't be left as Pressed
            this.ImageMapButton_MouseMove(sender, e); //handles the rest of the reset, calls invalidate
        }
        #endregion

        #region IEnumerable implementation
        public IEnumerator<ImageMapButtonArea> GetEnumerator()
        {
            foreach(var area in this.mapAreas)
            {
                yield return area.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}