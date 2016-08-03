using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Models;

namespace Quandl.Shared
{
    public class ExcelHelp
    {
        private readonly Range _currentFormulaCell;
        private readonly bool _includeHeader;
        private readonly ResultsData _results;

        public ExcelHelp(Range currentFormulaCell, ResultsData results, bool includeHeader)
        {
            _currentFormulaCell = currentFormulaCell;
            _results = results;
            _includeHeader = includeHeader;
        }

        public string PopulateData()
        {
            var previousCalculationMode = _currentFormulaCell.Worksheet.Application.Calculation;
            _currentFormulaCell.Application.Calculation = XlCalculation.xlCalculationManual;

            Populate();

            _currentFormulaCell.Worksheet.Application.Calculation = previousCalculationMode;

            // Determine the value present in the first cell.
            return _includeHeader ? _results.Headers[0] : _results.Data[0][0].ToString();
        }

        public void Populate()
        {
            var dataStartingRowOffset = 0;

            // Assume column names is the first row of data.
            if (_includeHeader)
            {
                dataStartingRowOffset = 1;
                for (var i = 1; i < _results.Headers.Count; i++)
                    _currentFormulaCell[1, i + 1].Value2 = _results.Headers[i];
            }

            // Populate data handling the first row separately if data is on the header row.
            var data = _results.Data;

            if (_includeHeader)
            {
                PopulateGrid(data, dataStartingRowOffset);
            }
            else if (data.Count >= 1)
            {
                for (var j = 1; j < data[0].Count; j++)
                    _currentFormulaCell[1, j + 1].Value2 = data[0][j]?.ToString() ?? "";
                PopulateGrid(data.GetRange(1, data.Count - 1), dataStartingRowOffset);
            }
        }

        public void PopulateGrid(List<List<object>> dataArray, int rowOffset = 0)
        {
            if (dataArray.Count == 0)
            {
                return;
            }

            var data = new object[dataArray.Count, dataArray[0].Count];
            for (var r = 0; r != dataArray.Count; r++)
                for (var c = 0; c != dataArray[0].Count; c++)
                    data[r, c] = dataArray[r][c];

            try
            {
                var worksheet = _currentFormulaCell.Worksheet;
                var rowStart = rowOffset + _currentFormulaCell.Row;
                var columnStart = _currentFormulaCell.Column;
                var startCell = (Range) worksheet.Cells[rowStart, columnStart];
                var endCell =
                    (Range) worksheet.Cells[rowStart + data.GetLength(0) - 1, columnStart + data.GetLength(1) - 1];
                var writeRange = worksheet.Range[startCell, endCell];
                writeRange.Value2 = data;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
        }
    }
}