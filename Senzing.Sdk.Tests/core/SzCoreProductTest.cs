namespace Senzing.Sdk.Tests.Core;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreProductTest : AbstractTest
{
    private SzCoreEnvironment? env;


    private SzCoreEnvironment Env
    {
        get
        {
            if (this.env != null)
            {
                return this.env;
            }
            else
            {
                throw new InvalidOperationException(
                    "The SzEnvironment is null");
            }
        }
    }

    [OneTimeSetUp]
    public void InitializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment();
        string settings = this.GetRepoSettings();

        string instanceName = this.GetType().Name;

        this.env = SzCoreEnvironment.NewBuilder()
                                    .InstanceName(instanceName)
                                    .Settings(settings)
                                    .VerboseLogging(false)
                                    .Build();
    }

    [OneTimeTearDown]
    public void TeardownEnvironment()
    {
        try
        {
            if (this.env != null)
            {
                this.env.Destroy();
                this.env = null;
            }
            this.TeardownTestEnvironment();
        }
        finally
        {
            this.EndTests();
        }
    }

    [Test]
    public void TestGetNativeApi()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreProduct product = (SzCoreProduct)this.Env.GetProduct();

                Assert.IsNotNull(product.GetNativeApi(),
                        "Underlying native API is unexpectedly null");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testGetNativeApi test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestGetLicense()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzProduct product = this.Env.GetProduct();

                string license = product.GetLicense();

                JsonObject jsonData = ParseJsonObject(license);

                ValidateJsonDataMap(
                    jsonData,
                    "customer", "contract", "issueDate", "licenseType",
                    "licenseLevel", "billing", "expireDate", "recordLimit");

            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception e)
            {
                Fail("Failed testGetLicense test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestGetVersion()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzProduct product = this.Env.GetProduct();

                string version = product.GetVersion();

                JsonObject jsonData = ParseJsonObject(version);

                ValidateJsonDataMap(
                    jsonData,
                    false,
                    "VERSION", "BUILD_NUMBER", "BUILD_DATE", "COMPATIBILITY_VERSION");

            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception e)
            {
                Fail("Failed testGetVersion test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestExceptionFunctions()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreProduct product = (SzCoreProduct)this.Env.GetProduct();

                NativeProduct nativeApi = product.GetNativeApi();

                nativeApi.ClearLastException();

                string version = product.GetVersion();

                string message = nativeApi.GetLastException();
                long errorCode = nativeApi.GetLastExceptionCode();

                Assert.That(message, Is.EqualTo(""),
                            "Unexpected exception message: " + message);

                Assert.That(errorCode, Is.EqualTo(0),
                            "Unexpeted error code: " + errorCode);
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception e)
            {
                Fail("Failed testGetVersion test with exception", e);
                throw;
            }
        });
    }
}
