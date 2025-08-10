namespace Senzing.Sdk.Demo;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzProductDemo : AbstractTest
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

        // @start SzEnvironment
        // get the settings (varies by application)
        string settings = GetSettings();

        // get the instance name (varies by application)
        string instanceName = GetInstanceName();

        // construct the environment
        SzEnvironment env = SzCoreEnvironment.NewBuilder()
                                             .InstanceName(instanceName)
                                             .Settings(settings)
                                             .VerboseLogging(false)
                                             .Build();

        // use the environment for some time (usually as long as the application is running)
        Assert.That(env, Is.Not.Null); // @replace . . .

        // destroy the environment when done (sometimes in a finally block)
        env.Destroy();
        // @end

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
                // @start DestroyEnvironment
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // check if already destroyed
                if (!env.IsDestroyed())
                {
                    // destroy the environment
                    env.Destroy();
                }
                // @end
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
    public void GetProductDemo()
    {
        try
        {
            // @start GetProduct
            // How to obtain an SzProduct instance
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the product from the environment
                SzProduct product = env.GetProduct();

                product.GetLicense(); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception (varies by application)
                LogError("Failed to get SzProduct.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail("Failed", e);
        }
    }

    [Test]
    public void GetLicenseDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetLicense
            // How to obtain the Senzing product license JSON 
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzProduct instance
                SzProduct product = env.GetProduct();

                // obtain the license JSON
                string license = product.GetLicense();
                demoResult = license; // @replace
                // do something with the returned JSON (e.g.: parse it and extract values)
                JsonObject? jsonObj = JsonNode.Parse(license)?.AsObject();

                string? expiration = jsonObj?["expireDate"]?.GetValue<string>();
                int? recordLimit = jsonObj?["recordLimit"]?.GetValue<int>();

                Assert.That(expiration, Is.Not.Null, "Expiration is null"); // @omit
                Assert.That(recordLimit, Is.Not.Null, "Record limit is null"); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get license information.", e);
            }
            // @end
            this.saveDemoResult("GetLicense", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetVersionDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetVersion
            // How to obtain the Senzing product version JSON 
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the SzProduct instance
                SzProduct product = env.GetProduct();

                // obtain the version JSON
                string result = product.GetVersion();
                demoResult = result; // @replace
                // do something with the returned JSON (e.g.: parse it and extract values)
                JsonObject? jsonObj = JsonNode.Parse(result)?.AsObject();

                string? version = jsonObj?["VERSION"]?.GetValue<string>();
                string? buildDate = jsonObj?["BUILD_DATE"]?.GetValue<string>();

                Assert.That(version, Is.Not.Null, "Version is null"); // @omit
                Assert.That(buildDate, Is.Not.Null, "Build date is null"); // @replace . . .

            }
            catch (SzException e)
            {
                LogError("Failed to get version information.", e);
            }
            // @end
            this.saveDemoResult("GetVersion", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }


}
