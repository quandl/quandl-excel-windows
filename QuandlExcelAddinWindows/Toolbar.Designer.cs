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
            this.udf_builder = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.refreshMulti = this.Factory.CreateRibbonSplitButton();
            this.refreshWorksheet = this.Factory.CreateRibbonButton();
            this.refreshWorkbook = this.Factory.CreateRibbonButton();
            this.btnStopAll = this.Factory.CreateRibbonButton();
            this.btnExecutionToggle = this.Factory.CreateRibbonButton();
            this.SettingsGroup = this.Factory.CreateRibbonGroup();
            this.openQuandlSettings = this.Factory.CreateRibbonButton();
            this.AboutButton = this.Factory.CreateRibbonButton();
            this.QuandlTab.SuspendLayout();
            this.DataGroup.SuspendLayout();
            this.group1.SuspendLayout();
            this.SettingsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // QuandlTab
            // 
            this.QuandlTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.QuandlTab.Groups.Add(this.DataGroup);
            this.QuandlTab.Groups.Add(this.group1);
            this.QuandlTab.Groups.Add(this.SettingsGroup);
            this.QuandlTab.Label = "Quandl";
            this.QuandlTab.Name = "QuandlTab";
            // 
            // DataGroup
            // 
            this.DataGroup.Items.Add(this.udf_builder);
            this.DataGroup.Name = "DataGroup";
            // 
            // udf_builder
            // 
            this.udf_builder.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.udf_builder.Label = "Get Data";
            this.udf_builder.Name = "udf_builder";
            this.udf_builder.OfficeImageId = "ChartShowData";
            this.udf_builder.ShowImage = true;
            this.udf_builder.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.udfBuilder_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.refreshMulti);
            this.group1.Items.Add(this.btnStopAll);
            this.group1.Items.Add(this.btnExecutionToggle);
            this.group1.Name = "group1";
            // 
            // refreshMulti
            // 
            this.refreshMulti.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.refreshMulti.Items.Add(this.refreshWorksheet);
            this.refreshMulti.Items.Add(this.refreshWorkbook);
            this.refreshMulti.Label = "Refresh";
            this.refreshMulti.Name = "refreshMulti";
            this.refreshMulti.OfficeImageId = "Refresh";
            this.refreshMulti.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.refreshMulti_Click);
            // 
            // refreshWorksheet
            // 
            this.refreshWorksheet.Label = "Worksheet";
            this.refreshWorksheet.Name = "refreshWorksheet";
            this.refreshWorksheet.ShowImage = true;
            this.refreshWorksheet.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.refreshWorksheet_Click);
            // 
            // refreshWorkbook
            // 
            this.refreshWorkbook.Label = "Workbook";
            this.refreshWorkbook.Name = "refreshWorkbook";
            this.refreshWorkbook.ShowImage = true;
            this.refreshWorkbook.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.refreshWorkbook_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnStopAll.Label = "Stop";
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.OfficeImageId = "InkDeleteAllInk";
            this.btnStopAll.ShowImage = true;
            this.btnStopAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStopAll_Click);
            // 
            // btnExecutionToggle
            // 
            this.btnExecutionToggle.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnExecutionToggle.Label = "Enable Formulas";
            this.btnExecutionToggle.Name = "btnExecutionToggle";
            this.btnExecutionToggle.ShowImage = true;
            this.btnExecutionToggle.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExecutionToggle_Click);
            // 
            // SettingsGroup
            // 
            this.SettingsGroup.Items.Add(this.openQuandlSettings);
            this.SettingsGroup.Items.Add(this.AboutButton);
            this.SettingsGroup.Name = "SettingsGroup";
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
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.SettingsGroup.ResumeLayout(false);
            this.SettingsGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab QuandlTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup DataGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AboutButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup SettingsGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton openQuandlSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton udf_builder;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton refreshMulti;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton refreshWorkbook;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton refreshWorksheet;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnStopAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExecutionToggle;
    }

    partial class ThisRibbonCollection
    {
        internal Toolbar Ribbon2
        {
            get { return this.GetRibbon<Toolbar>(); }
        }
    }
}
