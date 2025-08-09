using System;

namespace Senzing.Sdk
{

    /// <summary>
    /// Defines the C# interface to the Senzing config management functions.
    /// </summary>
    /// 
    /// <example>
    /// An <c>SzConfigManager</c> instance is typically obtained from an 
    /// <see cref="SzEnvironment"/> instance via the <see cref="SzEnvironment.GetConfigManager"/>
    /// method as follows:
    /// <include file="../target/examples/SzConfigManagerDemo_GetConfigManager.xml" path="/*"/>
    /// </example>
    public interface SzConfigManager
    {
        /// <summary>
        /// Creates a new <see cref="SzConfig"/> instance from the template
        /// configuration definition.
        /// </summary>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_CreateConfigFromTemplate.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// A newly created <see cref="SzConfig"/> instance representing the
        /// template configuration definition.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/InitDefaultConfig/Program.cs">Code Snippet: Initialize Config</seealso> 
        SzConfig CreateConfig();

        /// <summary>
        /// Creates a new <see cref="SzConfig"/> instance from a
        /// configuration definition.
        /// </summary>
        ///
        /// <param name="configDefinition">
        /// The definition for the Senzing configuration.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigDemo_CreateConfigFromDefinition.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// A newly created <see cref="SzConfig"/> representing the specified
        /// configuration definition.
        /// </returns>
        ///
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        SzConfig CreateConfig(string configDefinition);

        /// <summary>
        /// Creates a new <see cref="SzConfig"/> instance for a configuration ID.
        /// </summary>
        /// 
        /// <remarks>
        /// If the configuration ID is not found the an exception is thrown.
        /// </remarks>
        /// 
        /// <param name="configID">
        /// The configuration ID of the configuration to retrieve.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_CreateConfigFromConfigID.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// A newly created <see cref="SzConfig"/> instance representing the 
        /// configuration definition that is registered with the specified 
        /// config ID.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        SzConfig CreateConfig(long configID);

        /// <summary>
        /// Registers a configuration definition in the repository.
        /// </summary>
        ///
        /// <remarks>
        /// <b>NOTE:</b> Registered configurations do not become
        /// immediately active nor do they become the default.
        /// Further, registered configurations cannot be unregistered.
        /// </remarks>
        /// 
        /// <param name="configDefinition">
        /// The configuration definition to register.
        /// </param>
        ///
        /// <param name="configComment">
        /// The comments for the configuration.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_RegisterConfigWithComment.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The identifier for referencing the config in the entity repository.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        long RegisterConfig(string configDefinition, string configComment);

        /// <summary>
        /// Registers a configuration definition in the repository with an
        /// auto-generated comment.
        /// </summary>
        ///
        /// <remarks>
        /// <b>NOTE:</b> Registered configurations do not become
        /// immediately active nor do they become the default.
        /// Further, registered configurations cannot be unregistered.
        /// </remarks>
        /// 
        /// <param name="configDefinition">
        /// The configuration definition to register.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_RegisterConfig.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The identifier for referencing the config in the entity repository.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        long RegisterConfig(string configDefinition);

        /// <summary>
        /// Gets the configuration registry.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The registry contains the original timestamp, original comment and
        /// configuration ID of all configurations ever registered with the repository.
        /// </para>
        /// <para>
        /// <b>NOTE:</b> Registered configurations cannot be unregistered.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_GetConfigRegistry.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzConfigManagerDemo_GetConfigRegistry.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The JSON object <c>string</c> describing the configurations registered
        /// in the repository with their identifiers, timestamps and comments.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        string GetConfigRegistry();

        /// <summary>
        /// Gets the default configuration ID for the repository.
        /// </summary>
        ///
        /// <remarks>
        /// Unless an explicit configuration ID is specified at
        /// initialization, the default configuration ID is used.
        /// 
        /// <para><b>NOTE:</b> The default configuration ID may not be the
        /// same as the active configuration ID.</para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_GetDefaultConfigID.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The current default configuration ID, or zero (0) if the default
        /// configuration has not been set.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        long GetDefaultConfigID();

        /// <summary>
        /// Replaces the existing default configuration ID with a new configuration ID.
        /// </summary>
        ///
        /// <remarks>
        /// The change is prevented (with an <see cref="SzReplaceConflictException"/>
        /// being thrown) if the current default configuration ID value is not as 
        /// expected.  Use this in place of <see cref="SetDefaultConfigID(long)"/>
        /// to handle race conditions.
        /// </remarks>
        ///
        /// <param name="currentDefaultConfigID">
        /// The configuration ID that is believed to be the current default
        /// configuration ID.
        /// </param>
        ///
        /// <param name="newDefaultConfigID">
        /// The new configuration ID for the repository.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_ReplaceDefaultConfigID.xml" path="/*"/>
        /// </example>
        ///
        /// <exception cref="SzReplaceConflictException">
        /// If the default configuration ID was not updated to the specified
        /// new value because the current default configuration ID found in
        /// the repository was not equal to the specified expected current
        /// default configuration ID value.
        /// </exception>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/RegisterDataSources/Program.cs">Code Snippet: Register Data Sources</seealso> 
        void ReplaceDefaultConfigID(long currentDefaultConfigID,
                                    long newDefaultConfigID);

        /// <summary>
        /// Sets the default configuration ID.
        /// </summary>
        ///
        /// <remarks>
        /// Usually this method is sufficient for setting the default configuration ID.
        /// However in concurrent environments that could encounter race conditions, 
        /// consider using <see cref="ReplaceDefaultConfigID(long,long)"/> instead.
        /// </remarks>
        /// 
        /// <param name="configID">
        /// The configuration ID to set as the default configuration.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_SetDefaultConfigID.xml" path="/*"/>
        /// </example>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/InitDefaultConfig/Program.cs">Code Snippet: Initialize Config</seealso> 
        void SetDefaultConfigID(long configID);

        /// <summary>
        /// Registers a configuration in the repository and then sets its ID as
        /// the default for the repository.
        /// </summary>
        /// 
        /// <remarks>
        /// This is a convenience method for <see cref="RegisterConfig(string,string)"/>
        /// followed by <see cref="SetDefaultConfigID(long)"/>.
        /// </remarks>
        ///
        /// <param name="configDefinition">
        /// The configuration definition to register as the default.
        /// </param>
        ///
        /// <param name="configComment">
        /// The comments for the configuration.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_SetDefaultConfigWithComment.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The identifier for referencing the config in the entity repository.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/InitDefaultConfig/Program.cs">Code Snippet: Initialize Config</seealso> 
        long SetDefaultConfig(string configDefinition, string configComment);

        /// <summary>
        /// Registers a configuration in the repository and then sets its ID as
        /// the default for the repository with an auto-generated comment.
        /// </summary>
        /// 
        /// <remarks>
        /// This is a convenience method for <see cref="RegisterConfig(string)"/>
        /// followed by <see cref="SetDefaultConfigID(long)"/>.
        /// </remarks>
        ///
        /// <param name="configDefinition">
        /// The configuration definition to register as the default.
        /// </param>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzConfigManagerDemo_SetDefaultConfig.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The identifier for referencing the config in the entity repository.
        /// </returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If a failure occurs.
        /// </exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/configuration/InitDefaultConfig/Program.cs">Code Snippet: Initialize Config</seealso> 
        long SetDefaultConfig(string configDefinition);
    }
}