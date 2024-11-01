using System;

namespace Senzing.Sdk {

/// <summary>
/// Defines the Java interface to the Senzing config manager functions.
/// </summary>
/// 
/// <remarks>
/// The Senzing config functions provide means to manage the configurations
/// that are stored in the Senzing repository including the default configuration
/// that will be loaded if no config ID is specified during initialization.
/// </remarks>
public interface SzConfigManager {
    /// <summary>
    /// Adds the configuration described by the specified JSON to the repository
    /// with the specified comment and returns the identifier for referencing the
    /// the config in the entity repository.
    /// </summary>
    ///
    /// <param name="configDefinition">
    /// The JSON text describing the configuration.
    /// </param>
    ///
    /// <param name="configComment">
    /// The comments for the configuration.
    /// </param>
    ///
    /// <returns>
    /// The identifier for referncing the config in the entity repository.
    /// </returns>
    /// 
    /// <exception cref="Senzing.Sdk.SzException">
    /// If a failure occurs.
    /// </exception>
    long AddConfig(string configDefinition, string configComment);

    /// <summary>
    /// Gets the configuration with the specified config ID and returns the 
    /// configuration defintion as a <c>string</c>.
    /// </summary>
    ///
    /// <param name="configID">
    /// The configuration ID of the configuration to retrieve.
    /// </param>
    ///
    /// <returns>The configuration definition as a <c>string</c></returns>
    /// 
    /// <exception cref="Senzing.Sdk.SzException">
    /// If a failure occurs.
    /// </exception>
    string GetConfig(long configID);

    /// <summary>
    /// Gets the list of saved configuration ID's with their comments and
    /// timestamps and return the JSON <c>string</c> describing them.
    /// </summary>
    /// 
    /// <remarks>
    /// An example format for the response is:
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
    /// <returns>
    /// The JSON <c>string</c> describing the configurations registered
    /// in the entity repository with their identifiers, timestamps and 
    /// comments.
    /// </returns>
    /// 
    /// <exception cref="Senzing.Sdk.SzException">
    /// If a failure occurs.
    /// </exception>
    string GetConfigs();

    /// <summary>
    /// Gets the configuration ID of the default configuration for the repository
    /// and returns it.
    /// </summary>
    ///
    /// <remarks>
    /// If the entity repository is in the initial state and the default
    /// configuration ID has not yet been set, then zero (0) is returned.
    /// </remarks>
    ///
    /// <returns>
    /// The current default cofiguration ID in the repository, or zero (0)
    /// if the entity repository is in the initial state with no default
    /// configuration ID having yet been set.
    /// </returns>
    /// 
    /// <exception cref="Senzing.Sdk.SzException">
    /// If a failure occurs.
    /// </exception>
    long GetDefaultConfigID();

    /// <summary>
    /// Replaces the current configuration ID of the repository with the
    /// specified new configuration ID providing the current configuration
    //  ID of the repository is equal to the specified old configuration ID.
    /// </summary>
    ///
    /// <remarks>
    /// If the current configuration ID is not the same as the specified old
    /// configuration ID then this method fails to replace the default
    /// configuration ID with the new value and an <see 
    /// cref="SzReplaceConflictException"/>.
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
    void ReplaceDefaultConfigID(long currentDefaultConfigID,
                                long newDefaultConfigID);

    /// <summary>
    /// Sets the default configuration for the repository to the specified
    /// configuration ID.
    /// </summary>
    ///
    /// <param name="configID">
    /// The configuration ID to set as the default configuration.
    /// </param>
    /// 
    /// <exception cref="Senzing.Sdk.SzException">
    /// If a failure occurs.
    /// </exception>
    void SetDefaultConfigID(long configID);

}
}