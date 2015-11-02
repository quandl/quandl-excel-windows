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
        private DataTaskPane myUserControl1;
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;

        public void TaskPane_Show()
        {
            myCustomTaskPane.Visible = true;
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.WorkbookActivate += new Excel.AppEvents_WorkbookActivateEventHandler(this.Workbook_Activated);

            myUserControl1 = new DataTaskPane();
            myCustomTaskPane = this.CustomTaskPanes.Add(myUserControl1, "My Task Pane");
            myCustomTaskPane.Width = myUserControl1.PreferredSize.Width + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
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
            workbook.SheetChange += new Excel.WorkbookEvents_SheetChangeEventHandler(this.Sheet_Updated);
        }

        private void Sheet_Updated(object sh, Excel.Range target)
        {
            Array quandlCodes = target.Value2.Split(',');
            List<JObject> data = new List<JObject>();
            foreach (String code in quandlCodes)
            {
                data.Add(Quandl.Shared.TestFunctions.pullSomeData(code.Trim()));
            }
            
            Excel.Range currentCell = target.Value2.Split(',');
        }
    }
}
