namespace Senzing.Sdk.Tests.Core;

using NUnit.Framework;
using System;
using Senzing.Sdk.Core;
using System.Text.Json;
using System.Text.Json.Nodes;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreProductTest : AbstractTest {
    private SzCoreEnvironment? env = null;


    private SzCoreEnvironment Env {
        get {
            if (this.env != null) {
                return this.env;
            } else {
                throw new InvalidOperationException(
                    "The SzEnvironment is null");
            }
        }
    }

    [OneTimeSetUp]
    public void InitializeEnvironment() {
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
    public void TeardownEnvironment() {
        try {
            if (this.env != null) {
                this.env.Destroy();
                this.env = null;
            }
            this.TeardownTestEnvironment();
        } finally {
            this.EndTests();
        }
    }

    [Test]
    public void TestGetNativeApi() {
        this.PerformTest(() => {
            try {
                SzCoreProduct product = (SzCoreProduct) this.Env.GetProduct();

                Assert.IsNotNull(product.GetNativeApi(),
                        "Underlying native API is unexpectedly null");
                
            } catch (Exception e) {
                Fail("Failed testGetNativeApi test with exception", e);
            }
        });
    }

    [Test]
    public void TestGetLicense() {
        this.PerformTest(() => {
            try {
                SzProduct product = this.Env.GetProduct();

                string license = product.GetLicense();

                JsonObject jsonData = ParseJsonObject(license);
                
                ValidateJsonDataMap(
                    jsonData,
                    "customer", "contract", "issueDate", "licenseType",
                    "licenseLevel", "billing", "expireDate", "recordLimit");

            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Failed testGetLicense test with exception", e);
            }
        });
    }

    [Test]
    public void TestGetVersion() {
        this.PerformTest(() => {
            try {
                SzProduct product = this.Env.GetProduct();

                string version = product.GetVersion();

                JsonObject jsonData = ParseJsonObject(version);

                ValidateJsonDataMap(
                    jsonData,
                    false,
                    "VERSION", "BUILD_NUMBER", "BUILD_DATE", "COMPATIBILITY_VERSION");
          
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Failed testGetVersion test with exception", e);
            }
        });
    }
}
