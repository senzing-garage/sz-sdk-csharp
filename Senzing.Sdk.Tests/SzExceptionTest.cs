namespace Senzing.Sdk.Tests;

using System;

using NUnit.Framework;

using Senzing.Sdk.Tests.Core;

using static System.StringComparison;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzExceptionTest : AbstractTest
{
    public static List<Type> GetExceptionTypes()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(SzException));
        types.Add(typeof(SzConfigurationException));
        types.Add(typeof(SzDatabaseConnectionLostException));
        types.Add(typeof(SzDatabaseException));
        types.Add(typeof(SzBadInputException));
        types.Add(typeof(SzLicenseException));
        types.Add(typeof(SzNotFoundException));
        types.Add(typeof(SzNotInitializedException));
        types.Add(typeof(SzReplaceConflictException));
        types.Add(typeof(SzRetryableException));
        types.Add(typeof(SzRetryTimeoutExceededException));
        types.Add(typeof(SzUnhandledException));
        types.Add(typeof(SzUnknownDataSourceException));
        types.Add(typeof(SzUnrecoverableException));
        return types;
    }

    public static IList<(Type, long)> GetErrorCodeParameters()
    {
        List<Type> types = GetExceptionTypes();
        List<long> codes = ListOf(10L, 20L, 30L, 40L);
        IList<System.Collections.IList> combos = GenerateCombinations(types, codes);

        IList<(Type, long)> result = new List<(Type, long)>(combos.Count);
        foreach (System.Collections.IList args in combos)
        {
            Type exceptionType = ((Type?)args[0]) ?? typeof(object);
            long errorCode = ((long?)args[1]) ?? 0L;

            result.Add((exceptionType, errorCode));
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestDefaultConstruct(Type exceptionType)
    {
        SzException sze;
        try
        {
            SzException? instance = (SzException?)Activator.CreateInstance(exceptionType);
            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);
            sze = (instance ?? new SzException());

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }
        Assert.That(sze.Message, Is.EqualTo("Exception of type '" + exceptionType + "' was thrown."),
                    "Exception message not null: " + exceptionType);
        Assert.IsNull(sze.InnerException, "Exception cause not null: " + exceptionType);
        Assert.IsNull(sze.ErrorCode, "Exception error code is not null: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestMessageConstruct(Type exceptionType)
    {
        string message = "Some Message";
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, message);

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(message));

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.Message, Is.EqualTo(message),
            "Exception message not as expected: " + exceptionType);
        Assert.IsNull(sze.InnerException, "Exception cause not null: " + exceptionType);
        Assert.IsNull(sze.ErrorCode, "Exception error code is not null: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
        string text = sze.ToString();
        Assert.IsTrue(text.Contains(message, Ordinal),
            "Exception message not found in string representation: " + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestCodeAndMessageConstruct(Type exceptionType)
    {
        String message = "Some Message";
        long errorCode = 105;
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, errorCode, message);

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(message));

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.Message, Is.EqualTo(message),
            "Exception message not as expected: " + exceptionType);
        Assert.That(sze.ErrorCode, Is.EqualTo(errorCode),
            "Exception error code is not as expected: " + exceptionType);
        Assert.IsNull(sze.InnerException, "Exception cause not null: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
        string text = sze.ToString();
        Assert.IsTrue(text.Contains(message, Ordinal),
            "Exception message not found in string representation: " + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestCauseConstruct(Type exceptionType)
    {
        InvalidOperationException cause = new InvalidOperationException();
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, cause);

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(cause));

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.InnerException, Is.EqualTo(cause),
                    "Exception cause not as expected: " + exceptionType);
        Assert.IsTrue(cause == sze.InnerException, "Exception cause is not referrentially equal: " + exceptionType);
        Assert.IsNull(sze.ErrorCode, "Exception error code is not null: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestMessageAndCauseConstruct(Type exceptionType)
    {
        Exception cause = new InvalidOperationException();
        String message = "Some Message";
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, message, cause);

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(message, cause));

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.Message, Is.EqualTo(message),
                    "Exception message not as expected: " + exceptionType);
        Assert.That(sze.InnerException, Is.EqualTo(cause),
                    "Exception cause not as expected: " + exceptionType);
        Assert.IsTrue(cause == sze.InnerException,
                      "Exception cause is not referrentially equal: " + exceptionType);
        Assert.IsNull(sze.ErrorCode, "Exception error code is not null: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
        string text = sze.ToString();
        Assert.IsTrue(text.Contains(message, Ordinal),
            "Exception message not found in string representation: " + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetExceptionTypes))]
    public void TestFullConstruct(Type exceptionType)
    {
        InvalidOperationException cause = new InvalidOperationException();
        String message = "Some Message";
        long errorCode = 105;
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, errorCode, message, cause);

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(errorCode, message, cause));

        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.Message, Is.EqualTo(message),
                    "Exception message not as expected: " + exceptionType);
        Assert.That(sze.InnerException, Is.EqualTo(cause),
                    "Exception cause not as expected: " + exceptionType);
        Assert.IsTrue(cause == sze.InnerException,
                      "Exception cause is not referrentially equal: " + exceptionType);
        Assert.That(sze.ErrorCode, Is.EqualTo(errorCode),
                    "Exception error code is not as expected: " + exceptionType);
        Assert.IsNotNull(sze.ToString(), "Exception string is null: " + exceptionType);
        string text = sze.ToString();
        Assert.IsTrue(text.Contains(message, Ordinal),
                      "Exception message not found in string representation: "
                      + exceptionType);
    }

    [Test, TestCaseSource(nameof(GetErrorCodeParameters))]
    public void TestGetErrorCode((Type exceptionType, long errorCode) args)
    {
        Type exceptionType = args.exceptionType;
        long errorCode = args.errorCode;
        SzException sze;
        try
        {
            SzException? instance = (SzException?)
                Activator.CreateInstance(exceptionType, errorCode, "Test");

            Assert.IsNotNull(instance, "Exception was not created: " + exceptionType);

            sze = (instance ?? new SzException(errorCode, "Test"));
        }
        catch (Exception e)
        {
            Fail("Failed to construct exception of type: " + exceptionType, e);
            throw;
        }

        Assert.That(sze.ErrorCode, Is.EqualTo(errorCode),
                    "Error code not as expected: " + exceptionType);
    }

}
