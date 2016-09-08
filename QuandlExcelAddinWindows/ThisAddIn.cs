using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools;
using Quandl.Shared;
using Quandl.Shared.Excel;

namespace Quandl.Excel.Addin
{
    public partial class ThisAddIn
    {
        public delegate void ActiveCellChanged(Range target);

        public delegate void AuthTokenChanged();

        public delegate void LoginChanged();

        private Range _activeCells;

        public Range ActiveCells
        {
            get { return _activeCells; }
            set
            {
                _activeCells = value;
                OnActiveCellChangedEvent(value);
            }
        }

        public event AuthTokenChanged AuthTokenChangedEvent;
        public event LoginChanged LoginChangedEvent;
        public event ActiveCellChanged ActiveCellChangedEvent;

        public CustomTaskPane AddCustomTaskPane(UserControl userControl, string name)
        {
            return CustomTaskPanes.Add(userControl, name);
        }

        public void UpdateStatusBar(Exception error)
        {
            (new Shared.Excel.StatusBar()).AddException(error);
        }

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            ActiveCells = Application.ActiveCell;

            Application.WorkbookOpen += CheckWorkbookForFormulaUpdates;
            Application.WorkbookActivate += Workbook_Activate;
            Application.SheetSelectionChange += Workbook_SheetSelectionChange;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            FunctionGrimReaper.EndReaping();
        }

        private void TurnOffAutoUpdates()
        {
            QuandlConfig.PreventCurrentExecution = true;
            Globals.Ribbons.Ribbon2.SetExecutionToggleIcon();
        }

        private void CheckWorkbookForFormulaUpdates(Workbook wb)
        {
            TurnOffAutoUpdates();

            if (!FunctionUpdater.HasQuandlFormulaInWorkbook(wb) || QuandlConfig.AutoUpdateFrequency != QuandlConfig.AutoUpdateFrequencies.WorkbookOpen) return;

            const string message = @"Your workbook(s) contain Quandl formulas. Would you like to update your data?";
            const string caption = @"Update Data";
            var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                QuandlConfig.PreventCurrentExecution = false;
                Globals.Ribbons.Ribbon2.SetExecutionToggleIcon();

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

        private void Workbook_Activate(Workbook workbook)
        {
            ActiveCells = Application.ActiveCell;
        }

        private void Workbook_SheetSelectionChange(object sh, Range target)
        {
            ActiveCells = target;
        }

        public void OnAuthTokenChangedEvent()
        {
            AuthTokenChangedEvent?.Invoke();
        }

        public void OnLoginChangedEvent()
        {
            LoginChangedEvent?.Invoke();
        }

        public void OnActiveCellChangedEvent(Range target)
        {
            ActiveCellChangedEvent?.Invoke(target);
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