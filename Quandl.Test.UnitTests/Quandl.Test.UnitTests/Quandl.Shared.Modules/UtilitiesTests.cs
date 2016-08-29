using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace Quandl.Shared.Tests
{
    [TestFixture()]
    public class UtilitiesTests
    {
        [Test()]
        public void GetValuesFromStringTest()
        {
            string excelFormulaArray = "abc,efg";
            var result = Utilities.GetValuesFromString(excelFormulaArray);
            
            Assert.AreEqual(typeof(List<string>), result.GetType());
            Assert.IsTrue(result[0].Equals(excelFormulaArray.ToUpper()));
        }

        [Test()]
        public void ListToUpperTest()
        {
            var lowerCaseList = new List<string>(new string[] {"abc", "efg", "hijk"});
            var upperCaseList = new List<string>(new string[] { "abc".ToUpper(), "efg".ToUpper(), "hijk".ToUpper() });
            var result = Utilities.ListToUpper(lowerCaseList);

            Assert.That(upperCaseList, Is.EquivalentTo(result));
        }

        [Test()]
        public void SubListTest()
        {
            var indexList = new ArrayList(new int[] {1,3,5});
            var sourceList = new ArrayList(new string[] {"zero","first","second","third","forth","fifth", "sixth"});
            var expectedList = new ArrayList(new string[] {"first","third","fifth"});
            var resultList = Utilities.SubList(indexList, sourceList);

            Assert.That(expectedList, Is.EquivalentTo(resultList));
        }

        [Test()]
        public void ValidateEmptyDataNotEmptyTest()
        {
            var quandl_data = "quandl data";
            var result = Utilities.ValidateEmptyData(quandl_data);
            Assert.IsTrue(quandl_data.Equals(result));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ValidateEmptyDataNullOrEmpty(string testData)
        {
            ActualValueDelegate<Object> testDelegate = () => Utilities.ValidateEmptyData(testData);
            Assert.That(testDelegate, Throws.TypeOf<QuandlDataNotFoundException>());
        }
    }
}