namespace Senzing.Sdk.Tests.Core;

using System;
using System.IO;
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
using static Senzing.Sdk.Tests.Core.SzRecord;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreEngineWriteTest : AbstractTest
{
    private const string CustomersDataSource = "CUSTOMERS";
    private const string WatchlistDataSource = "WATCHLIST";
    private const string EmployeesDataSource = "EMPLOYEES";
    private const string PassengersDataSource = "PASSENGERS";
    private const string VipsDataSource = "VIPS";
    private const string UnknownDataSource = "UNKNOWN";

    private static readonly SzRecord RecordJoeSchmoe
        = new SzRecord(
            SzFullName.Of("Joe Schmoe"),
            SzPhoneNumber.Of("725-555-1313"),
            SzFullAddress.Of("101 Main Street, Las Vegas, NV 89101"));

    private static readonly SzRecord RecordJaneSmith
        = new SzRecord(
            SzFullName.Of("Jane Smith"),
            SzPhoneNumber.Of("725-555-1414"),
            SzFullAddress.Of("440 N Rancho Blvd, Las Vegas, NV 89101"));

    private static readonly SzRecord RECORD_JOHN_DOE
        = new SzRecord(
            SzFullName.Of("John Doe"),
            SzPhoneNumber.Of("725-555-1717"),
            SzFullAddress.Of("777 W Sahara Blvd, Las Vegas, NV 89107"));

    private static readonly SzRecord RecordJanesMoriarty
        = new SzRecord(
            SzNameByParts.Of("James", "Moriarty"),
            SzPhoneNumber.Of("44-163-555-1313"),
            SzFullAddress.Of("16A Upper Montagu St, London, W1H 2PB, England"));

    private static readonly SzRecord RecordSherlockHolmes
        = new SzRecord(
            SzFullName.Of("Sherlock Holmes"),
            SzPhoneNumber.Of("44-163-555-1212"),
            SzFullAddress.Of("221b Baker Street, London, NW1 6XE, England"));

    private static readonly SzRecord RecordJohnWatson
        = new SzRecord(
            SzFullName.Of("Dr. John H. Watson"),
            SzPhoneNumber.Of("44-163-555-1414"),
            SzFullAddress.Of("221b Baker Street, London, NW1 6XE, England"));

    private static readonly SzRecord RecordJoannSmith
        = new SzRecord(
            SzFullName.Of("Joann Smith"),
            SzPhoneNumber.Of("725-888-3939"),
            SzFullAddress.Of("101 Fifth Ave, Las Vegas, NV 89118"),
            SzDateOfBirth.Of("15-MAY-1983"));

    private static readonly SzRecord RecordBillWright
        = new SzRecord(
            SzNameByParts.Of("Bill", "Wright", "AKA"),
            SzNameByParts.Of("William", "Wright", "PRIMARY"),
            SzPhoneNumber.Of("725-444-2121"),
            SzAddressByParts.Of("101 Main StreetFifth Ave", "Las Vegas", "NV", "89118"),
            SzDateOfBirth.Of("15-MAY-1983"));

    private static readonly SzRecord RecordCraigSmith
        = new SzRecord(
            SzNameByParts.Of("Craig", "Smith"),
            SzPhoneNumber.Of("725-888-3940"),
            SzFullAddress.Of("101 Fifth Ave, Las Vegas, NV 89118"),
            SzDateOfBirth.Of("12-JUN-1981"));

    private static readonly SzRecord RecordKimLong
        = new SzRecord(
            SzFullName.Of("Kim Long"),
            SzPhoneNumber.Of("725-135-1913"),
            SzFullAddress.Of("451 Dover St., Las Vegas, NV 89108"),
            SzDateOfBirth.Of("24-OCT-1976"));

    private static readonly SzRecord RecordKathyOsbourne
        = new SzRecord(
            SzFullName.Of("Kathy Osbourne"),
            SzPhoneNumber.Of("725-111-2222"),
            SzFullAddress.Of("707 Seventh Ave, Las Vegas, NV 89143"),
            SzDateOfBirth.Of("24-OCT-1976"));

    private static readonly List<SzRecord> NewRecords
        = ListOf(RecordBillWright,
                 RecordCraigSmith,
                 RecordJanesMoriarty,
                 RecordJaneSmith,
                 RecordJoannSmith,
                 RecordJoeSchmoe,
                 RECORD_JOHN_DOE,
                 RecordJohnWatson,
                 RecordKathyOsbourne,
                 RecordKimLong,
                 RecordSherlockHolmes);

    private static readonly List<(string, string)> NewRecordKeys
        = ListOf(
            (CustomersDataSource, "ABC123"),
            (WatchlistDataSource, "DEF456"),
            (UnknownDataSource, "GHI789"),
            (CustomersDataSource, "JKL012"),
            (WatchlistDataSource, "MNO345"),
            (CustomersDataSource, "PQR678"),
            (WatchlistDataSource, "STU901"),
            (CustomersDataSource, "VWX234"),
            (WatchlistDataSource, "YZA567"),
            (CustomersDataSource, "BCD890"),
            (WatchlistDataSource, "EFG123")
        );

    private static readonly List<SzRecord> CountRedoTriggerRecords = ListOf(
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-01"),
            SzFullName.Of("Scott Summers"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-02"),
            SzFullName.Of("Jean Gray"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-03"),
            SzFullName.Of("Charles Xavier"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-04"),
            SzFullName.Of("Henry McCoy"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-05"),
            SzFullName.Of("James Howlett"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-06"),
            SzFullName.Of("Ororo Munroe"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-07"),
            SzFullName.Of("Robert Drake"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-08"),
            SzFullName.Of("Anna LeBeau"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-09"),
            SzFullName.Of("Lucas Bishop"),
            SzSocialSecurity.Of("999-99-9999")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-10"),
            SzFullName.Of("Erik Lehnsherr"),
            SzSocialSecurity.Of("999-99-9999")));

    private static readonly List<SzRecord> ProcessRedoTriggerRecords = ListOf(
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-11"),
            SzFullName.Of("Anthony Stark"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-12"),
            SzFullName.Of("Janet Van Dyne"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-13"),
            SzFullName.Of("Henry Pym"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-14"),
            SzFullName.Of("Bruce Banner"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-15"),
            SzFullName.Of("Steven Rogers"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-16"),
            SzFullName.Of("Clinton Barton"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-17"),
            SzFullName.Of("Wanda Maximoff"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-18"),
            SzFullName.Of("Victor Shade"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-19"),
            SzFullName.Of("Natasha Romanoff"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (CustomersDataSource, "SAME-SSN-20"),
            SzFullName.Of("James Rhodes"),
            SzSocialSecurity.Of("888-88-8888")));

    private static readonly IList<SzFlag?> WriteFlagSets;

    private static readonly IList<SzFlag?> RecordFlagSets;

    static SzCoreEngineWriteTest()
    {
        List<SzFlag?> list = new List<SzFlag?>(4);
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzWithInfo);
        WriteFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzRecordDefaultFlags);
        list.Add(SzRecordAllFlags);
        list.Add(SzEntityIncludeRecordMatchingInfo
                 | SzEntityIncludeRecordUnmappedData);
        list.Add(SzEntityIncludeInternalFeatures
                 | SzEntityIncludeRecordFeatureDetails
                 | SzEntityIncludeRecordFeatureStats);
        list.Add(SzEntityIncludeRecordFeatureDetails
                 | SzEntityIncludeRecordFeatureStats);
        list.Add(SzEntityIncludeRecordTypes
                 | SzEntityIncludeRecordJsonData);
        RecordFlagSets = list.AsReadOnly();
    }

    private readonly Dictionary<(string, string), long> LoadedRecordMap
        = new Dictionary<(string, string), long>();

    private readonly Dictionary<long, ISet<(string, string)>> LoadedEntityMap
        = new Dictionary<long, ISet<(string, string)>>();

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

    private static readonly IList<(string, string)> ExistingRecordKeys
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
                 VipJKL456).AsReadOnly();

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
        String settings = this.GetRepoSettings();

        String instanceName = this.GetType().Name;

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
            foreach ((string dataSourceCode, string recordID) key in ExistingRecordKeys)
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
            return (key.GetHashCode() % 2) == 0 ? 1000000L : -20L;
        }
    }

    /**
     * Overridden to configure some data sources.
     */
    protected override void PrepareRepository()
    {
        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();

        ISet<string> dataSources = SortedSetOf(CustomersDataSource,
                                               WatchlistDataSource,
                                               PassengersDataSource,
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
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] passengers = {
            new string[] {PassengerABC123.recordID, "Joseph", "Schmidt", "818-555-1212", "818-777-2424",
                "101 Main Street, Los Angeles, CA 90011", "12-JAN-1981"},
            new string[] {PassengerDEF456.recordID, "Joann", "Smith", "818-555-1212", "818-888-3939",
                "101 Fifth Ave, Los Angeles, CA 90018", "15-MAR-1982"},
            new string[] {PassengerGHI789.recordID, "John", "Parker", "818-555-1313", "818-999-2121",
                "101 Fifth Ave, Los Angeles, CA 90018", "17-DEC-1977"},
            new string[] {PassengerJKL012.recordID, "Jane", "Donaldson", "818-555-1313", "818-222-3131",
                "400 River Street, Pasadena, CA 90034", "23-MAY-1973"}
        };
        return this.PrepareCSVFile("test-passengers-", headers, passengers);
    }

    private FileInfo PrepareEmployeeFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] employees = {
            new string[] {EmployeeMNO345.recordID, "Bill", "Bandley", "818-444-2121", "818-123-4567",
                "101 Main Street, Los Angeles, CA 90011", "22-AUG-1981"},
            new string[] {EmployeePQR678.recordID, "Craig", "Smith", "818-555-1212", "818-888-3939",
                "451 Dover Street, Los Angeles, CA 90018", "17-OCT-1983"},
            new string[] {EmployeeABC567.recordID, "Kim", "Long", "818-246-8024", "818-135-7913",
                "451 Dover Street, Los Angeles, CA 90018", "24-NOV-1975"},
            new string[] {EmployeeDEF890.recordID, "Katrina", "Osmond", "818-444-2121", "818-111-2222",
                "707 Seventh Ave, Los Angeles, CA 90043", "27-JUN-1980"}
        };

        return this.PrepareJsonArrayFile("test-employees-", headers, employees);
    }

    private FileInfo PrepareVipFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] vips = {
            new string[] {VipSTU901.recordID, "Martha", "Wayne", "818-891-9292", "818-987-1234",
                "888 Sepulveda Blvd, Los Angeles, CA 90034", "27-NOV-1973"},
            new string[] {VipXYZ234.recordID, "Jane", "Johnson", "818-333-7171", "818-123-9876",
                "400 River Street, Pasadena, CA 90034", "6-SEP-1975"},
            new string[] {VipGHI123.recordID, "Martha", "Kent", "818-333-5757", "818-123-9876",
                "888 Sepulveda Blvd, Los Angeles, CA 90034", "17-AUG-1978"},
            new string[] {VipJKL456.recordID, "Kelly", "Rogers", "818-333-7171", "818-789-6543",
                "707 Seventh Ave, Los Angeles, CA 90043", "15-JAN-1979"}
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

    public static List<object?[]> GetPreprocessRecordArguments()
    {
        List<object?[]> result = new List<object?[]>();
        int count = Math.Min(NewRecordKeys.Count, NewRecords.Count);
        IEnumerator<(string, string)> keyIter = NewRecordKeys.GetEnumerator();
        IEnumerator<SzRecord> recordIter = NewRecords.GetEnumerator();

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(RecordFlagSets);

        for (int index = 0; index < count; index++)
        {
            keyIter.MoveNext();
            recordIter.MoveNext();
            (string, string) key = keyIter.Current;
            SzRecord record = recordIter.Current;
            Type? exceptionType = null;
            SzFlag? flagSet = flagSetIter.Next();

            record = new SzRecord(key, record);

            result.Add(new object?[] { record, flagSet, exceptionType });
        }

        return result;
    }

    [Test, TestCaseSource(nameof(GetPreprocessRecordArguments)), Order(5)]
    public void TestPreprocessRecord(SzRecord record,
                                     SzFlag? flags,
                                     Type? expectedExceptionType)
    {
        String testData = "record=[ " + record + " ], withFlags=[ "
            + SzRecordFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String result = engine.PreprocessRecord(record.ToString(),
                                                        flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in adding record: " + testData);
                }

                // parse the result as JSON and check that it parses
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                if (flags == null || flags == SzNoFlags)
                {
                    Assert.That(jsonObject?.Count, Is.EqualTo(0),
                                "Unexpected return properties on preprocess: "
                                + testData + ", " + result);
                }
                else
                {
                    if ((flags & SzEntityIncludeRecordUnmappedData) != SzNoFlags)
                    {
                        JsonNode? node = jsonObject?["UNMAPPED_DATA"];
                        Assert.IsNotNull(node,
                            "Did not get UNMAPPED_DATA property with "
                            + "SzEntityIncludeRecordUnmappedData: "
                            + testData + ", " + result);
                    }
                    if ((flags & SzEntityIncludeRecordJsonData) != SzNoFlags)
                    {
                        JsonNode? node = jsonObject?["JSON_DATA"];
                        Assert.IsNotNull(node,
                            "Did not get JSON_DATA property with "
                            + "SzEntityIncludeRecordJsonData: "
                            + testData + ", " + result);
                    }
                    if ((flags & SzEntityIncludeRecordFeatureDetails) != SzNoFlags)
                    {
                        JsonNode? node = jsonObject?["FEATURES"];
                        Assert.IsNotNull(node,
                            "Did not get FEATURES property with "
                            + "SzEntityIncludeRecordFeatureDetails: "
                            + testData + ", " + result);
                    }
                }

            }
            catch (Exception e)
            {
                String description = "";
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
                    Fail("Unexpectedly failed preprocessing a record: "
                         + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "preprocessRecord() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    public static List<object?[]> GetAddRecordArguments()
    {
        List<object?[]> result = new List<object?[]>();
        int count = Math.Min(NewRecordKeys.Count, NewRecords.Count);
        IEnumerator<(string, string)> keyIter = NewRecordKeys.GetEnumerator();
        IEnumerator<SzRecord> recordIter = NewRecords.GetEnumerator();

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WriteFlagSets);

        int errorCase = 0;
        for (int index = 0; index < count; index++)
        {
            keyIter.MoveNext();
            recordIter.MoveNext();
            (string dataSourceCode, string recordID) key = keyIter.Current;
            SzRecord record = recordIter.Current;
            Type? exceptionType = null;
            SzFlag? flagSet = flagSetIter.Next();

            switch (errorCase)
            {
                case 1:
                    {
                        String dataSource
                            = (CustomersDataSource.Equals(key.dataSourceCode, Ordinal))
                            ? WatchlistDataSource
                            : CustomersDataSource;

                        (string, string) wrongKey
                            = (dataSource, key.recordID);

                        record = new SzRecord(wrongKey, record);

                        exceptionType = typeof(SzBadInputException);
                        errorCase++;
                    }
                    break;
                case 2:
                    {
                        (string, string) wrongKey = (key.dataSourceCode, "WRONG_ID");

                        record = new SzRecord(wrongKey, record);

                        exceptionType = typeof(SzBadInputException);

                        errorCase++;
                    }
                    break;
                default:
                    {
                        record = new SzRecord(key, record);
                        if (key.dataSourceCode.Equals(UnknownDataSource, Ordinal))
                        {
                            errorCase++;
                            exceptionType = typeof(SzUnknownDataSourceException);
                        }
                    }
                    break;
            }

            result.Add(new object?[] { key, record, flagSet, exceptionType });
        }

        return result;
    }

    [Test, TestCaseSource(nameof(GetAddRecordArguments)), Order(10)]
    public void TestAddRecord(
        (string dataSourceCode, string recordID) recordKey,
        SzRecord record,
        SzFlag? flags,
        Type? expectedExceptionType)
    {
        String testData = "recordKey=[ " + recordKey
            + " ], record=[ " + record + " ], withFlags=[ "
            + SzAddRecordFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String result = engine.AddRecord(recordKey.dataSourceCode,
                                                 recordKey.recordID,
                                                 record.ToString(),
                                                 flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in adding record: " + testData);
                }

                // check if we are expecting info
                if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                {
                    // parse the result as JSON and check that it parses
                    JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                    JsonNode? node = jsonObject?["DATA_SOURCE"];
                    Assert.IsNotNull(node, "Info message lacking DATA_SOURCE key: "
                                     + testData);

                    node = jsonObject?["RECORD_ID"];
                    Assert.IsNotNull(node, "Info message lacking RECORD_ID key: "
                                      + testData);

                    node = jsonObject?["AFFECTED_ENTITIES"];
                    Assert.IsNotNull(node, "Info message lacking AFFECTED_ENTITIES key: "
                                     + testData);
                }
                else
                {
                    Assert.That(result, Is.Null,
                                "No INFO requested, but non-empty response received: "
                                + testData);
                }

            }
            catch (Exception e)
            {
                String description = "";
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
                    Fail("Unexpectedly failed adding a record: "
                         + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "addRecord() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, Order(20)]
    public void TestGetStats()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String stats = engine.GetStats();

                Assert.IsNotNull(stats, "Stats was unexpectedly null");

                try
                {
                    JsonObject? jsonObj = JsonNode.Parse(stats)?.AsObject();
                    if (jsonObj == null) throw new JsonException();

                }
                catch (Exception)
                {
                    Fail("Stats were not parseable as JSON: " + stats);
                }

            }
            catch (SzException e)
            {
                Fail("Getting stats failed with an exception", e);
            }

        });
    }


    public static List<object?[]> GetReevaluateRecordArguments()
    {
        List<object?[]> result = new List<object?[]>();
        int errorCase = 0;

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WriteFlagSets);

        foreach ((string dataSourceCode, string recordID) key in ExistingRecordKeys)
        {
            Type? exceptionType = null;
            SzFlag? flagSet = flagSetIter.Next();
            (string, string) paramKey = key;

            switch (errorCase)
            {
                case 0:
                    // do a bad data source
                    paramKey = (UnknownDataSource, key.recordID);
                    exceptionType = typeof(SzUnknownDataSourceException);

                    break;
                case 1:
                    // do a good data source with a bad record ID
                    paramKey = (key.dataSourceCode, key.recordID + "-UNKNOWN");

                    // Per decision, reevaluate record silently does nothing if record ID not found
                    // exceptionType = SzNotFoundException.class;

                    break;
                default:
                    // do nothing
                    break;
            }
            errorCase++;
            result.Add(new object?[] { paramKey, flagSet, exceptionType });
        };

        result.Add(new object?[] {
            (PassengersDataSource, "XXX000"),
            SzNoFlags,
            null});

        result.Add(new object?[] {
            (PassengersDataSource, "XXX000"),
            SzWithInfo,
            null});

        return result;
    }

    [Test, TestCaseSource(nameof(GetReevaluateRecordArguments)), Order(40)]
    public void TestReevaluateRecord(
        (string dataSourceCode, string recordID) recordKey,
        SzFlag? flags,
        Type? expectedExceptionType)
    {
        String testData = "recordKey=[ " + recordKey
            + " ], withFlags=[ " + SzReevaluateFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String result = engine.ReevaluateRecord(
                    recordKey.dataSourceCode, recordKey.recordID, flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in reevaluating record: "
                         + testData);
                }

                // check if we are expecting info
                if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                {
                    // parse the result as JSON and check that it parses
                    JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                    Assert.IsNotNull(jsonObject,
                                    "Failed to parse reevaluate INFO as JSON object: "
                                    + "result=[ " + result + " ], " + testData);

                    IDictionary<string, JsonNode?> dict = (jsonObject == null)
                        ? MapOf<string, JsonNode?>()
                        : (IDictionary<string, JsonNode?>)jsonObject;

                    if (dict.Count > 0)
                    {
                        Assert.IsTrue(dict.ContainsKey("DATA_SOURCE"),
                                      "Info message lacking DATA_SOURCE key: " + testData);
                        Assert.IsTrue(dict.ContainsKey("RECORD_ID"),
                                      "Info message lacking RECORD_ID key: " + testData);
                        Assert.IsTrue(dict.ContainsKey("AFFECTED_ENTITIES"),
                                      "Info message lacking AFFECTED_ENTITIES key: " + testData);
                    }
                }
                else
                {
                    Assert.That(result, Is.Null,
                                "No INFO requested, but non-empty response received");
                }

            }
            catch (Exception e)
            {
                String description = "";
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
                    Fail("Unexpectedly failed reevaluating a record: "
                         + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "Reevaluating record failed with an "
                        + "unexpected exception type: " + testData
                        + ", " + description);
                }
            }
        });
    }

    public static List<object?[]> GetReevaluateEntityArguments()
    {
        List<object?[]> result = new List<object?[]>();
        int errorCase = 0;

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WriteFlagSets);

        foreach ((string dataSourceCode, string recordID) key in ExistingRecordKeys)
        {
            Type? exceptionType = null;
            SzFlag? flagSet = flagSetIter.Next();
            (string, string) paramKey = key;
            Func<SzCoreEngineWriteTest, long> func = (SzCoreEngineWriteTest t) => t.GetEntityID(paramKey);

            switch (errorCase)
            {
                case 0:
                    // do a negative entity ID
                    func = (SzCoreEngineWriteTest t) => -1 & Math.Abs(paramKey.GetHashCode());

                    // Per decision, reevaluate entity silently does nothing if entity ID not found
                    // exceptionType = SzNotFoundException.class;

                    break;
                case 1:
                    // do a large entity that does not exist
                    func = (SzCoreEngineWriteTest t) => 1000000000L;

                    // Per decision, reevaluate entity silently does nothing if entity ID not found
                    // exceptionType = SzNotFoundException.class;

                    break;
                default:
                    // do nothing
                    break;
            }
            errorCase++;
            result.Add(new object?[] { func, flagSet, exceptionType });
        };

        result.Add(new object?[] {
            (SzCoreEngineWriteTest t) => 100000000L, SzNoFlags, null});

        result.Add(new object?[] {
            (SzCoreEngineWriteTest t) => 100000000L, SzWithInfo, null});

        return result;
    }

    [Test, TestCaseSource(nameof(GetReevaluateEntityArguments)), Order(50)]
    public void TestReevaluateEntity(
        Func<SzCoreEngineWriteTest, long> entityIDFunc,
        SzFlag? flags,
        Type? expectedExceptionType)
    {
        long entityID = entityIDFunc(this);
        ISet<(string, string)> recordKeys = (LoadedEntityMap.ContainsKey(entityID))
            ? LoadedEntityMap[entityID] : SortedSetOf<(string, string)>();

        String testData = "entityID=[ " + entityID + " ], havingRecords=[ "
            + recordKeys + " ], withFlags=[ "
            + SzReevaluateFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String result = engine.ReevaluateEntity(entityID, flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in reevaluating entity: " + testData);
                }

                // check if we are expecting info
                if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                {
                    // parse the result as JSON and check that it parses
                    JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                    Assert.IsNotNull(jsonObject,
                                    "Failed to parse INFO as JSON object: "
                                    + "result=[ " + result + " ], " + testData);

                    IDictionary<string, JsonNode?> dict = (jsonObject == null)
                        ? MapOf<string, JsonNode?>()
                        : (IDictionary<string, JsonNode?>)jsonObject;

                    if (dict.Count > 0)
                    {
                        Assert.IsTrue(dict.ContainsKey("AFFECTED_ENTITIES"),
                                      "Info message lacking AFFECTED_ENTITIES key: " + testData);
                    }
                }
                else
                {
                    Assert.That(result, Is.Null,
                                 "No INFO requested, but non-empty response received: "
                                 + testData);
                }

            }
            catch (Exception e)
            {
                String description = "";
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
                    Fail("Unexpectedly failed reevaluating an entity: "
                         + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "Reevaluating entity failed with an "
                        + "unexpected exception type: " + testData
                        + ", " + description);
                }
            }
        });
    }


    public static List<object?[]> GetDeleteRecordArguments()
    {
        List<object?[]> result = new List<object?[]>();
        int errorCase = 0;

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WriteFlagSets);

        foreach ((string dataSourceCode, string recordID) key in ExistingRecordKeys)
        {
            Type? exceptionType = null;
            SzFlag? flagSet = flagSetIter.Next();
            (string dataSourceCode, string recordID) paramKey = key;

            switch (errorCase)
            {
                case 0:
                    // do a bad data source
                    paramKey = (UnknownDataSource, key.recordID);
                    exceptionType = typeof(SzUnknownDataSourceException);
                    break;

                case 1:
                    // do a good data source with a bad record ID
                    paramKey = (key.dataSourceCode, key.recordID + "-UNKNOWN");

                    // NOTE: we expect no exception on deleting non-existent record

                    break;
                default:
                    // do nothing
                    break;
            }
            errorCase++;
            result.Add(new object?[] { paramKey, flagSet, exceptionType });
        };

        return result;
    }

    [Test, TestCaseSource(nameof(GetDeleteRecordArguments)), Order(100)]
    public void TestDeleteRecord(
        (string dataSourceCode, string recordID) recordKey,
        SzFlag? flags,
        Type? expectedExceptionType)
    {
        String testData = "recordKey=[ " + recordKey
            + " ], withFlags=[ " + SzDeleteRecordFlags.FlagsToString(flags)
            + " ], expectedException=[ " + expectedExceptionType + " ]";

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String result = engine.DeleteRecord(
                    recordKey.dataSourceCode, recordKey.recordID, flags);

                if (expectedExceptionType != null)
                {
                    Fail("Unexpectedly succeeded in deleting record: "
                         + testData);
                }

                // check if we are expecting info
                if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                {
                    // parse the result as JSON and check that it parses
                    JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                    Assert.IsNotNull(jsonObject,
                                    "Failed to parse INFO as JSON object: "
                                    + "result=[ " + result + " ], " + testData);

                    IDictionary<string, JsonNode?> dict = (jsonObject == null)
                        ? MapOf<string, JsonNode?>()
                        : (IDictionary<string, JsonNode?>)jsonObject;

                    if (dict.Count > 0)
                    {
                        Assert.IsTrue(dict.ContainsKey("DATA_SOURCE"),
                                      "Info message lacking DATA_SOURCE key: " + testData);
                        Assert.IsTrue(dict.ContainsKey("RECORD_ID"),
                                      "Info message lacking RECORD_ID key: " + testData);
                        Assert.IsTrue(dict.ContainsKey("AFFECTED_ENTITIES"),
                                      "Info message lacking AFFECTED_ENTITIES key: " + testData);
                    }
                }
                else
                {
                    Assert.That(result, Is.Null,
                                 "No INFO requested, but non-empty response received: "
                                 + testData);
                }

            }
            catch (Exception e)
            {
                String description = "";
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
                    Fail("Unexpectedly failed deleting a record: "
                         + testData + ", " + description, e);

                }
                else if (expectedExceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        expectedExceptionType, e,
                        "deleteRecord() failed with an unexpected exception type: "
                        + testData + ", " + description);
                }
            }
        });
    }

    [Test, Order(200)]
    public void TestCountRedoRecordsZero()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                long count = engine.CountRedoRecords();

                Assert.That(count, Is.EqualTo(0), "Unexpected redo record count");

            }
            catch (SzException e)
            {
                Fail("Failed to count records when none present", e);
            }
        });
    }

    [Test, Order(210)]
    public void TestGetRedoRecordZero()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                String redoRecord = engine.GetRedoRecord();

                Assert.IsNull(redoRecord, "Unexpected non-null redo record: " + redoRecord);

            }
            catch (SzException e)
            {
                Fail("Failed to get redo record when none present", e);
            }
        });
    }

    [Test, Order(220)]
    public void TestCountRedoRecordsNonZero()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                foreach (SzRecord record in CountRedoTriggerRecords)
                {
                    (string dataSourceCode, string recordID)? recordKey = record.GetRecordKey();
                    engine.AddRecord(recordKey?.dataSourceCode,
                                     recordKey?.recordID,
                                     record.ToString(),
                                     null);
                }

                long count = engine.CountRedoRecords();

                Assert.That(count, Is.Not.EqualTo(0), "Redo record count unexpectedly zero");

            }
            catch (SzException e)
            {
                Fail("Failed to get redo record when none present", e);
            }
        });
    }

    [Test, Order(230)]
    public void TestProcessRedoRecords()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                foreach (SzRecord record in ProcessRedoTriggerRecords)
                {
                    (string dataSourceCode, string recordID)? recordKey = record.GetRecordKey();
                    engine.AddRecord(recordKey?.dataSourceCode,
                                     recordKey?.recordID,
                                     record.ToString(),
                                     null);
                }

                Iterator<SzFlag?> flagSetIter = GetCircularIterator(WriteFlagSets);

                long redoCount = engine.CountRedoRecords();

                Assert.That(redoCount, Is.Not.EqualTo(0), "Redo record count is zero (0)");
                Assert.IsTrue((redoCount > 0), "Redo record count is negative: " + redoCount);
                long actualCount = 0;
                for (string redoRecord = engine.GetRedoRecord();
                     (redoRecord != null);
                     redoRecord = engine.GetRedoRecord())
                {
                    actualCount++;
                    SzFlag? flags = flagSetIter.Next();

                    String result = engine.ProcessRedoRecord(redoRecord, flags);

                    // check if we are expecting info
                    if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                    {
                        // parse the result as JSON and check that it parses
                        JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                        Assert.IsNotNull(jsonObject,
                                        "Failed to parse INFO as JSON object: "
                                        + "result=[ " + result + " ], " + result);

                        IDictionary<string, JsonNode?> dict = (jsonObject == null)
                            ? MapOf<string, JsonNode?>()
                            : (IDictionary<string, JsonNode?>)jsonObject;

                        if (dict.Count > 0)
                        {
                            Assert.IsTrue(dict.ContainsKey("DATA_SOURCE"),
                                        "Info message lacking DATA_SOURCE key: " + result);
                            Assert.IsTrue(dict.ContainsKey("RECORD_ID"),
                                        "Info message lacking RECORD_ID key: " + result);
                            Assert.IsTrue(dict.ContainsKey("AFFECTED_ENTITIES"),
                                        "Info message lacking AFFECTED_ENTITIES key: " + result);
                        }
                    }
                    else
                    {
                        Assert.That(result, Is.Null,
                                    "No INFO requested, but non-empty response received: "
                                    + result);
                    }
                }

                Assert.That(actualCount, Is.EqualTo(redoCount),
                            "Not all redo records were processed");

            }
            catch (SzException e)
            {
                Fail("Failed to get redo record when none present", e);
            }
        });
    }
}
