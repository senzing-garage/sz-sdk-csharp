namespace Senzing.Sdk.Tests.Core;

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;

using NUnit.Framework;
using NUnit.Framework.Internal;

using Senzing.Sdk;
using Senzing.Sdk.Core;

using static System.StringComparison;

using static Senzing.Sdk.SzFlags;
using static Senzing.Sdk.Tests.Core.SzRecord;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzConfigRetryableTest : AbstractTest
{
    /// <summary>
    /// UTF8 encoding constant.
    /// </summary>
    private static readonly Encoding UTF8 = new UTF8Encoding();

    /// <summary>
    /// A delegate that takes this test as the first parameter and the
    /// pre-process result object and returns a specific type.
    /// </summary>
    public delegate T? Getter<out T>(SzConfigRetryableTest test, object? pre);

    /// <summary>
    /// A delegate that takes this test as the first parameter and returns
    /// an object.
    /// </summary>
    public delegate object PreProcess(SzConfigRetryableTest test);

    /// <summary>
    /// A delegate that takes this test as the first paramerter, the
    /// pre-process object as the second parameter and the test result
    /// as the third parameter.
    /// </summary>
    public delegate void PostProcess(SzConfigRetryableTest test, object? pre, object? result);

    /// <summary>
    /// An empty object array.
    /// </summary>
    private static readonly object?[] EmptyArray = Array.Empty<object?>();

    /// <summary>
    /// A getter function that returns an empty array.
    /// </summary>
    private static readonly Getter<object?[]> EmptyGetter
        = (test, pre) => EmptyArray;

    /// <summary>
    /// The data source code for the customers data source.
    /// </summary>
    public const string Customers = "CUSTOMERS";

    /// <summary>
    /// The data source code for the employees data source.
    /// </summary>
    public const string Employees = "EMPLOYEES";

    /// <summary>
    /// The data source code for the employees data source.
    /// </summary>
    public const string Passengers = "PASSENGERS";

    /// <summary>
    /// The record key for customer ABC123.
    /// </summary>
    public static readonly (string dataSourceCode, string recordID)
        CustomerABC123 = (Customers, "ABC123");

    /// <summary>
    /// The record key for customer DEF456.
    /// </summary>
    public static readonly (string dataSourceCode, string recordID)
        CustomerDEF456 = (Customers, "DEF456");

    /// <summary>
    /// The record key for employee ABC123.
    /// </summary>
    public static readonly (string dataSourceCode, string recordID)
        EmployeeABC123 = (Employees, "ABC123");

    /// <summary>
    /// The record key for employee DEF456.
    /// </summary>
    private static readonly (string dataSourceCode, string recordID)
        EmployeeDEF456 = (Employees, "DEF456");

    /// <summary>
    /// The record key for passenger ABC123.
    /// </summary>
    public static readonly (string dataSourceCode, string recordID)
         PassengerABC123 = (Passengers, "ABC123");

    /// <summary>
    /// The record key for passenger DEF456.
    /// </summary>
    public static readonly (string dataSourceCode, string recordID)
        PassengerDEF456 = (Passengers, "DEF456");

    /// <summary>
    /// The record definition for the <see cref="CustomerABC123"/> 
    /// and <see cref="EmployeeABC123"/> record keys.
    /// </summary>
    public const string RecordABC123 = """
            {
                "NAME_FULL": "Joe Schmoe",
                "HOME_PHONE_NUMBER": "702-555-1212",
                "MOBILE_PHONE_NUMBER": "702-555-1313",
                "ADDR_FULL": "101 Main Street, Las Vegas, NV 89101"
            }
            """;

    /// <summary>
    /// The record definition for the <see cref="CustomerDEF456"/>
    /// and <see cref="EmployeeDEF456"/> record keys.
    /// </summary>
    public const string RecordDEF456 = """
            {
                "NAME_FULL": "Jane Schmoe",
                "HOME_PHONE_NUMBER": "702-555-1212",
                "MOBILE_PHONE_NUMBER": "702-555-1414",
                "ADDR_FULL": "101 Main Street, Las Vegas, NV 89101",
                "SSN_NUMBER": "888-88-8888"
            }
            """;

    /// <summary>
    /// The <see cref="System.Collections.Generic.List{T}"/> of 
    /// <see cref="SzRecord"/> instances to trigger a redo so 
    /// <see cref="SzEngine.ProcessRedoRecord(string, SzFlag?)"/>
    /// can be tested.
    /// </summary>
    private static readonly List<SzRecord> ProcessRedoTriggerRecords = ListOf(
        new SzRecord(
            (Employees, "SAME-SSN-11"),
            SzFullName.Of("Anthony Stark"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-12"),
            SzFullName.Of("Janet Van Dyne"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-13"),
            SzFullName.Of("Henry Pym"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-14"),
            SzFullName.Of("Bruce Banner"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-15"),
            SzFullName.Of("Steven Rogers"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-16"),
            SzFullName.Of("Clinton Barton"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-17"),
            SzFullName.Of("Wanda Maximoff"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-18"),
            SzFullName.Of("Victor Shade"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-19"),
            SzFullName.Of("Natasha Romanoff"),
            SzSocialSecurity.Of("888-88-8888")),
        new SzRecord(
            (Employees, "SAME-SSN-20"),
            SzFullName.Of("James Rhodes"),
            SzSocialSecurity.Of("888-88-8888")));

    /// <summary>
    /// A fake redo record for attempting redo pre-reinitialize.
    /// </summary>
    public static readonly Iterator<String> FakeRedoRecords;

    static SzConfigRetryableTest()
    {
        List<string> list = new List<string>();
        foreach (SzRecord record in ProcessRedoTriggerRecords)
        {
            (string dataSource, string recordID)? recordKey = record.GetRecordKey();
            list.Add($$"""
                {
                    "REASON": "LibFeatID[?] of FTypeID[7] went generic for CANDIDATES",
                    "DATA_SOURCE": "{{recordKey?.dataSource}}",
                    "RECORD_ID": "{{recordKey?.recordID}}",
                    "REEVAL_ITERATION": 1,
                    "ENTITY_CORRUPTION_TRANSIENT": false,
                    "DSRC_ACTION": "X"
                }
                """);
        }
        FakeRedoRecords = GetCircularIterator(list);
    }

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
    /// <summary>
    /// The <see cref="List"/> of feature ID's to use for the tests.
    /// </summary>
    private readonly IList<long> featureIDs = new List<long>();

    /// <summary>
    /// The <see cref="Dictionary"/> of record keys to 
    /// <c>long</c> entity ID values.
    /// </summary>
    private Dictionary<(string, string), long> byRecordKeyLookup
        = new Dictionary<(string, string), long>();

    /// <summary>
    /// Gets the entity ID for the specified record key.
    /// </summary>
    /// 
    /// <param name="key">
    /// The record key for which to lookup the entity.
    /// </param>
    /// 
    /// <returns>
    /// The entity ID for the specified record key, or
    /// <c>null</c> if not found.
    /// </returns>
    private long GetEntityID((string, string) key)
    {
        return this.byRecordKeyLookup[key];
    }

    [OneTimeSetUp]
    public void InitializeEnvironment()
    {
        this.BeginTests();
        this.InitializeTestEnvironment();

        string settings = this.GetRepoSettings();
        string instanceName = this.GetType().Name;

        this.env = SzCoreEnvironment.NewBuilder()
                                    .InstanceName(instanceName)
                                    .Settings(settings)
                                    .VerboseLogging(false)
                                    .Build();

        try
        {
            SzEngine engine = this.Env.GetEngine();

            engine.AddRecord(EmployeeABC123.dataSourceCode,
                             EmployeeABC123.recordID,
                             RecordABC123);

            engine.AddRecord(EmployeeDEF456.dataSourceCode,
                             EmployeeDEF456.recordID,
                             RecordDEF456);

            foreach (SzRecord record in ProcessRedoTriggerRecords)
            {
                (string dataSourceCode, string recordID)? recordKey
                    = record.GetRecordKey();

                engine.AddRecord(recordKey?.dataSourceCode ?? "",
                                 recordKey?.recordID ?? "",
                                 record.ToString());
            }
        }
        catch (SzException e)
        {
            Fail("Failed to load record", e);
        }

        // change config and load data in a sub-process
        this.ExecuteSubProcess();
    }

    /// <summary>
    /// Overridden to configure <b>ONLY</b> the <see cref="Employees"/>
    /// data source.
    /// </summary>
    protected override void PrepareRepository()
    {
        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();

        ISet<string> dataSources = SortedSetOf(Employees);

        RepositoryManager.ConfigSources(repoDirectory,
                                        dataSources,
                                        true);
    }

    /// <summary>
    /// Executes a sub-process that will add a data source to the config
    /// and load two records to that data source and then wait for that
    /// process to commplete.
    /// </summary>
    private void ExecuteSubProcess()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string assemblyName = assembly.GetName().Name ?? "";
        string dirPath = Directory.GetCurrentDirectory();

        DirectoryInfo? dir = new DirectoryInfo(dirPath);
        while (dir != null && !dir.Name.Equals(assemblyName, Ordinal))
        {
            dir = dir?.Parent;
        }
        // get the parent of the assembly directory
        dir = dir?.Parent;
        if (dir == null)
        {
            Fail("Failed to find project directory: " + Directory.GetCurrentDirectory());
            throw new InvalidOperationException("Failed to find parent directory");
        }

        DirectoryInfo repoDirectory = this.GetRepositoryDirectory();
        string initFilePath = Path.Combine(repoDirectory.FullName, "sz-init.json");
        string outputFilePath = Path.GetTempFileName();

        ProcessStartInfo startInfo = new ProcessStartInfo(
            "dotnet",
            ListOf("run",
                    "--project",
                    "Senzing.Sdk.TestHelpers",
                    typeof(SzConfigRetryableTest).Name + "Helper",
                    initFilePath,
                    outputFilePath));

        IDictionary origEnv = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry entry in origEnv)
        {
            startInfo.Environment[entry.Key?.ToString() ?? ""]
                = entry.Value?.ToString() ?? "";
        }

        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = false;
        startInfo.RedirectStandardError = false;
        startInfo.RedirectStandardOutput = false;
        startInfo.WorkingDirectory = dir.FullName;

        Process? process = Process.Start(startInfo);

        if (process == null)
        {
            Fail("Failed ot launch new process");
            throw new AssertionException("Failed to launch new process");
        }
        process.WaitForExit();

        int exitCode = process.ExitCode;

        if (exitCode != 0)
        {
            Fail("Failed to launch alternate process to update config");
        }

        string output = File.ReadAllText(outputFilePath, UTF8);

        JsonObject? jsonObj = JsonNode.Parse(output)?.AsObject();
        JsonArray? jsonArr = jsonObj?["ENTITIES"]?.AsArray();

        for (int index = 0; index < (jsonArr?.Count ?? 0); index++)
        {
            JsonObject? entityObj = jsonArr?[index]?.AsObject();

            entityObj = entityObj?["RESOLVED_ENTITY"]?.AsObject();

            // get the feature ID's
            JsonObject? features = entityObj?["FEATURES"]?.AsObject();

            IDictionary<string, JsonNode?> featuresDictionary
                = ((IDictionary<string, JsonNode?>?)features) ?? new Dictionary<string, JsonNode?>();

            foreach (KeyValuePair<string, JsonNode?> pair in featuresDictionary)
            {
                string key = pair.Key;
                JsonNode? value = pair.Value;

                JsonArray? valuesArr = value?.AsArray();

                for (int index2 = 0; index2 < (valuesArr?.Count ?? 0); index2++)
                {
                    JsonObject? featureObj = valuesArr?[index2]?.AsObject();

                    long featureID = featureObj?["LIB_FEAT_ID"]?.GetValue<long>() ?? 0L;

                    if (featureID != 0L && !featureIDs.Contains(featureID))
                    {
                        this.featureIDs.Add(featureID);
                    }
                }
            }

            // get the entity ID
            long? entityID = entityObj?["ENTITY_ID"]?.GetValue<long>();

            // get the record keys
            JsonArray? recordArr = entityObj?["RECORDS"]?.AsArray();
            for (int index2 = 0; index2 < (recordArr?.Count ?? 0); index2++)
            {
                JsonObject? recordObj = recordArr?[index2]?.AsObject();

                string? dataSource = recordObj?["DATA_SOURCE"]?.GetValue<string>();
                string? recordID = recordObj?["RECORD_ID"]?.GetValue<string>();

                if (Customers.Equals(dataSource, StringComparison.Ordinal)
                    && recordID != null)
                {
                    this.byRecordKeyLookup[(dataSource, recordID)] = (entityID ?? 0L);
                }
            }
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

    private static void AddMethod(ISet<MethodInfo> handledMethods,
                                  IList<object?[]> results,
                                  Getter<object> getter,
                                  MethodInfo? method,
                                  bool? expectRetryable,
                                  Getter<object?[]>? paramGetter,
                                  PreProcess? preProcess = null,
                                  PostProcess? postProcess = null)

    {
        ArgumentNullException.ThrowIfNull(method, "The method cannot be null");

        if (handledMethods.Contains(method)) return;
        results.Add(new object?[] {
            getter, method, expectRetryable, paramGetter, preProcess, postProcess });
        handledMethods.Add(method);
    }

    private static void AddProductMethods(ISet<MethodInfo> handledMethods,
                                          IList<object?[]> results)
    {
        AddMethod(handledMethods,
                    results,
                    (test, pre) => test.Env.GetProduct(),
                    typeof(SzProduct).GetMethod("GetVersion", []),
                    false,
                    EmptyGetter);

        AddMethod(handledMethods,
                    results,
                    (test, pre) => test.Env.GetProduct(),
                    typeof(SzProduct).GetMethod("GetLicense", []),
                    false,
                    EmptyGetter);

        // handle any methods not explicitly handled
        MethodInfo[] methods = typeof(SzProduct).GetMethods();
        foreach (MethodInfo method in methods)
        {
            AddMethod(handledMethods,
                      results,
                      (test, pre) => null,
                      method,
                      null,
                      null);
        }
    }

    private static void AddConfigManagerMethods(ISet<MethodInfo> handledMethods,
                                                IList<object?[]> results)
    {
        // handle config manager methods
        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod("CreateConfig", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod("CreateConfig", [typeof(long)]),
                  false,
                  (test, pre) => [test.Env.GetConfigManager().GetDefaultConfigID()]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod("CreateConfig", [typeof(string)]),
                  false,
                  (test, pre) => [test.Env.GetConfigManager().CreateConfig().Export()]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "RegisterConfig", [typeof(string), typeof(string)]),
                  false,
                  (test, pre) => [test.Env.GetConfigManager().CreateConfig().Export(), "Template Config"]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "RegisterConfig", [typeof(string)]),
                  false,
                  (test, pre) =>
                  {
                      SzConfig config = test.Env.GetConfigManager().CreateConfig();
                      config.RegisterDataSource(Employees);
                      return [config.Export()];
                  });

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod("GetConfigRegistry", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod("GetDefaultConfigID", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "ReplaceDefaultConfigID", [typeof(long), typeof(long)]),
                  false,
                  null);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "SetDefaultConfigID", [typeof(long)]),
                  false,
                  null);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "SetDefaultConfig", [typeof(string), typeof(string)]),
                  false,
                  null);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager(),
                  typeof(SzConfigManager).GetMethod(
                    "SetDefaultConfig", [typeof(string)]),
                  false,
                  null);

        MethodInfo[] methods = typeof(SzConfigManager).GetMethods();
        foreach (MethodInfo method in methods)
        {
            AddMethod(handledMethods,
                      results,
                      (test, pre) => null,
                      method,
                      null,
                      null);
        }
    }

    private static void AddConfigMethods(ISet<MethodInfo> handledMethods,
                                         IList<object?[]> results)
    {
        // handle config methods
        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager().CreateConfig(),
                  typeof(SzConfig).GetMethod("Export", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager().CreateConfig(),
                  typeof(SzConfig).GetMethod("GetDataSourceRegistry", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager().CreateConfig(),
                  typeof(SzConfig).GetMethod(
                    "RegisterDataSource", [typeof(string)]),
                  false,
                  (test, pre) => [Customers]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetConfigManager().CreateConfig(
                  test.Env.GetConfigManager().GetDefaultConfigID()),
                  typeof(SzConfig).GetMethod(
                    "UnregisterDataSource", [typeof(string)]),
                  false,
                  (test, pre) => [Customers]);

        MethodInfo[] methods = typeof(SzConfig).GetMethods();
        foreach (MethodInfo method in methods)
        {
            AddMethod(handledMethods,
                      results,
                      (test, pre) => null,
                      method,
                      null,
                      null);
        }
    }

    private static void AddDiagnosticMethods(ISet<MethodInfo> handledMethods,
                                             IList<object?[]> results)
    {
        // handle config methods
        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetDiagnostic(),
                  typeof(SzDiagnostic).GetMethod("GetRepositoryInfo", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetDiagnostic(),
                  typeof(SzDiagnostic).GetMethod(
                    "CheckRepositoryPerformance", [typeof(int)]),
                  false,
                  (test, pre) => [5]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetDiagnostic(),
                  typeof(SzDiagnostic).GetMethod("PurgeRepository", []),
                  false,
                  null);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetDiagnostic(),
                  typeof(SzDiagnostic).GetMethod("GetFeature", [typeof(long)]),
                  true,
                  (test, pre) => [test.featureIDs[0]]);

        MethodInfo[] methods = typeof(SzDiagnostic).GetMethods();
        foreach (MethodInfo method in methods)
        {
            AddMethod(handledMethods,
                      results,
                      (test, pre) => null,
                      method,
                      null,
                      null);
        }
    }

    private static void AddEngineMethods(ISet<MethodInfo> handledMethods,
                                         IList<object?[]> results)
    {
        // handle config methods
        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("PrimeEngine", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("GetStats", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "GetRecordPreview", [typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [RecordABC123, SzRecordPreviewAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "SearchByAttributes",
                    [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [RecordABC123, null, SzSearchAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "SearchByAttributes", [typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [RecordABC123, SzSearchAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "WhySearch",
                    [typeof(string), typeof(long), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      RecordABC123,
                      test.GetEntityID(CustomerDEF456),
                      null,
                      SzWhySearchAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "GetEntity", [typeof(long), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [test.GetEntityID(CustomerABC123), SzEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "GetEntity", [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      SzEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindInterestingEntities", [typeof(long), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [test.GetEntityID(CustomerABC123), SzEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindInterestingEntities",
                    [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      SzEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindPath",
                    [
                        typeof(long),
                        typeof(long),
                        typeof(int),
                        typeof(ISet<long>),
                        typeof(ISet<string>),
                        typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      test.GetEntityID(CustomerABC123),
                      test.GetEntityID(CustomerDEF456),
                      3,
                      null,
                      null,
                      SzFindPathAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindPath",
                    [
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(int),
                        typeof(ISet<(string, string)>),
                        typeof(ISet<string>),
                        typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      CustomerDEF456.dataSourceCode,
                      CustomerDEF456.recordID,
                      3,
                      null,
                      null,
                      SzFindPathAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindNetwork",
                    [
                        typeof(ISet<long>),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                        typeof(SzFlag?)]),
                      true,
                      (test, pre) => [
                          new SortedSet<long> {
                              test.GetEntityID(CustomerABC123),
                              test.GetEntityID(CustomerDEF456)
                          },
                          3,  // max degrees
                          0,  // build-out degrees
                          10, // build-out max entities
                          SzFindNetworkAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "FindNetwork",
                    [
                        typeof(ISet<(string, string)>),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                        typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      new SortedSet<(string, string)> {
                            CustomerABC123, CustomerDEF456 },
                      3,  // max degrees
                      0,  // build-out degrees
                      10, // build-out max entities
                      SzFindNetworkAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "WhyRecordInEntity",
                    [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      SzWhyRecordInEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "WhyRecords",
                    [
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(string),
                        typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      CustomerDEF456.dataSourceCode,
                      CustomerDEF456.recordID,
                      SzWhyRecordsAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "WhyEntities", [typeof(long), typeof(long), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      test.GetEntityID(CustomerABC123),
                      test.GetEntityID(CustomerDEF456),
                      SzWhyEntitiesAllFlags]);

        AddMethod(handledMethods,
                results,
                (test, pre) => test.Env.GetEngine(),
                typeof(SzEngine).GetMethod(
                    "HowEntity", [typeof(long), typeof(SzFlag?)]),
                    true,
                    (test, pre) => [test.GetEntityID(CustomerABC123), SzHowAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                  "GetVirtualEntity", [typeof(ISet<(string, string)>), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      new SortedSet<(string, string)> { CustomerABC123, CustomerDEF456 },
                      SzVirtualEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                     "GetRecord", [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      CustomerABC123.dataSourceCode,
                      CustomerABC123.recordID,
                      SzRecordAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "ExportJsonEntityReport", [typeof(SzFlag?)]),
                  true,
                  (test, pre) => [SzExportAllFlags],
                  null,
                  (test, pre, handle) =>
                  {
                      if (handle != null)
                      {
                          test.Env.GetEngine().CloseExportReport((IntPtr)handle);
                      }
                  });

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "ExportCsvEntityReport", [typeof(string), typeof(SzFlag?)]),
                    true,
                    (test, pre) => ["*", SzExportAllFlags],
                    null,
                    (test, pre, handle) =>
                    {
                        if (handle != null)
                        {
                            test.Env.GetEngine().CloseExportReport((IntPtr)handle);
                        }
                    });

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("CloseExportReport", [typeof(IntPtr)]),
                  false,
                  null); // requires a valid export handle which cannot be gotten

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("FetchNext", [typeof(IntPtr)]),
                  true,
                  null); // requires a valid export handle which cannot be gotten

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("CountRedoRecords", []),
                  false,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod("GetRedoRecord", []),
                  true,
                  EmptyGetter);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                  "ProcessRedoRecord", [typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [FakeRedoRecords.Next(), SzRedoAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "AddRecord",
                    [typeof(string), typeof(string), typeof(string), typeof(SzFlag?)]),
                    true,
                    (test, pre) => [
                        PassengerABC123.dataSourceCode,
                        PassengerABC123.recordID,
                        RecordABC123,
                        SzAddRecordAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "ReevaluateRecord",
                    [typeof(string), typeof(string), typeof(SzFlag?)]),
                    true,
                    (test, pre) => [
                        CustomerABC123.dataSourceCode,
                        CustomerABC123.recordID,
                        SzReevaluateEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "ReevaluateEntity", [typeof(long), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      test.GetEntityID(CustomerABC123),
                      SzReevaluateEntityAllFlags]);

        AddMethod(handledMethods,
                  results,
                  (test, pre) => test.Env.GetEngine(),
                  typeof(SzEngine).GetMethod(
                    "DeleteRecord", [typeof(string), typeof(string), typeof(SzFlag?)]),
                  true,
                  (test, pre) => [
                      EmployeeABC123.dataSourceCode,
                      EmployeeABC123.recordID,
                      SzDeleteRecordAllFlags]);

        MethodInfo[] methods = typeof(SzEngine).GetMethods();
        foreach (MethodInfo method in methods)
        {
            AddMethod(handledMethods,
                      results,
                      (test, pre) => null,
                      method,
                      null,
                      null);
        }
    }

    public static List<object?[]> GetTestParameters()
    {
        List<object?[]> results = new List<object?[]>();

        ISet<MethodInfo> handledMethods = new HashSet<MethodInfo>();

        AddProductMethods(handledMethods, results);

        AddConfigManagerMethods(handledMethods, results);

        AddConfigMethods(handledMethods, results);

        AddDiagnosticMethods(handledMethods, results);

        AddEngineMethods(handledMethods, results);

        // handle the diagnostic methods
        return results;
    }

    [Test, TestCaseSource(nameof(GetTestParameters)), Order(10)]
    public void TestMethodPreReinitialize(Getter<object> getter,
                                          MethodInfo method,
                                          bool? expectRetryable,
                                          Getter<object?[]> paramGetter,
                                          PreProcess? preProcess,
                                          PostProcess? postProcess)
    {
        try
        {
            object[] attrs = method.GetCustomAttributes(typeof(SzConfigRetryable), false);
            bool retryable = (attrs.Length > 0);

            if (expectRetryable == null)
            {
                Fail("Method from interface (" + method.DeclaringType?.Name
                     + ") is " + (retryable ? "" : "not ")
                     + "annotated and has no explicit config retryable test defined: "
                     + method.ToString());
            }

            Assert.That(retryable, Is.EqualTo(expectRetryable),
                        "Method from interface (" + method.DeclaringType?.Name
                        + ") is " + (retryable ? "" : "not ")
                        + "retryable but should " + (retryable ? "" : "not ")
                        + "be: " + method.ToString());

            if (paramGetter == null)
            {
                return;
            }

            object? preProcessResult = (preProcess == null) ? null : preProcess(this);

            object? target = (getter == null) ? null : getter(this, preProcessResult);

            object?[] args = paramGetter(this, preProcessResult) ?? [];

            object? result = null;
            try
            {
                // this may or may not succeed
                result = method.Invoke(target, args);
            }
            catch (Exception e)
            when (e is SystemException || e is ApplicationException || e is SzException)
            {
                Exception cause = (e is SzException) ? e : e.GetBaseException();
                if (!(cause is SzException))
                {
                    Fail("Method from " + method.DeclaringType?.Name + " got an "
                         + "unexpected exception: " + method.ToString(), e);
                }
                if (expectRetryable == false)
                {
                    Fail("Non-annotated method from " + method.DeclaringType?.Name
                         + "got an exception before reinitialization: "
                         + method.ToString(), e);
                }
            }
            finally
            {
                if (postProcess != null)
                {
                    postProcess(this, preProcessResult, result);
                }
            }

        }
        catch (Exception e) when (!(e is AssertionException))
        {
            Fail("Method from " + method.DeclaringType?.Name
                 + " failed with exception: " + method.ToString(), e);
        }
    }

    [Test, Order(20)]
    public void TestReinitialize()
    {
        try
        {
            this.Env.Reinitialize(this.Env.GetConfigManager().GetDefaultConfigID());

            Dictionary<(string, string), long> map
                = new Dictionary<(string, string), long>();
            foreach ((string dataSource, string recordID) in this.byRecordKeyLookup.Keys)
            {
                try
                {
                    string entity = this.Env.GetEngine().GetEntity(
                        dataSource, recordID, SzNoFlags);

                    JsonObject? jsonObj = JsonNode.Parse(entity)?.AsObject();
                    jsonObj = jsonObj?["RESOLVED_ENTITY"]?.AsObject();

                    long? entityID = jsonObj?["ENTITY_ID"]?.GetValue<long>();

                    map[(dataSource, recordID)] = entityID ?? 0L;

                }
                catch (SzException e)
                {
                    Fail("Failed to update entity ID for record key: "
                         + "[" + dataSource + ":" + recordID + "]", e);
                }
            }
            this.byRecordKeyLookup = map;

        }
        catch (Exception e) when (e is not AssertionException)
        {
            Fail("Failed to reinitialize", e);
        }
    }

    [Test, TestCaseSource(nameof(GetTestParameters)), Order(30)]
    public void TestMethodPostReinitialize(Getter<object> getter,
                                           MethodInfo method,
                                           bool? expectRetryable,
                                           Getter<object?[]> paramGetter,
                                           PreProcess? preProcess,
                                           PostProcess? postProcess)
    {
        try
        {
            object[] attrs = method.GetCustomAttributes(typeof(SzConfigRetryable), false);
            bool retryable = (attrs.Length > 0);

            if (expectRetryable == null)
            {
                Fail("Method from interface (" + method.DeclaringType?.Name
                     + ") is " + (retryable ? "" : "not ")
                     + "annotated and has no explicit config retryable test defined: "
                     + method.ToString());
            }

            Assert.That(retryable, Is.EqualTo(expectRetryable),
                        "Method from interface (" + method.DeclaringType?.Name
                        + ") is " + (retryable ? "" : "not ")
                        + "retryable but should " + (retryable ? "" : "not ")
                        + "be: " + method.ToString());

            if (paramGetter == null)
            {
                return;
            }

            object? preProcessResult = (preProcess == null) ? null : preProcess(this);

            object? target = (getter == null) ? null : getter(this, preProcessResult);

            object?[] args = paramGetter(this, preProcessResult) ?? [];

            object? result = null;
            try
            {
                // this may or may not succeed
                result = method.Invoke(target, args);
            }
            catch (Exception e)
            when (e is SystemException || e is ApplicationException || e is SzException)
            {
                Exception cause = (e is SzException) ? e : e.GetBaseException();

                Fail((retryable ? "Annotated" : "Non-annotated")
                      + " method from " + method.DeclaringType?.Name
                      + " got an exception AFTER reinitialization: "
                      + method.ToString(), e);
            }
            finally
            {
                if (postProcess != null)
                {
                    postProcess(this, preProcessResult, result);
                }
            }

        }
        catch (Exception e) when (!(e is AssertionException))
        {
            Fail("Method from " + method.DeclaringType?.Name
                 + " failed with exception: " + method.ToString(), e);
        }
    }
}
