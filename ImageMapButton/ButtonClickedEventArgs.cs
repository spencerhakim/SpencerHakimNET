using System;
using System.Windows.Forms;

namespace SpencerHakim.Windows.Forms
{
    public class ButtonClickedEventArgs : MouseEventArgs
    {
        public ImageMapButtonArea Area { get; private set; }

        public ButtonClickedEventArgs(MouseEventArgs e, ImageMapButtonArea area)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            this.Area = area;
        }
    }
}