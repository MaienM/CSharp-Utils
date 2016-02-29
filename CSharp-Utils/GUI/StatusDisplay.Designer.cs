namespace CSharpUtils.GUI
{
    partial class StatusDisplay
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
            this.tvStatus = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvStatus
            // 
            this.tvStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvStatus.Location = new System.Drawing.Point(0, 0);
            this.tvStatus.Name = "tvStatus";
            this.tvStatus.Size = new System.Drawing.Size(284, 761);
            this.tvStatus.TabIndex = 0;
            // 
            // StatusDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 761);
            this.ControlBox = false;
            this.Controls.Add(this.tvStatus);
            this.Location = new System.Drawing.Point(1000, 0);
            this.Name = "StatusDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "StatusDisplay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvStatus;
    }
}