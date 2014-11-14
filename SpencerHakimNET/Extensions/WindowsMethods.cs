using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods that use System.Windows.Forms
    /// </summary>
    public static class WindowsMethods
    {
        /// <summary>
        /// Non-broken replacement for Control.DesignMode
        /// </summary>
        /// <param name="ctrl">Control to check if in design mode</param>
        /// <returns>True if in design mode, false otherwise</returns>
        /// <remarks>See: http://stackoverflow.com/questions/34664/designmode-with-controls </remarks>
        public static bool InDesignMode(this Control ctrl)
        {
            //only reliable check for inside constructor
            if( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
                return true;

            //only reliable check for outside constructor; similar to official implementation
            while( ctrl != null )
            {
                if( (ctrl.Site != null) && ctrl.Site.DesignMode )
                    return true;
                ctrl = ctrl.Parent;
            }
            return false;
        }

        /// <summary>
        /// Determines if a Rectangle intersects a screen area
        /// </summary>
        /// <param name="rect">The rectangle that may intersect a screen area</param>
        /// <returns>True if the rectangle intersects a screen area, false otherwise</returns>
        public static bool IsVisibleOnAnyScreen(this Rectangle rect)
        {
            foreach( var screen in Screen.AllScreens )
            {
                if( screen.WorkingArea.IntersectsWith(rect) )
                    return true;
            }

            return false;
        }
    }
}
