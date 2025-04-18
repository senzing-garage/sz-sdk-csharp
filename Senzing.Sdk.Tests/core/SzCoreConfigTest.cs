namespace Senzing.Sdk.Tests.Core;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreConfigTest : AbstractTest
{

    private const string CUSTOMERS_DATA_SOURCE = "CUSTOMERS";

    private const string EMPLOYEES_DATA_SOURCE = "EMPLOYEES";

    private SzCoreEnvironment? env;

    private string? defaultConfig;

    private string? modifiedConfig;

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

        NativeConfig nativeConfig = new NativeConfigExtern();
        IntPtr configHandle = 0;
        try
        {
            // initialize the native config
            long returnCode = nativeConfig.Init(instanceName, settings, false);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // create the default config
            returnCode = nativeConfig.Create(out IntPtr handle);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }
            configHandle = handle;

            // export the default config JSON
            returnCode = nativeConfig.Save(configHandle, out string config);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // set the default config
            this.defaultConfig = config;

            // add the data source
            returnCode = nativeConfig.AddDataSource(configHandle,
                "{\"DSRC_CODE\": \"" + CUSTOMERS_DATA_SOURCE + "\"}",
                out string result);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // export the modified config JSON
            returnCode = nativeConfig.Save(configHandle, out string modConfig);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // set the modified config
            this.modifiedConfig = modConfig;

            // close the config handle
            returnCode = nativeConfig.Close(configHandle);
            configHandle = 0;
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

        }
        finally
        {
            if (configHandle != 0) nativeConfig.Close(configHandle);
            nativeConfig.Destroy();
        }

        this.env = SzCoreEnvironment.NewBuilder()
                                    .InstanceName(instanceName)
                                    .Settings(settings)
                                    .VerboseLogging(false)
                                    .Build();
    }

    [OneTimeTearDown]
    public void teardownEnvironment()
    {
        try
        {
            if (this.env != null)
            {
                this.Env.Destroy();
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
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzCoreConfig config = (SzCoreConfig)configMgr.CreateConfig();

                Assert.IsNotNull(config.GetNativeApi(),
                      "Underlying native API is unexpectedly null");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestGetNativeApi test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestCreateConfigFromTemplate()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig();

                Assert.That(config, Is.Not.Null, "SzConfig should not be null");

                Assert.That(((SzCoreConfig)config).GetNativeApi(), Is.Not.Null,
                            "Underlying native API is unexpectedly null");

                string configJson = config.Export();

                Assert.That(configJson, Is.EqualTo(this.defaultConfig),
                            "Unexpected configuration definition.");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestCreateConfigFromTemplate test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestCreateConfigFromDefinition()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig(this.modifiedConfig);

                Assert.That(config, Is.Not.Null, "SzConfig should not be null");

                Assert.That(((SzCoreConfig)config).GetNativeApi(), Is.Not.Null,
                            "Underlying native API is unexpectedly null");

                string configJson = config.Export();

                Assert.That(configJson, Is.EqualTo(this.modifiedConfig),
                            "Unexpected configuration definition.");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestCreateConfigFromDefinition test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestExportConfig()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig(this.modifiedConfig);

                string configJson = config.Export();

                Assert.That(configJson, Is.EqualTo(this.modifiedConfig),
                            "Unexpected configuration definition.");

                JsonObject jsonObj = ParseJsonObject(configJson);

                Assert.IsTrue(jsonObj.ContainsKey("G2_CONFIG"),
                                "Config JSON is missing G2_CONFIG property: "
                                + configJson);

                JsonObject? g2Config = (JsonObject?)jsonObj["G2_CONFIG"];

                Assert.IsTrue(g2Config?.ContainsKey("CFG_DSRC"),
                                "Config JSON is missing CFG_DSRC property: "
                                + configJson);

                JsonArray? cfgDsrc = (JsonArray?)g2Config?["CFG_DSRC"];

                Assert.That(cfgDsrc?.Count, Is.EqualTo(3),
                            "Data source array is wrong size");

                JsonObject? customerDataSource = (JsonObject?)cfgDsrc?[2];
                string? dsrcCode = (string?)customerDataSource?["DSRC_CODE"];

                Assert.That(dsrcCode, Is.EqualTo(CUSTOMERS_DATA_SOURCE),
                            "Third data source is not as expected");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestExportConfig test with exception", e);
            }
        });
    }

    [Test]
    public void TestAddDataSource()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig();

                string result = config.AddDataSource(EMPLOYEES_DATA_SOURCE);

                JsonObject? resultObj = null;
                try
                {
                    resultObj = ParseJsonObject(result);
                }
                catch (Exception e)
                {
                    Fail("The AddDataSource() result did not parse as JSON: " + result, e);
                }

                int? resultId = (int?)resultObj?["DSRC_ID"];

                Assert.IsNotNull(resultId,
                                    "The DSRC_ID was missing or null in the result: "
                                    + result);

                string configJson = config.Export();

                JsonObject jsonObj = ParseJsonObject(configJson);

                Assert.IsTrue(jsonObj.ContainsKey("G2_CONFIG"),
                                "Config JSON is missing G2_CONFIG property: "
                                + configJson);

                JsonObject? g2Config = (JsonObject?)jsonObj?["G2_CONFIG"];

                Assert.IsTrue(g2Config?.ContainsKey("CFG_DSRC"),
                                "Config JSON is missing CFG_DSRC property: "
                                + configJson);

                JsonArray? cfgDsrc = (JsonArray?)g2Config?["CFG_DSRC"];

                Assert.That(cfgDsrc?.Count, Is.EqualTo(3),
                            "Data source array is wrong size");

                JsonObject? customerDataSource = (JsonObject?)cfgDsrc?[2];
                string? dsrcCode = (string?)customerDataSource?["DSRC_CODE"];

                Assert.That(dsrcCode, Is.EqualTo(EMPLOYEES_DATA_SOURCE),
                            "Third data source is not as expected");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testAddDataSource test with exception", e);
            }
        });
    }

    [Test]
    public void TestDeleteDataSource()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig(this.modifiedConfig);

                config.DeleteDataSource("CUSTOMERS");

                string configJson = config.Export();

                JsonObject jsonObj = ParseJsonObject(configJson);

                Assert.IsTrue(jsonObj.ContainsKey("G2_CONFIG"),
                                "Config JSON is missing G2_CONFIG property: "
                                + configJson);

                JsonObject? g2Config = (JsonObject?)jsonObj?["G2_CONFIG"];

                Assert.IsTrue(g2Config?.ContainsKey("CFG_DSRC"),
                                "Config JSON is missing CFG_DSRC property: "
                                + configJson);

                JsonArray? cfgDsrc = (JsonArray?)g2Config?["CFG_DSRC"];

                Assert.That(cfgDsrc?.Count, Is.EqualTo(2),
                            "Data source array is wrong size");

                JsonObject? dataSource1 = (JsonObject?)cfgDsrc?[0];
                string? dsrcCode1 = (string?)dataSource1?["DSRC_CODE"];

                Assert.That(dsrcCode1, Is.EqualTo("TEST"),
                            "First data source is not as expected");

                JsonObject? dataSource2 = (JsonObject?)cfgDsrc?[1];
                string? dsrcCode2 = (string?)dataSource2?["DSRC_CODE"];

                Assert.That(dsrcCode2, Is.EqualTo("SEARCH"),
                            "Second data source is not as expected");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestDeleteDataSource test with exception", e);
            }
        });

    }

    [Test]
    public void TestGetDataSources()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzConfig config = configMgr.CreateConfig(this.modifiedConfig);

                string dataSources = config.GetDataSources();

                Assert.IsNotNull(dataSources, "Data sources result was null");

                JsonObject jsonObj = ParseJsonObject(dataSources);

                Assert.IsTrue(jsonObj.ContainsKey("DATA_SOURCES"),
                                "JSON is missing DATA_SOURCES property: "
                                + dataSources);

                JsonArray? jsonArray = (JsonArray?)jsonObj?["DATA_SOURCES"];

                Assert.That(jsonArray?.Count, Is.EqualTo(3),
                            "Data sources JSON array is wrong size.");

                JsonObject? dataSource1 = (JsonObject?)jsonArray?[0];
                string? dsrcCode1 = (string?)dataSource1?["DSRC_CODE"];

                Assert.That(dsrcCode1, Is.EqualTo("TEST"),
                            "First data source is not as expected");

                JsonObject? dataSource2 = (JsonObject?)jsonArray?[1];
                string? dsrcCode2 = (string?)dataSource2?["DSRC_CODE"];

                Assert.That(dsrcCode2, Is.EqualTo("SEARCH"),
                            "Second data source is not as expected");

                JsonObject? dataSource3 = (JsonObject?)jsonArray?[2];
                string? dsrcCode3 = (string?)dataSource3?["DSRC_CODE"];

                Assert.That(dsrcCode3, Is.EqualTo(CUSTOMERS_DATA_SOURCE),
                            "Third data source is not as expected");
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed GetDataSources test with exception", e);
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
                SzConfigManager configMgr = this.Env.GetConfigManager();
                SzCoreConfig config = (SzCoreConfig)configMgr.CreateConfig();

                NativeConfig nativeApi = config.GetNativeApi();

                nativeApi.ClearLastException();

                config.GetDataSources();

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
                Fail("Failed test config exception handling", e);
                throw;
            }
        });
    }
}
