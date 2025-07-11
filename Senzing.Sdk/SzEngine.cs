using System;
using System.Collections.Generic;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;
using static Senzing.Sdk.SzFlagUsageGroup;

namespace Senzing.Sdk
{
    /// <summary>
    /// Defines the interface to the Senzing engine functions.
    /// </summary>
    ///
    /// <remarks>
    /// The Senzing engine functions primarily provide means of working with
    /// identity data records, entities and their relationships.
    /// </remarks>
    /// 
    /// <example>
    /// An `SzEngine` instance is typically obtained from an
    /// <see cref="SzEnvironment"/> instance via the
    /// <see cref="SzEnvironment.GetEngine"/> method as follows.
    /// 
    /// For example:
    /// <include file="../target/examples/SzEngineDemo_GetEngine.xml" path="/*"/>
    /// </example>
    public interface SzEngine
    {
        /// <summary>
        /// May optionally be called to pre-initialize some of the heavier weight
        /// internal resources of the <c>SzEngine</c>.
        /// </summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_PrimeEngine.xml" path="/*"/>
        /// </example>
        /// 
        /// <exception cref="SzException">If a failure occurs.</exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/initialization/EnginePriming/Program.cs">Code Snippet: Engine Priming</seealso>
        void PrimeEngine();

        /// <summary>
        /// Returns the current internal engine workload statistics for the process.
        /// The counters are reset after each call.
        /// </summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetStats.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>The <c>string</c> describing the statistics as JSON.</returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadWithStatsViaLoop/Program.cs">Code Snippet: Load with Stats</seealso>
        string GetStats();

        /// <summary>
        /// Loads the record described by the specified <c>string</c> record
        /// definition having the specified data source code and record ID using
        /// the specified bitwise-OR'd <see cref="SzFlag"/> values.
        /// </summary>
        ///
        /// <remarks>
        /// If a record already exists with the same data source code and record ID,
        /// then it will be replaced.
        /// <para>
        /// The specified JSON data may optionally contain the <c>DATA_SOURCE</c>
        /// and <c>RECORD_ID</c> properties, but, if so, they must match the
        /// specified parameters.
        /// </para>
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzAddRecordFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_AddRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code identifying the data source for the record being
        /// added.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID that uniquely identifies the record being added within 
        /// the scope of its associated data source.
        /// </param>
        ///
        /// <param name="recordDefinition">
        /// The <c>string</c> that defines the record, typically in JSON format.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzAddRecordFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// will default its value to <see cref="SzFlags.SzAddRecordDefaultFlags"/>.
        /// Specify <see cref="SzWithInfo"/> for an INFO response.  Specifying <c>null</c>
        /// is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> result produced by adding the record to the
        /// repository, or <c>null</c> if the specified flags do not indicate 
        /// that an INFO message should be returned.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzBadInputException">
        /// If the specified record definition has a data source or record ID value
        /// that conflicts with the specified data source code and/or record ID
        /// values.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzNoFlags"/>
        /// <seealso cref="SzFlag.SzWithInfo"/>
        /// <seealso cref="SzFlags.SzAddRecordDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzAddRecordFlags"/>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadRecords/Program.cs">Code Snippet: Load Records</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadTruthSetWithInfoViaLoop/Program.cs">Code Snippet: Load Truth Set "With Info"</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadViaFutures/Program.cs">Code Snippet: Load via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadViaLoop/Program.cs">Code Snippet: Load via Loop</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadViaQueue/Program.cs">Code Snippet: Load via Queue</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadWithInfoViaFutures/Program.cs">Code Snippet: Load "With Info" via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadWithStatsViaLoop/Program.cs">Code Snippet: Load "With Stats" Via Loop</seealso>
        string AddRecord(string dataSourceCode,
                         string recordID,
                         string recordDefinition,
                         SzFlag? flags = SzAddRecordDefaultFlags);

        /// <summary>
        /// Performs a hypothetical load of a the record described by the specified
        /// <c>string</c> record definition using the specified bitwise-OR'd 
        /// <see cref="SzFlag"/> values.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRecordPreviewFlags"/> group will
        /// be recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetRecordPreview.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="recordDefinition">
        /// The <c>string</c> that defines the record, typically in JSON format.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzRecordFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// will default its value to <see cref="SzRecordPreviewDefaultFlags"/>.
        /// Specifying <c>null</c> is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        /// 
        /// <returns>
        /// The JSON <c>string</c> result produced by getting a record preview
        /// (depending on the specified flags).
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzRecordPreviewDefaultFlags"/>
        /// <seealso cref="SzRecordPreviewFlags"/>
        string GetRecordPreview(string recordDefinition,
                                SzFlag? flags = SzRecordPreviewDefaultFlags);

