using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Errors;
using Quandl.Shared.Excel;
using System;
using System.Threading;
using System.Linq;

namespace Quandl.Excel.Addin
{
    public class FunctionUpdater
    {
        private static readonly string[] UserDefinedFunctions = {"QSERIES", "QTABLE"};
        private const int RetryWaitTimeMs = 500;

        public static bool HasQuandlFormulaInWorkSheet(Worksheet worksheet)
        {
            return FindQuandlFormulaInWorkSheet(worksheet).Any();
        }
        static System.Collections.Generic.IEnumerable<Range> FindQuandlFormulaInWorkSheet(Worksheet worksheet)
        {
            Range formulaRange = null;
            Range usedRange = null;
            try
            {
                try
                {
                    usedRange = worksheet.UsedRange;
                    formulaRange = usedRange.SpecialCells(XlCellType.xlCellTypeFormulas);
                }
                catch (COMException ex)
                {
                    // No quandl formula cells were found
                    if (ex.HResult == Quandl.Shared.Excel.Exception.UNSPECIFIED_1)
                    {
                        yield break;
                    }

                    throw ex;
                }

                if (formulaRange != null)
                {
                    foreach (Range cell in formulaRange)
                    {
                        try
                        {
                            if ((bool) cell.HasFormula)
                            {
                                var checkFormula = cell.Formula as string;
                                if (!string.IsNullOrEmpty(checkFormula))
                                {
                                    var convertedString = checkFormula.ToUpperInvariant();
                                    foreach (var formulaDefinition in UserDefinedFunctions)
                                    {
                                        if (convertedString.Contains(formulaDefinition))
                                        {
                                            yield return cell;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(cell);
                        }
                    }
                }
            }
            finally
            {
                if (usedRange != null)
                {
                    Marshal.ReleaseComObject(usedRange);
                }

                if (formulaRange != null)
                {
                    Marshal.ReleaseComObject(formulaRange);
                }
            }
        }

        public static bool HasQuandlFormulaInWorkbook(Workbook wb)
        {
            return AllWorksheets(wb).Any(ws => HasQuandlFormulaInWorkSheet(ws));
        }
        static System.Collections.Generic.IEnumerable<Worksheet> AllWorksheets(Workbook wb)
        {
            Sheets worksheets = null;
            try
            {
                worksheets = wb.Worksheets;
                for (var idx = 1; idx <= worksheets.Count; idx++)
                {
                    object worksheet = null;
                    try
                    {
                        worksheet = worksheets[idx];
                        var ws = worksheet as Worksheet;
                        if (ws != null )
                        {
                            yield return ws;
                        }
                    }
                    finally
                    {
                        if (worksheet != null)
                        {
                            Marshal.ReleaseComObject(worksheet);
                        }
                    }
                }
            }
            finally
            {
                if (worksheets != null)
                {
                    Marshal.ReleaseComObject(worksheets);
                }
            }
        }

        public static void RecalculateQuandlFunctionsInWorkSheet(Worksheet worksheet)
        {
            // Find all Quandl formula in the worksheet and re-calculate them.
            foreach (var cell in FindQuandlFormulaInWorkSheet(worksheet))
            {
                RecalculateFormulaCell(cell);
            }
        }

        public static void RecalculateQuandlFunctions(Workbook wb)
        {
            bool foundFormulas = false;
            foreach (var worksheet in AllWorksheets(wb))
            {
                if (HasQuandlFormulaInWorkSheet(worksheet))
                {
                    foundFormulas = true;
                    RecalculateQuandlFunctionsInWorkSheet(worksheet);
                }
            }

            if (!foundFormulas)
            {
                throw new MissingFormulaException("No Quandl formula's were found to update.");
            }
        }
    

        public static void RecalculateQuandlFunctions(Worksheet ws)
        {
            if (HasQuandlFormulaInWorkSheet(ws))
            {
                RecalculateQuandlFunctionsInWorkSheet(ws);
            }
            else
            {
                throw new MissingFormulaException("No Quandl formula's were found to update.");
            }
        }

        private static void RecalculateFormulaCell(Range cell)
        {
            ExcelExecutionHelper.ExecuteWithAutoRetry(() => cell.Formula = cell.Formula);
        }
    }
}