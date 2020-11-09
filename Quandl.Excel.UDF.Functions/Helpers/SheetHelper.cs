using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Models;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Quandl.Shared;
using Quandl.Shared.Excel;
using Quandl.Shared.Helpers;

namespace Quandl.Excel.UDF.Functions.Helpers
{
    public class SheetHelper
    {
        // Check every 50ms whether the calculation is done. After 200 iterations continue anyways. Assume excel has borked as there is a bug where calculations don't finish.
        public const int CalculationWaitTimeMs = 50;
        public const int MaxCalculationWaitIntervals = 200;

        // Retry wait if excel is busy
        public const int RetryWaitTimeMs = 500;


        private readonly bool _includeHeader;
        private readonly ResultsData _results;
        private readonly bool _threaded;
        private readonly bool _firstRow;
        private readonly bool _transpose;
        private readonly DatatableMetadata _metaData;

        private List<string> _remainingHeaders => _results.Headers.GetRange(1, _results.Headers.Count - 1);

        public bool? ConfirmedOverwrite = null;

        public SheetHelper(ResultsData results, bool includeHeader, bool firstRow = false, bool threaded = false, bool transpose = false, DatatableMetadata metaData = null)
        {
            _results = results;
            _includeHeader = includeHeader;
            _threaded = threaded;
            _firstRow = firstRow;
            _transpose = transpose;
            _metaData = metaData;
        }

        sealed class ComCache : System.IDisposable
        {
            private Range _currentFormulaCell;
            public ComCache(Range currentFormulaCell)
            {
                _currentFormulaCell = currentFormulaCell;
            }

            public Range CurrentRange
            {
                get { return _currentFormulaCell; }
            }

            public void Dispose()
            {
                if (_worksheet != null)
                {
                    Marshal.ReleaseComObject(_worksheet);
                }
                if (_worksheetCells != null)
                {
                    Marshal.ReleaseComObject(_worksheetCells);
                }
            }

            private Worksheet _worksheet;

            public Worksheet Worksheet
            {
                get { return _worksheet = _worksheet ?? _currentFormulaCell.Worksheet; }
            }

            private Range _worksheetCells;

            private Range WorksheetCells
            {
                get { return _worksheetCells = _worksheetCells ?? Worksheet.Cells; }
            }

            public Range this[int x, int y]
            {
                get { return (Range) WorksheetCells[x, y]; }
            }

            
        }

        private ComCache cache;
        public void PopulateData(Range currentFormulaCell)
        {
            cache = new ComCache(currentFormulaCell);
            try
            {
                Shared.Excel.ExcelExecutionHelper.ExecuteWithAutoRetry(Populate);
            }
            finally
            {
                cache.Dispose();
                cache = null;
            }
        }

        // Determine the value present in the first cell depending on the user options and the data returned.
        public string firstCellValue()
        {
            if (_includeHeader)
            {
                return _results.Headers[0];
            }

            if (_results.Data.Count > 0 && _results.Data[0].Count > 0)
            {
                return _results.Data[0][0].ToString();
            }
            else
            {
                return Locale.English.NoDataReturned;
            }
        }


        private void Populate()
        {
            // Populate data handling the first row separately if data is on the header row.
            var data = _results.Data;

            // Transpose the data is it needs transposing.
            if (_transpose) data = Transpose(data);

            // The first row contains headers and the original UDF formula.
            if (_firstRow && _includeHeader)
            {
                PopulateHeader();
                if (_transpose)
                {
                    PopulateGrid(data, 0, 1);
                }
                else
                {
                    PopulateGrid(data, 1);
                }

            }
            // The first row contains data (no headers) and the original UDF formula.
            else if (_firstRow && data.Count >= 1)
            {
                for (var j = 1; j < data[0].Count; j++)
                {
                    var setValueCell = this.cache.CurrentRange[1, j + 1];
                    try
                    {
                        setValueCell.Value2 = data[0][j]?.ToString() ?? "";
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(setValueCell);
                    }
                }
                    
                PopulateGrid(data.GetRange(1, data.Count - 1), 1);
            }
            // Purely populating data, the first row was written in another call.
            else if (data.Count >= 1)
            {
                PopulateGrid(data);
            }
        }

