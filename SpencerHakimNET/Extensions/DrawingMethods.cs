using System.Drawing;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods that use System.Drawing
    /// </summary>
    public static class DrawingMethods
    {
        /// <summary>
        /// Returns a Rectangle with it's Location and Size properties multiplied by the provided scales
        /// </summary>
        /// <param name="rect">The Rectangle to scale</param>
        /// <param name="scale">The factor to scale the Location and Size properties by</param>
        /// <returns>A scaled Rectangle</returns>
        public static Rectangle Scale(this Rectangle rect, float scale)
        {
            return rect.Scale(scale, scale);
        }

        /// <summary>
        /// Returns a Rectangle with it's Location and Size properties multiplied by the provided scales
        /// </summary>
        /// <param name="rect">The Rectangle to scale</param>
        /// <param name="scaleWX">The factor to scale the Width and X properties by</param>
        /// <param name="scaleHY">The factor to scale the Height and Y properties by</param>
        /// <returns>A scaled Rectangle</returns>
        public static Rectangle Scale(this Rectangle rect, float scaleWX, float scaleHY)
        {
            return new Rectangle()
            {
                Location = rect.Location.Scale(scaleWX, scaleHY),
                Size = rect.Size.Scale(scaleHY, scaleWX)
            };
        }

        /// <summary>
        /// Returns a new Point with it's X and Y properties multiplied by the provided scale
        /// </summary>
        /// <param name="point">The Point to scale</param>
        /// <param name="scale">The factor to scale the X and Y property by</param>
        /// <returns>A scaled Point</returns>
        public static Point Scale(this Point point, float scale)
        {
            return point.Scale(scale, scale);
        }

        /// <summary>
        /// Returns a new Point with it's X and Y properties multiplied by the provided scales
        /// </summary>
        /// <param name="point">The Point to scale</param>
        /// <param name="scaleX">The factor to scale the X property by</param>
        /// <param name="scaleY">The factor to scale the Y property by</param>
        /// <returns>A scaled Point</returns>
        public static Point Scale(this Point point, float scaleX, float scaleY)
        {
            return new Point()
            {
                X = (int)(point.X * scaleX),
                Y = (int)(point.Y * scaleY)
            };
        }

        /// <summary>
        /// Returns a new Size with it's Height and Width properties multiplied by the provided scale
        /// </summary>
        /// <param name="size">The Size to scale</param>
        /// <param name="scale">The factor to scale the Height and Width properties by</param>
        /// <returns>A scaled Size</returns>
        public static Size Scale(this Size size, float scale)
        {
            return size.Scale(scale, scale);
        }

        /// <summary>
        /// Returns a new Size with it's Height and Width properties multiplied by the provided scales
        /// </summary>
        /// <param name="point">The Size to scale</param>
        /// <param name="scaleH">The factor to scale the Height property by</param>
        /// <param name="scaleW">The factor to scale the Width property by</param>
        /// <returns>A scaled Size</returns>
        public static Size Scale(this Size point, float scaleH, float scaleW)
        {
            return new Size()
            {
                Height = (int)(point.Height * scaleH),
                Width = (int)(point.Width * scaleW)
            };
        }
    }
}