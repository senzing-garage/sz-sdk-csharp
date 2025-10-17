namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
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
internal class SzCoreEngineGraphTest : AbstractTest
{
    private const string EmployeesDataSource = "EMPLOYEES";
    private const string PassengersDataSource = "PASSENGERS";
    private const string VipsDataSource = "VIPS";
    private const string UnknownDataSource = "UNKNOWN";

    private static readonly IList<SzFlag?> FindPathFlagSet;

    private static readonly IList<SzFlag?> FindNetworkFlagSet;

    static SzCoreEngineGraphTest()
    {
        List<SzFlag?> list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzFlags.SzNoFlags);
        list.Add(SzFlags.SzFindPathDefaultFlags);
        list.Add(SzFlags.SzFindPathAllFlags);
        list.Add(SzFlag.SzFindPathIncludeMatchingInfo
                 | SzFlag.SzEntityIncludeEntityName
                 | SzFlag.SzEntityIncludeRecordSummary
                 | SzFlag.SzEntityIncludeRecordData
                 | SzFlag.SzEntityIncludeRecordMatchingInfo);
        list.Add(SzFlag.SzEntityIncludeEntityName);
        FindPathFlagSet = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzFlags.SzNoFlags);
        list.Add(SzFlags.SzFindNetworkDefaultFlags);
        list.Add(SzFlags.SzFindNetworkAllFlags);
        list.Add(SzFlag.SzFindNetworkIncludeMatchingInfo
                 | SzFlag.SzEntityIncludeEntityName
                 | SzFlag.SzEntityIncludeRecordSummary
                 | SzFlag.SzEntityIncludeRecordData
                 | SzFlag.SzEntityIncludeRecordMatchingInfo);
        list.Add(SzFlag.SzEntityIncludeEntityName);
        FindNetworkFlagSet = list.AsReadOnly();
    }

    private static readonly (string dataSourceCode, string recordID) PassengerABC123
        = (PassengersDataSource, "ABC123");

    private static readonly (string dataSourceCode, string recordID) PassengerDEF456
        = (PassengersDataSource, "DEF456");

    private static readonly (string dataSourceCode, string recordID) PassengerGHI789
        = (PassengersDataSource, "GHI789");

    private static readonly (string dataSourceCode, string recordID) PassengerJKL012
        = (PassengersDataSource, "JKL012");

    private static readonly (string dataSourceCode, string recordID) EmployeeMNO345
        = (EmployeesDataSource, "MNO345");

    private static readonly (string dataSourceCode, string recordID) EmployeePQR678
        = (EmployeesDataSource, "PQR678");

    private static readonly (string dataSourceCode, string recordID) EmployeeABC567
        = (EmployeesDataSource, "ABC567");

    private static readonly (string dataSourceCode, string recordID) EmployeeDEF890
        = (EmployeesDataSource, "DEF890");

    private static readonly (string dataSourceCode, string recordID) VipSTU901
        = (VipsDataSource, "STU901");

    private static readonly (string dataSourceCode, string recordID) VipXYZ234
        = (VipsDataSource, "XYZ234");

    private static readonly (string dataSourceCode, string recordID) VipGHI123
        = (VipsDataSource, "GHI123");

    private static readonly (string dataSourceCode, string recordID) VipJKL456
        = (VipsDataSource, "JKL456");


    private static readonly List<(string, string)> GraphRecordKeys
        = ListOf(PassengerABC123,
                 PassengerDEF456,
                 PassengerGHI789,
                 PassengerJKL012,
                 EmployeeMNO345,
                 EmployeePQR678,
                 EmployeeABC567,
                 EmployeeDEF890,
                 VipSTU901,
                 VipXYZ234,
                 VipGHI123,
                 VipJKL456);

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

            //Get the loaded records and entity ID's
            foreach ((string dataSourceCode, string recordID) key in GraphRecordKeys)
            {
                // clear the buffer
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
                    LoadedEntityMap.Add(entityID, new SortedSet<(string, string)>());
                }
                ISet<(string, string)> recordKeySet = LoadedEntityMap[entityID];
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

        SortedSet<string> dataSources = SortedSetOf(PassengersDataSource,
                                                    EmployeesDataSource,
                                                    VipsDataSource);

        FileInfo passengerFile = this.PreparePassengerFile();
        FileInfo employeeFile = this.PrepareEmployeeFile();
        FileInfo vipFile = this.PrepareVipFile();

        RepositoryManager.ConfigSources(repoDirectory,
                                        dataSources,
                                        true);

        RepositoryManager.LoadFile(repoDirectory,
                                    passengerFile,
                                    PassengersDataSource,
                                    true);

        RepositoryManager.LoadFile(repoDirectory,
                                    employeeFile,
                                    EmployeesDataSource,
                                    true);

        RepositoryManager.LoadFile(repoDirectory,
                                    vipFile,
                                    VipsDataSource,
                                    true);
    }

    private FileInfo PreparePassengerFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        string[][] passengers = {
            new string[] {PassengerABC123.recordID, "Joseph", "Schmidt", "213-555-1212",
                "818-777-2424", "101 Main Street, Los Angeles, CA 90011", "12-JAN-1981"},
            new string[] {PassengerDEF456.recordID, "Joann", "Smith", "213-555-1212",
                "818-888-3939", "101 Fifth Ave, Los Angeles, CA 90018", "15-MAR-1982"},
            new string[] {PassengerGHI789.recordID, "John", "Parker", "818-555-1313",
                "818-999-2121", "101 Fifth Ave, Los Angeles, CA 90018", "17-DEC-1977"},
            new string[] {PassengerJKL012.recordID, "Jane", "Donaldson", "818-555-1313",
                "818-222-3131", "400 River Street, Pasadena, CA 90034", "23-MAY-1973"}
        };
        return this.PrepareCSVFile("test-passengers-", headers, passengers);
    }

    private FileInfo PrepareEmployeeFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        string[][] employees = {
            new string[] {EmployeeMNO345.recordID, "Bill", "Bandley", "818-444-2121",
                "818-123-4567", "101 Main Street, Los Angeles, CA 90011", "22-AUG-1981"},
            new string[] {EmployeePQR678.recordID, "Craig", "Smith", "818-555-1212",
                "818-888-3939", "451 Dover Street, Los Angeles, CA 90018", "17-OCT-1983"},
            new string[] {EmployeeABC567.recordID, "Kim", "Long", "818-246-8024",
                "818-135-7913", "451 Dover Street, Los Angeles, CA 90018", "24-NOV-1975"},
            new string[] {EmployeeDEF890.recordID, "Katrina", "Osmond", "818-444-2121",
                "818-111-2222", "707 Seventh Ave, Los Angeles, CA 90043", "27-JUN-1980"}
        };

        return this.PrepareJsonArrayFile("test-employees-", headers, employees);
    }

    private FileInfo PrepareVipFile()
    {
        string[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        string[][] vips = {
            new string[] {VipSTU901.recordID, "Martha", "Wayne", "818-891-9292",
                "818-987-1234", "888 Sepulveda Blvd, Los Angeles, CA 90034", "27-NOV-1973"},
            new string[] {VipXYZ234.recordID, "Jane", "Johnson", "818-333-7171",
                "818-123-9876", "400 River Street, Pasadena, CA 90034", "6-SEP-1975"},
            new string[] {VipGHI123.recordID, "Martha", "Kent", "818-333-5757",
                "818-123-9876", "888 Sepulveda Blvd, Los Angeles, CA 90034", "17-AUG-1978"},
            new string[] {VipJKL456.recordID, "Kelly", "Rogers", "818-333-7171",
                "818-789-6543", "707 Seventh Ave, Los Angeles, CA 90043", "15-JAN-1979"}
        };

        return this.PrepareJsonFile("test-vips-", headers, vips);
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

    public List<long>? GetEntityIDsNull(ICollection<(string, string)?>? recordKeys)
    {
        if (recordKeys == null) return null;
        List<long> result = new List<long>(recordKeys.Count);
        foreach ((string dataSourceCode, string recordID)? recordKey in recordKeys)
        {
            if (recordKey != null)
            {
                result.Add(this.GetEntityID(recordKey ?? ("A", "B")));
            }
        }
        return result;
    }

    public List<long>? GetEntityIDsNull(ICollection<(string, string)>? recordKeys)
    {
        if (recordKeys == null) return null;
        List<long> result = new List<long>(recordKeys.Count);
        foreach ((string dataSourceCode, string recordID) recordKey in recordKeys)
        {
            result.Add(this.GetEntityID(recordKey));
        }
        return result;
    }

    public List<long> GetEntityIDs(ICollection<(string, string)> recordKeys)
    {
        List<long> result = new List<long>(recordKeys.Count);
        foreach ((string dataSourceCode, string recordID) recordKey in recordKeys)
        {
            result.Add(this.GetEntityID(recordKey));
        }
        return result;

    }
    public List<long> GetEntityIDs(ICollection<(string, string)?> recordKeys)
    {
        List<long> result = new List<long>(recordKeys.Count);
        foreach ((string dataSourceCode, string recordID)? recordKey in recordKeys)
        {
            if (recordKey != null)
            {
                result.Add(this.GetEntityID(recordKey ?? ("A", "B")));
            }
        }
        return result;
    }

    public SortedSet<long> EntityIDSet(ISet<(string, string)> recordKeys)
    {
        SortedSet<long> result = new SortedSet<long>();
        foreach ((string dataSourceCode, string recordID) key in recordKeys)
        {
            result.Add(this.GetEntityID(key));
        }
        return result;
    }

    public void ValidatePath(string pathJson,
                             string testData,
                             (string, string) startRecordKey,
                             (string, string) endRecordKey,
                             int maxDegrees,
                             ISet<(string, string)>? avoidances,
                             ISet<long>? avoidanceIDs,
                             ISet<string>? requiredSources,
                             SzFlag? flags,
                             int expectedPathLength,
                             IList<(string, string)> expectedPath)
    {
        JsonObject? jsonObject = null;
        try
        {
            jsonObject = JsonNode.Parse(pathJson)?.AsObject();
            if (jsonObject == null) throw new JsonException();

        }
        catch (Exception e)
        {
            Fail("Unable to parse find-path result as JSON: " + pathJson, e);
        }

        JsonArray? entityPaths = jsonObject?["ENTITY_PATHS"]?.AsArray();

        Assert.IsNotNull(entityPaths, "Entity path is missing: path=[ "
                        + pathJson + " ], " + testData);

        Assert.That(entityPaths.Count, Is.EqualTo(1),
                     "Paths array has unexpected length: paths=[ "
                     + entityPaths + " ], " + testData);

        JsonObject? path0 = entityPaths?[0]?.AsObject();

        Assert.IsNotNull(path0, "Entity path was null: paths=[ " + entityPaths
                         + " ], " + testData);

        JsonArray? entityIDs = path0?["ENTITIES"]?.AsArray();

        JsonArray? pathLinks = jsonObject?["ENTITY_PATH_LINKS"]?.AsArray();

        if (flags != null
            && ((flags & SzFlag.SzFindPathIncludeMatchingInfo) != SzNoFlags))
        {
            Assert.IsNotNull(pathLinks, "Entity path links missing or null: "
                             + "pathLinks=[ " + pathLinks + " ], " + testData);
        }
        else
        {
            Assert.IsNull(pathLinks, "Entity path links present when not requested.  "
                           + "pathLinks=[ " + pathLinks + " ], " + testData);
        }

        JsonArray? entities = jsonObject?["ENTITIES"]?.AsArray();

        Assert.IsNotNull(entities,
                         "Entity details for path are missing: " + testData);

        // validate the path length
        Assert.That(entityIDs?.Count, Is.EqualTo(expectedPathLength),
                     "Path is not of expected length: pathJson=[ "
                     + pathJson + " ], path=[ "
                     + entityIDs + "], " + testData);

        long startEntityID = this.GetEntityID(startRecordKey);
        long endEntityID = this.GetEntityID(endRecordKey);
        List<long>? expectedIDs = this.GetEntityIDs(expectedPath);
        List<long> actualPathIDs = new List<long>(entityIDs?.Count ?? 10);

        long? prevID = null;
        int entityIDCount = entityIDs?.Count ?? 0;
        for (int index = 0; index < entityIDCount; index++)
        {

            long? jsonID = entityIDs?[index]?.GetValue<long>();

            Assert.IsNotNull(jsonID, "The entity ID was null.  entityIDs=[ "
                             + entityIDs + " ], " + testData);

            Assert.That(jsonID, Is.Not.EqualTo(0L),
                        "The entity ID was zero.  entityIDs=[ "
                        + entityIDs + " ], " + testData);

            long entityID = (jsonID ?? 0L); // cannot be null or zero

            if (prevID == null)
            {
                Assert.That(entityID, Is.EqualTo(startEntityID),
                             "The starting entity ID in the path is not as expected: "
                            + "entityIDs=[ " + entityIDs + " ], " + testData);

            }

            if (avoidances != null && avoidances.Count > 0
                && ((flags != null) && ((flags & SzFindPathStrictAvoid) != SzNoFlags)))
            {
                Assert.IsFalse(avoidanceIDs?.Contains(entityID) ?? false,
                               "Strictly avoided entity ID (" + entityID + ") found "
                               + "in path: entityIDs=[ " + entityIDs
                               + " ], recordKeys=[ " + LoadedEntityMap[entityID]
                               + " ], " + testData);
            }

            // create the path list
            actualPathIDs.Add(entityID);
            prevID = jsonID;
        }

        // assert the end of the path is as expected
        if (prevID != null)
        {
            Assert.That(prevID, Is.EqualTo(endEntityID),
                         "The ending entity ID (" + prevID
                         + ") in the path is not as expected: " + testData);
        }

        // now check the path is as expected
        Assert.IsTrue(actualPathIDs.SequenceEqual(expectedIDs),
                      "Entity path is not as expected: path=[ " + entityPaths
                      + " ], " + testData);

        // add the start and end entity ID to the ID set
        actualPathIDs.Add(startEntityID);
        actualPathIDs.Add(endEntityID);

        // check that the entities we found are on the path
        ISet<long> detailEntityIDs = new HashSet<long>();
        int entitiesCount = entities?.Count ?? 0; // cannot be null

        for (int index = 0; index < entitiesCount; index++)
        {
            JsonObject? entity = entities?[index]?.AsObject();

            Assert.IsNotNull(entity, "Entity from path was null: "
                             + entities + ", " + testData);

            entity = entity?["RESOLVED_ENTITY"]?.AsObject();

            Assert.IsNotNull(entity, "Resolved entity from path was null: "
                             + entities + ", " + testData);

            //Get the entity ID
            long? jsonID = entity?["ENTITY_ID"]?.GetValue<long>();

            Assert.IsNotNull(jsonID, "The entity detail from path was missing or "
                             + "null ENTITY_ID: " + entity + ", " + testData);

            long id = (jsonID ?? 0L); // cannot be null

            // ensure the entity ID is expected
            Assert.IsTrue(actualPathIDs.Contains(id),
                          "Entity (" + id + ") returned that is not in "
                          + "the path: entity=[ " + entity + " ], pathIds=[ "
                          + actualPathIDs + " ], " + testData);

            // add to the ID set
            detailEntityIDs.Add(id);
        }

        // check that all the path ID's have details
        foreach (long id in actualPathIDs)
        {
            Assert.IsTrue(
                detailEntityIDs.Contains(id),
                 "A path entity (" + id + ") was missing from entity "
                 + "details: entities=[ " + entities + " ], "
                 + testData);
        }

        // validate the required data sources
        if (requiredSources != null && requiredSources.Count > 0)
        {
            bool sourcesSatisified = false;
            foreach (long entityID in actualPathIDs)
            {
                if (entityID == startEntityID) continue;
                if (entityID == endEntityID) continue;
                ISet<(string, string)> keys = LoadedEntityMap[entityID];
                foreach ((string dataSourceCode, string recordID) key in keys)
                {
                    if (requiredSources.Contains(key.dataSourceCode))
                    {
                        sourcesSatisified = true;
                        break;
                    }
                }
                if (sourcesSatisified) break;
            }
            if (!sourcesSatisified)
            {
                Fail("Entity path does not contain required data sources: "
                     + "entityPath=[ " + actualPathIDs + " ], " + testData);
            }
        }
    }

    public static SzFlag? FlagsWith(SzFlag? baseFlags, params SzFlag[] otherFlags)
    {
        SzFlag result = SzNoFlags;
        if (baseFlags != null)
        {
            result = (baseFlags ?? SzNoFlags);
        }
        foreach (SzFlag flag in otherFlags)
        {
            result |= flag;
        }
        if (result == SzNoFlags && baseFlags == null) return null;
        return result;
    }

    public static SzFlag? FlagsWithout(SzFlag? baseFlags, params SzFlag[] otherFlags)
    {
        if (baseFlags == null) return null;
        SzFlag result = (baseFlags ?? SzNoFlags);
        foreach (SzFlag flag in otherFlags)
        {
            result &= (~flag);
        }
        return result;
    }

    public static SzFlag? FlagsWithStrictAvoid(SzFlag? baseFlags)
    {
        return FlagsWith(baseFlags, SzFindPathStrictAvoid);
    }

    public static SzFlag? FlagsWithDefaultAvoid(SzFlag? baseFlags)
    {
        return FlagsWithout(baseFlags, SzFindPathStrictAvoid);
    }

    public static List<object?[]> GetEntityPathParameters()
    {
        Iterator<SzFlag?> flagSetIter = GetCircularIterator(FindPathFlagSet);

        List<object?[]> result = new List<object?[]>();

        IList<(string, string)> EmptyPath
            = ListOf<(string, string)>().AsReadOnly();

        Type UnknownSource = typeof(SzUnknownDataSourceException);
        Type NotFound = typeof(SzNotFoundException);

        result.Add(new object?[] {
            "Basic path find at 2 degrees",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            EmployeeDEF890,
            (SzCoreEngineGraphTest t) => t.GetEntityID(EmployeeDEF890),
            2, null, null, null, flagSetIter.Next(), null, null, 3,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890)});

        result.Add(new object?[] {
            "Basic path found at 3 degrees",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456),
            3, null, null, null, flagSetIter.Next(), null, null, 4,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890, VipJKL456)});

        result.Add(new object?[] {
            "Path not found due to max degrees",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456),
            2, null, null, null,
            flagSetIter.Next(), null, null, 0, EmptyPath});

        result.Add(new object?[] {
            "Diverted path found with avoidance",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456), 4,
            SortedSetOf(EmployeeDEF890),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(EmployeeDEF890)),
            null, FlagsWithDefaultAvoid(flagSetIter.Next()), null, null,
            4, ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890, VipJKL456)});

        result.Add(new object?[] {
            "No path found due to strict avoidance and max degrees",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456),
            3, SortedSetOf(EmployeeDEF890),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(EmployeeDEF890)),
            null, FlagsWithStrictAvoid(flagSetIter.Next()), null, null, 0,
            EmptyPath});

        result.Add(new object?[] {
            "Diverted path at 5 degrees with strict avoidance",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456),
            10, SortedSetOf(EmployeeDEF890),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(EmployeeDEF890)),
            null, FlagsWithStrictAvoid(flagSetIter.Next()),  null, null, 6,
            ListOf(PassengerABC123, PassengerDEF456, PassengerGHI789,
                   PassengerJKL012, VipXYZ234, VipJKL456)});

        result.Add(new object?[] {
            "Diverted path at 5 degrees due to required EMPLOYEES source",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            PassengerJKL012,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
            10, null, null, SortedSetOf(EmployeesDataSource),
            FlagsWithDefaultAvoid(flagSetIter.Next()), null, null, 6,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890,
                    VipJKL456, VipXYZ234, PassengerJKL012)});

        result.Add(new object?[] {
            "Diverted path at 5 degrees due to required VIP source",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            PassengerJKL012,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
            10, null, null, SortedSetOf(VipsDataSource),
            FlagsWithDefaultAvoid(flagSetIter.Next()), null, null, 6,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890,
                    VipJKL456, VipXYZ234, PassengerJKL012)});

        result.Add(new object?[] {
            "Diverted path at 5 degrees due to 2 required sources",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            PassengerJKL012,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
            10, null, null,
            SortedSetOf(EmployeesDataSource, VipsDataSource),
            FlagsWithDefaultAvoid(flagSetIter.Next()), null, null, 6,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890,
                    VipJKL456, VipXYZ234, PassengerJKL012)});

        result.Add(new object?[] {
            "Diverted path with required sources and avoidance",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            PassengerJKL012,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
            10, SortedSetOf(VipSTU901),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(VipSTU901)),
            SortedSetOf(EmployeesDataSource, VipsDataSource),
            FlagsWithDefaultAvoid(flagSetIter.Next()), null, null, 6,
            ListOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890,
                    VipJKL456, VipXYZ234, PassengerJKL012)});

        result.Add(new object?[] {
            "Unknown required data source",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            EmployeeDEF890,
            (SzCoreEngineGraphTest t) => t.GetEntityID(EmployeeDEF890),
            10, null, null, SortedSetOf(UnknownDataSource), flagSetIter.Next(),
            UnknownSource, UnknownSource, 0, EmptyPath});

        result.Add(new object?[] {
            "Unknown source for avoidance record",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456), 4,
            SortedSetOf((UnknownDataSource, "DEF890")),
            (SzCoreEngineGraphTest t) => SortedSetOf(-300L),
            null, FlagsWithDefaultAvoid(flagSetIter.Next()),
            UnknownSource, null, 4, ListOf(PassengerABC123, EmployeeMNO345,
                                          EmployeeDEF890, VipJKL456)});

        result.Add(new object?[] {
            "Not found record ID for avoidance record",
            PassengerABC123,
            (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
            VipJKL456,
            (SzCoreEngineGraphTest t) => t.GetEntityID(VipJKL456), 4,
            SortedSetOf((PassengersDataSource, "XXX000")),
            (SzCoreEngineGraphTest t) => SortedSetOf(300000000L),
            null, FlagsWithDefaultAvoid(flagSetIter.Next()),
            null, null, 4, ListOf(PassengerABC123, EmployeeMNO345,
                                  EmployeeDEF890, VipJKL456)});

        result.Add(new object?[] {
                "Unknown start data source in find path via key",
                (UnknownDataSource, "ABC123"),
                (SzCoreEngineGraphTest t) => -100L,
                PassengerJKL012,
                (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
                10, null, null, null, flagSetIter.Next(),
                UnknownSource, NotFound, 0, EmptyPath});

        result.Add(new object?[] {
                "Unknown end data source in find path via key",
                PassengerABC123,
                (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
                (UnknownDataSource, "JKL012"),
                (SzCoreEngineGraphTest t) => -200L,
                10, null, null, null, flagSetIter.Next(),
                UnknownSource, NotFound, 0, EmptyPath});

        result.Add(new object?[] {
                "Unknown start record ID in find path via key",
                (PassengersDataSource, "XXX000"),
                (SzCoreEngineGraphTest t) => 100000000L,
                PassengerJKL012,
                (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerJKL012),
                10, null, null, null, flagSetIter.Next(),
                NotFound, NotFound, 0, EmptyPath});

        result.Add(new object?[] {
                "Unknown end record ID in find path via key",
                PassengerABC123,
                (SzCoreEngineGraphTest t) => t.GetEntityID(PassengerABC123),
                (PassengersDataSource, "XXX000"),
                (SzCoreEngineGraphTest t) => 200000000L,
                10, null, null, null, flagSetIter.Next(),
                NotFound, NotFound, 0, EmptyPath});

        return result;
    }

    public static List<object?[]> GetEntityPathDefaultParameters()
    {
        List<object?[]> argsList = GetEntityPathParameters();

        List<object?[]> result = new List<object?[]>(argsList.Count);

        for (int index = 0; index < argsList.Count; index++)
        {
            object?[] args = argsList[index];
            // skip the ones that expect an exception
            if (args[args.Length - 3] != null) continue;
            if (args[args.Length - 4] != null) continue;
            result.Add(new object?[] {
                args[1], args[3], args[5], args[6], args[7], args[8], args[9] });
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetEntityPathParameters))]
    public void TestFindPathByRecordID(
        string testDescription,
        (string dataSourceCode, string recordID) startRecordKey,
        Func<SzCoreEngineGraphTest, long> startEntityIDFunc,
        (string dataSourceCode, string recordID) endRecordKey,
        Func<SzCoreEngineGraphTest, long> endEntityIDFunc,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType,
        int expectedPathLength,
        IList<(string, string)> expectedPath)
    {
        long startEntityID = startEntityIDFunc(this);
        long endEntityID = endEntityIDFunc(this);
        ISet<long>? avoidanceIDs = avoidanceIDsFunc != null ? avoidanceIDsFunc(this) : null;

        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], startRecordKey=[ "
            + startRecordKey + " ], startRecordId=[ " + startEntityID
            + " ] endRecordKey=[ " + endRecordKey + " ], endRecordId=[ "
            + endEntityID + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
            + avoidances + " ], avoidanceIDs=[ " + avoidanceIDs
            + " ] requiredSources=[ " + requiredSources + " ], flags=[ "
            + SzFindPathFlags.FlagsToString(flags) + " ], expectedException=[ "
            + recordExceptionType + " ], expectedPathLength=[ "
            + expectedPathLength + " ], expectedPath=[ ");

        string prefix = "";
        foreach ((string dataSourceCode, string recordID) key in expectedPath)
        {
            sb.Append(prefix).Append(key).Append(" {");
            sb.Append(this.GetEntityID(key)).Append('}');
            prefix = ", ";
        }
        sb.Append(" ], recordMap=[ ");
        sb.Append(LoadedRecordMap.ToDebugString()).Append(" ]");
        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindPath(
                        startRecordKey.dataSourceCode,
                        startRecordKey.recordID,
                        endRecordKey.dataSourceCode,
                        endRecordKey.recordID,
                        maxDegrees,
                        avoidances,
                        requiredSources,
                        flags);

                if (recordExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in finding a path: " + testData);
                }

                this.ValidatePath(result,
                                  testData,
                                  startRecordKey,
                                  endRecordKey,
                                  maxDegrees,
                                  avoidances,
                                  avoidanceIDs,
                                  requiredSources,
                                  flags,
                                  expectedPathLength,
                                  expectedPath);


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

                if (recordExceptionType == null)
                {
                    Fail("Unexpectedly failed finding an entity path: "
                         + testData + ", " + description, e);

                }
                else if (recordExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        recordExceptionType, e,
                        "FindPath() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });

    }

    [Test, TestCaseSource(nameof(GetEntityPathParameters))]
    public void TestFindPathByEntityID(
        string testDescription,
        (string dataSourceCode, string recordID) startRecordKey,
        Func<SzCoreEngineGraphTest, long> startEntityIDFunc,
        (string dataSourceCode, string recordID) endRecordKey,
        Func<SzCoreEngineGraphTest, long> endEntityIDFunc,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType,
        int expectedPathLength,
        IList<(string, string)> expectedPath)
    {
        long startEntityID = startEntityIDFunc(this);
        long endEntityID = endEntityIDFunc(this);
        ISet<long>? avoidanceIDs = avoidanceIDsFunc != null ? avoidanceIDsFunc(this) : null;

        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], startRecordKey=[ "
            + startRecordKey + " ], startRecordId=[ " + startEntityID
            + " ] endRecordKey=[ " + endRecordKey + " ], endRecordId=[ "
            + endEntityID + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
            + avoidances + " ], avoidanceIDs=[ " + avoidanceIDs
            + " ] requiredSources=[ " + requiredSources + " ], flags=[ "
            + SzFindPathFlags.FlagsToString(flags) + " ], expectedException=[ "
            + entityExceptionType + " ], expectedPathLength=[ "
            + expectedPathLength + " ], expectedPath=[ ");

        string prefix = "";
        foreach ((string, string) key in expectedPath)
        {
            sb.Append(prefix).Append(key).Append(" {");
            sb.Append(this.GetEntityID(key)).Append('}');
            prefix = ", ";
        }
        sb.Append(" ], recordMap=[ ");
        sb.Append(LoadedRecordMap.ToDebugString()).Append(" ]");
        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindPath(
                        startEntityID,
                        endEntityID,
                        maxDegrees,
                        avoidanceIDs,
                        requiredSources,
                        flags);

                if (entityExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in finding a path: " + testData);
                }

                ValidatePath(result,
                             testData,
                             startRecordKey,
                             endRecordKey,
                             maxDegrees,
                             avoidances,
                             avoidanceIDs,
                             requiredSources,
                             flags,
                             expectedPathLength,
                             expectedPath);


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

                if (entityExceptionType == null)
                {
                    Fail("Unexpectedly failed finding an entity path: "
                         + testData + ", " + description, e);

                }
                else if (entityExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        entityExceptionType, e,
                        "FindPath() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityPathDefaultParameters))]
    public void TestFindPathByRecordIDDefaults(
        (string dataSourceCode, string recordID) startRecordKey,
        (string dataSourceCode, string recordID) endRecordKey,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag _)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string startDataSourceCode = startRecordKey.dataSourceCode;
                string startRecordID = startRecordKey.recordID;

                string endDataSourceCode = endRecordKey.dataSourceCode;
                string endRecordID = endRecordKey.recordID;

                string defaultResult = engine.FindPath(startDataSourceCode,
                                                       startRecordID,
                                                       endDataSourceCode,
                                                       endRecordID,
                                                       maxDegrees,
                                                       avoidances,
                                                       requiredSources);

                string explicitResult = engine.FindPath(startDataSourceCode,
                                                        startRecordID,
                                                        endDataSourceCode,
                                                        endRecordID,
                                                        maxDegrees,
                                                        avoidances,
                                                        requiredSources,
                                                        SzFindPathDefaultFlags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string avoidanceJson = SzCoreEngine.EncodeRecordKeys(avoidances);

                string sourcesJson = SzCoreEngine.EncodeDataSources(requiredSources);

                string nativeResult;
                long returnCode;
                if (avoidances == null && requiredSources == null)
                {
                    returnCode = nativeEngine.FindPathByRecordID(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        out nativeResult);
                }
                else if (requiredSources == null)
                {
                    returnCode = nativeEngine.FindPathByRecordIDWithAvoids(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        avoidanceJson,
                        out nativeResult);
                }
                else
                {
                    returnCode = nativeEngine.FindPathByRecordIDIncludingSource(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        avoidanceJson,
                        sourcesJson,
                        out nativeResult);
                }

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
                Fail("Unexpectedly failed to find path.  startRecord=[ "
                     + startRecordKey + " ], endRecordKey=[ " + endRecordKey
                     + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
                     + (avoidances == null ? null : avoidances.ToDebugString())
                     + " ], requiredSources=[ "
                     + (requiredSources == null ? null : requiredSources.ToDebugString())
                     + " ]", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityPathDefaultParameters))]
    public void TestFindPathByEntityIDDefaults(
        (string dataSourceCode, string recordID) startRecordKey,
        (string dataSourceCode, string recordID) endRecordKey,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag _)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                long startEntityID = GetEntityID(startRecordKey);
                long endEntityID = GetEntityID(endRecordKey);

                ISet<long>? avoidanceIDs = avoidanceIDsFunc != null
                    ? avoidanceIDsFunc(this) : null;

                string defaultResult = engine.FindPath(startEntityID,
                                                       endEntityID,
                                                       maxDegrees,
                                                       avoidanceIDs,
                                                       requiredSources);

                string explicitResult = engine.FindPath(startEntityID,
                                                        endEntityID,
                                                        maxDegrees,
                                                        avoidanceIDs,
                                                        requiredSources,
                                                        SzFindPathDefaultFlags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string avoidanceJson = SzCoreEngine.EncodeEntityIDs(avoidanceIDs);

                string sourcesJson = SzCoreEngine.EncodeDataSources(requiredSources);

                string nativeResult;
                long returnCode;
                if (avoidances == null && requiredSources == null)
                {
                    returnCode = nativeEngine.FindPathByEntityID(
                        startEntityID,
                        endEntityID,
                        maxDegrees,
                        out nativeResult);
                }
                else if (requiredSources == null)
                {
                    returnCode = nativeEngine.FindPathByEntityIDWithAvoids(
                        startEntityID,
                        endEntityID,
                        maxDegrees,
                        avoidanceJson,
                        out nativeResult);
                }
                else
                {
                    returnCode = nativeEngine.FindPathByEntityIDIncludingSource(
                        startEntityID,
                        endEntityID,
                        maxDegrees,
                        avoidanceJson,
                        sourcesJson,
                        out nativeResult);
                }

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
                Fail("Unexpectedly failed to find path.  startRecord=[ "
                     + startRecordKey + " ], endRecordKey=[ " + endRecordKey
                     + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
                     + (avoidances == null ? null : avoidances.ToDebugString())
                     + " ], requiredSources=[ "
                     + (requiredSources == null ? null : requiredSources.ToDebugString())
                     + " ]", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityPathDefaultParameters))]
    public void TestFindPathByRecordIDSimple(
        (string dataSourceCode, string recordID) startRecordKey,
        (string dataSourceCode, string recordID) endRecordKey,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag flags)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string startDataSourceCode = startRecordKey.dataSourceCode;
                string startRecordID = startRecordKey.recordID;

                string endDataSourceCode = endRecordKey.dataSourceCode;
                string endRecordID = endRecordKey.recordID;

                string simpleResult = engine.FindPath(startDataSourceCode,
                                                      startRecordID,
                                                      endDataSourceCode,
                                                      endRecordID,
                                                      maxDegrees,
                                                      flags: flags);

                string explicitResult = engine.FindPath(startDataSourceCode,
                                                        startRecordID,
                                                        endDataSourceCode,
                                                        endRecordID,
                                                        maxDegrees,
                                                        null,
                                                        null,
                                                        flags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string avoidanceJson = SzCoreEngine.EncodeRecordKeys(avoidances);

                string sourcesJson = SzCoreEngine.EncodeDataSources(requiredSources);

                string nativeResult;
                long returnCode = nativeEngine.FindPathByRecordID(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        (long)flags,
                        out nativeResult);

                if (returnCode != 0)
                {
                    Fail("Errant return code from native function: " +
                         engine.GetNativeApi().GetLastExceptionCode()
                         + " / " + engine.GetNativeApi().GetLastException());
                }

                Assert.That(simpleResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(simpleResult, Is.EqualTo(nativeResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the native function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed to find path.  startRecord=[ "
                     + startRecordKey + " ], endRecordKey=[ " + endRecordKey
                     + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
                     + (avoidances == null ? null : avoidances.ToDebugString())
                     + " ], requiredSources=[ "
                     + (requiredSources == null ? null : requiredSources.ToDebugString())
                     + " ]", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityPathDefaultParameters))]
    public void TestFindPathByEntityIDSimple(
        (string dataSourceCode, string recordID) startRecordKey,
        (string dataSourceCode, string recordID) endRecordKey,
        int maxDegrees,
        ISet<(string, string)>? avoidances,
        Func<SzCoreEngineGraphTest, ISet<long>?>? avoidanceIDsFunc,
        ISet<string>? requiredSources,
        SzFlag flags)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                long startEntityID = GetEntityID(startRecordKey);
                long endEntityID = GetEntityID(endRecordKey);

                string simpleResult = engine.FindPath(startEntityID,
                                                      endEntityID,
                                                      maxDegrees,
                                                      flags: flags);

                string explicitResult = engine.FindPath(startEntityID,
                                                        endEntityID,
                                                        maxDegrees,
                                                        null,
                                                        null,
                                                        flags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string avoidanceJson = SzCoreEngine.EncodeRecordKeys(avoidances);

                string sourcesJson = SzCoreEngine.EncodeDataSources(requiredSources);

                string nativeResult;
                long returnCode = nativeEngine.FindPathByEntityID(
                        startEntityID,
                        endEntityID,
                        maxDegrees,
                        (long)flags,
                        out nativeResult);

                if (returnCode != 0)
                {
                    Fail("Errant return code from native function: " +
                         engine.GetNativeApi().GetLastExceptionCode()
                         + " / " + engine.GetNativeApi().GetLastException());
                }

                Assert.That(simpleResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(simpleResult, Is.EqualTo(nativeResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the native function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed to find path.  startRecord=[ "
                     + startRecordKey + " ], endRecordKey=[ " + endRecordKey
                     + " ], maxDegrees=[ " + maxDegrees + " ], avoidances=[ "
                     + (avoidances == null ? null : avoidances.ToDebugString())
                     + " ], requiredSources=[ "
                     + (requiredSources == null ? null : requiredSources.ToDebugString())
                     + " ]", e);
            }
        });
    }

    public static List<object?[]> GetEntityNetworkParameters()
    {
        Iterator<SzFlag?> flagSetIter = GetCircularIterator(FindNetworkFlagSet);

        List<object?[]> result = new List<object?[]>();

        IList<IList<(string, string)?>> NoPaths
            = ListOf<IList<(string, string)?>>().AsReadOnly();

        ISet<(string, string)> NoEntities
            = ImmutableHashSet<(string, string)>.Empty;

        Type UnknownSource = typeof(SzUnknownDataSourceException);
        Type NotFound = typeof(SzNotFoundException);

        result.Add(new object?[] {
            "Single entity network",
            SortedSetOf(PassengerABC123),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(PassengerABC123)),
            1, 0, 10, flagSetIter.Next(), null, null, 0,
            NoPaths, SortedSetOf(PassengerABC123)});

        result.Add(new object?[] {
            "Single entity with one-degree build-out",
            SortedSetOf(PassengerABC123),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(PassengerABC123)),
            1, 1, 1000, flagSetIter.Next(), null, null, 0,
            NoPaths, SortedSetOf(PassengerABC123, PassengerDEF456, EmployeeMNO345)});

        result.Add(new object?[] {
            "Two entities with no path",
            SortedSetOf(PassengerABC123,VipJKL456),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(PassengerABC123,VipJKL456)),
            1, 0, 10, flagSetIter.Next(), null, null, 1,
            ListOf<IList<(string,string)?>>(
                ListOf<(string,string)?>(null, PassengerABC123, VipJKL456)),
            SortedSetOf(PassengerABC123, VipJKL456)});

        result.Add(new object?[] {
            "Two entities at three degrees",
            SortedSetOf(PassengerABC123,VipJKL456),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(PassengerABC123,VipJKL456)),
            3, 0, 10, flagSetIter.Next(), null, null, 1,
            ListOf<IList<(string,string)?>>(
                ListOf<(string,string)?>(PassengerABC123, EmployeeMNO345, EmployeeDEF890, VipJKL456)),
            SortedSetOf(PassengerABC123, EmployeeMNO345, EmployeeDEF890, VipJKL456)});

        result.Add(new object?[] {
            "Three entities at three degrees with no build-out",
            SortedSetOf(PassengerABC123,EmployeeABC567,VipJKL456),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(PassengerABC123,EmployeeABC567,VipJKL456)),
            3, 0, 10, flagSetIter.Next(), null, null, 3,
            ListOf<IList<(string,string)?>>(
                ListOf<(string,string)?>(PassengerABC123,EmployeeMNO345,EmployeeDEF890,VipJKL456),
                ListOf<(string,string)?>(PassengerABC123,PassengerDEF456,EmployeePQR678,EmployeeABC567),
                ListOf<(string,string)?>(null, EmployeeABC567, VipJKL456)),
            SortedSetOf(PassengerABC123,EmployeeMNO345,EmployeeDEF890,VipJKL456,
                PassengerDEF456,EmployeePQR678,EmployeeABC567)});

        result.Add(new object?[] {
            "Three entities at zero degrees with single buid-out",
            SortedSetOf(EmployeeABC567,VipGHI123,EmployeeMNO345),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(EmployeeABC567,VipGHI123,EmployeeMNO345)),
            0, 1, 10, flagSetIter.Next(), null, null, 3,
            ListOf<IList<(string,string)?>>(
                 ListOf<(string,string)?>(null, EmployeeABC567, VipGHI123),
                 ListOf<(string,string)?>(null, EmployeeABC567, EmployeeMNO345),
                 ListOf<(string,string)?>(null, VipGHI123, EmployeeMNO345)),
            SortedSetOf(EmployeeABC567,VipGHI123,EmployeeMNO345,EmployeePQR678,
                VipXYZ234,VipSTU901,PassengerABC123,EmployeeDEF890)});

        result.Add(new object?[] {
            "Two entities at zero degrees with single build-out",
            SortedSetOf(PassengerABC123, PassengerDEF456),
            (SzCoreEngineGraphTest t) => t.EntityIDSet(SortedSetOf(PassengerABC123, PassengerDEF456)), 0, 1, 10,
            flagSetIter.Next(), null, null, 1,
            ListOf<IList<(string,string)?>>(
                ListOf<(string,string)?>(null, PassengerABC123, PassengerDEF456)),
            SortedSetOf(PassengerABC123,PassengerDEF456,EmployeeMNO345,
                PassengerGHI789,EmployeePQR678)});

        result.Add(new object?[] {
            "Unknown data source for network entity",
            SortedSetOf((UnknownDataSource,"ABC123"), VipXYZ234),
            (SzCoreEngineGraphTest t) => SortedSetOf(100000000L, t.GetEntityID(VipXYZ234)),
            3, 0, 10, flagSetIter.Next(),
            UnknownSource, NotFound, 0, NoPaths, NoEntities});

        result.Add(new object?[] {
            "Not-found record ID for network entity",
            SortedSetOf(VipXYZ234, (PassengersDataSource,"XXX000")),
            (SzCoreEngineGraphTest t) => SortedSetOf(t.GetEntityID(VipXYZ234), -100L),
            3, 0, 10, flagSetIter.Next(),
            NotFound, NotFound, 0, NoPaths, NoEntities});

        return result;
    }

    public static List<object?[]> GetEntityNetworkDefaultParameters()
    {
        List<object?[]> argsList = GetEntityNetworkParameters();

        List<object?[]> result = new List<object?[]>(argsList.Count);

        for (int index = 0; index < argsList.Count; index++)
        {
            object?[] args = argsList[index];

            // skip the ones that expect an exception
            if (args[args.Length - 4] != null) continue;
            if (args[args.Length - 5] != null) continue;
            result.Add(new object?[] { args[1], args[3], args[4], args[5] });
        }
        return result;
    }

    public void ValidateNetwork(string networkJson,
                                string testData,
                                ISet<(string, string)> recordKeys,
                                int maxDegrees,
                                int buildOutDegrees,
                                int buildOutMaxEntities,
                                SzFlag? flags,
                                int expectedPathCount,
                                IList<IList<(string, string)?>> expectedPaths)
    {
        JsonObject? jsonObject = null;
        try
        {
            jsonObject = JsonNode.Parse(networkJson)?.AsObject();
            if (jsonObject == null) throw new JsonException();

        }
        catch (Exception e)
        {
            Fail("Unable to parse find-network result as JSON: " + networkJson, e);
        }

        JsonArray? entityPaths = jsonObject?["ENTITY_PATHS"]?.AsArray();

        Assert.IsNotNull(entityPaths, "Entity paths are missing: " + testData);

        Assert.IsNotNull(entityPaths, "Entity paths is missing: path=[ "
                         + networkJson + " ], " + testData);

        Assert.That(entityPaths.Count, Is.EqualTo(expectedPathCount),
                     "Paths array has unexpected length: paths=[ " + entityPaths
                     + " ], " + testData);

        ISet<long> allEntityIDs = new SortedSet<long>();

        IDictionary<string, List<long>> actualPaths = new Dictionary<string, List<long>>();
        int entityPathCount = entityPaths?.Count ?? 0;

        for (int index = 0; index < entityPathCount; index++)
        {
            JsonObject? path = entityPaths?[index]?.AsObject();

            Assert.IsNotNull(path, "Entity path was null: paths=[ "
                             + entityPaths + " ], " + testData);

            long? jsonID = path?["START_ENTITY_ID"]?.GetValue<long>();

            Assert.IsNotNull(jsonID,
                             "Starting entity ID on path should not be null."
                             + "  path=[ " + path + " ], " + testData);

            long startEntityID = jsonID ?? 0L; // cannot be null

            jsonID = path?["END_ENTITY_ID"]?.GetValue<long>();
            Assert.IsNotNull(jsonID,
                             "Ending entity ID on path should not be null."
                            + "  path=[ " + path + " ], " + testData);

            long endEntityID = jsonID ?? 0L; // cannot be null

            JsonArray? entityIDs = path?["ENTITIES"]?.AsArray();
            int entityIDCount = entityIDs?.Count ?? 0;
            List<long> pathIDs = new List<long>(entityIDCount);
            for (int index2 = 0; index2 < entityIDCount; index2++)
            {
                long? entityID = entityIDs?[index2]?.GetValue<long>();

                Assert.IsNotNull(entityID, "The discovered entity ID was null.  "
                                 + entityIDs + "" + testData);
                pathIDs.Add(entityID ?? 0L);
                allEntityIDs.Add(entityID ?? 0L);
            }

            long minEntityID = Math.Min(startEntityID, endEntityID);
            long maxEntityID = Math.Max(startEntityID, endEntityID);
            if (minEntityID != startEntityID)
            {
                pathIDs.Reverse();
            }
            string key = minEntityID + ":" + maxEntityID;
            actualPaths.Add(key, pathIDs);
        }

        JsonArray? pathLinks = jsonObject?["ENTITY_NETWORK_LINKS"]?.AsArray();
        if (flags != null && ((flags & SzFindNetworkIncludeMatchingInfo) != SzNoFlags))
        {
            Assert.IsNotNull(pathLinks, "Entity path links missing or null: "
                             + "pathLinks=[ " + pathLinks + " ], " + testData);
        }
        else
        {
            Assert.IsNull(pathLinks, "Entity path links present when not requested.  "
                          + "pathLinks=[ " + pathLinks + " ], " + testData);
        }

        JsonArray? entities = jsonObject?["ENTITIES"]?.AsArray();

        Assert.IsNotNull(entities,
                         "Entity details for network are missing: " + testData);

        int entitiesCount = entities?.Count ?? 0;

        // validate the entity count
        int minEntityCount = allEntityIDs.Count;
        int maxEntityCount = minEntityCount + buildOutMaxEntities;
        Assert.IsTrue(entitiesCount >= minEntityCount,
                      "Too few entity details provided -- expected at least "
                      + minEntityCount + ", but got only " + entitiesCount
                      + ": " + testData);
        Assert.IsTrue(entitiesCount <= maxEntityCount,
                      "Too many entity details provided -- expected at most "
                      + maxEntityCount + ", but got " + entitiesCount
                      + ": " + testData);


        // handle the expected paths
        IDictionary<string, List<long>> expectedPathMap = new Dictionary<string, List<long>>();
        IDictionary<IList<(string, string)?>, string> expectedLookup
            = new Dictionary<IList<(string, string)?>, string>(ReferenceEqualityComparer.Instance);

        ISet<long> expectedEntityIDs = new HashSet<long>();
        foreach (IList<(string, string)?> expectedPath in expectedPaths)
        {
            if (expectedPath[0] == null)
            {
                (string code, string id) startKey = expectedPath[1] ?? ("A", "B");
                (string code, string id) endKey = expectedPath[2] ?? ("C", "D");
                long startId = this.GetEntityID(startKey);
                long endId = this.GetEntityID(endKey);

                long minId = Math.Min(startId, endId);
                long maxId = Math.Max(startId, endId);

                string key = minId + ":" + maxId;
                expectedPathMap.Add(key, ListOf<long>());
                expectedLookup.Add(expectedPath, key);

                expectedEntityIDs.Add(startId);
                expectedEntityIDs.Add(endId);

            }
            else
            {
                List<long> entityIDs = this.GetEntityIDs(expectedPath);
                long startID = entityIDs[0];
                long endID = entityIDs[entityIDs.Count - 1];
                long minID = Math.Min(startID, endID);
                long maxID = Math.Max(startID, endID);

                if (minID != startID)
                {
                    entityIDs.Reverse();
                }

                string key = minID + ":" + maxID;
                expectedPathMap.Add(key, entityIDs);
                expectedLookup.Add(expectedPath, key);
                foreach (long entityID in entityIDs)
                {
                    expectedEntityIDs.Add(entityID);
                }
            }
        }

        foreach (KeyValuePair<IList<(string, string)?>, string> pair in expectedLookup)
        {
            IList<(string, string)?> expectedPath = pair.Key;
            string key = pair.Value;

            List<long> expectedIDs = expectedPathMap[key];
            List<long> actualIDs = actualPaths[key];

            Assert.IsNotNull(actualIDs,
                             "Did not find actual path for expected path.  "
                             + "expected=[ " + expectedPath + " ], key=[ "
                             + key + " ], expectedEntityIDs=[ "
                             + expectedIDs + " ]");

            Assert.That(actualIDs, Is.EqualTo(expectedIDs),
                        "Path between entities is not as expected.  expected=[ "
                        + expectedPath + " ], key=[ " + key + " ], actualPaths=[ "
                        + actualPaths + " ], " + testData);
        }

        foreach (KeyValuePair<string, List<long>> pair in actualPaths)
        {
            string key = pair.Key;
            List<long> actualIDs = pair.Value;

            Assert.IsTrue(expectedPathMap.ContainsKey(key),
                          "Actual path provided that was not expected."
                          + "  path=[ " + actualIDs + " ], key=[ " + key + " ]");
        }

        // check that the entities we found are on the path
        ISet<long> detailEntityIDs = new HashSet<long>();
        for (int index = 0; index < entitiesCount; index++)
        {
            JsonObject? entity = entities?[index]?.AsObject();

            Assert.IsNotNull(entity, "Entity detail from network was null: "
                             + entities + ", " + testData);

            entity = entity?["RESOLVED_ENTITY"]?.AsObject();

            Assert.IsNotNull(entity, "Resolved entity from network was null: "
                             + entities + ", " + testData);

            // Get the entity ID
            long? id = entity?["ENTITY_ID"]?.GetValue<long>();

            Assert.IsNotNull(
                id, "The entity detail from path was missing or "
                + "null ENTITY_ID: " + entity + ", " + testData);

            // add to the ID set
            detailEntityIDs.Add(id ?? 0L); // id cannot be null
        }

        foreach (long entityID in expectedEntityIDs)
        {
            Assert.IsTrue(detailEntityIDs.Contains(entityID),
                "Entity ID from network not found in entity details.  "
                + "entityID=[ " + entityID + " ], " + testData);
        }
    }

    [Test, TestCaseSource(nameof(GetEntityNetworkParameters))]
    public void TestFindNetworkByEntityID(
        string testDescription,
        ISet<(string, string)> recordKeys,
        Func<SzCoreEngineGraphTest, ISet<long>> entityIDsFunc,
        int maxDegrees,
        int buildOutDegrees,
        int buildOutMaxEntities,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType,
        int expectedPathCount,
        IList<IList<(string, string)?>> expectedPaths,
        ISet<(string, string)> expectedEntities)
    {
        ISet<long> entityIDs = entityIDsFunc(this);

        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], recordKeys=[ "
            + recordKeys + " ], entityIDs=[ " + entityIDs
            + " ], maxDegrees=[ " + maxDegrees + " ], buildOutDegrees=[ "
            + buildOutDegrees + " ], buildOutMaxEntities=[ "
            + buildOutMaxEntities + " ], flags=[ "
            + SzFindNetworkFlags.FlagsToString(flags)
            + " ], expectedException=[ " + entityExceptionType
            + " ], expectedPathCount=[ " + expectedPathCount
            + " ], expectedPaths=[ ");

        string prefix1 = "";
        foreach (IList<(string, string)?> expectedPath in expectedPaths)
        {
            sb.Append(prefix1);
            if (expectedPath[0] == null)
            {
                (string code, string id) startKey = expectedPath[1] ?? ("A", "B");
                (string code, string id) endKey = expectedPath[2] ?? ("C", "D");
                sb.Append("{ NO PATH BETWEEN ").Append(startKey).Append(" (");
                sb.Append(this.GetEntityID(startKey)).Append(") AND ");
                sb.Append(endKey).Append(" (").Append(this.GetEntityID(endKey)).Append(") }");
            }
            else
            {
                string prefix2 = "";
                sb.Append("{ ");
                foreach ((string code, string id)? key in expectedPath)
                {
                    sb.Append(prefix2).Append(key).Append(" (");
                    sb.Append(key == null ? null : this.GetEntityID(key ?? ("A", "B")));
                    sb.Append(')');
                    prefix2 = ", ";
                }
                sb.Append('}');
            }
            prefix1 = ", ";
        }
        sb.Append(" ], recordMap=[ ").Append(LoadedRecordMap.ToDebugString()).Append(" ]");
        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindNetwork(
                    entityIDs,
                    maxDegrees,
                    buildOutDegrees,
                    buildOutMaxEntities,
                    flags);

                if (entityExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in finding a path: " + testData);
                }

                this.ValidateNetwork(result,
                                     testData,
                                     recordKeys,
                                     maxDegrees,
                                     buildOutDegrees,
                                     buildOutMaxEntities,
                                     flags,
                                     expectedPathCount,
                                     expectedPaths);

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

                if (entityExceptionType == null)
                {
                    Fail("Unexpectedly failed finding an entity network: "
                         + testData + ", " + description, e);

                }
                else if (entityExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        entityExceptionType, e,
                        "FindNetwork() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityNetworkParameters))]
    public void TestFindNetworkByRecordId(
        string testDescription,
        ISet<(string, string)> recordKeys,
        Func<SzCoreEngineGraphTest, ISet<long>> entityIDsFunc,
        int maxDegrees,
        int buildOutDegrees,
        int buildOutMaxEntities,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType,
        int expectedPathCount,
        IList<IList<(string, string)?>> expectedPaths,
        ISet<(string, string)> expectedEntities)
    {
        ISet<long> entityIDs = entityIDsFunc(this);

        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], recordKeys=[ "
            + recordKeys + " ], entityIDs=[ " + entityIDs
            + " ], maxDegrees=[ " + maxDegrees + " ], buildOutDegrees=[ "
            + buildOutDegrees + " ], buildOutMaxEntities=[ "
            + buildOutMaxEntities + " ], flags=[ "
            + SzFindNetworkFlags.FlagsToString(flags)
            + " ], expectedException=[ " + recordExceptionType
            + " ], expectedPathCount=[ " + expectedPathCount
            + " ], expectedPaths=[ ");

        string prefix1 = "";
        foreach (IList<(string, string)?> expectedPath in expectedPaths)
        {
            sb.Append(prefix1);
            if (expectedPath[0] == null)
            {
                (string code, string id) startKey = expectedPath[1] ?? ("A", "B");
                (string code, string id) endKey = expectedPath[2] ?? ("C", "D");
                sb.Append("{ NO PATH BETWEEN ").Append(startKey).Append(" (");
                sb.Append(this.GetEntityID(startKey)).Append(") AND ");
                sb.Append(endKey).Append(" (").Append(this.GetEntityID(endKey)).Append(") }");
            }
            else
            {
                string prefix2 = "";
                sb.Append("{ ");
                foreach ((string code, string id)? key in expectedPath)
                {
                    sb.Append(prefix2).Append(key).Append(" (");
                    sb.Append(key == null ? 0L : this.GetEntityID(key ?? ("A", "B")));
                    sb.Append(')');
                    prefix2 = ", ";
                }
                sb.Append('}');
            }
            prefix1 = ", ";
        }
        sb.Append(" ], recordMap=[ ").Append(LoadedRecordMap).Append(" ]");
        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindNetwork(
                    recordKeys,
                    maxDegrees,
                    buildOutDegrees,
                    buildOutMaxEntities,
                    flags);

                if (recordExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in finding a path: " + testData);
                }

                this.ValidateNetwork(result,
                                     testData,
                                     recordKeys,
                                     maxDegrees,
                                     buildOutDegrees,
                                     buildOutMaxEntities,
                                     flags,
                                     expectedPathCount,
                                     expectedPaths);

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

                if (recordExceptionType == null)
                {
                    Fail("Unexpectedly failed finding an entity network: "
                         + testData + ", " + description, e);

                }
                else if (recordExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        recordExceptionType, e,
                        "FindNetwork() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEntityNetworkDefaultParameters))]
    public void TestFindNetworkByRecordIDDefaults(
        ISet<(string, string)> recordKeys,
        int maxDegrees,
        int buildOutDegrees,
        int buildOutMaxEntities)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string defaultResult = engine.FindNetwork(recordKeys,
                                                          maxDegrees,
                                                          buildOutDegrees,
                                                          buildOutMaxEntities);

                string explicitResult = engine.FindNetwork(recordKeys,
                                                           maxDegrees,
                                                           buildOutDegrees,
                                                           buildOutMaxEntities,
                                                           SzFindNetworkDefaultFlags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string recordsJson = SzCoreEngine.EncodeRecordKeys(recordKeys);

                long returnCode = nativeEngine.FindNetworkByRecordID(recordsJson,
                                                                     maxDegrees,
                                                                     buildOutDegrees,
                                                                     buildOutMaxEntities,
                                                                     out string nativeResult);

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

    [Test, TestCaseSource(nameof(GetEntityNetworkDefaultParameters))]
    public void TestFindNetworkByEntityIDDefaults(
        ISet<(string, string)> recordKeys,
        int maxDegrees,
        int buildOutDegrees,
        int buildOutMaxEntities)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                ISet<long> entityIDs = new SortedSet<long>(GetEntityIDs(recordKeys));

                string defaultResult = engine.FindNetwork(entityIDs,
                                                          maxDegrees,
                                                          buildOutDegrees,
                                                          buildOutMaxEntities);

                string explicitResult = engine.FindNetwork(entityIDs,
                                                           maxDegrees,
                                                           buildOutDegrees,
                                                           buildOutMaxEntities,
                                                           SzFindNetworkDefaultFlags);

                NativeEngine nativeEngine = engine.GetNativeApi();

                string entitiesJson = SzCoreEngine.EncodeEntityIDs(entityIDs);

                long returnCode = nativeEngine.FindNetworkByEntityID(entitiesJson,
                                                                     maxDegrees,
                                                                     buildOutDegrees,
                                                                     buildOutMaxEntities,
                                                                     out string nativeResult);

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

}
