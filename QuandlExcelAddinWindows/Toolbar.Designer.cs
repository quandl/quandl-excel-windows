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
            this.QuandlTab = this.Factory.CreateRibbonTab();
            this.DataGroup = this.Factory.CreateRibbonGroup();
            this.SettingsGroup = this.Factory.CreateRibbonGroup();
            this.prototypeGroup = this.Factory.CreateRibbonGroup();
            this.udf_builder = this.Factory.CreateRibbonButton();
            this.login = this.Factory.CreateRibbonButton();
            this.openQuandlSettings = this.Factory.CreateRibbonButton();
            this.AboutButton = this.Factory.CreateRibbonButton();
            this.GetDataButton = this.Factory.CreateRibbonButton();
            this.button1 = this.Factory.CreateRibbonButton();
            this.refreshMulti = this.Factory.CreateRibbonSplitButton();
            this.refreshWorkbook = this.Factory.CreateRibbonButton();
            this.refreshWorksheet = this.Factory.CreateRibbonButton();
            this.QuandlTab.SuspendLayout();
            this.DataGroup.SuspendLayout();
            this.SettingsGroup.SuspendLayout();
            this.prototypeGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // QuandlTab
            // 
            this.QuandlTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.QuandlTab.Groups.Add(this.DataGroup);
            this.QuandlTab.Groups.Add(this.SettingsGroup);
            this.QuandlTab.Groups.Add(this.prototypeGroup);
            this.QuandlTab.Label = "Quandl";
            this.QuandlTab.Name = "QuandlTab";
            // 
            // DataGroup
            // 
            this.DataGroup.Items.Add(this.udf_builder);
            this.DataGroup.Items.Add(this.refreshMulti);
            this.DataGroup.Label = "Data";
            this.DataGroup.Name = "DataGroup";
            // 
            // SettingsGroup
            // 
            this.SettingsGroup.Items.Add(this.login);
            this.SettingsGroup.Items.Add(this.openQuandlSettings);
            this.SettingsGroup.Items.Add(this.AboutButton);
            this.SettingsGroup.Name = "SettingsGroup";
            // 
            // prototypeGroup
            // 
            this.prototypeGroup.Items.Add(this.GetDataButton);
            this.prototypeGroup.Items.Add(this.button1);
            this.prototypeGroup.Label = "Prototype";
            this.prototypeGroup.Name = "prototypeGroup";
            this.prototypeGroup.Visible = false;
            // 
            // udf_builder
            // 
            this.udf_builder.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.udf_builder.Label = "UDF Creator";
            this.udf_builder.Name = "udf_builder";
            this.udf_builder.OfficeImageId = "ChartShowData";
            this.udf_builder.ShowImage = true;
            this.udf_builder.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.udfBuilder_Click);
            // 
            // login
            // 
            this.login.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.login.Label = "Login";
            this.login.Name = "login";
            this.login.OfficeImageId = "ContactPictureMenu";
            this.login.ShowImage = true;
            this.login.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.login_Click);
            // 
            // openQuandlSettings
            // 
            this.openQuandlSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.openQuandlSettings.Label = "Settings";
            this.openQuandlSettings.Name = "openQuandlSettings";
            this.openQuandlSettings.OfficeImageId = "TableSharePointListsModifyColumnsAndSettings";
            this.openQuandlSettings.ShowImage = true;
            this.openQuandlSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.openQuandlSettings_Click);
            // 
            // AboutButton
            // 
            this.AboutButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.AboutButton.Image = global::Quandl.Excel.Addin.Properties.Resources.Quandl_Icon_Image;
            this.AboutButton.Label = "About";
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.ShowImage = true;
            this.AboutButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AboutButton_Click);
            // 
            // GetDataButton
            // 
            this.GetDataButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.GetDataButton.Image = global::Quandl.Excel.Addin.Properties.Resources.Quandl_Icon_Image;
            this.GetDataButton.Label = "Task Pane";
            this.GetDataButton.Name = "GetDataButton";
            this.GetDataButton.ShowImage = true;
            this.GetDataButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.GetDataButton_Click);
            // 
            // button1
            // 
            this.button1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button1.Image = global::Quandl.Excel.Addin.Properties.Resources.Quandl_Icon_Image;
            this.button1.Label = "Popup";
            this.button1.Name = "button1";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // refreshMulti
            // 
            this.refreshMulti.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.refreshMulti.Items.Add(this.refreshWorkbook);
            this.refreshMulti.Items.Add(this.refreshWorksheet);
            this.refreshMulti.Label = "Refresh";
            this.refreshMulti.Name = "refreshMulti";
            this.refreshMulti.OfficeImageId = "Refresh";
            this.refreshMulti.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.refreshMulti_Click);
            // 
            // refreshWorkbook
            // 
            this.refreshWorkbook.Label = "Workbook";
            this.refreshWorkbook.Name = "refreshWorkbook";
            this.refreshWorkbook.ShowImage = true;
            // 
            // refreshWorksheet
            // 
            this.refreshWorksheet.Label = "Worksheet";
            this.refreshWorksheet.Name = "refreshWorksheet";
            this.refreshWorksheet.ShowImage = true;
            // 
            // Toolbar
            // 
            this.Name = "Toolbar";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.QuandlTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon2_Load);
            this.QuandlTab.ResumeLayout(false);
            this.QuandlTab.PerformLayout();
            this.DataGroup.ResumeLayout(false);
            this.DataGroup.PerformLayout();
            this.SettingsGroup.ResumeLayout(false);
            this.SettingsGroup.PerformLayout();
            this.prototypeGroup.ResumeLayout(false);
            this.prototypeGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab QuandlTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup DataGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton GetDataButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AboutButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton login;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup SettingsGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton openQuandlSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton udf_builder;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup prototypeGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton refreshMulti;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton refreshWorkbook;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton refreshWorksheet;
    }

    partial class ThisRibbonCollection
    {
        internal Toolbar Ribbon2
        {
            get { return this.GetRibbon<Toolbar>(); }
        }
    }
}
