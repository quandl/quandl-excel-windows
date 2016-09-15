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
    }
}
