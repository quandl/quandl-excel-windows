using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using System.Linq;
using System.Collections.Generic;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class ColumnSelectionTest
    {
        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;

        private Dataset dataset;
        private Datatable datatable;

        public ColumnSelectionTest()
        {
            dataset = new Dataset
            {
                DatabaseCode = "EOD",
                DatasetCode = "AAPL",
                Name = "Apple Inc. (AAPL) Stock Prices, Dividends and Splits"
            };

            datatable = new Datatable
            {
                VendorCode = "ZACKS",
                DatatableCode = "CP",
                Name = "Zacks Company Profiles"
            };
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 10;
            UIMap.ClearRegistryApiKey();
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
            UIMap.LoginWithApiKey();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

        private void CompleteStep1(string databaseCode)
        {
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        private void CompleteStep2(DataHolderDefinition dataHolder, string filterText = null)
        {
            if (filterText != null)
            {
                UIMap.FilterDatasetsDatatables(filterText);
            }
            UIMap.SelectDatasetOrDatatableByName(dataHolder.Name.Replace(",", "\\,"));
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        private string convertListToUDFArray(List<string> content)
        {
            string temp = string.Join("\",\"", content);
            return $"{{\"{temp}\"}}";
        }

        private string datasetUDF(string datasetCode, List<DataColumn> columns = null)
        {
            if (columns == null || columns.Count == 0)
            {
                return $"=QSERIES(\"{datasetCode}\")";
            }
            else if (columns.Count == 1)
            {
                return $"=QSERIES(\"{datasetCode}/{columns.First().Code}\")";
            }
            else
            {
                var columnNames = columns.Select(column => $"{dataset.Code}/{column.Name.ToUpper()}").ToList();
                return $"=QSERIES({convertListToUDFArray(columnNames)})";
            }
        }

        private string datatableUDF(string datatableCode, List<DataColumn> columns = null)
        {
            if (columns == null || columns.Count == 0)
            {
                return $"=QTABLE(\"{datatableCode}\")";
            }
            else if (columns.Count == 1)
            {
                return $"=QTABLE(\"{datatableCode}\",\"{columns[0].Name}\")";
            }
            else
            {
                var columnNames = columns.Select(column => column.Name).ToList();
                return $"=QTABLE(\"{datatableCode}\",{convertListToUDFArray(columnNames)})";
            }
        }

        [TestMethod]
        public void SelectDatasetColumns()
        {
            CompleteStep1(dataset.DatabaseCode);
            CompleteStep2(dataset, dataset.Name);

            List<DataColumn> columns = new List<DataColumn>
            {
                new DataColumn() { Name = "Volume", Parent = dataset },
                new DataColumn() { Name = "Open",   Parent = dataset },
                new DataColumn() { Name = "Close",  Parent = dataset }
            };

            columns.ForEach(delegate (DataColumn column)
            {
                UIMap.SelectColumn(column);
                UIMap.AssertColumnAddedToSelection(column);
            });

            UIMap.AssertNumberOfColumnsSelected(columns.Count);
            UIMap.AssertCorrectUDFSignature(datasetUDF(dataset.Code, columns));
        }

        [TestMethod]
        public void SelectDatatableColumns()
        {
            CompleteStep1("ZCP");
            CompleteStep2(datatable);

            List<DataColumn> columns = new List<DataColumn>
            {
                new DataColumn() { Name = "ticker",         Parent = datatable },
                new DataColumn() { Name = "exchange",       Parent = datatable },
                new DataColumn() { Name = "address_line_1", Parent = datatable },
                new DataColumn() { Name = "city",           Parent = datatable }
            };

            for (int n = 0; n < columns.Count; n++)
            {
                UIMap.SelectColumn(columns[n]);
                UIMap.AssertColumnAddedToSelection(columns[n]);
                UIMap.AssertCorrectUDFSignature(datatableUDF(datatable.Code, columns.GetRange(0, n + 1)));
            };

            UIMap.AssertNumberOfColumnsSelected(columns.Count);
            UIMap.AssertCorrectUDFSignature(datatableUDF(datatable.Code, columns));
        }

        [TestMethod]
        public void SelectAddAllColumns()
        {
            CompleteStep1(dataset.DatabaseCode);
            CompleteStep2(dataset, dataset.Name);

            var allAvailableColumns  = UIMap.GetAllAvailableColumns();
            var expectedUDFSignature = datasetUDF(dataset.Code, allAvailableColumns);

            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.ClickAddAllColumnsButton();
            UIMap.AssertNumberOfColumnsSelected(UIMap.CountAvailableColumns());
            UIMap.AssertCorrectUDFSignature(expectedUDFSignature);
        }

        [TestMethod]
        public void RemoveAllColumnsFromSelection()
        {
            CompleteStep1(dataset.DatabaseCode);
            CompleteStep2(dataset, dataset.Name);

            List<DataColumn> columns = new List<DataColumn>
            {
                new DataColumn() { Name = "Volume", Parent = dataset },
                new DataColumn() { Name = "Open",   Parent = dataset },
                new DataColumn() { Name = "Close",  Parent = dataset }
            };

            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.AssertCorrectUDFSignature(datasetUDF(dataset.Code));

            for (int n = 0; n < columns.Count; n++)
            {
                UIMap.SelectColumn(columns[n]);
                UIMap.AssertNumberOfColumnsSelected(n + 1);
                UIMap.AssertCorrectUDFSignature(datasetUDF(dataset.Code, columns.GetRange(0, n + 1)));
            }

            UIMap.ClickRemoveAllColumnsButton();
            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.AssertCorrectUDFSignature(datasetUDF(dataset.Code));
        }
    }
}
