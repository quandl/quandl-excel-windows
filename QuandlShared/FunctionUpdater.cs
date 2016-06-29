using Excel = Microsoft.Office.Interop.Excel;

namespace Quandl.Shared
{
    public class FunctionUpdater
    {
        public static bool HasQuandlFormulaInWorkSheet(Excel.Worksheet worksheet)
        {
            Excel.Range range;
            try
            {
                range = worksheet.UsedRange.SpecialCells(Excel.XlCellType.xlCellTypeFormulas);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message == "No cells were found.")
                {
                    return false;
                }
                throw;
            }
            var count = range.Count;

            foreach (Excel.Range c in range.Cells)
            {
                if (!c.HasFormula) continue;
                var quandlFormula = c.Formula.ToString().ToLower().Contains("qdata");
                if (quandlFormula)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasQuandlFormulaInWorkbook(Excel.Workbook wb)
        {
            var worksheets = wb.Worksheets;
            foreach (Excel.Worksheet worksheet in worksheets)
            {
                if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    return true;
                }
            }
            return false;


        }

        public static void RecalculateQuandlFunctionsInWorkSheet(Excel.Worksheet worksheet)
        {
            // force recalculation of workbook
            worksheet.EnableCalculation = false;
            worksheet.EnableCalculation = true;
        }

        public static void RecalculateQuandlFunctions(Excel.Workbook wb)
        {
            var worksheets = wb.Worksheets;
            foreach (Excel.Worksheet worksheet in worksheets)
            {
            if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    RecalculateQuandlFunctionsInWorkSheet(worksheet);
                }
            }
        }
    }
}
