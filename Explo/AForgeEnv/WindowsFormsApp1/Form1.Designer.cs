namespace WindowsFormsApp1
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
            if (disposing && (components != null))
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
            this.histogram1 = new AForge.Controls.Histogram();
            this.SuspendLayout();
            // 
            // histogram1
            // 
            this.histogram1.Location = new System.Drawing.Point(102, 204);
            this.histogram1.Name = "histogram1";
            this.histogram1.Size = new System.Drawing.Size(1242, 302);
            this.histogram1.TabIndex = 0;
            this.histogram1.Text = "histogram1";
            this.histogram1.Values = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1950, 820);
            this.Controls.Add(this.histogram1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private AForge.Controls.Histogram histogram1;
    }
}