        /// <summary>
        /// Delete a previously loaded record identified by the specified 
        /// data source code and record ID.
        /// </summary>
        ///
        /// <remarks>
        /// This method is idempotent, meaning multiple calls this method with
        /// the same parameters will all succeed regardless of whether or not the
        /// record is found in the repository.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzDeleteRecordFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_DeleteRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code identifying the data source for the record being
        /// deleted.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID that uniquely identifies the record being deleted within 
        /// the scope of its associated data source.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzDeleteRecordFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// will default its value to <see cref="SzDeleteRecordDefaultFlags"/>.  Specify 
        /// <see cref="SzWithInfo"/> for an INFO response.  Specifying <c>null</c>
        /// is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> result produced by deleting the record from
        /// the repository, or <c>null</c> if the specified flags do not
        /// indicate that an INFO message should be returned.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzNoFlags"/>
        /// <seealso cref="SzWithInfo"/>
        /// <seealso cref="SzDeleteRecordFlags"/>
        /// <seealso cref="SzDeleteRecordDefaultFlags"/>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/deleting/DeleteViaLoop/Program.cs">Code Snippet: Delete via Loop</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/deleting/DeleteViaFutures/Program.cs">Code Snippet: Delete via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/deleting/DeleteWithInfoViaFutures/Program.cs">Code Snippet: Delete "With Info" via Futures</seealso>
        string DeleteRecord(string dataSourceCode,
                            string recordID,
                            SzFlag? flags = SzDeleteRecordDefaultFlags);

        /// <summary>
        /// Reevaluate the record identified by the specified data source code
        /// and record ID.
        /// </summary>
        /// <remarks>
        /// If the data source code is not recognized then an
        /// <see cref="SzUnknownDataSourceException"/> is thrown but if the record
        /// for the record ID is not found, then the operation silently does nothing
        /// with no exception.  This is to ensure consistent behavior in case of a
        /// race condition with record deletion.  To ensure that the record was
        /// found, specify the <see cref="SzWithInfo"/> flag and check the returned
        /// INFO document for affected entities.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzReevaluateRecordFlags"/> group will
        /// be recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ReevaluateRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <param name="dataSourceCode">
        /// The data source code identifying the data source for the record to
        /// reevaluate.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID that uniquely identifies the record to reevaluate within 
        /// the scope of its associated data source.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzReevaluateRecordFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter will default its value to
        /// <see cref="SzFlags.SzReevaluateRecordDefaultFlags"/>.   Specify
        /// <see cref="SzWithInfo"/> for an INFO response.  Specifying <c>null</c>
        /// is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> result produced by reevaluating the record,
        /// or <c>null</c> if the specified flags do not indicate that an INFO
        /// message should be returned.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzNoFlags"/>
        /// <seealso cref="SzFlag.SzWithInfo"/>
        /// <seealso cref="SzFlags.SzReevaluateRecordDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzReevaluateRecordFlags"/>
        string ReevaluateRecord(string dataSourceCode,
                                 string recordID,
                                 SzFlag? flags = SzReevaluateRecordDefaultFlags);

        /// <summary>
        /// Reevaluate a resolved entity identified by the specified entity ID.
        /// </summary>
        /// <remarks>
        /// If the entity for the entity ID is not found, then the operation
        /// silently does nothing with no exception.  This is to ensure consistent
        /// behavior in case of a race condition with entity re-resolve or unresolve.
        /// To ensure that the entity was found, specify the
        /// <see cref="SzFlag.SzWithInfo"/> flag and check the returned INFO document
        /// for affected entities.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzReevaluateEntityFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ReevaluateEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityID">
        /// The ID of the resolved entity to reevaluate.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzFlagUsageGroup.SzReevaluateEntityFlags"/> group to
        /// control how the operation is performed and the content of the response.
        /// Omitting this parameter will default its value to
        /// <see cref="SzFlags.SzReevaluateEntityDefaultFlags"/>.  Specify
        /// <see cref="SzFlag.SzWithInfo"/> for an INFO response.  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> result produced by reevaluating the entity,
        /// or <c>null</c> if the specified flags do not indicate that an INFO
        /// message should be returned.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzNoFlags"/>
        /// <seealso cref="SzFlag.SzWithInfo"/>
        /// <seealso cref="SzFlags.SzReevaluateEntityDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzReevaluateEntityFlags"/>
        string ReevaluateEntity(long entityID,
                                SzFlag? flags = SzReevaluateEntityDefaultFlags);

        /// <summary>
        /// This method searches for entities that match or relate to the provided
        /// search attributes using the optionally specified search profile.
        /// </summary>
        /// <remarks>
        /// The specified search attributes are treated as a hypothetical record and 
        /// the search results are those entities that would match or relate to 
        /// that hypothetical record on some level (depending on the specified flags).
        /// <para>
        /// If the specified search profile is <c>null</c> then the default
        /// generic thresholds from the default search profile will be used for the
        /// search (alternatively, use <see cref="SearchByAttributes(string,SzFlag?)"/>
        /// to omit the parameter).  If your search requires different behavior using
        /// alternate generic thresholds, please contact 
        /// <see href="mailto:support@senzing.com">support@senzing.com</see> for
        /// details on configuring a custom search profile.
        /// </para>
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the search is performed but also the content of the
        /// response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzSearchFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_SearchByAttributesWithProfile.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="attributes">
        /// The search attributes defining the hypothetical record to match and/or
        /// relate to in order to obtain the search results.
        /// </param>
        ///
        /// <param name="searchProfile">
        /// The optional search profile identifier, or <c>null</c> if the default
        /// search profile should be used for the search.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzFlagUsageGroup.SzSearchFlags"/> group to control
        /// how the operation is performed and the content of the response,
        /// omitting this parameter will default its value to 
        /// <see cref="SzFlags.SzSearchByAttributesDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The resulting JSON <c>string</c> describing the result of the search.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SearchByAttributes(string, SzFlag?)"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesDefaultFlags"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesAll"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesStrong"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesMinimalAll"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesMinimalStrong"/>
        /// <seealso cref="SzFlagUsageGroup.SzSearchFlags"/>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/searching/SearchRecords/Program.cs">Code Snippet: Search Records</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/searching/SearchViaFutures/Program.cs">Code Snippet: Search via Futures</seealso>
        string SearchByAttributes(
            string attributes,
            string searchProfile,
            SzFlag? flags = SzSearchByAttributesDefaultFlags);

