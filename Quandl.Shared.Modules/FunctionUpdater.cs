using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Errors;

namespace Quandl.Shared
{
    public class FunctionUpdater
    {
        public static readonly string[] UserDefinedFunctions = {"QSERIES", "QTABLE"};

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
                    throw new MissingFormulaException("No formula's found to update.");
                }
                throw;
            }
            var count = range.Count;

            foreach (Range c in range.Cells)
            {
                if (!c.HasFormula) continue;
                string convertedString = c.Formula.ToString().ToUpper();
                foreach (var formulaDefinition in UserDefinedFunctions)
                {
                    if (convertedString.Contains(formulaDefinition))
                    {
                        return true;
                    }
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
            var oldValue = worksheet.EnableCalculation;
            worksheet.EnableCalculation = false;
            worksheet.Calculate();
            worksheet.EnableCalculation = oldValue;
        }

        public static void RecalculateQuandlFunctions(Workbook wb)
        {
            var worksheets = wb.Worksheets;
            foreach (Worksheet worksheet in worksheets)
            {
                RecalculateQuandlFunctions(worksheet);
            }
        }

        public static void RecalculateQuandlFunctions(Worksheet ws)
        {
            if (HasQuandlFormulaInWorkSheet(ws))
            {
                RecalculateQuandlFunctionsInWorkSheet(ws);
            }
        }
    }
}