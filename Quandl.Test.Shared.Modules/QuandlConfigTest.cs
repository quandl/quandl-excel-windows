using NUnit.Framework;
using Quandl.Shared;

namespace Quandl.Test.Shared.Modules
{
    [TestFixture]
    public class QuandlConfigTests
    {
        [TearDown]
        public void Cleanup()
        {
            QuandlConfig.Reset();
        }

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
    }
}