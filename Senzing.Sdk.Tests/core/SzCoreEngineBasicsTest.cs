namespace Senzing.Sdk.Tests.Core;

using NUnit.Framework;
using System;
using Senzing.Sdk;
using Senzing.Sdk.Tests;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreEngineBasicsTest : AbstractTest {

    private SzCoreEnvironment? env = null;

    private SzCoreEnvironment Env {
        get {
            if (this.env != null) {
                return this.env;
            } else {
                throw new InvalidOperationException(
                    "The SzEnvironment is null");
            }
        }
    }

    [OneTimeSetUp]
    public void InitializeEnvironment() {
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
    public void TeardownEnvironment() {
        try {
            if (this.env != null) {
                this.env.Destroy();
                this.env = null;
            }
            this.TeardownTestEnvironment();
        } finally {
            this.EndTests();
        }
    }

    [Test]
    public void TestGetNativeApi() {
        this.PerformTest(() => {
            try {
                SzCoreEngine engine = (SzCoreEngine) this.Env.GetEngine();

                Assert.IsNotNull(engine.GetNativeApi(),
                      "Underlying native API is unexpectedly null");

            } catch (Exception e) {
                Fail("Failed testGetNativeApi test with exception", e);
            }
        });
    }

    private static List<(ISet<string>?, string)> GetEncodeDataSourcesArguments() {
        List<(ISet<string>?,string)> result = new List<(ISet<string>?,string)>();

        result.Add((null, "{\"DATA_SOURCES\":[]}"));

        result.Add((SortedSetOf<string>(), "{\"DATA_SOURCES\":[]}"));

        result.Add((SortedSetOf("CUSTOMERS"), "{\"DATA_SOURCES\":[\"CUSTOMERS\"]}"));

        result.Add(
            (SortedSetOf("CUSTOMERS","EMPLOYEES"),
             "{\"DATA_SOURCES\":[\"CUSTOMERS\",\"EMPLOYEES\"]}"));

        result.Add(
            (SortedSetOf("CUSTOMERS","EMPLOYEES","WATCHLIST"),
             "{\"DATA_SOURCES\":[\"CUSTOMERS\",\"EMPLOYEES\",\"WATCHLIST\"]}"));
    
        return result;
    }

    [Test,TestCaseSource(nameof(GetEncodeDataSourcesArguments))]
    public void TestEncodeDataSources(
        (ISet<string>? sources, string expectedJson) args)
    {
        ISet<string>?   sources         = args.sources;
        string          expectedJson    = args.expectedJson;
        this.PerformTest(() => {
            string actualJson = SzCoreEngine.EncodeDataSources(sources);
                
            Assert.That(actualJson, Is.EqualTo(expectedJson),
                        "Data sources not properly encoded");
        });
    }

    private static List<(ISet<long>?,string)> GetEncodeEntityIDsArguments() {
        List<(ISet<long>?,string)> result
            = new List<(ISet<long>?,string)>();

        result.Add((null, "{\"ENTITIES\":[]}"));

        result.Add((SortedSetOf<long>(), "{\"ENTITIES\":[]}"));

        result.Add((SortedSetOf(10L), "{\"ENTITIES\":[{\"ENTITY_ID\":10}]}"));

        result.Add(
            (SortedSetOf(10L, 20L),
             "{\"ENTITIES\":[{\"ENTITY_ID\":10},{\"ENTITY_ID\":20}]}"));

        result.Add(
            (SortedSetOf(10L, 20L, 15L),
            "{\"ENTITIES\":[{\"ENTITY_ID\":10},{\"ENTITY_ID\":15},{\"ENTITY_ID\":20}]}"));
    
        return result;
    }

    [Test, TestCaseSource(nameof(GetEncodeEntityIDsArguments))]
    public void TestEncodeEntityIds(
        (ISet<long>? entityIDs, string expectedJson) args)
    {
        ISet<long>?     entityIDs       = args.entityIDs;
        string          expectedJson    = args.expectedJson;

        this.PerformTest(() => {
            string actualJson = SzCoreEngine.EncodeEntityIDs(entityIDs);
                
            Assert.That(actualJson, Is.EqualTo(expectedJson),
                        "Entity ID's not properly encoded");
        });
    }

    private static List<(ISet<(string,string)>?, string)> 
        GetEncodeRecordKeysArguments() 
    {
        List<(ISet<(string,string)>?, string)> result 
            = new List<(ISet<(string,string)>?, string)>();

        result.Add((null, "{\"RECORDS\":[]}"));

        result.Add((SortedSetOf<(string,string)>(), "{\"RECORDS\":[]}"));

        result.Add(
            (SortedSetOf(("CUSTOMERS","ABC123")),
             "{\"RECORDS\":[{\"DATA_SOURCE\":\"CUSTOMERS\",\"RECORD_ID\":\"ABC123\"}]}"));

        result.Add(
            (SortedSetOf(("CUSTOMERS","ABC123"), ("EMPLOYEES","DEF456")),
             "{\"RECORDS\":[{\"DATA_SOURCE\":\"CUSTOMERS\",\"RECORD_ID\":\"ABC123\"},"
             + "{\"DATA_SOURCE\":\"EMPLOYEES\",\"RECORD_ID\":\"DEF456\"}]}"));

        result.Add(
            (SortedSetOf(("CUSTOMERS","ABC123"),
                         ("EMPLOYEES","DEF456"),
                         ("WATCHLIST","GHI789")),
            "{\"RECORDS\":[{\"DATA_SOURCE\":\"CUSTOMERS\",\"RECORD_ID\":\"ABC123\"},"
            + "{\"DATA_SOURCE\":\"EMPLOYEES\",\"RECORD_ID\":\"DEF456\"},"
            + "{\"DATA_SOURCE\":\"WATCHLIST\",\"RECORD_ID\":\"GHI789\"}]}"));
        
        return result;
    }

    [Test, TestCaseSource(nameof(GetEncodeRecordKeysArguments))]
    public void TestEncodeRecordKeys(
        (ISet<(string,string)>? recordKeys, string expectedJson) args)
    {
        ISet<(string,string)>?  recordKeys      = args.recordKeys;
        string                  expectedJson    = args.expectedJson;

        this.PerformTest(() => {
            string actualJson = SzCoreEngine.EncodeRecordKeys(recordKeys);
                
            Assert.That(actualJson, Is.EqualTo(expectedJson),
                        "Record Keys not properly encoded");
        });
    }


    [Test]
    public void TestPrimeEngine() {
        this.PerformTest(() => {
            try {
                SzEngine engine = this.Env.GetEngine();

                engine.PrimeEngine();

            } catch (Exception e) {
                Fail("Priming engine failed with an exception", e);
            }
        });
    }
}
