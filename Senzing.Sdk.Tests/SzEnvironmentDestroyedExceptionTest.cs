namespace Senzing.Sdk.Tests;

using System;

using NUnit.Framework;

using Senzing.Sdk.Tests.Core;
using Senzing.Sdk.Tests.Util;

using static System.StringComparison;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzEnvironmentDestroyedExceptionTest : AbstractTest
{
    [Test]
    public void TestDefaultConstruct()
    {
        this.PerformTest(() =>
        {
            try
            {
                InvalidOperationException ioe = new InvalidOperationException();
                Exception e = new SzEnvironmentDestroyedException();
                Assert.That(e.Message, Is.EqualTo(ioe.Message),
                            "Exception message was not as expected");
                Assert.IsNull(e.InnerException, "Exception cause was not null");
                Assert.IsNotNull(e.ToString(), "Exception string is null");

            }
            catch (Exception e)
            {
                Fail("Unexpected exception", e);
            }
        });
    }

    [Test]
    public void TestMessageConstruct()
    {
        this.PerformTest(() =>
        {
            try
            {
                String message = TextUtilities.RandomAlphanumericText(20);
                Exception e = new SzEnvironmentDestroyedException(message);
                Assert.That(e.Message, Is.EqualTo(message),
                            "Exception message not as expected");
                Assert.IsNull(e.InnerException, "Exception cause was not null");
                Assert.IsNotNull(e.ToString(), "Exception string is null");
                string text = e.ToString();
                Assert.IsTrue(text.Contains(message, StringComparison.Ordinal),
                              "Exception message not found in string representation");

            }
            catch (Exception e)
            {
                Fail("Unexpected exception", e);
            }
        });
    }

    [Test]
    public void TestCauseConstruct()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzException cause = new SzException();
                Exception e = new SzEnvironmentDestroyedException(cause);
                Assert.IsNotNull(e.Message, "Exception message was unexpectedly null");
                Assert.That(e.InnerException, Is.EqualTo(cause),
                             "Exception cause not as expected");
                Assert.IsTrue(cause == e.InnerException,
                              "Exception cause is not referrentially equal");
                Assert.IsNotNull(e.ToString(), "Exception string is null");

            }
            catch (Exception e)
            {
                Fail("Unexpected exception", e);
            }
        });
    }

    [Test]
    public void TestFullConstruct()
    {
        this.PerformTest(() =>
        {
            try
            {
                String message = TextUtilities.RandomAlphanumericText(20);
                SzException cause = new SzException();
                Exception e = new SzEnvironmentDestroyedException(message, cause);
                Assert.That(e.Message, Is.EqualTo(message),
                            "Exception message was not as expected");
                Assert.That(e.InnerException, Is.EqualTo(cause),
                             "Exception cause not as expected");
                Assert.IsTrue(cause == e.InnerException,
                              "Exception cause is not referrentially equal");
                Assert.IsNotNull(e.ToString(), "Exception string is null");
                string text = e.ToString();
                Assert.IsTrue(text.Contains(message, StringComparison.Ordinal),
                              "Exception message not found in string representation");

            }
            catch (Exception e)
            {
                Fail("Unexpected exception", e);
            }
        });
    }
}
