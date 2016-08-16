using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Quandl.Shared.Models
{
    public class ResultsData
    {
        public ResultsData(List<List<object>> data, List<string> headers)
        {
            Headers = headers;
            Data = data;
        }

        public List<string> Headers { get; }
        public List<List<object>> Data { get; }

        public ResultsData Combine(ResultsData newResults)
        {
            if (Headers.Count == 0)
            {
                return newResults;
            }

            var headers = Headers.Concat(newResults.Headers.GetRange(1, newResults.Headers.Count - 1)).ToList();
            var data = Data.FullGroupJoin(newResults.Data,
                x => x[0],
                y => y[0],
                (id, x, y) =>
                {
                    var cx = x.ToList().Count > 0 ? x.ToList()[0] : null;
                    var cy = y.ToList().Count > 0 ? y.ToList()[0] : null;

                    return MergeRows(cx, cy, Headers.Count, newResults.Headers.Count);
                }).ToList();
            return new ResultsData(data, headers);
        }

        public List<List<object>> SortedData(string field, bool ascending = false)
        {
            var indexToSort = Headers.Select(s => s.ToLower()).ToList().IndexOf(field.ToLower());
            if (ascending)
            {
                return (from row in Data
                    orderby row[indexToSort] ascending
                    select row).ToList();
            }
            return (from row in Data
                orderby row[indexToSort] descending
                select row).ToList();
        }

        private List<object> MergeRows(List<object> x, List<object> y, int xSize, int ySize)
        {
            if (x == null)
            {
                // Take the date from the `y` list and concat with nulls from x list then the remaining data from y list
                return
                    new List<object> {y[0]}.Concat(
                        DefaultList<object>(xSize - 1).Concat(y.GetRange(1, y.Count - 1)).ToList()).ToList();
            }
            if (y == null)
            {
                // Take the date from the `x` list and concat with data from x list then the remaining nulls from y list
                return x.Concat(DefaultList<object>(ySize - 1)).ToList();
            }

            return x.Concat(y.GetRange(1, y.Count - 1)).ToList();
        }

        private static List<T> DefaultList<T>(int capacity)
        {
            return Enumerable.Repeat(default(T), capacity).ToList();
        }

        public ResultsData ExpandAndReorderColumns(List<string> quandlCodeColumns)
        {
            // Expand the column header names
            var expandedHeaders = quandlCodeColumns.Select(qcc =>
            {
                var splitString = qcc.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return splitString.Length == 3
                    ? new List<string> {qcc}
                    : Headers.FindAll(x => x.StartsWith(qcc)).ToList();
            }).SelectMany(i => i).ToList();

            // Add a `DATE` field in if the user has not specified one already.
            expandedHeaders = expandedHeaders.Select(s => s.Length - 4 >= 0 && s.Substring(s.Length - 4, 4) == "DATE" ? "DATE" : s).ToList();
            if (expandedHeaders.Where(s => s.Length - 4 >= 0 ? s.Substring(s.Length - 4, 4) == "DATE" : false).Count() == 0)
            {
                expandedHeaders.Insert(0, Headers.First()); 
            }

            // Re-order the columns appropriately
            var shuffledData = new List<List<object>>();
            for (var r = 0; r < Data.Count; r++)
                shuffledData.Add(new List<object>());

            foreach (var header in expandedHeaders)
            {
                var columnIndex = Headers.IndexOf(header);
                for (var r = 0; r < Data.Count; r++)
                {
                    shuffledData[r].Add(Data[r][columnIndex]);
                }
            }

            return new ResultsData(shuffledData, expandedHeaders);
        }
    }
}