        /// <summary>
        /// Convenience method for calling
        /// <see cref="SearchByAttributes(string,string,SzFlag?)"/>
        /// with a <c>null</c> value for the search profile parameter.
        /// </summary>
        /// <remarks>
        /// See <see cref="SearchByAttributes(string,string,SzFlag?)"/>
        /// documentation for details.
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_SearchByAttributes.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="attributes">
        /// The search attributes defining the hypothetical record to match and/or
        /// relate to in order to obtain the search results.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzFlagUsageGroup.SzSearchFlags"/> group to control
        /// how the operation is performed and the content of the response,
        /// omitting this parameter will default its value to 
        /// <see cref="SzFlags.SzSearchByAttributesDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The resulting JSON <c>string</c> describing the result of the search.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SearchByAttributes(string, string, SzFlag?)"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesDefaultFlags"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesAll"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesStrong"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesMinimalAll"/>
        /// <seealso cref="SzFlags.SzSearchByAttributesMinimalStrong"/>
        /// <seealso cref="SzFlagUsageGroup.SzSearchFlags"/>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/searching/SearchRecords/Program.cs">Code Snippet: Search Records</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/searching/SearchViaFutures/Program.cs">Code Snippet: Search via Futures</seealso>
        string SearchByAttributes(
            string attributes,
            SzFlag? flags = SzSearchByAttributesDefaultFlags);

        /// <summary>
        /// Compares the specified search attribute criteria against the entity
        /// identified by the specified entity ID to determine why that entity was
        /// or was not included in the results of a "search by attributes" operation.
        /// </summary>
        /// <remarks>
        /// The specified search attributes are treated as a hypothetical
        ///  single-record entity and the result of this operation is the
        /// "why analysis" of the entity identified by the specified entity ID
        /// against that hypothetical entity.  The details included in the response
        /// are determined by the specified flags.
        /// <para>
        /// If the specified search profile is <c>null</c> then the default
        /// generic thresholds from the default search profile will be used for
        /// the search candidate determination.  If your search requires different
        /// behavior using alternate generic thresholds, please contact 
        /// <see href="mailto:support@senzing.com">support@senzing.com</see> for
        /// details on configuring a custom search profile.
        /// </para>
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the search is performed but also the content of the
        /// response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzWhySearchFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_WhySearch.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="attributes">
        /// The search attributes defining the hypothetical record to match
        /// and/or relate to in order to obtain the search results.
        /// </param>
        ///
        /// <param name="entityID">
        /// The entity ID identifying the entity to analyze against the
        /// search attribute criteria.
        /// </param>
        ///
        /// <param name="searchProfile">
        /// The optional search profile identifier, or <c>null</c> if the default
        /// search profile should be used for the search.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzFlagUsageGroup.SzWhySearchFlags"/> group to control
        /// how the operation is performed and the content of the response,
        /// omitting this parameter will default its value to 
        /// <see cref="SzFlags.SzWhySearchDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The resulting JSON <c>string</c> describing the result of the
        /// why analysis against the search criteria.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If no entity could be found with the specified entity ID.
        /// </exception>
        /// 
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzWhySearchDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzWhySearchFlags"/>
        string WhySearch(
            string attributes,
            long entityID,
            string searchProfile = null,
            SzFlag? flags = SzWhySearchDefaultFlags);

        /// <summary>
        /// This method is used to retrieve information about a specific resolved
        /// entity.
        /// </summary>
        ///
        /// <remarks>
        /// The result is returned as a JSON document describing the entity.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzEntityFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetEntityByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityID">
        /// The entity ID identifying the entity to retrieve.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFlagUsageGroup.SzEntityFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this 
        /// parameter defaults it value to <see cref="SzFlags.SzEntityDefaultFlags"/>
        /// for the default recommended flags.  Specifying <c>null</c> is equivalent
        /// to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>The JSON <c>string</c> describing the entity.</returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If no entity could be found with the specified entity ID.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzEntityDefaultFlags"/>
        /// <seealso cref="SzFlags.SzEntityBriefDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzEntityFlags"/>
        string GetEntity(long entityID,
                         SzFlag? flags = SzEntityDefaultFlags);

