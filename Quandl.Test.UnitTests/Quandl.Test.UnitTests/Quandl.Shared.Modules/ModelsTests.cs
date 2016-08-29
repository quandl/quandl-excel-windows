using System;
using System.CodeDom;
using NUnit.Framework;
using Quandl.Shared.Models;

namespace Quandl.Test.UnitTests.Quandl.Shared.Modules
{
    [TestFixture()]
    public class ModelsTests
    {
        [Test()]
        public void DatabaseTest()
        {
            var database = new Database();
          
            Assert.AreEqual(typeof(int), GetType(database.Id));
            Assert.AreEqual(typeof(string), GetType(database.Name));
            Assert.AreEqual(typeof(string), GetType(database.DatabaseCode));
            Assert.AreEqual(typeof(string), GetType(database.Description));
            Assert.AreEqual(typeof(long), GetType(database.DatasetsCount));
            Assert.AreEqual(typeof(long), GetType(database.Downloads));
            Assert.AreEqual(typeof(bool), GetType(database.Premium));
            Assert.AreEqual(typeof(string), GetType(database.Image));
        }

        private Type GetType<T>(T obj)
        {
            return typeof(T);
        }
    }
}
