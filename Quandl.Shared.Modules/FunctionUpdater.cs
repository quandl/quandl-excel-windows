using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Errors;

namespace Quandl.Shared
{
    public class FunctionUpdater
    {
        public static readonly string[] UserDefinedFunctions = {"QSERIES", "QTABLE"};
        public static bool? RefreshConfirmation = null;

        public static bool HasQuandlFormulaInWorkSheet(Worksheet worksheet)
        {
            Range range;
            try
            {
                range = worksheet.UsedRange.SpecialCells(XlCellType.xlCellTypeFormulas);
            }
            catch (COMException ex)
            {
                // No quandl formula cells were found
                if (ex.HResult == -2146827284)
                {
                    throw new MissingFormulaException("No Quandl formula's were found to update.");
                }
                throw;
            }

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
            // force recalculation of workbook by reseting calculation mode.
            var oldValue = worksheet.EnableCalculation;
            worksheet.EnableCalculation = false;
            //worksheet.Calculate();

            // Find all quandl formula in the worksheet and re-calculate them.
            Range range = worksheet.UsedRange.SpecialCells(XlCellType.xlCellTypeFormulas);
            foreach (Range c in range.Cells)
            {
                if (!c.HasFormula) continue;
                string convertedString = c.Formula.ToString().ToUpper();
                foreach (var formulaDefinition in UserDefinedFunctions)
                {
                    if (convertedString.Contains(formulaDefinition))
                    {
                        c.Calculate();
                    }
                }
            }
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