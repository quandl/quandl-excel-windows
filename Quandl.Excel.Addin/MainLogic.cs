using AddinExpress.MSO;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quandl.Excel.Addin
{
    sealed class MainLogic  : IDisposable
    {
        public static MainLogic Instance
        {
            get
            {
                return AddinModule.CurrentInstance.Logic;
            }
        }

        public void Dispose()
        {

            
        }

        public void Shutdown()
        {
            Shared.Excel.FunctionGrimReaper.EndReaping();
        }
        public void UpdateStatusBar(System.Exception error)
        {
            Quandl.Shared.Globals.Instance.StatusBar.AddException(error);
        }

        private _Application _excelApp;
        public void Connect(_Application excel)
        {
            _excelApp = excel;
        }
        private _Application ExcelApp
        {
            get { return _excelApp; }
        }

        public void OnSheetSelectionChange()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool SetActiveCellFormula(string formula)
        {
            Range cell = null;
            try
            {
                cell = this.ExcelApp.ActiveCell;
                if (cell != null && cell.Count == 1)
                {
                    cell.Value2 = formula;
                    return true;
                }
            }
            finally
            {
                if (cell != null)
                {
                    Marshal.ReleaseComObject(cell);
                }
            }
            return false;
        }
        private Shared.Helpers.Updater _updater = new Shared.Helpers.Updater();

        public void CloseBuilder()
        {
            taskPaneUpdater.Hide<UI.WizardGuideControlHost>();
        }

        void SetExecutionToggleIcon()
        {
            AddinModule.CurrentInstance.SetExecutionToggleIcon();
        }
        public void OnWorkbookOpen(Workbook wb)
        {
            try
            {
                Shared.QuandlConfig.PreventCurrentExecution = true;
                SetExecutionToggleIcon();



                if (!FunctionUpdater.HasQuandlFormulaInWorkbook(wb)
                    || Shared.QuandlConfig.AutoUpdateFrequency !=
                    Shared.QuandlConfig.AutoUpdateFrequencies.WorkbookOpen)
                {
                    return;
                }

                var result = MessageBox.Show(Properties.Resources.UpdateWorkbookFormulas,
                    Properties.Resources.UpdateCaption,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Shared.QuandlConfig.PreventCurrentExecution = false;
                    SetExecutionToggleIcon();

                    FunctionUpdater.RecalculateQuandlFunctions(wb);
                }
            }
            catch (Exception ex)
            {
                this.UpdateStatusBar(ex);
            }

        }
        public void OnStart()
        {
            Shared.Helpers.HttpHelper.EnableTlsSupport();
            Quandl.Shared.Globals.Instance.HostService = new HostService(this.ExcelApp);
            Task.Run((System.Action)_updater.RunCheck);
        }

        public Shared.Helpers.Updater Updater
        {
            get { return _updater; }
        }
        public bool? UpdateAvailable
        {
            get { return _updater.UpdateAvailable; }
        }

        private readonly TaskPaneUpdater taskPaneUpdater = new TaskPaneUpdater();

        public TaskPaneUpdater TaskPaneUpdater
        {
            get { return taskPaneUpdater; }
        }

        public event EventHandler SelectionChanged;
        public string SelectedCellReference()
        {
            Range target = null;
            Worksheet ws = null;
            try
            {
                target = ExcelApp?.ActiveCell;
                ws = target?.Worksheet;
                if (ws != null)
                {
                    var useAddress = target.Address;
                    // cut away to the first part only
                    if (!string.IsNullOrEmpty(useAddress))
                    {
                        var idxSep = useAddress.IndexOf(':');
                        if (idxSep > 0)
                        {
                            useAddress = useAddress.Substring(0, idxSep);
                        }
                    }

                    return $"{ws.Name}!{useAddress}";
                }
            
            }
            catch (COMException ex)
            {
                // Ignore no cells being selected error.
                if (ex.HResult == Quandl.Shared.Excel.Exception.BAD_REFERENCE)
                {
                    Trace.WriteLine(ex.Message);
                    return null;
                }

                throw;
            }
            finally
            {
                if (target != null)
                {
                    Marshal.ReleaseComObject(target);
                }

                if (ws != null)
                {
                    Marshal.ReleaseComObject(ws);
                }
            }
            return null;
        }
    }

    class TaskPaneUpdater
    {
        private readonly float _scalingFactor = Shared.Utilities.WindowsScalingFactor();
        public void UpdateTaskPane<T>(ADXTaskPane taskPane)
        {
            if (string.IsNullOrEmpty(taskPane.ControlProgID))
            {
                taskPane.ControlProgID = typeof(T).FullName;
            }
            //taskPane.DockPosition = ADXCTPDockPosition.ctpDockPositionFloating;
            //taskPane.DockPositionRestrict = ADXCTPDockPositionRestrict.ctpDockPositionRestrictNoHorizontal;
            taskPane.Width = (int)(taskPane.Width * _scalingFactor);
            taskPane.Height = (int)(taskPane.Height * _scalingFactor);
            registeredPanes.Add(typeof(T), taskPane);
        }

        private readonly System.Collections.Generic.Dictionary<Type, ADXTaskPane> registeredPanes
            = new System.Collections.Generic.Dictionary<Type, ADXTaskPane>();

        public void Show<T>(object context)
        {
            Show<T>(context, null);
        }
        public void Show<T>(object context, Action<T> showAction)
        {
            var pane = registeredPanes[typeof(T)];
            if (pane.DockPosition == ADXCTPDockPosition.ctpDockPositionFloating)
            {
                // move to active screen when possible
                try
                {
                    //pane.Delete(context);
                    pane.Visible = true;
                    //pane.Create(context);
                    var instance = pane[context];
                    if (instance != null)
                    {
                        SetCustomPanePositionWhenFloating(instance, Screen.FromPoint(Cursor.Position));
                    }
                }
                catch // this is unimportant functionality, let it fail without logging
                {
                }
            }
            else
            {
                pane.Visible = true;
            }

            if (showAction != null)
            {
                showAction.Invoke((T) pane[context].Control);
            }
        }

        
        public void Hide<T>()
        {
            var pane = registeredPanes[typeof(T)];
            pane.Visible = false;
        }

        public void SetCustomPanePositionWhenFloating(ADXTaskPane.ADXCustomTaskPaneInstance customTaskPane, object control)
        {
            //var screen = Screen.FromControl((Control)control);
            customTaskPane.Width = customTaskPane.Parent.Width;
            customTaskPane.Height = customTaskPane.Parent.Height;
        }

        void SetCustomPanePositionWhenFloating(ADXTaskPane.ADXCustomTaskPaneInstance customTaskPane, Screen screen)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }

            var area = screen.WorkingArea;
            SetCustomPanePositionWhenFloating(customTaskPane,
                area.Left +  area.Width / 2 - customTaskPane.Width / 2,
                area.Top + area.Height / 2 - customTaskPane.Height / 2);
        }
        // http://stackoverflow.com/questions/6916402/c-excel-addin-cant-reposition-floating-custom-task-pane
        private void SetCustomPanePositionWhenFloating(ADXTaskPane.ADXCustomTaskPaneInstance customTaskPane, int x, int y)
        {
            // only do if pane is not docked
            if (customTaskPane.DockPosition == ADXCTPDockPosition.ctpDockPositionFloating)
            {
                customTaskPane.Visible = true; //The task pane must be visible to set its position

                var window = NativeMethods.FindWindowW("MsoCommandBar", customTaskPane.Title); //MLHIDE
                if (window != IntPtr.Zero)
                {
                    NativeMethods.MoveWindow(window, x, y, customTaskPane.Width, customTaskPane.Height, true);
                }
            }

        }
    }

    class HostService : Quandl.Shared.Excel.IHostService
    {
        private readonly _Application _application;
        public HostService(_Application application)
        {
            _application = application;
        }
        public void SetStatusBar(string message)
        {
            _application.StatusBar = message;
        }
    }

}
