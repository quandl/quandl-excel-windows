using NUnit.Framework;
using Quandl.Shared;

namespace UnitTestSharedModules
{
    [TestFixture]
    public class TestFunctionsTest
    {
        [TestCase]
        public void TestAuthToken()
        {
            var data = Utilities.AuthToken("foobar@test.com", "12345678");
            Assert.IsNotNull(data);
        }
    }
}
