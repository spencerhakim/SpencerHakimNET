namespace SpencerHakim.Windows.Forms
{
    partial class ImageMapButton
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.Active = false;
            // 
            // ImageMapButton
            // 
            this.EnabledChanged += new System.EventHandler(this.ImageMapButton_EnabledChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageMapButton_MouseDown);
            this.MouseLeave += new System.EventHandler(this.ImageMapButton_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageMapButton_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageMapButton_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;



    }
}