        /// <summary>
        /// This method is used to retrieve information about the resolved entity
        /// that contains a specific record that is identified by the specified 
        /// data source code and record ID.
        /// </summary>
        ///
        /// <remarks>
        /// The result is returned as a JSON document describing the entity.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzEntityFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetEntityByRecordKey.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source of the constituent
        /// record belonging to the entity to be retrieved.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID (unique within the scope of the record's data source)
        /// identifying the constituent record belonging to the entity to be retrieved.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFlagUsageGroup.SzEntityFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this 
        /// parameter defaults it value to <see cref="SzFlags.SzEntityDefaultFlags"/>
        /// for the default recommended flags.  Specifying <c>null</c> is equivalent
        /// to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>The JSON <c>string</c> describing the entity.</returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If no entity could be found with the specified entity ID.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzEntityDefaultFlags"/>
        /// <seealso cref="SzFlags.SzEntityBriefDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzEntityFlags"/>
        string GetEntity(string dataSourceCode,
                         string recordID,
                         SzFlag? flags = SzEntityDefaultFlags);

        /// <summary>
        /// An <b>experimental</b> method to obtain interesting entities pertaining
        /// to the entity identified by the specified entity ID using the specified
        /// flags.
        /// </summary>
        ///
        /// <remarks>
        /// The result is returned as a JSON document describing the interesting
        /// entities.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values
        /// may contain any flags, but only flags belonging to the
        /// <see cref="SzFindInterestingEntitiesFlags"/> group are guaranteed to
        /// be recognized (other <see cref="SzFlag"/> instances will be ignored
        /// unless they have equivalent bit flags to supported flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindInterestingByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityID">
        /// The entity ID identifying the entity that will be the focus for the
        /// interesting entities to be returned.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFindInterestingEntitiesFlags"/> group control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter will default its value to
        /// <see cref="SzFlags.SzFindInterestingEntitiesDefaultFlags"/>.  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the interesting entities.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If no entity could be found with the specified entity ID.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        /// 
        /// <seealso cref="SzFlags.SzNoFlags"/>
        /// <seealso cref="SzFlags.SzFindInterestingEntitiesDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzFindInterestingEntitiesFlags"/>
        string FindInterestingEntities(
            long entityID,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags);

        /// <summary>
        /// An <b>experimental</b> method to obtain interesting entities pertaining
        /// to the entity that contains a specific record that is identified by the
        /// specified data source code and record ID using the specified flags.
        /// </summary>
        ///
        /// <remarks>
        /// The result is returned as a JSON document describing the interesting
        /// entities.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values
        /// may contain any flags, but only flags belonging to the
        /// <see cref="SzFindInterestingEntitiesFlags"/> group are guaranteed to
        /// be recognized (other <see cref="SzFlag"/> instances will be ignored
        /// unless they have equivalent bit flags to supported flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindInterestingByRecordKey.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source of the constituent
        /// record belonging to the entity that is the focus for the interesting
        /// entities to be returned.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID (unique within the scope of the record's data source)
        /// identifying the constituent record belonging to the entity that is
        /// the focus for the interesting entities to be returned.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFindInterestingEntitiesFlags"/> group control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter will default its value to
        /// <see cref="SzFlags.SzFindInterestingEntitiesDefaultFlags"/>.  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the interesting entities.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If no record can be found for the specified record ID.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        string FindInterestingEntities(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags);

        /// <summary>
        /// Finds a relationship path between two entities identified by their
        /// entity ID's.
        /// </summary>
        ///
        /// <remarks>
        /// Entities to be avoided when finding the path may optionally be specified
        /// as a non-null <see cref="ISet{T}"/> of <c>long</c> entity ID's identifying
        /// entities to be avoided.  By default the specified entities will be avoided
        /// unless absolutely necessary to find the path.  To strictly avoid the
        /// specified entities specify the <see cref="SzFindPathStrictAvoid"/> flag.
        /// <para>
        /// Further, a <see cref="ISet{T}"/> of <c>string</c> data source codes may
        /// optionally be specified to identify required data sources.  If specified
        /// as non-null, then the required data sources <see cref="ISet{T}">set</see>
        /// contains non-null <c>string</c> data source codes that identify data
        /// sources for which a record from <b>at least one</b> must exist on the path.
        /// </para>
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the level of
        /// detail provided for the entity path and those entities on the path.
        /// Any <see cref="SzFlag"/> value may be included, but only flags
        /// belonging to the <see cref="SzFindPathFlags"/> group will be recognized
        /// (other <see cref="SzFlag"/> values will be ignored unless they have
        /// equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindPathByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="startEntityID">The entity ID of the first entity.</param>
        ///
        /// <param name="endEntityID">The entity ID of the second entity.</param>
        ///
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        ///
        /// <param name="avoidEntityIDs">
        /// The optional <see cref="System.Collections.Generic.ISet{T}"/> of <c>long</c>
        /// entity ID's identifying entities to be avoided when finding the path, or
        /// <c>null</c> if no entities are to be avoided.
        /// </param>
        ///
        /// <param name="requiredDataSources">
        /// The optional <see cref="System.Collections.Generic.ISet{T}"/> of non-null
        /// <c>string</c> data source codes identifying the data sources for
        /// which at least one record must be included on the path, or <c>null</c>
        /// if none are required.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFlagUsageGroup.SzFindPathFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter will default its value to the default recommended flags
        /// (<see cref="SzFlags.SzFindPathDefaultFlags"/>).  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzFlags.SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the resultant entity path which
        /// may be an empty path if no path exists between the two entities
        /// given the path parameters.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If either the path-start or path-end entities for the specified entity
        /// ID's cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized required data source is specified.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzFindPathDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzFindPathFlags"/>
        /// <seealso cref="FindPath(string,string,string,string,int,ISet{ValueTuple{string,string}},ISet{string},SzFlag?)"/>
        string FindPath(long startEntityID,
                        long endEntityID,
                        int maxDegrees,
                        ISet<long> avoidEntityIDs = null,
                        ISet<string> requiredDataSources = null,
                        SzFlag? flags = SzFindPathDefaultFlags);

