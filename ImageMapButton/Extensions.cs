using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SpencerHakim
{
    public static class Extensions
    {
        public static void SafeDispose(this IDisposable disposable)
        {
            if( disposable != null )
                disposable.Dispose();
        }

        public static Rectangle Scale(this Rectangle rect, float scale)
        {
            return rect.Scale(scale, scale);
        }

        public static Rectangle Scale(this Rectangle rect, float scaleWX, float scaleHY)
        {
            return new Rectangle()
            {
                Location = rect.Location.Scale(scaleWX, scaleHY),
                Size = rect.Size.Scale(scaleWX, scaleHY)
            };
        }

        public static Point Scale(this Point point, float scale)
        {
            return point.Scale(scale, scale);
        }

        public static Point Scale(this Point point, float scaleX, float scaleY)
        {
            return new Point()
            {
                X = (int)(point.X * scaleX),
                Y = (int)(point.Y * scaleY)
            };
        }

        public static Size Scale(this Size point, float scale)
        {
            return point.Scale(scale, scale);
        }

        public static Size Scale(this Size point, float scaleW, float scaleH)
        {
            return new Size()
            {
                Height = (int)(point.Height * scaleH),
                Width = (int)(point.Width * scaleW)
            };
        }
    }
}