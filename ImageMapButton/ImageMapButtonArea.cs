using System;
using System.Drawing;

namespace SpencerHakim.Windows.Forms
{
    public class ImageMapButtonArea
    {
        #region Properties
        /// <summary>
        /// Gets the name of the area
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the map chroma key color
        /// </summary>
        public Color ChromaKey { get; private set; }

        /// <summary>
        /// Gets the destination location to draw over the base image
        /// </summary>
        public Point Destination { get; private set; }

        /// <summary>
        /// Gets the source location for the over and down images 
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets the size of the area to copy from the over and down images
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Gets the Rectangle based on Location and Size
        /// </summary>
        public Rectangle SourceRect
        {
            get
            {
                return new Rectangle()
                {
                    X = this.Location.X,
                    Y = this.Location.Y,
                    Height = this.Size.Height,
                    Width = this.Size.Width
                };
            }
        }

        /// <summary>
        /// Gets the Rectangle based on Destination and Size
        /// </summary>
        public Rectangle DestinationRect
        {
            get
            {
                return new Rectangle()
                {
                    X = this.Destination.X,
                    Y = this.Destination.Y,
                    Height = this.Size.Height,
                    Width = this.Size.Width
                };
            }
        }

        /// <summary>
        /// Gets whether the button area is a normal or toggle button
        /// </summary>
        public bool ToggleMode { get; private set; }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        //mutable properties below, don't affect equality/hashcode

        /// <summary>
        /// Gets or sets whether the area is mouse-pressed. Only used when ToggleMode is enabled
        /// </summary>
        public bool Pressed { get; set; }

        /// <summary>
        /// Gets or sets whether the area is enabled
        /// </summary>
        public bool Enabled { get; set; }
        #endregion

        /// <summary>
        /// Creates a default map area for the image map
        /// </summary>
        public ImageMapButtonArea()
            : this("", Color.Magenta, Point.Empty, Size.Empty)
        {
            //
        }

        /// <summary>
        /// Creates a new map area for the image map
        /// </summary>
        /// <param name="text">The text associated with the area. Used for the tooltip display</param>
        /// <param name="chromaKey">The color to chroma key the area to</param>
        /// <param name="source">Defines the source location from the over and down images</param>
        /// <param name="size">Defines the size of the area</param>
        public ImageMapButtonArea(string text, Color chromaKey, Point location, Size size)
            : this(text, chromaKey, location, location, size)
        {
            //
        }

        /// <summary>
        /// Creates a new map area for the image map
        /// </summary>
        /// <param name="text">The text associated with the area. Used for the tooltip display</param>
        /// <param name="chromaKey">The color to chroma key the area to</param>
        /// <param name="source">Defines the source location from the over and down images</param>
        /// <param name="size">Defines the size of the area</param>
        /// <param name="toggleMode">Defines whether the button area is a normal or toggle button</param>
        public ImageMapButtonArea(string text, Color chromaKey, Point location, Size size, bool toggleMode)
            : this(text, chromaKey, location, location, size, toggleMode)
        {
            //
        }

        /// <summary>
        /// Creates a new map area for the image map
        /// </summary>
        /// <param name="text">The text associated with the area. Used for the tooltip display</param>
        /// <param name="chromaKey">The color to chroma key the area to</param>
        /// <param name="destination">Defines the destination location to be painted over the foreground/background images</param>
        /// <param name="source">Defines the source location from the over and down images</param>
        /// <param name="size">Defines the size of the area</param>
        public ImageMapButtonArea(string text, Color chromaKey, Point destination, Point source, Size size)
            : this(text, chromaKey, destination, source, size, false)
        {
            //
        }

        /// <summary>
        /// Creates a new map area for the image map
        /// </summary>
        /// <param name="text">The text associated with the area. Used for the tooltip display</param>
        /// <param name="chromaKey">The color to chroma key the area to</param>
        /// <param name="destination">Defines the destination location to be painted over the foreground/background images</param>
        /// <param name="source">Defines the source location from the over and down images</param>
        /// <param name="size">Defines the size of the area</param>
        /// <param name="toggleMode">Defines whether the button area is a normal or toggle button</param>
        public ImageMapButtonArea(string text, Color chromaKey, Point destination, Point source, Size size, bool toggleMode)
        {
            //null is allowed, but we don't want any NPEs
            if( text == null )
                text = "";

            this.Text = text;
            this.ChromaKey = chromaKey;
            this.Location = source;
            this.Size = size;
            this.Destination = destination;
            this.ToggleMode = toggleMode;
            this.Pressed = false;
            this.Enabled = true;
        }

        #region Equals/GetHashCode
        public bool Equals(ImageMapButtonArea other)
        {
            if( other == null )
                return false;

            return (
                this.Text == other.Text &&
                this.ChromaKey.ToArgb() == other.ChromaKey.ToArgb() &&
                this.Location == other.Location &&
                this.Size == other.Size &&
                this.Destination == other.Destination &&
                this.ToggleMode == other.ToggleMode
            );
        }

        public override bool Equals(object other)
        {
            if( other == null )
                return false;

            if( other as ImageMapButtonArea == null )
                return base.Equals(other);

            return Equals( (ImageMapButtonArea)other );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + this.Text.GetHashCode();
                hash = (hash * 7) + this.ChromaKey.GetHashCode();
                hash = (hash * 7) + this.Location.GetHashCode();
                hash = (hash * 7) + this.Size.GetHashCode();
                hash = (hash * 7) + this.Destination.GetHashCode();
                hash = (hash * 7) + this.ToggleMode.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ImageMapButtonArea a, ImageMapButtonArea b)
        {
            if( Object.ReferenceEquals(a, b) )
                return true;

            if( (object)a == null || (object)b == null )
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ImageMapButtonArea a, ImageMapButtonArea b)
        {
            return !(a == b);
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", this.Text, this.ChromaKey, this.Pressed);
        }
    }
}