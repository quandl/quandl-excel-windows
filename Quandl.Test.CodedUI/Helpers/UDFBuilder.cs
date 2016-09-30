using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Quandl.Shared.Models;
using System.Collections.Generic;

namespace Quandl.Test.CodedUI.Helpers
{
    public partial class CodedUITestHelpers
    {
        private static void SetCodedUITestsConfiguration()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 10;
            Playback.PlaybackSettings.MatchExactHierarchy = true;
            Playback.PlaybackSettings.SearchInMinimizedWindows = false;
            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.None;
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
        }

        private static void OpenExcelAndLogin()
        {
            UIMap.ClearRegistryApiKey();
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
            UIMap.LoginWithApiKey();
        }

        public static void SetupCodedUITest()
        {
            SetCodedUITestsConfiguration();
            OpenExcelAndLogin();
        }

        public static void CompleteCodedUITest()
        {
            UIMap.ClearRegistryApiKey();
        }

        public static void CompleteStep1(string databaseCode)
        {
            UIMap.InputDatabaseCode(databaseCode);
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        public static void CompleteBrowseStep1(Dictionary<string, string> browseOptions)
        {
            UIMap.SelectBrowseCategory(browseOptions["data type"], browseOptions["region"], browseOptions["category"]);
            UIMap.SelectDatabase(browseOptions["database"]);
            UIMap.ClickNextButton();
        }

        public static void CompleteStep2(DataHolderDefinition dataHolder, string filterText = null)
        {
            if (filterText != null)
            {
                UIMap.FilterDatasetsDatatables(filterText);
            }
            UIMap.SelectDatasetOrDatatableByName(dataHolder.Name.Replace(",", "\\,"));
            UIMap.NextButton().WaitForControlEnabled();
            UIMap.ClickNextButton();
        }

        public static void CompleteStep3(List<DataColumn> columns = null)
        {
            if (columns != null)
            {
                columns.ForEach(column => UIMap.SelectColumn(column));
            }

            UIMap.ClickNextButton();
        }
        public static void CompleteStep4(Dictionary<string, string> filters, bool filter = true)
        {
            if (filter == true)
            {
                if (filters["frequency"] != null)
                {
                    UIMap.SelectFrequencyFilter(filters["frequency"], filters["frequency code"]);
                }
                if (filters["date from"] != null)
                {
                    UIMap.SelectDatasetDateRangeFilter("Period Range", "Range");
                    UIMap.SelectDatasetDateFromFilter(filters["date from"]);
                    UIMap.SelectDatasetDateToFilter(filters["date to"]);
                }
                if (filters["single date"] != null)
                {
                    UIMap.SelectDatasetDateRangeFilter("Single Date", "Single");
                    UIMap.SelectDatasetDateFromFilter(filters["single date"]);
                }
                if (filters["transformation"] != null)
                {
                    UIMap.SelectTransformationFilter(filters["transformation"], filters["transformation code"]);
                }
                if (filters["sort"] != null)
                {
                    UIMap.SelectSortFilter(filters["sort"], filters["sort code"]);
                }
                if (filters["limit"] != null)
                {
                    UIMap.SelectSortFilter(filters["sort"], filters["sort code"]);
                }
            }

            UIMap.ClickNextButton();
        }
 
        public static void CompleteStep5(bool includeHeaders)
        {
            if (includeHeaders == true)
            {
                UIMap.SelectIncludeHeaders();
            }
        }
    }
}
