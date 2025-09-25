namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

using NUnit.Framework;
using NUnit.Framework.Internal;

using Senzing.Sdk;
using Senzing.Sdk.Core;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;
using static Senzing.Sdk.SzFlagUsageGroup;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreEngineHowTest : AbstractTest
{
    private const string UNKNOWN_DATA_SOURCE = "UNKNOWN";
    private const string Passengers = "PASSENGERS";
    private const string Employees = "EMPLOYEES";
    private const string Vips = "VIPS";
    private const string DualIdentities = "DUAL_IDENTITIES";

    private static readonly (string dataSourceCode, string recordID) ABC123
        = (Passengers, "ABC123");
    private static readonly (string dataSourceCode, string recordID) DEF456
        = (Passengers, "DEF456");
    private static readonly (string dataSourceCode, string recordID) GHI789
        = (Passengers, "GHI789");
    private static readonly (string dataSourceCode, string recordID) JKL012
        = (Passengers, "JKL012");
    private static readonly (string dataSourceCode, string recordID) MNO345
        = (Employees, "MNO345");
    private static readonly (string dataSourceCode, string recordID) PQR678
        = (Employees, "PQR678");
    private static readonly (string dataSourceCode, string recordID) STU901
        = (Vips, "STU901");
    private static readonly (string dataSourceCode, string recordID) XYZ234
        = (Vips, "XYZ234");
    private static readonly (string dataSourceCode, string recordID) STU234
        = (Vips, "STU234");
    private static readonly (string dataSourceCode, string recordID) XYZ456
        = (Vips, "XYZ456");
    private static readonly (string dataSourceCode, string recordID) ZYX321
        = (Employees, "ZYX321");
    private static readonly (string dataSourceCode, string recordID) CBA654
        = (Employees, "CBA654");

    private static readonly (string dataSourceCode, string recordID) BCD123
        = (DualIdentities, "BCD123");
    private static readonly (string dataSourceCode, string recordID) CDE456
        = (DualIdentities, "CDE456");
    private static readonly (string dataSourceCode, string recordID) EFG789
        = (DualIdentities, "EFG789");
    private static readonly (string dataSourceCode, string recordID) FGH012
        = (DualIdentities, "FGH012");

    private const SzFlag FeatureFlags
        = SzEntityIncludeAllFeatures
        | SzEntityIncludeRepresentativeFeatures
        | SzEntityIncludeInternalFeatures;

    private const SzFlag RecordFlags
        = SzEntityIncludeRecordData
        | SzEntityIncludeRecordFeatures
        | SzEntityIncludeRecordFeatureDetails
        | SzEntityIncludeRecordFeatureStats;

    private static readonly IList<SzFlag?> VirtualEntityFlagSets;

    private static readonly IList<SzFlag?> HowFlagSets;

    static SzCoreEngineHowTest()
    {
        List<SzFlag?> list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzVirtualEntityDefaultFlags);
        list.Add(SzVirtualEntityAllFlags);
        list.Add(SzEntityIncludeRecordFeatures
            | SzEntityIncludeAllFeatures
            | SzEntityIncludeRecordData
            | SzEntityIncludeInternalFeatures
            | SzIncludeMatchKeyDetails
            | SzEntityIncludeRecordMatchingInfo);

        list.Add(SzEntityIncludeRecordFeatures
            | SzEntityIncludeRepresentativeFeatures
            | SzEntityIncludeInternalFeatures
            | SzIncludeMatchKeyDetails
            | SzEntityIncludeRecordMatchingInfo);
        VirtualEntityFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzIncludeMatchKeyDetails);
        list.Add(SzIncludeFeatureScores);
        list.Add(SzHowEntityDefaultFlags);
        list.Add(SzHowAllFlags);
        list.Add(SzIncludeMatchKeyDetails | SzIncludeFeatureScores);
        HowFlagSets = list.AsReadOnly();
    }

    private readonly Dictionary<(string, string), long> LoadedRecordMap
        = new Dictionary<(string, string), long>();

    private readonly Dictionary<long, ISet<(string, string)>> LoadedEntityMap
        = new Dictionary<long, ISet<(string, string)>>();

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

    public long GetEntityID(string dataSourceCode, string recordID)
    {
        return GetEntityID((dataSourceCode, recordID));
    }

    public long GetEntityID((string dataSourceCode, string recordID) key)
    {
        if (LoadedRecordMap.ContainsKey(key))
        {
            return LoadedRecordMap[key];
        }
        else
        {
            throw new ArgumentException("Record ID not found: " + key);
        }
    }

    private static readonly IList<(string, string)> RecordKeys
        = ListOf(ABC123,
                 DEF456,
                 GHI789,
                 JKL012,
                 MNO345,
                 PQR678,
                 STU901,
                 XYZ234,
                 STU234,
                 XYZ456,
                 ZYX321,
                 CBA654,
                 BCD123,
                 CDE456,
                 EFG789,
                 FGH012).AsReadOnly();

    [OneTimeSetUp]
    public void InitializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment();

        string settings = this.GetRepoSettings();

        string instanceName = this.GetType().Name;

        // now we just need the entity ID's for the loaded records to use later
        NativeEngine nativeEngine = new NativeEngineExtern();
        try
        {
            long returnCode = nativeEngine.Init(instanceName, settings, false);
            if (returnCode != 0)
            {
                throw new TestException(nativeEngine.GetLastException());
            }

            // get the loaded records and entity ID's
            foreach ((string dataSourceCode, string recordID) key in RecordKeys)
            {
                returnCode = nativeEngine.GetEntityByRecordID(
                    key.dataSourceCode, key.recordID, out string result);
                if (returnCode != 0)
                {
                    throw new TestException(nativeEngine.GetLastException());
                }
                // parse the JSON 
                JsonObject? jsonObj = JsonNode.Parse(result)?.AsObject();
                JsonObject? entity = jsonObj?["RESOLVED_ENTITY"]?.AsObject();
                long entityID = entity?["ENTITY_ID"]?.GetValue<long>() ?? 0L;
                LoadedRecordMap.Add(key, entityID);
                if (!LoadedEntityMap.ContainsKey(entityID))
                {
                    LoadedEntityMap.Add(entityID, new HashSet<(string, string)>());
                }
                ISet<(string dataSourceCode, string recordID)> recordKeySet
                    = LoadedEntityMap[entityID];
                recordKeySet.Add(key);
            }
            ;

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
    }

    /**
     * Overridden to configure some data sources.
     */
    override protected void PrepareRepository()
    {
        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();

        ISet<string> dataSources = SortedSetOf(Passengers,
                                               Employees,
                                               Vips,
                                               DualIdentities);

        FileInfo passengerFile = this.PreparePassengerFile();
        FileInfo employeeFile = this.PrepareEmployeeFile();
        FileInfo vipFile = this.PrepareVipFile();
        FileInfo dualIdentitiesFile = this.PrepareDualIdentitiesFile();

        RepositoryManager.ConfigSources(repoDirectory,
                                        dataSources,
                                        true);

        RepositoryManager.LoadFile(repoDirectory,
                                   passengerFile,
                                   Passengers,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   employeeFile,
                                   Employees,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   vipFile,
                                   Vips,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   dualIdentitiesFile,
                                   DualIdentities,
                                   true);
    }

    private FileInfo PreparePassengerFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH"};

        string[][] passengers = {
            new string[] {ABC123.recordID, "Joe", "Schmoe", "702-555-1212",
                "101 Main Street, Las Vegas, NV 89101", "1981-01-12"},
            new string[] {DEF456.recordID, "Joanne", "Smith", "212-555-1212",
                "101 Fifth Ave, Las Vegas, NV 10018", "1983-05-15"},
            new string[] {GHI789.recordID, "John", "Doe", "818-555-1313",
                "100 Main Street, Los Angeles, CA 90012", "1978-10-17"},
            new string[] {JKL012.recordID, "Jane", "Doe", "818-555-1212",
                "100 Main Street, Los Angeles, CA 90012", "1979-02-05"}
        };
        return this.PrepareCSVFile("test-passengers-", headers, passengers);
    }

    private FileInfo PrepareEmployeeFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH","MOTHERS_MAIDEN_NAME", "SSN_NUMBER"};

        string[][] employees = {
            new string[] {MNO345.recordID, "Joseph", "Schmoe", "702-555-1212",
                "101 Main Street, Las Vegas, NV 89101", "1981-01-12", "WILSON",
                "145-45-9866"},
            new string[] {PQR678.recordID, "Jo Anne", "Smith", "212-555-1212",
                "101 Fifth Ave, Las Vegas, NV 10018", "1983-05-15", "JACOBS",
                "213-98-9374"},
            new string[] {ZYX321.recordID, "Mark", "Hightower", "563-927-2833",
                "1882 Meadows Lane, Las Vegas, NV 89125", "1981-06-22", "JENKINS",
                "873-22-4213"},
            new string[] {CBA654.recordID, "Mark", "Hightower", "781-332-2824",
                "2121 Roscoe Blvd, Los Angeles, CA 90232", "1980-09-09", "BROOKS",
                "827-27-4829"}
        };

        return this.PrepareJsonArrayFile("test-employees-", headers, employees);
    }

    private FileInfo PrepareVipFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH","MOTHERS_MAIDEN_NAME"};

        string[][] vips = {
            new string[] {STU901.recordID, "Joe", "Schmoe", "702-555-1212",
                "101 Main Street, Las Vegas, NV 89101", "1981-12-01", "WILSON"},
            new string[] {XYZ234.recordID, "Joanne", "Smith", "212-555-1212",
                "101 5th Avenue, Las Vegas, NV 10018", "1983-05-15", "JACOBS"},
            new string[] {STU234.recordID, "John", "Doe", "818-555-1313",
                "100 Main Street Ste. A, Los Angeles, CA 90012", "1978-10-17",
                "WILLIAMS" },
            new string[] {XYZ456.recordID, "Jane", "Doe", "818-555-1212",
                "100 Main Street Suite A, Los Angeles, CA 90012", "1979-02-05",
                "JENKINS" }
        };

        return this.PrepareJsonFile("test-vips-", headers, vips);
    }

    private FileInfo PrepareDualIdentitiesFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FULL", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH", "GENDER" };

        string[][] spouses = {
            new string[] {BCD123.recordID, "Bruce Wayne", "201-765-3451",
                "101 Wayne Court; Gotham City, NJ 07017", "1974-06-05", "M" },
            new string[] {CDE456.recordID, "Jack Napier", "201-875-2314",
                "101 Falconi Boulevard; Gotham City, NJ 07017", "1965-05-14", "M" },
            new string[] {EFG789.recordID, "Batman", "201-782-3214",
                "Batcave; Gotham City, NJ 07020", "", "M" },
            new string[] {FGH012.recordID, "Joker", "201-832-2321",
                "101 Arkham Road; Gotham City, NJ 07018", "1965-05-14", "M" }
        };

        return this.PrepareJsonFile("test-marriages-", headers, spouses);
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

    private static List<object?> BatmanVirtualEntityArgs()
    {
        IDictionary<string, ISet<string>> primaryFeatureValues
            = new Dictionary<string, ISet<string>>();

        primaryFeatureValues.Add("NAME", SetOf("Bruce Wayne", "Batman"));

        int expectedRecordCount = 2;

        IDictionary<string, int> expectedFeatureCounts
            = new Dictionary<string, int>();
        expectedFeatureCounts.Add("NAME", 2);
        expectedFeatureCounts.Add("DOB", 1);
        expectedFeatureCounts.Add("ADDRESS", 2);
        expectedFeatureCounts.Add("PHONE", 2);

        return ListOf<object?>(SortedSetOf(BCD123, EFG789),
                               null,                   // flags
                               expectedRecordCount,
                               expectedFeatureCounts,
                               primaryFeatureValues,
                               null);                  // expected exception
    }

    private static List<object?> JokerVirtualEntityArgs()
    {
        IDictionary<string, ISet<string>> primaryFeatureValues
            = new Dictionary<string, ISet<string>>();

        primaryFeatureValues.Add("NAME", SetOf("Jack Napier", "Joker"));

        int expectedRecordCount = 2;

        IDictionary<string, int> expectedFeatureCounts
            = new Dictionary<string, int>();
        expectedFeatureCounts.Add("NAME", 2);
        expectedFeatureCounts.Add("DOB", 1);
        expectedFeatureCounts.Add("ADDRESS", 2);
        expectedFeatureCounts.Add("PHONE", 2);

        return ListOf<object?>(SortedSetOf(CDE456, FGH012),
                               null,
                               expectedRecordCount,
                               expectedFeatureCounts,
                               primaryFeatureValues,
                               null);
    }

    private static List<object?[]> GetVirtualEntityParameters()
    {
        List<object?[]> result = new List<object?[]>();

        List<List<object?>> templateArgs = ListOf(
            BatmanVirtualEntityArgs(), JokerVirtualEntityArgs());

        foreach (List<object?> args in templateArgs)
        {
            foreach (SzFlag? flags in VirtualEntityFlagSets)
            {
                object?[] argsArray = args.ToArray();
                argsArray[1] = flags;
                result.Add(argsArray);
            }
        }

        result.Add(new object?[] {
            SortedSetOf((UNKNOWN_DATA_SOURCE, "ABC123"), CDE456),
            SzHowEntityDefaultFlags,
            0,
            null,
            null,
            typeof(SzUnknownDataSourceException)});

        result.Add(new object?[] {
            SortedSetOf((DualIdentities, "XXX000"), CDE456),
            SzHowEntityDefaultFlags,
            0,
            null,
            null,
            typeof(SzNotFoundException)});

        return result;
    }

    [Test, TestCaseSource(nameof(GetVirtualEntityParameters))]
    public void TestGetVirtualEntity(
        ISet<(string, string)> recordKeys,
        SzFlag? flags,
        int expectedRecordCount,
        IDictionary<string, int>? expectedFeatureCounts,
        IDictionary<string, ISet<string>>? primaryFeatureValues,
        Type? exceptionType)
    {
        string testData = "recordKeys=[ " + recordKeys + " ], flags=[ "
            + SzVirtualEntityFlags.FlagsToString(flags)
            + " ], expectedRecordCount=[ " + expectedRecordCount
            + " ], expectedFeatureCounts=[ " + expectedFeatureCounts
            + " ], primaryFeatureValues=[ "
            + (primaryFeatureValues == null ? null : primaryFeatureValues.ToDebugString())
            + " ], expectedException=[ " + exceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.GetVirtualEntity(recordKeys, flags);

                if (exceptionType != null)
                {
                    Fail("Unexpectedly succeeded getVirtualEntity() call: "
                         + testData);
                }

                this.ValidateVirtualEntity(result,
                                           testData,
                                           recordKeys,
                                           flags,
                                           expectedRecordCount,
                                           expectedFeatureCounts,
                                           primaryFeatureValues);

            }
            catch (Exception e)
            {
                string description = "";
                if (e is SzException)
                {
                    SzException sze = (SzException)e;
                    description = "errorCode=[ " + sze.ErrorCode
                        + " ], exception=[ " + e.ToString() + " ]";
                }
                else
                {
                    description = "exception=[ " + e.ToString() + " ]";
                }

                if (exceptionType == null)
                {
                    Fail("Unexpectedly failed getVirtualEntity(): "
                         + testData + ", " + description, e);

                }
                else if (exceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        exceptionType, e,
                        "whyEntities() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    private static List<object?[]> GetVirtualEntityDefaultParameters()
    {
        List<object?[]> argsList = GetVirtualEntityParameters();

        List<object?[]> result = new List<object?[]>(argsList.Count);

        for (int index = 0; index < argsList.Count; index++)
        {
            object?[] args = argsList[index];

            // skip the ones that expect an exception
            if (args[args.Length - 1] != null) continue;
            result.Add(new object?[] { args[0] });
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetVirtualEntityDefaultParameters))]
    public void TestVirtualEntityDefaults(ISet<(string, string)> recordKeys)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string defaultResult = engine.GetVirtualEntity(recordKeys);

                string explicitResult = engine.GetVirtualEntity(
                    recordKeys, SzVirtualEntityDefaultFlags);

                string encodedRecordKeys = SzCoreEngine.EncodeRecordKeys(recordKeys);

                long returnCode = engine.GetNativeApi().GetVirtualEntityByRecordID(
                    encodedRecordKeys, out string nativeResult);

                if (returnCode != 0)
                {
                    Fail("Errant return code from native function: " +
                         engine.GetNativeApi().GetLastExceptionCode()
                         + " / " + engine.GetNativeApi().GetLastException());
                }

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(defaultResult, Is.EqualTo(nativeResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the native function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting entity by record", e);
            }
        });
    }

    public virtual void ValidateVirtualEntity(
        string result,
        string testData,
        ISet<(string, string)> recordKeys,
        SzFlag? flags,
        int expectedRecordCount,
        IDictionary<string, int>? expectedFeatureCounts,
        IDictionary<string, ISet<string>>? primaryFeatureValues)
    {
        JsonObject? jsonObject = null;
        try
        {
            jsonObject = JsonNode.Parse(result)?.AsObject();

            if (jsonObject == null)
            {
                throw new JsonException("Not parsed as object");
            }

        }
        catch (Exception e)
        {
            Fail("Virtual entity result did not parse as JSON: "
                 + testData, e);
        }

        JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();
        Assert.IsNotNull(entity,
                    "The RESOLVED_ENTITY property is missing or null: "
                      + testData + ", result=[ " + result + " ]");

        long? jsonID = entity?["ENTITY_ID"]?.GetValue<long>();
        Assert.IsNotNull(jsonID, "The ENTITY_ID property is missing or null: "
                      + testData + ", result=[ " + result + " ]");

        long entityID = (jsonID ?? 0L); // cannot be null

        string? name = entity?["ENTITY_NAME"]?.GetValue<string>();
        if (flags != null && ((flags & SzEntityIncludeEntityName) != SzNoFlags))
        {
            Assert.IsNotNull(name, "The ENTITY_NAME property is missing or null: "
                             + testData + ", entity=[ " + entity + " ]");
        }
        else
        {
            Assert.IsNull(name,
                          "The ENTITY_NAME property was provided despite flags: "
                          + testData + " ], name=[ " + name + " ]");
        }

        JsonObject? features = entity?["FEATURES"]?.AsObject();
        if (flags != null && (flags & FeatureFlags) != SzNoFlags)
        {
            Assert.IsNotNull(features, "The FEATURES property is missing or null: "
                             + testData + ", entity=[ " + entity + " ]");

            IDictionary<string, ISet<string>> actualFeatures = new Dictionary<string, ISet<string>>();
            IDictionary<string, JsonNode?> featuresDictionary
                = ((IDictionary<string, JsonNode?>?)features) ?? new Dictionary<string, JsonNode?>();

            foreach (KeyValuePair<string, JsonNode?> pair in featuresDictionary)
            {
                string key = pair.Key;
                JsonNode? value = pair.Value;

                JsonArray? valuesArray = value?.AsArray();
                if (!actualFeatures.ContainsKey(key))
                {
                    actualFeatures.Add(key, new SortedSet<string>());
                }
                ISet<string> valueSet = actualFeatures[key];

                int featureCount = valuesArray?.Count ?? 0;
                for (int index = 0; index < featureCount; index++)
                {
                    JsonObject? feature = valuesArray?[index]?.AsObject();
                    string primaryValue = feature?["FEAT_DESC"]?.GetValue<string>() ?? "";
                    valueSet.Add(primaryValue);
                }
            }
            ;

            // verify the feature counts
            if (expectedFeatureCounts != null)
            {
                foreach (KeyValuePair<string, int> pair in expectedFeatureCounts)
                {
                    string key = pair.Key;
                    int count = pair.Value;

                    Assert.IsTrue(actualFeatures.ContainsKey(key),
                        "No features found for expected type (" + key + "): "
                        + testData);

                    ISet<string> actuals = actualFeatures[key];

                    Assert.That(actuals.Count, Is.EqualTo(count),
                        "Unexpected number of values for feature (" + key
                        + "): values=[ " + actuals + " ], " + testData);
                }
            }

            // verify the features
            if (primaryFeatureValues != null)
            {
                foreach (KeyValuePair<string, ISet<string>> pair in primaryFeatureValues)
                {
                    string key = pair.Key;
                    ISet<string> values = pair.Value;

                    Assert.IsTrue(actualFeatures.ContainsKey(key),
                        "No primary feature values found for expected "
                        + "type (" + key + "): " + testData);

                    ISet<string> actuals = actualFeatures[key];

                    Assert.IsTrue(actuals.SetEquals(values),
                        "Unexpected primary values for feature (" + key
                        + "): " + testData);
                }
            }
        }
        else
        {
            Assert.IsNull(features,
                       "The FEATURES property was provided despite flags: "
                       + testData + " ], features=[ " + features + " ]");
        }

        JsonArray? records = entity?["RECORDS"]?.AsArray();
        if (flags != null && (flags & RecordFlags) != SzNoFlags)
        {
            Assert.IsNotNull(records, "The RECORDS property is missing or null: "
                          + testData + ", entity=[ " + entity + " ]");

            Assert.That(records?.Count, Is.EqualTo(expectedRecordCount),
                "Unexpected number of records: " + testData
                + " ], records=[ " + records + " ]");

        }
        else
        {
            Assert.IsNull(records,
                        "The RECORDS property was provided despite flags: "
                        + testData + " ], records=[ " + records + " ]");
        }

    }

    private static List<object?[]> GetHowEntityParameters()
    {
        List<object?[]> result = new List<object?[]>();

        List<ISet<(string, string)>> recordSets = ListOf<ISet<(string, string)>>(
            SortedSetOf(ABC123, MNO345, STU901),  // Joe Schmoe
            SortedSetOf(DEF456, PQR678, XYZ234),  // Joanne Smith
            SortedSetOf(GHI789, STU234),          // John Doe
            SortedSetOf(JKL012, XYZ456));         // Jane Doe

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(HowFlagSets);

        foreach (ISet<(string, string)> set in recordSets)
        {
            foreach ((string, string) recordKey in set)
            {
                result.Add(new object?[] {
                    recordKey,
                    (SzCoreEngineHowTest t) => t.GetEntityID(recordKey),
                    flagSetIter.Next(),
                    set.Count,
                    set,
                    null});
            }
        }

        result.Add(new object?[] {
            (UNKNOWN_DATA_SOURCE, "ABC123"),
            (SzCoreEngineHowTest t) => -200L,
            flagSetIter.Next(),
            0,
            ImmutableHashSet<(string,string)>.Empty,
            typeof(SzNotFoundException)});

        result.Add(new object?[] {
            (Passengers, "XXX000"),
            (SzCoreEngineHowTest t) => 200000000L,
            flagSetIter.Next(),
            0,
            ImmutableHashSet<(string,string)>.Empty,
            typeof(SzNotFoundException)});

        return result;
    }

    [Test, TestCaseSource(nameof(GetHowEntityParameters))]
    public void TestHowEntity(
        (string dataSourceCode, string recordID) recordKey,
        Func<SzCoreEngineHowTest, long> entityIDFunc,
        SzFlag? flags,
        int? expectedRecordCount,
        ISet<(string, string)> expectedRecordKeys,
        Type? exceptionType)
    {
        long entityID = entityIDFunc(this);

        string testData = "recordKey=[ " + recordKey
            + " ], entityID=[ " + entityID + " ], flags=[ "
            + SzHowFlags.FlagsToString(flags)
            + " ], expectedRecordCount=[ " + expectedRecordCount
            + " ], expectedRecordKeys=[ " + expectedRecordKeys.ToDebugString()
            + " ], expectedException=[ " + exceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.HowEntity(entityID, flags);

                if (exceptionType != null)
                {
                    Fail("Unexpectedly succeeded howEntity() call: "
                         + testData);
                }

                this.ValidateHowEntityResult(result,
                                             testData,
                                             recordKey,
                                             entityID,
                                             flags,
                                             expectedRecordCount,
                                             expectedRecordKeys);

            }
            catch (Exception e)
            {
                string description = "";
                if (e is SzException)
                {
                    SzException sze = (SzException)e;
                    description = "errorCode=[ " + sze.ErrorCode
                        + " ], exception=[ " + e.ToString() + " ]";
                }
                else
                {
                    description = "exception=[ " + e.ToString() + " ]";
                }

                if (exceptionType == null)
                {
                    Fail("Unexpectedly failed howEntity(): "
                         + testData + ", " + description, e);

                }
                else if (exceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        exceptionType, e,
                        "howEntity() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestHowEntityDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                long entityID = GetEntityID(recordKey);

                string defaultResult = engine.HowEntity(entityID);

                string explicitResult = engine.HowEntity(
                    entityID, SzHowEntityDefaultFlags);

                long returnCode = engine.GetNativeApi().HowEntityByEntityID(
                    entityID, out string nativeResult);

                if (returnCode != 0)
                {
                    Fail("Errant return code from native function: " +
                         engine.GetNativeApi().GetLastExceptionCode()
                         + " / " + engine.GetNativeApi().GetLastException());
                }

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(defaultResult, Is.EqualTo(nativeResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the native function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting entity by record", e);
            }
        });
    }


    public virtual void ValidateHowEntityResult(
        string result,
        string testData,
        (string dataSourceCode, string recordID) recordKey,
        long entityID,
        SzFlag? flags,
        int? expectedRecordCount,
        ISet<(string, string)> expectedRecordKeys)
    {
        JsonObject? jsonObject = null;
        try
        {
            jsonObject = JsonNode.Parse(result)?.AsObject();

            if (jsonObject == null)
            {
                throw new JsonException("Did not parse as JsonObject");
            }

        }
        catch (Exception e)
        {
            Fail("How entity result did not parse as JSON: "
                 + testData, e);
        }

        JsonObject? how = jsonObject?["HOW_RESULTS"]?.AsObject();
        Assert.IsNotNull(how,
                         "The HOW_RESULTS property is missing: "
                         + testData + ", result=[ " + result + " ]");

        JsonArray? steps = how["RESOLUTION_STEPS"]?.AsArray();
        Assert.IsNotNull(steps, "The RESOLUTION_STEPS property is missing: "
            + testData + ", result=[ " + result + " ]");

        SzFlag stepFlags = SzIncludeFeatureScores | SzIncludeMatchKeyDetails;

        if (flags != null && (flags & stepFlags) != SzNoFlags)
        {
            int stepCount = steps?.Count ?? 0;
            for (int index = 0; index < stepCount; index++)
            {
                JsonObject? step = steps?[index]?.AsObject();
                JsonObject? matchInfo = step?["MATCH_INFO"]?.AsObject();
                Assert.IsNotNull(matchInfo, "The MATCH_INFO property is missing: "
                    + testData + ", step=[ " + step + " ]");

                JsonObject? scores = matchInfo?["FEATURE_SCORES"]?.AsObject();
                if (flags != null && (flags & SzIncludeFeatureScores) != SzNoFlags)
                {
                    Assert.IsNotNull(scores, "The FEATURE_SCORES property is missing: "
                        + testData + ", matchInfo=[ " + matchInfo + " ]");

                }
                else
                {
                    Assert.IsNull(scores, "The FEATURE_SCORES are present, despite flags: "
                        + testData + ", matchInfo=[ " + matchInfo + " ]");
                }

                JsonObject? details = matchInfo?["MATCH_KEY_DETAILS"]?.AsObject();
                if ((flags & SzIncludeMatchKeyDetails) != SzNoFlags)
                {
                    Assert.IsNotNull(details, "The MATCH_KEY_DETAILS property is missing: "
                        + testData + ", matchInfo=[ " + matchInfo + " ]");
                }
                else
                {
                    Assert.IsNull(details, "The MATCH_KEY_DETAILS are present, despite "
                        + "flags: " + testData + ", matchInfo=[ " + matchInfo + " ]");
                }
            }
            ;
        }
        JsonObject? finalState = how?["FINAL_STATE"]?.AsObject();
        Assert.IsNotNull(finalState, "The FINAL_STATE property is missing: "
                         + testData + ", result=[ " + result + " ]");

        JsonArray? virtualEntities = finalState["VIRTUAL_ENTITIES"]?.AsArray();
        Assert.IsNotNull(virtualEntities, "The VIRTUAL_ENTITIES property is missing: "
                         + testData + ", finalState=[ " + finalState + " ]");

        ISet<(string, string)> actualRecords = new HashSet<(string, string)>();
        int virtualCount = virtualEntities?.Count ?? 0; // cannot be null
        for (int index = 0; index < virtualCount; index++)
        {
            JsonObject? virtualEntity = virtualEntities?[index]?.AsObject();
            JsonArray? memberRecords = virtualEntity?["MEMBER_RECORDS"]?.AsArray();

            Assert.IsNotNull(memberRecords, "The MEMBER_RECORDS property is missing: "
                + testData + ", virtualEntity=[ " + virtualEntity + " ]");

            int memberCount = memberRecords?.Count ?? 0;

            for (int index2 = 0; index2 < memberCount; index2++)
            {
                JsonObject? memberRecord = memberRecords?[index2]?.AsObject();
                JsonArray? records = memberRecord?["RECORDS"]?.AsArray();

                Assert.IsNotNull(records, "The RECORDS property is missing: "
                    + testData + ", memberRecord=[ " + memberRecord + " ]");

                int recordCount = records?.Count ?? 0; // cannot be null

                for (int index3 = 0; index3 < recordCount; index3++)
                {
                    JsonObject? record = records?[index3]?.AsObject();
                    string? dataSourceCode = record?["DATA_SOURCE"]?.GetValue<string>();
                    Assert.IsNotNull(dataSourceCode, "The DATA_SOURCE property is missing: "
                        + testData + ", record=[ " + record + " ]");

                    string? recordID = record?["RECORD_ID"]?.GetValue<string>();
                    Assert.IsNotNull(recordID, "The RECORD_ID property is missing: "
                        + testData + ", record=[ " + record + " ]");

                    // neither field can be null at this point
                    actualRecords.Add((dataSourceCode ?? "", recordID ?? ""));
                }
            }
        }

        if (expectedRecordKeys != null)
        {
            Assert.IsTrue(actualRecords.SetEquals(expectedRecordKeys),
                "The records (" + actualRecords.ToDebugString()
                + ") were not as expected: " + testData);
        }

        if (expectedRecordCount != null)
        {
            Assert.That(actualRecords.Count, Is.EqualTo(expectedRecordCount),
                "The number of records were not as expected: " + testData);
        }
    }
}
