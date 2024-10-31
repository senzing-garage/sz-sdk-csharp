using System;
using System.IO;
using System.Text;
using Senzing.Sdk.Core;
using System.Text.Json.Nodes;
using System.Collections;
using System.Collections.Generic;

namespace Senzing.Sdk.Tests.Core {
internal abstract class AbstractTest {
    /// <summary>
    /// UTF8 encoding constant.
    /// </summary>
    private static readonly Encoding UTF8 = new UTF8Encoding();

    /// <summary>
    /// The number of tests that failed for this instance.
    /// </summary>
    private int failureCount = 0;

    /// <summary>
    /// The number of tests that succeeded for this instance.
    /// </summary>
    private int successCount = 0;

    /// <summary>
    /// The time of the last progress log.
    /// </summary>
    private long progressLogTimestamp = -1L;

    /// <summary>
    /// The class-wide repo directory.
    /// </summary>
    private DirectoryInfo repoDirectory;

    /// <summary>
    /// Whether or not the repository has been created.
    /// </summary>
    private bool repoCreated = false;

    /// <summary>
    /// Protected default constructor.
    /// </summary>
    protected AbstractTest() : this(null) {
        // do nothing
    }

    /// <summary>
    /// Protected constructor allowing the derived class to specify the
    /// location for the entity respository.
    /// </summary>
    /// 
    /// <param name="repoDirectory">
    /// The directory in which to include the entity repository.
    /// </param>
    protected AbstractTest(DirectoryInfo? repoDirectory) {
        if (repoDirectory == null) {
            repoDirectory = CreateTestRepoDirectory(this.GetType());
        }
        this.repoDirectory = repoDirectory;
    }

    /// <summary>
    /// Signals the beginning of the current test suite.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> if replaying previous results and <c>false</c>
    /// if using the live API.
    /// </summary>
    protected void BeginTests() {
        // do nothing for now
    }

    /// <summary>
    /// Signals the end of the current test suite.
    /// </summary>
    protected void EndTests() {
        // do nothing for now
    }

    /// <summary>
    /// Initializes the specified <see cref="Senzing.Sdk.Core.NativeConfig"/>
    /// with the specified parameters.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> to initialize.
    /// </param>
    /// 
    /// <param name="instanceName">
    /// The instance name to use for initialization.
    /// </param>
    /// 
    /// <param name="settings">
    /// The settings to use for initialization.
    /// </param>
    protected void Init(NativeConfig config, string instanceName, string settings) 
    {
        long returnCode = config.Init(instanceName, settings, false);
        if (returnCode != 0) {
            throw new Exception(config.GetLastException());
        }
    }

    /// <summary>
    /// Initializes the specified <see cref="Senzing.Sdk.Core.NativeConfigManager"/>
    /// with the specified parameters.
    /// </summary>
    /// 
    /// <param name="configMgr">
    /// The <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to initialize.
    /// </param>
    /// 
    /// <param name="instanceName">
    /// The instance name to use for initialization.
    /// </param>
    /// 
    /// <param name="settings">
    /// The settings to use for initialization.
    /// </param>
    protected void Init(NativeConfigManager configMgr, 
                        string              instanceName,
                        string              settings) 
    {
        long returnCode = configMgr.Init(instanceName, settings, false);
        if (returnCode != 0) {
            throw new Exception(configMgr.GetLastException());
        }
    }

    /// <summary>
    /// Destroys the specified <see cref="Senzing.Sdk.Core.NativeConfig"/>
    /// instance.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to destroy.
    /// </param>
    /// 
    /// <returns>Always returns <c>null</c>.</summary>
    protected NativeConfig? Destroy(NativeConfig? config) {
        if (config == null) {
            return null;
        }
        config.Destroy();
        return null;
    }

    /// <summary>
    /// Destroys the specified <see cref="Senzing.Sdk.Core.NativeConfigManager"/>
    /// instance.
    /// </summary>
    /// 
    /// <param name="configMgr">
    /// The <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to destroy.
    /// </param>
    /// 
    /// <returns>Always returns <c>null</c>.</returns>
    protected NativeConfigManager? Destroy(NativeConfigManager? configMgr) {
        if (configMgr == null) {
            return null;
        }
        configMgr.Destroy();
        return null;
    }

    /// <summary>
    /// Creates a default config and returns the config handle.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to use.
    /// </param>
    /// 
    /// <returns>The config handle for the config.</returns>
    protected IntPtr CreateDefaultConfig(NativeConfig config) {
        // create the default config
        long returnCode = config.Create(out IntPtr configHandle);
        if (returnCode != 0) {
            throw new Exception(config.GetLastException());
        }
        return configHandle;
    }

    /// <summary>
    /// Adds the specified config to the repository using the specified
    /// <see cref="Senzing.Sdk.Core.NativeConfigManager"/>.
    /// </summary>
    /// 
    /// <param name="configMgr">
    /// The <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to use.
    /// </param>
    /// 
    /// <param name="config">
    /// The <c>string</c> config to be added.
    /// </param>
    /// 
    /// <param name="comment">
    /// The <c>string</c> comment to associated with the config.
    /// </param>
    /// 
    /// <returns>The configuration ID for the added config.</returns>
    protected long AddConfig(NativeConfigManager    configMgr, 
                             string                 config,
                             string                 comment) 
    {
        long returnCode = configMgr.AddConfig(
            config, comment, out long configID);
        if (returnCode != 0) {
            throw new Exception(configMgr.GetLastException());
        }
        return configID;
    }

