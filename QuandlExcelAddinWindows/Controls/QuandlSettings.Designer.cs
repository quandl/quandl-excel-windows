namespace Quandl.Excel.Addin.Controls
{
    partial class QuandlSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.autoUpdate = new System.Windows.Forms.CheckBox();
            this.apiKeyTextBox = new System.Windows.Forms.TextBox();
            this.apiKeyLabel = new System.Windows.Forms.Label();
            this.saveSettings = new System.Windows.Forms.Button();
            this.updateFrequency = new System.Windows.Forms.ComboBox();
            this.frequencyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // autoUpdate
            // 
            this.autoUpdate.AutoSize = true;
            this.autoUpdate.Location = new System.Drawing.Point(18, 36);
            this.autoUpdate.Name = "autoUpdate";
            this.autoUpdate.Size = new System.Drawing.Size(86, 17);
            this.autoUpdate.TabIndex = 0;
            this.autoUpdate.Text = "Auto Update";
            this.autoUpdate.UseVisualStyleBackColor = true;
            this.autoUpdate.CheckedChanged += new System.EventHandler(this.autoUpdate_CheckedChanged);
            // 
            // apiKeyTextBox
            // 
            this.apiKeyTextBox.Location = new System.Drawing.Point(18, 254);
            this.apiKeyTextBox.Name = "apiKeyTextBox";
            this.apiKeyTextBox.Size = new System.Drawing.Size(170, 20);
            this.apiKeyTextBox.TabIndex = 1;
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(15, 229);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(43, 13);
            this.apiKeyLabel.TabIndex = 2;
            this.apiKeyLabel.Text = "Api Key";
            // 
            // saveSettings
            // 
            this.saveSettings.Location = new System.Drawing.Point(18, 290);
            this.saveSettings.Name = "saveSettings";
            this.saveSettings.Size = new System.Drawing.Size(109, 23);
            this.saveSettings.TabIndex = 3;
            this.saveSettings.Text = "Save Settings";
            this.saveSettings.UseVisualStyleBackColor = true;
            this.saveSettings.Click += new System.EventHandler(this.saveSettings_Click);
            // 
            // updateFrequency
            // 
            this.updateFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updateFrequency.FormattingEnabled = true;
            this.updateFrequency.Items.AddRange(new object[] {
            "On Open Only",
            "Daily",
            "Weekly"});
            this.updateFrequency.Location = new System.Drawing.Point(18, 103);
            this.updateFrequency.Name = "updateFrequency";
            this.updateFrequency.Size = new System.Drawing.Size(121, 21);
            this.updateFrequency.TabIndex = 4;
            // 
            // frequencyLabel
            // 
            this.frequencyLabel.AutoSize = true;
            this.frequencyLabel.Location = new System.Drawing.Point(18, 84);
            this.frequencyLabel.Name = "frequencyLabel";
            this.frequencyLabel.Size = new System.Drawing.Size(120, 13);
            this.frequencyLabel.TabIndex = 5;
            this.frequencyLabel.Text = "Auto Update Frequency";
            // 
            // QuandlSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.frequencyLabel);
            this.Controls.Add(this.updateFrequency);
            this.Controls.Add(this.saveSettings);
            this.Controls.Add(this.apiKeyLabel);
            this.Controls.Add(this.apiKeyTextBox);
            this.Controls.Add(this.autoUpdate);
            this.Name = "QuandlSettings";
            this.Size = new System.Drawing.Size(237, 331);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autoUpdate;
        private System.Windows.Forms.TextBox apiKeyTextBox;
        private System.Windows.Forms.Label apiKeyLabel;
        private System.Windows.Forms.Button saveSettings;
        private System.Windows.Forms.ComboBox updateFrequency;
        private System.Windows.Forms.Label frequencyLabel;
    }
}
