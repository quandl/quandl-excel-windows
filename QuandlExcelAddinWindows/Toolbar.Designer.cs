namespace Quandl.Excel.Addin
{
    partial class Toolbar : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Toolbar()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbar));
            this.tab1 = this.Factory.CreateRibbonTab();
            this.Data = this.Factory.CreateRibbonGroup();
            this.GetDataButton = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.AboutButton = this.Factory.CreateRibbonButton();
            this.button1 = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.Data.SuspendLayout();
            this.group2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.Data);
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "Quandl";
            this.tab1.Name = "tab1";
            // 
            // Data
            // 
            this.Data.Items.Add(this.GetDataButton);
            this.Data.Items.Add(this.button1);
            this.Data.Label = "Data";
            this.Data.Name = "Data";
            // 
            // GetDataButton
            // 
            this.GetDataButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.GetDataButton.Image = ((System.Drawing.Image)(resources.GetObject("GetDataButton.Image")));
            this.GetDataButton.Label = "Task Pane";
            this.GetDataButton.Name = "GetDataButton";
            this.GetDataButton.ShowImage = true;
            this.GetDataButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.GetDataButton_Click);
            // 
            // group2
            // 
            this.group2.Items.Add(this.AboutButton);
            this.group2.Name = "group2";
            // 
            // AboutButton
            // 
            this.AboutButton.Label = "About";
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AboutButton_Click);
            // 
            // button1
            // 
            this.button1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Label = "Popup";
            this.button1.Name = "button1";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // Toolbar
            // 
            this.Name = "Toolbar";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon2_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.Data.ResumeLayout(false);
            this.Data.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Data;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton GetDataButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AboutButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
    }

    partial class ThisRibbonCollection
    {
        internal Toolbar Ribbon2
        {
            get { return this.GetRibbon<Toolbar>(); }
        }
    }
}
