using NUnit.Framework;
using Quandl.Shared;

namespace Quandl.Test.Shared.Modules
{
    [TestFixture]
    public class QuandlConfigTests
    {
        [TestCase]
        public void TestApiKeySetGet()
        {
            QuandlConfig.ApiKey = "foo";
            Assert.AreEqual("foo", QuandlConfig.ApiKey);
        }

        [TestCase]
        public void TestQuandlConfigResetSettings()
        {
            QuandlConfig.ApiKey = "foo";
            QuandlConfig.Reset();
            Assert.IsEmpty(QuandlConfig.ApiKey);
        }

        [TearDown]
        public void Cleanup()
        {
            QuandlConfig.Reset();
        }
    }
}
