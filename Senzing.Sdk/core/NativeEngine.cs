using System;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Defines the C# interface to the Senzing engine functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing engine functions primarily provide means of working
    /// with identity data records, entities and their relationships.
    /// </remarks>
    internal interface NativeEngine : NativeApi
    {
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
        /// Uninitializes the Senzing engine API.
        /// </summary>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long Destroy();

        /// <summary>
        /// May optionally be called to pre-initialize some of the heavier weight
        /// internal resources of the Senzing engine.
        /// </summary>
        /// 
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        long PrimeEngine();

        /// <summary>
        /// Returns the current internal engine workload statistics for the process.
        /// </summary>
        /// 
        /// <remarks>
        /// The counters are reset after each call.
        /// </remarks>
        ///
        /// <returns>JSON document of workload statistics</returns>
        string Stats();

        /// <summary>
        /// Returns an identifier for the loaded Senzing engine configuration.
        /// </summary>
        /// 
        /// <param name="configID">
        /// The out parameter that will be set to the value of the config ID.
        /// </param>
        /// 
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        long GetActiveConfigID(out long configID);

        /// <summary>
        /// Loads the JSON record with the specified data source code and record ID.
        /// </summary>
        /// 
        /// <remarks>
        /// The specified JSON data may contain the <c>DATA_SOURCE</c> and
        /// <c>RECORD_ID</c> elements, but, if so, they must match the specified
        /// parameters.
        /// </remarks>
        ///
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// 
        /// <param name="recordID">The ID for the record</param>
        /// 
        /// <param name="jsonData">
        /// A JSON document containing the attribute information for the record.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long AddRecord(string dataSourceCode,
                       string recordID,
                       string jsonData);

        /// <summary>
        /// Loads the JSON record with the specified data source code and record ID
        /// using the specified flags responding with a JSON document describing
        /// how the repository was changed.
        /// </summary>
        ///
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// 
        /// <param name="recordID">The ID for the record.</param>
        /// 
        /// <param name="jsonData">
        /// A JSON document containing the attribute information for the record.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically the
        /// content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the JSON
        /// document describing how the repository was modified.
        /// </param>
        /// 
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        long AddRecordWithInfo(string dataSourceCode,
                               string recordID,
                               string jsonData,
                               long flags,
                               out string response);

        /// <summary>
        /// Peforms a hypothetical load of the specified JSON record without
        /// actually loading the record responding with a JSON document describing
        /// how the record would be loaded and how the repository would be changed.
        /// </summary>
        /// 
        /// <param name="jsonData">
        /// A JSON document containing the attribute information for the record.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        ///
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the JSON
        /// response document describing how the record would be loaded.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long PreprocessRecord(string jsonData,
                              long flags,
                              out string response);


        /// <summary>
        /// Delete the record that has already been loaded.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// <param name="recordID">The ID for the record.</param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long DeleteRecord(string dataSourceCode, string recordID);

        /// <summary>
        /// Delete the record that has already been loaded, responding with
        /// a JSON document describing how the repository was changed.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// 
        /// <param name="recordID">The ID for the record</param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically the
        /// content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the JSON
        /// document describing how the repository was modified.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long DeleteRecordWithInfo(string dataSourceCode,
                                  string recordID,
                                  long flags,
                                  out string response);

        /// <summary>
        /// Reevaluate a record that has already been loaded.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// <param name="recordID">The ID for the record.</param>
        /// <param name="flags">
        /// The flags to control how the operation is performed.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        /// </returns>
        long ReevaluateRecord(string dataSourceCode, string recordID, long flags);

        /// <summary>
        /// Reevaluate a record that has already been loaded, responding with
        /// a JSON document describing how the repository was changed.
        /// </summary>
        ///
        /// <param name="dataSourceCode">The data source for the record.</param>
        /// <param name="recordID">The ID for the record.</param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and
        /// specifically the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out string response parameter that will be set to the JSON
        /// response document.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ReevaluateRecordWithInfo(string dataSourceCode,
                                      string recordID,
                                      long flags,
                                      out string response);

        /// <summary>
        /// Reevaluate a resolved entity identified by the specified entity ID.
        /// </summary>
        /// 
        /// <param name="entityID">
        /// The ID of the resolved entity to reevaluate
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ReevaluateEntity(long entityID, long flags);

        /// <summary>
        /// Reevaluate a resolved entity, responding with a JSON document
        /// describing how the repository was changed.
        /// </summary>
        /// 
        /// <param name="entityID">
        /// The ID of the resolved entity to reevaluate.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the JSON response document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ReevaluateEntityWithInfo(long entityID,
                                      long flags,
                                      out string response);

        /// <summary>
        /// Searches for entities that contain attribute information that are
        /// relevant to a set of input search attributes.
        /// </summary>
        /// 
        /// <param name="jsonData">
        /// A JSON document containing the attribute information definining
        /// the criteria for the search.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the search results.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long SearchByAttributes(string jsonData, out string response);

        /// <summary>
        /// Searches for entities that contain attribute information that
        /// are relevant to a set of input search attributes.
        /// </summary>
        /// 
        /// <param name="jsonData">
        /// A JSON document containing the attribute information definining
        /// the criteria for the search.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the JSON
        /// response document describing the search results.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long SearchByAttributes(string jsonData, long flags, out string response);

        /// <summary>
        /// Searches for entities that contain attribute information that are
        /// relevant to a set of input search attributes using a specific 
        /// search profile.
        /// </summary>
        ///
        /// <param name="jsonData">
        /// A JSON document containing the attribute information definining
        /// the criteria for the search.
        /// </param>
        /// 
        /// <param name="searchProfile">
        /// The search-profile identifier identifying the search profile to use
        /// for the search operation.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the JSON
        /// response document describing the search results.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long SearchByAttributes(string jsonData,
                                string searchProfile,
                                long flags,
                                out string response);

        /// <summary>
        /// Retrieves information about a specific resolved entity identified
        /// by its entity ID.
        /// </summary>
        /// 
        /// <param name="entityID">
        /// The entity ID of the resolved entity to retrieve.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the entity.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetEntityByEntityID(long entityID, out string response);

        /// <summary>
        /// Retrieves information about a specific resolved entity identified
        /// by its entity ID.
        /// </summary>
        /// 
        /// <param name="entityID">
        /// The entity ID of the resolved entity to retrieve.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the entity.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetEntityByEntityID(long entityID,
                                 long flags,
                                 out string response);


        /// <summary>
        /// Retrieves information about a specific resolved entity identified by
        /// the data source code and record ID of one of its constituent records.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code of the constituent record.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID of the constituent record.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the entity.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetEntityByRecordID(string dataSourceCode,
                                 string recordID,
                                 out string response);

        /// <summary>
        /// Retrieves information about a specific resolved entity identified by
        /// the data source code and record ID of one of its constituent records.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code of the constituent record.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID of the constituent record.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the entity.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetEntityByRecordID(string dataSourceCode,
                                 string recordID,
                                 long flags,
                                 out string response);

        /// <summary>
        /// Finds interesting entities close to a specific resolved entity
        /// identified by its entity ID.
        /// </summary>
        /// 
        /// <param name="entityID">
        /// The resolved entity to search around.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the interesting entities.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindInterestingEntitiesByEntityID(long entityID,
                                               long flags,
                                               out string response);

        /// <summary>
        /// Finds interesting entities close to a specific resolved entity
        /// identified by the data source code and record ID of one of its
        /// constituent records.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code of the constituent record.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID of the constituent record.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The <c>string</c> response out parameter that will be set to the
        /// JSON response document describing the interesting entities.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindInterestingEntitiesByRecordID(string dataSourceCode,
                                               string recordID,
                                               long flags,
                                               out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by entity ID.
        /// </summary>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityID(long entityID1,
                                long entityID2,
                                int maxDegrees,
                                out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by entity ID.
        /// </summary>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityID(long entityID1,
                                long entityID2,
                                int maxDegrees,
                                long flags,
                                out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by the data soure and record ID pairs of their constituent records.
        /// </summary>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordID(string dataSourceCode1,
                                string recordID1,
                                string dataSourceCode2,
                                string recordID2,
                                int maxDegrees,
                                out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by the data soure and record ID pairs of their constituent records.
        /// </summary>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordID(string dataSourceCode1,
                               string recordID1,
                               string dataSourceCode2,
                               string recordID2,
                               int maxDegrees,
                               long flags,
                               out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by entity ID with the avoidance (preferred or strict) of specific
        /// entities (also identified by their entity ID's).
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via their entity ID's.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityIDWithAvoids(long entityID1,
                                          long entityID2,
                                          int maxDegrees,
                                          string avoidedEntities,
                                          out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by entity ID with the avoidance (preferred or strict) of specific
        /// entities (also identified by their entity ID's).
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via their entity ID's.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityIDWithAvoids(long entityID1,
                                         long entityID2,
                                         int maxDegrees,
                                         string avoidedEntities,
                                         long flags,
                                         out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by the data soure and record ID pairs of their constituent records
        /// with the avoidance (preferred or strict) of specific entities
        /// (also identified by the data source code and record ID pairs of
        /// their constituent records).
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by the data source codes and
        /// record ID's pairs of their constituent records in a JSON document
        /// with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via the data source
        /// code and record ID pairs of their constituent records.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordIDWithAvoids(string dataSourceCode1,
                                          string recordID1,
                                          string dataSourceCode2,
                                          string recordID2,
                                          int maxDegrees,
                                          string avoidedEntities,
                                          out string response);

        /// <summary>
        /// Finds a relationship path between entities that are identified
        /// by the data soure and record ID pairs of their constituent records
        /// with the avoidance (preferred or strict) of specific entities
        /// (also identified by the data source code and record ID pairs of
        /// their constituent records).
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by the data source codes and
        /// record ID's pairs of their constituent records in a JSON document
        /// with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via the data source
        /// code and record ID pairs of their constituent records.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordIDWithAvoids(string dataSourceCode1,
                                          string recordID1,
                                          string dataSourceCode2,
                                          string recordID2,
                                          int maxDegrees,
                                          string avoidedEntities,
                                          long flags,
                                          out string response);

        /// <summary>
        /// Finds a relationship path between two entities identified by their
        /// entity ID's with the avoidance (preferred or strict) of specific
        /// entities (also identified by their entity ID's) and the requirement
        /// <b>at least one</b> of one or more specified data sources in the path.
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// <para>
        /// The required set of data sources are identified by their data source
        /// codes in a JSON document with the following format:
        /// <code>
        ///    { "DATA_SOURCES": [
        ///        "&lt;data_source_code1&gt;",
        ///        "&lt;data_source_code2&gt;",
        ///        . . .
        ///        "&lt;data_source_codeN&gt;"
        ///      ]
        ///    }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via their entity ID's.
        /// </param>
        /// <param name="requiredSources">
        /// The JSON document identifying the data sources that must be included
        /// on the path.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityIDIncludingSource(long entityID1,
                                               long entityID2,
                                               int maxDegrees,
                                               string avoidedEntities,
                                               string requiredSources,
                                               out string response);

        /// <summary>
        /// Finds a relationship path between two entities identified by their
        /// entity ID's with the avoidance (preferred or strict) of specific
        /// entities (also identified by their entity ID's) and the requirement
        /// <b>at least one</b> of one or more specified data sources in the path.
        /// </summary>
        /// 
        /// <remarks>
        /// The avoided entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// <para>
        /// The required set of data sources are identified by their data source
        /// codes in a JSON document with the following format:
        /// <code>
        ///    { "DATA_SOURCES": [
        ///        "&lt;data_source_code1&gt;",
        ///        "&lt;data_source_code2&gt;",
        ///        . . .
        ///        "&lt;data_source_codeN&gt;"
        ///      ]
        ///    }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via their entity ID's.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="requiredSources">
        /// The JSON document identifying the data sources that must be included
        /// on the path.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByEntityIDIncludingSource(long entityID1,
                                               long entityID2,
                                               int maxDegrees,
                                               string avoidedEntities,
                                               string requiredSources,
                                               long flags,
                                               out string response);

        /// <summary>
        /// Finds a relationship path between two entities identified by the
        /// data source code and record ID pairs of their constituent records
        /// with the avoidance (preferred or strict) of specific entities (also
        /// identified by the data source code and record ID pairs of their 
        /// constituent records) and the requirement <b>at least one</b> of
        /// one or more specified data sources in the path.
        /// </summary>
        ///
        /// 
        /// <remarks>
        /// The avoided entities are identified by the data source codes and
        /// record ID's pairs of their constituent records in a JSON document
        /// with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// <para>
        /// The required set of data sources are identified by their data source
        /// codes in a JSON document with the following format:
        /// <code>
        ///    { "DATA_SOURCES": [
        ///        "&lt;data_source_code1&gt;",
        ///        "&lt;data_source_code2&gt;",
        ///        . . .
        ///        "&lt;data_source_codeN&gt;"
        ///      ]
        ///    }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via the data source
        /// code and record ID pairs of their constituent records.
        /// </param>
        /// <param name="requiredSources">
        /// The JSON document identifying the data sources that must be included
        /// on the path.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordIDIncludingSource(string dataSourceCode1,
                                               string recordID1,
                                               string dataSourceCode2,
                                               string recordID2,
                                               int maxDegrees,
                                               string avoidedEntities,
                                               string requiredSources,
                                               out string response);

        /// <summary>
        /// Finds a relationship path between two entities identified by the
        /// data source code and record ID pairs of their constituent records
        /// with the avoidance (preferred or strict) of specific entities (also
        /// identified by the data source code and record ID pairs of their 
        /// constituent records) and the requirement <b>at least one</b> of
        /// one or more specified data sources in the path.
        /// </summary>
        ///
        /// 
        /// <remarks>
        /// The avoided entities are identified by the data source codes and
        /// record ID's pairs of their constituent records in a JSON document
        /// with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// <para>
        /// The required set of data sources are identified by their data source
        /// codes in a JSON document with the following format:
        /// <code>
        ///    { "DATA_SOURCES": [
        ///        "&lt;data_source_code1&gt;",
        ///        "&lt;data_source_code2&gt;",
        ///        . . .
        ///        "&lt;data_source_codeN&gt;"
        ///      ]
        ///    }
        /// </code>
        /// </remarks>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the consituent record of the first entity.
        /// </param>
        /// <param name="recordID1">
        /// The record ID for the consituent record of the first entity.
        /// </param>
        /// <param name="dataSourceCode2">
        /// The data source code for the consituent record of the second entity.
        /// </param>
        /// <param name="recordID2">
        /// The record ID for the consituent record of the second entity.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        /// <param name="avoidedEntities">
        /// The JSON document identifying the avoided entities via the data source
        /// code and record ID pairs of their constituent records.
        /// </param>
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// <param name="requiredSources">
        /// The JSON document identifying the data sources that must be included
        /// on the path.
        /// </param>
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindPathByRecordIDIncludingSource(string dataSourceCode1,
                                               string recordID1,
                                               string dataSourceCode2,
                                               string recordID2,
                                               int maxDegrees,
                                               string avoidedEntities,
                                               string requiredSources,
                                               long flags,
                                               out string response);

        /// <summary>
        /// Finds a network of entity relationships, surrounding the paths
        /// between a set of entities that are identified by their entity IDs.
        /// </summary>
        ///
        /// <remarks>
        /// The desired entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        ///
        /// <param name="entityList">
        /// The JSON document specififying the entity ID's of the desired entities.
        /// </param>
        /// 
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        /// 
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the
        /// found entities.
        /// </param>
        /// 
        /// <param name="maxEntities">
        /// The maximum number of entities to build out to.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindNetworkByEntityID(string entityList,
                                   int maxDegrees,
                                   int buildOutDegrees,
                                   int maxEntities,
                                   out string response);

        /// <summary>
        /// Finds a network of entity relationships, surrounding the paths
        /// between a set of entities that are identified by their entity IDs.
        /// </summary>
        ///
        /// <remarks>
        /// The desired entities are identified by their entity ID's in a JSON
        /// document with the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        { "ENTITY_ID": &lt;entity_id1&gt; },
        ///        { "ENTITY_ID": &lt;entity_id2&gt; },
        ///        . . .
        ///        { "ENTITY_ID": &lt;entity_idN&gt; }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        ///
        /// <param name="entityList">
        /// The JSON document specififying the entity ID's of the desired entities.
        /// </param>
        /// 
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        /// 
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the
        /// found entities.
        /// </param>
        /// 
        /// <param name="maxEntities">
        /// The maximum number of entities to build out to.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindNetworkByEntityID(string entityList,
                                   int maxDegrees,
                                   int buildOutDegrees,
                                   int maxEntities,
                                   long flags,
                                   out string response);

        /// <summary>
        /// Finds a network of entity relationships, surrounding the paths
        /// between a set of entities that are identified by the the data
        /// source code and record ID pairs of their constituent records.
        /// </summary>
        ///
        /// <remarks>
        /// The composite records af the desired entities are identified by
        /// the data source code and record ID pairs in a JSON document with
        /// the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        ///
        /// <param name="recordList">
        /// The JSON document containing the data source code and record ID pairs
        /// for the constituent records of the desired entities.
        /// </param>
        /// 
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        /// 
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the
        /// found entities.
        /// </param>
        /// 
        /// <param name="maxEntities">
        /// The maximum number of entities to build out to.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindNetworkByRecordID(string recordList,
                                   int maxDegrees,
                                   int buildOutDegrees,
                                   int maxEntities,
                                   out string response);

        /// <summary>
        /// Finds a network of entity relationships, surrounding the paths
        /// between a set of entities that are identified by the the data
        /// source code and record ID pairs of their constituent records.
        /// </summary>
        ///
        /// <remarks>
        /// The composite records af the desired entities are identified by
        /// the data source code and record ID pairs in a JSON document with
        /// the following format:
        /// <code>
        ///   {
        ///     "ENTITIES": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        ///
        /// <param name="recordList">
        /// The JSON document containing the data source code and record ID pairs
        /// for the constituent records of the desired entities.
        /// </param>
        /// 
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        /// 
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the
        /// found entities.
        /// </param>
        /// 
        /// <param name="maxEntities">
        /// The maximum number of entities to build out to.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FindNetworkByRecordID(string recordList,
                                   int maxDegrees,
                                   int buildOutDegrees,
                                   int maxEntities,
                                   long flags,
                                   out string response);

        /// <summary>
        /// Determines why a particular record is included in its resolved entity.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code for the composite record of the subject entity.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID for the composite record of the subject entity.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyRecordInEntity(string dataSourceCode,
                               string recordID,
                               out string response);

        /// <summary>
        /// Determines why a particular record is included in its resolved entity.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code for the composite record of the subject entity.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID for the composite record of the subject entity.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyRecordInEntity(string dataSourceCode,
                               string recordID,
                               long flags,
                               out string response);

        /// <summary>
        /// Determines how two records (identified by their data source code and
        /// record ID pairs) are related to each other.
        /// </summary>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the first record.
        /// </param>
        /// 
        /// <param name="recordID1">The record ID for the first record.</param>
        /// 
        /// <param name="dataSourceCode2">
        /// The data source code for the second record.
        /// </param>
        /// 
        /// <param name="recordID2">The record ID for the second record.</param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyRecords(string dataSourceCode1,
                        string recordID1,
                        string dataSourceCode2,
                        string recordID2,
                        out string response);

        /// <summary>
        /// Determines how two records (identified by their data source code and
        /// record ID pairs) are related to each other.
        /// </summary>
        /// 
        /// <param name="dataSourceCode1">
        /// The data source code for the first record.
        /// </param>
        /// 
        /// <param name="recordID1">The record ID for the first record.</param>
        /// 
        /// <param name="dataSourceCode2">
        /// The data source code for the second record.
        /// </param>
        /// 
        /// <param name="recordID2">The record ID for the second record.</param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyRecords(string dataSourceCode1,
                        string recordID1,
                        string dataSourceCode2,
                        string recordID2,
                        long flags,
                        out string response);

        /// <summary>
        /// Determines how two entities (identified by their entity ID's) are
        /// related to each other.
        /// </summary>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyEntities(long entityID1, long entityID2, out string response);

        /// <summary>
        /// Determines how two entities (identified by their entity ID's) are
        /// related to each other.
        /// </summary>
        /// 
        /// <param name="entityID1">The entity ID of the first entity.</param>
        /// <param name="entityID2">The entity ID of the second entity.</param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long WhyEntities(long entityID1,
                         long entityID2,
                         long flags,
                         out string response);

        /// <summary>
        /// Determines how an entity (identified by its entity ID) was
        /// constructed from its base records.
        /// </summary>
        /// 
        /// <param name="entityID">The entity ID identifying the entity</entity>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long HowEntityByEntityID(long entityID, out string response);

        /// <summary>
        /// Determines how an entity (identified by its entity ID) was
        /// constructed from its base records.
        /// </summary>
        /// 
        /// <param name="entityID">The entity ID identifying the entity</entity>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long HowEntityByEntityID(long entityID, long flags, out string response);

        /// <summary>
        /// Retrieves a hypothetical entity composed of the specified records.
        /// </summary>
        /// 
        /// <param name="recordList">
        /// The list of records used to build the virtual entity.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetVirtualEntityByRecordID(string recordList, out string response);

        /// <summary>
        /// Retrieves a hypothetical entity composed of the specified records.
        /// </summary>
        /// 
        /// <param name="recordList">
        /// The list of records used to build the virtual entity.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetVirtualEntityByRecordID(string recordList,
                                        long flags,
                                        out string response);


        /// <summary>
        /// Retrieves a record that is identified by its data source code and record ID.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source of the record to retrieve.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID of the record to retrieve.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetRecord(string dataSourceCode, string recordID, out string response);

        /// <summary>
        /// Retrieves a record that is identified by its data source code and record ID.
        /// </summary>
        /// 
        /// <param name="dataSourceCode">
        /// The data source of the record to retrieve.
        /// </param>
        /// 
        /// <param name="recordID">
        /// The record ID of the record to retrieve.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetRecord(string dataSourceCode,
                       string recordID,
                       long flags,
                       out string response);

        /// <summary>
        /// Begins an export of entity data from the respository, returning an
        /// export-handle that can be used to fetch the data in single-entity
        /// chunks in JSON format.
        /// </summary>
        /// 
        /// <remarks>
        /// The exported data is read via the <see cref="fetchNext"/> function
        /// and should be closed via the <see cref="closeExport"/> function when
        /// the export is complete.
        /// </remarks>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="exportHandle">
        /// The out <c>IntPtr</c> parameter that is set to the value of the 
        /// export handle.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ExportJSONEntityReport(long flags, out IntPtr exportHandle);

        /// <summary>
        /// Begins an export of entity data from the respository, returning an
        /// export-handle that can be used to fetch the data in single entity
        /// rows in CSV format.
        /// </summary>
        ///
        /// <remarks>
        /// The exported data is read via the <see cref="fetchNext"/> function
        /// and should be closed via the <see cref="closeExport"/> function when
        /// the export is complete.
        /// <para>
        /// The first output row returned by the export-handle contains the
        /// column headers as a string.  Each following row contains the
        /// exported entity data.
        /// </remarks>
        ///
        /// <param name="csvColumnList">
        /// Specify <c>"*"</c> to indicate "all columns", specify empty-string to
        /// indicate the "standard columns", otherwise specify a comma-sepatated
        /// list of column names.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to control how the operation is performed and specifically
        /// the content of the response JSON document.
        /// </param>
        /// 
        /// <param name="exportHandle">
        /// The out <c>IntPtr</c> parameter that is set to the value of the 
        /// export handle.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ExportCSVEntityReport(string csvColumnList,
                                   long flags,
                                   out IntPtr exportHandle);

        /// <summary>
        /// Reads the next chunk of entity data from an export handle, one 
        /// entity at a time.
        /// </summary>
        ///
        /// <param name="exportHandle">The export handle for the export.</param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long FetchNext(IntPtr exportHandle, out string response);

        /// <summary>
        /// This function closes an export handle, to clean up system resources.
        /// </summary>
        /// 
        /// <param name="exportHandle">The export handle for the export.</param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long CloseExport(IntPtr exportHandle);

        /// <summary>
        /// Processes a redo record.
        /// </summary>
        /// 
        /// <param name="redoRecord">The record to be processed.</param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ProcessRedoRecord(string redoRecord);

        /// <summary>
        /// Processes a redo record.
        /// </summary>
        ///
        /// <param name="redoRecord">The record to be processed.</param>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// response document.
        /// </param>
        /// 
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long ProcessRedoRecordWithInfo(string redoRecord,
                                       out string response);


        /// <summary>
        /// Retrieves a pending redo record from the reevaluation queue.
        /// </summary>
        /// 
        /// <param name="response">
        /// The out <c>string</c> response parameter to set to the JSON 
        /// redo record.
        /// </param>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        long GetRedoRecord(out string response);

        /// <summary>
        /// Gets the number of redo records waiting to be processed.
        /// </summary>
        ///
        /// <returns>
        /// The number of redo records waiting to be processed, or a negative
        /// number if an error occurred.
        /// </returns>
        long CountRedoRecords();

    }
}