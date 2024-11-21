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
                SzCoreConfig config = (SzCoreConfig)this.Env.GetConfig();

                Assert.IsNotNull(config.GetNativeApi(),
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
    public void TestCreateConfig()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.CreateConfig();

                    Assert.That(configHandle, Is.Not.EqualTo(0),
                                 "Config handle was zero (0)");

                    string configJson = config.ExportConfig(configHandle);

                    Assert.That(configJson, Is.EqualTo(this.defaultConfig),
                                "Unexpected configuration definition.");

                }
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testCreateConfig test with exception", e);
                throw;
            }
        });
    }

    [Test]
    public void TestImportConfig()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.ImportConfig(this.modifiedConfig);

                    Assert.That(configHandle, Is.Not.EqualTo(0),
                                "Config handle was zero (0)");

                    string configJson = config.ExportConfig(configHandle);

                    Assert.That(configJson, Is.EqualTo(this.modifiedConfig),
                                "Unexpected configuration definition.");

                }
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testImportConfig test with exception", e);
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
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.ImportConfig(this.modifiedConfig);
                    string configJson = config.ExportConfig(configHandle);

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
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testExportConfig test with exception", e);
            }
        });
    }

    [Test]
    public void TestCloseConfig()
    {
        try
        {
            SzConfig config = this.Env.GetConfig();

            IntPtr configHandle = 0;

            try
            {
                configHandle = config.CreateConfig();

                config.CloseConfig(configHandle);

                // now try to use the handle that has been closed
                try
                {
                    config.ExportConfig(configHandle);
                    Fail("The configuration handle was still valid after closing");

                }
                catch (SzException)
                {
                    // success if we get here

                }
                finally
                {
                    // clear the config handle
                    configHandle = 0;
                }

            }
            finally
            {
                if (configHandle != 0) config.CloseConfig(configHandle);
            }

        }
        catch (AssertionException)
        {
            throw;

        }
        catch (Exception e)
        {
            Fail("Failed testCloseConfig test with exception", e);
            throw;
        }

    }

    [Test]
    public void TestAddDataSource()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.CreateConfig();

                    string result = config.AddDataSource(configHandle, EMPLOYEES_DATA_SOURCE);

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

                    string configJson = config.ExportConfig(configHandle);

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
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

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
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.ImportConfig(this.modifiedConfig);

                    config.DeleteDataSource(configHandle, "CUSTOMERS");

                    string configJson = config.ExportConfig(configHandle);

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
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testDeleteDataSource test with exception", e);
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
                SzConfig config = this.Env.GetConfig();

                IntPtr configHandle = 0;

                try
                {
                    configHandle = config.ImportConfig(this.modifiedConfig);

                    string dataSources = config.GetDataSources(configHandle);

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
                finally
                {
                    if (configHandle != 0) config.CloseConfig(configHandle);
                }

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed getDataSources test with exception", e);
            }
        });
    }
}
