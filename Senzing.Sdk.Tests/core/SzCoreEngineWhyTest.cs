namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections;
using System.IO;
using System.Text;
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
internal class SzCoreEngineWhyTest : AbstractTest
{
    private const string CustomersDataSource = "CUSTOMERS";
    private const string CompaniesDataSource = "COMPANIES";
    private const string EmployeesDataSource = "EMPLOYEES";
    private const string PassengersDataSource = "PASSENGERS";
    private const string VipsDataSource = "VIPS";
    private const string ContactsDataSource = "CONTACTS";
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
        = (CustomersDataSource, "MNO345");
    private static readonly (string dataSourceCode, string recordID) PQR678
        = (CustomersDataSource, "PQR678");
    private static readonly (string dataSourceCode, string recordID) ABC567
        = (CustomersDataSource, "ABC567");
    private static readonly (string dataSourceCode, string recordID) DEF890
        = (CustomersDataSource, "DEF890");
    private static readonly (string dataSourceCode, string recordID) STU901
        = (VipsDataSource, "STU901");
    private static readonly (string dataSourceCode, string recordID) XYZ234
        = (VipsDataSource, "XYZ234");
    private static readonly (string dataSourceCode, string recordID) GHI123
        = (VipsDataSource, "GHI123");
    private static readonly (string dataSourceCode, string recordID) JKL456
        = (VipsDataSource, "JKL456");

    private static readonly (string dataSourceCode, string recordID) Company1
        = (CompaniesDataSource, "COMPANY_1");
    private static readonly (string dataSourceCode, string recordID) Company2
        = (CompaniesDataSource, "COMPANY_2");
    private static readonly (string dataSourceCode, string recordID) Employee1
        = (EmployeesDataSource, "EMPLOYEE_1");
    private static readonly (string dataSourceCode, string recordID) Employee2
        = (EmployeesDataSource, "EMPLOYEE_2");
    private static readonly (string dataSourceCode, string recordID) Employee3
        = (EmployeesDataSource, "EMPLOYEE_3");
    private static readonly (string dataSourceCode, string recordID) Contact1
        = (ContactsDataSource, "CONTACT_1");
    private static readonly (string dataSourceCode, string recordID) Contact2
        = (ContactsDataSource, "CONTACT_2");
    private static readonly (string dataSourceCode, string recordID) Contact3
        = (ContactsDataSource, "CONTACT_3");
    private static readonly (string dataSourceCode, string recordID) Contact4
        = (ContactsDataSource, "CONTACT_4");

    private static readonly IList<(string, string)> RecordKeys;
    private static readonly IList<(string, string)> RelatedRecordKeys;
    private static readonly IList<SzFlag?> WhyEntitiesFlagSets;
    private static readonly IList<SzFlag?> WhyRecordsFlagSets;
    private static readonly IList<SzFlag?> WhyRecordInEntityFlagSets;

