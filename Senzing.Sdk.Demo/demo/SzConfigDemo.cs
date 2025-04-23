namespace Senzing.Sdk.Demo;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzConfigDemo : AbstractTest
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
    public void CreateConfigFromTemplateDemo()
    {
        try
        {
            // @start CreateConfigFromTemplate
            // How to create an SzConfig instance representing the template configuration
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // create the config from the template
                SzConfig config = configMgr.CreateConfig();

                // do something with the SzConfig
                Assert.That(config, Is.Not.Null, "Config is null"); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to create new SzConfig from the template.", e);
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
    /// <returns>The config definition JSON.</returns>
    public string ReadConfigFile()
    {
        SzEnvironment env = GetEnvironment();
        SzConfigManager configMgr = env.GetConfigManager();
        SzConfig config = configMgr.CreateConfig();
        return config.Export();
    }

    [Test]
    public void CreateConfigFromDefinitionDemo()
    {
        try
        {
            // @start CreateConfigFromDefinition
            // How to create an SzConfig instance representing a specified config definition
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // obtain a JSON config definition (varies by application)
                string configDefinition = ReadConfigFile();

                // create the config using the config definition
                SzConfig config = configMgr.CreateConfig(configDefinition);

                // do something with the SzConfig
                Assert.That(config, Is.Not.Null, "Config is null"); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to create a new SzConfig from a definition.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void ExportConfigDemo()
    {
        try
        {
            // @start ExportConfig
            // How to export config JSON from a config handle
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // get an SzConfig object (varies by application)
                SzConfig config = configMgr.CreateConfig();

                // export the config
                String configDefinition = config.Export();

                Assert.That(configDefinition, Is.Not.Null, "Config definition is null"); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to export configuration.", e);
            }
            // @end
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void getDataSourcesDemo()
    {
        try
        {
            // @start GetDataSources
            // How to get the data sources from an in-memory config
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // get an SzConfig object (varies by application)
                SzConfig config = configMgr.CreateConfig();

                // get the data sources
                String sourcesJson = config.GetDataSources();

                // do something with the returned JSON (e.g.: parse it and extract values)
                JsonObject? jsonObj = JsonNode.Parse(sourcesJson)?.AsObject();

                JsonArray? jsonArr = jsonObj?["DATA_SOURCES"]?.AsArray();

                Assert.That(jsonArr, Is.Not.Null, "DATA_SOURCES array was null"); // @omit
                // iterate over the data sources
                if (jsonArr != null)
                {
                    for (int index = 0; index < jsonArr.Count; index++)
                    {

                        JsonObject? sourceObj = jsonArr[index]?.AsObject();

                        string? dataSourceCode = sourceObj?["DSRC_CODE"]?.GetValue<string>();

                        Assert.That(dataSourceCode, Is.Not.Null, "DSRC_CODE is null"); // @replace . . .
                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get data sources.", e);
            }
            // @end
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void AddDataSourceDemo()
    {
        try
        {
            // @start AddDataSource
            // How to add data sources to an in-memory config
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // get an SzConfig object (varies by application)
                SzConfig config = configMgr.CreateConfig();

                // add data sources to the config
                config.AddDataSource("CUSTOMERS");
                config.AddDataSource("EMPLOYEES");
                config.AddDataSource("WATCHLIST");

                Assert.That(config, Is.Not.Null); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to add data sources.", e);
            }
            // @end region = "addDataSource"

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void DeleteDataSourceDemo()
    {
        try
        {
            // @start DeleteDataSource
            // How to delete a data source from an in-memory config
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzConfigManager instance
                SzConfigManager configMgr = env.GetConfigManager();

                // get an SzConfig object (varies by application)
                SzConfig config = configMgr.CreateConfig();

                // delete the data source from the config
                config.DeleteDataSource("CUSTOMERS");

                Assert.That(config, Is.Not.Null); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to delete data source.", e);
            }
            // @end region = "deleteDataSource"

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

}