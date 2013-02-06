namespace DWMThumbnailDemoApp
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
            this.dwmThumbnail1 = new SpencerHakim.Windows.Forms.DWMThumbnail();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dwmThumbnail1
            // 
            this.dwmThumbnail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dwmThumbnail1.Location = new System.Drawing.Point(0, 0);
            this.dwmThumbnail1.Name = "dwmThumbnail1";
            this.dwmThumbnail1.Size = new System.Drawing.Size(838, 654);
            this.dwmThumbnail1.TabIndex = 0;
            this.dwmThumbnail1.Text = "dwmThumbnail1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dwmThumbnail1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(292, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(838, 654);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(274, 39);
            this.label1.TabIndex = 2;
            this.label1.Text = "We need to set the destination to the top level window,\r\nand draw relative to it," +
    " not the control. This is a good test\r\nof that.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 654);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SpencerHakim.Windows.Forms.DWMThumbnail dwmThumbnail1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}