        /// <summary>
        /// Finds a relationship path between two entities identified by the
        /// specified data source codes and record ID's of their constituent records.
        /// </summary>
        ///
        /// <remarks>
        /// Entities to be avoided when finding the path may optionally be specified
        /// as a non-null <see cref="ISet{T}"/> of tuples of data source code and
        /// record ID pairs identifying the constituent records of entities to be
        /// avoided.  By default the associated entities will be avoided unless
        /// absolutely necessary to find the path.  To strictly avoid the associated
        /// entities specify the <see cref="SzFindPathStrictAvoid"/> flag. 
        /// <para>
        /// Further, a <see cref="ISet{T}"/> of <c>string</c> data source codes may
        /// optionally be specified to identify required data sources.  If specified
        /// as non-null, then the required data sources <see cref="ISet{T}">set</see>
        /// contains non-null <c>string</c> data source codes that identify data
        /// sources for which a record from <b>at least one</b> must exist on the path.
        /// </para>
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values may
        /// contain any <see cref="SzFlag"/> value, but only flags belonging to the
        /// <see cref="SzFindPathFlags"/> group will be recognized (other
        /// <see cref="SzFlag"/> values will be ignored unless they have equivalent
        /// bit flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindPathByRecordKey.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="startDataSourceCode">
        /// The data source code identifying the data source for the starting record
        /// of the requested path.
        /// </param>
        ///
        /// <param name="startRecordID">
        /// The record ID that uniquely identifies the starting record of the requested
        /// path within the scope of its associated data source.
        /// </param>
        ///
        /// <param name="endDataSourceCode">
        /// The data source code identifying the data source for the ending record
        /// of the requested path.
        /// </param>
        ///
        /// <param name="endRecordID">
        /// The record ID that uniquely identifies the ending record of the requested
        /// path within the scope of its associated data source.
        /// </param>
        ///
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search.
        /// </param>
        ///
        /// <param name="avoidRecordKeys">
        /// The optional <see cref="ISet{T}"/> of tuples containing data source code
        /// and record ID pairs identifying records whose entities are to be avoided
        /// when finding the path, or <c>null</c> if no entities are to be avoided.
        /// </param>
        ///
        /// <param name="requiredDataSources">
        /// The optional<see cref="ISet{T}"/> of non-null <c>string</c> data source
        /// codes identifying the data sources for which at least one record must
        /// be included on the path, or <c>null</c> if none are required.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFindPathFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter will
        /// default its value to the default recommended flags
        /// (<see cref="SzFindPathDefaultFlags"/>).  Specifying <c>null</c> is
        /// equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the resultant entity path which
        /// may be an empty path if no path exists between the two entities
        /// given the path parameters.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If either the path-start or path-end records for the specified data source
        /// code and record ID pairs cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFindPathDefaultFlags"/>
        /// <seealso cref="SzFindPathFlags"/>
        /// <seealso cref="FindPath(long,long,int,ISet{long},ISet{string},SzFlag?)"/>
        string FindPath(
            string startDataSourceCode,
            string startRecordID,
            string endDataSourceCode,
            string endRecordID,
            int maxDegrees,
            ISet<(string dataSourceCode, string recordID)> avoidRecordKeys = null,
            ISet<string> requiredDataSources = null,
            SzFlag? flags = SzFindPathDefaultFlags);

        /// <summary>
        /// Finds a network of entity relationships surrounding the paths between
        /// a set of entities identified by one or more <c>long</c> entity ID's
        /// included in the specified <see cref="ISet{T}"/>.
        /// </summary>
        ///
        /// <remarks>
        /// Additionally, the maximum degrees of separation for the paths between
        /// entities must be specified so as to prevent the network growing beyond
        /// the desired size.  Further, a non-zero number of degrees to build out
        /// the network may be specified to find other related entities.  If build
        /// out is specified, it can be limited to a maximum total number of
        /// build-out entities for the whole network.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFindNetworkFlags"/> group
        /// will be recognized (other <see cref="SzFlag"/> values will be ignored
        /// unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindNetworkByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityIDs">
        /// The non-null <see cref="ISet{T}"/> of <c>long</c> entity ID's
        /// identifying the entities for which to build the network.
        /// </param>
        ///
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        ///
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the found
        /// entities on the network, or zero to prevent network build-out.
        /// </param>
        ///
        /// <param name="buildOutMaxEntities">
        /// The maximum number of entities to build out for the entire network.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFindNetworkFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter will
        /// default its value to the default recommended flags
        /// (<see cref="SzFindNetworkDefaultFlags"/>).  Specifying <c>null</c> is
        /// equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the resultant entity network
        /// and the entities on the network.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If any of the entities for the specified entity ID's cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzFindNetworkDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzFindNetworkFlags"/>
        /// <seealso cref="FindNetwork(ISet{ValueTuple{string,string}},int,int,int,SzFlag?)"/>
        string FindNetwork(
            ISet<long> entityIDs,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags);

