namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Defines the C# interface to the Senzing diagnostic functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing diagnostic functions provide diagnostics and statistics
    /// pertaining to the host system and the Senzing repository.
    /// </remarks>
    internal interface NativeDiagnostic : NativeApi
    {
        /// <summary>
        /// Initializes the Senzing diagnostic API with the specified module name,
        /// init parameters and flag indicating verbose logging.
        /// </summary>
        /// 
        /// <param name="moduleName">
        /// A short name given to this instance of the diagnostic API.
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
        /// Initializes the Senzing diagnostic API with the specified module
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
        long InitWithConfigID(string moduleName,
                              string iniParams,
                              long initConfigID,
                              bool verboseLogging);

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
        /// Uninitializes the Senzing diagnostic API.
        /// </summary>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Destroy();

        /// <summary>
        /// Gathers database information and sets the specified out parameter
        /// to the JSON <c>string</c> describing the details.
        /// </summary>
        ///
        /// <param name="response">
        /// The out string response parameter to be set to the JSON <c>string</c>
        /// that details the database information.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetDatastoreInfo(out string response);

        /// <summary>
        /// Runs non-destructive DB performance tests and sets the specified
        /// out parameter to the JSON <c>string</c> describing the details
        /// of the test results.
        /// </summary>
        ///
        /// <param name="secondsToRun">
        /// How long to run the database performance test.
        /// </param>
        /// 
        /// <param name="response">
        /// The out string response parameter to be set to the JSON <c>string</c>
        /// that details the results of the performance test.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long CheckDatastorePerformance(int secondsToRun, out string response);

        /// <summary>
        /// Purges all data in the configured repository.
        /// </summary>
        /// 
        /// <remarks>
        /// WARNING: There is no undoing from this.  Make sure your repository is
        /// regularly backed up.
        /// </remarks>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long PurgeRepository();

        /// <summary>
        /// Experimental/internal method for obtaining diagnostic feature information.
        /// </summary>
        ///
        /// <param name="libFeatID">
        /// The <c>LIB_FEAT_ID</c> identifying the feature.
        /// </param>
        /// 
        /// <param name="response">
        /// The out string response parameter that will be set to the JSON <c>string</c>
        /// result document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetFeature(long libFeatID, out string response);

    }
}