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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dwmThumbnail1 = new SpencerHakim.Windows.Forms.DWMThumbnail();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.dwmThumbnail1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(278, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(852, 654);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 58);
            this.label1.TabIndex = 2;
            this.label1.Text = "We need to set the destination to the top level window, and draw relative to it, " +
    "not the control. This is a good test of that.";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(278, 654);
            this.panel2.TabIndex = 3;
            // 
            // dwmThumbnail1
            // 
            this.dwmThumbnail1.BackColor = System.Drawing.Color.Magenta;
            this.dwmThumbnail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dwmThumbnail1.Location = new System.Drawing.Point(0, 0);
            this.dwmThumbnail1.Name = "dwmThumbnail1";
            this.dwmThumbnail1.Size = new System.Drawing.Size(852, 654);
            this.dwmThumbnail1.TabIndex = 0;
            this.dwmThumbnail1.Text = "dwmThumbnail1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 654);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SpencerHakim.Windows.Forms.DWMThumbnail dwmThumbnail1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
    }
}