    /// <summary>
    /// Gets the default config ID using the specified
    /// <see cref="Senzing.Sdk.Core.NativeConfigManager"/>.
    /// </summary>
    /// 
    /// <param name="configMgr">
    /// The <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to use.
    /// </param>
    /// 
    /// <returns>
    /// The configuration ID for the default config ID.
    /// </returns>
    protected long GetDefaultConfigID(NativeConfigManager configMgr) {
        long returnCode = configMgr.GetDefaultConfigID(out long configID);
        if (returnCode != 0) {
            throw new Exception(configMgr.GetLastException());
        }
        return configID;
    }

    /// <summary>
    /// Closes the config associated with the specified config handle.
    /// </summary>
    /// 
    /// <remarks>
    /// This does nothing if the specified
    /// <see cref="Senzing.Sdk.Core.NativeConfig"/> is <c>null</c> <b>or</b>
    /// the specified config handle is zero (0).
    /// </remarks>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to use.
    /// </param>
    /// 
    /// <param name="configHandle">
    /// The <see cref="System.IntPtr"/> config handle to close.
    /// </param>
    /// 
    /// <returns>Always returns <c>null</c></returns>
    protected long? CloseConfig(NativeConfig? config, IntPtr? configHandle) {
        if (config != null && configHandle != null) {
            config.Close((IntPtr) configHandle);
        }
        return null;
    }

    /// <summary>
    /// Adds a data source to the config associated with the specified
    /// config handle.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to use.
    /// </param>
    /// 
    /// <param name="configHandle">
    /// The <see cref="System.IntPtr"/> config handle associated with
    /// the config to be modified.
    /// </param>
    /// 
    /// <param name="dataSource">
    /// The data source code for the data source to add to the config.
    /// </param>
    protected void AddDataSource(NativeConfig   config,
                                 IntPtr         configHandle,
                                 string         dataSource)
    {
        StringBuilder sb = new StringBuilder("{\"DSRC_CODE\": ");
        sb.Append(Utilities.jsonEscape(dataSource)).Append('}');
        string json = sb.ToString();

        long returnCode = config.AddDataSource(
            configHandle, json, out string result);
        if (returnCode != 0) {
            throw new Exception(config.GetLastException());
        }
    }

    /// <summary>
    /// Exports the config associated with the specified config handle as
    /// a <c>string</c>.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to use.
    /// </param>
    /// 
    /// <param name="configHandle">
    /// The <see cref="System.IntPtr"/> config handle associated with
    /// the config to be modified.
    /// </param>
    /// 
    /// <returns>The JSON <c>string</c> for the config.</returns>
    protected string ExportConfig(NativeConfig config, IntPtr configHandle) {
        long returnCode = config.Save(configHandle, out string configJson);
        if (returnCode != 0) {
            throw new Exception(config.GetLastException());
        }
        return configJson;
    }

    /// <summary>
    /// Creates a new default config and adds the specified zero or more
    /// data sources to it and then returns the JSON <c>string</c> for
    /// that config.
    /// </summary>
    /// 
    /// <param name="config">
    /// The <see cref="Senzing.Sdk.Core.NativeConfig"/> instance to use.
    /// </param>
    /// 
    /// <param name="dataSources">
    /// The zero or more data sources to add to the config.
    /// </param>
    /// 
    /// <returns>The JSON <c>string</c> that for the created config.</returns>
    protected string CreateConfig(NativeConfig      config,
                                  params string[]   dataSources) 
    {
        string result;
        IntPtr configHandle = CreateDefaultConfig(config);
        try {
            foreach (string dataSource in dataSources) {
                AddDataSource(config, configHandle, dataSource);
            }
            result = ExportConfig(config, configHandle);
        } finally {
            CloseConfig(config, configHandle);
        }
        return result;
    }

    /// <summary>
    /// Returns the instance name with which to initialize the server.
    /// </summary>
    /// 
    /// <remarks>
    /// This returns the simple class name of the test class. Override to use a
    /// different instance name.
    /// </remarks>
    /// 
    /// <returns>The instance name with which to initialize the API.</returns>
    protected string GetInstanceName() {
        return this.GetInstanceName(null);
    }

    /// <summary>
    /// Returns the instance name with which to initialize the server.
    /// </summary>
    /// 
    /// <remarks>
    /// This returns the simple class name of the test class optionally
    /// concatenated with a specified non-null suffix. Override to use a
    /// different instance name.
    /// </remarks>
    ///
    /// <param name="suffix">
    /// The optional suffix to append to the instance name.
    /// </param>
    /// 
    /// <returns>
    /// The instance name with which to initialize the API.
    /// </returns>
    protected string GetInstanceName(string? suffix) {
        if (suffix != null && suffix.Trim().Length > 0) {
            suffix = " - " + suffix.Trim();
        } else {
            suffix = "";
        }
        return this.GetType().Name + suffix;
    }

