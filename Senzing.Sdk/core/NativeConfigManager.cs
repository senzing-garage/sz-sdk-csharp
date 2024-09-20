namespace Senzing.Sdk.Core {
/// <summary>
/// Defines the C# interface to the Senzing config management functions.
/// </summary>
/// 
/// <remarks>
/// The Senzing config management functions provide a means of storing
/// and retrieving configurations in the repository as well as setting
/// the default configuration for the repository.
/// </remarks>
internal interface NativeConfigManager : NativeApi {
    /// <summary>
    /// Initializes the Senzing config management API with the specified
    /// module name, init parameters and flag indicating verbose logging.
    /// </summary>
    /// 
    /// <param name="moduleName">
    /// A short name given to this instance of the config management API.
    /// </param>
    /// 
    /// <param name="iniParams">
    /// A JSON string containing configuration parameters.
    /// </param>
    /// 
    /// <param name="verboseLogging">
    /// Enable diagnostic logging which will print a massive amount of
    /// information to stdout.
    /// </param>
    ///
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long Init(string moduleName, string iniParams, bool verboseLogging);

    /// <summary>
    /// Uninitializes the Senzing config management API.
    /// </summary>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long Destroy();

    /// <summary>
    /// Adds the configuration described by the specified JSON to the repository
    /// with the specified comments and returns the ID of the config in the
    /// specified out parameter.
    /// </summary>
    /// 
    /// <param name="configStr">
    /// The JSON text describing the configuration.
    /// /param>
    /// 
    /// <param name="configComments">
    /// The comments for the configuration.
    /// </param>
    /// 
    /// <param name="configID">
    /// The configuration ID for the registered config.
    /// </param>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    /// 
    long AddConfig(string configStr, string configComments, out long configID);

    /// <summary>
    /// Gets the configuration with the specified config ID and sets the 
    /// specified out parameter to the JSON text of configuration.
    /// </summary>
    /// 
    /// <param name="configID">
    /// The configuration ID of the configuration to retrieve.
    /// </param>
    /// 
    /// <param name="response">
    /// The out parameter to set to the JSON text of the configuration.
    /// </param>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long GetConfig(long configID, out string response);

    /// <summary>
    /// Gets the list of saved configuration IDs with their comments and
    /// timestamps and sets the specified out parameter to the the JSON text
    /// describing those configurations.
    /// </summary>
    /// 
    /// <remarks>
    /// The format of the response is:
    /// <code>
    /// {
    ///   "CONFIGS": [
    ///     {
    ///        "CONFIG_ID": 12345678912345,
    ///        "SYS_CREATE_DT": "2021-03-25 18:35:00.743",
    ///        "CONFIG_COMMENTS": "Added EMPLOYEES data source."
    ///     },
    ///     {
    ///        "CONFIG_ID": 23456789123456,
    ///        "SYS_CREATE_DT": "2021-02-08 23:27:09.876",
    ///        "CONFIG_COMMENTS": "Added CUSTOMERS data source."
    ///     },
    ///     {
    ///        "CONFIG_ID": 34567891234567,
    ///        "SYS_CREATE_DT": "2021-02-08 23:27:05.212",
    ///        "CONFIG_COMMENTS": "Initial Config"
    ///     },
    ///     . . .
    ///   ]
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <param name="response">
    /// The out parameter to set to the JSON text of the configuration list.
    /// </param>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long GetConfigList(out string response);

    /// <summary>
    /// Sets the default configuration for the repository to the specified
    /// configuration ID.
    /// </summary>
    /// 
    /// <param name="configID">
    /// The configuration ID to set as the default configuration.
    /// </param>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long SetDefaultConfigID(long configID);

    /// <summary>
    /// Gets the configuration ID of the default configuration for the repository
    /// and sets the specified out parameter to the value of the configuration ID.
    /// </summary>
    /// 
    /// <param name="configID">
    /// The out parameter to be set to the default default configuration ID.
    /// </param>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long GetDefaultConfigID(out long configID);

    /// <summary>
    /// Replaces the current configuration ID of the repository with the specified
    /// new configuration ID providing the current configuration ID of the
    /// repository is equal to the specified old configuration ID.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current configuration ID is not the same as the specified old
    /// configuration ID then this method fails to replace the default
    /// configuration ID with the new value.
    /// </remarks>
    /// 
    /// <param name="oldConfigID">
    /// The configuration ID that is believed to be the current default
    /// configuration ID.
    /// </param>
    /// 
    /// <param name="newConfigID">
    /// The new configuration ID for the repository.
    /// </param>
    /// 
    /// <returns>
    /// Zero (0) on success and non-zero on failure.
    /// </returns>
    /// 
    long ReplaceDefaultConfigID(long oldConfigID, long newConfigID);
}
}