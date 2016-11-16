﻿using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using Quandl.Shared.Models;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Quandl.Shared.Excel
{
    public class SheetHelper
    {
        // Check every 50ms whether the calculation is done. After 200 iterations continue anyways. Assume excel has borked as there is a bug where calculations don't finish.
        public const int CalculationWaitTimeMs = 50;
        public const int MaxCalculationWaitIntervals = 200;

        // Retry wait if excel is busy
        public const int RetryWaitTimeMs = 500;

        // Don't allow two UDF threads to write at once.
        public static Mutex DataWriteMutex = new Mutex();

        // Some basics for writing data.
        private readonly Range _currentFormulaCell;
        private readonly bool _includeHeader;
        private readonly ResultsData _results;
        private readonly bool _threaded;
        private readonly bool _firstRow;

        // Helpers
        private Worksheet _currentWorksheet => _currentFormulaCell.Worksheet;
        private List<string> _remainingHeaders => _results.Headers.GetRange(1, _results.Headers.Count - 1);

        public bool? ConfirmedOverwrite = null;

        public SheetHelper(Range currentFormulaCell, ResultsData results, bool includeHeader, bool firstRow = false, bool threaded = false)
        {
            _currentFormulaCell = currentFormulaCell;
            _results = results;
            _includeHeader = includeHeader;
            _threaded = threaded;
            _firstRow = firstRow;
        }

        public string PopulateData()
        {
            try
            {
                // Acquire Mutex to avoid multiple functions writing at the same time.
                DataWriteMutex.WaitOne();

                // Since this is executing in a thread wait for excel to be finished whatever calculations its currently doing before writing to other cells. Helps avoid some issues.
                if (_threaded)
                {
                    WaitForExcelToBeReady();
                }

                Populate();

                // Release Mutex to allow another function to write data.
                DataWriteMutex.ReleaseMutex();
            }
            catch (COMException e)
            {
                Trace.WriteLine(e.Message);

                // Release Mutex to allow another function to write data.
                DataWriteMutex.ReleaseMutex();

                // The excel RPC server is busy. We need to wait and then retry (RPC_E_SERVERCALL_RETRYLATER or VBA_E_IGNORE)
                if (e.HResult == -2147417846 || e.HResult == -2146777998)
                {
                    Thread.Sleep(RetryWaitTimeMs);
                    return PopulateData();
                }

                throw;
            }

            // Determine the value present in the first cell.
            return _includeHeader ? _results.Headers[0] : _results.Data[0][0].ToString();
        }

        // Wait for the calculations to be done or force adding data to sheet when they are not.
        private void WaitForExcelToBeReady()
        {
            var iterations = 0;
            var calculationState = _currentWorksheet.Application.CalculationState;
            while (calculationState != XlCalculationState.xlDone && iterations < MaxCalculationWaitIntervals)
            {
                Thread.Sleep(CalculationWaitTimeMs);
                calculationState = _currentWorksheet.Application.CalculationState;
                iterations++;
            }

            if (iterations >= MaxCalculationWaitIntervals)
            {
                Trace.WriteLine("Max wait calculations iterations exceeded.");
            }
        }

        private void Populate()
        {
            // Populate data handling the first row separately if data is on the header row.
            var data = _results.Data;

            // The first row contains headers and the original UDF formula.
            if (_firstRow && _includeHeader)
            {
                PopulateHeader();
                PopulateGrid(data, 1);
            }
            // The first row contains data (no headers) and the original UDF formula.
            else if (_firstRow && data.Count >= 1)
            {
                for (var j = 1; j < data[0].Count; j++)
                    _currentFormulaCell[1, j + 1].Value2 = data[0][j]?.ToString() ?? "";
                PopulateGrid(data.GetRange(1, data.Count - 1), 1);
            }
            // Purely populating data, the first row was written in another call.
            else if (data.Count >= 1)
            {
                PopulateGrid(data, 0);
            }
        }

        private void PopulateHeader()
        {
            if (ConfirmedOverwrite == false) return;

            var dataArray = new List<List<object>>() { _remainingHeaders.Select(d => (object)d).ToList() };
            var rowStart = _currentFormulaCell.Row;
            var columnStart = _currentFormulaCell.Column + 1;
            var startCell = (Range)_currentWorksheet.Cells[rowStart, columnStart];
            WriteDataToGrid(dataArray, startCell);
        }

        private void PopulateGrid(List<List<object>> dataArray, int rowOffset = 0)
        {
            if (ConfirmedOverwrite == false) return;

            var rowStart = rowOffset + _currentFormulaCell.Row;
            var columnStart = _currentFormulaCell.Column;
            var startCell = (Range)_currentWorksheet.Cells[rowStart, columnStart];
            WriteDataToGrid(dataArray, startCell);
        }
        private void WriteDataToGrid(List<List<object>> dataArray, Range startCell)
        {
            if (dataArray.Count == 0)
            {
                return;
            }

            var data = ConvertNestedListToArray(dataArray);

            try
            {
                var endCell = (Range)_currentWorksheet.Cells[startCell.Row + data.GetLength(0) - 1, startCell.Column + data.GetLength(1) - 1];
                var writeRange = _currentWorksheet.Range[startCell, endCell];

                if (!CanWriteData())
                {
                    return;
                }

                // Take control from user, write data, show it.
                writeRange.Value2 = data;

                if (QuandlConfig.ScrollOnInsert)
                {
                    writeRange.Cells[data.GetLength(0), 1].Show();
                }
            }
            catch (COMException e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
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
                var result = System.Windows.Forms.MessageBox.Show(
                        Locale.English.OverwriteExistingDataPopupDesc,
                        Locale.English.OverwriteExistingDataPopupTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                ConfirmedOverwrite = (result == DialogResult.Yes);
                return ConfirmedOverwrite == true;
            }

            return true;
        }
    }
}