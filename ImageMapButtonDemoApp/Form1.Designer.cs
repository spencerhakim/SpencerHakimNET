namespace ImageMapButtonDemoApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imageMapButton = new SpencerHakim.Windows.Forms.ImageMapButton();
            this.SuspendLayout();
            // 
            // imageMapButton
            // 
            this.imageMapButton.ChromaKeyFuzziness = 0D;
            this.imageMapButton.ChromaKeyImage = global::ImageMapButtonDemoApp.Properties.Resources.chromaKey;
            this.imageMapButton.DisabledImage = global::ImageMapButtonDemoApp.Properties.Resources.imageDisabled;
            this.imageMapButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageMapButton.Image = global::ImageMapButtonDemoApp.Properties.Resources.image;
            this.imageMapButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.imageMapButton.Location = new System.Drawing.Point(0, 0);
            this.imageMapButton.MouseDownImage = global::ImageMapButtonDemoApp.Properties.Resources.imageDown;
            this.imageMapButton.MouseOverImage = global::ImageMapButtonDemoApp.Properties.Resources.imageOver;
            this.imageMapButton.Name = "imageMapButton";
            this.imageMapButton.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            this.imageMapButton.Size = new System.Drawing.Size(166, 155);
            this.imageMapButton.TabIndex = 0;
            this.imageMapButton.Text = "imageMapButton1";
            this.imageMapButton.MapAreaClicked += new System.EventHandler<SpencerHakim.Windows.Forms.ButtonClickedEventArgs>(this.imageMapButton1_ButtonClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(166, 155);
            this.Controls.Add(this.imageMapButton);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(168, 190);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private SpencerHakim.Windows.Forms.ImageMapButton imageMapButton;
    }
}