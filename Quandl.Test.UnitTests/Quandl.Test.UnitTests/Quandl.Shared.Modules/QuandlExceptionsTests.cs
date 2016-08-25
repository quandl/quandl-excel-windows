using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace Quandl.Shared.Tests
{
    [TestFixture()]
    public class QuandlExceptionsTests
    {
        [Test()]
        public void QuandlDataNotFoundExceptionTest()
        {
            QuandlDataNotFoundException exp =
                Assert.Throws<QuandlDataNotFoundException>(
                    delegate { throw new QuandlDataNotFoundException(); });
            Assert.That(exp.Message, Is.EqualTo("Quandl data is not found!"));
        }

        [Test()]
        public void QuandlDateCanNotBlankExceptionTest()
        {
            QuandlDateCanNotBlankException exp =
                Assert.Throws<QuandlDateCanNotBlankException>(
                    delegate { throw new QuandlDateCanNotBlankException(); });
            Assert.That(exp.Message, Is.EqualTo("Date can not be blank!"));
        }

        [Test()]
        public void QuandlInvalidDateFormatExceptionTest()
        {
            QuandlInvalidDateFormatException exp =
                Assert.Throws<QuandlInvalidDateFormatException>(
                    delegate { throw new QuandlInvalidDateFormatException(); });
            Assert.That(exp.Message, Is.EqualTo("Invalid date fromat!"));
        }

        [Test()]
        public void QuandlFromDateIsGreaterThanEndDateExceptionTest()
        {
            QuandlFromDateIsGreaterThanEndDateException exp =
                Assert.Throws<QuandlFromDateIsGreaterThanEndDateException>(
                     delegate { throw new QuandlFromDateIsGreaterThanEndDateException(); });
            Assert.That(exp.Message, Is.EqualTo("Start date is greater than end date!"));
        }
    }
}