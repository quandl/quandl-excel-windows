using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace Quandl.Test.CodedUI
{
    /// <summary>
    /// Summary description for CodedUITest4
    /// </summary>
    [CodedUITest]
    public class QuandlUDFBuilderTest
    {
        #region Additional test attributes

        [TestInitialize()]
        public void MyTestInitialize()
        {
            UIMap.OpenExcelAndWorksheet();
            UIMap.OpenLoginPage();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            UIMap.ClearRegistryApiKey();
        }

        #endregion

        public UIMap UIMap => map ?? (map = new UIMap());
        private UIMap map;

        [TestMethod]
        public void LoginWithAPIKey()
        {
            UIMap.LoginWithApiKey();
            UIMap.AssertLoggedIn();
        }

        [TestMethod]
        public void LoginWithUsernameAndPassword()
        {
            UIMap.LoginWithUsernameAndPassword();
            UIMap.AssertLoggedIn();
        }
    }
}