    static SzCoreEngineWhyTest()
    {
        List<(string, string)> recordKeys = new List<(string, string)>(12);
        List<(string, string)> relatedKeys = new List<(string, string)>(9);

        try
        {
            recordKeys.Add(ABC123);
            recordKeys.Add(DEF456);
            recordKeys.Add(GHI789);
            recordKeys.Add(JKL012);
            recordKeys.Add(MNO345);
            recordKeys.Add(PQR678);
            recordKeys.Add(ABC567);
            recordKeys.Add(DEF890);
            recordKeys.Add(STU901);
            recordKeys.Add(XYZ234);
            recordKeys.Add(GHI123);
            recordKeys.Add(JKL456);

            relatedKeys.Add(Company1);
            relatedKeys.Add(Company2);
            relatedKeys.Add(Employee1);
            relatedKeys.Add(Employee2);
            relatedKeys.Add(Employee3);
            relatedKeys.Add(Contact1);
            relatedKeys.Add(Contact2);
            relatedKeys.Add(Contact3);
            relatedKeys.Add(Contact4);

        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            throw;

        }
        finally
        {
            RecordKeys = recordKeys.AsReadOnly();
            RelatedRecordKeys = relatedKeys.AsReadOnly();
        }

        List<SzFlag?> list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzWhyEntitiesDefaultFlags);
        list.Add(SzWhyAllFlags);
        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzEntityIncludeRecordMatchingInfo);
        list.Add(SzEntityIncludeEntityName);
        WhyEntitiesFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzWhyRecordsDefaultFlags);
        list.Add(SzWhyAllFlags);
        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzEntityIncludeRecordMatchingInfo);
        list.Add(SzEntityIncludeEntityName);
        WhyRecordsFlagSets = list.AsReadOnly();

        list = new List<SzFlag?>();
        list.Add(null);
        list.Add(SzNoFlags);
        list.Add(SzWhyRecordInEntityDefaultFlags);
        list.Add(SzWhyAllFlags);
        list.Add(SzEntityIncludeEntityName
                 | SzEntityIncludeRecordSummary
                 | SzEntityIncludeRecordData
                 | SzEntityIncludeRecordMatchingInfo);
        list.Add(SzEntityIncludeEntityName);
        WhyRecordInEntityFlagSets = list.AsReadOnly();
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

    [OneTimeSetUp]
    public void InitializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment();
        string settings = this.GetRepoSettings();
        string instanceName = this.GetInstanceName();

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
            ISet<(string, string)> recordKeys = new HashSet<(string, string)>();
            foreach ((string, string) recordKey in RecordKeys)
            {
                recordKeys.Add(recordKey);
            }
            foreach ((string, string) recordKey in RelatedRecordKeys)
            {
                recordKeys.Add(recordKey);
            }
            foreach ((string dataSourceCode, string recordID) key in recordKeys)
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

    /**
     * Overridden to configure some data sources.
     */
    protected override void PrepareRepository()
    {
        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();

        ISet<String> dataSources = SortedSetOf(PassengersDataSource,
                                               CustomersDataSource,
                                               VipsDataSource,
                                               CompaniesDataSource,
                                               EmployeesDataSource,
                                               ContactsDataSource);

        FileInfo passengerFile = this.PreparePassengerFile();
        FileInfo customerFile = this.PrepareCustomerFile();
        FileInfo vipFile = this.PrepareVipFile();
        FileInfo companyFile = this.PrepareCompanyFile();
        FileInfo employeeFile = this.PrepareEmployeeFile();
        FileInfo contactFile = this.PrepareContactFile();

        RepositoryManager.ConfigSources(repoDirectory,
                                        dataSources,
                                        true);

        RepositoryManager.LoadFile(repoDirectory,
                                   passengerFile,
                                   PassengersDataSource,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   customerFile,
                                   CustomersDataSource,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   vipFile,
                                   VipsDataSource,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   companyFile,
                                   CompaniesDataSource,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   employeeFile,
                                   EmployeesDataSource,
                                   true);

        RepositoryManager.LoadFile(repoDirectory,
                                   contactFile,
                                   ContactsDataSource,
                                   true);
    }

    private FileInfo PreparePassengerFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] passengers = {
            new string[] {ABC123.recordID, "Joe", "Schmoe", "702-555-1212",
                "702-777-2424", "101 Main Street, Las Vegas, NV 89101", "12-JAN-1981"},
            new string[] {DEF456.recordID, "Joann", "Smith", "702-555-1212",
                "702-888-3939", "101 Fifth Ave, Las Vegas, NV 10018", "15-MAY-1983"},
            new string[] {GHI789.recordID, "John", "Doe", "818-555-1313",
                "818-999-2121", "101 Fifth Ave, Las Vegas, NV 10018", "17-OCT-1978"},
            new string[] {JKL012.recordID, "Jane", "Doe", "818-555-1313",
                "818-222-3131", "400 River Street, Pasadena, CA 90034", "23-APR-1974"}
        };
        return this.PrepareCSVFile("test-passengers-", headers, passengers);
    }

    private FileInfo PrepareCustomerFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] customers = {
            new string[] {MNO345.recordID, "Joseph", "Schmoe", "702-555-1212",
                "702-777-2424", "101 Main Street, Las Vegas, NV 89101", "12-JAN-1981"},
            new string[] {PQR678.recordID, "Craig", "Smith", "212-555-1212",
                "702-888-3939", "451 Dover Street, Las Vegas, NV 89108", "17-NOV-1982"},
            new string[] {ABC567.recordID, "Kim", "Long", "702-246-8024",
                "702-135-7913", "451 Dover Street, Las Vegas, NV 89108", "24-OCT-1976"},
            new string[] {DEF890.recordID, "Kathy", "Osborne", "702-444-2121",
                "702-111-2222", "707 Seventh Ave, Las Vegas, NV 89143", "27-JUL-1981"}
        };

        return this.PrepareJsonArrayFile("test-customers-", headers, customers);
    }

    private FileInfo PrepareVipFile()
    {
        String[] headers = {
            "RECORD_ID", "NAME_FIRST", "NAME_LAST", "MOBILE_PHONE_NUMBER",
            "HOME_PHONE_NUMBER", "ADDR_FULL", "DATE_OF_BIRTH"};

        String[][] vips = {
            new string[] {STU901.recordID, "Martha", "Wayne", "818-891-9292",
                "818-987-1234", "888 Sepulveda Blvd, Los Angeles, CA 90034", "27-NOV-1973"},
            new string[] {XYZ234.recordID, "Jane", "Doe", "818-555-1313",
                "818-222-3131", "400 River Street, Pasadena, CA 90034", "23-APR-1974"},
            new string[] {GHI123.recordID, "Martha", "Kent", "818-333-5757",
                "702-123-9876", "888 Sepulveda Blvd, Los Angeles, CA 90034", "17-OCT-1978"},
            new string[] {JKL456.recordID, "Katherine", "Osborne", "702-444-2121",
                "702-111-2222", "707 Seventh Ave, Las Vegas, NV 89143", "27-JUL-1981"}
        };

        return this.PrepareJsonFile("test-vips-", headers, vips);
    }

    private FileInfo PrepareCompanyFile()
    {
        List<JsonNode?> list = new List<JsonNode?>();
        IDictionary<string, JsonNode?> map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Company1.recordID);
        map.Add("DATA_SOURCE", Company1.dataSourceCode);
        map.Add("NAME_ORG", "Acme Corporation");

        List<JsonNode?> relList = new List<JsonNode?>();
        IDictionary<string, JsonNode?> relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "EMPLOYER_ID");
        relMap.Add("REL_ANCHOR_KEY", "ACME_CORP_KEY");
        relList.Add(new JsonObject(relMap));

        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "CORP_HIERARCHY");
        relMap.Add("REL_ANCHOR_KEY", "ACME_CORP_KEY");
        relList.Add(new JsonObject(relMap));

        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "CORP_HIERARCHY");
        relMap.Add("REL_POINTER_KEY", "COYOTE_SOLUTIONS_KEY");
        relMap.Add("REL_POINTER_ROLE", "ULTIMATE_PARENT");
        relList.Add(new JsonObject(relMap));

        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "CORP_HIERARCHY");
        relMap.Add("REL_POINTER_KEY", "COYOTE_SOLUTIONS_KEY");
        relMap.Add("REL_POINTER_ROLE", "PARENT");
        relList.Add(new JsonObject(relMap));

        // add the relationship list to the main object
        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Company2.recordID);
        map.Add("DATA_SOURCE", Company2.dataSourceCode);
        map.Add("NAME_ORG", "Coyote Solutions");

        relList = new List<JsonNode?>();
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "EMPLOYER_ID");
        relMap.Add("REL_ANCHOR_KEY", "COYOTE_SOLUTIONS_KEY");
        relList.Add(new JsonObject(relMap));
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "CORP_HIERARCHY");
        relMap.Add("REL_ANCHOR_KEY", "COYOTE_SOLUTIONS_KEY");
        relList.Add(new JsonObject(relMap));
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "CORP_HIERARCHY");
        relMap.Add("REL_POINTER_KEY", "ACME_CORP_KEY");
        relMap.Add("REL_POINTER_ROLE", "SUBSIDIARY");
        relList.Add(new JsonObject(relMap));

        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        return this.PrepareJsonFile(
            "test-companies-", new JsonArray(list.ToArray()));
    }

    private FileInfo PrepareEmployeeFile()
    {
        List<JsonNode?> list = new List<JsonNode?>();
        Dictionary<string, JsonNode?> map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Employee1.recordID);
        map.Add("DATA_SOURCE", Employee1.dataSourceCode);
        map.Add("NAME_FULL", "Jeff Founder");

        List<JsonNode?> relList = new List<JsonNode?>(); ;
        Dictionary<string, JsonNode?> relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "EMPLOYEE_NUM");
        relMap.Add("REL_ANCHOR_KEY", "1");
        relList.Add(new JsonObject(relMap));
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "EMPLOYER_ID");
        relMap.Add("REL_POINTER_KEY", "ACME_CORP_KEY");
        relMap.Add("REL_POINTER_ROLE", "EMPLOYED_BY");
        relList.Add(new JsonObject(relMap));
        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Employee2.recordID);
        map.Add("DATA_SOURCE", Employee2.dataSourceCode);
        map.Add("NAME_FULL", "Jane Leader");
        relList = new List<JsonNode?>(); ;
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "EMPLOYEE_NUM");
        relMap.Add("REL_ANCHOR_KEY", "2");
        relList.Add(new JsonObject(relMap));
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "EMPLOYEE_NUM");
        relMap.Add("REL_POINTER_KEY", "1");
        relMap.Add("REL_POINTER_ROLE", "MANAGED_BY");
        relList.Add(new JsonObject(relMap));
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "EMPLOYER_ID");
        relMap.Add("REL_POINTER_KEY", "ACME_CORP_KEY");
        relMap.Add("REL_POINTER_ROLE", "EMPLOYED_BY");
        relList.Add(new JsonObject(relMap));
        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Employee3.recordID);
        map.Add("DATA_SOURCE", Employee3.dataSourceCode);
        map.Add("NAME_FULL", "Joe Workman");

        relList = new List<JsonNode?>(); ;
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_ANCHOR_DOMAIN", "EMPLOYEE_NUM");
        relMap.Add("REL_ANCHOR_KEY", "6");
        relList.Add(new JsonObject(relMap));

        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "EMPLOYEE_NUM");
        relMap.Add("REL_POINTER_KEY", "2");
        relMap.Add("REL_POINTER_ROLE", "MANAGED_BY");
        relList.Add(new JsonObject(relMap));

        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("REL_POINTER_DOMAIN", "EMPLOYER_ID");
        relMap.Add("REL_POINTER_KEY", "ACME_CORP_KEY");
        relMap.Add("REL_POINTER_ROLE", "EMPLOYED_BY");
        relList.Add(new JsonObject(relMap));

        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        return this.PrepareJsonFile("test-employees-", new JsonArray(list.ToArray()));
    }

    private FileInfo PrepareContactFile()
    {
        List<JsonNode?> list = new List<JsonNode?>();
        Dictionary<string, JsonNode?> map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Contact1.recordID);
        map.Add("DATA_SOURCE", Contact1.dataSourceCode);
        map.Add("NAME_FULL", "Richard Couples");
        map.Add("PHONE_NUMBER", "718-949-8812");
        map.Add("ADDR_FULL", "10010 WOODLAND AVE; ATLANTA, GA 30334");

        List<JsonNode?> relList = new List<JsonNode?>(); ;
        Dictionary<string, JsonNode?> relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("RELATIONSHIP_TYPE", "SPOUSE");
        relMap.Add("RELATIONSHIP_KEY", "SPOUSES-1-2");
        relMap.Add("RELATIONSHIP_ROLE", "WIFE");
        relList.Add(new JsonObject(relMap));
        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Contact2.recordID);
        map.Add("DATA_SOURCE", Contact2.dataSourceCode);
        map.Add("NAME_FULL", "Brianna Couples");
        map.Add("PHONE_NUMBER", "718-949-8812");
        map.Add("ADDR_FULL", "10010 WOODLAND AVE; ATLANTA, GA 30334");
        relList = new List<JsonNode?>(); ;
        relMap = new Dictionary<string, JsonNode?>();
        relMap.Add("RELATIONSHIP_TYPE", "SPOUSE");
        relMap.Add("RELATIONSHIP_KEY", "SPOUSES-1-2");
        relMap.Add("RELATIONSHIP_ROLE", "HUSBAND");
        relList.Add(new JsonObject(relMap));
        map.Add("RELATIONSHIP_LIST", new JsonArray(relList.ToArray()));
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Contact3.recordID);
        map.Add("DATA_SOURCE", Contact3.dataSourceCode);
        map.Add("NAME_FULL", "Samuel Strong");
        map.Add("PHONE_NUMBER", "312-889-3340");
        map.Add("ADDR_FULL", "10010 LAKE VIEW RD; SPRINGFIELD, MO 65807");
        list.Add(new JsonObject(map));

        map = new Dictionary<string, JsonNode?>();
        map.Add("RECORD_ID", Contact4.recordID);
        map.Add("DATA_SOURCE", Contact4.dataSourceCode);
        map.Add("NAME_FULL", "Melissa Powers");
        map.Add("PHONE_NUMBER", "312-885-4236");
        map.Add("ADDR_FULL", "10010 LAKE VIEW RD; SPRINGFIELD, MO 65807");
        list.Add(new JsonObject(map));

        return this.PrepareJsonFile("test-contacts-", new JsonArray(list.ToArray()));
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

    public static List<object?[]> GetWhyEntitiesParameters()
    {
        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WhyEntitiesFlagSets);

        List<object?[]> result = new List<object?[]>();

        System.Collections.ArrayList relatedKeys
            = new System.Collections.ArrayList(RelatedRecordKeys.Count);
        foreach ((string, string) recordKey in RelatedRecordKeys)
        {
            relatedKeys.Add(recordKey);
        }

        IList<System.Collections.IList> recordKeyCombos
            = GenerateCombinations(relatedKeys, relatedKeys);

        IList<((string, string), (string, string))> comboTuples
            = new List<((string, string), (string, string))>();

        IEnumerator<System.Collections.IList> ienum
             = recordKeyCombos.GetEnumerator();
        while (ienum.MoveNext())
        {
            System.Collections.IList list = ienum.Current;
            (string, string) key1 = (((string, string)?)list[0] ?? ("A", "B"));
            (string, string) key2 = (((string, string)?)list[1] ?? ("B", "C"));

            // thin the list out to reduce the number of tests
            if (!key1.Equals(key2))
            {
                int index1 = RelatedRecordKeys.IndexOf(key1);
                int index2 = RelatedRecordKeys.IndexOf(key2);
                if (Math.Abs(index2 - index1) <= 4)
                {
                    comboTuples.Add((key1, key2));
                }
            }
        }

        Type? NotFound = typeof(SzNotFoundException);

        foreach (((string, string), (string, string)) recordKeyPair in comboTuples)
        {
            (string, string) recordKey1 = recordKeyPair.Item1;
            (string, string) recordKey2 = recordKeyPair.Item2;

            result.Add(new object?[] {
                "Test " + recordKey1 + " vs " + recordKey2,
                recordKey1,
                (SzCoreEngineWhyTest t) => t.GetEntityID(recordKey1),
                recordKey2,
                (SzCoreEngineWhyTest t) => t.GetEntityID(recordKey2),
                flagSetIter.Next(),
                null});
        }

        result.Add(new object?[] {
            "Why entities with same entity twice: " + Company1,
            Company1,
            (SzCoreEngineWhyTest t) => t.GetEntityID(Company1),
            Company1,
            (SzCoreEngineWhyTest t) => t.GetEntityID(Company1),
            flagSetIter.Next(),
            null});

        result.Add(new object?[] {
            "Not found entity ID test",
            (PassengersDataSource, "XXX000"),
            (SzCoreEngineWhyTest t) => 10000000L,
            Company1,
            (SzCoreEngineWhyTest t) => t.GetEntityID(Company1),
            flagSetIter.Next(),
            NotFound});

        result.Add(new object?[] {
            "Illegal entity ID test",
            (PassengersDataSource, "XXX000"),
            (SzCoreEngineWhyTest t) => -100L,
            Company1,
            (SzCoreEngineWhyTest t) => t.GetEntityID(Company1),
            flagSetIter.Next(),
            NotFound});

        return result;
    }

    public virtual void ValidateWhyEntities(
        string whyEntitiesResult,
        string testData,
        (string, string) recordKey1,
        long entityID1,
        (string, string) recordKey2,
        long entityID2,
        SzFlag? flags)
    {
        JsonObject? jsonObject = JsonNode.Parse(whyEntitiesResult)?.AsObject();

        JsonArray? whyResults = jsonObject?["WHY_RESULTS"]?.AsArray();

        Assert.IsNotNull(whyResults,
            "Missing WHY_RESULTS from whyEntities() result JSON: " + testData);

        Assert.That(whyResults?.Count, Is.EqualTo(1),
            "The WHY_RESULTS array is not of the expected size: " + testData);

        JsonObject? whyResult = whyResults?[0]?.AsObject();

        Assert.IsNotNull(whyResult,
            "First WHY_RESULTS element was null: " + testData);

        long? jsonID = whyResult?["ENTITY_ID"]?.GetValue<long>();

        Assert.IsNotNull(jsonID, "First entity ID was null: whyResult=[ "
                         + whyResult + " ], " + testData);

        long whyID1 = jsonID ?? 0L;

        jsonID = whyResult?["ENTITY_ID_2"]?.GetValue<long>();

        Assert.IsNotNull(jsonID, "Second entity ID was null: whyResult=[ "
                         + whyResult + " ], " + testData);

        long whyID2 = jsonID ?? 0L;

        ISet<long> entityIDs = new SortedSet<long>();
        entityIDs.Add(whyID1);
        entityIDs.Add(whyID2);

        Assert.IsTrue(entityIDs.Contains(entityID1),
                      "First entity ID not found in why result: whyResult=[ "
                      + whyResult + " ], " + testData);

        Assert.IsTrue(entityIDs.Contains(entityID2),
                      "Second entity ID not found in why result: whyResult=[ "
                      + whyResult + " ], " + testData);

        JsonArray? entities = jsonObject?["ENTITIES"]?.AsArray();

        Assert.IsNotNull(entities,
                         "Entity details are missing: " + testData);

        Assert.That(entities?.Count, Is.EqualTo(entityIDs.Count),
            "Unexpected number of entities in entity details. testData=[ "
            + testData + " ]");

        // check that the entities we found are those requested
        ISet<long> detailEntityIDs = new HashSet<long>();

        int count = entities?.Count ?? 0;
        for (int index = 0; index < count; index++)
        {
            JsonObject? entity = entities?[index]?.AsObject();

            Assert.IsNotNull(entity, "Entity detail was null: "
                             + entities + ", " + testData);

            entity = entity?["RESOLVED_ENTITY"]?.AsObject();

            Assert.IsNotNull(entity, "Resolved entity in details was null: "
                             + entities + ", " + testData);

            // get the entity ID
            long? id = entity?["ENTITY_ID"]?.GetValue<long>();
            Assert.IsNotNull(
                id, "The entity detail was missing or has a null "
                + "ENTITY_ID: " + entity + ", " + testData);

            // add to the ID set
            detailEntityIDs.Add(id ?? 0L); // id cannot be null
        }

        Assert.IsTrue(entityIDs.SetEquals(detailEntityIDs),
                      "Entity detail entity ID's are not as expected: "
                      + testData);
    }

    [Test, TestCaseSource(nameof(GetWhyEntitiesParameters))]
    public void TestWhyEntities(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey1,
        Func<SzCoreEngineWhyTest, long> entityIDFunc1,
        (string dataSourceCode, string recordID) recordKey2,
        Func<SzCoreEngineWhyTest, long> entityIDFunc2,
        SzFlag? flags,
        Type? exceptionType)
    {
        long entityID1 = entityIDFunc1(this);
        long entityID2 = entityIDFunc2(this);

        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], recordKey1=[ "
            + recordKey1 + " ], entityID1=[ " + entityID1
            + " ], recordKey2=[ " + recordKey2 + " ], entityID2=[ "
            + entityID2 + " ], flags=[ " + SzWhyFlags.FlagsToString(flags)
            + " ], expectedException=[ " + exceptionType + " ]");

        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.WhyEntities(entityID1, entityID2, flags);

                if (exceptionType != null)
                {
                    Fail("Unexpectedly succeeded whyEntities(): " + testData);
                }

                this.ValidateWhyEntities(result,
                                         testData,
                                         recordKey1,
                                         entityID1,
                                         recordKey2,
                                         entityID2,
                                         flags);

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
                    Fail("Unexpectedly failed whyEntities(): "
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

    public static List<object?[]> GetWhyRecordInEntityParameters()
    {
        List<object?[]> result = new List<object?[]>();

        Iterator<SzFlag?> flagSetIter
            = GetCircularIterator(WhyRecordInEntityFlagSets);

        Type? NotFound = typeof(SzNotFoundException);
        Type? UnknownSource = typeof(SzUnknownDataSourceException);

        foreach ((string, string) recordKey in RecordKeys)
        {
            result.Add(new object?[] {
                "Why " + recordKey + " in Entity",
                recordKey,
                flagSetIter.Next(),
                null});
        }

        result.Add(new object?[] {
            "Unknown data source code test",
            (UnknownDataSource, "ABC123"),
            flagSetIter.Next(),
            UnknownSource});

        result.Add(new object?[] {
            "Not found record ID test",
            (PassengersDataSource, "XXX000"),
            flagSetIter.Next(),
            NotFound});

        return result;
    }

    public void ValidateWhyRecordInEntity(
        string whyResultJson,
        string testData,
        (string dataSourceCode, string recordID) recordKey,
        SzFlag? flags)
    {
        JsonObject? jsonObject = JsonNode.Parse(whyResultJson)?.AsObject();

        JsonArray? whyResults = jsonObject?["WHY_RESULTS"]?.AsArray();

        Assert.IsNotNull(whyResults,
            "Missing WHY_RESULTS from whyEntities() result JSON: " + testData);

        Assert.That(whyResults?.Count, Is.EqualTo(1),
            "The WHY_RESULTS array is not of the expected size: " + testData);

        JsonObject? whyResult = whyResults?[0]?.AsObject();

        Assert.IsNotNull(whyResult,
            "First WHY_RESULTS element was null: " + testData);

        long? jsonID = whyResult?["ENTITY_ID"]?.GetValue<long>();

        Assert.IsNotNull(jsonID, "The entity ID was null: whyResult=[ "
                         + whyResult + " ], " + testData);

        long whyID = jsonID ?? 0L;

        long entityID = this.GetEntityID(recordKey);

        Assert.That(whyID, Is.EqualTo(entityID),
            "The entity ID in the why result was not as expected: "
            + "whyResult=[ " + whyResult + " ], " + testData);

        JsonArray? entities = jsonObject?["ENTITIES"]?.AsArray();

        Assert.IsNotNull(entities,
                         "Entity details are missing: " + testData);

        Assert.That(entities?.Count, Is.EqualTo(1),
            "Unexpected number of entities in entity details. testData=[ "
            + testData + " ]");

        JsonArray? focusRecords = whyResult?["FOCUS_RECORDS"]?.AsArray();

        Assert.IsNotNull(focusRecords, "The FOCUS_RECORDS array is missing "
            + "from the why results: whyResult=[ " + whyResult + " ], "
            + testData);

        Assert.That(focusRecords?.Count, Is.EqualTo(1),
            "Size of FOCUS_RECORDS array not as expected: focusRecords=[ "
            + focusRecords + " ], " + testData);

        JsonObject? focusRecord = focusRecords?[0]?.AsObject();
        Assert.IsNotNull(focusRecords, "The first element of FOCUS_RECORDS array "
            + "is missing or null: focusRecords=[ " + focusRecords + " ], "
            + testData);

        string? dataSource = focusRecord?["DATA_SOURCE"]?.GetValue<string>();
        string? recordID = focusRecord?["RECORD_ID"]?.GetValue<string>();

        Assert.IsNotNull(dataSource,
            "Focus record data source is missing or null: focusRecord=[ "
            + focusRecord + " ], " + testData);

        Assert.IsNotNull(recordID,
            "Focus record ID is missing or null: focusRecord=[ "
            + focusRecord + " ], " + testData);

        Assert.That(dataSource, Is.EqualTo(recordKey.dataSourceCode),
            "Focus record data source is not as expected: focusRecord=[ "
            + focusRecord + " ], " + testData);

        Assert.That(recordID, Is.EqualTo(recordKey.recordID),
            "Focus record ID is not as expected: focusRecord=[ "
            + focusRecord + " ], " + testData);

        // check that the entities we found are those requested
        ISet<long> detailEntityIDs = new HashSet<long>();
        int count = entities?.Count ?? 0;
        for (int index = 0; index < count; index++)
        {
            JsonObject? entity = entities?[index]?.AsObject();

            Assert.IsNotNull(entity, "Entity detail was null: "
                             + entities + ", " + testData);

            entity = entity?["RESOLVED_ENTITY"]?.AsObject();

            Assert.IsNotNull(entity, "Resolved entity in details was null: "
                             + entities + ", " + testData);

            // get the entity ID
            long? id = entity?["ENTITY_ID"]?.GetValue<long>();

            Assert.IsNotNull(
                id, "The entity detail was missing or has a null "
                + "ENTITY_ID: " + entity + ", " + testData);

            // add to the ID set
            detailEntityIDs.Add(id ?? 0L);
        }

        Assert.IsTrue(detailEntityIDs.SetEquals(SetOf(entityID)),
            "Entity detail entity ID's are not as expected: " + testData);
    }

    [Test, TestCaseSource(nameof(GetWhyRecordInEntityParameters))]
    public void TestWhyRecordInEntity(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey,
        SzFlag? flags,
        Type? exceptionType)
    {
        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], recordKey=[ "
            + recordKey + " ], flags=[ " + SzWhyFlags.FlagsToString(flags)
            + " ], expectedException=[ " + exceptionType + " ]");

        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.WhyRecordInEntity(
                    recordKey.dataSourceCode, recordKey.recordID, flags);

                if (exceptionType != null)
                {
                    Fail("Unexpectedly succeeded whyRecordInEntity(): "
                         + testData);
                }

                this.ValidateWhyRecordInEntity(
                    result, testData, recordKey, flags);

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
                    Fail("Unexpectedly failed whyRecordInEntity(): "
                         + testData + ", " + description, e);

                }
                else if (exceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        exceptionType, e,
                        "whyRecordInEntity() failed with an unexpected "
                        + "exception type: " + testData + ", " + description);
                }
            }
        });
    }

    public static List<object?[]> GetWhyRecordsParameters()
    {
        List<object?[]> result = new List<object?[]>();

        ArrayList recordKeys = new ArrayList(RecordKeys.Count);
        foreach ((string, string) recordKey in RecordKeys)
        {
            recordKeys.Add(recordKey);
        }

        IList<System.Collections.IList> recordKeyCombos
            = GenerateCombinations(recordKeys, recordKeys);

        IEnumerator<System.Collections.IList> ienum
            = recordKeyCombos.GetEnumerator();

        IList<((string, string), (string, string))> comboTuples
            = new List<((string, string), (string, string))>(recordKeyCombos.Count);

        while (ienum.MoveNext())
        {
            System.Collections.IList list = ienum.Current;

            (string, string) key1 = ((string, string)?)list[0] ?? ("A", "B");
            (string, string) key2 = ((string, string)?)list[1] ?? ("C", "D");

            // thin the list out to reduce the number of tests
            if (!key1.Equals(key2))
            {
                int index1 = RecordKeys.IndexOf(key1);
                int index2 = RecordKeys.IndexOf(key2);
                if (Math.Abs(index2 - index1) <= 4)
                {
                    comboTuples.Add((key1, key2));
                }
            }
        }

        Iterator<SzFlag?> flagSetIter = GetCircularIterator(WhyRecordsFlagSets);

        Type? NotFound = typeof(SzNotFoundException);
        Type? UnknownSource = typeof(SzUnknownDataSourceException);

        foreach (((string, string), (string, string)) keyPair in comboTuples)
        {
            (string, string) recordKey1 = keyPair.Item1;
            (string, string) recordKey2 = keyPair.Item2;

            result.Add(new object?[] {
                "Why " + recordKey1 + " versus " + recordKey2,
                recordKey1,
                recordKey2,
                flagSetIter.Next(),
                null});
        }

        result.Add(new object?[] {
            "Why records with same record twice: " + RecordKeys[0],
            RecordKeys[0],
            RecordKeys[0],
            flagSetIter.Next(),
            null});

        result.Add(new object?[] {
            "Unknown data source code test",
            (UnknownDataSource, "ABC123"),
            DEF890,
            flagSetIter.Next(),
            UnknownSource});

        result.Add(new object?[] {
            "Not found record ID test",
            (PassengersDataSource, "XXX000"),
            DEF890,
            flagSetIter.Next(),
            NotFound});

        return result;
    }

    public void ValidateWhyRecords(
        string whyResultJson,
        string testData,
        (string dataSourceCode, string recordID) recordKey1,
        (string dataSourceCode, string recordID) recordKey2,
        SzFlag? flags)
    {
        JsonObject? jsonObject = JsonNode.Parse(whyResultJson)?.AsObject();

        JsonArray? whyResults = jsonObject?["WHY_RESULTS"]?.AsArray();

        Assert.IsNotNull(whyResults,
            "Missing WHY_RESULTS from whyRecords() result JSON: " + testData);

        Assert.That(whyResults?.Count, Is.EqualTo(1),
            "The WHY_RESULTS array is not of the expected size: " + testData);

        JsonObject? whyResult = whyResults?[0]?.AsObject();

        Assert.IsNotNull(whyResult,
            "First WHY_RESULTS element was null: " + testData);

        long? whyID1 = whyResult?["ENTITY_ID"]?.GetValue<long>();
        long? whyID2 = whyResult?["ENTITY_ID_2"]?.GetValue<long>();

        Assert.IsNotNull(whyID1, "The first entity ID was null: whyResult=[ "
                         + whyResult + " ], " + testData);
        Assert.IsNotNull(whyID2, "The second entity ID was null: whyResult=[ "
                         + whyResult + " ], " + testData);

        ISet<long> whyIDs = SortedSetOf(whyID1 ?? 0L, whyID2 ?? 0L);

        long entityID1 = this.GetEntityID(recordKey1);
        long entityID2 = this.GetEntityID(recordKey2);

        ISet<long> entityIDs = SortedSetOf(entityID1, entityID2);

        Assert.IsTrue(whyIDs.SetEquals(entityIDs),
            "The entity ID's in the why result were not as expected: "
            + "whyResult=[ " + whyResult + " ], " + testData);

        JsonArray? entities = jsonObject?["ENTITIES"]?.AsArray();

        Assert.IsNotNull(entities,
                         "Entity details are missing: " + testData);

        Assert.That(entities?.Count, Is.EqualTo(entityIDs.Count),
            "Unexpected number of entities in entity details. testData=[ "
            + testData + " ]");

        JsonArray? focusRecords1 = whyResult?["FOCUS_RECORDS"]?.AsArray();

        Assert.IsNotNull(focusRecords1, "The FOCUS_RECORDS array is missing "
            + "from the why results: whyResult=[ " + whyResult + " ], "
            + testData);

        Assert.That(focusRecords1?.Count, Is.EqualTo(1),
            "Size of FOCUS_RECORDS array not as expected: focusRecord1=[ "
            + focusRecords1 + " ], " + testData);

        JsonObject? focusRecord1 = focusRecords1?[0]?.AsObject();

        Assert.IsNotNull(focusRecords1, "The first element of FOCUS_RECORDS array "
            + "is missing or null: focusRecord1=[ " + focusRecords1 + " ], "
            + testData);

        string? dataSource1 = focusRecord1?["DATA_SOURCE"]?.GetValue<string>();
        string? recordID1 = focusRecord1?["RECORD_ID"]?.GetValue<string>();

        Assert.IsNotNull(dataSource1,
            "Focus record data source is missing or null: focusRecord=[ "
            + focusRecord1 + " ], " + testData);

        Assert.IsNotNull(recordID1,
            "Focus record ID is missing or null: focusRecord=[ "
            + focusRecord1 + " ], " + testData);

        JsonArray? focusRecords2 = whyResult?["FOCUS_RECORDS_2"]?.AsArray();

        Assert.IsNotNull(focusRecords2, "The FOCUS_RECORDS_2 array is missing "
            + "from the why results: whyResult=[ " + whyResult + " ], "
            + testData);

        Assert.That(focusRecords2?.Count, Is.EqualTo(1),
            "Size of FOCUS_RECORDS_2 array not as expected: focusRecords2=[ "
            + focusRecords2 + " ], " + testData);

        JsonObject? focusRecord2 = focusRecords2?[0]?.AsObject();

        Assert.IsNotNull(focusRecords2, "The first element of FOCUS_RECORDS_2 "
            + "array is missing or null: focusRecords=[ " + focusRecords2
            + " ], " + testData);

        string? dataSource2 = focusRecord2?["DATA_SOURCE"]?.GetValue<string>();
        string? recordID2 = focusRecord2?["RECORD_ID"]?.GetValue<string>();

        Assert.IsNotNull(dataSource2,
            "Focus record data source is missing or null: focusRecord=[ "
            + focusRecord2 + " ], " + testData);

        Assert.IsNotNull(recordID2,
            "Focus record ID is missing or null: focusRecord=[ "
            + focusRecord2 + " ], " + testData);

        (string, string) whyKey1 = (dataSource1 ?? "", recordID1 ?? "");
        (string, string) whyKey2 = (dataSource2 ?? "", recordID2 ?? "");

        ISet<(string, string)> whyKeys = SortedSetOf(whyKey1, whyKey2);
        ISet<(string, string)> recordKeys = SortedSetOf(recordKey1, recordKey2);

        Assert.IsTrue(whyKeys.SetEquals(recordKeys),
            "Focus records not as expected: focusRecord=[ "
            + focusRecord1 + " ], " + testData);

        // check that the entities we found are those requested
        ISet<long> detailEntityIDs = new HashSet<long>();
        int count = entities?.Count ?? 0;
        for (int index = 0; index < count; index++)
        {
            JsonObject? entity = entities?[index]?.AsObject();

            Assert.IsNotNull(entity, "Entity detail was null: "
                             + entities + ", " + testData);

            entity = entity?["RESOLVED_ENTITY"]?.AsObject();

            Assert.IsNotNull(entity, "Resolved entity in details was null: "
                             + entities + ", " + testData);

            // get the entity ID
            long? id = entity?["ENTITY_ID"]?.GetValue<long>();

            Assert.IsNotNull(
                id, "The entity detail was missing or has a null "
                + "ENTITY_ID: " + entity + ", " + testData);

            // add to the ID set
            detailEntityIDs.Add(id ?? 0L);
        }

        Assert.IsTrue(detailEntityIDs.SetEquals(entityIDs),
            "Entity detail entity ID's are not as expected: " + testData);
    }

    [Test, TestCaseSource(nameof(GetWhyRecordsParameters))]
    public void TestWhyRecords(
        string testDescription,
        (string dataSourceCode, string recordID) recordKey1,
        (string dataSourceCode, string recordID) recordKey2,
        SzFlag? flags,
        Type? exceptionType)
    {
        StringBuilder sb = new StringBuilder(
            "description=[ " + testDescription + " ], recordKey1=[ "
            + recordKey1 + " ], recordKey2=[ " + recordKey2
            + " ], flags=[ " + SzWhyFlags.FlagsToString(flags)
            + " ], expectedException=[ " + exceptionType + " ]");

        string testData = sb.ToString();

        this.PerformTest(() =>
        {
            try
            {
                SzEngine engine = this.Env.GetEngine();

                string result = engine.WhyRecords(
                    recordKey1.dataSourceCode,
                    recordKey1.recordID,
                    recordKey2.dataSourceCode,
                    recordKey2.recordID,
                    flags);

                if (exceptionType != null)
                {
                    Fail("Unexpectedly succeeded whyRecords(): "
                         + testData);
                }

                this.ValidateWhyRecords(
                    result, testData, recordKey1, recordKey2, flags);

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
                    Fail("Unexpectedly failed whyRecords(): "
                         + testData + ", " + description, e);

                }
                else if (exceptionType != e.GetType())
                {
                    Assert.IsInstanceOf(
                        exceptionType, e,
                        "whyRecords() failed with an unexpected exception "
                        + "type: " + testData + ", " + description);
                }
            }
        });

    }
}
