﻿namespace DWMThumbnailDemoApp
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
            this.SuspendLayout();
            // 
            // dwmThumbnail1
            // 
            this.dwmThumbnail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dwmThumbnail1.Location = new System.Drawing.Point(0, 0);
            this.dwmThumbnail1.Name = "dwmThumbnail1";
            this.dwmThumbnail1.Size = new System.Drawing.Size(438, 340);
// TODO: Code generation for '' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Consider using CodeObjectCreateExpression.'.
            this.dwmThumbnail1.TabIndex = 0;
            this.dwmThumbnail1.Text = "dwmThumbnail1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 340);
            this.Controls.Add(this.dwmThumbnail1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SpencerHakim.Windows.Forms.DWMThumbnail dwmThumbnail1;
    }
}

