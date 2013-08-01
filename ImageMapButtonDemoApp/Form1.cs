using System;
using System.Drawing;
using System.Windows.Forms;
using SpencerHakim.Windows.Forms;

namespace ImageMapButtonDemoApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //enable repeated clicks from holding down
            this.imageMapButton.MultiClick = true;

            //keeps hard edges, otherwise resizing this demo makes things look like shit
            this.imageMapButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.imageMapButton.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            //add some areas
            this.imageMapButton.AddMap( new ImageMapButtonArea("Upper Left",  Color.Red,    new Point(0, 0),   new Size(64, 64)) );
            this.imageMapButton.AddMap( new ImageMapButtonArea("Upper Right", Color.Lime,   new Point(64, 0),  new Size(64, 64)) );
            this.imageMapButton.AddMap( new ImageMapButtonArea(null,          Color.Blue,   new Point(0, 64),  new Size(64, 64)) ); //no tooltip text for this area
            this.imageMapButton.AddMap( new ImageMapButtonArea("Lower Right", Color.Yellow, new Point(64, 64), new Size(64, 64), true) );

            //disable the Upper Left button
            this.imageMapButton[Color.Red].Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.imageMapButton.ClearMaps();
        }

        private void imageMapButton1_ButtonClicked(object sender, SpencerHakim.Windows.Forms.ButtonClickedEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " - Pressed: " + e.Area);
        }
    }
}