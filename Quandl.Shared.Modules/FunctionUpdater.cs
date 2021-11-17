using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Errors;
using Quandl.Shared.Excel;
using System;
using System.Threading;

namespace Quandl.Shared
{
    public class FunctionUpdater
    {
        private static readonly string[] UserDefinedFunctions = {"QSERIES", "QTABLE"};
        private const int RetryWaitTimeMs = 500;

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
                if (ex.HResult == Quandl.Shared.Excel.Exception.UNSPECIFIED_1)
                {
                    return false;
                }
                throw ex;
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
            // Find all Quandl formula in the worksheet and re-calculate them.
            Range range = worksheet.UsedRange.SpecialCells(XlCellType.xlCellTypeFormulas);
            foreach (Range c in range.Cells)
            {
                if (!c.HasFormula) continue;
                string convertedString = c.Formula.ToString().ToUpper();
                foreach (var formulaDefinition in UserDefinedFunctions)
                {
                    if (convertedString.Contains(formulaDefinition))
                    {
                        RecalculateFormulaCell(c);
                    }
                }
            }
        }

        public static void RecalculateQuandlFunctions(Workbook wb)
        {
            bool foundFormulas = false;
            var worksheets = wb.Worksheets;
            foreach (Worksheet worksheet in worksheets)
            {
                if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    foundFormulas = true;
                    RecalculateQuandlFunctionsInWorkSheet(worksheet);
                }
                
            }
            if (!foundFormulas) throw new MissingFormulaException("No formula's were found to update.");
        }

        public static void RecalculateQuandlFunctions(Worksheet ws)
        {
            if (HasQuandlFormulaInWorkSheet(ws))
            {
                RecalculateQuandlFunctionsInWorkSheet(ws);
            }
            else
            {
                throw new MissingFormulaException("No formula's were found to update.");
            }
        }

        private static void RecalculateFormulaCell(Range cell)
        {
            try
            {
                // Force formula re-calculate by resetting it.
                cell.Formula = cell.Formula;
            }
            catch (System.Exception e)
            {
                // The excel RPC server is busy. We need to wait and then retry (RPC_E_SERVERCALL_RETRYLATER or VBA_E_IGNORE)
                if (e.HResult == Excel.Exception.RPC_E_SERVERCALL_RETRYLATER || e.HResult == Excel.Exception.VBA_E_IGNORE)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    RecalculateFormulaCell(cell);
                }
            }
        }
    }
}
