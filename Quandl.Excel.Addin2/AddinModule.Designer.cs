namespace Quandl.Excel.Addin
{
    partial class AddinModule
    {
        /// <summary>
        /// Required by designer
        /// </summary>
        private System.ComponentModel.IContainer components;
 
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
        /// Required by designer support - do not modify
        /// the following method
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddinModule));
            this.adxTaskPaneAbout = new AddinExpress.MSO.ADXTaskPane(this.components);
            this.adxRibbonTabQuandl = new AddinExpress.MSO.ADXRibbonTab(this.components);
            this.adxRibbonTabQuandlData = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxRibbonTabQuandlUdfBuilder = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.imageList32 = new System.Windows.Forms.ImageList(this.components);
            this.adxRibbonTabQuandlGroup1 = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxRibbonTabQuandlRefreshSheet = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlRefreshWorkbook = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlStopAll = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlFormulaToggleSplit = new AddinExpress.MSO.ADXRibbonSplitButton(this.components);
            this.adxRibbonTabQuandlFormulaToggleSplitMenu = new AddinExpress.MSO.ADXRibbonMenu(this.components);
            this.adxRibbonTabQuandlFormulaToggleSplitEnable = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlFormulaToggleSplitDisable = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlGroupSettings = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxRibbonTabQuandlOpenSettings = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabQuandlAbout = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonTabCheckUpdate = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxExcelAppEvents1 = new AddinExpress.MSO.ADXExcelAppEvents(this.components);
            this.timerCheckUI = new System.Windows.Forms.Timer(this.components);
            this.adxTaskPaneSettings = new AddinExpress.MSO.ADXTaskPane(this.components);
            this.adxTaskPaneBuilder = new AddinExpress.MSO.ADXTaskPane(this.components);
            this.adxTaskPaneUpdater = new AddinExpress.MSO.ADXTaskPane(this.components);
            // 
            // adxTaskPaneAbout
            // 
            this.adxTaskPaneAbout.ControlProgID = "QuandlExcelAddin2.AboutControlHost";
            this.adxTaskPaneAbout.DockPosition = AddinExpress.MSO.ADXCTPDockPosition.ctpDockPositionFloating;
            this.adxTaskPaneAbout.DockPositionRestrict = AddinExpress.MSO.ADXCTPDockPositionRestrict.ctpDockPositionRestrictNoHorizontal;
            this.adxTaskPaneAbout.Height = 400;
            this.adxTaskPaneAbout.Title = "About";
            this.adxTaskPaneAbout.Visible = false;
            this.adxTaskPaneAbout.Width = 600;
            // 
            // adxRibbonTabQuandl
            // 
            this.adxRibbonTabQuandl.Caption = "Quandl";
            this.adxRibbonTabQuandl.Controls.Add(this.adxRibbonTabQuandlData);
            this.adxRibbonTabQuandl.Controls.Add(this.adxRibbonTabQuandlGroup1);
            this.adxRibbonTabQuandl.Controls.Add(this.adxRibbonTabQuandlGroupSettings);
            this.adxRibbonTabQuandl.Id = "adxRibbonTab_81dd1d96a50545f0ace452ce59582487";
            this.adxRibbonTabQuandl.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // adxRibbonTabQuandlData
            // 
            this.adxRibbonTabQuandlData.Caption = "Data";
            this.adxRibbonTabQuandlData.Controls.Add(this.adxRibbonTabQuandlUdfBuilder);
            this.adxRibbonTabQuandlData.Id = "adxRibbonGroup_d6e77ea1973841659b5cc00961273379";
            this.adxRibbonTabQuandlData.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlData.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // adxRibbonTabQuandlUdfBuilder
            // 
            this.adxRibbonTabQuandlUdfBuilder.Caption = "Get Data";
            this.adxRibbonTabQuandlUdfBuilder.Id = "adxRibbonButton_ca447a63f5804c7485e187f8aed36d8c";
            this.adxRibbonTabQuandlUdfBuilder.Image = 0;
            this.adxRibbonTabQuandlUdfBuilder.ImageList = this.imageList32;
            this.adxRibbonTabQuandlUdfBuilder.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlUdfBuilder.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlUdfBuilder.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlUdfBuilder.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlUdfBuilder_OnClick);
            // 
            // imageList32
            // 
            this.imageList32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList32.ImageStream")));
            this.imageList32.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList32.Images.SetKeyName(0, "get_data.png");
            this.imageList32.Images.SetKeyName(1, "refresh_sheet.png");
            this.imageList32.Images.SetKeyName(2, "refresh_workbook.png");
            this.imageList32.Images.SetKeyName(3, "disable.png");
            this.imageList32.Images.SetKeyName(4, "enable.png");
            this.imageList32.Images.SetKeyName(5, "FileSave.png");
            this.imageList32.Images.SetKeyName(6, "formulas_disabled.png");
            this.imageList32.Images.SetKeyName(7, "formulas_enabled.png");
            this.imageList32.Images.SetKeyName(8, "settings.png");
            this.imageList32.Images.SetKeyName(9, "stop.png");
            this.imageList32.Images.SetKeyName(10, "quandl_lq_sqr.png");
            this.imageList32.Images.SetKeyName(11, "update_available.png");
            this.imageList32.Images.SetKeyName(12, "update_check.png");
            // 
            // adxRibbonTabQuandlGroup1
            // 
            this.adxRibbonTabQuandlGroup1.Controls.Add(this.adxRibbonTabQuandlRefreshSheet);
            this.adxRibbonTabQuandlGroup1.Controls.Add(this.adxRibbonTabQuandlRefreshWorkbook);
            this.adxRibbonTabQuandlGroup1.Controls.Add(this.adxRibbonTabQuandlStopAll);
            this.adxRibbonTabQuandlGroup1.Controls.Add(this.adxRibbonTabQuandlFormulaToggleSplit);
            this.adxRibbonTabQuandlGroup1.Id = "adxRibbonGroup_d27f90a681f0437da92b4a3d74a1b231";
            this.adxRibbonTabQuandlGroup1.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlGroup1.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // adxRibbonTabQuandlRefreshSheet
            // 
            this.adxRibbonTabQuandlRefreshSheet.Caption = "Refresh Sheet";
            this.adxRibbonTabQuandlRefreshSheet.Id = "adxRibbonButton_75cbac538b7f40b5a02ff3001d2b4212";
            this.adxRibbonTabQuandlRefreshSheet.Image = 1;
            this.adxRibbonTabQuandlRefreshSheet.ImageList = this.imageList32;
            this.adxRibbonTabQuandlRefreshSheet.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlRefreshSheet.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlRefreshSheet.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlRefreshSheet.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlRefreshSheet_OnClick);
            // 
            // adxRibbonTabQuandlRefreshWorkbook
            // 
            this.adxRibbonTabQuandlRefreshWorkbook.Caption = "Refresh Workbook";
            this.adxRibbonTabQuandlRefreshWorkbook.Id = "adxRibbonButton_c0c0e300091b44e38d21f8d79b92a758";
            this.adxRibbonTabQuandlRefreshWorkbook.Image = 2;
            this.adxRibbonTabQuandlRefreshWorkbook.ImageList = this.imageList32;
            this.adxRibbonTabQuandlRefreshWorkbook.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlRefreshWorkbook.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlRefreshWorkbook.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlRefreshWorkbook.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlRefreshWorkbook_OnClick);
            // 
            // adxRibbonTabQuandlStopAll
            // 
            this.adxRibbonTabQuandlStopAll.Caption = "Stop";
            this.adxRibbonTabQuandlStopAll.Id = "adxRibbonButton_f7303410ea5240548ce5bf6a891263db";
            this.adxRibbonTabQuandlStopAll.Image = 9;
            this.adxRibbonTabQuandlStopAll.ImageList = this.imageList32;
            this.adxRibbonTabQuandlStopAll.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlStopAll.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlStopAll.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlStopAll.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlStopAll_OnClick);
            // 
            // adxRibbonTabQuandlFormulaToggleSplit
            // 
            this.adxRibbonTabQuandlFormulaToggleSplit.Caption = "Formulas";
            this.adxRibbonTabQuandlFormulaToggleSplit.Controls.Add(this.adxRibbonTabQuandlFormulaToggleSplitMenu);
            this.adxRibbonTabQuandlFormulaToggleSplit.Id = "adxRibbonSplitButton_02f5e404eae245e98dd5d5199177db3b";
            this.adxRibbonTabQuandlFormulaToggleSplit.Image = 7;
            this.adxRibbonTabQuandlFormulaToggleSplit.ImageList = this.imageList32;
            this.adxRibbonTabQuandlFormulaToggleSplit.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlFormulaToggleSplit.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlFormulaToggleSplit.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            // 
            // adxRibbonTabQuandlFormulaToggleSplitMenu
            // 
            this.adxRibbonTabQuandlFormulaToggleSplitMenu.Controls.Add(this.adxRibbonTabQuandlFormulaToggleSplitEnable);
            this.adxRibbonTabQuandlFormulaToggleSplitMenu.Controls.Add(this.adxRibbonTabQuandlFormulaToggleSplitDisable);
            this.adxRibbonTabQuandlFormulaToggleSplitMenu.Id = "adxRibbonMenu_73837df7b746499d84c02b16d68de013";
            this.adxRibbonTabQuandlFormulaToggleSplitMenu.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlFormulaToggleSplitMenu.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // adxRibbonTabQuandlFormulaToggleSplitEnable
            // 
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.Caption = "Enable";
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.Id = "adxRibbonButton_580765f609584aea8302c72d28928690";
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.Image = 7;
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.ImageList = this.imageList32;
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlFormulaToggleSplitEnable.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlFormulaToggleSplitEnable_OnClick);
            // 
            // adxRibbonTabQuandlFormulaToggleSplitDisable
            // 
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.Caption = "Disable";
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.Id = "adxRibbonButton_a8b8221b36d548d899423684699edafc";
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.Image = 6;
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.ImageList = this.imageList32;
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlFormulaToggleSplitDisable.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlFormulaToggleSplitDisable_OnClick);
            // 
            // adxRibbonTabQuandlGroupSettings
            // 
            this.adxRibbonTabQuandlGroupSettings.Caption = "Settings";
            this.adxRibbonTabQuandlGroupSettings.Controls.Add(this.adxRibbonTabQuandlOpenSettings);
            this.adxRibbonTabQuandlGroupSettings.Controls.Add(this.adxRibbonTabQuandlAbout);
            this.adxRibbonTabQuandlGroupSettings.Controls.Add(this.adxRibbonTabCheckUpdate);
            this.adxRibbonTabQuandlGroupSettings.Id = "adxRibbonGroup_57b112bb7167472daffd386bf362510f";
            this.adxRibbonTabQuandlGroupSettings.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlGroupSettings.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            // 
            // adxRibbonTabQuandlOpenSettings
            // 
            this.adxRibbonTabQuandlOpenSettings.Caption = "Settings";
            this.adxRibbonTabQuandlOpenSettings.Id = "adxRibbonButton_4e141c5521f64bcfbe5f98ed263af91f";
            this.adxRibbonTabQuandlOpenSettings.Image = 8;
            this.adxRibbonTabQuandlOpenSettings.ImageList = this.imageList32;
            this.adxRibbonTabQuandlOpenSettings.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlOpenSettings.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlOpenSettings.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlOpenSettings.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlOpenSettings_OnClick);
            // 
            // adxRibbonTabQuandlAbout
            // 
            this.adxRibbonTabQuandlAbout.Caption = "About";
            this.adxRibbonTabQuandlAbout.Id = "adxRibbonButton_f8e95f39724e44bda044894787003fcf";
            this.adxRibbonTabQuandlAbout.Image = 10;
            this.adxRibbonTabQuandlAbout.ImageList = this.imageList32;
            this.adxRibbonTabQuandlAbout.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabQuandlAbout.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabQuandlAbout.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabQuandlAbout.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabQuandlAbout_OnClick);
            // 
            // adxRibbonTabCheckUpdate
            // 
            this.adxRibbonTabCheckUpdate.Caption = "Check Update";
            this.adxRibbonTabCheckUpdate.Id = "adxRibbonButton_1ee51f5a76a14574b5c5cb606456ef1f";
            this.adxRibbonTabCheckUpdate.Image = 12;
            this.adxRibbonTabCheckUpdate.ImageList = this.imageList32;
            this.adxRibbonTabCheckUpdate.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonTabCheckUpdate.Ribbons = AddinExpress.MSO.ADXRibbons.msrExcelWorkbook;
            this.adxRibbonTabCheckUpdate.Size = AddinExpress.MSO.ADXRibbonXControlSize.Large;
            this.adxRibbonTabCheckUpdate.Visible = false;
            this.adxRibbonTabCheckUpdate.OnClick += new AddinExpress.MSO.ADXRibbonOnAction_EventHandler(this.adxRibbonTabCheckUpdate_OnClick);
            // 
            // adxExcelAppEvents1
            // 
            this.adxExcelAppEvents1.SheetSelectionChange += new AddinExpress.MSO.ADXExcelSheet_EventHandler(this.adxExcelAppEvents1_SheetSelectionChange);
            this.adxExcelAppEvents1.WorkbookOpen += new AddinExpress.MSO.ADXHostActiveObject_EventHandler(this.adxExcelAppEvents1_WorkbookOpen);
            // 
            // timerCheckUI
            // 
            this.timerCheckUI.Interval = 1000;
            this.timerCheckUI.Tick += new System.EventHandler(this.timerCheckUI_Tick);
            // 
            // adxTaskPaneSettings
            // 
            this.adxTaskPaneSettings.ControlProgID = "QuandlExcelAddin2.SettingsControlHost";
            this.adxTaskPaneSettings.DockPosition = AddinExpress.MSO.ADXCTPDockPosition.ctpDockPositionFloating;
            this.adxTaskPaneSettings.DockPositionRestrict = AddinExpress.MSO.ADXCTPDockPositionRestrict.ctpDockPositionRestrictNoHorizontal;
            this.adxTaskPaneSettings.Height = 600;
            this.adxTaskPaneSettings.Title = "Settings";
            this.adxTaskPaneSettings.Visible = false;
            this.adxTaskPaneSettings.Width = 400;
            // 
            // adxTaskPaneBuilder
            // 
            this.adxTaskPaneBuilder.ControlProgID = "QuandlExcelAddin2.WizardGuideControlHost";
            this.adxTaskPaneBuilder.DockPosition = AddinExpress.MSO.ADXCTPDockPosition.ctpDockPositionFloating;
            this.adxTaskPaneBuilder.DockPositionRestrict = AddinExpress.MSO.ADXCTPDockPositionRestrict.ctpDockPositionRestrictNoHorizontal;
            this.adxTaskPaneBuilder.Height = 540;
            this.adxTaskPaneBuilder.Title = "Quandl Formula Builder";
            this.adxTaskPaneBuilder.Visible = false;
            this.adxTaskPaneBuilder.Width = 700;
            // 
            // adxTaskPaneUpdater
            // 
            this.adxTaskPaneUpdater.ControlProgID = "QuandlExcelAddin2.UpdateControlHost";
            this.adxTaskPaneUpdater.DockPosition = AddinExpress.MSO.ADXCTPDockPosition.ctpDockPositionFloating;
            this.adxTaskPaneUpdater.DockPositionRestrict = AddinExpress.MSO.ADXCTPDockPositionRestrict.ctpDockPositionRestrictNoHorizontal;
            this.adxTaskPaneUpdater.Height = 450;
            this.adxTaskPaneUpdater.Title = "New Updates Available!";
            this.adxTaskPaneUpdater.Visible = false;
            this.adxTaskPaneUpdater.Width = 640;
            // 
            // AddinModule
            // 
            this.AddinName = "Quandl for Excel";
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaExcel;
            this.TaskPanes.Add(this.adxTaskPaneAbout);
            this.TaskPanes.Add(this.adxTaskPaneSettings);
            this.TaskPanes.Add(this.adxTaskPaneBuilder);
            this.TaskPanes.Add(this.adxTaskPaneUpdater);
            this.AddinInitialize += new AddinExpress.MSO.ADXEvents_EventHandler(this.AddinModule_AddinInitialize);
            this.AddinStartupComplete += new AddinExpress.MSO.ADXEvents_EventHandler(this.AddinModule_AddinStartupComplete);
            this.AddinFinalize += new AddinExpress.MSO.ADXEvents_EventHandler(this.AddinModule_AddinFinalize);
            this.AddinBeginShutdown += new AddinExpress.MSO.ADXEvents_EventHandler(this.AddinModule_AddinBeginShutdown);
            this.OnError += new AddinExpress.MSO.ADXError_EventHandler(this.AddinModule_OnError);
            this.OnTaskPaneAfterCreate += new AddinExpress.MSO.ADXTaskPaneAfterCreate_EventHandler(this.AddinModule_OnTaskPaneAfterCreate);

        }
        #endregion
        private AddinExpress.MSO.ADXTaskPane adxTaskPaneAbout;
        private AddinExpress.MSO.ADXRibbonTab adxRibbonTabQuandl;
        private AddinExpress.MSO.ADXRibbonGroup adxRibbonTabQuandlData;
        private AddinExpress.MSO.ADXRibbonGroup adxRibbonTabQuandlGroup1;
        private AddinExpress.MSO.ADXRibbonGroup adxRibbonTabQuandlGroupSettings;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlUdfBuilder;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlRefreshSheet;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlRefreshWorkbook;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlStopAll;
        private System.Windows.Forms.ImageList imageList32;
        private AddinExpress.MSO.ADXRibbonSplitButton adxRibbonTabQuandlFormulaToggleSplit;
        private AddinExpress.MSO.ADXRibbonMenu adxRibbonTabQuandlFormulaToggleSplitMenu;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlFormulaToggleSplitEnable;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlFormulaToggleSplitDisable;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlOpenSettings;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabQuandlAbout;
        private AddinExpress.MSO.ADXRibbonButton adxRibbonTabCheckUpdate;
        private AddinExpress.MSO.ADXExcelAppEvents adxExcelAppEvents1;
        private System.Windows.Forms.Timer timerCheckUI;
        private AddinExpress.MSO.ADXTaskPane adxTaskPaneSettings;
        private AddinExpress.MSO.ADXTaskPane adxTaskPaneBuilder;
        private AddinExpress.MSO.ADXTaskPane adxTaskPaneUpdater;
    }
}

