namespace Senzing.Sdk.Tests.Core;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreConfigManagerTest : AbstractTest
{
    private const string CUSTOMERS_DATA_SOURCE = "CUSTOMERS";

    private SzCoreEnvironment? env;

    private string? defaultConfig;

    private string? modifiedConfig;

    private long defaultConfigID;

    private long modifiedConfigID;

    private const string DEFAULT_COMMENT = "Default";

    private const string MODIFIED_COMMENT = "Modified";

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
    public void initializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment(true);
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

            // get the config handle
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
            configHandle = 0;
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

    [Test, Order(5)]
    public void TestGetNativeApi()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreConfigManager configMgr
                    = (SzCoreConfigManager) this.Env.GetConfigManager();

                Assert.IsNotNull(configMgr.GetNativeApi(),
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

    [Test, Order(10)]
    public void TestAddConfigDefault()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.defaultConfigID = configMgr.AddConfig(this.defaultConfig,
                                                           DEFAULT_COMMENT);

                Assert.That(this.defaultConfigID, Is.Not.EqualTo(0L),
                            "Config ID is zero (0)");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testAddConfigDefault test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(20)]
    public void TestAddConfigModified()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.modifiedConfigID = configMgr.AddConfig(this.modifiedConfig,
                                                            MODIFIED_COMMENT);

                Assert.That(this.modifiedConfigID, Is.Not.EqualTo(0L),
                            "Config ID is zero (0)");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testAddConfigModified test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(30)]
    public void TestGetConfigDefault()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                string configDefinition = configMgr.GetConfig(this.defaultConfigID);

                Assert.That(configDefinition, Is.EqualTo(this.defaultConfig),
                             "Configuration retrieved is not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testGetConfigDefault test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(40)]
    public void TestGetConfigModified()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                string configDefinition = configMgr.GetConfig(this.modifiedConfigID);

                Assert.That(configDefinition, Is.EqualTo(this.modifiedConfig),
                             "Configuration retrieved is not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testGetConfigModified test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(50)]
    public void TestGetConfigs()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                string result = configMgr.GetConfigs();

                JsonObject jsonObj = ParseJsonObject(result);

                Assert.IsTrue(jsonObj.ContainsKey("CONFIGS"),
                              "Did not find CONFIGS in result");

                JsonArray? configs = (JsonArray?)jsonObj?["CONFIGS"];

                Assert.That(configs?.Count, Is.EqualTo(2),
                            "CONFIGS array not of expected size");

                ValidateJsonDataMapArray(configs, true,
                    "CONFIG_ID", "SYS_CREATE_DT", "CONFIG_COMMENTS");

                JsonObject? config0 = (JsonObject?)configs?[0];
                long? configID1 = (long?)config0?["CONFIG_ID"];
                string? comments1 = (string?)config0?["CONFIG_COMMENTS"];

                JsonObject? config1 = (JsonObject?)configs?[1];
                long? configID2 = (long?)config1?["CONFIG_ID"];
                string? comments2 = (string?)config1?["CONFIG_COMMENTS"];

                ISet<long> configIDs = new SortedSet<long>();
                configIDs.Add(defaultConfigID);
                configIDs.Add(modifiedConfigID);
                ISet<string> comments = new SortedSet<string>();
                comments.Add(DEFAULT_COMMENT);
                comments.Add(MODIFIED_COMMENT);

                Assert.IsTrue(configID1 != null && configIDs.Contains((long)configID1),
                              "First config ID not as expected");
                Assert.IsTrue(comments1 != null && comments.Contains((string)comments1),
                              "First config comment not as expected");

                Assert.IsTrue(configID2 != null && configIDs.Contains((long)configID2),
                              "Second config ID not as expected");
                Assert.IsTrue(comments2 != null && comments.Contains((string)comments2),
                              "Second config comment not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testGetConfigs test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(60)]
    public void TestGetDefaultConfigIDInitial()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                long configID = configMgr.GetDefaultConfigID();

                Assert.That(configID, Is.EqualTo(0L),
                            "Initial default config ID is not zero (0)");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testGetDefaultConfigIDInitial test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(70)]
    public void TestSetDefaultConfigID()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                configMgr.SetDefaultConfigID(this.defaultConfigID);

                long configID = configMgr.GetDefaultConfigID();

                Assert.That(configID, Is.EqualTo(this.defaultConfigID),
                            "Set default config ID is not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testSetDefaultConfigID test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(80)]
    public void TestReplaceDefaultConfigID()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                configMgr.ReplaceDefaultConfigID(this.defaultConfigID, this.modifiedConfigID);

                long configID = configMgr.GetDefaultConfigID();

                Assert.That(configID, Is.EqualTo(this.modifiedConfigID),
                            "Replaced default config ID is not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed testReplaceDefaultConfigID test with exception", e);
                throw;
            }
        });

    }

    [Test, Order(90)]
    public void TestNotReplaceDefaultConfigID()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                configMgr.ReplaceDefaultConfigID(this.defaultConfigID, this.modifiedConfigID);

                Fail("Replaced default config ID when it should not have been possible");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (SzReplaceConflictException)
            {
                // expected exception

            }
            catch (Exception e)
            {
                Fail("Failed testNotReplaceDefaultConfigID test with exception", e);
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
                SzCoreConfigManager configManager = (SzCoreConfigManager)
                    this.Env.GetConfigManager();

                NativeConfigManager nativeApi = configManager.GetNativeApi();

                nativeApi.ClearLastException();

                long configID = configManager.GetDefaultConfigID();

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
                Fail("Failed test configManager exception handling", e);
                throw;
            }
        });
    }
}
