namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreConfigManagerTest : AbstractTest
{
    private const string CustomersDataSource = "CUSTOMERS";

    private const string EmployeesDataSource = "EMPLOYEES";

    private const string WatchlistDataSource = "WATCHLIST";

    private SzCoreEnvironment? env;

    private string? defaultConfig;

    private string? modifiedConfig1;

    private string? modifiedConfig2;

    private string? modifiedConfig3;

    private long defaultConfigID;

    private long modifiedConfigID1;

    private long modifiedConfigID2;

    private long modifiedConfigID3;

    private const string ModifiedComment1 = "Modified: CUSTOMERS";

    private const string ModifiedComment3 = "Modified: CUSTOMERS, EMPLOYEES, WATCHLIST";

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

            // add the CUSTOMERS data source
            returnCode = nativeConfig.AddDataSource(configHandle,
                "{\"DSRC_CODE\": \"" + CustomersDataSource + "\"}",
                out string result1);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // export the modified config JSON
            returnCode = nativeConfig.Save(configHandle, out string modConfig1);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // set the modified config
            this.modifiedConfig1 = modConfig1;

            // add the EMPLOYEES data source
            returnCode = nativeConfig.AddDataSource(configHandle,
                "{\"DSRC_CODE\": \"" + EmployeesDataSource + "\"}",
                out string result2);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // export the modified config JSON
            returnCode = nativeConfig.Save(configHandle, out string modConfig2);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // set the modified config
            this.modifiedConfig2 = modConfig2;

            // add the WATCHLIST data source
            returnCode = nativeConfig.AddDataSource(configHandle,
                "{\"DSRC_CODE\": \"" + WatchlistDataSource + "\"}",
                out string result3);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // export the modified config JSON
            returnCode = nativeConfig.Save(configHandle, out string modConfig3);
            if (returnCode != 0)
            {
                throw new TestException(nativeConfig.GetLastException());
            }

            // set the modified config
            this.modifiedConfig3 = modConfig3;

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
    public void TestGetConfigApi()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreConfigManager configMgr
                    = (SzCoreConfigManager)this.Env.GetConfigManager();

                Assert.IsNotNull(configMgr.GetConfigApi(),
                                 "Underlying config API is unexpectedly null");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestGetConfigApi test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(10)]
    public void TestRegisterConfigDefault()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.defaultConfigID = configMgr.RegisterConfig(this.defaultConfig);

                Assert.That(this.defaultConfigID, Is.Not.EqualTo(0L), "Config ID is zero (0)");

            }
            catch (Exception e)
            {
                Fail("Failed TestRegisterConfigDefault test with exception", e);
            }
        });
    }

    [Test, Order(20)]
    public void TestRegisterConfigDefaultWithComment()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.modifiedConfigID1 = configMgr.RegisterConfig(this.modifiedConfig1, ModifiedComment1);

                Assert.That(this.modifiedConfigID1, Is.Not.EqualTo(0L), "Config ID is zero (0)");

            }
            catch (Exception e)
            {
                Fail("Failed TestRegisterConfigDefaultWithComment test with exception", e);
            }
        });
    }

    [Test, Order(25)]
    public void TestGetConfigManagerApi()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreConfigManager configMgr = (SzCoreConfigManager)this.Env.GetConfigManager();

                Assert.That(configMgr.GetConfigManagerApi(), Is.Not.Null,
                            "Underlying native config manager API is unexpectedly null");

            }
            catch (Exception e)
            {
                Fail("Failed testGetConfigManagerApi test with exception", e);
            }
        });
    }

    [Test, Order(30)]
    public void TestRegisterConfigModified()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.modifiedConfigID2 = configMgr.RegisterConfig(this.modifiedConfig2);

                Assert.That(this.modifiedConfigID2, Is.Not.EqualTo(0L), "Config ID is zero (0)");

            }
            catch (Exception e)
            {
                Fail("Failed testAddConfigModified test with exception", e);
            }
        });
    }

    [Test, Order(40)]
    public void TestRegisterConfigModifiedWithComment()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                this.modifiedConfigID3 = configMgr.RegisterConfig(this.modifiedConfig3, ModifiedComment3);

                Assert.That(this.modifiedConfigID3, Is.Not.EqualTo(0L), "Config ID is zero (0)");

            }
            catch (Exception e)
            {
                Fail("Failed testAddConfigModified test with exception", e);
            }
        });
    }

    private static List<object?[]> CreateConfigFromConfigIDParameters()
    {
        List<object?[]> result = new List<object?[]>(4);
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.defaultConfigID,
            (SzCoreConfigManagerTest t) => t.defaultConfig });
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfigID1,
            (SzCoreConfigManagerTest t) => t.modifiedConfig1 });
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfigID2,
            (SzCoreConfigManagerTest t) => t.modifiedConfig2 });
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfigID3,
            (SzCoreConfigManagerTest t) => t.modifiedConfig3 });
        return result;
    }

    [Test, Order(50), TestCaseSource(nameof(CreateConfigFromConfigIDParameters))]
    public void TestCreateConfigFromConfigID(
        Func<SzCoreConfigManagerTest, long> configIDFunc,
        Func<SzCoreConfigManagerTest, string> expectedDefFunc)
    {
        long configID = configIDFunc(this);
        string expectedDefinition = expectedDefFunc(this);

        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                SzConfig config = configMgr.CreateConfig(configID);

                Assert.That(config, Is.Not.Null, "SzConfig should not be null");

                Assert.That(((SzCoreConfig)config).GetNativeApi(), Is.Not.Null,
                            "Underlying native API is unexpectedly null");

                string configDefinition = config.Export();

                Assert.That(configDefinition, Is.EqualTo(expectedDefinition),
                             "Configuration retrieved is not as expected: configId=[ "
                             + configID + " ]");

            }
            catch (Exception e)
            {
                Fail("Failed TestCreateConfigFromConfigID test with exception", e);
            }
        });
    }

    [Test, Order(60)]
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

                Assert.That(configs?.Count, Is.EqualTo(4),
                            "CONFIGS array not of expected size");

                ValidateJsonDataMapArray(configs, true,
                    "CONFIG_ID", "SYS_CREATE_DT", "CONFIG_COMMENTS");

                JsonObject? config0 = (JsonObject?)configs?[0];
                long? configID1 = (long?)config0?["CONFIG_ID"];
                string? comments1 = (string?)config0?["CONFIG_COMMENTS"];

                JsonObject? config1 = (JsonObject?)configs?[1];
                long? configID2 = (long?)config1?["CONFIG_ID"];
                string? comments2 = (string?)config1?["CONFIG_COMMENTS"];

                List<long> actualConfigIDs = new List<long>(4);
                List<string> actualComments = new List<string>(4);

                for (int index = 0; index < 4; index++)
                {
                    JsonObject? config = (JsonObject?)configs?[index];
                    actualConfigIDs.Add((long?)config?["CONFIG_ID"] ?? 0L);
                    actualComments.Add((string?)config?["CONFIG_COMMENTS"] ?? "BAD");
                }

                ISet<long> configIDs = new SortedSet<long>();
                configIDs.Add(this.defaultConfigID);
                configIDs.Add(this.modifiedConfigID1);
                configIDs.Add(this.modifiedConfigID2);
                configIDs.Add(this.modifiedConfigID3);

                ISet<string> comments = new SortedSet<string>();
                comments.Add("Data Sources: [ ONLY DEFAULT ]");
                comments.Add(ModifiedComment1);
                comments.Add("Data Sources: CUSTOMERS, EMPLOYEES");
                comments.Add(ModifiedComment3);

                for (int index = 0; index < actualConfigIDs.Count; index++)
                {
                    long configID = actualConfigIDs[index];
                    string comment = actualComments[index];

                    Assert.That(configIDs.Contains(configID),
                        "Config ID (" + index + ") not as expected: actual=[ "
                        + configID + " ], expected=[ " + configIDs + " ]");

                    Assert.That(comments.Contains(comment),
                        "Comments (" + index + ") not as expecte: actual=[ "
                        + comment + " ], expected=[ " + comments + " ]");
                }
            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestGetConfigs test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(70)]
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
                Fail("Failed TestGetDefaultConfigIDInitial test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(80)]
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
                Fail("Failed TestSetDefaultConfigID test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(90)]
    public void TestReplaceDefaultConfigID()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                configMgr.ReplaceDefaultConfigID(this.defaultConfigID, this.modifiedConfigID1);

                long configID = configMgr.GetDefaultConfigID();

                Assert.That(configID, Is.EqualTo(this.modifiedConfigID1),
                            "Replaced default config ID is not as expected");

            }
            catch (AssertionException)
            {
                throw;

            }
            catch (Exception e)
            {
                Fail("Failed TestReplaceDefaultConfigID test with exception", e);
                throw;
            }
        });

    }

    [Test, Order(100)]
    public void TestNotReplaceDefaultConfigID()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                configMgr.ReplaceDefaultConfigID(this.defaultConfigID, this.modifiedConfigID1);

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

    [Test, Order(110)]
    public void TestSetDefaultConfig()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                long configId = configMgr.SetDefaultConfig(this.modifiedConfig1);

                Assert.That(configId, Is.EqualTo(this.modifiedConfigID1),
                            "Default config ID is not as expected");

            }
            catch (SzReplaceConflictException)
            {
                // expected exception

            }
            catch (Exception e)
            {
                Fail("Failed TestSetDefaultConfig test with exception", e);
            }
        });

    }

    [Test, Order(120)]
    public void TestSetDefaultConfigWithComment()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzConfigManager configMgr = this.Env.GetConfigManager();

                long configID = configMgr.SetDefaultConfig(this.modifiedConfig3, ModifiedComment3);

                Assert.That(configID, Is.EqualTo(this.modifiedConfigID3),
                            "Default config ID is not as expected");

            }
            catch (SzReplaceConflictException)
            {
                // expected exception

            }
            catch (Exception e)
            {
                Fail("Failed TestSetDefaultConfigWithComment test with exception", e);
            }
        });
    }

    private String ConfigWithout(params string[] dataSources)
    {
        SzConfigManager configMgr = this.Env.GetConfigManager();
        SzConfig config = configMgr.CreateConfig();
        foreach (string dataSource in dataSources)
        {
            config.DeleteDataSource(dataSource);
        }
        return config.Export();
    }

    public static List<object?[]> ConfigCommentParameters()
    {
        List<object?[]> result = new List<object?[]>();

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.defaultConfig,
            "Data Sources: [ ONLY DEFAULT ]"});
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfig1,
            "Data Sources: CUSTOMERS" });
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfig2,
            "Data Sources: CUSTOMERS, EMPLOYEES"});
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.modifiedConfig3,
            "Data Sources: CUSTOMERS, EMPLOYEES, WATCHLIST"});

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.ConfigWithout("TEST"),
            "Data Sources: [ SOME DEFAULT (SEARCH) ]"});

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.ConfigWithout("TEST", "SEARCH"),
            "Data Sources: [ NONE ]"});

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => t.ConfigWithout("SEARCH"),
            "Data Sources: [ SOME DEFAULT (TEST) ]"});

        String missingDataSources = """
                {
                    "G2_CONFIG": {
                        "CFG_ATTR": [ ]
                    }
                }
                """;
        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingDataSources, ""});

        String missingColon = """
                {
                    "G2_CONFIG": {
                        "CFG_DSRC" [ 

                        ]
                    }
                }
                """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingColon, ""});

        String missingColonEnd = """
                {
                    "G2_CONFIG": {
                        "CFG_DSRC"
                """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingColonEnd, ""});

        String missingBracket = """
                {
                    "G2_CONFIG": {
                        "CFG_DSRC"  :  
                            { }
                        ]
                    }
                }
                """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingBracket, ""});

        String missingEndBracket = """
                {
                    "G2_CONFIG": {
                        "CFG_DSRC"  :  [
                            { }
                    }
                }
                """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingEndBracket, ""});

        String missingBracketEnd = """
                {
                    "G2_CONFIG": {
                        "CFG_DSRC"  :
                """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingBracketEnd, ""});

        String missingSubcolon = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE"  "TEST"
                        }
                    ]
                }
            }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingSubcolon, ""});

        String missingSubcolonEnd = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE"
                    ]
                }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingSubcolonEnd, ""});

        String missingQuote = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE" : TEST"
                        }
                    ]
                }
            }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingQuote, ""});

        String missingQuoteEnd = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE" : 
                    ]
                }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingQuoteEnd, ""});

        String missingEndQuote = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE" : "TEST
                        }
                    ]
                }
            }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingEndQuote, ""});

        String missingEndQuoteEnd = """
            {
                "G2_CONFIG": {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE" : "TEST
                    ]
                }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => missingEndQuoteEnd, ""});

        String minimalConfig = """
            {
                "G2_CONFIG" : {
                    "CFG_DSRC" : [ 
                        {
                            "DSRC_CODE" : "TEST"
                        },
                        {
                            "DSRC_CODE" : "SEARCH"
                        },
                        {
                            "DSRC_CODE" : "CUSTOMERS"
                        }
                    ]
                }
            }
            """;

        result.Add(new object?[] {
            (SzCoreConfigManagerTest t) => minimalConfig, "Data Sources: CUSTOMERS"});

        return result;
    }

    [Test, Order(130), TestCaseSource(nameof(ConfigCommentParameters))]
    public void TestCreateConfigComment(
        Func<SzCoreConfigManagerTest, string> configDefFunc,
        string expected)
    {
        String configDefinition = configDefFunc(this);

        this.PerformTest(() =>
        {
            try
            {
                SzCoreConfigManager configMgr
                    = (SzCoreConfigManager)this.Env.GetConfigManager();

                String comment = configMgr.CreateConfigComment(configDefinition);

                Assert.That(comment, Is.EqualTo(expected),
                            "Comment not as expected: " + configDefinition);

            }
            catch (SzReplaceConflictException)
            {
                // expected exception

            }
            catch (Exception e)
            {
                Fail("Failed TestCreateConfigComment test with exception", e);
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

                NativeConfigManager nativeApi = configManager.GetConfigManagerApi();

                nativeApi.ClearLastException();

                long configID = configManager.GetDefaultConfigID();

                string message = nativeApi.GetLastException();
                long errorCode = nativeApi.GetLastExceptionCode();

                Assert.That(message, Is.EqualTo(""),
                            "Unexpected exception message: " + message);

                Assert.That(errorCode, Is.EqualTo(0),
                            "Unexpected error code: " + errorCode);
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
