using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Newtonsoft.Json.Linq;

namespace Quandl.Excel.Addin
{
    using Excel = Microsoft.Office.Interop.Excel;

    public partial class ThisAddIn
    {
        public Excel.Range activeCells;
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;

        public void TaskPane_Show()
        {
            DataTaskPane taskPane = new DataTaskPane(this.activeCells);
            myCustomTaskPane = this.CustomTaskPanes.Add(taskPane, "My Task Pane");
            myCustomTaskPane.Width = taskPane.PreferredSize.Width + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            myCustomTaskPane.Visible = true;
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.activeCells = this.Application.ActiveCell;
            this.Application.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(this.Workbook_Activated);
            this.Application.WorkbookActivate += new Excel.AppEvents_WorkbookActivateEventHandler(this.Workbook_Activated);
        }


        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
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
            this.activeCells = this.Application.ActiveCell;
            workbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(this.Sheet_Updated);
            workbook.SheetSelectionChange += Workbook_SheetSelectionChange;
        }

        private void Workbook_SheetSelectionChange(object Sh, Excel.Range Target)
        {
            this.activeCells = Target;
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
    }
}
