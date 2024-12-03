namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Defines the C# interface to the Senzing product functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing product functions provide information regarding the
    /// Senzing product installation and user license.
    /// </remarks>
    internal interface NativeProduct : NativeApi
    {
        /// <summary>
        /// Initializes the Senzing product API with the specified module name,
        /// init parameters and flag indicating verbose logging.
        /// </summary>
        /// 
        /// <param name="moduleName">
        /// A short name given to this instance of the product API.
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
        /// Uninitializes the Senzing product API.
        /// </summary>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Destroy();

        /// <summary>
        /// Returns the currently configured license details.
        /// </summary>
        ///
        /// <returns>The JSON document describing the license details.</returns>
        string License();

        /// <summary>
        /// Returns the currently installed version details.
        /// </summary>
        ///
        /// <returns>A JSON document describing version details.</returns>
        string Version();
    }
}