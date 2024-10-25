using Senzing.Sdk.Core;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Senzing.Sdk.Tests.NativeSzApi;
using Senzing.Sdk.Tests.IO;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace Senzing.Sdk.Tests.Core
{
public class RepositoryManager
{
    /// <summary>
    /// UTF8 encoding constant.
    /// </summary>
    private static readonly Encoding UTF8 = new UTF8Encoding();

    private const string SQLITE_SCHEMA_FILE_NAME
        = "szcore-schema-sqlite-create.sql";

    private static readonly DirectoryInfo? INSTALL_DIR;

    private static readonly DirectoryInfo? RESOURCE_DIR;

    private static readonly DirectoryInfo? SUPPORT_DIR;

    private static readonly DirectoryInfo? TEMPLATES_DIR;

    private static readonly NativeEngine ENGINE_API;

    private static readonly NativeDiagnostic DIAGNOSTIC_API;

    private static readonly NativeConfig CONFIG_API;

    private static readonly NativeConfigManager CONFIG_MGR_API;

    private static readonly FrozenSet<string> EXCLUDED_TEMPLATE_FILES;

    private static readonly InstallLocations? INSTALL_LOCATIONS;

    private static readonly ThreadLocal<string?> THREAD_MODULE_NAME
        = new ThreadLocal<string?>();

    private static readonly object MONITOR = new object();

    private static string? baseInitializedWith = null;
    private static string? engineInitializedWith = null;

    private static readonly IDictionary<FileInfo, FileInfo>
        TEMPLATE_DATABASE_MAP = new Dictionary<FileInfo, FileInfo>();

    static RepositoryManager()
    {
        try
        {
            ISet<string> set = new HashSet<string>();
            set.Add("G2Module.ini".ToLower());
            set.Add("G2Project.ini".ToLower());
            set.Add("G2C.db".ToLower());
            set.Add("g2config.json".ToLower());
            EXCLUDED_TEMPLATE_FILES = set.ToFrozenSet();

            INSTALL_LOCATIONS = InstallLocations.FindLocations();


            if (INSTALL_LOCATIONS != null)
            {
                INSTALL_DIR = INSTALL_LOCATIONS.GetInstallDirectory();
                SUPPORT_DIR = INSTALL_LOCATIONS.GetSupportDirectory();
                RESOURCE_DIR = INSTALL_LOCATIONS.GetResourceDirectory();
                TEMPLATES_DIR = INSTALL_LOCATIONS.GetTemplatesDirectory();


                Console.Error.WriteLine();
                Console.Error.WriteLine("--------------------------------------------");
                Console.Error.WriteLine("Senzing Install Directory   : " + INSTALL_DIR);
                Console.Error.WriteLine("Senzing Support Directory   : " + SUPPORT_DIR);
                Console.Error.WriteLine("Senzing Resource Directory  : " + RESOURCE_DIR);
                Console.Error.WriteLine("Senzing Templates Directory : " + TEMPLATES_DIR);
                Console.Error.WriteLine();

            }
            else
            {
                INSTALL_DIR = null;
                SUPPORT_DIR = null;
                RESOURCE_DIR = null;
                TEMPLATES_DIR = null;
            }

            try
            {
                ENGINE_API = NativeApiFactory.CreateEngineApi();
                DIAGNOSTIC_API = NativeApiFactory.CreateDiagnosticApi();
                CONFIG_API = NativeApiFactory.CreateConfigApi();
                CONFIG_MGR_API = NativeApiFactory.CreateConfigMgrApi();

            }
            catch (Exception e)
            {
                DirectoryInfo libPath = (INSTALL_DIR == null)
                    ? new DirectoryInfo("lib")
                    : new DirectoryInfo(
                        Path.Combine(INSTALL_DIR.FullName, "lib"));

                Console.Error.WriteLine(e);
                Console.Error.WriteLine();
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        Console.Error.WriteLine("Failed to load native G2.dll library.");
                        Console.Error.WriteLine(
                            "Check PATH environment variable for " + libPath);
                        break;
                    case PlatformID.MacOSX:
                        Console.Error.WriteLine("Failed to load native libG2.so library");
                        Console.Error.WriteLine(
                            "Check DYLD_LIBRARY_PATH environment variable for: ");
                        Console.Error.WriteLine("     - " + libPath);
                        Console.Error.WriteLine("     - " + Path.Combine(libPath.FullName, "macos"));
                        break;
                    case PlatformID.Unix:
                        Console.Error.WriteLine("Failed to load native libG2.so library");
                        Console.Error.WriteLine(
                            "Check LD_LIBRARY_PATH environment variable for: ");
                        Console.Error.WriteLine("     - " + libPath);
                        Console.Error.WriteLine("     - " + Path.Combine(libPath.FullName, "debian"));
                        break;
                    default:
                        break;
                }
                throw;
            }
        }
        catch (Exception e) {
            Console.Error.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Copies the config template files from the template directory to the
    /// specified configuration directory (creating the directory if necessary).
    /// </summary>
    /// 
    /// <param name="templateDir">
    /// The template directory to get the templates from.
    /// </param>
    /// 
    /// <param name="configDir">
    /// The config directory to copy the files to.
    /// </param>
    /// 
    /// <exception cref="System.IO.IOException">
    /// IOException If an I/O failure occurs.
    /// </summary>
    public static void CopyConfigFiles(DirectoryInfo? templateDir,
                                        DirectoryInfo configDir)
    {
        if (templateDir != null)
        {
            List<FileInfo> templateFiles = new List<FileInfo>();
            string[] files = Directory.GetFiles(templateDir.FullName);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.EndsWith(".template")
                    && !EXCLUDED_TEMPLATE_FILES.Contains(fileInfo.Name.ToLower()))
                {
                    templateFiles.Add(fileInfo);
                }
            }

            if (templateFiles.Count > 0)
            {
                if (!configDir.Exists)
                {
                    Directory.CreateDirectory(configDir.FullName);
                }
                foreach (FileInfo templateFile in templateFiles)
                {
                    FileInfo targetFile = new FileInfo(
                        Path.Combine(configDir.FullName, templateFile.Name));
                    File.Copy(templateFile.FullName, targetFile.FullName);
                }
            }
        }
    }

    /// <summary>
    /// Describes a repository configuration.
    /// </summary>
    public class Configuration
    {
        private long configId;
        private JsonObject configJson;

        /// <summary>
        /// Constructs with the specified config ID and
        /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
        /// </summary>
        /// 
        /// <param name="configId">The config ID for the configuration.</param>
        /// 
        /// <param name="configJson">
        /// The <see cref="System.Text.Json.Nodes.JsonObject"/> for the configuraiton.
        /// </param>
        public Configuration(long configId, JsonObject configJson)
        {
            this.configId = configId;
            this.configJson = configJson;
        }

        /// <summary>
        /// Returns the configuration ID.
        /// </summary>
        /// 
        /// <returns>The configuration ID.</returns>
        public long GetConfigId() {
            return this.configId;
        }

        /// <summary>
        /// Returns the configuration JSON as a
        /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="System.Text.Json.Nodes.JsonObject"/> describing
        /// the configuration.
        /// </summary>
        public JsonObject GetConfigJson() {
            return this.configJson;
        }
    }

    /// <summary>
    /// Use this method in conjunction with <see cref="ClearThreadModuleName"/> 
    /// to provide a specific module name for the repository manager to use
    /// when initializing the native API's.
    /// </summary>
    /// 
    /// <param name="moduleName">
    /// The module name to initialize with, or <c>null</c> to do the equivalent
    /// of clearing the name.
    /// </param>
    public static void SetThreadModuleName(string? moduleName)
    {
        RepositoryManager.THREAD_MODULE_NAME.Value = moduleName;
    }

    /// <summary>
    /// Clears any previously set thread module name.
    /// </summary>
    /// 
    /// <remarks>
    /// This method should be called in a "finally" block.
    /// </remarks>
    public static void ClearThreadModuleName()
    {
        RepositoryManager.SetThreadModuleName(null);
    }


    /// <summary>
    /// Creates a new Senzing SQLite repository from the default repository data.
    /// </summary>
    /// 
    /// <param name="directory">
    /// The directory at which to create the repository.
    /// </param>
    /// 
    /// <returns>
    /// The <see cref="Configuration"/> describing the initial configuration.
    /// </returns>
    public static Configuration? CreateRepo(DirectoryInfo directory)
    {
        return CreateRepo(directory, false);
    }

    /// <summary>
    /// Creates a new Senzing SQLite repository from the default repository data.
    /// </summary>
    /// 
    /// <param name="directory">
    /// The directory at which to create the repository.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the initial configuration.
    /// </returns>
    public static Configuration? CreateRepo(DirectoryInfo directory, bool silent)
    {
        return CreateRepo(directory, false, false);
    }

    /// <summary>
    /// Creates a temporary SQLite database file with the G2 schema laid down.
    /// </summary>
    /// 
    /// <param name="schemaFile">
    /// The schema file containing the SQL DDL.
    /// </param>
    /// 
    /// <returns>The SQLite database file.</returns>
    /// 
    /// <exception cref="System.IO.IOException">
    /// If an IO failure occurs.
    /// </exception>
    /// 
    /// <exception cref="System.IO.FileNotFoundException">
    /// If the specified schema file is not found.
    /// </exception>
    /// 
    /// <exception cref="System.Data.Common.DbException">
    /// If a database failure occurs.
    /// </exception>
    private static FileInfo CreateTemplateDatabase(FileInfo schemaFile)
    {
        if (!schemaFile.Exists)
        {
            throw new FileNotFoundException(
                "Schema file does not exist: " + schemaFile);
        }

        // check if we already created the template database for the
        // specified schema file
        lock (TEMPLATE_DATABASE_MAP)
        {
            FileInfo templateDatabase = TEMPLATE_DATABASE_MAP[schemaFile];
            if (templateDatabase != null)
            {
                return templateDatabase;
            }
        }

        SqliteConnection? sqlite = null;
        try
        {
            string databaseFile = Path.GetTempFileName();
            String connectSpec = "Data Source=" + databaseFile;

            sqlite = new SqliteConnection(connectSpec);
            sqlite.Open();
            SqliteCommand cmd = sqlite.CreateCommand();

            string[] sqlLines = File.ReadAllLines(schemaFile.FullName, UTF8);

            foreach (string sql in sqlLines)
            {
                if (sql.Trim().Length == 0) continue;
                cmd.CommandText = sql.Trim();
                cmd.ExecuteNonQuery();
            }

            // return the file
            return new FileInfo(databaseFile);

        }
        finally
        {
            if (sqlite != null)
            {
                sqlite.Close();
            }
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
    /// Creates a new Senzing SQLite repository from the default repository data.
    /// </summary>
    /// 
    /// <param name="directory">
    /// The directory at which to create the repository.
    /// </param>
    /// 
    /// <param name="excludeConfig">
    /// <c>true</c> if the default configuration should be excluded from the
    /// repository, and <c>false</c> if it should be included.
    /// </param>
    /// 
    /// <returns>
    /// The <see cref="Configuration"/> describing the initial configuration.
    /// </returns>
    public static Configuration? CreateRepo(DirectoryInfo directory,
                                            bool excludeConfig,
                                            bool silent)
    {
        Configuration? result = null;

        if (directory.Exists)
        {
            if (!IsDirectory(directory.FullName))
            {
                throw new ArgumentException(
                    "Repository directory exists and is not a directory: " + directory);
            }
            string[] files = Directory.GetFiles(directory.FullName);
            if (files.Length > 0)
            {
                throw new ArgumentException(
                    "Repository directory exists and is not empty: "
                        + directory.FullName + " / " + files[0]);
            }
        }
        try
        {
            Directory.CreateDirectory(directory.FullName);
            DirectoryInfo repoConfigDir = new DirectoryInfo(
                Path.Combine(directory.FullName, "etc"));
            CopyConfigFiles(TEMPLATES_DIR, repoConfigDir);

            // find the template DB file
            FileInfo? templateDB = null;
            if (INSTALL_LOCATIONS != null && INSTALL_LOCATIONS.IsDevelopmentBuild())
            {
                DirectoryInfo? resourceDir = INSTALL_LOCATIONS.GetResourceDirectory();

                DirectoryInfo? schemaDir = (resourceDir == null)
                    ? null
                    : new DirectoryInfo(Path.Combine(resourceDir.FullName, "schema"));

                FileInfo? sqliteFile = (schemaDir == null)
                    ? null
                    : new FileInfo(Path.Combine(schemaDir.FullName,
                                                SQLITE_SCHEMA_FILE_NAME));

                FileInfo? tmp = (sqliteFile == null)
                    ? null : CreateTemplateDatabase(sqliteFile);

                // set the template DB to the temp file
                templateDB = tmp;

            }
            else
            {
                templateDB = (TEMPLATES_DIR != null)
                    ? new FileInfo(Path.Combine(TEMPLATES_DIR.FullName, "G2C.db"))
                    : new FileInfo(Path.Combine(
                        ((SUPPORT_DIR == null) ? "" : SUPPORT_DIR.FullName), "G2C.db"));

                if (!templateDB.Exists && SUPPORT_DIR != null)
                {
                    templateDB = new FileInfo(
                        Path.Combine(SUPPORT_DIR.FullName, "G2C.db"));
                }
                if (!templateDB.Exists && INSTALL_LOCATIONS != null)
                {
                    DirectoryInfo? resourceDir = INSTALL_LOCATIONS.GetResourceDirectory();

                    DirectoryInfo? schemaDir = (resourceDir == null)
                        ? null
                        : new DirectoryInfo(
                            Path.Combine(resourceDir.FullName, "schema"));

                    FileInfo? sqliteFile = (schemaDir == null)
                        ? null
                        : new FileInfo(
                            Path.Combine(schemaDir.FullName, SQLITE_SCHEMA_FILE_NAME));

                    templateDB = (sqliteFile == null)
                        ? null : CreateTemplateDatabase(sqliteFile);
                }
            }

            if (templateDB != null && templateDB.Exists)
            {
                // copy the file
                CopyFile(templateDB, new FileInfo(Path.Combine(directory.FullName,
                                                                "G2C.db")));
                CopyFile(templateDB, new FileInfo(Path.Combine(directory.FullName,
                                                                "G2_RES.db")));
                CopyFile(templateDB, new FileInfo(Path.Combine(directory.FullName,
                                                                "G2_LIB_FEAT.db")));
            }
            else
            {
                // handle running in mock replay mode (no installation)
                TouchFile(Path.Combine(directory.FullName, "G2C.db"));
                TouchFile(Path.Combine(directory.FullName, "G2_RES.db"));
                TouchFile(Path.Combine(directory.FullName, "G2_LIB_FEAT.db"));
            }

            // define the license path
            FileInfo? licensePath = null;

            // check if there is a license file in the installation
            if (INSTALL_LOCATIONS != null)
            {
                DirectoryInfo? installDir = INSTALL_LOCATIONS.GetInstallDirectory();

                DirectoryInfo? etcDir = (installDir == null)
                    ? null : new DirectoryInfo(Path.Combine(installDir.FullName, "etc"));

                licensePath = (etcDir == null)
                    ? null : new FileInfo(Path.Combine(etcDir.FullName, "g2.lic"));
            }

            // if no existing license then set a license path in the repo directory
            if (licensePath == null || !licensePath.Exists) {
                licensePath = new FileInfo(Path.Combine(directory.FullName, "g2.lic"));
            }

            string fileSep = "" + Path.DirectorySeparatorChar;
            string sqlitePrefix = "sqlite3://na:na@" + directory.FullName + fileSep;

            FileInfo jsonInitFile = new FileInfo(
                Path.Combine(directory.FullName, "sz-init.json"));

            IDictionary<string, JsonNode?> subDict
                = new Dictionary<string, JsonNode?>();

            IDictionary<string, JsonNode?> objDict
                = new Dictionary<string, JsonNode?>();

            if (SUPPORT_DIR != null)
            {
                subDict.Add("SUPPORTPATH", JsonValue.Create(SUPPORT_DIR.FullName).Root);
            }
            if (RESOURCE_DIR != null)
            {
                subDict.Add("RESOURCEPATH", JsonValue.Create(RESOURCE_DIR.FullName).Root);
            }
            subDict.Add("CONFIGPATH", JsonValue.Create(repoConfigDir.FullName).Root);

            objDict.Add("PIPELINE", new JsonObject(subDict));

            subDict = new Dictionary<string, JsonNode?>();

            subDict.Add("BACKEND", JsonValue.Create("HYBRID").Root);
            subDict.Add("CONNECTION", JsonValue.Create(sqlitePrefix + "G2C.db"));

            objDict.Add("SQL", new JsonObject(subDict));

            subDict = new Dictionary<string, JsonNode?>();

            subDict.Add("RES_FEAT", JsonValue.Create("C1").Root);
            subDict.Add("RES_FEAT_EKEY", JsonValue.Create("C1").Root);
            subDict.Add("RES_FEAT_LKEY", JsonValue.Create("C1").Root);
            subDict.Add("RES_FEAT_STAT", JsonValue.Create("C1").Root);
            subDict.Add("LIB_FEAT", JsonValue.Create("C2").Root);
            subDict.Add("LIB_FEAT_HKEY", JsonValue.Create("C2").Root);
            objDict.Add("HYBRID", new JsonObject(subDict));

            subDict = new Dictionary<string, JsonNode?>();

            subDict.Add("CLUSTER_SIZE", JsonValue.Create("1").Root);
            subDict.Add("DB_1", JsonValue.Create(sqlitePrefix + "G2_RES.db").Root);
            objDict.Add("C1", new JsonObject(subDict));

            subDict = new Dictionary<string, JsonNode?>();

            subDict.Add("CLUSTER_SIZE", JsonValue.Create("1").Root);
            subDict.Add("DB_1", JsonValue.Create(sqlitePrefix + "G2_LIB_FEAT.db").Root);
            objDict.Add("C2", new JsonObject(subDict));

            JsonObject initJson = new JsonObject(objDict);

            JsonSerializerOptions options
                = new JsonSerializerOptions(JsonSerializerOptions.Default);
            options.WriteIndented = true;

            String initJsonText = initJson.ToJsonString(options);

            File.WriteAllText(jsonInitFile.FullName, initJsonText, UTF8);

            if (!excludeConfig)
            {
                // setup the initial configuration
                InitBaseApis(directory, false);
                try
                {
                    long returnCode = CONFIG_API.Create(out IntPtr configHandle);
                    if (returnCode != 0)
                    {
                        Console.Error.WriteLine("RETURN CODE: " + returnCode);
                        String msg = LogError("NativeConfig.create()", CONFIG_API);
                        throw new InvalidOperationException(msg);
                    }
                    returnCode = CONFIG_API.Save(configHandle, out string configJsonText);
                    if (returnCode != 0)
                    {
                        String msg = LogError("NativeConfig.save()", CONFIG_API);
                        throw new InvalidOperationException(msg);
                    }
                    CONFIG_API.Close(configHandle);

                    JsonNode? node = JsonNode.Parse(configJsonText);
                    if (node == null) {
                        throw new JsonException("Failed to parse config JSON: " + configJsonText);
                    }
                    JsonObject resultConfig = ((JsonNode) node).AsObject();

                    returnCode = CONFIG_MGR_API.AddConfig(configJsonText,
                                                          "Initial Config",
                                                          out long resultConfigId);
                    if (returnCode != 0)
                    {
                        String msg = LogError("NativeConfigMgr.addConfig()",
                                                CONFIG_MGR_API);
                        throw new InvalidOperationException(msg);
                    }

                    returnCode = CONFIG_MGR_API.SetDefaultConfigID(resultConfigId);
                    if (returnCode != 0)
                    {
                        String msg = LogError("NativeConfigMgr.setDefaultConfigID()",
                                                CONFIG_MGR_API);
                        throw new InvalidOperationException(msg);
                    }

                    result = new Configuration(resultConfigId, resultConfig);

                }
                finally
                {
                    DestroyBaseApis();
                }
            }

            if (!silent)
            {
                Console.Error.WriteLine("Entity repository created at: " + directory);
            }

            return result;

        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Directory.Delete(directory.FullName, true);
            throw;
        }
    }

    private static FileInfo TouchFile(string path)
    {
        File.Create(path).Dispose();
        return new FileInfo(path);
    }

    private static void CopyFile(FileInfo source, FileInfo target)
    {
        File.Copy(source.FullName, target.FullName, true);
    }

    private static void InitBaseApis(DirectoryInfo repository, bool verbose)
    {
        lock (MONITOR)
        {
            string? moduleName = THREAD_MODULE_NAME.Value;
            if (moduleName == null) moduleName = "Sz Repository Manager";
            string initializer = verbose + ":" + repository.FullName;
            if (baseInitializedWith == null
                || !baseInitializedWith.Equals(initializer))
            {
                if (baseInitializedWith != null)
                {
                    DestroyBaseApis();
                }

                string iniJsonFile = Path.Combine(repository.FullName,
                                                    "sz-init.json");

                String initJsonText = File.ReadAllText(iniJsonFile, UTF8);
                long returnCode = CONFIG_API.Init(moduleName, initJsonText, verbose);
                if (returnCode != 0L)
                {
                    LogError("NativeConfig.init()", CONFIG_API);
                    throw new InvalidOperationException(initJsonText);
                }
                returnCode = CONFIG_MGR_API.Init(moduleName, initJsonText, verbose);
                if (returnCode != 0)
                {
                    CONFIG_API.Destroy();
                    LogError("NativeConfigMgr.init()", CONFIG_MGR_API);
                    throw new InvalidOperationException(initJsonText);
                }
                baseInitializedWith = initializer;
            }
        }
    }

    private static void InitApis(DirectoryInfo repository, bool verbose)
    {
        lock (MONITOR)
        {
            string? moduleName = THREAD_MODULE_NAME.Value;
            if (moduleName == null) moduleName = "Sz Repository Manager";

            InitBaseApis(repository, verbose);

            String initializer = verbose + ":" + repository.FullName;
            if (engineInitializedWith == null
                || !engineInitializedWith.Equals(initializer))
            {
                if (engineInitializedWith != null)
                {
                    DestroyApis();
                }

                string iniJsonFile = Path.Combine(repository.FullName, "sz-init.json");
                String initJsonText = File.ReadAllText(iniJsonFile, UTF8);
                long returnCode = ENGINE_API.Init(moduleName, initJsonText, verbose);
                if (returnCode != 0)
                {
                    DestroyBaseApis();
                    LogError("NativeEngine.init()", ENGINE_API);
                    throw new InvalidOperationException(initJsonText);
                }
                engineInitializedWith = initializer;
            }
        }
    }

    private static void DestroyBaseApis()
    {
        lock (MONITOR)
        {
            if (baseInitializedWith != null)
            {
                CONFIG_API.Destroy();
                CONFIG_MGR_API.Destroy();
                baseInitializedWith = null;
            }
        }
    }


    private static void DestroyApis()
    {
        lock (MONITOR)
        {
            if (engineInitializedWith != null)
            {
                ENGINE_API.Destroy();
                engineInitializedWith = null;
            }
            DestroyBaseApis();
        }
    }

    /// <summary>
    /// Shuts down the repository manager after use to ensure the native
    /// </summary>
    /// <remarks>
    /// Senzing API destroy() functions are called.
    /// </remarks>
    public static void Conclude()
    {
        DestroyApis();
    }

    private static String LogError(String operation, NativeApi nativeApi)
    {
        String errorMsg = LoggingUtilities.FormatError(operation, nativeApi, true);
        Console.Error.WriteLine();
        Console.Error.WriteLine(errorMsg);
        Console.Error.WriteLine();
        return errorMsg;
    }

    private static ISet<string> GetDataSources()
    {
        IntPtr? configHandle = null;
        try {
            ISet<string> set = GetDataSources(out IntPtr handle);
            configHandle = handle;
            return set;

        } finally {
            if (configHandle != null) {
                CONFIG_API.Close((IntPtr) configHandle);
            }
        }
    }

    private static IntPtr LoadActiveConfig()
    {
        long returnCode = ENGINE_API.GetActiveConfigID(out long configId);
        if (returnCode != 0)
        {
            LogError("NativeEngine.GetActiveConfig()", ENGINE_API);
            throw new Exception("Failed engine operation");
        }
        returnCode = CONFIG_MGR_API.GetConfig(configId, out string configJson);
        if (returnCode != 0)
        {
            LogError("NativeEngine.ExportConfig()", ENGINE_API);
            throw new Exception("Failed engine operation");
        }
        returnCode = CONFIG_API.Load(configJson, out IntPtr handle);
        if (returnCode != 0L)
        {
            LogError("NativeConfig.Load()", CONFIG_API);
            throw new Exception("Failed engine operation");
        }
        return handle;
    }

    private static ISet<string> GetDataSources(out IntPtr configHandle)
    {
        configHandle = LoadActiveConfig();

        return DoGetDataSources(configHandle);
    }

    private static ISet<string> DoGetDataSources(IntPtr configHandle)
    {
        long returnCode = CONFIG_API.ListDataSources(
            configHandle, out string resultJson);
        if (returnCode != 0)
        {
            LogError("NativeConfig.listDataSources()", CONFIG_API);
            throw new Exception("Failed NativeConfig.listDataSources()");
        }

        ISet<string> existingSet = new HashSet<string>();

        // parse the raw data
        JsonDocument jsonDoc = JsonDocument.Parse(resultJson);
        JsonElement jsonArray = jsonDoc.RootElement.GetProperty("DATA_SOURCES");
        foreach (JsonElement elem in jsonArray.EnumerateArray())
        {
            string? code = elem.GetProperty("DSRC_CODE").GetString();
            if (code != null) existingSet.Add(code);
        }

        return existingSet;
    }

    /// <summary>
    /// Purges the repository that resides at the specified repository directory.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    public static void PurgeRepo(DirectoryInfo repository)
    {
        PurgeRepo(repository, false);
    }

    /// <summary>
    /// Purges the repository that resides at the specified repository directory.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    public static void PurgeRepo(DirectoryInfo repository, bool verbose)
    {
        PurgeRepo(repository, verbose, false);
    }

    /// <summary>
    /// Purges the repository that resides at the specified repository directory.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    public static void PurgeRepo(DirectoryInfo repository, bool verbose, bool silent)
    {
        InitApis(repository, verbose);
        long result = DIAGNOSTIC_API.PurgeRepository();
        if (result != 0) {
            LogError("NativeEngine.purgeRepository()", ENGINE_API);
        } else if (!silent) {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Repository purged: " + repository);
            Console.Error.WriteLine();
        }
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                FileInfo        sourceFile,
                                string?         dataSource)
    {
        return LoadFile(repository,
                        false,
                        sourceFile,
                        dataSource,
                        null,
                        null,
                        false);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                bool            silent)
    {
        return LoadFile(repository,
                        false,
                        sourceFile,
                        dataSource,
                        null,
                        null,
                        silent);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="loadedCount">
    /// The output result parameter for the number successfully loaded.
    /// </param>
    /// 
    /// <param name="failedCount">
    /// The output result parameter for the number that failed to load.
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                Result<int?>    loadedCount,
                                Result<int?>    failedCount)
    {
        return LoadFile(repository,
                        false,
                        sourceFile,
                        dataSource,
                        loadedCount,
                        failedCount,
                        false);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="loadedCount">
    /// The output result parameter for the number successfully loaded.
    /// </param>
    /// 
    /// <param name="failedCount">
    /// The output result parameter for the number that failed to load.
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                Result<int?>    loadedCount,
                                Result<int?>    failedCount,
                                bool            silent)
    {
        return LoadFile(repository,
                        false,
                        sourceFile,
                        dataSource,
                        loadedCount,
                        failedCount,
                        silent);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                bool            verbose,
                                FileInfo        sourceFile,
                                string?         dataSource)
    {
        return LoadFile(repository,
                        verbose,
                        sourceFile,
                        dataSource,
                        null,
                        null,
                        false);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                bool            verbose,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                bool            silent)
    {
        return LoadFile(repository,
                        verbose,
                        sourceFile,
                        dataSource,
                        null,
                        null,
                        silent);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="loadedCount">
    /// The output result parameter for the number successfully loaded.
    /// </param>
    /// 
    /// <param name="failedCount">
    /// The output result parameter for the number that failed to load.
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                bool            verbose,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                Result<int?>?   loadedCount,
                                Result<int?>?   failedCount)
    {
        return LoadFile(repository,
                        verbose,
                        sourceFile,
                        dataSource,
                        loadedCount,
                        failedCount,
                        false);
    }

    /// <summary>
    /// Loads a single CSV or JSON file to the repository -- optionally setting
    /// the data source for all the records.  NOTE: if the records in the file do
    /// not have a defined DATA_SOURCE then the specified data source is required.
    /// </summary>
    /// 
    /// <param name="repository">The directory for the repository.</param>
    /// 
    /// <param name="verbose">
    /// <c>true</c> for verbose API logging, otherwise <c>false</c>
    /// </param>
    /// 
    /// <param name="sourceFile">The source file to load (JSON or CSV).</param>
    /// 
    /// <param name="dataSource">
    /// The data source to use for loading the records.
    /// </param>
    /// 
    /// <param name="loadedCount">
    /// The output result parameter for the number successfully loaded.
    /// </param>
    /// 
    /// <param name="failedCount">
    /// The output result parameter for the number that failed to load.
    /// </param>
    /// 
    /// <param name="silent">
    /// <c>true</c> if no feedback should be given to the user upon completion,
    /// otherwise <c>false</c>
    /// </param>
    /// 
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool LoadFile(DirectoryInfo   repository,
                                bool            verbose,
                                FileInfo        sourceFile,
                                string?         dataSource,
                                Result<int?>?   loadedCount,
                                Result<int?>?   failedCount,
                                bool            silent)
    {
        string normalizedFileName = sourceFile.FullName.ToUpper();
        if ((!normalizedFileName.EndsWith(".JSON"))
            && (!normalizedFileName.EndsWith(".CSV")))
        {
            throw new ArgumentException(
                "File must be a CSV or JSON file: " + sourceFile);
        }

        InitApis(repository, verbose);

        if (loadedCount != null) loadedCount.SetValue(0);
        if (failedCount != null) failedCount.SetValue(0);
        if (dataSource != null) dataSource = dataSource.ToUpper();

        ISet<string> dataSources = GetDataSources();
        // check if the data source is configured
        if (dataSource != null && !dataSources.Contains(dataSource))
        {
            if (!AddDataSource(repository, dataSource, verbose)) return false;
            dataSources.Add(dataSource);
        }

        RecordReader? recordReader = null;
        // check the file type
        if (normalizedFileName.EndsWith(".JSON")) {
            recordReader = ProvideJsonRecords(sourceFile, dataSource);
            
        } else if (normalizedFileName.EndsWith(".CSV")) {
            recordReader = ProvideCsvRecords(sourceFile, dataSource);
        }

        if (recordReader == null) {
            return false;
        }

        int loaded = 0;
        int failed = 0;
        int loadedInterval = 100;
        int failedInterval = 100;
        TextWriter? textWriter = Console.Error;
        try {
            for (JsonObject? record = recordReader.ReadRecord();
                 (record != null);
                 record = recordReader.ReadRecord())
            {
                string? recordID = record.ContainsKey("RECORD_ID")
                    ? record["RECORD_ID"]?.GetValue<string>() : null;
                if (recordID == null || recordID.Trim().Length == 0)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine(
                        "All records in the file must have a RECORD_ID: " + record);
                    return false;
                }

                string? recordSource = record.ContainsKey("DATA_SOURCE")
                    ? record["DATA_SOURCE"]?.GetValue<string>() : null;
                
                if (recordSource == null) {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine(
                        "If records in the file do not have a DATA_SOURCE then a "
                        + "data source must be specified.");
                    return false;
                }

                if (!dataSources.Contains(recordSource)) {
                    if (!AddDataSource(repository, recordSource, verbose)) return false;
                    dataSources.Add(recordSource);
                }

                string jsonRecord = record.ToJsonString();
                
                long returnCode = ENGINE_API.AddRecord(dataSource, recordID, jsonRecord);

                if (returnCode == 0) {
                    loaded++;
                    loadedInterval = DoLoadFeedback(
                        "Loaded so far", loaded, loadedInterval, loaded, failed, silent);

                } else {
                    failed++;
                    if (failed == 1 || ((failed % failedInterval) == 0))
                    {
                        LogError("NativeEngine.AddRecord()", ENGINE_API);
                    }
                    failedInterval = DoLoadFeedback(
                        "Loaded so far", failed, failedInterval, loaded, failed, silent);
                }
            }
            DoLoadFeedback(
                "Loaded all records", loaded, 0, loaded, failed, silent);
            ProcessRedos(silent);
            textWriter = (silent) ? null : Console.Out;

            return true;
        } finally {
            if (loaded > 0 || failed > 0) {
                if (textWriter != null) {
                    textWriter.WriteLine();
                    textWriter.WriteLine("Loaded records from file:");
                    textWriter.WriteLine("     Repository  : " + repository);
                    textWriter.WriteLine("     File        : " + sourceFile);
                    if (dataSource != null) {
                        textWriter.WriteLine("     Data Source : " + dataSource);
                    }
                    textWriter.WriteLine("     Load Count  : " + loaded);
                    textWriter.WriteLine("     Fail Count  : " + failed);
                    textWriter.WriteLine();
                }
            }
            // set the counts
            if (failedCount != null) failedCount.SetValue(failed);
            if (loadedCount != null) loadedCount.SetValue(loaded);
        }
    }

    private static int ProcessRedos(bool silent) {
        int loaded = 0;
        int failed = 0;
        try
        {
            // process redos
            int loadedInterval = 100;
            int failedInterval = 100;
            long originalCount = ENGINE_API.CountRedoRecords();
            if (originalCount == 0) return 0;
            if (originalCount > 0) {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Found redos to process: " + originalCount);
                Console.Error.WriteLine();
            }
            for (int count = 0; ENGINE_API.CountRedoRecords() > 0; count++)
            {
                long returnCode = ENGINE_API.GetRedoRecord(out string recordJson);
                if (returnCode != 0) {
                    LogError("NativeEngine.GetRedoRecord()", ENGINE_API);
                    failed++;
                    failedInterval = DoLoadFeedback(
                        "Redo's so far", failed, failedInterval, loaded, failed, silent);
                } else {
                    returnCode = ENGINE_API.ProcessRedoRecord(recordJson);
                    if (returnCode != 0) {
                        LogError("NativeEngine.ProcessRedoRecord()", ENGINE_API);
                        failed++;
                        failedInterval = DoLoadFeedback(
                            "Redo's so far", failed, failedInterval, loaded, failed, silent);
                    } else {
                        loaded++;
                        loadedInterval = DoLoadFeedback(
                            "Redo's so far", loaded, loadedInterval, loaded, failed, silent);
                    }
                }
                if (count > (originalCount * 5)) {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("Processing redo's not converging -- giving up.");
                    Console.Error.WriteLine();
                    return count;
                }
            }
            Console.Error.WriteLine();
            Console.Error.WriteLine("Processed all redos (succeeded / failed): "
                                    + loaded + " / " + failed);
            Console.Error.WriteLine();

            return loaded;

        } catch (Exception ignore) {
            Console.Error.WriteLine();
            Console.Error.WriteLine("IGNORING EXCEPTION DURING REDOS:");
            Console.Error.WriteLine(ignore);
            Console.Error.WriteLine(ignore.StackTrace);
            Console.Error.WriteLine();
            return loaded;
        }

    }

    private static int DoLoadFeedback(string    prefix,
                                      int       count,
                                      int       interval,
                                      int       loaded,
                                      int       failed,
                                      bool      silent)
    {
        if (count > (interval * 10)) {
            interval *= 10;
        }
        if ((count > 0) && ((interval == 0) || (count % interval) == 0))
        {
            if (!silent) {
                Console.Error.WriteLine(prefix + " (succeeded / failed): "
                                    + loaded + " / " + failed);
            }
        }
        return interval;
    }

    private static bool AddDataSource(DirectoryInfo repository,
                                      string        dataSource,
                                      bool          verbose)
    {
        // add the data source and reinitialize
        IList<string> dataSourceList = new List<string>(1);
        dataSourceList.Add(dataSource);
        Configuration config = ConfigSources(repository,
                                             dataSourceList,
                                             verbose);
        if (config == null) return false;
        DestroyApis();
        InitApis(repository, verbose);
        return true;
    }

    private static RecordReader? ProvideJsonRecords(FileInfo sourceFile,
                                                    string?  dataSource)
    {
        RecordReader? recordReader = null;
        // check if we have a real JSON array
        try {
            FileStream fs = new FileStream(sourceFile.FullName, FileMode.Open);
            StreamReader reader = new StreamReader(fs, UTF8);
            recordReader = new RecordReader(reader, dataSource);

            RecordFormat format = recordReader.GetFormat();
            if (format != RecordFormat.Json && format != RecordFormat.JsonLines)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine(
                    "JSON file does not contain JSON or JSON-lines formatted records");
                Console.Error.WriteLine();
                return null;
            }

        } catch (Exception e) {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine(e.StackTrace);
            Console.Error.WriteLine();
            Console.Error.WriteLine("Failed to read file: " + sourceFile);
            Console.Error.WriteLine();
            return null;
        }

        // return the record reader
        return recordReader;
    }

    private static RecordReader? ProvideCsvRecords(FileInfo sourceFile,
                                                   string?  dataSource)
    {
        RecordReader? recordReader = null;
        // check if we have a real JSON array
        try {
            FileStream fs = new FileStream(sourceFile.FullName, FileMode.Open);
            StreamReader reader = new StreamReader(fs, UTF8);
            
            recordReader = new RecordReader(RecordFormat.CSV, reader, dataSource);

        } catch (Exception e) {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine(e.StackTrace);
            Console.Error.WriteLine();
            Console.Error.WriteLine("Failed to read file: " + sourceFile);
            Console.Error.WriteLine();
            return null;
        }

        // return the record reader
        return recordReader;
    }

    /// <summary>
    /// Loads a single JSON record to the repository -- optionally setting
    /// the data source for the record.
    /// </summary>
    ///
    /// <remarks>
    /// <b>NOTE:</b> if the specified record does not have a DATA_SOURCE property
    /// then the specified data source is required.
    /// </remarks>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="jsonRecord">The JSON record to load.</param>
    /// <param name="dataSource">The data source to use for loading the record.</param>
    ///
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool AddRecord(DirectoryInfo  repository,
                                 string         jsonRecord,
                                 string?        dataSource)
    {
        return AddRecord(repository, false, jsonRecord, dataSource, false);
    }

    /// <summary>
    /// Loads a single JSON record to the repository -- optionally setting
    /// the data source for the record.
    /// </summary>
    ///
    /// <remarks>
    /// <b>NOTE:</b> if the specified record does not have a DATA_SOURCE property
    /// then the specified data source is required.
    /// </remarks>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="jsonRecord">The JSON record to load.</param>
    /// <param name="dataSource">The data source to use for loading the record.</param>
    ///
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool AddRecord(DirectoryInfo  repository,
                                 bool           verbose,
                                 string         jsonRecord,
                                 string?        dataSource)
    {
        return AddRecord(repository, verbose, jsonRecord, dataSource, false);
    }

    /// <summary>
    /// Loads a single JSON record to the repository -- optionally setting
    /// the data source for the record.
    /// </summary>
    ///
    /// <remarks>
    /// <b>NOTE:</b> if the specified record does not have a DATA_SOURCE property
    /// then the specified data source is required.
    /// </remarks>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="jsonRecord">The JSON record to load.</param>
    /// <param name="dataSource">The data source to use for loading the record.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool AddRecord(DirectoryInfo  repository,
                                 string         jsonRecord,
                                 string?        dataSource,
                                 bool           silent)
    {
        return AddRecord(repository, false, jsonRecord, dataSource, silent);
    }


    /// <summary>
    /// Loads a single JSON record to the repository -- optionally setting
    /// the data source for the record.
    /// </summary>
    ///
    /// <remarks>
    /// <b>NOTE:</b> if the specified record does not have a DATA_SOURCE property
    /// then the specified data source is required.
    /// </remarks>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="jsonRecord">The JSON record to load.</param>
    /// <param name="dataSource">The data source to use for loading the record.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static bool AddRecord(DirectoryInfo  repository,
                                 bool           verbose,
                                 string         jsonRecord,
                                 string?        dataSource,
                                 bool           silent)
    {
        InitApis(repository, verbose);

        ISet<string> dataSources = GetDataSources();
        JsonNode? node = JsonNode.Parse(jsonRecord);
        if (node == null) {
            throw new ArgumentException("Failed to parse JSON record: " + jsonRecord);
        }
        JsonObject jsonObject = node.AsObject();
        
        if (dataSource == null) {
            dataSource = jsonObject.ContainsKey("DATA_SOURCE")
                ? jsonObject["DATA_SOURCE"]?.GetValue<string>() : null;
        }

        if (dataSource == null) {
            Console.Error.WriteLine();
            Console.Error.WriteLine("ERROR: Could not determine data source for record.");
            Console.Error.WriteLine();
            return false;
        }

        // check if the data source is configured
        dataSource = dataSource.ToUpper();
        if (dataSource != null && !dataSources.Contains(dataSource)) {
            if (!AddDataSource(repository, dataSource, verbose)) return false;
            dataSources.Add(dataSource);
        }

        string? recordID = jsonObject.ContainsKey("RECORD_ID")
            ? jsonObject["RECORD_ID"]?.GetValue<string>() : null;
        if (recordID == null || recordID.Trim().Length == 0)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine(
                "All records in the file must have a RECORD_ID: " + jsonObject);
            return false;
        }

        long returnCode = ENGINE_API.AddRecord(dataSource, recordID, jsonRecord);
        if (returnCode != 0) {
            LogError("NativeEngine.addRecord()", ENGINE_API);
            return false;
        }

        ProcessRedos(silent);

        if (!silent) {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Added record to " + dataSource + " data source: ");
            Console.Error.WriteLine(jsonRecord);
            Console.Error.WriteLine();
        }

        return true;
    }

    /// <summary>
    /// Updates the configuration to be the configuration in the specified
    /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="configJson">TThe JSON config</param>
    /// <param name="comment">The comment for the configuration.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration? UpdateConfig(DirectoryInfo repository,
                                              JsonObject    configJson,
                                              string        comment)
    {
        return UpdateConfig(repository, configJson, comment, false);
    }

    /// <summary>
    /// Updates the configuration to be the configuration in the specified
    /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="configJson">TThe JSON config</param>
    /// <param name="comment">The comment for the configuration.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration? UpdateConfig(DirectoryInfo repository,
                                              bool          verbose,
                                              JsonObject    configJson,
                                              string        comment)
    {
        return UpdateConfig(repository, verbose, configJson, comment, false);
    }

    /// <summary>
    /// Updates the configuration to be the configuration in the specified
    /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="configJson">TThe JSON config</param>
    /// <param name="comment">The comment for the configuration.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration? UpdateConfig(DirectoryInfo repository,
                                              JsonObject    configJson,
                                              string        comment,
                                              bool          silent)
    {
        return UpdateConfig(repository, false, configJson, comment, silent);
    }

    /// <summary>
    /// Updates the configuration to be the configuration in the specified
    /// <see cref="System.Text.Json.Nodes.JsonObject"/>.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="configJson">TThe JSON config</param>
    /// <param name="comment">The comment for the configuration.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration? UpdateConfig(DirectoryInfo repository,
                                              bool          verbose,
                                              JsonObject    configJson,
                                              string        comment,
                                              bool          silent)
    {
        InitApis(repository, verbose);

        long returnCode = 0;
        string configJsonText = configJson.ToJsonString();
        returnCode = CONFIG_MGR_API.AddConfig(configJsonText, comment, out long resultConfigId);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.AddConfig()", CONFIG_MGR_API);
            return null;
        }
        returnCode = CONFIG_MGR_API.SetDefaultConfigID(resultConfigId);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.SetDefaultConfigID()", CONFIG_MGR_API);
            return null;
        }

        returnCode = CONFIG_MGR_API.GetConfig(resultConfigId, out string resultConfigJson);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.GetConfig()", CONFIG_MGR_API);
            return null;
        }

        // parse the configuration
        JsonNode? node = JsonNode.Parse(resultConfigJson);
        if (node == null) {
            throw new JsonException("Failed to parse config JSON: " + resultConfigJson);
        }
        JsonObject resultConfig = ((JsonNode) node).AsObject();
        
        if (!silent) {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Added config and set as default: " + resultConfigId);
            Console.Error.WriteLine();
        }

        DestroyApis();
        InitApis(repository, verbose);

        return new Configuration(resultConfigId, resultConfig);
    }


    /// <summary>
    /// Configures the specified data sources for the specified repository
    /// if not already configured.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="dataSources">The set of data source codes to configure.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration ConfigSources(DirectoryInfo         repository,
                                              ICollection<string>   dataSources)
    {
        return ConfigSources(repository, dataSources, false);
    }

    /// <summary>
    /// Configures the specified data sources for the specified repository
    /// if not already configured.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="dataSources">The set of data source codes to configure.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration ConfigSources(DirectoryInfo         repository,
                                              bool                  verbose,
                                              ICollection<string>   dataSources)
    {
        return ConfigSources(repository, verbose, dataSources, false);
    }

    /// <summary>
    /// Configures the specified data sources for the specified repository
    /// if not already configured.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="dataSources">The set of data source codes to configure.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration ConfigSources(DirectoryInfo         repository,
                                              ICollection<string>   dataSources,
                                              bool                  silent)
    {
        return ConfigSources(repository, false, dataSources, silent);
    }

    /// <summary>
    /// Configures the specified data sources for the specified repository
    /// if not already configured.
    /// </summary>
    ///
    /// <param name="repository">The directory for the repository.</param>
    /// <param name="verbose">Whether or not to initialize in verbose mode.</param>
    /// <param name="dataSources">The set of data source codes to configure.</param>
    /// <param name="silent">Whether or not prevent console output.</param>
    ///
    /// <returns>
    /// The <see cref="Configuration"/> describing the new configuration or
    /// <c>null</c> if the operation failed.
    /// </returns>
    public static Configuration ConfigSources(DirectoryInfo         repository,
                                              bool                  verbose,
                                              ICollection<string>   dataSources,
                                              bool                  silent)
    {
        InitApis(repository, verbose);
        long?       resultConfigId  = null;
        JsonObject? resultConfig    = null;
        IntPtr?     configHandle    = null;
        long returnCode = 0;
        try
        {
            ISet<String> existingSet = GetDataSources(out IntPtr handle);
            configHandle = handle;

            IDictionary<string, bool> dataSourceActions
                = new Dictionary<string,bool>();
            ISet<string> addedDataSources = new HashSet<string>();
            int addedCount = 0;
            foreach (string dataSourceCode in dataSources)
            {
                if (existingSet.Contains(dataSourceCode)) {
                    dataSourceActions.Add(dataSourceCode, false);
                    continue;
                }
                JsonObject jsonObj = new JsonObject();
                JsonValue jsonVal = JsonValue.Create(dataSourceCode);
                jsonObj.Add("DSRC_CODE", jsonVal);
                returnCode = CONFIG_API.AddDataSource(
                    handle, jsonObj.ToJsonString(), out string addResult);
                if (returnCode != 0) {
                    LogError("NativeConfig.AddDataSource()", CONFIG_API);
                    throw new Exception("NativeConfig.AddDataSource() failed");
                }
                dataSourceActions.Add(dataSourceCode, true);
                addedDataSources.Add(dataSourceCode);
                addedCount++;
            }

            if (addedCount > 0) {
                string comment = BuildAddedComment(
                    "Added data sources: ", addedDataSources);

                resultConfig = AddConfigAndSetDefault(
                    handle, comment, out long configId);
                resultConfigId = configId;
            }

            if (!silent) {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Ensured specified data sources are configured.");
                Console.Error.WriteLine("     Repository   : " + repository);
                Console.Error.WriteLine("     Data Sources : ");
                foreach (KeyValuePair<string,bool> entry in dataSourceActions) {
                    Console.Error.WriteLine(
                        "          - " + entry.Key
                        + " (" + ((entry.Value) ? "added" : "preconfigured") + ")");
                }
                Console.Error.WriteLine();
            }

            if (addedCount > 0) {
                DestroyApis();
                InitApis(repository, verbose);
            }

        } finally {
            if (configHandle != null)
            {
                CONFIG_API.Close((IntPtr) configHandle);
            }
        }

        // check if the result config ID is not set (usually means that all the
        // data sources to be added already existed)
        if (resultConfigId == null || resultConfig == null) {
            return GetDefaultConfig();
        }

        return new Configuration((long) resultConfigId, (JsonObject) resultConfig);
    }

    /// <summary>
    /// Builds a comment for adding config objects.
    /// </summary>
    private static string BuildAddedComment(string prefix, ISet<string> addedSet)
    {
        string comment;
        if (addedSet.Count == 1) {
            IEnumerator<string> iter = addedSet.GetEnumerator();
            iter.MoveNext();
            comment = prefix + iter.Current;

        } else {
            StringBuilder commentSB = new StringBuilder();
            commentSB.Append(prefix);
            IEnumerator<string> iter = addedSet.GetEnumerator();
            string sep = "";
            int index = 0;
            int count = addedSet.Count;
            while (iter.MoveNext()) {
                String code = iter.Current;
                commentSB.Append(sep).Append(code);
                sep = (++index < (count - 1)) ? ", " : " and ";
            }
            comment = commentSB.ToString();
        }
        return comment;
    }

    /// <summary>
    /// Adds the config associated witht he specified handle using the specified
    /// comment and returns the <see cref="System.Text.Json.Nodes.JsonObject"/>
    /// for the config along with setting the config's ID in the specified result
    /// parameter.
    /// </summary>
    private static JsonObject AddConfigAndSetDefault(
        IntPtr configHandle, string comment, out long resultConfig)
    {
        // write the modified config to a string buffer
        string? configJsonText = null;
        long returnCode = CONFIG_API.Save(configHandle, out configJsonText);
        if (returnCode != 0)
        {
            LogError("NativeConfig.Save()", CONFIG_API);
            throw new Exception("NativeConfig.Save() failed");
        }

        returnCode = CONFIG_MGR_API.AddConfig(configJsonText, comment, out resultConfig);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.AddConfig()", CONFIG_MGR_API);
            throw new Exception("NativeConfigMgr.AddConfig() failed");
        }

        returnCode = CONFIG_MGR_API.SetDefaultConfigID(resultConfig);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.SetDefaultConfigID()", CONFIG_MGR_API);
            throw new Exception("NativeConfigMgr.SetDefaultConfigID() failed");
        }

        // get the result config and its ID for the result
        JsonNode? node = JsonNode.Parse(configJsonText);
        if (node == null) {
            throw new JsonException("Failed to parse JSON config: " + configJsonText);
        }
        return ((JsonNode) node).AsObject();
    }

    /// <summary>
    /// Gets the {@link JsonObject} describing the current default config as well
    /// as setting the default config's ID in the specified result parameter.
    /// </summary>
    private static Configuration GetDefaultConfig()
    {
        long returnCode = CONFIG_MGR_API.GetDefaultConfigID(out long configID);
        if (returnCode != 0)
        {
            LogError("NativeConfigMgr.GetDefaultConfigID()", CONFIG_MGR_API);
            throw new Exception("NativeConfigMgr.GetDefaultConfigID() failed");
        }
        string? configJson = null;
        returnCode = CONFIG_MGR_API.GetConfig(configID, out configJson);
        if (returnCode != 0) {
            LogError("NativeConfigMgr.GetConfig()", CONFIG_MGR_API);
            throw new Exception("NativeConfigMgr.GetConfig() failed");
        }
        JsonNode? node = JsonNode.Parse(configJson);
        if (node == null) {
            throw new JsonException("Failed to parse config JSON: " + configJson);
        }
        JsonObject config = ((JsonNode) node).AsObject();
        return new Configuration(configID, config);
    }
}
}