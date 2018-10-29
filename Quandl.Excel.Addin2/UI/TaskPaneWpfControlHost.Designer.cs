namespace Quandl.Excel.Addin.UI
{
    partial class TaskPaneWpfControlHost
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
            this.WpfElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // wpfElementHost
            // 
            this.WpfElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WpfElementHost.Location = new System.Drawing.Point(0, 0);
            this.WpfElementHost.Name = "WpfElementHost";
            this.WpfElementHost.Size = new System.Drawing.Size(284, 261);
            this.WpfElementHost.TabIndex = 0;
            this.WpfElementHost.Child = null;
            // 
            // TaskPaneWpfControlHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.WpfElementHost);
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Name = "TaskPaneWpfControlHost";
            this.Size = new System.Drawing.Size(284, 261);
            this.ResumeLayout(false);

        }

        #endregion
    }
}