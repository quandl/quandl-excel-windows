using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Newtonsoft.Json.Linq;
using Quandl.Excel.Addin.Controls;
using Quandl.Shared;

namespace Quandl.Excel.Addin
{
    using Excel = Microsoft.Office.Interop.Excel;

    public partial class ThisAddIn
    {
        public Excel.Range ActiveCells;
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;


        public delegate void AuthTokenChanged();
        public delegate void LoginChanged();

        public event AuthTokenChanged AuthTokenChangedEvent;
        public event LoginChanged LoginChangedEvent;

        public void TaskPane_Show()
        {
            CreateCustomPane(new DataTaskPane(ActiveCells), "My Task Pane");
        }

        public void SettingsPane_Show(Toolbar toolbar)
        {
            var quandlSettings = new QuandlSettings();
            // allows toolbar to handle auth token changed events
            quandlSettings.SettingsAuthTokenChanged += OnAuthTokenChangedEvent;
            quandlSettings.SettingsAutoUpdateChanged += OnAutoUpdateChangedEvent;

            // allows quandl settings pane to handle login changed events
            LoginChangedEvent += quandlSettings.UpdateApiKeyTextBox;

            CreateCustomPane(quandlSettings, "Quandl Settings");
        }

        public void CreateCustomPane(UserControl userControl, string name)
        {
            myCustomTaskPane = this.CustomTaskPanes.Add(userControl, name);
            myCustomTaskPane.Width = userControl.PreferredSize.Width + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            myCustomTaskPane.Visible = true;
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.ActiveCells = this.Application.ActiveCell;
            this.Application.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(this.Workbook_Activated);
            this.Application.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(Application_WorkbookOpen);
            this.Application.WorkbookActivate += new Excel.AppEvents_WorkbookActivateEventHandler(this.Workbook_Activated);

            SetupAutoUpdateTimer();
        }


        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void Application_WorkbookOpen(Excel.Workbook wb)
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        public Microsoft.Office.Tools.CustomTaskPane TaskPane
        {
            get
            {
                return myCustomTaskPane;
            }
        }

        private void Workbook_Activated(Excel.Workbook workbook)
        {
            this.ActiveCells = this.Application.ActiveCell;
            workbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(this.Sheet_Updated);
            workbook.SheetSelectionChange += Workbook_SheetSelectionChange;
        }

        private void Workbook_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            this.ActiveCells = Target;
        }

        private void Sheet_Updated(object sh, Excel.Range target)
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

        public void TimeoutEventHandler(object sender, System.Timers.ElapsedEventArgs eventArg)
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
            foreach (Excel.Workbook workbook in workbooks)
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
            catch (System.Runtime.InteropServices.COMException)
            {
                return true;
            }
            return false;
        }
    }
}
