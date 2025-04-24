namespace Senzing.Sdk.Demo;

using System;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzDiagnosticDemo : AbstractTest
{
    private const string TestDataSource = "TEST";
    private const string TestRecordID = "ABC123";

    private const SzFlag Flags
        = SzEntityIncludeAllFeatures
        | SzEntityIncludeEntityName
        | SzEntityIncludeRecordSummary
        | SzEntityIncludeRecordTypes
        | SzEntityIncludeRecordData
        | SzEntityIncludeRecordJsonData
        | SzEntityIncludeRecordMatchingInfo
        | SzEntityIncludeRecordUnmappedData
        | SzEntityIncludeRecordFeatures
        | SzEntityIncludeInternalFeatures
        | SzIncludeMatchKeyDetails
        | SzIncludeFeatureScores;

    private SzCoreEnvironment? env;

    private long featureID = 0L;

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

        // get the settings (varies by application)
        string settings = GetSettings();

        // get the instance name (varies by application)
        string instanceName = GetInstanceName();

        // construct the environment
        this.env = SzCoreEnvironment.NewBuilder()
                                    .InstanceName(instanceName)
                                    .Settings(settings)
                                    .VerboseLogging(false)
                                    .Build();

        JsonObject jsonObj = new JsonObject();
        jsonObj.Add("DATA_SOURCE", JsonValue.Create(TestDataSource));
        jsonObj.Add("RECORD_ID", JsonValue.Create(TestRecordID));
        jsonObj.Add("NAME_FULL", JsonValue.Create("Joe Schmoe"));
        jsonObj.Add("EMAIL_ADDRESS", JsonValue.Create("joeschmoe@nowhere.com"));
        jsonObj.Add("PHONE_NUMBER", JsonValue.Create("702-555-1212"));
        string recordDefinition = jsonObj.ToJsonString();

        try
        {
            SzEngine engine = this.env.GetEngine();

            engine.AddRecord(TestDataSource, TestRecordID, recordDefinition, SzNoFlags);

            string entityJson = engine.GetEntity(TestDataSource, TestRecordID, Flags);

            // parse the entity and get the feature ID's
            JsonObject? entity = ParseJsonObject(entityJson);
            entity = entity?["RESOLVED_ENTITY"]?.AsObject();
            JsonObject features = ((JsonObject?)entity?["FEATURES"]) ?? new JsonObject();
            IDictionary<string, JsonNode?> dictionary
                = ((IDictionary<string, JsonNode?>)features);
            foreach (string featureName in dictionary.Keys)
            {
                JsonArray? featureArr = (JsonArray?)features[featureName];
                for (int index = 0; index < (featureArr?.Count ?? 0); index++)
                {
                    JsonObject? feature = featureArr?[index]?.AsObject();
                    this.featureID = feature?["LIB_FEAT_ID"]?.GetValue<long>() ?? 0L;
                    if (this.featureID != 0L)
                    {
                        break;
                    }
                }
            }

        }
        catch (SzException e)
        {
            Fail(e);
        }
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

    /// <summary>
    /// Dummy logging function
    /// </summary>
    /// <param name=""message">The message to log</param>
    protected static void Log(string message)
    {
        if (message == null) throw new ArgumentNullException("message");
    }

    [Test, Order(10)]
    public void GetDiagnosticDemo()
    {
        try
        {
            // @start GetDiagnostic
            // How to obtain an SzDiagnostic instance
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                SzDiagnostic diagnostic = env.GetDiagnostic();

                Assert.That(diagnostic, Is.Not.Null); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get SzDiagnostic.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test, Order(20)]
    public void GetDatastoreInfoDemo()
    {
        try
        {
            // @start GetDatastoreInfo
            // How to get datastore info via SzDiagnostic
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the diagnostic instance
                SzDiagnostic diagnostic = env.GetDiagnostic();

                // get the datastore info
                string datastoreJson = diagnostic.GetDatastoreInfo();

                // do something with the returned JSON (varies by application)
                Log(datastoreJson);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get the datastore info.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test, Order(30)]
    public void CheckDatastorePerformanceDemo()
    {
        try
        {
            // @start CheckDatastorePerformance
            // How to get datastore info via SzDiagnostic
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the diagnostic instance
                SzDiagnostic diagnostic = env.GetDiagnostic();

                // check the datastore performance
                string performanceJson = diagnostic.CheckDatastorePerformance(10);

                // do something with the returned JSON (varies by application)
                Log(performanceJson);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to check the datastore performance.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    public long GetFeatureID()
    {
        return this.featureID;
    }

    [Test, Order(40)]
    public void GetFeatureDemo()
    {
        try
        {
            // @start GetFeature
            // How to get a feature by its feature ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the diagnostic instance
                SzDiagnostic diagnostic = env.GetDiagnostic();

                // get a valid feature (varies by application)
                long featureID = GetFeatureID();

                // get the feature for the feature ID
                string featureJson = diagnostic.GetFeature(featureID);

                // do something with the returned JSON
                Log(featureJson);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to purge the repository.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    public static bool ConfirmPurge()
    {
        return true;
    }

    [Test, Order(50)]
    public void PurgeRepositoryDemo()
    {
        try
        {
            // @start PurgeRepository
            // How to purge the Senzing repository
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the diagnostic instance
                SzDiagnostic diagnostic = env.GetDiagnostic();

                // purge the repository (MAKE SURE YOU WANT TO DO THIS)
                if (ConfirmPurge())
                {
                    diagnostic.PurgeRepository();
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to purge the repository.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

}