    /// <summary>
    /// Returns the contents of the JSON init file from the default
    /// repository directory.
    /// </summary>
    /// 
    /// <returns>
    /// The contents of the JSON init file as a <c>string</c>.
    /// </returns>
    protected string GetRepoSettings() {
        return this.ReadRepoSettingsFile(this.GetRepositoryDirectory());
    }

    /// <summary>
    /// Returns the contents of the JSON init file as a <c>string</c>.
    /// </summary>
    ///
    /// <param name="repoDirectory">
    /// The <see cref="System.IO.Directory"/> representing the directory
    /// of the test repository.
    /// </param>
    /// 
    /// <returns>
    /// The contents of the JSON init file as a <c>string</c>.
    /// </returns>
    protected string ReadRepoSettingsFile(DirectoryInfo repoDirectory) {
        string initJsonFile = Path.Combine(repoDirectory.FullName, 
                                            "sz-init.json");


        return File.ReadAllText(initJsonFile, UTF8).Trim();
    }

    /// <summary>
    /// Creates a temp repository directory for the test class.
    /// </summary>
    ///
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> representing the directory.
    /// </returns>
    protected DirectoryInfo CreateTestRepoDirectory() {
        return CreateTestRepoDirectory(this.GetType(), null);
    }

    /// <summary>
    /// Creates a temp repository directory for a specific test.
    /// </summary>
    ///
    /// <param name="testName">
    /// The name of the test that the directory will be used for.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> representing the directory.
    /// </summary>
    protected DirectoryInfo CreateTestRepoDirectory(string testName) {
        return CreateTestRepoDirectory(this.GetType(), testName);
    }

    /// <summary>
    /// Creates a temp repository directory for a specific test.
    /// </summary>
    /// 
    /// <param name="t">
    /// The <see cref="System.Type"/> for which the directory is
    /// being created.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> representing the directory.
    /// </summary>
    protected static DirectoryInfo CreateTestRepoDirectory(Type t) {
        return CreateTestRepoDirectory(t, null);
    }

    /// <summary>
    /// Creates a temp repository directory for a specific test.
    /// </summary>
    /// 
    /// <param name="t">
    /// The <see cref="System.Type"/> for which the directory is
    /// being created.
    /// </param>
    /// 
    /// <param name="testName">
    /// The name of the test that the directory will be used for.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> representing the directory.
    /// </summary>
    protected static DirectoryInfo CreateTestRepoDirectory(Type t, string? testName)
    {
        string prefix = "sz-repo-" + t.Name + "-"
                + (testName == null ? "" : (testName + "-"));
        return DoCreateTestRepoDirectory(prefix);
    }

    /// <summary>
    /// Creates a temp repository directory.
    /// </summary>
    /// 
    /// <param name="prefix">
    /// The directory name prefix for the temp repo directory.
    /// </param>
    /// 
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> representing the directory.
    /// </summary>
    protected static DirectoryInfo DoCreateTestRepoDirectory(string prefix) 
    {
        // create the test repo directory name
        string dirName = (prefix == null || prefix.Length == 0)
            ? "test-repo-" + Path.GetRandomFileName()
            : "test-repo-" + prefix + "-" + Path.GetRandomFileName();

        // check if the current directory is the repository root
        string currentDir = Environment.CurrentDirectory;
        string solutionFile = Path.Combine(currentDir, "sz-sdk-csharp.sln");

        if (!File.Exists(solutionFile)) {
            string tempDir = Path.Combine(Path.GetTempPath(), dirName);
            return Directory.CreateDirectory(tempDir);
        }
        
        // we found the solution file, ensure we have a test-repos directory
        string testRepoDir = Path.Combine(currentDir, "test-repos");
        if (!Directory.Exists(testRepoDir)) {
            Directory.CreateDirectory(testRepoDir);
        }

        // create the repo directory
        string dirPath = Path.Combine(testRepoDir, dirName);
        return Directory.CreateDirectory(dirPath);
    }

    /// <summary>
    /// Returns the <see cref="System.IO.DirectoryInfo"/> identifying the
    /// repository directory used for the test.
    /// </summary>
    /// 
    /// <remarks>
    /// This can be specified in the constructor, but if not specified
    /// is a newly created temporary directory.
    /// </remarks>
    /// 
    /// <returns>
    /// The <see cref="System.IO.DirectoryInfo"/> identifying the repository
    /// directory used for the test.
    /// </returns>
    protected DirectoryInfo GetRepositoryDirectory() {
        return this.repoDirectory;
    }

    /// <summary>
    /// This method can typically be called from a method annotated with
    /// <c>[OneTimeSetup]</c> to create and initialize the Senzing
    /// entity repository.
    /// </summary>
    protected void InitializeTestEnvironment() {
        this.InitializeTestEnvironment(false);
    }

