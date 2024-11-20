using System;

namespace Senzing.Sdk
{

    /// <summary>
    /// Defines the Java interface to the Senzing config functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing config functions provide means to create, manipulate and
    /// export Senzing JSON configurations.
    /// </remarks>
    public interface SzConfig
    {
        /// <summary>
        /// Creates a new in-memory configuration using the default configuraiton
        /// template and returns the configuration handle for working with it.
        /// </summary>
        ///
        /// <returns>
        /// The configuraton handle for working with the configuration that was
        /// created.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        IntPtr CreateConfig();

        /// <summary>
        /// Creates a new in-memory configuration using the specified configuration
        /// definition and returns the configuration handle for working with it.
        /// </summary>
        ///
        /// <remarks>
        /// Depending upon implementation of this interface, the specified definition
        /// may allow other forms, but it is typically a JSON-formatted Senzing
        /// configuration (an example template JSON configuration ships with the 
        /// Senzing product).
        /// </remarks>
        ///
        /// <param name="configDefinition">
        /// The definition for the Senzing configuration.
        /// </param>
        /// 
        /// <returns>
        /// The configuraton handle for working with the configuration
        /// that was created and populated with the specified definition.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        IntPtr ImportConfig(string configDefinition);

        /// <summary>
        /// Obtains the configuration definition formatted as JSON for the in-memory
        /// configuration associated with the specified configuration handle.
        /// </summary>
        ///
        /// <param name="configHandle">
        /// The configuration handle associated with the in-memory configuration to
        /// be formatted as JSON.
        /// </param>
        ///
        /// <returns>The configuration defininition formatted as JSON.</returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string ExportConfig(IntPtr configHandle);

        /// <summary>
        /// Closes the in-memory configuration associated with the specified config
        /// handle and cleans up system resources.
        /// </summary>
        ///
        /// <remarks>
        /// After calling this method, the configuration handle can no longer be
        /// used and becomes invalid.
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The configuration handle associated with the in-memory configuration
        /// to be closed.
        /// </param>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        void CloseConfig(IntPtr configHandle);

        /// <summary>
        /// Extracts the data sources from the in-memory configuration associated
        /// with the specified config handle returns the JSON text describing the
        /// data sources from the configuration.
        /// </summary>
        /// 
        /// <remarks>
        /// The format of the JSON response is as follows:
        /// <code>
        /// {
        ///   "DATA_SOURCES": [
        ///     {
        ///       "DSRC_ID": 1,
        ///       "DSRC_CODE": "TEST"
        ///     },
        ///     {
        ///       "DSRC_ID": 2,
        ///       "DSRC_CODE": "SEARCH"
        ///     }
        ///   ]
        /// }
        /// </code>
        /// </remarks>
        ///
        /// <param name="configHandle">
        /// The config handle associated witht he in-memory configuration from
        /// which to obtain the data sources.
        /// </param>
        /// 
        /// <returns>
        /// The JSON <c>string</c> describing the data sources found in
        /// the configuration.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string GetDataSources(IntPtr configHandle);

        /// <summary>
        /// Adds a new data source that is identified by the specified data source
        /// code to the in-memory configuration associated with the specified 
        /// configuraiton handle.
        /// </summary>
        ///
        /// <param name="configHandle">
        /// The config handle associated witht he in-memory configuration to
        /// which to add the data source.
        /// </param>
        ///
        /// <param name="dataSourceCode">
        /// The data source code for the new data source.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the data source was 
        /// added to the configuration.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string AddDataSource(IntPtr configHandle, string dataSourceCode);

        /// <summary>
        /// Deletes the data source identified by the specified data source code
        /// from the in-memory configuration associated with the specified config
        /// handle.
        /// </summary>
        ///
        /// <param name="configHandle">
        /// The config handle associated witht he in-memory configuration from
        /// which to delete the data source.
        /// </param>
        ///
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source to delete from
        /// the configuration.
        /// </param>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        void DeleteDataSource(IntPtr configHandle, string dataSourceCode);

    }
}