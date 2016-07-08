using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools;
using Quandl.Shared;
using Timer = System.Timers.Timer;

namespace Quandl.Excel.Addin
{
    public partial class ThisAddIn
    {
        public delegate void AuthTokenChanged();

        public delegate void LoginChanged();

        public Range ActiveCells;

        private Timer statusTimer;

        public event AuthTokenChanged AuthTokenChangedEvent;
        public event LoginChanged LoginChangedEvent;

        public CustomTaskPane AddCustomTaskPane(UserControl userControl, string name)
        {
            return CustomTaskPanes.Add(userControl, name);
        }

        public void UpdateStatusBar(Exception error)
        {
            var oldStatusBarVisibility = Application.DisplayStatusBar;
            Application.DisplayStatusBar = true;
            Application.StatusBar = "⚠ Quandl plugin error: " + error.Message;

            // Clean up an old timers;
            if (statusTimer != null)
            {
                statusTimer.Stop();
                statusTimer.Close();
            }

            // Create a new timer to show the error temporarily
            statusTimer = new Timer(20000);
            statusTimer.Elapsed += async (sender, e) => await Task.Run(() =>
            {
                Application.StatusBar = false;
                Application.DisplayStatusBar = oldStatusBarVisibility;
            });
            statusTimer.Start();
        }

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            ActiveCells = Application.ActiveCell;
            Application.WorkbookOpen += Workbook_Activated;
            Application.WorkbookOpen += Application_WorkbookOpen;
            Application.WorkbookActivate += Workbook_Activated;

            SetupAutoUpdateTimer();
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
        }

        private void Application_WorkbookOpen(Workbook wb)
        {
            if (!FunctionUpdater.HasQuandlFormulaInWorkbook(wb) || !QuandlConfig.AutoUpdate) return;
            const string message = @"Your workbook(s) contain Quandl formulas. Would you like to update your data?";
            const string caption = @"Update Data";
            var a = Application.Calculation;
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                FunctionUpdater.RecalculateQuandlFunctions(wb);
            }
        }

        #region VSTO generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion

        private void Workbook_Activated(Workbook workbook)
        {
            ActiveCells = Application.ActiveCell;
            workbook.SheetChange += Sheet_Updated;
            workbook.SheetSelectionChange += Workbook_SheetSelectionChange;
        }

        private void Workbook_SheetSelectionChange(object Sh, Range Target)
        {
            ActiveCells = Target;
        }

        private void Sheet_Updated(object sh, Range target)
        {
            //Array quandlCodes = target.Value2.Split(',');
            //List<JObject> data = new List<JObject>();
            //foreach (String code in quandlCodes)
            //{
            //    data.Add(Quandl.Shared.TestFunctions.pullSomeData(code.Trim()));
            //}

            //Excel.Range currentCell = target.Value2.Split(',');
        }

        public void OnAuthTokenChangedEvent()
        {
            AuthTokenChangedEvent?.Invoke();
        }

        public void OnLoginChangedEvent()
        {
            LoginChangedEvent?.Invoke();
        }

        public void OnAutoUpdateChangedEvent()
        {
            SetupAutoUpdateTimer();
        }

        private void SetupAutoUpdateTimer()
        {
            if (!QuandlConfig.AutoUpdate)
            {
                QuandlTimer.Instance.DisableUpdateTimer();
                return;
            }

            QuandlTimer.Instance.SetupAutoRefreshTimer(TimeoutEventHandler);
        }

        public void TimeoutEventHandler(object sender, ElapsedEventArgs eventArg)
        {
            // don't try to update if user is editing the sheet(s)
            // try again in later by enabling retry interval timeout if user is editing
            var isEditing = IsEditing();
            QuandlTimer.Instance.SetTimeoutInterval(isEditing);

            if (isEditing)
            {
                return;
            }

            var workbooks = Application.Workbooks;

            Application.Interactive = false;
            foreach (Workbook workbook in workbooks)
            {
                FunctionUpdater.RecalculateQuandlFunctions(workbook);
            }
            Application.Interactive = true;
        }

        // Excel Interop will throw an exception if the user is editing
        // at the same time the addin is trying to update a workbook
        // Detect if a user is editing in excel through trying to toggle
        // Application.Interactive
        // for more detail see: http://stackoverflow.com/questions/22482935/how-can-i-force-a-cell-to-stop-editing-in-excel-interop
        public bool IsEditing()
        {
            try
            {
                Application.Interactive = false;
                Application.Interactive = true;
            }
            catch (COMException)
            {
                return true;
            }
            return false;
        }
    }
}