        /// <summary>
        /// Finds a network of entity relationships surrounding the paths between
        /// a set of entities having the constituent records identified by the tuples
        /// of data source code and record ID pairs included in the specified 
        /// <see cref="ISet{T}"/>.
        /// </summary>
        ///
        /// <remarks>
        /// Additionally, the maximum degrees of separation for the paths between
        /// entities must be specified so as to prevent the network growing beyond
        /// the desired size.  Further, a non-zero number of degrees to build out
        /// the network may be specified to find other related entities.  If build
        /// out is specified, it can be limited to a maximum total number of
        /// build-out entities for the whole network.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFindNetworkFlags"/> group
        /// will be recognized (other <see cref="SzFlag"/> values will be ignored
        /// unless they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_FindNetworkByRecordKey.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="recordKeys">
        /// The non-null  <see cref="ISet{T}"/> of tuples of data source code and
        /// record ID pairs identifying the constituent records of the entities for
        /// which to build the network.
        /// </param>
        ///
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.
        /// </param>
        ///
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the found
        /// entities on the network, or zero to prevent network build-out.
        /// </param>
        ///
        /// <param name="buildOutMaxEntities">
        /// The maximum number of entities to build out for the entire network.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to the
        /// <see cref="SzFindNetworkFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter will
        /// default its value to the default recommended flags
        /// (<see cref="SzFindNetworkDefaultFlags"/>).  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the resultant entity network
        /// and the entities on the network.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If any of the records for the specified data source code and
        /// record ID pairs cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFindNetworkDefaultFlags"/>
        /// <seealso cref="SzFindNetworkFlags"/>
        /// <seealso cref="FindNetwork(ISet{long},int,int,int,SzFlag?)"/>
        string FindNetwork(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags);

        /// <summary>
        /// Determines why the record identified by the specified data source
        /// code and record ID is included in its respective entity.
        /// </summary>
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzWhyRecordInEntityFlags"/> group will be recognized
        /// (other <see cref="SzFlag"/> values will be ignored unless they have
        /// equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_WhyRecordInEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="dataSourceCode">
        /// The data source code that identifies the data source of the record.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID that identifies the record within the scope of the record's 
        /// data source.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzWhyRecordInEntityFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter defaults it value to <see cref="SzWhyRecordInEntityDefaultFlags"/>
        /// for the default recommended flags.  Specifying <c>null</c> is equivalent
        /// to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing why the record is included in its
        /// respective entity.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If any of the records for the specified data source code and record
        /// ID pairs cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzWhyRecordInEntityDefaultFlags"/>
        /// <seealso cref="SzWhyRecordInEntityFlags"/>
        /// <seealso cref="WhyEntities(long,long,SzFlag?)"/>
        /// <seealso cref="WhyRecords(string,string,string,string,SzFlag?)"/>
        string WhyRecordInEntity(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzWhyRecordInEntityDefaultFlags);

        /// <summary>
        /// Determines ways in which two records identified by their data source
        /// code and record IDs are related to each other.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzWhyRecordsFlags"/> group will be recognized
        /// (other <see cref="SzFlag"/> values will be ignored unless they have
        /// equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_WhyRecords.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="dataSourceCode1">
        /// The data source code identifying the data source for the first record.
        /// </param>
        ///
        /// <param name="recordID1">
        /// The record ID that uniquely identifies the first record within the
        /// scope of its associated data source.
        /// </param>
        ///
        /// <param name="dataSourceCode2">
        /// The data source code identifying the data source for the second record.
        /// </param>
        ///
        /// <param name="recordID2">
        /// The record ID that uniquely identifies the second record within the
        /// scope of its associated data source.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzWhyRecordsFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzWhyRecordsDefaultFlags"/> for the
        /// default recommended flags.  Specifying <c>null</c> is equivalent to
        /// specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the ways in which the records are
        /// related to one another.
        /// </returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If either of the records for the specified data source code and
        /// record ID pairs cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzWhyRecordsDefaultFlags"/>
        /// <seealso cref="SzWhyRecordsFlags"/>
        /// <seealso cref="WhyRecordInEntity(string,string,SzFlag?)"/>
        /// <seealso cref="WhyEntities(long,long,SzFlag?)"/>
        string WhyRecords(string dataSourceCode1,
                          string recordID1,
                          string dataSourceCode2,
                          string recordID2,
                          SzFlag? flags = SzWhyRecordsDefaultFlags);

        /// <summary>
        /// Determines the ways in which two entities identified by the specified
        /// entity ID's are related to each other.
        /// </summary>
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzWhyEntitiesFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_WhyEntities.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityID1">The entity ID of the first entity.</param>
        ///
        /// <param name="entityID2">The entity ID of the second entity.</param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzWhyEntitiesFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzWhyEntitiesDefaultFlags"/> for the
        /// default recommended flags.  Specifying <c>null</c> is equivalent to
        /// specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the ways in which the records are
        /// related to one another.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If either of the entities for the specified entity ID's could not be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzWhyEntitiesDefaultFlags"/>
        /// <seealso cref="SzWhyEntitiesFlags"/>
        /// <seealso cref="WhyRecords(string,string,string,string,SzFlag?)"/>
        /// <seealso cref="WhyRecordInEntity(string,string,SzFlag?)"/>
        string WhyEntities(long entityID1,
                           long entityID2,
                           SzFlag? flags = SzWhyEntitiesDefaultFlags);

