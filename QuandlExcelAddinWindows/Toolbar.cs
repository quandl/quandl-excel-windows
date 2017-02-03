using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Octokit;
using Quandl.Excel.Addin.Controls;
using Quandl.Excel.Addin.UI;
using Quandl.Excel.Addin.UI.Settings;
using Quandl.Excel.Addin.UI.UDF_Builder;
using Quandl.Shared;
using Quandl.Shared.Errors;
using Brushes = System.Windows.Media.Brushes;
using MessageBox = System.Windows.MessageBox;
using System.Collections.Generic;

namespace Quandl.Excel.Addin
{
    public partial class Toolbar
    {
        private Dictionary<int, TaskPaneControl> _builderPane = new Dictionary<int, TaskPaneControl>();
        private Dictionary<int,TaskPaneControl> _settingsPane = new Dictionary<int, TaskPaneControl>();
        private Shared.Helpers.Updater _updater = new Shared.Helpers.Updater();
        private int winID;

        private void Ribbon2_Load(object sender, RibbonUIEventArgs e)
        {
            SetExecutionToggleIcon();
        }

        private void AboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            new TaskPaneControl(new About(), "About").Show();
        }

        private void openQuandlSettings_Click(object sender, RibbonControlEventArgs e)
        {
            winID = Globals.ThisAddIn.Application.Hwnd;

            if (!_settingsPane.ContainsKey(winID))
            {
                _settingsPane[winID] = new TaskPaneControl(new Settings(), "Settings");
            }
            _settingsPane[winID].Show();
        }

        private void udfBuilder_Click(object sender, RibbonControlEventArgs e)
        {
            winID = Globals.ThisAddIn.Application.Hwnd;
            if (!_builderPane.ContainsKey(winID))
            {
                WizardGuide _guideChild = new WizardGuide();
                _builderPane[winID] = new TaskPaneControl(_guideChild, "Quandl Formula Builder");
                _guideChild.Reset();
                _guideChild.Background = Brushes.White;
                _guideChild.Margin = new Thickness(0);
                _guideChild.Padding = new Thickness(0);
            }
           
            _builderPane[winID].Show();
        }

        private void btnStopAll_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.StopCurrentExecution = true;
        }

        public void SetExecutionToggleIcon()
        {
            if (QuandlConfig.PreventCurrentExecution)
            {
                btnFormulaToggleSplit.Image = Properties.Resources.formulas_disabled;
                btnEnableFormula.Enabled = true;
                btnDisableFormula.Enabled = false;
            }
            else
            {
                btnFormulaToggleSplit.Image = Properties.Resources.formulas_enabled;
                btnEnableFormula.Enabled = false;
                btnDisableFormula.Enabled = true;
            }
            if (QuandlConfig.CheckUpdateAtStart)
                CheckUpdate();
        }

        public void CloseBuilder()
        {
            _builderPane[winID].Close();
        }

        private void btnRefreshWorkSheet_Click(object sender, RibbonControlEventArgs e)
        {
            var activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            try
            {
                FunctionUpdater.RecalculateQuandlFunctions(activeSheet);
            }
            catch (MissingFormulaException ex)
            {
                Globals.ThisAddIn.UpdateStatusBar(ex);
            }
        }

        private void btnRefreshWorkbook_Click(object sender, RibbonControlEventArgs e)
        {
            var activeWorkBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            try
            {
                FunctionUpdater.RecalculateQuandlFunctions(activeWorkBook);
            }
            catch (MissingFormulaException ex)
            {
                Globals.ThisAddIn.UpdateStatusBar(ex);
            }
        }

        private void btnEnableFormula_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.PreventCurrentExecution = !QuandlConfig.PreventCurrentExecution;
            SetExecutionToggleIcon();
        }

        private void btnDisableFormula_Click(object sender, RibbonControlEventArgs e)
        {
            QuandlConfig.PreventCurrentExecution = !QuandlConfig.PreventCurrentExecution;
            SetExecutionToggleIcon();
        }

        private void btnCheckUpdate_Click(object sender, RibbonControlEventArgs e)
        {
            new TaskPaneControl(new Update(_updater), "New Updates Available!").Show();
        }

        private void CheckUpdate()
        {
            if (_updater.UpdateAvailable)
            {
                btnCheckUpdate.Visible = true;
            }
        }
    }
}