using NUnit.Framework;
using Quandl.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //webMock.Setup(w => w.WhoAmI(fakeKey)).Returns(Task<new User()>());
            //Assert.IsTrue(typeof(webMock.Object.WhoAmI(fakeKey)), "");
            //Assert.
        }

        [Test()]
        public void SearchDatabasesAsyncTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SearchDatasetsAsyncTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void SearchDatasetAsyncTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void BrowseAsyncTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatasetDataTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatasetMetadataTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatatableDataTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void PostTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetResponseJsonTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetModelByIdsTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatabaseTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatatableCollectionTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetDatatableMetadataTest()
        {
            Assert.Fail();
        }
    }
}