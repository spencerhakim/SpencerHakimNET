using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SpencerHakim.Extensions;

namespace SpencerHakim.Windows
{
    /// <summary>
    /// Methods for easily taking screenshots
    /// </summary>
    public static class Screenshot
    {
        /// <summary>
        /// Takes a screenshot of all screens
        /// </summary>
        /// <returns>A Image of the screenshot</returns>
        public static Image Grab()
        {
            return Grab(Screen.AllScreens);
        }

        /// <summary>
        /// Takes a screenshot of the specified screen
        /// </summary>
        /// <param name="screen">The screen to take a screenshot of</param>
        /// <returns>A Image of the screenshot</returns>
        public static Image Grab(Screen screen)
        {
            if( screen == null )
                throw new ArgumentNullException("screen");

            return Grab(new[]{ screen });
        }

        /// <summary>
        /// Takes a screenshot of the specified screens (and any screens between them)
        /// </summary>
        /// <param name="screens">The screens to take a screenshot of</param>
        /// <returns>A Image of the screenshot</returns>
        public static Image Grab(Screen[] screens)
        {
            if( screens == null )
                throw new ArgumentNullException("screens");

            if( screens.Length == 0 )
                throw new ArgumentException("Empty array", "screens");

            Point start = new Point(0, 0);
            Point end = new Point(0, 0);

            //get bounds of total screen space
            foreach( Screen tempScreen in screens )
            {
                if( tempScreen.Bounds.X < start.X )
                    start.X = tempScreen.Bounds.X;

                if( tempScreen.Bounds.Y < start.Y )
                    start.Y = tempScreen.Bounds.Y;

                if( tempScreen.Bounds.X + tempScreen.Bounds.Width > end.X )
                    end.X = tempScreen.Bounds.X + tempScreen.Bounds.Width;

                if( tempScreen.Bounds.Y + tempScreen.Bounds.Height > end.Y )
                    end.Y = tempScreen.Bounds.Y + tempScreen.Bounds.Height;
            }

            return Grab(start, end);
        }

        /// <summary>
        /// Takes a screenshot of the specified screen region
        /// </summary>
        /// <param name="upperleft">The upper left point of the region to screenshot</param>
        /// <param name="bottomright">The bottom right point of the region to screenshot</param>
        /// <returns>A Image of the screenshot</returns>
        public static Image Grab(Point upperleft, Point bottomright)
        {
            Bitmap bitmap = new Bitmap(Math.Abs(bottomright.X - upperleft.X), Math.Abs(bottomright.Y - upperleft.Y));

            using( Graphics g = Graphics.FromImage(bitmap) )
                g.CopyFromScreen(upperleft, Point.Empty, bitmap.Size);

            return bitmap;
        }
    }
}