        /// <summary>
        /// Determines how an entity identified by the specified entity ID was
        /// constructed from its constituent records.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzHowFlags"/> group will be recognized
        /// (other <see cref="SzFlag"/> values will be ignored unless they have
        /// equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_HowEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityID">The entity ID of the entity.</param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzHowFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzHowEntityDefaultFlags"/> for the
        /// default recommended flags.  Specifying <c>null</c> is equivalent to
        /// specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the how the entity was constructed.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If the entity for the specified entity ID could not be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzHowEntityDefaultFlags"/>
        /// <seealso cref="SzHowFlags"/>
        /// <seealso cref="GetVirtualEntity"/>
        string HowEntity(long entityID, SzFlag? flags = SzHowEntityDefaultFlags);

        /// <summary>
        /// Describes a hypothetically entity that would be composed of the one or
        /// more records identified by the tuples of data source code and record ID
        /// pairs in the specified <see cref="ISet{T}"/>.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzVirtualEntityFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless they
        /// have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetVirtualEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="recordKeys">
        /// The non-null non-empty <see cref="ISet{T}"/> of tuples of data source
        /// code and record ID pairs identifying the records to use to build the
        /// virtual entity.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzVirtualEntityFlags"/> group to control how the
        /// operation is performed and the content of the response.  Omitting this
        /// parameter defaults it value to <see cref="SzWhyEntitiesDefaultFlags"/>
        /// for the default recommended flags.  Specifying <c>null</c> is equivalent
        /// to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> describing the virtual entity having
        /// the specified constituent records.
        /// </returns>
        ///
        /// <exception cref="SzNotFoundException">
        /// If any of the records for the specified data source code and
        /// record ID pairs cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzVirtualEntityDefaultFlags"/>
        /// <seealso cref="SzHowFlags"/>
        /// <seealso cref="HowEntity(long,SzFlag?)"/>
        string GetVirtualEntity(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            SzFlag? flags = SzVirtualEntityDefaultFlags);

        /// <summary>
        /// Retrieves the record identified by the specified data source code
        /// and record ID.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRecordFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetRecord.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="dataSourceCode">
        /// The data source code identifying the data source for the record.
        /// </param>
        ///
        /// <param name="recordID">
        /// The record ID that uniquely identifies the record within the
        /// scope of its associated data source.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzRecordFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzRecordDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>The JSON <c>string</c> describing the record.</returns>
        ///
        /// <exception cref="SzUnknownDataSourceException">
        /// If an unrecognized data source code is specified.
        /// </exception>
        ///
        /// <exception cref="SzNotFoundException">
        /// If the record for the specified data source code and record ID pairs
        /// cannot be found.
        /// </exception>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzFlags.SzRecordDefaultFlags"/>
        /// <seealso cref="SzFlagUsageGroup.SzRecordFlags"/>
        string GetRecord(string dataSourceCode,
                         string recordID,
                         SzFlag? flags = SzRecordDefaultFlags);

        /// <summary>
        /// Initiates an export of entity data as JSON-lines format and returns an
        /// "export handle" that can be used to <see cref="FetchNext">read
        /// the export data</see> and must be 
        /// <see cref="CloseExportReport">closed</see> when complete.
        /// </summary>
        ///
        /// <remarks>
        /// Each output line contains the exported entity data for a single resolved
        /// entity.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzExportFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ExportJson.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzExportFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzExportDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>The export handle to use for retrieving the export data.</returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzExportDefaultFlags"/>
        /// <seealso cref="SzExportFlags"/>
        /// <seealso cref="FetchNext"/>
        /// <seealso cref="CloseExportReport"/>
        /// <seealso cref="ExportCsvEntityReport"/>
        IntPtr ExportJsonEntityReport(SzFlag? flags = SzExportDefaultFlags);

        /// <summary>
        /// Initiates an export of entity data in CSV format and returns an
        /// "export handle" that can be used to <see cref="FetchNext">read
        /// the export data</see> and must be
        /// <see cref="CloseExportReport">closed</see> when complete.
        /// </summary>
        ///
        /// <remarks>
        /// The first exported line contains the CSV header and each subsequent line
        /// contains the exported entity data for a single resolved entity.
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzExportFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless they
        /// have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ExportCsv.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="csvColumnList">
        /// Specify <c>"*"</c> to indicate "all columns", specify empty-string to
        /// indicate the "standard columns", otherwise specify a comma-separated
        /// list of column names.
        /// </param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging to
        /// the <see cref="SzExportFlags"/> group to control how the operation is
        /// performed and the content of the response.  Omitting this parameter
        /// defaults it value to <see cref="SzExportDefaultFlags"/> for the default
        /// recommended flags.  Specifying <c>null</c> is equivalent to specifying
        /// <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>The export handle to use for retrieving the export data.</returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzExportDefaultFlags"/>
        /// <seealso cref="SzExportFlags"/>
        /// <seealso cref="FetchNext"/>
        /// <seealso cref="CloseExportReport"/>
        /// <seealso cref="ExportJsonEntityReport(SzFlag?)"/>
        IntPtr ExportCsvEntityReport(
            string csvColumnList,
            SzFlag? flags = SzExportDefaultFlags);

