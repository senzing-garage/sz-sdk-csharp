namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;

using static System.StringComparison;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreDiagnosticTest : AbstractTest
{
    private const string TESTData_SOURCE = "TEST";
    private const string TEST_RECORD_ID = "ABC123";

    private const long FLAGS
        = NativeFlags.SzEntityIncludeAllFeatures
        | NativeFlags.SzEntityIncludeEntityName
        | NativeFlags.SzEntityIncludeRecordSummary
        | NativeFlags.SzEntityIncludeRecordTypes
        | NativeFlags.SzEntityIncludeRecordData
        | NativeFlags.SzEntityIncludeRecordJsonData
        | NativeFlags.SzEntityIncludeRecordMatchingInfo
        | NativeFlags.SzEntityIncludeRecordUnmappedData
        | NativeFlags.SzEntityIncludeRecordFeatures
        | NativeFlags.SzEntityIncludeInternalFeatures
        | NativeFlags.SzIncludeMatchKeyDetails
        | NativeFlags.SzIncludeFeatureScores;

    private SzCoreEnvironment? env;

    private static readonly IList<string> FeatureDescriptions;

    private static string AddAndGetTestEntity(NativeEngine nativeEngine)
    {
        JsonObject jsonObj = new JsonObject();
        jsonObj.Add("DATA_SOURCE", JsonValue.Create(TESTData_SOURCE));
        jsonObj.Add("RECORD_ID", JsonValue.Create(TEST_RECORD_ID));
        jsonObj.Add("NAME_FULL", JsonValue.Create("Joe Schmoe"));
        jsonObj.Add("EMAIL_ADDRESS", JsonValue.Create("joeschmoe@nowhere.com"));
        jsonObj.Add("PHONE_NUMBER", JsonValue.Create("702-555-1212"));
        string recordDefinition = jsonObj.ToJsonString();

        // add a record
        long returnCode = nativeEngine.AddRecord(
            TESTData_SOURCE, TEST_RECORD_ID, recordDefinition);

        if (returnCode != 0)
        {
            throw new TestException(nativeEngine.GetLastException());
        }

        // get the entity 
        returnCode = nativeEngine.GetEntityByRecordID(
            TESTData_SOURCE, TEST_RECORD_ID, FLAGS, out string result);

        // return the result
        return result;
    }

    private class DiagnosticHelper : AbstractTest
    {
        private IList<string> featureDescriptions;
        private String settings;
        public DiagnosticHelper()
        {
            this.featureDescriptions = new List<string>();
            this.settings = "";
        }
        public void Initialize()
        {
            this.InitializeTestEnvironment();
            string settings = this.GetRepoSettings();
            this.settings = settings;
            string instanceName = this.GetType().Name;
            NativeEngine nativeEngine = new NativeEngineExtern();
            try
            {
                // initialize the native engine
                long returnCode = nativeEngine.Init(instanceName, settings, false);
                if (returnCode != 0)
                {
                    throw new TestException(nativeEngine.GetLastException());
                }

                string result = AddAndGetTestEntity(nativeEngine);

                // parse the entity and get the feature ID's
                JsonObject? entity = ParseJsonObject(result);
                entity = (JsonObject?)entity?["RESOLVED_ENTITY"];
                JsonObject features = ((JsonObject?)entity?["FEATURES"]) ?? new JsonObject();
                IDictionary<string, JsonNode?> dictionary
                    = ((IDictionary<string, JsonNode?>)features);
                foreach (string featureName in dictionary.Keys)
                {
                    JsonArray? featureArr = (JsonArray?)features[featureName];
                    for (int index = 0; index < (featureArr?.Count ?? 0); index++)
                    {
                        JsonObject? feature = (JsonObject?)featureArr?[index];
                        string featureDesc = ((string?)feature?["FEAT_DESC"]) ?? "";
                        this.featureDescriptions.Add(featureDesc);
                    }
                }

            }
            finally
            {
                nativeEngine.Destroy();
            }

            this.featureDescriptions = this.featureDescriptions.ToImmutableList();
        }
        public string GetSettings()
        {
            return this.settings;
        }
        public IList<string> GetFeatureDescriptions()
        {
            if (this.featureDescriptions is ImmutableList<string>)
            {
                return this.featureDescriptions;
            }
            else
            {
                return this.featureDescriptions.ToImmutableList();
            }
        }

        public void TearDown()
        {
            this.TeardownTestEnvironment(true);
        }
    }

    private static readonly string HelperSettings;

    static SzCoreDiagnosticTest()
    {
        DiagnosticHelper helper = new DiagnosticHelper();
        helper.Initialize();
        HelperSettings = helper.GetSettings();
        FeatureDescriptions = helper.GetFeatureDescriptions();
        helper.TearDown();
    }

    private readonly Dictionary<long, string> featureMaps
        = new Dictionary<long, string>();

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

        NativeEngine nativeEngine = new NativeEngineExtern();
        try
        {
            // initialize the native engine
            long returnCode = nativeEngine.Init(instanceName, settings, false);
            if (returnCode != 0)
            {
                throw new TestException(nativeEngine.GetLastException());
            }

            string result = AddAndGetTestEntity(nativeEngine);

            // parse the entity and get the feature ID's
            JsonObject? entity = ParseJsonObject(result);
            entity = (JsonObject?)entity?["RESOLVED_ENTITY"];
            JsonObject features = ((JsonObject?)entity?["FEATURES"]) ?? new JsonObject();
            IDictionary<string, JsonNode?> dictionary
                = ((IDictionary<string, JsonNode?>)features);
            foreach (string featureName in dictionary.Keys)
            {
                JsonArray? featureArr = (JsonArray?)features[featureName];
                for (int index = 0; index < (featureArr?.Count ?? 0); index++)
                {
                    JsonObject? feature = (JsonObject?)featureArr?[index];
                    long featureID = ((long?)feature?["LIB_FEAT_ID"]) ?? 0L;
                    string jsonText = feature?.ToJsonString() ?? "{}";
                    this.featureMaps.Add(featureID, jsonText);
                }
            }

        }
        finally
        {
            nativeEngine.Destroy();
        }

        this.env = SzCoreEnvironment.NewBuilder()
                                    .InstanceName(instanceName)
                                    .Settings(settings)
                                    .VerboseLogging(false)
                                    .Build();

        NativeEngine engine = ((SzCoreEngine)this.Env.GetEngine()).GetNativeApi();
        engine.GetEntityByRecordID(TESTData_SOURCE,
                                   TEST_RECORD_ID,
                                   FLAGS,
                                   out string entityJSON);
        string dataStoreInfo = this.Env.GetDiagnostic().GetDatastoreInfo();
    }

    [OneTimeTearDown]
    public void TeardownEnvironment()
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
                SzCoreDiagnostic diagnostic = (SzCoreDiagnostic)
                    this.Env.GetDiagnostic();

                Assert.IsNotNull(diagnostic.GetNativeApi(),
                      "Underlying native API is unexpectedly null");

            }
            catch (Exception e)
            {
                Fail("Failed TestGetNativeApi test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(10)]
    public void TestGetDatastoreInfo()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzDiagnostic diagnostic = this.Env.GetDiagnostic();

                string result = diagnostic.GetDatastoreInfo();

                // parse the result as JSON and check that it parses
                ParseJsonObject(result);

            }
            catch (Exception e)
            {
                Fail("Failed TestGetDatastoreInfo test with exception", e);
                throw;
            }
        });
    }

    [Test, Order(20)]
    public void TestCheckDatastorePerformance()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzDiagnostic diagnostic = this.Env.GetDiagnostic();

                string result = diagnostic.CheckDatastorePerformance(5);

                // parse the result as JSON and check that it parses
                ParseJsonObject(result);

            }
            catch (Exception e)
            {
                Fail("Failed TestCheckDatastorePerformance test with exception", e);
                throw;
            }
        });
    }

    protected (long, string) GetArgsByFeatureDesc(string featureDesc)
    {
        foreach (KeyValuePair<long, string> pair in this.featureMaps)
        {
            if (pair.Value.Contains('"' + featureDesc + '"', Ordinal))
            {
                return (pair.Key, pair.Value);
            }
        }
        throw new ArgumentException(
            "Feature description not found: " + featureDesc);
    }

    [Test, Order(30), TestCaseSource(nameof(FeatureDescriptions))]
    public void TestGetFeature(string featureDesc)
    {
        (long featureID, string expected) args = GetArgsByFeatureDesc(featureDesc);
        long featureID = args.featureID;
        string expected = args.expected;
        this.PerformTest(() =>
        {
            try
            {
                SzDiagnostic diagnostic = this.Env.GetDiagnostic();

                string actual = diagnostic.GetFeature(featureID);

                JsonObject actualObj = ParseJsonObject(actual);
                JsonObject expectedObj = ParseJsonObject(expected);

                Assert.That((long?)actualObj["LIB_FEAT_ID"],
                            Is.EqualTo((long?)expectedObj["LIB_FEAT_ID"]),
                            "Feature ID does not match what is expected");

            }
            catch (Exception e)
            {
                String dataStoreInfo = this.Env.GetDiagnostic().GetDatastoreInfo();
                NativeEngine engine = ((SzCoreEngine)this.Env.GetEngine()).GetNativeApi();
                long returnCode = engine.GetEntityByRecordID(TESTData_SOURCE,
                                                             TEST_RECORD_ID,
                                                             FLAGS,
                                                             out string entityJSON);
                string message = (returnCode != 0) ? engine.GetLastException() : "";

                Fail("Failed TestGetFeature test with exception: featureID=[ "
                     + featureID + " ], featureDesc=[ " + featureDesc
                     + " ], expected=[ " + expected + " ], entityJSON=[ "
                     + entityJSON + " ], message=[ " + message
                     + " ], dataStoreInfo=[ " + dataStoreInfo + " ]", e);
                throw;
            }
        });
    }

    [Test, Order(40)]
    public void TestGetBadFeature()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzDiagnostic diagnostic = this.Env.GetDiagnostic();

                diagnostic.GetFeature(100000000L);

                Assert.Fail("GetFeature() unexpected succeeded with bad "
                            + "feature ID");

            }
            catch (SzException)
            {
                /// expected
            }
        });
    }

    [Test, Order(100)]
    public void TestPurgeRepository()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzDiagnostic diagnostic = this.Env.GetDiagnostic();

                diagnostic.PurgeRepository();

            }
            catch (Exception e)
            {
                Fail("Failed TestPurgeRepository test with exception", e);
                throw;
            }
        });

    }
}
