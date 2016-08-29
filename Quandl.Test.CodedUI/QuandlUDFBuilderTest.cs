using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quandl.Test.CodedUI
{
    /// <summary>
    /// Summary description for CodedUITest4
    /// </summary>
    [CodedUITest]
    public class QuandlUDFBuilderTest
    {
        #region Additional test attributes

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;

        #region Tests for the Quandl UDF Builder

        [TestMethod]
        public void LoginWithUsername()
        {
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
            UIMap.LoginWithUsername();
            UIMap.AssertLoggedIn();
        }

        #endregion Tests
    }
}
