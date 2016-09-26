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
            this.btnRefreshSheet = this.Factory.CreateRibbonButton();
            this.btnRefreshWorkbook = this.Factory.CreateRibbonButton();
            this.btnStopAll = this.Factory.CreateRibbonButton();
            this.btnFormulaToggleSplit = this.Factory.CreateRibbonSplitButton();
            this.btnEnableFormula = this.Factory.CreateRibbonButton();
            this.btnDisableFormula = this.Factory.CreateRibbonButton();
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
            this.udf_builder.Image = global::Quandl.Excel.Addin.Properties.Resources.get_data;
            this.udf_builder.Label = "Get Data";
            this.udf_builder.Name = "udf_builder";
            this.udf_builder.OfficeImageId = "ChartShowData";
            this.udf_builder.ShowImage = true;
            this.udf_builder.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.udfBuilder_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.btnRefreshSheet);
            this.group1.Items.Add(this.btnRefreshWorkbook);
            this.group1.Items.Add(this.btnStopAll);
            this.group1.Items.Add(this.btnFormulaToggleSplit);
            this.group1.Name = "group1";
            // 
            // btnRefreshSheet
            // 
            this.btnRefreshSheet.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRefreshSheet.Image = global::Quandl.Excel.Addin.Properties.Resources.refresh_sheet;
            this.btnRefreshSheet.Label = "Refresh Sheet";
            this.btnRefreshSheet.Name = "btnRefreshSheet";
            this.btnRefreshSheet.OfficeImageId = "InkDeleteAllInk";
            this.btnRefreshSheet.ShowImage = true;
            this.btnRefreshSheet.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRefreshWorkSheet_Click);
            // 
            // btnRefreshWorkbook
            // 
            this.btnRefreshWorkbook.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRefreshWorkbook.Image = global::Quandl.Excel.Addin.Properties.Resources.refresh_workbook;
            this.btnRefreshWorkbook.Label = "Refresh Workbook";
            this.btnRefreshWorkbook.Name = "btnRefreshWorkbook";
            this.btnRefreshWorkbook.OfficeImageId = "InkDeleteAllInk";
            this.btnRefreshWorkbook.ShowImage = true;
            this.btnRefreshWorkbook.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRefreshWorkbook_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnStopAll.Image = global::Quandl.Excel.Addin.Properties.Resources.stop;
            this.btnStopAll.Label = "Stop";
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.OfficeImageId = "InkDeleteAllInk";
            this.btnStopAll.ShowImage = true;
            this.btnStopAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStopAll_Click);
            // 
            // btnFormulaToggleSplit
            // 
            this.btnFormulaToggleSplit.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnFormulaToggleSplit.Image = global::Quandl.Excel.Addin.Properties.Resources.formulas_enabled;
            this.btnFormulaToggleSplit.Items.Add(this.btnEnableFormula);
            this.btnFormulaToggleSplit.Items.Add(this.btnDisableFormula);
            this.btnFormulaToggleSplit.Label = "Formulas";
            this.btnFormulaToggleSplit.Name = "btnFormulaToggleSplit";
            this.btnFormulaToggleSplit.OfficeImageId = "Refresh";
            // 
            // btnEnableFormula
            // 
            this.btnEnableFormula.Image = global::Quandl.Excel.Addin.Properties.Resources.enable;
            this.btnEnableFormula.Label = "Enable";
            this.btnEnableFormula.Name = "btnEnableFormula";
            this.btnEnableFormula.ShowImage = true;
            this.btnEnableFormula.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnEnableFormula_Click);
            // 
            // btnDisableFormula
            // 
            this.btnDisableFormula.Image = global::Quandl.Excel.Addin.Properties.Resources.disable;
            this.btnDisableFormula.Label = "Disable";
            this.btnDisableFormula.Name = "btnDisableFormula";
            this.btnDisableFormula.ShowImage = true;
            this.btnDisableFormula.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDisableFormula_Click);
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
            this.openQuandlSettings.Image = global::Quandl.Excel.Addin.Properties.Resources.settings;
            this.openQuandlSettings.Label = "Settings";
            this.openQuandlSettings.Name = "openQuandlSettings";
            this.openQuandlSettings.OfficeImageId = "TableSharePointListsModifyColumnsAndSettings";
            this.openQuandlSettings.ShowImage = true;
            this.openQuandlSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.openQuandlSettings_Click);
            // 
            // AboutButton
            // 
            this.AboutButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.AboutButton.Image = global::Quandl.Excel.Addin.Properties.Resources.quandl;
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
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnStopAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRefreshSheet;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRefreshWorkbook;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton btnFormulaToggleSplit;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnEnableFormula;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDisableFormula;
    }

    partial class ThisRibbonCollection
    {
        internal Toolbar Ribbon2
        {
            get { return this.GetRibbon<Toolbar>(); }
        }
    }
}
