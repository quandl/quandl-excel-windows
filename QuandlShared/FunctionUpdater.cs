using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Quandl.Shared
{
    public class FunctionUpdater
    {
        public static bool HasQuandlFormulaInWorkSheet(Worksheet worksheet)
        {
            Range range;
            try
            {
                range = worksheet.UsedRange.SpecialCells(XlCellType.xlCellTypeFormulas);
            }
            catch (COMException ex)
            {
                if (ex.Message == "No cells were found.")
                {
                    return false;
                }
                throw;
            }
            var count = range.Count;

            foreach (Range c in range.Cells)
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

        public static bool HasQuandlFormulaInWorkbook(Workbook wb)
        {
            var worksheets = wb.Worksheets;
            foreach (Worksheet worksheet in worksheets)
            {
                if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    return true;
                }
            }
            return false;
        }

        public static void RecalculateQuandlFunctionsInWorkSheet(Worksheet worksheet)
        {
            // force recalculation of workbook
            worksheet.EnableCalculation = false;
            worksheet.EnableCalculation = true;
        }

        public static void RecalculateQuandlFunctions(Workbook wb)
        {
            var worksheets = wb.Worksheets;
            foreach (Worksheet worksheet in worksheets)
            {
                if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    RecalculateQuandlFunctionsInWorkSheet(worksheet);
                }
            }
        }
    }
}