    /// <summary>
    /// This method can typically be called from a method annotated with
    /// <c>[OneTimeSetup]</c> to create and initialize the Senzing
    /// entity repository.
    /// </summary>
    /// 
    /// <param name="excludeConfig">
    /// <c>true</c> if the default configuration should be excluded
    /// from the repository, and <c>false</c> if it should be included.
    /// </param>
    protected void InitializeTestEnvironment(bool excludeConfig) {
        String moduleName = this.GetInstanceName("RepoMgr (create)");
        RepositoryManager.SetThreadModuleName(moduleName);
        bool concluded = false;
        try {
            RepositoryManager.CreateRepo(this.GetRepositoryDirectory(),
                                         excludeConfig,
                                         true);
            
            this.repoCreated = true;

            this.PrepareRepository();

            RepositoryManager.Conclude();
            concluded = true;

        } catch (Exception e) {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine(e.StackTrace);
            throw;

        } finally {
            if (!concluded) {
                RepositoryManager.Conclude();
            }
            RepositoryManager.ClearThreadModuleName();
        }
    }

    /// <summary>
    /// This method can typically be called from a method annotated with
    /// <c>[OneTimeTearDown]</c>.
    /// </summary>
    ///
    /// <remarks>
    /// It will delete the entity repository that was created for the tests if
    /// there are no test failures recorded via <see cref="InrementFailureCount"/>.
    /// </remarks>
    protected void TeardownTestEnvironment() {
        int failureCount = this.GetFailureCount();
        TeardownTestEnvironment((failureCount == 0));
    }

    /// <summary>
    /// This method can typically be called from a method annotated with
    /// <c>[OneTimeTearDown]</c>.
    /// </summary>
    ///
    /// <remarks>
    /// It will optionally delete the entity repository that was created
    /// for the tests.
    /// </remarks>
    ///
    /// <param name="deleteRepository">
    /// <c>true</c> if the test repository should be deleted,
    /// otherwise <c>false</c>
    /// </param>
    protected void TeardownTestEnvironment(bool deleteRepository) {
        string? preserveProp
            = Environment.GetEnvironmentVariable("SENZING_TEST_PRESERVE_REPOS");
        if (preserveProp != null) {
            preserveProp = preserveProp.Trim().ToLower();
        }
        bool preserve = (preserveProp != null && preserveProp.Equals("true"));

        // cleanup the repo directory
        if (this.repoCreated && deleteRepository && !preserve) {
            DeleteRepository(this.repoDirectory);
        }
    }

    /// <summary>
    /// Checks if the specified path is a directory.
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns><c>true</c> if it is a directory, otherwise <c>false</c></returns>
    private static bool IsDirectory(string path)
    {
        FileAttributes attrs = File.GetAttributes(path);
        return (attrs & FileAttributes.Directory) == FileAttributes.Directory;
    }

    /// <summary>
    /// Deletes the repository at the specified repository directory.
    /// </summary>
    ///
    /// <param name="repoDirectory">
    ///  The repository directory to delete.
    /// </param>
    protected static void DeleteRepository(DirectoryInfo repoDirectory) {
        if (repoDirectory.Exists && IsDirectory(repoDirectory.FullName)) {
            repoDirectory.Delete(true);
        }
    }

    /// <summary>
    /// Override this function to prepare the repository by configuring
    /// data sources or loading records.
    /// </summary>
    ///
    /// <remarks>
    /// By default this function does nothing.  The repository directory
    /// can be obtained via <see cref="GetRepositoryDirectory"/>.
    /// </remarks>
    protected void PrepareRepository() {
        // do nothing
    }

    /// <summary>
    /// Increments the failure count and returns the new failure count.
    /// </summary>
    ///
    /// <returns>The new failure count.</returns>
    protected int IncrementFailureCount() {
        this.failureCount++;
        this.ConditionallyLogCounts(false);
        return this.failureCount;
    }

    /// <summary>
    /// Increments the success count and returns the new success count.
    /// </summary>
    ///
    /// <returns>The new success count.</returns>
    protected int IncrementSuccessCount() {
        this.successCount++;
        this.ConditionallyLogCounts(false);
        return this.successCount;
    }

    /// <summary>
    /// Conditionally logs the progress of the tests.
    /// </summary>
    /// 
    /// <param name="complete">
    /// <c>true</c> if tests are complete for this class, otherwise <c>false</c>.
    /// </param>
    protected void ConditionallyLogCounts(bool complete) {
        int successCount = this.GetSuccessCount();
        int failureCount = this.GetFailureCount();

        long now = Environment.TickCount;
        long lapse = (this.progressLogTimestamp > 0L)
                ? (now - this.progressLogTimestamp)
                : 0L;

        if (complete || (lapse > 30000L)) {
            Console.WriteLine(this.GetType().Name
                    + (complete ? " Complete: " : " Progress: ")
                    + successCount + " (succeeded) / " + failureCount
                    + " (failed)");
            this.progressLogTimestamp = now;
        }
        if (complete) {
            Console.WriteLine();
        }
        if (this.progressLogTimestamp < 0L) {
            this.progressLogTimestamp = now;
        }
    }

    /// <summary>
    /// Returns the current failure count.
    /// </summary>
    /// 
    /// <remarks>
    /// The failure count is incremented via <see cref="IncrementFailureCount"/>.
    /// </remarks>
    ///
    /// <returns>The current failure count.</returns>
    protected int GetFailureCount() {
        return this.failureCount;
    }

