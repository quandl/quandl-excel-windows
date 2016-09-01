## Unit testing
Building c# unit testing project using unit test framework [NUnit](http://www.nunit.org/) and mocking framework [Moq](https://github.com/moq/moq4)

### Why NUnit
- Like many other C# unit test framework, NUnit test is a C# clone of JUnit. So, most C# unit test framework can almost accomplish the same functions but with very little defference.
- One of the advatage of Nunit over MSTest is: NUnit is open sourced and it has better integration with other third party unit test librares(such as Mono, Moq ans such).
- NUnit is one of the acively maintained C# unit test framework during the past 10 years.
- Before NUnit lack functions like parallel execution and extensibility, but recently released NUnit 3 bring these features to user(Note: this project use NUnit 3).
- NUnit doesn't support [DNX core](https://www.simple-talk.com/dotnet/net-framework/what-is-dnx/), but it is not applicatable to this project.
- On the other hand, based on different needs, choosing between diffreent C# unit test frameworks could be different from project to prject.

### Setup
Go to Visual studio menu: Tools -> Extensions and Updates, install following two packages, these two packages install unit test runner and Visual studio UI integration with NUnit test.
- NUnit 3 Test Adapter
- Test Generator NUnit extension

### First NUnit test case
```
using NUnit.Framework;
using Quandl.Shared;
using System;

namespace Quandl.Shared.Tests
{
   [TestFixture()]
   public class QuandlConfigTests
   {
       [TestCase]
       public void TestApiKeySetGet()
       {
           QuandlConfig.ApiKey = "foo";
           Assert.AreEqual("foo", QuandlConfig.ApiKey);
       }
   }
}
```
- For more detail, check documentation for nunit: [https://github.com/nunit/docs/wiki/NUnit-Documentation](https://github.com/nunit/docs/wiki/NUnit-Documentation)
- Also, here is a very useful C# Demo project: [https://github.com/nunit/nunit-csharp-samples](https://github.com/nunit/nunit-csharp-samples)

### Mocking framework
- [Moq](https://github.com/moq/moq4) is the most popular mocking framework for .Net and actively maintained. Moq can mocking oject which is interface based or function declared as virtual.
- Here is example to mock a web request

```
public void AuthenticateWithCredentialsTest()
{
    // declare mock oject, here mocked object could be any public class
    Mock<Web> webMock = new Mock<Web>();
    // setup for test
    var obj = new { user = new { account = "account", password = "password" } };
    var payload = JsonConvert.SerializeObject(obj);
    JObject userJson = JObject.Parse(@"{'user': {'api_key': 'api_key'}}");

    // mocking setup
    // function arguments(for example payload here) should match the actualy function calls
    // otherwise mocking won't work
    webMock.Setup(w => w.Authenticate(payload)).Returns(userJson);
    try
    {
        QuandlConfig.AuthenticateWithCredentials(webMock.Object, "account", "password");
    }
    // Since function QuandlConfig.AuthenticateWithCredentials return nothing,
    // for this test case, we just make sure it won't throw any exception
    catch (Exception e)
    {
        Assert.Fail("QuandlConfig.AuthenticateWithCredentials should not throw exception:" + e.Message);
    }
}
```
- For any further details, check Moq API: http://www.nudoq.org/#!/Projects/Moq