        /// <summary>
        /// Fetches the next line of entity data from the export identified
        /// by the specified export handle.
        /// </summary>
        ///
        /// <remarks>
        /// The specified export handle can be obtained from
        /// <see cref="ExportJsonEntityReport"/> or
        /// <see cref="ExportCsvEntityReport"/>.
        /// </remarks>
        ///
        /// <example>
        /// Usage (JSON Export):
        /// <include file="../target/examples/SzEngineDemo_ExportJson.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// Usage (CSV Export):
        /// <include file="../target/examples/SzEngineDemo_ExportCsv.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="exportHandle">
        /// The export handle to identify the export from which to retrieve
        /// the next line of data.
        /// </param>
        ///
        /// <returns>
        /// The next line of export data whose format depends on which function
        /// was used to initiate the export, or <c>null</c> if there is no more
        /// data to be exported via the specified handle.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="CloseExportReport"/>
        /// <seealso cref="ExportJsonEntityReport"/>
        /// <seealso cref="ExportCsvEntityReport"/>
        string FetchNext(IntPtr exportHandle);

        /// <summary>
        /// This function closes an export handle of a previously opened 
        /// export to clean up system resources.
        /// </summary>
        ///
        /// <remarks>
        /// This function is idempotent and may be called for an export
        /// that has already been closed.
        /// </remarks>
        ///
        /// <example>
        /// Usage (JSON Export):
        /// <include file="../target/examples/SzEngineDemo_ExportJson.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// Usage (CSV Export):
        /// <include file="../target/examples/SzEngineDemo_ExportCsv.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="exportHandle">
        /// The export handle of the export to close.
        /// </param>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="FetchNext"/>
        /// <seealso cref="ExportJsonEntityReport"/>
        /// <seealso cref="ExportCsvEntityReport"/>
        void CloseExportReport(IntPtr exportHandle);

        /// <summary>
        /// Processes the specified redo record using the specified flags.
        /// The redo record can be retrieved from <see cref="GetRedoRecord"/>.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRedoFlags"/> group will be 
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ProcessRedos.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="redoRecord">The redo record to be processed.</param>
        ///
        /// <param name="flags">
        /// The optional bitwise-OR'd <see cref="SzFlag"/> values belonging
        /// to the <see cref="SzRedoFlags"/> group to control how the operation
        /// is performed and the content of the response.  Omitting this parameter
        /// will default its value to <see cref="SzRedoDefaultFlags"/>.  Specify
        /// <see cref="SzWithInfo"/> for an INFO response.  Specifying
        /// <c>null</c> is equivalent to specifying <see cref="SzNoFlags"/>.
        /// </param>
        ///
        /// <returns>
        /// The JSON <c>string</c> result produced by processing the redo record,
        /// or <c>null</c> if the specified flags do not indicate that an INFO
        /// message should be returned.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="SzWithInfo"/>
        /// <seealso cref="SzRedoFlags"/>
        /// <seealso cref="SzRedoDefaultFlags"/>
        /// <seealso cref="GetRedoRecord"/>
        /// <seealso cref="CountRedoRecords"/> 
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/LoadWithRedoViaLoop/Program.cs">Code Snippet: Processing Redos while Loading</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuous/Program.cs">Code Snippet: Continuous Redo Processing</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuousViaFutures/Program.cs">Code Snippet: Continuous Redo Processing via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoWithInfoContinuous/Program.cs">Code Snippet: Continuous Redo "With Info" Processing</seealso>
        string ProcessRedoRecord(string redoRecord, SzFlag? flags = SzRedoDefaultFlags);

        /// <summary>
        /// Retrieves a pending redo record from the reevaluation queue.
        /// </summary>
        ///
        /// <remarks>
        /// If no redo records are available then this returns an <c>null</c>.
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ProcessRedos.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The retrieved redo record or <c>null</c> if there were no pending
        /// redo records.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="ProcessRedoRecord"/>
        /// <seealso cref="CountRedoRecords"/>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/LoadWithRedoViaLoop/Program.cs">Code Snippet: Processing Redos while Loading</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuous/Program.cs">Code Snippet: Continuous Redo Processing</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuousViaFutures/Program.cs">Code Snippet: Continuous Redo Processing via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoWithInfoContinuous/Program.cs">Code Snippet: Continuous Redo "With Info" Processing</seealso>
        string GetRedoRecord();

        /// <summary>
        /// Gets the number of redo records pending to be processed.
        /// </summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_ProcessRedos.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The number of redo records pending to be processed.
        /// </returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        ///
        /// <seealso cref="ProcessRedoRecord"/>
        /// <seealso cref="GetRedoRecord"/>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/LoadWithRedoViaLoop/Program.cs">Code Snippet: Processing Redos while Loading</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuous/Program.cs">Code Snippet: Continuous Redo Processing</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoContinuousViaFutures/Program.cs">Code Snippet: Continuous Redo Processing via Futures</seealso>
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/redo/RedoWithInfoContinuous/Program.cs">Code Snippet: Continuous Redo "With Info" Processing</seealso>
        long CountRedoRecords();
    }
}