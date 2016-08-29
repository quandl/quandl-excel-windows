using NUnit.Framework;
using Quandl.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quandl.Shared.Tests
{
    [TestFixture()]
    public class QuandlConfigTests
    {
        [SetUp,TearDown]
        public void Cleanup()
        {
            try
            {
                QuandlConfig.Reset();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Cannot delete a subkey tree because the subkey does not exist."))
                    throw;
                // do nothing if key not exist
            }

            Assert.That(null, Is.Null);
        }

        [Test()]
        public void AuthenticateWithCredentialsTest()
        {
            Mock<Web> webMock = new Mock<Web>();
            var obj = new { user = new { account = "account", password = "password" } };
            var payload = JsonConvert.SerializeObject(obj);
            JObject userJson = JObject.Parse(@"{'user': {'api_key': 'api_key'}}");
            webMock.Setup(w => w.Authenticate(payload)).Returns(userJson);
            try
            {
                QuandlConfig.AuthenticateWithCredentials(webMock.Object, "account", "password");
            }
            catch (Exception e)
            { 
                Assert.Fail("QuandlConfig.AuthenticateWithCredentials should not throw exception:" + e.Message);
            }
            
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
            Assert.IsNull(QuandlConfig.ApiKey);
        }
    }
}