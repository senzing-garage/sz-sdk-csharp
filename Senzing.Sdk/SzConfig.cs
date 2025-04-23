using System;

namespace Senzing.Sdk
{

    /// <summary>
    /// Defines the C# interface that encapsulates and represents a 
    /// Senzing configuration and provides functions to operate on that
    /// configuration.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// The Senzing config functions provide means to create, manipulate and
    /// export Senzing JSON configurations.
    /// </para>
    /// <para>
    /// An <c>SzConfig</c> instance is typically obtained from an 
    /// <see cref="SzConfigManager"/> instance via one of the following methods:
    /// <list type="bullet">
    ///     <item>
    ///         <description><see cref="SzConfigManager.CreateConfig()"/></description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SzConfigManager.CreateConfig(string)"/> </description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="SzConfigManager.CreateConfig(long)"/> </description>
    ///     </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface SzConfig
    {
        /// <summary>
        /// Obtains the configuration definition (typically formatted as JSON)
        /// for this configuration.
        /// </summary>
        ///
        /// <remarks>
        /// <b>NOTE:</b> Typically, an implementations <c>ToString()</c> function
        /// will be implemented to return the result from this function.
        /// </remarks>
        /// 
        /// <returns>The configuration definition (typically formatted as JSON).</returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string Export();

        /// <summary>
        /// Extracts the data sources from this configuration and returns the
        /// JSON text describing the data sources from the configuration.
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
        /// <returns>
        /// The JSON <c>string</c> describing the data sources found in
        /// the configuration.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string GetDataSources();

        /// <summary>
        /// Adds a new data source that is identified by the specified data source
        /// code to this configuration.  An exception is thrown if the data source
        /// already exists in the configuration.
        /// </summary>
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
        string AddDataSource(string dataSourceCode);

        /// <summary>
        /// Deletes the data source identified by the specified data source code
        /// from this configuration.
        /// </summary>
        ///
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source to delete from
        /// the configuration.
        /// </param>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        void DeleteDataSource(string dataSourceCode);

    }
}