namespace Senzing.Sdk.Core {
/// <summary>
/// Defines the C# interface to the Senzing engine functions.
/// </summary>
/// 
/// <remarks>
/// The Senzing engine functions primarily provide means of working
/// with identity data records, entities and their relationships.
/// </remarks>
internal interface NativeEngine : NativeApi {
    /// <summary>
    /// Initializes the Senzing engine API with the specified module name,
    /// init parameters and flag indicating verbose logging.
    /// </summary>
    /// 
    /// <param name="moduleName">
    /// A short name given to this instance of the engine API.
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
    /// Initializes the Senzing engine API with the specified module
    /// name, initialization parameters, verbose logging flag and a
    /// specific configuration ID identifying the configuration to use.
    /// </summary>
    ///
    /// <param name="moduleName">
    /// The module name with which to initialize.
    /// </param>
    ///
    /// <param name="iniParams">
    /// The JSON initialization parameters.
    /// </param>
    ///
    /// <param name="initConfigID">
    /// The specific configuration ID with which to initialize.
    /// </param>
    ///
    /// <param name="verboseLogging">
    /// Whether or not to initialize with verbose logging.
    /// </param>
    ///
    /// <returns>
    /// Zero (0) on success and non-zero on failure.
    /// </returns>
    long InitWithConfigID(string    moduleName,
                          string    iniParams,
                          long      initConfigID,
                          bool      verboseLogging);

    /// <summary>
    /// Reinitializes with the specified configuration ID.
    /// </summary>
    /// 
    /// <param name="initConfigID">
    /// The configuration ID with which to reinitialize.
    /// </param>
    ///
    /// <returns>
    /// Zero (0) on success and non-zero on failure.
    /// </returns>
    long Reinit(long initConfigID);

    /// <summary>
    /// Uninitializes the Senzing engine API.
    /// </summary>
    /// 
    /// <returns>Zero (0) on success and non-zero on failure.</returns>
    long Destroy();

}
}