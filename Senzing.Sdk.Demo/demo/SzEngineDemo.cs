namespace Senzing.Sdk.Demo;

using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzEngineDemo : AbstractTest
{
    private const string EmployeesDataSource = "EMPLOYEES";

    private const string PassengersDataSource = "PASSENGERS";

    private const string VipsDataSource = "VIPS";

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

    private long addedEntityID = 0L;

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

        try
        {
            SzEngine engine = env.GetEngine();

            // get the loaded records and entity ID's
            foreach ((string dataSourceCode, string recordID) key in GraphRecordKeys)
            {
                string responseJson = engine.GetEntity(key.dataSourceCode, key.recordID, null);

                // parse the JSON 
                JsonObject? jsonObj = JsonNode.Parse(responseJson)?.AsObject();
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

        }
        catch (SzException e)
        {
            Fail(e);
        }
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

    /// <summary>Dummy logging function</summary>
    /// 
    /// <param name="message">The message to log.</param>
    protected static void Log(string message)
    {
        if (message == null)
        {
            throw new ArgumentNullException("message");
        }
    }

    [Test]
    public void GetEngineDemo()
    {
        try
        {
            // @start GetEngine
            // How to obtain an SzEngine instance
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                Assert.That(engine, Is.Not.Null); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get SzEngine.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void PrimeEngineDemo()
    {
        try
        {
            // @start PrimeEngine
            // How to prime the SzEngine to expedite subsequent operations
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // prime the engine
                engine.PrimeEngine();

                // use the primed engine to perform additional tasks
                Assert.That("" + engine, Is.Not.EqualTo("")); // @replace . . .

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to prime engine.", e);
            }
            // @end

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetStatsDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetStats
            // How to get engine stats after loading records
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // load some records to prime the stats
                Assert.That("" + engine, Is.Not.EqualTo("")); // @replace . . .

                // get the stats
                string stats = engine.GetStats();
                demoResult = stats; // @omit

                // do something with the stats
                Log(stats);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to load records with stats.", e);
            }
            // @end
            this.saveDemoResult("GetStats", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }


    [Test, Order(20)]
    public void AddRecordDemo()
    {
        try
        {
            string demoResult = "";
            // @start AddRecord
            // How to load a record
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get a record definition (varies by application)
                string recordDefinition =
                        """
                        {
                            "DATA_SOURCE": "TEST",
                            "RECORD_ID": "ABC123",
                            "NAME_FULL": "Joe Schmoe",
                            "PHONE_NUMBER": "702-555-1212",
                            "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
                        }
                        """;

                // add the record to the repository
                string info = engine.AddRecord(
                    "TEST", "ABC123", recordDefinition, SzWithInfo);
                demoResult = info; // @omit

                // do something with the "info JSON" (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(info)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("AFFECTED_ENTITIES"))
                {
                    JsonArray? affectedArr = jsonObject["AFFECTED_ENTITIES"]?.AsArray();
                    for (int index = 0; affectedArr != null && index < affectedArr.Count; index++)
                    {
                        JsonObject? affected = affectedArr[index]?.AsObject();
                        long affectedID = affected?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        this.addedEntityID = affectedID; // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to add record.", e);
            }
            // @end
            this.saveDemoResult("AddRecord", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetRecordPreviewDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetRecordPreview
            // How to get a record preview
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get a record definition (varies by application)
                string recordDefinition =
                        """
                        {
                            "DATA_SOURCE": "TEST",
                            "RECORD_ID": "DEF456",
                            "NAME_FULL": "John Doe",
                            "PHONE_NUMBER": "702-555-1212",
                            "EMAIL_ADDRESS": "johndoe@nowhere.com"
                        }
                        """;

                // get the record preview
                String preview = engine.GetRecordPreview(
                    recordDefinition, SzRecordPreviewDefaultFlags);
                demoResult = preview; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(preview)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("FEATURES"))
                {
                    JsonObject? featuresObj = jsonObject["FEATURES"]?.AsObject();

                    if (featuresObj != null)
                    {
                        foreach (KeyValuePair<string, JsonNode?> pair in featuresObj)
                        {
                            string featureName = pair.Key;
                            Assert.That(featureName.Length, Is.Not.EqualTo(0)); // @replace . . .
                        }
                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to get a record preview.", e);
            }
            // @end
            this.saveDemoResult("GetRecordPreview", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test, Order(10)]
    public void DeleteRecordDemo()
    {
        try
        {
            string recordDefinition =
            """
            {
                "DATA_SOURCE": "TEST",
                "RECORD_ID": "ABC123",
                "NAME_FULL": "Joe Schmoe",
                "PHONE_NUMBER": "702-555-1212",
                "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
            }
            """;

            // add the record to the repository
            GetEnvironment().GetEngine().AddRecord(
                "TEST", "ABC123", recordDefinition);

            string demoResult = "";
            // @start DeleteRecord
            // How to delete a record
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // delete the record from the repository
                string info = engine.DeleteRecord("TEST", "ABC123", SzWithInfo);
                demoResult = info; // @omit

                // do something with the "info JSON" (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(info)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("AFFECTED_ENTITIES"))
                {
                    JsonArray? affectedArr = jsonObject["AFFECTED_ENTITIES"]?.AsArray();

                    for (int index = 0; affectedArr != null && index < affectedArr.Count; index++)
                    {
                        JsonObject? affected = affectedArr[index]?.AsObject();

                        long affectedID = affected?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        Assert.That(affectedID > 0); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to delete record.", e);
            }
            // @end
            this.saveDemoResult("DeleteRecord", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void ReevaluateRecordDemo()
    {
        try
        {
            string demoResult = "";
            // @start ReevaluateRecord
            // How to reevaluate a record
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // reevaluate a record in the repository
                string info = engine.ReevaluateRecord("TEST", "ABC123", SzWithInfo);
                demoResult = info; // @omit

                // do something with the "info JSON" (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(info)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("AFFECTED_ENTITIES"))
                {
                    JsonArray? affectedArr = jsonObject["AFFECTED_ENTITIES"]?.AsArray();

                    for (int index = 0; affectedArr != null && index < affectedArr.Count; index++)
                    {
                        JsonObject? affected = affectedArr[index]?.AsObject();

                        long affectedID = affected?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        Assert.That(affectedID > 0L); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to reevaluate record.", e);
            }
            // @end
            this.saveDemoResult("ReevaluateRecord", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    /// <summary>
    /// Dummy function to return an entity ID.
    /// </summary>
    /// 
    /// <returns>An arbitrary entity ID.</returns>
    private long GetEntityID()
    {
        return this.addedEntityID;
    }

    [Test]
    public void ReevaluateEntityDemo()
    {
        try
        {
            string demoResult = "";
            // @start ReevaluateEntity
            // How to reevaluate an entity
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the ID of an entity to reevaluate (varies by application)
                long entityID = GetEntityID();

                // reevaluate an entity in the repository
                string info = engine.ReevaluateEntity(entityID, SzWithInfo);
                demoResult = info; // @omit

                // do something with the "info JSON" (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(info)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("AFFECTED_ENTITIES"))
                {
                    JsonArray? affectedArr = jsonObject["AFFECTED_ENTITIES"]?.AsArray();

                    for (int index = 0; affectedArr != null && index < affectedArr.Count; index++)
                    {
                        JsonObject? affected = affectedArr[index]?.AsObject();

                        long affectedID = affected?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        Assert.That(affectedID > 0); // @replace . . .
                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to reevaluate entity.", e);
            }
            // @end
            this.saveDemoResult("ReevaluateEntity", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void SearchByAttributesDemo()
    {
        try
        {
            string demoResult = "";
            // @start SearchByAttributes
            // How to search for entities matching criteria
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the search attributes (varies by application)
                string searchAttributes =
                        """
                        {
                            "NAME_FULL": "Joe Schmoe",
                            "PHONE_NUMBER": "702-555-1212",
                            "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
                        }
                        """;

                // search for matching entities in the repository                
                string results = engine.SearchByAttributes(
                    searchAttributes,
                    SzSearchByAttributesDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("RESOLVED_ENTITIES"))
                {
                    JsonArray? resultsArr = jsonObject["RESOLVED_ENTITIES"]?.AsArray();

                    for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                    {
                        JsonObject? result = resultsArr[index]?.AsObject();

                        // get the match info for the result
                        JsonObject? matchInfo = result?["MATCH_INFO"]?.AsObject();

                        Assert.That(matchInfo, Is.Not.Null);  // @replace . . .

                        // get the entity for the result
                        JsonObject? entityInfo = result?["ENTITY"]?.AsObject();
                        JsonObject? entity = entityInfo?["RESOLVED_ENTITY"]?.AsObject();
                        long entityID = entity?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0);  // @replace . . .
                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to search for entities.", e);
            }
            // @end
            this.saveDemoResult("SearchByAttributes", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    /// <summary>
    /// Function to return a dummy search profile.
    /// </summary>
    /// 
    /// <returns>Returns the <c>"SEARCH"</c> profile.</returns>
    private string GetSearchProfile()
    {
        return "SEARCH";
    }

    [Test]
    public void SearchByAttributesWithProfileDemo()
    {
        try
        {
            string demoResult = "";
            // @start SearchByAttributesWithProfile
            // How to search for entities matching criteria using a search profile
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the search attributes (varies by application)
                string searchAttributes =
                        """
                        {
                            "NAME_FULL": "Joe Schmoe",
                            "PHONE_NUMBER": "702-555-1212",
                            "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
                        }
                        """;

                // get a search profile (varies by application)
                string searchProfile = GetSearchProfile();

                // search for matching entities in the repository                
                string results = engine.SearchByAttributes(
                    searchAttributes,
                    searchProfile,
                    SzSearchByAttributesDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                if (jsonObject != null && jsonObject.ContainsKey("RESOLVED_ENTITIES"))
                {
                    JsonArray? resultsArr = jsonObject["RESOLVED_ENTITIES"]?.AsArray();

                    for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                    {
                        JsonObject? result = resultsArr[index]?.AsObject();

                        // get the match info for the result
                        JsonObject? matchInfo = result?["MATCH_INFO"]?.AsObject();

                        Assert.That(matchInfo, Is.Not.Null);  // @replace . . .

                        // get the entity for the result
                        JsonObject? entityInfo = result?["ENTITY"]?.AsObject();
                        JsonObject? entity = entityInfo?["RESOLVED_ENTITY"]?.AsObject();
                        long entityID = entity?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0);  // @replace . . .
                    }
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to search for entities.", e);
            }
            // @end
            this.saveDemoResult("SearchByAttributesWithProfile", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetEntityByEntityIDDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetEntityByEntityID
            // How to retrieve an entity via its entity ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the ID of an entity to retrieve (varies by application)
                long entityID = GetEntityID();

                // retrieve the entity by entity ID                
                string result = engine.GetEntity(entityID, SzEntityDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();
                string? entityName = entity?["ENTITY_NAME"]?.GetValue<string>();

                Assert.That(entityName != "FOO"); // @replace . . .

                if (jsonObject != null && jsonObject.ContainsKey("RECORDS"))
                {

                    JsonArray? recordArr = jsonObject["RECORDS"]?.AsArray();

                    for (int index = 0; recordArr != null && index < recordArr.Count; index++)
                    {
                        JsonObject? record = recordArr[index]?.AsObject();

                        string? dataSource = record?["DATA_SOURCE"]?.GetValue<string>();
                        string? recordID = record?["RECORD_ID"]?.GetValue<string>();

                        Assert.That(dataSource != null && recordID != null); // @replace . . .
                    }
                }

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity by entity ID.", e);
            }
            // @end
            this.saveDemoResult("GetEntityByEntityID", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetEntityByRecordKeyDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetEntityByRecordKey
            // How to retrieve an entity via its record key
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // retrieve the entity by record key
                string result = engine.GetEntity("TEST", "ABC123", SzEntityDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();
                string? entityName = entity?["ENTITY_NAME"]?.GetValue<string>();

                Assert.That(entityName != "FOO"); // @replace . . .

                if (jsonObject != null && jsonObject.ContainsKey("RECORDS"))
                {

                    JsonArray? recordArr = jsonObject["RECORDS"]?.AsArray();

                    for (int index = 0; recordArr != null && index < recordArr.Count; index++)
                    {
                        JsonObject? record = recordArr[index]?.AsObject();

                        string? dataSource = record?["DATA_SOURCE"]?.GetValue<string>();
                        string? recordID = record?["RECORD_ID"]?.GetValue<string>();

                        Assert.That(dataSource != null && recordID != null); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity by record key.", e);
            }
            // @end
            this.saveDemoResult("GetEntityByRecordKey", demoResult, true);

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void FindInterestingByEntityIDDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindInterestingByEntityID
            // How to find interesting entities related to an entity via entity ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the ID of an entity to retrieve (varies by application)
                long entityID = GetEntityID();

                // find the interesting entities by entity ID
                string result = engine.FindInterestingEntities(
                    entityID, SzFindInterestingEntitiesDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                Assert.That(jsonObject, Is.Not.Null); // @replace . . .

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to find interesting entities by entity ID.", e);
            }
            // @end
            this.saveDemoResult("FindInterestingByEntityID", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void FindInterestingByRecordKeyDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindInterestingByRecordKey
            // How to find interesting entities related to an entity via record key
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // retrieve the entity by record key                
                string result = engine.FindInterestingEntities(
                    "TEST", "ABC123", SzFindInterestingEntitiesDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();

                Assert.That(jsonObject, Is.Not.Null); // @replace . . .

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to find interesting entities by record key.", e);
            }
            // @end
            this.saveDemoResult("FindInterestingByRecordKey", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }


    private (string, string) GetPathStartRecordKey()
    {
        return PassengerABC123;
    }

    private (string, string) GetPathEndRecordKey()
    {
        return VipJKL456;
    }

    private long GetPathStartEntityID()
    {
        return this.LoadedRecordMap[this.GetPathStartRecordKey()];
    }

    private long GetPathEndEntityID()
    {
        return this.LoadedRecordMap[this.GetPathEndRecordKey()];
    }

    private ISet<(string, string)> GetPathAvoidRecordKeys()
    {
        return ImmutableHashSet.Create<(string, string)>(EmployeeDEF890);
    }

    private ISet<long> GetPathAvoidEntityIDs()
    {
        HashSet<long> set = new HashSet<long>();
        foreach ((string, string) key in this.GetPathAvoidRecordKeys())
        {
            set.Add(this.LoadedRecordMap[key]);
        }
        return set.ToImmutableHashSet();
    }

    [Test]
    public void FindPathByEntityIDDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindPathByEntityID
            // How to find an entity path using entity ID's
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get entity ID's for the path endpoints (varies by application)
                long startEntityID = GetPathStartEntityID();
                long endEntityID = GetPathEndEntityID();

                // determine the maximum path degrees (varies by application)
                int maxDegrees = 4;

                // determine any entities to be avoided (varies by application)
                ISet<long> avoidEntities = GetPathAvoidEntityIDs();

                // determine any data sources to require in the path (varies by application)
                ISet<string>? requiredSources = null;

                // retrieve the entity path using the entity ID's
                string result = engine.FindPath(startEntityID,
                                                endEntityID,
                                                maxDegrees,
                                                avoidEntities,
                                                requiredSources,
                                                SzFindPathDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonArray? pathArr = jsonObject?["ENTITY_PATHS"]?.AsArray();

                for (int index = 0; pathArr != null && index < pathArr.Count; index++)
                {
                    JsonObject? path = pathArr[index]?.AsObject();
                    JsonArray? entityIDs = path?["ENTITIES"]?.AsArray();

                    for (int index2 = 0; entityIDs != null && index2 < entityIDs.Count; index2++)
                    {
                        long entityID = entityIDs[index2]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0L); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity path by entity ID.", e);
            }
            // @end
            this.saveDemoResult("FindPathByEntityID", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void FindPathByRecordKeyDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindPathByRecordKey
            // How to find an entity path using record keys
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get entity ID's for the path endpoints (varies by application)
                (string dataSourceCode, string recordID) startRecordKey = GetPathStartRecordKey();
                (string dataSourceCode, string recordID) endRecordKey = GetPathEndRecordKey();

                // determine the maximum path degrees (varies by application)
                int maxDegrees = 4;

                // determine any records to be avoided (varies by application)
                ISet<(string, string)> avoidRecords = GetPathAvoidRecordKeys();

                // determine any data sources to require in the path (varies by application)
                ISet<string>? requiredSources = null;

                // retrieve the entity path using the record keys
                string result = engine.FindPath(startRecordKey.dataSourceCode,
                                                startRecordKey.recordID,
                                                endRecordKey.dataSourceCode,
                                                endRecordKey.recordID,
                                                maxDegrees,
                                                avoidRecords,
                                                requiredSources,
                                                SzFindPathDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonArray? pathArr = jsonObject?["ENTITY_PATHS"]?.AsArray();

                for (int index = 0; pathArr != null && index < pathArr.Count; index++)
                {
                    JsonObject? path = pathArr[index]?.AsObject();
                    JsonArray? entityIDs = path?["ENTITIES"]?.AsArray();

                    for (int index2 = 0; entityIDs != null && index2 < entityIDs.Count; index2++)
                    {
                        long entityID = entityIDs[index2]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0L); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity path by record key.", e);
            }
            // @end
            this.saveDemoResult("FindPathByRecordKey", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private ISet<(string, string)> GetNetworkRecordKeys()
    {
        return ImmutableHashSet.Create<(string, string)>(
            new (string, string)[] { PassengerABC123, EmployeeABC567, VipJKL456 });
    }

    private ISet<long> GetNetworkEntityIDs()
    {
        HashSet<long> set = new HashSet<long>();
        foreach ((string, string) key in this.GetNetworkRecordKeys())
        {
            set.Add(this.LoadedRecordMap[key]);
        }
        return set.ToImmutableHashSet();
    }

    [Test]
    public void FindNetworkByEntityIDDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindNetworkByEntityID
            // How to find an entity network using entity ID's
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get entity ID's for the path endpoints (varies by application)
                ISet<long> entityIDs = GetNetworkEntityIDs();

                // determine the maximum path degrees (varies by application)
                int maxDegrees = 3;

                // determine the degrees to build-out the network (varies by application)
                int buildOutDegrees = 0;

                // determine the max entities to build-out (varies by application)
                int buildOutMaxEntities = 10;

                // retrieve the entity network using the entity ID's
                string result = engine.FindNetwork(entityIDs,
                                                   maxDegrees,
                                                   buildOutDegrees,
                                                   buildOutMaxEntities,
                                                   SzFindNetworkDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonArray? pathArr = jsonObject?["ENTITY_PATHS"]?.AsArray();

                for (int index = 0; pathArr != null && index < pathArr.Count; index++)
                {
                    JsonObject? path = pathArr[index]?.AsObject();

                    long startEntityID = path?["START_ENTITY_ID"]?.GetValue<long>() ?? 0L;
                    long endEntityID = path?["END_ENTITY_ID"]?.GetValue<long>() ?? 0L;
                    JsonArray? pathIDs = path?["ENTITIES"]?.AsArray();

                    for (int index2 = 0; pathIDs != null && index2 < pathIDs.Count; index2++)
                    {
                        long entityID = pathIDs[index2]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0L && startEntityID > 0L && endEntityID > 0L);  // @replace . . .
                    }
                }

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity network.", e);
            }
            // @end
            this.saveDemoResult("FindNetworkByEntityID", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void FindNetworkByRecordKeyDemo()
    {
        try
        {
            string demoResult = "";
            // @start FindNetworkByRecordKey
            // How to find an entity network using record keys
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get entity ID's for the path endpoints (varies by application)
                ISet<(string, string)> recordKeys = GetNetworkRecordKeys();

                // determine the maximum path degrees (varies by application)
                int maxDegrees = 3;

                // determine the degrees to build-out the network (varies by application)
                int buildOutDegrees = 0;

                // determine the max entities to build-out (varies by application)
                int buildOutMaxEntities = 10;

                // retrieve the entity network using the record keys
                string result = engine.FindNetwork(recordKeys,
                                                   maxDegrees,
                                                   buildOutDegrees,
                                                   buildOutMaxEntities,
                                                   SzFindNetworkDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonArray? pathArr = jsonObject?["ENTITY_PATHS"]?.AsArray();

                for (int index = 0; pathArr != null && index < pathArr.Count; index++)
                {
                    JsonObject? path = pathArr[index]?.AsObject();

                    long startEntityID = path?["START_ENTITY_ID"]?.GetValue<long>() ?? 0L;
                    long endEntityID = path?["END_ENTITY_ID"]?.GetValue<long>() ?? 0L;
                    JsonArray? pathIDs = path?["ENTITIES"]?.AsArray();

                    for (int index2 = 0; pathIDs != null && index2 < pathIDs.Count; index2++)
                    {
                        long entityID = pathIDs[index2]?.GetValue<long>() ?? 0L;

                        Assert.That(entityID > 0L && startEntityID > 0L && endEntityID > 0L);  // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve entity network.", e);
            }
            // @end
            this.saveDemoResult("FindNetworkByRecordKey", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void WhyRecordInEntityDemo()
    {
        try
        {
            string demoResult = "";
            // @start WhyRecordInEntity
            // How to determine why a record is a member of its respective entity
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // determine why the record is part of its entity                
                string results = engine.WhyRecordInEntity(
                    "TEST", "ABC123", SzWhyRecordInEntityDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                JsonArray? resultsArr = jsonObject?["WHY_RESULTS"]?.AsArray();

                for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                {
                    JsonObject? result = resultsArr[index]?.AsObject();

                    long entityID = result?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                    Assert.That(entityID > 0L);  // @replace . . .
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to reevaluate record.", e);
            }
            // @end
            this.saveDemoResult("WhyRecordInEntity", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private (string, string) GetWhyRecordsKey1()
    {
        return EmployeeMNO345;
    }

    private (string, string) GetWhyRecordsKey2()
    {
        return EmployeeDEF890;
    }

    [Test]
    public void WhyRecordsDemo()
    {
        try
        {
            string demoResult = "";
            // @start WhyRecords
            // How to determine how two records are related
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the records on which to operate (varies by application)
                (string dataSourceCode, string recordID) recordKey1 = GetWhyRecordsKey1();
                (string dataSourceCode, string recordID) recordKey2 = GetWhyRecordsKey2();

                // determine how the records are related
                string results = engine.WhyRecords(recordKey1.dataSourceCode,
                                                   recordKey1.recordID,
                                                   recordKey2.dataSourceCode,
                                                   recordKey2.recordID,
                                                   SzWhyRecordsDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                JsonArray? resultsArr = jsonObject?["WHY_RESULTS"]?.AsArray();

                for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                {
                    JsonObject? result = resultsArr[index]?.AsObject();

                    long entityID1 = result?["ENTITY_ID"]?.GetValue<long>() ?? 0L;
                    long entityID2 = result?["ENTITY_ID_2"]?.GetValue<long>() ?? 0L;

                    Assert.That(entityID1 > 0L && entityID2 > 0L); // @replace . . .
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to reevaluate record.", e);
            }
            // @end
            this.saveDemoResult("WhyRecords", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private long GetWhyEntitiesID1()
    {
        return this.LoadedRecordMap[this.GetWhyRecordsKey1()];
    }

    private long GetWhyEntitiesID2()
    {
        return this.LoadedRecordMap[this.GetWhyRecordsKey2()];
    }

    private long GetWhySearchEntityID()
    {
        return this.LoadedRecordMap[this.GetWhyRecordsKey1()];
    }

    [Test]
    public void WhySearchDemo()
    {
        try
        {
            string demoResult = "";
            // @start WhySearch
            // How to determine why an entity was excluded from search results
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the search attributes (varies by application)
                string searchAttributes =
                        """
                        {
                            "NAME_FULL": "Joe Schmoe",
                            "PHONE_NUMBER": "702-555-1212",
                            "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
                        }
                        """;

                // get the entities on which to operate (varies by application)
                long entityID = GetWhySearchEntityID();

                // determine how the entities are related                
                string results = engine.WhySearch(searchAttributes,
                                                  entityID,
                                                  null, // search profile
                                                  SzWhySearchDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)                
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                JsonArray? resultsArr = jsonObject?["WHY_RESULTS"]?.AsArray();

                for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                {
                    JsonObject? result = resultsArr[index]?.AsObject();

                    long whyEntityID1 = result?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                    Assert.That(whyEntityID1 > 0L); // @replace . . .
                }

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to perform why search.", e);
            }
            // @end
            this.saveDemoResult("WhySearch", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void WhyEntitiesDemo()
    {
        try
        {
            string demoResult = "";
            // @start WhyEntities
            // How to determine how two entities are related
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the entities on which to operate (varies by application)
                long entityID1 = GetWhyEntitiesID1();
                long entityID2 = GetWhyEntitiesID2();

                // determine how the entities are related
                string results = engine.WhyEntities(entityID1,
                                                    entityID2,
                                                    SzWhyEntitiesDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                JsonArray? resultsArr = jsonObject?["WHY_RESULTS"]?.AsArray();

                for (int index = 0; resultsArr != null && index < resultsArr.Count; index++)
                {
                    JsonObject? result = resultsArr[index]?.AsObject();

                    long whyEntityID1 = result?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                    long whyEntityID2 = result?["ENTITY_ID_2"]?.GetValue<long>() ?? 0L;

                    Assert.That(whyEntityID1 > 0L && whyEntityID2 > 0L); // @replace . . .
                }

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the exception
                LogError("Failed to perform why entities.", e);
            }
            // @end
            this.saveDemoResult("WhyEntities", demoResult, true);

        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void HowEntityDemo()
    {
        try
        {
            string recordDefinition =
            """
            {
                "DATA_SOURCE": "TEST",
                "RECORD_ID": "XYZ987",
                "NAME_FULL": "Joseph Schmoe",
                "WORK_PHONE_NUMBER": "702-555-1313",
                "ADDR_FULL": "101 Main St.; Las Vegas, NV 89101",
                "EMAIL_ADDRESS": "joeschmoe@nowhere.com"
            }
            """;

            GetEnvironment().GetEngine().AddRecord(
                "TEST", "XYZ987", recordDefinition);

            recordDefinition =
            """
            {
                "DATA_SOURCE": "TEST",
                "RECORD_ID": "ZYX789",
                "NAME_FULL": "Joseph W. Schmoe",
                "HOME_PHONE_NUMBER": "702-555-1212",
                "WORK_PHONE_NUMBER": "702-555-1313",
                "ADDR_FULL": "101 Main St.; Las Vegas, NV 89101"
            }
            """;

            GetEnvironment().GetEngine().AddRecord(
                "TEST", "ZYX789", recordDefinition);

            string demoResult = "";
            // @start HowEntity
            // How to retrieve the "how analysis" for an entity via its entity ID
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the entity ID on which to operate (varies by application)
                long entityID = GetEntityID();

                // determine how the entity was formed
                string results = engine.HowEntity(entityID, SzHowEntityDefaultFlags);
                demoResult = results; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(results)?.AsObject();
                JsonObject? howResults = jsonObject?["HOW_RESULTS"]?.AsObject();
                JsonArray? stepsArr = howResults?["RESOLUTION_STEPS"]?.AsArray();

                for (int index = 0; stepsArr != null && index < stepsArr.Count; index++)
                {
                    JsonObject? step = stepsArr[index]?.AsObject();

                    JsonObject? matchInfo = step?["MATCH_INFO"]?.AsObject();

                    Assert.That(matchInfo != null); // @replace . . .
                }

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Entity not found for entity ID.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve how analysis.", e);
            }
            // @end
            this.saveDemoResult("HowEntity", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private ISet<(string, string)> GetVirtualEntityRecordKeys()
    {
        return ImmutableHashSet.Create<(string, string)>(
            new (string, string)[] { PassengerDEF456, PassengerGHI789, PassengerJKL012 });
    }

    [Test]
    public void GetVirtualEntityDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetVirtualEntity
            // How to retrieve a virtual entity via a set of record keys
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // get the records to operate on (varies by application)
                ISet<(string, string)> recordKeys = GetVirtualEntityRecordKeys();

                // retrieve the virtual entity for the record keys
                string result = engine.GetVirtualEntity(recordKeys, SzVirtualEntityDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                JsonObject? entity = jsonObject?["RESOLVED_ENTITY"]?.AsObject();
                string? entityName = entity?["ENTITY_NAME"]?.GetValue<string>();

                Assert.That(entityName != "FOO"); // @replace . . .

                if (jsonObject != null && jsonObject.ContainsKey("RECORDS"))
                {
                    JsonArray? recordArr = jsonObject["RECORDS"]?.AsArray();

                    for (int index = 0; recordArr != null && index < recordArr.Count; index++)
                    {
                        JsonObject? record = recordArr[index]?.AsObject();

                        string? dataSource = record?["DATA_SOURCE"]?.GetValue<string>();
                        string? recordID = record?["RECORD_ID"]?.GetValue<string>();

                        Assert.That(dataSource != null && recordID != null); // @replace . . .
                    }
                }

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Specified record key was not found.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve virtual entity.", e);
            }
            // @end
            this.saveDemoResult("GetVirtualEntity", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void GetRecordDemo()
    {
        try
        {
            string demoResult = "";
            // @start GetRecord
            // How to retrieve a record via its record key
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // retrieve the entity by record key
                string result = engine.GetRecord("TEST", "ABC123", SzRecordDefaultFlags);
                demoResult = result; // @omit

                // do something with the response JSON (varies by application)
                JsonObject? jsonObject = JsonNode.Parse(result)?.AsObject();
                string? dataSource = jsonObject?["DATA_SOURCE"]?.GetValue<string>();
                string? recordID = jsonObject?["RECORD_ID"]?.GetValue<string>();

                Assert.That(dataSource != null && recordID != null); // @replace . . .

            }
            catch (SzUnknownDataSourceException e)
            {
                // handle the unknown data source exception
                LogError("Expected data source is not configured.", e);

            }
            catch (SzNotFoundException e)
            {
                // handle the not-found exception
                LogError("Record not found for record key.", e);

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to retrieve record by record key.", e);
            }
            // @end
            this.saveDemoResult("GetRecord", demoResult, true);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private static void ProcessJsonRecord(JsonObject? jsonObject)
    {
        if (jsonObject == null)
        {
            throw new ArgumentNullException("jsonObject");
        }
    }

    [Test]
    public void ExportJsonDemo()
    {
        try
        {
            StringWriter sw = new StringWriter();
            // @start ExportJson
            // How to export entity data in JSON format
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // export the JSON data
                IntPtr exportHandle = engine.ExportJsonEntityReport(SzExportDefaultFlags);

                // read the data
                try
                {
                    // fetch the first JSON record
                    string jsonData = engine.FetchNext(exportHandle);

                    while (jsonData != null)
                    {
                        sw.WriteLine(jsonData.Trim()); // @omit
                        // parse the JSON data
                        JsonObject? jsonObject = JsonNode.Parse(jsonData)?.AsObject();

                        // do something with the parsed data (varies by application)
                        ProcessJsonRecord(jsonObject);

                        // fetch the next JSON record
                        jsonData = engine.FetchNext(exportHandle);
                    }

                }
                finally
                {
                    // close the export handle
                    engine.CloseExportReport(exportHandle);
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to perform JSON export.", e);
            }
            // @end
            string exportReport = sw.ToString();
            StringReader sr = new StringReader(exportReport);
            string firstLine = sr.ReadLine() ?? "";
            this.saveDemoResult("ExportJson", exportReport, false);
            this.saveDemoResult("ExportJson-fetchNext", firstLine, false);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    private static void ProcessCsvHeaders(string? headerLine)
    {
        if (headerLine == null)
        {
            throw new ArgumentNullException("headerLine");
        }
    }

    private static void ProcessCsvRecord(string? recordLine)
    {
        if (recordLine == null)
        {
            throw new ArgumentNullException("recordLine");
        }
    }

    [Test]
    public void ExportCsvDemo()
    {
        try
        {
            StringWriter sw = new StringWriter();
            // @start ExportCsv
            // How to export entity data in CSV format
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // export the JSON data
                IntPtr exportHandle = engine.ExportCsvEntityReport("*", SzExportDefaultFlags);

                // read the data
                try
                {
                    // fetch the CSV header line from the exported data
                    string csvHeaders = engine.FetchNext(exportHandle);

                    // process the CSV headers (varies by application)
                    ProcessCsvHeaders(csvHeaders);
                    sw.WriteLine(csvHeaders.Trim()); // @omit

                    // fetch the first CSV record from the exported data
                    string csvRecord = engine.FetchNext(exportHandle);

                    while (csvRecord != null)
                    {
                        // do something with the exported record (varies by application)
                        ProcessCsvRecord(csvRecord);
                        sw.WriteLine(csvRecord.Trim()); // @omit

                        // fetch the next exported CSV record
                        csvRecord = engine.FetchNext(exportHandle);
                    }

                }
                finally
                {
                    // close the export handle
                    engine.CloseExportReport(exportHandle);
                }

            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to perform CSV export.", e);
            }
            // @end
            string exportReport = sw.ToString();
            StringReader sr = new StringReader(exportReport);
            string headerLine = sr.ReadLine() ?? "";
            string dataLine = sr.ReadLine() ?? "";
            this.saveDemoResult("ExportCsv", exportReport, false);
            this.saveDemoResult("ExportCsv-fetchNext-header", headerLine, false);
            this.saveDemoResult("ExportCsv-fetchNext-data", dataLine, false);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    [Test]
    public void ProcessRedosDemo()
    {
        try
        {
            // @start ProcessRedos
            // How to check for and process redo records
            try
            {
                // obtain the SzEnvironment (varies by application)
                SzEnvironment env = GetEnvironment();

                // get the engine
                SzEngine engine = env.GetEngine();

                // loop through any redo records
                for (string redoRecord = engine.GetRedoRecord();
                     redoRecord != null;
                     redoRecord = engine.GetRedoRecord())
                {
                    try
                    {
                        // process the redo record
                        string info = engine.ProcessRedoRecord(redoRecord, SzWithInfo);

                        // do something with the "info JSON" (varies by application)
                        JsonObject? jsonObject = JsonNode.Parse(info)?.AsObject();
                        if (jsonObject != null && jsonObject.ContainsKey("AFFECTED_ENTITIES"))
                        {
                            JsonArray? affectedArr = jsonObject["AFFECTED_ENTITIES"]?.AsArray();
                            for (int index = 0; affectedArr != null && index < affectedArr.Count; index++)
                            {
                                JsonObject? affected = affectedArr[index]?.AsObject();
                                long affectedID = affected?["ENTITY_ID"]?.GetValue<long>() ?? 0L;

                                Assert.That(affectedID > 0L); // @replace . . .
                            }
                        }

                    }
                    catch (SzException e)
                    {
                        // handle or rethrow the other exceptions
                        LogError("Failed to process redo record: " + redoRecord, e);
                    }
                }

                // get the redo count
                long redoCount = engine.CountRedoRecords();

                // do something with the redo count
                Log("Pending Redos: " + redoCount);
            }
            catch (SzException e)
            {
                // handle or rethrow the other exceptions
                LogError("Failed to process redos.", e);
            }
            // @end
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

}
