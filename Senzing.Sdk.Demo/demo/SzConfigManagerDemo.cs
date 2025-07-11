namespace Senzing.Sdk.Demo;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzConfigManagerDemo : AbstractTest
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


    protected override string GetInstanceName()
    {
        return this.GetType().Name;
    }

    private string GetSettings()
    {
        return this.GetRepoSettings();
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

    protected SzEnvironment GetEnvironment()
    {
        if (this.env != null)
        {
            return this.env;
        }
        else
        {
            throw new InvalidOperationException("Environment is null");
        }
    }

    protected static void LogError(string message, Exception e)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("**********************************");
        Console.Error.WriteLine("FAILURE: " + message);
        Console.Error.WriteLine(e);
        Console.Error.WriteLine();
        throw e;
    }

    [Test]
    public void GetConfigManagerDemo()
    {
        try
        {
            // @start GetConfigManager
            // How to obtain an SzConfigManager instance
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                Assert.That(configMgr, Is.Not.Null); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get SzConfigManager.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    /// <summary>
    /// Simulates reading a config file from disk.
    /// 
    /// <param name="dataSources">
    /// The zero or more data sources to add to the config.
    /// </param>
    ///
    /// <returns>The config definition JSON.</returns>
    public string CreateConfigWithDataSources(params string[] dataSources)
    {
        SzEnvironment env = GetEnvironment();
        SzConfigManager configMgr = env.GetConfigManager();

        SzConfig config = configMgr.CreateConfig();

        if (dataSources != null)
        {
            foreach (string dataSource in dataSources)
            {
                config.RegisterDataSource(dataSource);
            }
        }

        return config.Export();
    }

    /**
     * Simulates reading a config file from disk.
     * 
     * @param dataSources The zero or more data sources to add to the config.
     * 
     * @return The config definition JSON.
     */
    public string AddDataSourcesToConfig(long configID, params string[] dataSources)
    {
        SzEnvironment env = GetEnvironment();

        SzConfigManager configMgr = env.GetConfigManager();

        SzConfig config = configMgr.CreateConfig(configID);

        if (dataSources != null)
        {
            foreach (string dataSource in dataSources)
            {
                config.RegisterDataSource(dataSource);
            }
        }
        return config.Export();
    }

    [Test]
    public void RegisterConfigDemo()
    {
        try
        {
            // @start RegisterConfig
            // How to register a configuration with the Senzing repository
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // obtain a JSON config definition (will vary by application)
                string configDefinition = CreateConfigWithDataSources("EMPLOYEES");

                // register the config (using an auto-generated comment)
                long configID = configMgr.RegisterConfig(configDefinition);

                // do something with the config ID
                Assert.That(configID > 0); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to register configuration.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void RegisterConfigWithCommentDemo()
    {
        try
        {
            // @start RegisterConfigWithComment
            // How to register a configuration with the Senzing repository
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // obtain a JSON config definition (will vary by application)
                string configDefinition = CreateConfigWithDataSources("CUSTOMERS");

                // register the config with a custom comment
                long configID = configMgr.RegisterConfig(configDefinition, "Added CUSTOMERS data source");

                // do something with the config ID
                Assert.That(configID > 0); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to register configuration.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }


    [Test]
    public void CreateConfigFromConfigIDDemo()
    {
        try
        {
            // @start CreateConfigFromConfigID
            // How to get a config definition by its configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get a valid configuration ID (will vary by application)
                long configID = configMgr.GetDefaultConfigID();

                // get the config definition for the config ID
                SzConfig config = configMgr.CreateConfig(configID);

                // do something with the SzConfig
                Assert.That(configID, Is.Not.EqualTo(0L)); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to Create SzConfig from config ID.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetConfigRegistryDemo()
    {
        try
        {
            // @start GetConfigRegistry
            // How to get a JSON document describing all registered configs
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get the config definition for the config ID
                string configRegistry = configMgr.GetConfigRegistry();

                // do something with the returned JSON (e.g.: parse it and extract values)
                JsonObject? jsonObj = JsonNode.Parse(configRegistry)?.AsObject();

                JsonArray? jsonArr = jsonObj?["CONFIGS"]?.AsArray();

                // iterate over the registered configurations
                if (jsonArr != null)
                {
                    for (int index = 0; index < jsonArr.Count; index++)
                    {
                        JsonObject? configObj = jsonArr[index]?.AsObject();

                        long? configID = configObj?["CONFIG_ID"]?.GetValue<long>();
                        string? createdOn = configObj?["SYS_CREATE_DT"]?.GetValue<string>();
                        string? comment = configObj?["CONFIG_COMMENTS"]?.GetValue<string>();

                        Assert.That(configID, Is.Not.Null, "CONFIG_ID is null"); // @omit
                        Assert.That(createdOn, Is.Not.Null, "SYS_CREATE_DT is null"); // @omit
                        Assert.That(comment, Is.Not.Null, "CONFIG_COMMENTS is null"); // @replace . . .

                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get configurations.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetDefaultConfigIDDemo()
    {
        try
        {
            // @start GetDefaultConfigID
            // How to get the registered default configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get the default configuration ID
                long configID = configMgr.GetDefaultConfigID();

                // check if no default configuration ID is registered
                if (configID == 0)
                {
                    // handle having no registered configuration ID
                    throw new Exception(); // @replace . . .

                }
                else
                {
                    // do something with the configuration ID
                    Assert.That(configID != 0);  // @replace . . .

                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get the default configuration ID.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void SetDefaultConfigIDDemo()
    {
        try
        {
            // @start SetDefaultConfigID
            // How to set the registered default configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get the configuration ID (varies by application)
                string configDefinition = CreateConfigWithDataSources("WATCHLIST");
                long configID = configMgr.RegisterConfig(configDefinition);

                // set the default config ID
                configMgr.SetDefaultConfigID(configID);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to set default configuration ID.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void SetDefaultConfigDemo()
    {
        try
        {
            // @start SetDefaultConfig
            // How to set the registered default configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get the configuration ID (varies by application)
                string configDefinition = CreateConfigWithDataSources("VIPS");

                // set the default config (using an auto-generated comment)
                long configID = configMgr.SetDefaultConfig(configDefinition);

                // do something with the registered config ID
                Assert.That(configID != 0); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to set default configuration.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void SetDefaultConfigWithCommentDemo()
    {
        try
        {
            // @start SetDefaultConfigWithComment
            // How to set the registered default configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                // get the configuration ID (varies by application)
                string configDefinition = CreateConfigWithDataSources("COMPANIES");

                // set the default config (using a specific comment)
                long configID = configMgr.SetDefaultConfig(configDefinition, "Initial config with COMPANIES");

                // do something with the registered config ID
                Assert.That(configID != 0); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to set default configuration.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void ReplaceDefaultConfigIDDemo()
    {
        try
        {
            // @start ReplaceDefaultConfigID
            // How to replace the registered default configuration ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the config manager
                SzConfigManager configMgr = env.GetConfigManager();

                do
                {
                    // get the current default configuration ID
                    long oldConfigID = configMgr.GetDefaultConfigID();

                    // Create a new config (usually modifying the current -- varies by application)
                    string configDefinition = AddDataSourcesToConfig(oldConfigID, "PASSENGERS");
                    long newConfigID = configMgr.RegisterConfig(configDefinition, "Added PASSENGERS data source");

                    try
                    {
                        // replace the default config ID with the new config ID
                        configMgr.ReplaceDefaultConfigID(oldConfigID, newConfigID);

                        // if we get here then break out of the loop
                        break;

                    }
                    catch (SzReplaceConflictException)
                    {
                        // race condition detected
                        // do nothing so we loop through and try again
                    }

                } while (true);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to replace default configuration ID.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetActiveConfigIDDemo()
    {
        try
        {
            // @start GetActiveConfigID
            // How to get the active config ID for the SzEnvironment
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the active config ID
                long activeConfigID = env.GetActiveConfigID();

                // do something with the active config ID (varies by application)
                SzConfigManager configMgr = env.GetConfigManager();

                long defaultConfigID = configMgr.GetDefaultConfigID();

                if (activeConfigID != defaultConfigID)
                {
                    // reinitialize the environment with the default config ID                    
                    env.Reinitialize(defaultConfigID);
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to verify the active config ID.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }
}