    /// <summary>
    /// Returns the current success count.
    /// </summary>
    ///
    /// <remarks>
    /// The success count is incremented via <see cref="IncrementSuccessCount"/>.
    /// </remarks>
    ///
    /// <returns>The current success count.</returns>
    protected int GetSuccessCount() {
        return this.successCount;
    }

    /// <summary>
    /// Wrapper function for performing a test that will first check if
    /// the native API is available via <see cref="AssumeNativeApiAvailable"/>
    /// and then record a success or failure.
    /// </summary>
    ///
    /// <param name="testFunction">The function to execute.</param>
    protected void PerformTest(Action testFunction) {
        bool success = false;
        try {
            testFunction();
            success = true;

        } catch (Exception e) {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine(e.StackTrace);
            string? fastFail = Environment.GetEnvironmentVariable("SENZING_TEST_FAST_FAIL");
            if ("true".Equals(fastFail)) {
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
            throw;

        } finally {
            if (!success) {
                this.IncrementFailureCount();
            } else {
                this.IncrementSuccessCount();
            }
        }
    }

    /// <summary>
    /// Validates the json data and ensures the expected JSON property keys are
    /// present and that no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="expectedKeys">
    /// The zero or more expected property keys.
    /// </param>
    public static void ValidateJsonDataMap(JsonObject       jsonData, 
                                           params string[]  expectedKeys) 
    {
        ValidateJsonDataMap(null,
                            jsonData,
                            true,
                            expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures the expected JSON property keys are
    /// present and that no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="testInfo">
    /// Additional test information to be logged with failures.
    /// </param>
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="expectedKeys">
    /// The zero or more expected property keys.
    /// </param>
    public static void ValidateJsonDataMap(string?          testInfo,
                                           JsonObject       jsonData,
                                           params string[]  expectedKeys)
    {
        ValidateJsonDataMap(testInfo, jsonData, true, expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures the expected JSON property keys are
    /// present and that, optionally, no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="strict">
    /// Whether or not property keys other than those specified are
    /// allowed to be present.
    /// </param>
    /// <param name="expectedKeys">
    /// The zero or more expected property keys -- these are either a minimum 
    /// or exact set depending on the <c>strict</c> parameter.
    /// </param>
    public static void ValidateJsonDataMap(JsonObject       jsonData,
                                           bool             strict,
                                           params string[]  expectedKeys) {
        ValidateJsonDataMap(null, jsonData, strict, expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures the expected JSON property keys are
    /// present and that, optionally, no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="testInfo">
    /// Additional test information to be logged with failures.
    /// </param>
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="strict">
    /// Whether or not property keys other than those specified are
    /// allowed to be present.
    /// </param>
    /// <param name="expectedKeys">
    /// The zero or more expected property keys -- these are either a minimum 
    /// or exact set depending on the <c>strict</c> parameter.
    /// </param>
    public static void ValidateJsonDataMap(string?          testInfo,
                                           JsonObject       jsonData,
                                           bool             strict,
                                           params string[]  expectedKeys) 
    {
        String suffix = (testInfo != null && testInfo.Trim().Length > 0)
                ? " ( " + testInfo + " )"
                : "";

        if (jsonData == null) {
            Assert.Fail("Expected json data but got null value" + suffix);
            throw new Exception("Expected json data but got null value" + suffix);
        }

        ISet<string> actualKeySet = new HashSet<string>();
        ISet<string> expectedKeySet = new HashSet<string>();
        IEnumerator<KeyValuePair<string,JsonNode?>> enumerator = jsonData.GetEnumerator();
        while (enumerator.MoveNext()) {
            actualKeySet.Add(enumerator.Current.Key);
        }
        foreach (string key in expectedKeys) {
            expectedKeySet.Add(key);
            if (!actualKeySet.Contains(key)) {
                Assert.Fail("JSON property missing from json data: " + key + " / " + jsonData
                            + suffix);
            }
        }
        if (strict && expectedKeySet.Count != actualKeySet.Count) {
            ISet<string> extraKeySet = new HashSet<string>(actualKeySet);
            foreach (string key in expectedKeySet) {
                extraKeySet.Remove(key);
            }
            Assert.Fail("Unexpected JSON properties in json data: " + extraKeySet + suffix);
        }

    }

    /// <summary>
    /// Validates the json data and ensures it is a collection of objects and the
    /// expected JSON property keys are present in the array objects and that no
    /// unexpected keys are present.
    /// </summary>
    ///
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="expectedKeys">The zero or more expected property keys.</param>
    public static void ValidateJsonDataMapArray(JsonArray       jsonData,
                                                params string[] expectedKeys)
    {
        ValidateJsonDataMapArray(null, jsonData, true, expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures it is a collection of objects and the
    /// expected JSON property keys are present in the array objects and that no
    /// unexpected keys are present.
    /// </summary>
    ///
    /// <param name="testInfo">
    /// Additional test information to be logged with failures.
    /// </param>
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="expectedKeys">The zero or more expected property keys.</param>
    public static void ValidateJsonDataMapArray(string?         testInfo,
                                                JsonArray       jsonData,
                                                params string[] expectedKeys) {
        ValidateJsonDataMapArray(testInfo, jsonData, true, expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures it is a collection of objects and the
    /// expected JSON property keys are present in the array objects and that,
    /// optionally, no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="strict">
    /// Whether or not property keys other than those specified are
    /// allowed to be present.
    /// </param>
    /// <param name="expectedKeys">The zero or more expected property keys.</param>
    public static void ValidateJsonDataMapArray(JsonArray       jsonData,
                                                bool            strict,
                                                params string[] expectedKeys) {
        ValidateJsonDataMapArray(null, jsonData, strict, expectedKeys);
    }

    /// <summary>
    /// Validates the json data and ensures it is a collection of objects and the
    /// expected JSON property keys are present in the array objects and that,
    /// optionally, no unexpected keys are present.
    /// </summary>
    ///
    /// <param name="testInfo">
    /// Additional test information to be logged with failures.
    /// </param>
    /// <param name="jsonData">The json data to validate.</param>
    /// <param name="strict">
    /// Whether or not property keys other than those specified are
    /// allowed to be present.
    /// </param>
    /// <param name="expectedKeys">The zero or more expected property keys.</param>
    /// </summary>
    public static void ValidateJsonDataMapArray(string?         testInfo,
                                                JsonArray       jsonData,
                                                bool            strict,
                                                params string[] expectedKeys)
    {
        string suffix = (testInfo != null && testInfo.Trim().Length > 0)
                ? " ( " + testInfo + " )"
                : "";

        ISet<string> expectedKeySet = new HashSet<string>();
        foreach (string key in expectedKeys) {
            expectedKeySet.Add(key);
        }

        for (int index = 0; index < jsonData.Count; index++) {
            JsonNode? node = jsonData[index];
            if (node == null) {
                throw new Exception("Null object in an array with valid index");
            }
            JsonObject obj = ((JsonNode) node).AsObject();
            
            ISet<string> actualKeySet = new HashSet<string>();
            IEnumerator<KeyValuePair<string,JsonNode?>> enumerator = obj.GetEnumerator();
            while (enumerator.MoveNext()) {
                actualKeySet.Add(enumerator.Current.Key);
            }
            foreach (string key in expectedKeySet) {
                if (!actualKeySet.Contains(key)) {
                    Assert.Fail("JSON property missing from json data array element: "
                                 + key + " / " + actualKeySet + suffix);
                }
            }
            if (strict && expectedKeySet.Count != actualKeySet.Count) {
                ISet<string> extraKeySet = new HashSet<string>(actualKeySet);
                foreach (string key in expectedKeySet) {
                    extraKeySet.Remove(key);
                } 
                Assert.Fail("Unexpected JSON properties in json data: " + extraKeySet + suffix);
            }
        }
    }

    /// <summary>
    /// Generates option combos for the specified variants.
    /// </summary>
    /// <param name="variants">The one or more lists of variants</param>
    protected static IList<System.Collections.IList> GenerateCombinations(
        params System.Collections.IList[] variants) 
    {
        // determine the total number of combinations
        int comboCount = 1;
        foreach (System.Collections.IList v in variants) {
            comboCount *= v.Count;
        }

        // determine the intervals for each variant
        IList<int> intervals = new List<int>(variants.Length);
        // iterate over the variants
        for (int index = 0; index < variants.Length; index++) {
            // default the interval count to one (1)
            int intervalCount = 1;

            // loop over the remaining variants after the current
            for (int index2 = index + 1; index2 < variants.Length; index2++) {
                // multiply the interval count by the remaining variant sizes
                intervalCount *= variants[index2].Count;
            }

            // add the interval count
            intervals.Add(intervalCount);
        }

        List<System.Collections.IList> optionCombos
            = new List<System.Collections.IList>(comboCount);
        for (int comboIndex = 0; comboIndex < comboCount; comboIndex++) {
            System.Collections.IList optionCombo
                = new ArrayList(variants.Length);

            for (int index = 0; index < variants.Length; index++) {
                int interval = intervals[index];
                int valueIndex = (comboIndex / interval) % variants[index].Count;
                optionCombo.Add(variants[index][valueIndex]);
            }

            optionCombos.Add(optionCombo);
        }

        return optionCombos;
    }

    /// <summary>
    /// Gets the variant possible values of booleans for the specified
    /// parameter count. This includes <c>null</c> values.
    /// </summary>
    ///
    /// <param name="paramCount">The number of boolean parameters.</param>
    /// <param name="includeNull">Whether or not to include null values.</param>
    ///
    /// <returns>The list of parameter value lists.</returns>
    protected static IList<IList<Boolean?>> GetBooleanVariants(int   paramCount, 
                                                               bool  includeNull) 
    {
        Boolean?[] boolsSansNull = [ true, false ];
        Boolean?[] boolsWithNull = [ null, true, false ];

        Boolean?[] booleanValues = (includeNull) ? boolsWithNull : boolsSansNull;
        
    
        int variantCount = (int) Math.Ceiling(Math.Pow(booleanValues.Length, paramCount));
        IList<IList<Boolean?>> variants = new List<IList<Boolean?>>(variantCount);

        // iterate over the variants
        for (int index1 = 0; index1 < variantCount; index1++) {
            // create the parameter list
            IList<Boolean?> parms = new List<Boolean?>(paramCount);

            // iterate over the parameter slots
            for (int index2 = 0; index2 < paramCount; index2++) {
                int repeat = (int) Math.Ceiling(Math.Pow(booleanValues.Length, index2));
                int valueIndex = (index1 / repeat) % booleanValues.Length;
                parms.Add(booleanValues[valueIndex]);
            }

            // add the combinatorial variant
            variants.Add(parms);
        }
        return variants;
    }

    /// <summary>
    /// Interface to mirror Java iterators
    /// </summary>
    public interface Iterator<T> {
        /// <summary>
        /// Checks if there is another element in the iteration.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if there is another element, otherwise <c>false</c>.
        /// </returns>
        bool HasNext();

        /// <summary>
        /// Gets the next element in the iteration.
        /// </summary>
        ///
        /// <returns>
        /// Returns the next element in the iteration.
        /// </returns>
        T Next();
    }

    /// <summary>
    /// Utility class for circular iteration.
    /// </summary>
    internal class CircularIterator<T> : Iterator<T> {
        private ICollection<T> collection;
        private IEnumerator<T> enumerator;

        internal CircularIterator(ICollection<T> collection) {
            this.collection = collection;
            this.enumerator = this.collection.GetEnumerator();
        }

        public bool HasNext() {
            return (this.collection.Count > 0);
        }

        public T Next() {
            if (!this.HasNext()) {
                throw new InvalidOperationException("No such element");
            }
            bool found = this.enumerator.MoveNext();
            if (!found) {
                this.enumerator.Reset();
                this.enumerator.MoveNext();
            }
            return this.enumerator.Current;
        }
    }

    /// <summary>
    /// Returns an iterator that iterates over the specified
    /// <see cref="System.Collections.Generic.ICollection"/>
    /// in a circular fashion.
    /// </summary>
    ///
    /// <param name="collection">
    /// The <see cref="System.Collections.Generic.ICollection"/> to iterate over.
    /// </param>
    ///
    /// <returns>The circular <see cref="Iterator"/></returns>
    protected static Iterator<T> GetCircularIterator<T>(ICollection<T> collection)
    {
        return new CircularIterator<T>(collection);
    }

    /// <summary>
    /// Creates a CSV temp file with the specified headers and records.
    /// </summary>
    ///
    /// <param name="filePrefix">The prefix for the temp file name.</param>
    /// <param name="headers">The CSV headers.</param>
    /// <param name="records">The one or more records.</param>
    ///
    /// <returns>The <see cref="System.IO.FileInfo"/> that was created.</returns>
    protected FileInfo PrepareCSVFile(string            filePrefix,
                                      string[]          headers,
                                      params string[][] records)
    {
        // check the arguments
        int count = headers.Length;
        for (int index = 0; index < records.Length; index++) {
            string[] record = records[index];
            if (record.Length != count) {
                throw new ArgumentException(
                    "The header and records do not all have the same number of "
                        + "elements.  expected=[ " + count + " ], received=[ "
                        + record.Length + " ], index=[ " + index + " ]");
            }
        }

        string fileName = filePrefix + Path.GetRandomFileName() + ".csv";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);

        FileStream? fs = null;
        
        // populate the file as a CSV
        try {
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs, UTF8);

            string prefix = "";
            foreach (string header in headers) {
                sw.Write(prefix);
                sw.Write(CSVQuote(header));
                prefix = ",";
            }
            sw.WriteLine();
            sw.Flush();

            foreach (String[] record in records) {
                prefix = "";
                foreach (string value in record) {
                    sw.Write(prefix);
                    sw.Write(CSVQuote(value));
                    prefix = ",";
                }
                sw.WriteLine();
                sw.Flush();
            }
            sw.Flush();

        } finally {
            if (fs != null) {
                fs.Close();
            }
        }

        return new FileInfo(filePath);
    }


    /// <summary>
    /// Creates a JSON array temp file with the specified headers and records.
    /// </summary>
    ///
    /// <param name="filePrefix">The prefix for the temp file name.</param>
    /// <param name="headers">The CSV headers.</param>
    /// <param name="records">The one or more records.</param>
    ///
    /// <returns>The <see cref="System.IO.FileInfo"/> that was created.</returns>
    protected FileInfo PrepareJsonArrayFile(string              filePrefix,
                                            string[]            headers,
                                            params string[][]   records) 
    {
        // check the arguments
        int count = headers.Length;
        for (int index = 0; index < records.Length; index++) {
            string[] record = records[index];
            if (record.Length != count) {
                throw new ArgumentException(
                    "The header and records do not all have the same number of "
                        + "elements.  expected=[ " + count + " ], received=[ "
                        + record.Length + " ], index=[ " + index + " ]");
            }
        }

        string fileName = filePrefix + Path.GetRandomFileName() + ".json";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        FileStream? fs = null;
        
        // populate the file as a CSV
        try {
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, UTF8);

            JsonArray jsonArr = new JsonArray();
            foreach (string[] record in records) {
                JsonObject jsonObj = new JsonObject();
                for (int index = 0; index < record.Length; index++) {
                    string key      = headers[index];
                    string value    = record[index];
                    jsonObj.Add(key, JsonValue.Create(value));
                }
                jsonArr.Add(jsonObj);
            }

            string jsonText = jsonArr.ToJsonString();
            sw.Write(jsonText);
            sw.Flush();

        } finally {
            if (fs != null) {
                fs.Close();
            }
        }

        return new FileInfo(filePath);
    }

    /// <summary>
    /// Creates a JSON temp file with the specified headers and records.
    /// </summary>
    ///
    /// <param name="filePrefix">The prefix for the temp file name.</param>
    /// <param name="headers">The CSV headers.</param>
    /// <param name="records">The one or more records.</param>
    ///
    /// <returns>
    /// The <see cref="System.IO.FileInfo"/> for the file that was created.
    /// </returns>
    protected FileInfo PrepareJsonFile(string               filePrefix,
                                       string[]             headers,
                                       params string[][]    records) {
        // check the arguments
        int count = headers.Length;
        for (int index = 0; index < records.Length; index++) {
            string[] record = records[index];
            if (record.Length != count) {
                throw new ArgumentException(
                    "The header and records do not all have the same number of "
                        + "elements.  expected=[ " + count + " ], received=[ "
                        + record.Length + " ], index=[ " + index + " ]");
            }
        }

        string fileName = filePrefix + Path.GetRandomFileName() + ".json";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        FileStream? fs = null;

        // populate the file as one JSON record per line
        try {
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, UTF8);

            foreach (string[] record in records) {
                JsonObject jsonObj = new JsonObject();
                for (int index = 0; index < record.Length; index++) {
                    string key      = headers[index];
                    string value    = record[index];
                    jsonObj.Add(key, JsonValue.Create(value));
                }
                string jsonText = jsonObj.ToJsonString();
                sw.WriteLine(jsonText);
                sw.Flush();
            }
            sw.Flush();

        } finally {
            if (fs != null) {
                fs.Close();
            }
        }

        return new FileInfo(filePath);
    }

    /// <summary>
    /// Creates a JSON-lines temp file with the specified headers and records.
    /// </summary>
    ///
    /// <param name="filePrefix">The prefix for the temp file name.</param>
    /// <param name="jsonArray">
    /// The <see cref="System.Text.Json.Nodes.JsonArray"/> describing the records.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="System.IO.FileInfo"/> for the file that was created.
    /// </returns>
    protected FileInfo PrepareJsonArrayFile(string      filePrefix,
                                            JsonArray   jsonArray) 
    {
        string fileName = filePrefix + Path.GetRandomFileName() + ".json";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        FileStream? fs = null;

        // populate the file as one JSON record per line
        try {
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, UTF8);

            String jsonText = jsonArray.ToJsonString();
            sw.WriteLine(jsonText);
            sw.Flush();

        } finally {
            if (fs != null) {
                fs.Close();
            }
        }

        return new FileInfo(filePath);
    }

    /// <summary>
    /// Creates a JSON-lines temp file with the specified headers and records.
    /// </summary>
    ///
    /// <param name="filePrefix">The prefix for the temp file name.</param>
    /// <param name="jsonArray">
    /// The <see cref="System.Text.Json.Nodes.JsonArray"/> describing the records.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="System.IO.FileInfo"/> for the file that was created.
    /// </returns>
    protected FileInfo PrepareJsonFile(string       filePrefix,
                                       JsonArray    jsonArray) 
    {
        string fileName = filePrefix + Path.GetRandomFileName() + ".json";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        FileStream? fs = null;

        // populate the file as one JSON record per line
        try {
            fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, UTF8);
            
            for (int index = 0; index < jsonArray.Count; index++) {
                JsonNode? node = jsonArray[index];
                if (node == null) {
                    throw new Exception("Null object in an array with valid index");
                }
                JsonObject record = ((JsonNode) node).AsObject();

                string jsonText = record.ToJsonString();
                sw.WriteLine(jsonText);
                sw.Flush();
            }
            sw.Flush();
        } finally {
            if (fs != null) {
                fs.Close();
            }
        }

        return new FileInfo(filePath);
    }

    /// <summary>
    /// Quotes the specified text as a quoted string for a CSV value or header.
    /// </summary>
    ///
    /// <param name="text">The text to be quoted.</param>
    ///
    /// <returns>The quoted text</returns>
    protected string CSVQuote(string text) {
        if (text.IndexOf("\"") < 0 && text.IndexOf("\\") < 0) {
            return "\"" + text + "\"";
        }
        char[] textChars = text.ToCharArray();
        StringBuilder sb = new StringBuilder(text.Length * 2);
        foreach (char c in textChars) {
            if (c == '"' || c == '\\') {
                sb.Append('\\');
            }
            sb.Append(c);
        }
        return sb.ToString();
    }
}
}