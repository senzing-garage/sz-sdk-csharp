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


    protected override string GetInstanceName() {
        return this.GetType().Name;
    }

    private string GetSettings() {
        return this.GetRepoSettings();
    }


    [OneTimeSetUp]
    public void InitializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment();


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
        // . . .

        // destroy the environment when done (sometimes in a finally block)
        env.Destroy();

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

    protected SzEnvironment GetEnvironment() {
        if (this.env != null) {
            return this.env;
        } else {
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
    public void GetProductDemo() {
        try {
            // How to obtain an SzProduct instance
            try {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                SzProduct product = env.GetProduct();

                // . . .

            } catch (SzException e) {
                // handle or rethrow the exception (varies by application)
                LogError("Failed to get SzProduct.", e);
            }

        } catch (Exception e) {
            Fail("Failed", e);
        }
    }

}
