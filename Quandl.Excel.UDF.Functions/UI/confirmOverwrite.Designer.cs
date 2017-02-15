namespace Quandl.Excel.UDF.Functions.UI
{
    partial class confirmOverwrite
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
            this.checkShow = new System.Windows.Forms.CheckBox();
            this.warningPicture = new System.Windows.Forms.PictureBox();
            this.warningMessage = new System.Windows.Forms.Label();
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.warningPicture)).BeginInit();
            this.groupOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkShow
            // 
            this.checkShow.AutoSize = true;
            this.checkShow.Location = new System.Drawing.Point(24, 34);
            this.checkShow.Name = "checkShow";
            this.checkShow.Size = new System.Drawing.Size(175, 17);
            this.checkShow.TabIndex = 0;
            this.checkShow.Text = "Don\'t show this message again.";
            this.checkShow.UseVisualStyleBackColor = true;
            this.checkShow.CheckedChanged += new System.EventHandler(this.checkShow_CheckedChanged);
            // 
            // warningPicture
            // 
            this.warningPicture.ImageLocation = "";
            this.warningPicture.Location = new System.Drawing.Point(24, 32);
            this.warningPicture.Name = "warningPicture";
            this.warningPicture.Size = new System.Drawing.Size(38, 41);
            this.warningPicture.TabIndex = 1;
            this.warningPicture.TabStop = false;
            // 
            // warningMessage
            // 
            this.warningMessage.AutoSize = true;
            this.warningMessage.Location = new System.Drawing.Point(68, 32);
            this.warningMessage.Name = "warningMessage";
            this.warningMessage.Size = new System.Drawing.Size(325, 26);
            this.warningMessage.TabIndex = 3;
            this.warningMessage.Text = "The cells you are trying to update may contain data already. Do you\r\nwant to cont" +
    "inue?";
            // 
            // yesButton
            // 
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.Location = new System.Drawing.Point(255, 30);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(74, 23);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // noButton
            // 
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.noButton.Location = new System.Drawing.Point(347, 30);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(72, 23);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // groupOptions
            // 
            this.groupOptions.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.groupOptions.Controls.Add(this.checkShow);
            this.groupOptions.Controls.Add(this.noButton);
            this.groupOptions.Controls.Add(this.yesButton);
            this.groupOptions.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupOptions.Location = new System.Drawing.Point(-12, 78);
            this.groupOptions.Margin = new System.Windows.Forms.Padding(0);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(454, 79);
            this.groupOptions.TabIndex = 5;
            this.groupOptions.TabStop = false;
            // 
            // confirmOverwrite
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.noButton;
            this.ClientSize = new System.Drawing.Size(431, 144);
            this.ControlBox = false;
            this.Controls.Add(this.groupOptions);
            this.Controls.Add(this.warningMessage);
            this.Controls.Add(this.warningPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "confirmOverwrite";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overwrite Data?";
            ((System.ComponentModel.ISupportInitialize)(this.warningPicture)).EndInit();
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkShow;
        private System.Windows.Forms.PictureBox warningPicture;
        private System.Windows.Forms.Label warningMessage;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.GroupBox groupOptions;
    }
}