namespace Senzing.Sdk.Tests.Core;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


using NUnit.Framework;
using NUnit.Framework.Internal;

using Senzing.Sdk;
using Senzing.Sdk.Core;

using static System.StringComparison;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;
using static Senzing.Sdk.SzFlagUsageGroup;
using static Senzing.Sdk.Tests.Util.TextUtilities;
using static Senzing.Sdk.Utilities;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreEngineReadTest : AbstractTest
{
    private const string PassengersDataSource = "PASSENGERS";
    private const string EmployeesDataSource = "EMPLOYEES";
    private const string VipsDataSource = "VIPS";
    private const string MarriagesDataSource = "MARRIAGES";
    private const string UnknownDataSource = "UNKNOWN";

    private static readonly (string dataSourceCode, string recordID) ABC123
        = (PassengersDataSource, "ABC123");
    private static readonly (string dataSourceCode, string recordID) DEF456
        = (PassengersDataSource, "DEF456");
    private static readonly (string dataSourceCode, string recordID) GHI789
        = (PassengersDataSource, "GHI789");
    private static readonly (string dataSourceCode, string recordID) JKL012
        = (PassengersDataSource, "JKL012");
    private static readonly (string dataSourceCode, string recordID) MNO345
        = (EmployeesDataSource, "MNO345");
    private static readonly (string dataSourceCode, string recordID) PQR678
        = (EmployeesDataSource, "PQR678");
    private static readonly (string dataSourceCode, string recordID) STU901
        = (VipsDataSource, "STU901");
    private static readonly (string dataSourceCode, string recordID) XYZ234
        = (VipsDataSource, "XYZ234");
    private static readonly (string dataSourceCode, string recordID) ZYX321
        = (EmployeesDataSource, "ZYX321");
    private static readonly (string dataSourceCode, string recordID) CBA654
        = (EmployeesDataSource, "CBA654");

    private static readonly (string dataSourceCode, string recordID) BCD123
        = (MarriagesDataSource, "BCD123");
    private static readonly (string dataSourceCode, string recordID) CDE456
        = (MarriagesDataSource, "CDE456");
    private static readonly (string dataSourceCode, string recordID) EFG789
        = (MarriagesDataSource, "EFG789");
    private static readonly (string dataSourceCode, string recordID) FGH012
        = (MarriagesDataSource, "FGH012");

    private static readonly IList<SzFlag?> EntityFlagSets;
    private static readonly IList<SzFlag?> RecordFlagSets;
    private static readonly IList<SzFlag> SearchIncludeFlags
            = ListOf<SzFlag>(SzSearchIncludePossiblyRelated,
                             SzSearchIncludePossiblySame,
                             SzSearchIncludeResolved).AsReadOnly();

    private static readonly IList<SzFlag?> SearchFlagSets;

    private static readonly IList<SzFlag> ExportIncludeFlags
        = ListOf(SzExportIncludePossiblyRelated,
                 SzExportIncludePossiblySame,
                 SzExportIncludeDisclosed,
                 SzExportIncludeNameOnly,
                 SzExportIncludeMultiRecordEntities,
                 SzExportIncludeSingleRecordEntities).AsReadOnly();

    private static readonly IList<SzFlag?> ExportFlagSets;

    static SzCoreEngineReadTest()
    {
        List<SzFlag?> list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzEntityDefaultFlags);
        list.Add(SzEntityBriefDefaultFlags);
        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzEntityIncludeRecordMatchingInfo);
        list.Add(SzEntityIncludeEntityName);
        EntityFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzRecordDefaultFlags);
        list.Add(SzRecordAllFlags);
        list.Add(SzEntityIncludeRecordMatchingInfo
                 | SzEntityIncludeRecordUnmappedData);
        list.Add(SzEntityIncludeRecordTypes
                 | SzEntityIncludeRecordJsonData);
        RecordFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzSearchByAttributesDefaultFlags);
        list.Add(SzSearchByAttributesMinimalAll);
        list.Add(SzSearchByAttributesMinimalStrong);
        list.Add(SzSearchByAttributesStrong);
        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzSearchIncludeAllEntities);

        ISet<SzFlag> includeSets = new HashSet<SzFlag>();
        ISet<SzFlag> prevSets = SetOf(SzEntityDefaultFlags);
        for (int index = 0; index < SearchIncludeFlags.Count; index++)
        {
            ISet<SzFlag> currentSets = new HashSet<SzFlag>();
            foreach (SzFlag prevFlags in prevSets)
            {
                foreach (SzFlag flag in SearchIncludeFlags)
                {
                    currentSets.Add(prevFlags | flag);
                }
            }
            prevSets = currentSets;
            foreach (SzFlag flags in currentSets)
            {
                includeSets.Add(flags);
            }
            currentSets = new HashSet<SzFlag>();
        }
        foreach (SzFlag flags in includeSets)
        {
            list.Add(flags);
        }
        SearchFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzExportDefaultFlags);
        list.Add(SzExportIncludeAllEntities);
        list.Add(SzExportIncludeAllHavingRelationships);
        list.Add(SzExportAllFlags);

        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzExportIncludeAllEntities);

        includeSets = new HashSet<SzFlag>();
        prevSets = SetOf(SzEntityDefaultFlags);
        for (int index = 0; index < ExportIncludeFlags.Count; index++)
        {
            ISet<SzFlag> currentSets = new HashSet<SzFlag>();
            foreach (SzFlag prevFlags in prevSets)
            {
                foreach (SzFlag flag in ExportIncludeFlags)
                {
                    currentSets.Add(prevFlags | flag);
                }
            }
            prevSets = currentSets;
            foreach (SzFlag flags in currentSets)
            {
                includeSets.Add(flags);
            }
            currentSets = new HashSet<SzFlag>();
        }
        foreach (SzFlag flags in includeSets)
        {
            list.Add(flags);
        }
        ExportFlagSets = list.AsReadOnly();
    }

    private readonly Dictionary<(string, string), long> LoadedRecordMap
        = new Dictionary<(string, string), long>();

    private readonly Dictionary<long, ISet<(string, string)>> LoadedEntityMap
        = new Dictionary<long, ISet<(string, string)>>();

    private static readonly IList<(string, string)> RecordKeys
        = ListOf(ABC123,
                 DEF456,
                 GHI789,
                 JKL012,
                 MNO345,
                 PQR678,
                 STU901,
                 XYZ234,
                 ZYX321,
                 CBA654,
                 BCD123,
                 CDE456,
                 EFG789,
                 FGH012).AsReadOnly();

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

            // get the loaded records and entity ID's
            foreach ((string dataSourceCode, string recordID) key in RecordKeys)
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
                    LoadedEntityMap.Add(entityID, new HashSet<(string, string)>());
                }
                ISet<(string dataSourceCode, string recordID)> recordKeySet
                    = LoadedEntityMap[entityID];
                recordKeySet.Add(key);
            };

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

    /**
     * Overridden to configure some data sources.
     */
    override protected void PrepareRepository()
    {
        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();

        ISet<string> dataSources = SortedSetOf(PassengersDataSource,
                                               EmployeesDataSource,
                                               VipsDataSource,
                                               MarriagesDataSource);

        FileInfo passengerFile = this.PreparePassengerFile();
        FileInfo employeeFile = this.PrepareEmployeeFile();
        FileInfo vipFile = this.PrepareVipFile();
        FileInfo marriagesFile = this.PrepareMarriagesDataSourceFile();

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

        RepositoryManager.LoadFile(repoDirectory,
                                   marriagesFile,
                                   MarriagesDataSource,
                                   true);
    }

    private static string RelationshipKey((string, string) recordKey1,
                                          (string, string) recordKey2)
    {
        string rec1 = recordKey1.ToString();
        string rec2 = recordKey2.ToString();
        if (string.Compare(rec1, rec2, Ordinal) <= 0)
        {
            return rec1 + "|" + rec2;
        }
        else
        {
            return rec2 + "|" + rec1;
        }
    }

    private FileInfo PreparePassengerFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH"};

        String[][] passengers = {
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
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH","MOTHERS_MAIDEN_NAME", "SSN_NUMBER"};

        String[][] employees = {
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
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "PHONE_NUMBER", "ADDR_FULL",
            "DATE_OF_BIRTH","MOTHERS_MAIDEN_NAME"};

        String[][] vips = {
            new string[] {STU901.recordID, "John", "Doe", "818-555-1313",
                "100 Main Street, Los Angeles, CA 90012", "1978-10-17", "GREEN"},
            new string[] {XYZ234.recordID, "Jane", "Doe", "818-555-1212",
                "100 Main Street, Los Angeles, CA 90012", "1979-02-05", "GRAHAM"}
        };

        return this.PrepareJsonFile("test-vips-", headers, vips);
    }

    private FileInfo PrepareMarriagesDataSourceFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FULL", "AKA_NAME_FULL", "PHONE_NUMBER", "ADDR_FULL",
            "MARRIAGE_DATE", "DATE_OF_BIRTH", "GENDER", "RELATIONSHIP_TYPE",
            "RELATIONSHIP_ROLE", "RELATIONSHIP_KEY" };

        String[][] spouses = {
            new string[] {BCD123.recordID, "Bruce Wayne", "Batman", "201-765-3451",
                "101 Wayne Manor Rd; Gotham City, NJ 07017", "2008-06-05",
                "1971-09-08", "M", "SPOUSE", "HUSBAND",
                RelationshipKey(BCD123, CDE456)},
            new string[] {CDE456.recordID, "Selina Kyle", "Catwoman", "201-875-2314",
                "101 Wayne Manor Rd; Gotham City, NJ 07017", "2008-06-05",
                "1981-12-05", "F", "SPOUSE", "WIFE",
                RelationshipKey(BCD123, CDE456)},
            new string[] {EFG789.recordID, "Barry Allen", "The Flash", "330-982-2133",
                "1201 Main Street; Star City, OH 44308", "2014-11-07",
                "1986-03-04", "M", "SPOUSE", "HUSBAND",
                RelationshipKey(EFG789, FGH012)},
            new string[] {FGH012.recordID, "Iris West-Allen", "", "330-675-1231",
                "1201 Main Street; Star City, OH 44308", "2014-11-07",
                "1986-05-14", "F", "SPOUSE", "WIFE",
                RelationshipKey(EFG789, FGH012)}
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

    private static List<object?[]> GetGetEntityParameters()
    {
        List<object?[]> result = new List<object?[]>();
        Iterator<SzFlag?> flagSetIter = GetCircularIterator(EntityFlagSets);


        Type UnknownSource = typeof(SzUnknownDataSourceException);
        Type NotFound = typeof(SzNotFoundException);

        foreach ((string, string) recordKey in RecordKeys)
        {
            result.Add(new object?[] {
                "Get entity for " + recordKey + " test",
                recordKey,
                (SzCoreEngineReadTest t) => t.GetEntityID(recordKey),
                flagSetIter.Next(),
                null,
                null});
        }

        result.Add(new object?[] {
            "Get Entity with bad data source code / entity ID test",
            (UnknownDataSource, "ABC123"),
            (SzCoreEngineReadTest t) => -200L,
            flagSetIter.Next(),
            UnknownSource,
            NotFound});

        result.Add(new object?[] {
            "Get Entity with not-found record ID / entity ID test",
            (PassengersDataSource, "XXX000"),
            (SzCoreEngineReadTest t) => 200000000L,
            flagSetIter.Next(),
            NotFound,
            NotFound});

        return result;
    }

    public static List<object?[]> GetExportCsvParameters()
    {
        List<object?[]> result = new List<object?[]>();

        List<string> columnLists = ListOf(
            "", "*", "RESOLVED_ENTITY_ID,RESOLVED_ENTITY_NAME,RELATED_ENTITY_ID");

        Iterator<String> columnListIter = GetCircularIterator(columnLists);

        foreach (SzFlag? flags in ExportFlagSets)
        {
            result.Add(new object?[] { columnListIter.Next(), flags, null });
        }

        IEnumerator<SzFlag?> enumerator = ExportFlagSets.GetEnumerator();
        enumerator.MoveNext();
        SzFlag? flagSet = enumerator.Current;
        result.Add(new object?[] {
            "RESOLVED_ENTITY_ID,BAD_COLUMN_NAME",
            enumerator.Current,
            typeof(SzBadInputException)});

        return result;
    }

    public static List<object?[]> GetExportJsonParameters()
    {
        List<object?[]> result = new List<object?[]>();

        foreach (SzFlag? flagSet in ExportFlagSets)
        {
            result.Add(new object?[] { flagSet, null });
        }

        return result;
    }

    private static string ReadExportFullyAndClose(SzEngine engine, IntPtr exportHandle)
    {
        StringBuilder sb = new StringBuilder();
        for (string? data = engine.FetchNext(exportHandle);
             data != null;
             data = engine.FetchNext(exportHandle))
        {
            sb.AppendLine(data);
        }
        engine.CloseExportReport(exportHandle);
        return sb.ToString();
    }

    [Test, TestCaseSource(nameof(GetExportCsvParameters))]
    public void TestExportCsvEntityReport(string columnList,
                                         SzFlag? flags,
                                         Type? expectedException)
    {
        string testData = "columnList=[ " + columnList + " ], flags=[ "
            + SzExportFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedException + " ]";

        this.PerformTest(() =>
        {
            IntPtr handle = IntPtr.Zero;
            SzEngine engine = this.Env.GetEngine();
            try
            {
                handle = engine.ExportCsvEntityReport(columnList, flags);

                if (expectedException != null)
                {
                    Fail("Export unexpectedly succeeded when exception expected: "
                         + testData);
                }

                StringBuilder sb = new StringBuilder();
                for (string? data = engine.FetchNext(handle);
                     data != null;
                     data = engine.FetchNext(handle))
                {
                    sb.AppendLine(data);
                }

                IntPtr invalidHandle = handle;
                try
                {
                    engine.CloseExportReport(handle);
                }
                finally
                {
                    handle = IntPtr.Zero;
                }
                try
                {
                    // try fetching with an invalid handle
                    engine.FetchNext(invalidHandle);
                    Fail("Succeeded in fetching with an invalid export handle");

                }
                catch (SzException)
                { // TODO(bcaceres): change this to SzBadInputException??
                    // expected
                }
                catch (Exception e)
                {
                    Fail("Fetching with invalid handle failed with unexpected exception.", e);
                }
                try
                {
                    // try closing the handle twice (should succeed)
                    engine.CloseExportReport(invalidHandle);

                    // should not be able to close an invalid handle
                    Fail("Unexpectedly succeeded in closing an invalid export handle.");

                }
                catch (Exception)
                {
                    // expected
                }

                string fullExport = sb.ToString();

                StringReader sr = new StringReader(fullExport);
                string? headerLine = sr.ReadLine();
                IList<string> headerTokens = ParseCSVLine(headerLine ?? "");
                IDictionary<string, int> headerMap = new Dictionary<string, int>();
                IList<string> headers = new List<string>(headerTokens.Count);

                for (int index = 0; index < headerTokens.Count; index++)
                {
                    string header = headerTokens[index].Trim().ToUpperInvariant();
                    headerMap.Add(header, index);
                    headers.Add(header);
                }

                if (columnList.Equals("*", Ordinal) || columnList.Length == 0)
                {
                    Assert.IsTrue(headers.Contains("RESOLVED_ENTITY_ID"),
                        "Default columns exported, but RESOLVED_ENTITY_ID not found "
                        + "in headers (" + headerLine + " / " + headers.ToDebugString()
                        + "): " + testData);

                }
                else
                {
                    string[] columns = columnList.Split(",");
                    foreach (string col in columns)
                    {
                        string column = col.Trim().ToUpperInvariant();
                        Assert.IsTrue(headers.Contains(column),
                            "Specified column (" + column + ") was not found "
                            + "in columns (" + headerLine + " / " + headers.ToDebugString()
                            + "): " + testData);
                    }
                }

                for (string? line = sr.ReadLine(); line != null; line = sr.ReadLine())
                {
                    if (line.Length == 0) continue;
                    IList<string> lineTokens = ParseCSVLine(line);
                    Assert.That(lineTokens.Count, Is.EqualTo(headers.Count),
                        "Read CSV line with wrong number of columns: headers=[ "
                        + headers.ToDebugString() + " ], line=[ " + line + " ]");
                }

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

                if (expectedException == null)
                {
                    Fail("Unexpectedly failed exporting CSV entities: "
                         + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        expectedException, e,
                        "Export CSV failed with an unexpected exception type: "
                        + description);
                }


            }
            finally
            {
                if (handle != IntPtr.Zero && engine != null)
                {
                    try
                    {
                        engine.CloseExportReport(handle);
                    }
                    catch (SzException e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }
        });
    }

    [Test]
    [TestCase("")]
    [TestCase("*")]
    [TestCase("RESOLVED_ENTITY_ID,RESOLVED_ENTITY_NAME,RELATED_ENTITY_ID")]
    public void TestExportCsvDefaults(string columnList)
    {
        this.PerformTest(() =>
        {
            IntPtr exportHandle = IntPtr.Zero;
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                exportHandle = engine.ExportCsvEntityReport(columnList);
                string defaultResult = ReadExportFullyAndClose(engine, exportHandle);
                exportHandle = IntPtr.Zero;

                exportHandle = engine.ExportCsvEntityReport(columnList, SzExportDefaultFlags);
                string explicitResult = ReadExportFullyAndClose(engine, exportHandle);
                exportHandle = IntPtr.Zero;

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting entity by record", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetExportJsonParameters))]
    public void TestExportJsonEntityReport(SzFlag? flags,
                                           Type? expectedException)
    {
        string testData = "flags=[ " + SzExportFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedException + " ]";

        this.PerformTest(() =>
        {
            IntPtr handle = IntPtr.Zero;
            SzEngine engine = this.Env.GetEngine();
            try
            {
                handle = engine.ExportJsonEntityReport(flags);

                if (expectedException != null)
                {
                    Fail("Export unexpectedly succeeded when exception expected: "
                        + testData);
                }

                StringBuilder sb = new StringBuilder();
                for (string? data = engine.FetchNext(handle);
                     data != null;
                     data = engine.FetchNext(handle))
                {
                    sb.AppendLine(data);
                }

                IntPtr invalidHandle = handle;
                try
                {
                    engine.CloseExportReport(handle);
                }
                finally
                {
                    handle = IntPtr.Zero;
                }
                try
                {
                    // try fetching with an invalid handle
                    engine.FetchNext(invalidHandle);
                    Fail("Succeeded in fetching with an invalid export handle");

                }
                catch (SzException)
                { // TODO(bcaceres): change this to SzBadInputException??
                    // expected
                }
                catch (Exception e)
                {
                    Fail("Fetching with invalid handle failed with unexpected exception.", e);
                }

                try
                {
                    // try closing the handle twice (should succeed)
                    engine.CloseExportReport(invalidHandle);

                }
                catch (Exception)
                {
                    // TODO(bcaceres): This should have succeeded but currently fails
                    // Fail("Failed to close an export handle more than once.", e);
                }

                string fullExport = sb.ToString();

                // ensure we can parse the JSON
                StringReader sr = new StringReader(fullExport);
                for (string? line = sr.ReadLine();
                     line != null;
                     line = sr.ReadLine())
                {

                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }

                    JsonObject? jsonObj = JsonNode.Parse(line)?.AsObject();
                    Assert.IsNotNull(jsonObj,
                        "Export line did NOT parse as JSON: " + testData
                        + ", line=[ " + line + " ]");
                }

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

                if (expectedException == null)
                {
                    Fail("Unexpectedly failed exporting JSON entities: "
                         + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        expectedException, e,
                        "Export JSON failed with an unexpected exception type: "
                        + description);
                }


            }
            finally
            {
                if (handle != IntPtr.Zero && engine != null)
                {
                    try
                    {
                        engine.CloseExportReport(handle);

                    }
                    catch (SzException e)
                    {
                        Console.Error.WriteLine(e);
                    }
                }
            }
        });
    }

    [Test]
    public void TestExportJsonDefaults()
    {
        this.PerformTest(() =>
        {
            IntPtr exportHandle = IntPtr.Zero;
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                exportHandle = engine.ExportJsonEntityReport();
                string defaultResult = ReadExportFullyAndClose(engine, exportHandle);
                exportHandle = IntPtr.Zero;

                exportHandle = engine.ExportJsonEntityReport(SzExportDefaultFlags);
                string explicitResult = ReadExportFullyAndClose(engine, exportHandle);
                exportHandle = IntPtr.Zero;

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");
            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting entity by record", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestGetEntityByRecordIDDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string dataSourceCode = recordKey.dataSourceCode;
                string recordID = recordKey.recordID;

                string defaultResult = engine.GetEntity(dataSourceCode, recordID);

                string explicitResult = engine.GetEntity(
                    dataSourceCode, recordID, SzEntityDefaultFlags);

                long returnCode = engine.GetNativeApi().GetEntityByRecordID(
                    dataSourceCode, recordID, out string nativeResult);

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

    [Test, TestCaseSource(nameof(GetGetEntityParameters))]
    public void TestGetEntityByRecordID(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        Func<SzCoreEngineReadTest, long> entityIDFunc,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType)
    {
        long entityID = entityIDFunc(this);

        string testData = "description=[ " + testDescription
            + " ], recordKey=[ " + recordKey + " ], entityID=[ "
            + entityID + " ], flags=[ " + SzEntityFlags.FlagsToString(flags)
            + " ], expectedExceptionType=[ " + recordExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.GetEntity(
                    recordKey.dataSourceCode, recordKey.recordID, flags);

                if (recordExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in getting an entity: " + testData);
                }

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result)?.AsObject();

                    if (jsonObject == null)
                    {
                        throw new JsonException("Failed to parse as JsonObject");
                    }
                }
                catch (Exception e)
                {
                    Fail("Failed to parse entity result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

                // get the entity
                JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();

                Assert.IsNotNull(entity,
                    "No RESOLVED_ENTITY property in entity JSON: "
                    + testData + ", result=[ " + result + " ]");

                // get the entity ID
                long? actualEntityID = entity?["ENTITY_ID"]?.GetValue<long>();

                Assert.IsNotNull(entity, "No ENTITY_ID property in entity JSON: "
                                 + testData + ", result=[ " + result + " ]");

                Assert.That(actualEntityID, Is.EqualTo(entityID),
                             "Unexpected entity ID: " + testData);

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
                    Fail("Unexpectedly failed getting entity by record: "
                         + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        recordExceptionType, e,
                        "get-entity-by-record failed with an unexpected "
                        + "exception type: " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetGetEntityParameters))]
    public void TestGetEntityByEntityID(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        Func<SzCoreEngineReadTest, long> entityIDFunc,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType)
    {
        long entityID = entityIDFunc(this);

        string testData = "description=[ " + testDescription
            + " ], recordKey=[ " + recordKey + " ], entityID=[ "
            + entityID + " ], flags=[ " + SzEntityFlags.FlagsToString(flags)
            + " ], expectedExceptionType=[ " + entityExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.GetEntity(entityID, flags);

                if (entityExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in getting an entity: " + testData);
                }

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result)?.AsObject();

                }
                catch (Exception e)
                {
                    Fail("Failed to parse entity result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

                // get the entity
                JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();

                Assert.IsNotNull(entity, "No RESOLVED_ENTITY property in entity JSON: "
                              + testData + ", result=[ " + result + " ]");

                // get the entity ID
                long? actualEntityID = entity?["ENTITY_ID"]?.GetValue<long>();

                Assert.IsNotNull(entity, "No ENTITY_ID property in entity JSON: "
                                 + testData + ", result=[ " + result + " ]");

                Assert.That(actualEntityID, Is.EqualTo(entityID),
                             "Unexpected entity ID: " + testData);

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
                    Fail("Unexpectedly failed getting entity by record: "
                         + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        entityExceptionType, e,
                        "get-entity-by-id failed with an unexpected exception type: "
                        + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestGetEntityByEntityIDDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                long entityID = GetEntityID(recordKey);

                string defaultResult = engine.GetEntity(entityID);

                string explicitResult = engine.GetEntity(
                    entityID, SzEntityDefaultFlags);

                long returnCode = engine.GetNativeApi().GetEntityByEntityID(
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

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestFindInterestingEntitiesByRecordIDDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string dataSourceCode = recordKey.dataSourceCode;
                string recordID = recordKey.recordID;

                string defaultResult = engine.FindInterestingEntities(
                    dataSourceCode, recordID);

                string explicitResult = engine.FindInterestingEntities(
                    dataSourceCode, recordID, SzFindInterestingEntitiesDefaultFlags);

                string nullResult = engine.FindInterestingEntities(
                    dataSourceCode, recordID, null);

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(defaultResult, Is.EqualTo(nullResult),
                    "Explicitly setting default flags yields a different result "
                    + "than setting the flags parameter to null for the SDK function.");

            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting interesting entities by record", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetGetEntityParameters))]
    public void TestFindInterestingEntitiesByRecordID(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        Func<SzCoreEngineReadTest, long> entityIDFunc,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType)
    {
        long entityID = entityIDFunc(this);

        string testData = "description=[ " + testDescription
            + " ], recordKey=[ " + recordKey + " ], entityID=[ "
            + entityID + " ], flags=[ " + SzEntityFlags.FlagsToString(flags)
            + " ], expectedExceptionType=[ " + recordExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindInterestingEntities(
                    recordKey.dataSourceCode, recordKey.recordID, flags);

                if (recordExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in getting an entity: " + testData);
                }

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result)?.AsObject();

                    if (jsonObject == null)
                    {
                        throw new JsonException("Failed to parse as JsonObject");
                    }
                }
                catch (Exception e)
                {
                    Fail("Failed to parse entity result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

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
                    Fail("Unexpectedly failed getting interesting entities "
                         + "by record: " + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        recordExceptionType, e,
                        "find-interesting-entities-by-record failed with an "
                        + "unexpected exception type: " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetGetEntityParameters))]
    public void TestFindInterestingEntitiesByEntityID(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        Func<SzCoreEngineReadTest, long> entityIDFunc,
        SzFlag? flags,
        Type? recordExceptionType,
        Type? entityExceptionType)
    {
        long entityID = entityIDFunc(this);

        string testData = "description=[ " + testDescription
            + " ], recordKey=[ " + recordKey + " ], entityID=[ "
            + entityID + " ], flags=[ " + SzEntityFlags.FlagsToString(flags)
            + " ], expectedExceptionType=[ " + entityExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.FindInterestingEntities(entityID, flags);

                if (entityExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in getting an entity: " + testData);
                }

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result)?.AsObject();

                }
                catch (Exception e)
                {
                    Fail("Failed to parse entity result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

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
                    Fail("Unexpectedly failed getting interesting entities by ID: "
                         + description, e);

                }
                else
                {
                    Assert.IsInstanceOf(
                        entityExceptionType, e,
                        "find-interesting-entities-by-id failed with an unexpected "
                        + "exception type: " + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestFindInterestingEntitiesByEntityIDDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                long entityID = GetEntityID(recordKey);

                string defaultResult = engine.FindInterestingEntities(entityID);

                string explicitResult = engine.FindInterestingEntities(
                    entityID, SzNoFlags);

                string nullResult = engine.FindInterestingEntities(
                    entityID, null);

                Assert.That(defaultResult, Is.EqualTo(explicitResult),
                    "Explicitly setting default flags yields a different result "
                    + "than omitting the flags parameter to the SDK function.");

                Assert.That(defaultResult, Is.EqualTo(nullResult),
                    "Explicitly setting default flags yields a different result "
                    + "than setting the flags parameter to null for the SDK function.");

            }
            catch (Exception e)
            {
                Fail("Unexpectedly failed getting entity by record", e);
            }
        });
    }

    private static List<object?[]> GetGetRecordParameters()
    {
        List<object?[]> result = new List<object?[]>();
        Iterator<SzFlag?> flagSetIter = GetCircularIterator(RecordFlagSets);

        Type UnknownSource = typeof(SzUnknownDataSourceException);
        Type NotFound = typeof(SzNotFoundException);

        foreach ((string, string) recordKey in RecordKeys)
        {
            result.Add(new object?[] {
                "Get record for " + recordKey + " test",
                recordKey,
                flagSetIter.Next(),
                null});
        }

        result.Add(new object?[] {
            "Get record with bad data source code test",
            (UnknownDataSource, "ABC123"),
            flagSetIter.Next(),
            UnknownSource});

        result.Add(new object?[] {
            "Get record with not-found record ID test",
            (PassengersDataSource, "XXX000"),
            flagSetIter.Next(),
            NotFound});

        return result;
    }

    [Test, TestCaseSource(nameof(GetGetRecordParameters))]
    public void TestGetRecord(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        SzFlag? flags,
        Type? expectedExceptionType)
    {
        string testData = "description=[ " + testDescription
            + " ], recordKey=[ " + recordKey + " ], flags=[ "
            + SzRecordFlags.FlagsToString(flags)
            + " ], expectedExceptionType=[ "
            + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.GetRecord(recordKey.dataSourceCode,
                                                 recordKey.recordID,
                                                 flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in getting a record: " + testData);
                }

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result)?.AsObject();

                    if (jsonObject == null)
                    {
                        throw new JsonException("Failed to parse as JSON object");
                    }
                }
                catch (Exception e)
                {
                    Fail("Failed to parse record result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

                // get the data source code
                string? dataSourceCode
                    = jsonObject?["DATA_SOURCE"]?.GetValue<string>();

                Assert.IsNotNull(dataSourceCode,
                    "No DATA_SOURCE property in record JSON: "
                    + testData + ", result=[ " + result + " ]");

                // get the record ID
                string? recordID = jsonObject?["RECORD_ID"]?.GetValue<string>();

                Assert.IsNotNull(recordID, "No RECORD_ID property in record JSON: "
                    + testData + ", result=[ " + result + " ]");

                (string, string) actualRecordKey = (dataSourceCode ?? "",
                                                   recordID ?? "");

                Assert.That(actualRecordKey, Is.EqualTo(recordKey),
                    "The data source code and/or record ID are not as expected: "
                    + testData + ", result=[ " + result + " ]");

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

                if (expectedExceptionType == null)
                {
                    Fail("Unexpectedly failed getting entity by record: "
                         + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "get-record failed with an unexpected exception type: "
                        + description);
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(RecordKeys))]
    public void TestGetRecordDefaults(
        (string dataSourceCode, string recordID) recordKey)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string dataSourceCode = recordKey.dataSourceCode;
                string recordID = recordKey.recordID;

                string defaultResult = engine.GetRecord(dataSourceCode, recordID);

                string explicitResult = engine.GetRecord(
                    dataSourceCode, recordID, SzRecordDefaultFlags);

                long returnCode = engine.GetNativeApi().GetRecord(
                    dataSourceCode, recordID, out string nativeResult);

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

    private class Criterion
    {
        internal string key;
        internal ISet<string> values;

        internal Criterion(string key, params string[] values)
        {
            this.key = key;
            this.values = new SortedSet<string>();
            foreach (string value in values)
            {
                this.values.Add(value);
            }
        }
    }

    private static Criterion CriterionOf(string key, params string[] values)
    {
        return new Criterion(key, values);
    }

    private static IDictionary<string, ISet<string>> CriteriaOf(
        string key, params string[] values)
    {
        Criterion criterion = CriterionOf(key, values);
        return CriteriaOf(criterion);
    }

    private static IDictionary<string, ISet<string>> CriteriaOf(
        params Criterion[] criteria)
    {
        IDictionary<string, ISet<string>> result
            = new Dictionary<string, ISet<string>>();

        foreach (Criterion criterion in criteria)
        {
            ISet<String>? values = (result.ContainsKey(criterion.key))
                ? result[criterion.key] : null;
            if (values == null)
            {
                result.Add(criterion.key, criterion.values);
            }
            else
            {
                foreach (string value in criterion.values)
                {
                    values?.Add(value);
                }
            }
        }
        return result;
    }

    public static List<object?[]> GetSearchParameters()
    {
        List<object?[]> result = new List<object?[]>();

        IDictionary<IDictionary<string, ISet<string>>, IDictionary<SzFlag, int>>
            searchCountMap = new Dictionary<IDictionary<string, ISet<string>>, IDictionary<SzFlag, int>>();

        searchCountMap.Add(CriteriaOf("PHONE_NUMBER", "702-555-1212"),
                           SortedMapOf((SzSearchIncludePossiblyRelated, 1)));

        searchCountMap.Add(CriteriaOf("PHONE_NUMBER", "212-555-1212"),
                           SortedMapOf((SzSearchIncludePossiblyRelated, 1)));

        searchCountMap.Add(CriteriaOf("PHONE_NUMBER", "818-555-1313"),
                           SortedMapOf((SzSearchIncludePossiblyRelated, 1)));

        searchCountMap.Add(CriteriaOf("PHONE_NUMBER", "818-555-1212"),
                           SortedMapOf((SzSearchIncludePossiblyRelated, 1)));

        searchCountMap.Add(
            CriteriaOf("PHONE_NUMBER", "818-555-1212", "818-555-1313"),
            SortedMapOf((SzSearchIncludePossiblyRelated, 2)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("ADDR_LINE1", "100 MAIN STREET"),
                       CriterionOf("ADDR_CITY", "LOS ANGELES"),
                       CriterionOf("ADDR_STATE", "CALIFORNIA"),
                       CriterionOf("ADDR_POSTAL_CODE", "90012")),
            SortedMapOf((SzSearchIncludePossiblyRelated, 2)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("NAME_FULL", "JOHN DOE", "JANE DOE"),
                       CriterionOf("ADDR_LINE1", "100 MAIN STREET"),
                       CriterionOf("ADDR_CITY", "LOS ANGELES"),
                       CriterionOf("ADDR_STATE", "CALIFORNIA"),
                       CriterionOf("ADDR_POSTAL_CODE", "90012")),
            SortedMapOf((SzSearchIncludeResolved, 2)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("NAME_FULL", "JOHN DOE"),
                       CriterionOf("ADDR_LINE1", "100 MAIN STREET"),
                       CriterionOf("ADDR_CITY", "LOS ANGELES"),
                       CriterionOf("ADDR_STATE", "CALIFORNIA"),
                       CriterionOf("ADDR_POSTAL_CODE", "90012")),
            SortedMapOf((SzSearchIncludeResolved, 1),
                        (SzSearchIncludePossiblyRelated, 1)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("NAME_FULL", "Mark Hightower"),
                       CriterionOf("PHONE_NUMBER", "563-927-2833")),
            SortedMapOf((SzSearchIncludeResolved, 1),
                        (SzSearchIncludePossiblySame, 1)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("NAME_FULL", "Mark Hightower"),
                       CriterionOf("DATE_OF_BIRTH", "1981-03-22")),
            SortedMapOf((SzSearchIncludePossiblySame, 1)));

        searchCountMap.Add(
            CriteriaOf(CriterionOf("NAME_FULL", "Mark Hightower"),
                       CriterionOf("PHONE_NUMBER", "563-927-2833"),
                       CriterionOf("PHONE_NUMBER", "781-332-2824"),
                       CriterionOf("DATE_OF_BIRTH", "1981-06-22")),
            SortedMapOf((SzSearchIncludeResolved, 1),
                        (SzSearchIncludePossiblySame, 1)));

        foreach (SzFlag? flagSet in SearchFlagSets)
        {
            foreach (KeyValuePair<IDictionary<string, ISet<string>>, IDictionary<SzFlag, int>> pair
                     in searchCountMap)
            {
                IDictionary<string, ISet<string>> criteria = pair.Key;
                IDictionary<SzFlag, int> countsMap = pair.Value;
                int expectedCount = 0;
                int totalCount = 0;
                int flagCount = 0;
                foreach (SzFlag flag in SearchIncludeFlags)
                {
                    if (countsMap.ContainsKey(flag))
                    {
                        totalCount += countsMap[flag];
                    }
                    if (flagSet == null || ((flagSet & flag) == SzNoFlags)) continue;
                    flagCount++;
                    if (!countsMap.ContainsKey(flag)) continue;
                    expectedCount += countsMap[flag];
                }
                if (flagCount == 0)
                {
                    expectedCount = totalCount;
                }

                string attributes = CriteriaToJson(criteria);

                if (result.Count == 0)
                {
                    result.Add(new object?[] {
                        attributes,
                        "BAD_SEARCH_PROFILE",
                        flagSet,
                        0,
                        typeof(SzBadInputException)});
                }

                result.Add(new object?[] {
                    attributes,
                    null,
                    flagSet,
                    expectedCount,
                    null});

                result.Add(new object?[] {
                    attributes,
                    "SEARCH",
                    flagSet,
                    expectedCount,
                    null});

                result.Add(new object?[] {
                    attributes,
                    "INGEST",
                    flagSet,
                    expectedCount,
                    null});

            }
        }

        return result;
    }

    public static List<object?[]> GetSearchDefaultParameters()
    {
        List<object?[]> searchParams = GetSearchParameters();

        List<object?[]> defaultParams = new List<object?[]>(searchParams.Count);

        for (int index = 0; index < searchParams.Count; index++)
        {
            object?[] args = searchParams[index];

            // skip parameters that expect exceptions
            if (args[args.Length - 1] != null) continue;

            defaultParams.Add(new object?[] { args[0], args[1] });
        }

        return defaultParams;
    }

    private static string CriteriaToJson(IDictionary<string, ISet<string>> criteria)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('{');
        string prefix1 = "";
        foreach (KeyValuePair<string, ISet<String>> pair in criteria)
        {
            string key = pair.Key.ToUpperInvariant();
            ISet<string> values = pair.Value;
            if (values.Count == 0) continue;
            if (values.Count == 1)
            {
                sb.Append(prefix1);
                sb.Append(JsonEscape(key)).Append(':');
                IEnumerator<string> ienum = values.GetEnumerator();
                ienum.MoveNext();
                sb.Append(JsonEscape(ienum.Current));
                prefix1 = ",";
                continue;
            }
            string pluralKey = (key.EndsWith('S'))
                ? (key + "ES") : (key + "S");
            sb.Append(prefix1);
            sb.Append(JsonEscape(pluralKey)).Append(": [");
            string prefix2 = "";
            foreach (string value in values)
            {
                sb.Append(prefix2);
                sb.Append('{').Append(JsonEscape(key));
                sb.Append(':').Append(JsonEscape(value)).Append('}');
                prefix2 = ",";
            }
            sb.Append(']');
            prefix1 = ",";
        }
        sb.Append('}');
        return sb.ToString();
    }

    [Test, TestCaseSource(nameof(GetSearchParameters))]
    public void TestSearchByAttributes(string attributes,
                                       string searchProfile,
                                       SzFlag? flags,
                                       int expectedCount,
                                       Type? expectedExceptionType)
    {
        string testData = "description=[ " + attributes
            + " ], searchProfile=[ " + searchProfile
            + " ], flags=[ " + SzSearchFlags.FlagsToString(flags)
            + " ], expectedCount=[ " + expectedCount + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string? result = null;
                if (searchProfile == null)
                {
                    result = engine.SearchByAttributes(attributes, flags);
                }
                else
                {
                    result = engine.SearchByAttributes(attributes,
                                                       searchProfile,
                                                       flags);
                }

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in searching: " + testData);
                }

                Assert.IsNotNull(result, "Search result is null: " + testData);
                Assert.That(result.Trim(), Is.Not.EqualTo(""),
                    "Search result is empty: " + testData);

                // parse the result
                JsonObject? jsonObject = null;
                try
                {
                    jsonObject = JsonNode.Parse(result ?? "")?.AsObject();

                }
                catch (Exception e)
                {
                    Fail("Failed to parse search result JSON: " + testData
                         + ", result=[ " + result + " ]", e);
                }

                JsonArray? entities = jsonObject?["RESOLVED_ENTITIES"]?.AsArray();
                Assert.IsNotNull(entities, "The RESOLVED_ENTITIES property was not found "
                    + "in the result.  " + testData + ", result=[ " + result + " ]");

                int actualCount = entities?.Count ?? 0;
                Assert.That(actualCount, Is.EqualTo(expectedCount),
                    "Unexpected number of search results.  " + testData
                    + ", entities=[ " + entities + " ]");

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

                if (expectedExceptionType == null)
                {
                    Fail("Unexpectedly failed search: " + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "search failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });

    }

    [Test, TestCaseSource(nameof(GetSearchDefaultParameters))]
    public void TestSearchByAttributesDefaults(string attributes,
                                               string searchProfile)
    {
        this.PerformTest(() =>
        {
            try
            {
                SzCoreEngine engine = (SzCoreEngine)this.Env.GetEngine();

                string defaultResult = (searchProfile == null)
                    ? engine.SearchByAttributes(attributes)
                    : engine.SearchByAttributes(attributes, searchProfile);

                string explicitResult = (searchProfile == null)
                    ? engine.SearchByAttributes(attributes,
                                                SzSearchByAttributesDefaultFlags)
                    : engine.SearchByAttributes(attributes,
                                                searchProfile,
                                                SzSearchByAttributesDefaultFlags);

                long returnCode = engine.GetNativeApi().SearchByAttributes(
                    attributes, out string nativeResult);

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
                Fail("Unexpectedly failed searching by attributes", e);
            }
        });
    }

}
