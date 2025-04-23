using System;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Defines the C# interface to the Senzing config functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing config functions provide information regarding the
    /// Senzing config installation and user license.
    /// </remarks>
    internal interface NativeConfig : NativeApi
    {
        /// <summary>
        /// Initializes the Senzing config API with the specified module name,
        /// init parameters and flag indicating verbose logging.
        /// </summary>
        /// 
        /// <param name="moduleName">
        /// A short name given to this instance of the config API.
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
        /// Uninitializes the Senzing config API.
        /// </summary>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Destroy();

        /// <summary>
        /// Creates a new in-memory configuration using the default template and
        /// sets the specified parameter with the value of the configuration handle
        /// for working with it.
        /// </summary>
        ///
        /// <param name="configHandle">
        /// The out parameter on which to set the value of the configuration handle.
        /// </param>
        /// 
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        long Create(out IntPtr configHandle);

        /// <summary>
        /// Creates a new in-memory configuration using the specified JSON text and
        /// sets the specified parameter with the value of the configuration handle
        /// for working with it.
        /// </summary>
        ///
        /// <param name="jsonConfig">The JSON text for the config.</param>
        /// <param name="configHandle">
        /// The out parameter on which to set the value of the configuration handle.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Load(string jsonConfig, out IntPtr configHandle);

        /// <summary>
        /// Sets the JSON text for the configuration associated with the specified
        /// configuration handle to the specified response out parameter.
        /// </summary>
        ///
        /// <param name="configHandle">
        /// The configuration handle to export the JSON text from.
        /// </param>
        /// 
        /// <param name="response">
        /// The out parameter which will be set with the JSON configuration text.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Save(IntPtr configHandle, out string response);

        /// <summary>
        /// Closes the in-memory configuration associated with the specified config
        /// handle and cleans up system resources.
        /// </summary>
        /// 
        /// <remarks>
        /// After calling this method, the configuration handle can no longer be used
        /// and becomes invalid.
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The config handle identifying the in-memory configuration to close.
        /// </param>
        /// 
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        long Close(IntPtr configHandle);

        /// <summary>
        /// Extracts the data sources from the in-memory configuration associated
        /// with the specified config handle and sets the specified response out
        /// parameter to the JSON text describing the data sources from the
        /// configuration.
        /// </summary>
        /// 
        /// <remarks>
        /// The format of the JSON response is as follows:
        /// <code>
        /// {
        ///   "DATA_SOURCES": [
        ///     {
        ///        "DSRC_ID": 1,
        ///        "DSRC_CODE": "TEST"
        ///     },
        ///     {
        ///        "DSRC_ID": 2,
        ///        "DSRC_CODE": "SEARCH"
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The config handle identifying the in-memory configuration from which
        /// to list the data sources.
        /// </param>
        /// 
        /// <param name="response">
        /// The out parameter that will be set to the JSON <c>string</c> describing
        /// the data sources from the configuration.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ListDataSources(IntPtr configHandle, out string response);

        /// <summary>
        /// Adds a data source described by the specified JSON to the in-memory
        /// configuration associated with the specified config handle.
        /// </summary>
        /// 
        /// <remarks>
        /// The response out parameter is set to the JSON <c>string</c> describing the
        /// data source ID that was added.
        /// 
        /// <para>
        /// The input JSON has the following format:
        /// <code>
        ///  {
        ///    "DSRC_CODE": "CUSTOMERS"
        ///  }
        /// </code>
        /// </para>
        /// <para>
        /// Optionally, you can specify the data source ID:
        /// <code>
        ///  {
        ///    "DSRC_CODE": "CUSTOMERS",
        ///    "DSRC_ID": 410
        ///  }
        /// </code>
        /// </para>
        /// <para>
        /// The response JSON provides the data source ID of the created data source,
        /// which is especially useful if the data source ID was not specified in the
        /// input:
        /// <code>
        ///  {
        ///    "DSRC_ID": 410
        ///  }
        /// </code>
        /// </para>
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The config handle identifying the in-memory to which to add the data source.
        /// </param>
        /// 
        /// <param name="inputJson">
        /// The JSON text describing the data source to add to the configuration.
        /// </param>
        /// 
        /// <param name="response">
        /// The out parameter to set to the JSON <c>string</c> describing the data
        /// source that was added.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long AddDataSource(IntPtr configHandle,
                           string inputJson,
                           out string response);

        /// <summary>
        /// Deletes the data source described by the specified JSON from the
        /// in-memory configuration associated with the specified config handle.
        /// </summary>
        /// 
        /// <remarks>
        /// The input JSON has the following format:
        /// <code>
        ///  {
        ///    "DSRC_CODE": "CUSTOMERS"
        ///  }
        /// </code>
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The config handle identifying the in-memory configuration from which 
        /// to delete the data source.
        /// </param>
        /// 
        /// <param name="inputJson">
        /// The JSON text describing the data source to delete.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns> 
        long DeleteDataSource(IntPtr configHandle, string inputJson);

    }
}