        private void PopulateHeader()
        {
            if (ConfirmedOverwrite == false) return;

            var dataArray = new List<List<object>>() { _remainingHeaders.Select(d => (object)d).ToList() };
            if (_transpose) dataArray = Transpose(dataArray);
            var _currentFormulaCell = cache.CurrentRange;
            var rowStart = _currentFormulaCell.Row + (_transpose ? 1 : 0);
            var columnStart = _currentFormulaCell.Column + (_transpose ? 0 : 1);
            var startCell = cache[rowStart, columnStart];
            try
            {
                WriteDataToGrid(dataArray, startCell);
            }
            finally
            {
                Marshal.ReleaseComObject(startCell);
            }
        }

        private void PopulateGrid(List<List<object>> dataArray, int rowOffset = 0, int colOffset = 0)
        {
            if (ConfirmedOverwrite == false) return;
            var _currentFormulaCell = cache.CurrentRange;
            var rowStart = rowOffset + _currentFormulaCell.Row;
            var columnStart = colOffset + _currentFormulaCell.Column;
            var startCell = cache[rowStart, columnStart];
            try
            {
                WriteDataToGrid(dataArray, startCell);
            }
            finally
            {
                Marshal.ReleaseComObject(startCell);
            }
        }
        private void WriteDataToGrid(List<List<object>> dataArray, Range startCell)
        {
            if (dataArray.Count == 0)
            {
                return;
            }

            IList<DataColumn> columnInfo = null;
            if (_metaData != null)
            {
                columnInfo = _metaData.datatable.Columns;
            }

            var data = ConvertNestedListToArray(dataArray);
            Range endCell = null;
            Range writeRange = null;
            Range writeRangeCells = null;
            Range writeRangeCellsToShow = null;
            try
            {
                if (!CanWriteData())
                {
                    return;
                }
                endCell = cache[startCell.Row + data.GetLength(0) - 1, startCell.Column + data.GetLength(1) - 1];
                writeRange = cache.Worksheet.Range[startCell, endCell]; // .Range is an indexed property
                // Take control from user, write data, show it.
                // writeRange.NumberFormat = "@";
                if (columnInfo != null)
                {
                    var columnsToFormat = GetColumnsToFormatIndexes(columnInfo);
                    foreach (int index in columnsToFormat)
                    {
                        writeRange.Cells[startCell.Row, startCell.Column + index].EntireColumn.NumberFormat = "@";
                    }
                }
                // writeRange.Cells[1, 4].EntireColumn.NumberFormat = "@";
                writeRange.Value2 = data;

                if (QuandlConfig.ScrollOnInsert)
                {
                    writeRangeCells = writeRange.Cells;
                    writeRangeCellsToShow = writeRangeCells[data.GetLength(0), 1];
                    writeRangeCellsToShow.Show();
                }
            }
            catch (COMException e)
            {
                Logger.log(e);
                throw;
            }
            finally
            {
                if (endCell != null)
                {
                    Marshal.ReleaseComObject(endCell);
                }

                if (writeRange != null)
                {
                    Marshal.ReleaseComObject(writeRange);
                }

                if (writeRangeCells != null)
                {
                    Marshal.ReleaseComObject(writeRangeCells);
                }

                if (writeRangeCellsToShow != null)
                {
                    Marshal.ReleaseComObject(writeRangeCellsToShow);
                }
            }
        }

        private IList<int> GetColumnsToFormatIndexes(IList<DataColumn> dataColumns)
        {
            IList<int> offsetArray = new List<int>();

            for (int i = 0; i < dataColumns.Count; i++)
            {
                if (dataColumns[i].Type == "text")
                {
                    offsetArray.Add(i);
                }    
            }

            return offsetArray;
        }

        private object[,] ConvertNestedListToArray(List<List<object>> data)
        {
            var newData = new object[data.Count, data[0].Count];
            for (var r = 0; r != data.Count; r++)
                for (var c = 0; c != data[0].Count; c++)
                    newData[r, c] = data[r][c];
            return newData;
        }

        private bool CanWriteData()
        {
            if (ConfirmedOverwrite != true && QuandlConfig.OverwriteDataWarning)
            {
                var form = new UI.confirmOverwrite();
                var result = form.ShowDialog();
                ConfirmedOverwrite = (result == DialogResult.Yes);
                return ConfirmedOverwrite == true;
            }

            return true;
        }

        // http://stackoverflow.com/questions/13586524/how-to-transpose-a-list-of-lists-filling-blanks-with-defaultt
        private List<List<T>> Transpose<T>(List<List<T>> lists)
        {
            var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default(T));
            return outer;
        }
    }
}