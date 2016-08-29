using NUnit.Framework;
using System.Threading.Tasks;
using Moq;
using Quandl.Shared.Models;

namespace Quandl.Shared.Tests
{
    [TestFixture()]
    public class WebTests
    {
        [Test()]
        public void WhoAmITest()
        {
            Mock<Web> webMock = new Mock<Web>();
            string fakeKey = "abc";
            User user = new User();
            webMock.Setup(w => w.WhoAmI(fakeKey)).Returns(Task.FromResult(user));
            var userTask = webMock.Object.WhoAmI(fakeKey).Result.GetType();
            Assert.IsTrue(userTask.Name.Equals("User"));
        }

        [Test()]
        public void SearchDatabasesAsyncTest()
        {

        }

        [Test()]
        public void SearchDatasetsAsyncTest()
        {

        }

        [Test()]
        public void SearchDatasetAsyncTest()
        {

        }

        [Test()]
        public void BrowseAsyncTest()
        {

        }

        [Test()]
        public void GetDatasetDataTest()
        {

        }

        [Test()]
        public void GetDatasetMetadataTest()
        {

        }

        [Test()]
        public void GetDatatableDataTest()
        {

        }

        [Test()]
        public void PostTest()
        {

        }

        [Test()]
        public void GetResponseJsonTest()
        {

        }

        [Test()]
        public void GetModelByIdsTest()
        {

        }

        [Test()]
        public void GetDatabaseTest()
        {

        }

        [Test()]
        public void GetDatatableCollectionTest()
        {

        }

        [Test()]
        public void GetDatatableMetadataTest()
        {

        }
    }
}