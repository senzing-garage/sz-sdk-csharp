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
    ///
    /// <example>
    /// Create from template configuration:
    /// <include file="../target/examples/SzConfigDemo_CreateConfigFromTemplate.xml" path="/*"/>
    /// </example>
    /// 
    /// <example>
    /// Create from configuration definition:
    /// <include file="../target/examples/SzConfigDemo_CreateConfigFromDefinition.xml" path="/*"/>
    /// </example>
    /// 
    /// <example>
    /// Create from registered configuration ID:
    /// <include file="../target/examples/SzConfigManagerDemo_CreateConfigFromConfigID.xml" path="/*"/>
    /// </example>
    public interface SzConfig
    {
        /// <summary>
        /// Retrieves the definition for this configuration.
        /// </summary>
        ///
        /// <remarks>
        /// <b>NOTE:</b> Typically, an implementations <c>ToString()</c> function
        /// will be implemented to return the result from this function.
        /// </remarks>
        /// 
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_ExportConfig.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b><br/>
        /// The example result is rather large, but can be viewed
        /// <see target="_blank" href="../examples/results/SzConfigDemo_ExportConfig.txt">here</see>
        /// (formatted for readability). 
        /// </example>
        /// 
        /// <returns>The configuration definition formatted as a JSON object.</returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/InitDefaultConfig/Program.cs">Code Snippet: Initialize Config</seealso> 
        string Export();

        /// <summary>
        /// Gets the data source registry for this configuration.
        /// </summary>
        /// 
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_GetDataSourceRegistry.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzConfigDemo_GetDataSourceRegistry.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The data source registry describing the data sources for this
        /// configuration formatted as a JSON object.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string GetDataSourceRegistry();

        /// <summary>
        /// Adds a data source to this configuration.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Because <see cref="SzConfig"/> is an in-memory representation, the repository
        /// is not changed unless the configuration is <see cref="SzConfig.Export()"
        /// >exported</see> and then <see cref="SzConfigManager.RegisterConfig(string, string)"
        /// >registered</see> via <see cref="SzConfigManager"/>.
        /// </para>
        /// 
        /// <para>
        /// An exception is thrown if the data source already exists in the configuration.
        /// </para>
        /// </remarks>
        ///
        /// <param name="dataSourceCode">
        /// The data source code for the new data source.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_RegisterDataSource.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzConfigDemo_RegisterDataSource.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The JSON <c>string</c> describing the data source was registered.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        string RegisterDataSource(string dataSourceCode);

        /// <summary>
        /// Removes a data source from this configuration.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Because <see cref="SzConfig"/> is an in-memory representation, the repository
        /// is not changed unless the configuration is <see cref="SzConfig.Export()"
        /// >exported</see> and then <see cref="SzConfigManager.RegisterConfig(string, string)"
        /// >registered</see> via <see cref="SzConfigManager"/>.
        /// </para>
        /// 
        /// <para>
        /// <b>NOTE:</b> This method is idempotent in that it succeeds with no changes
        /// being made when specifying a data source code that is not found in the registry.
        /// </para>
        /// 
        /// <para>
        /// <b>WARNING:</b> If records in the repository refer to the unregistered data 
        /// source, the configuration cannot be used as the active configuration.
        /// </para>
        /// </remarks>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source to delete from
        /// the configuration.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_UnregisterDataSource.xml" path="/*"/>
        /// </example>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        void UnregisterDataSource(string dataSourceCode);

    }
}