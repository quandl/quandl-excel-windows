using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quandl.Shared.Models;
using System.Linq;
using System.Collections.Generic;
using Quandl.Test.CodedUI.Helpers;

namespace Quandl.Test.CodedUI.UI.UDF_Builder
{
    [CodedUITest]
    public class ColumnSelectionTest
    {
        private UIMap UIMap;

        private Dataset _dataset;
        private Datatable _datatable;

        public ColumnSelectionTest()
        {
            UIMap = CodedUITestHelpers.UIMap;
            _dataset = CodedUITestHelpers.SampleDataset();
            _datatable = CodedUITestHelpers.SampleDatatable();
        }

        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            CodedUITestHelpers.SetupCodedUITests();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            CodedUITestHelpers.CompleteCodedUITests();
        }

        #endregion

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
                var columnNames = columns.Select(column => $"{_dataset.Code}/{column.Name.ToUpper()}").ToList();
                return $"=QSERIES({CodedUITestHelpers.convertListToUDFArray(columnNames)})";
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
                return $"=QTABLE(\"{datatableCode}\",{CodedUITestHelpers.convertListToUDFArray(columnNames)})";
            }
        }

        [TestMethod]
        public void SelectDatasetColumns()
        {
            CodedUITestHelpers.CompleteStep1(_dataset.DatabaseCode);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);

            var columns = CodedUITestHelpers.SampleDatasetColumns();
            columns.ForEach(delegate (DataColumn column)
            {
                UIMap.SelectColumn(column);
                UIMap.AssertColumnAddedToSelection(column);
            });

            UIMap.AssertNumberOfColumnsSelected(columns.Count);
            UIMap.AssertCorrectUDFSignature(datasetUDF(_dataset.Code, columns));
        }

        [TestMethod]
        public void SelectDatatableColumns()
        {
            CodedUITestHelpers.CompleteStep1("ZCP");
            CodedUITestHelpers.CompleteStep2(_datatable);

            var columns = CodedUITestHelpers.SampleDatatableColumns();
            for (int n = 0; n < columns.Count; n++)
            {
                UIMap.SelectColumn(columns[n]);
                UIMap.AssertColumnAddedToSelection(columns[n]);
                UIMap.AssertCorrectUDFSignature(datatableUDF(_datatable.Code, columns.GetRange(0, n + 1)));
            };

            UIMap.AssertNumberOfColumnsSelected(columns.Count);
            UIMap.AssertCorrectUDFSignature(datatableUDF(_datatable.Code, columns));
        }

        [TestMethod]
        public void SelectAddAllColumns()
        {
            CodedUITestHelpers.CompleteStep1(_dataset.DatabaseCode);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);

            var allAvailableColumns  = UIMap.GetAllAvailableColumns();
            var expectedUDFSignature = datasetUDF(_dataset.Code, allAvailableColumns);

            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.ClickAddAllColumnsButton();
            UIMap.AssertNumberOfColumnsSelected(UIMap.CountAvailableColumns());
            UIMap.AssertCorrectUDFSignature(expectedUDFSignature);
        }

        [TestMethod]
        public void RemoveAllColumnsFromSelection()
        {
            CodedUITestHelpers.CompleteStep1(_dataset.DatabaseCode);
            CodedUITestHelpers.CompleteStep2(_dataset, _dataset.Name);

            var columns = CodedUITestHelpers.SampleDatasetColumns();

            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.AssertCorrectUDFSignature(datasetUDF(_dataset.Code));

            for (int n = 0; n < columns.Count; n++)
            {
                UIMap.SelectColumn(columns[n]);
                UIMap.AssertNumberOfColumnsSelected(n + 1);
                UIMap.AssertCorrectUDFSignature(datasetUDF(_dataset.Code, columns.GetRange(0, n + 1)));
            }

            UIMap.ClickRemoveAllColumnsButton();
            UIMap.AssertNumberOfColumnsSelected(0);
            UIMap.AssertCorrectUDFSignature(datasetUDF(_dataset.Code));
        }
    }
}
