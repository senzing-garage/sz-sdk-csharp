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
        /// Pre-loads engine resources.
        /// </summary>
        /// 
        /// <remarks>
        /// Explicitly calling this method ensures the performance cost is incurred
        /// at a predictable time rather than unexpectedly with the first call 
        /// requiring the resources.
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_PrimeEngine.xml" path="/*"/>
        /// </example>
        /// 
        /// <exception cref="SzException">If a failure occurs.</exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/initialization/EnginePriming/Program.cs">Code Snippet: Engine Priming</seealso>
        void PrimeEngine();

        /// <summary>
        /// Gets and resets the internal engine workload statistics for
        /// the current operating system process.
        /// </summary>
        /// 
        /// <remarks>
        /// The output is helpful when interacting with Senzing support.
        /// Best practice to periodically log the results.
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetStats.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b><br/>
        /// The example result is rather large, but can be viewed
        /// <see target="_blank" href="../examples/results/SzEngineDemo_GetStats.txt">here</see>
        /// (formatted for readability). 
        /// </example>
        /// 
        /// <returns>The <c>string</c> describing the statistics as JSON.</returns>
        ///
        /// <exception cref="SzException">If a failure occurs.</exception>
        /// 
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/loading/LoadWithStatsViaLoop/Program.cs">Code Snippet: Load with Stats</seealso>
        string GetStats();

        /// <summary>
        /// Loads a record into the repository and performs entity resolution.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// If a record already exists with the same data source code and
        /// record ID, it will be replaced.  If the record definition contains
        /// <c>DATA_SOURCE</c> and <c>RECORD_ID</c> JSON keys, the values must
        /// match the <c>dataSourceCode</c> and <c>recordId</c> parameters.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_AddRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_AddRecord.xml" path="/*"/>
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
        [SzConfigRetryable]
        string AddRecord(string dataSourceCode,
                         string recordID,
                         string recordDefinition,
                         SzFlag? flags = SzAddRecordDefaultFlags);

        /// <summary>
        /// Describes the features resulting from the hypothetical load of a record.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method is used to preview the features for a record that has not
        /// been loaded.
        /// </para>
        /// 
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRecordPreviewFlags"/> group will
        /// be recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetRecordPreview.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_GetRecordPreview.xml" path="/*"/>
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
        [SzConfigRetryable]
        string GetRecordPreview(string recordDefinition,
                                SzFlag? flags = SzRecordPreviewDefaultFlags);

        /// <summary>
        /// Deletes a record from the repository and performs entity resolution.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// <b>NOTE:</b> This method is idempotent in that it succeeds with
        /// no changes being made when the record is not found in the repository.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_DeleteRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_DeleteRecord.xml" path="/*"/>
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
        [SzConfigRetryable]
        string DeleteRecord(string dataSourceCode,
                            string recordID,
                            SzFlag? flags = SzDeleteRecordDefaultFlags);

        /// <summary>
        /// Reevaluates an entity by record ID.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This operation performs entity resolution.  If the record is not found,
        /// then no changes are made.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_ReevaluateRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_ReevaluateRecord.xml" path="/*"/>
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
        [SzConfigRetryable]
        string ReevaluateRecord(string dataSourceCode,
                                 string recordID,
                                 SzFlag? flags = SzReevaluateRecordDefaultFlags);

        /// <summary>
        /// Reevaluates an entity by entity ID.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This operation performs entity resolution.  If the entity is not found,
        /// then no changes are made.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_ReevaluateEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_ReevaluateEntity.xml" path="/*"/>
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
        [SzConfigRetryable]
        string ReevaluateEntity(long entityID,
                                SzFlag? flags = SzReevaluateEntityDefaultFlags);

        /// <summary>
        /// Searches for entities that match or relate to the provided attributes.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The default search profile is <c>"SEARCH"</c>.  Alternatively, 
        /// <c>"INGEST"</c> may be used.
        /// </para>
        /// 
        /// <para>
        /// If the specified search profile is <c>null</c> then the default
        /// will be used (alternatively, use 
        /// <see cref="SearchByAttributes(string,SzFlag?)"/> as a convenience
        /// method to omit the parameter).
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_SearchByAttributesWithProfile.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_SearchByAttributesWithProfile.xml" path="/*"/>
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
        [SzConfigRetryable]
        string SearchByAttributes(
            string attributes,
            string searchProfile,
            SzFlag? flags = SzSearchByAttributesDefaultFlags);

        /// <summary>
        /// Convenience method for calling
        /// <see cref="SearchByAttributes(string,string,SzFlag?)"/>
        /// with a <c>null</c> value for the search profile parameter.
        /// </summary>
        /// 
        /// <remarks>
        /// See <see cref="SearchByAttributes(string,string,SzFlag?)"/>
        /// documentation for details.
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_SearchByAttributes.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_SearchByAttributes.xml" path="/*"/>
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
        [SzConfigRetryable]
        string SearchByAttributes(
            string attributes,
            SzFlag? flags = SzSearchByAttributesDefaultFlags);

        /// <summary>
        /// Describes the ways a set of search attributes relate to an entity.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The default search profile is <c>"SEARCH"</c>.  Alternatively, 
        /// <c>"INGEST"</c> may be used.
        /// </para>
        /// 
        /// <para>
        /// If the specified search profile is <c>null</c> then the default
        /// will be used.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_WhySearch.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_WhySearch.xml" path="/*"/>
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
        [SzConfigRetryable]
        string WhySearch(
            string attributes,
            long entityID,
            string searchProfile = null,
            SzFlag? flags = SzWhySearchDefaultFlags);

        /// <summary>
        /// Retrieves information about an entity, specified by entity ID.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzEntityFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetEntityByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_GetEntityByEntityID.xml" path="/*"/>
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
        [SzConfigRetryable]
        string GetEntity(long entityID,
                         SzFlag? flags = SzEntityDefaultFlags);

        /// <summary>
        /// Retrieves information about an entity, specified by record ID.
        /// </summary>
        ///
        /// <remarks>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzFlagUsageGroup.SzEntityFlags"/>
        /// group will be recognized (other <see cref="SzFlag"/> values will be
        /// ignored unless they have equivalent bit flags to recognized flags).
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetEntityByRecordKey.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_GetEntityByRecordKey.xml" path="/*"/>
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
        [SzConfigRetryable]
        string GetEntity(string dataSourceCode,
                         string recordID,
                         SzFlag? flags = SzEntityDefaultFlags);

        /// <summary>
        /// Experimental method.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Contact Senzing support for the further information.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
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
        [SzConfigRetryable]
        string FindInterestingEntities(
            long entityID,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags);

        /// <summary>
        /// Experimental method.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Contact Senzing support for the further information.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
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
        [SzConfigRetryable]
        string FindInterestingEntities(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags);

        /// <summary>
        /// Searches for the shortest relationship path between two entities,
        /// specified by entity IDs.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The returned path is the shortest path among the paths that satisfy
        /// the parameters.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_FindPathByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_FindPathByEntityID.xml" path="/*"/>
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
        /// The optional <see cref="ISet{T}"/> of <c>long</c> entity ID's identifying
        /// entities to be avoided when finding the path, or <c>null</c> if no 
        /// entities are to be avoided.  By default the entities will be avoided unless
        /// necessary to find the path.  To strictly avoid the entities specify the 
        /// <see cref="SzFindPathStrictAvoid"/> flag.
        /// </param>
        ///
        /// <param name="requiredDataSources">
        /// The optional <see cref="ISet{T}"/> of non-null <c>string</c> data source
        /// codes identifying the data sources for which at least one record must be
        /// included on the path, or <c>null</c> if none are required.
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
        [SzConfigRetryable]
        string FindPath(long startEntityID,
                        long endEntityID,
                        int maxDegrees,
                        ISet<long> avoidEntityIDs = null,
                        ISet<string> requiredDataSources = null,
                        SzFlag? flags = SzFindPathDefaultFlags);

        /// <summary>
        /// Searches for the shortest relationship path between two entities,
        /// specified by record IDs.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The returned path is the shortest path among the paths that satisfy
        /// the parameters.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_FindPathByRecordKey.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_FindPathByRecordKey.xml" path="/*"/>
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
        /// By default the entities will be avoided unless necessary to find the path.
        /// To strictly avoid the entities specify the 
        /// <see cref="SzFindPathStrictAvoid"/> flag.
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
        [SzConfigRetryable]
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
        /// Retrieves a network of relationships among entities, specified by
        /// entity IDs.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// <b>WARNING:</b> Entity networks may be very large due to the volume
        /// of inter-related data in the repository.  The parameters of this
        /// method can be used to limit the information returned.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_FindNetworkByEntityID.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_FindNetworkByEntityID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="entityIDs">
        /// The non-null <see cref="ISet{T}"/> of <c>long</c> entity ID's
        /// identifying the entities for which to build the network.
        /// </param>
        ///
        /// <param name="maxDegrees">
        /// The maximum number of degrees for the path search between the specified
        /// entities.  The maximum degrees of separation for the paths between
        /// entities must be specified so as to prevent the network growing beyond
        /// the desired size.
        /// </param>
        ///
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the found
        /// entities on the network, or zero to prevent network build-out.  If this
        /// is non-zero, the size of the network can be limited to a maximum total
        /// number of build-out entities for the whole network.
        /// </param>
        ///
        /// <param name="buildOutMaxEntities">
        /// The maximum number of entities to build out for the entire network.
        /// This limits the size of the build-out network when the build-out
        /// degrees is non-zero.
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
        [SzConfigRetryable]
        string FindNetwork(
            ISet<long> entityIDs,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags);

        /// <summary>
        /// Retrieves a network of relationships among entities, specified by
        /// record IDs.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// <b>WARNING:</b> Entity networks may be very large due to the volume
        /// of inter-related data in the repository.  The parameters of this
        /// method can be used to limit the information returned.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_FindNetworkByRecordKey.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_FindNetworkByRecordKey.xml" path="/*"/>
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
        /// entities.  The maximum degrees of separation for the paths between
        /// entities must be specified so as to prevent the network growing beyond
        /// the desired size.
        /// </param>
        ///
        /// <param name="buildOutDegrees">
        /// The number of relationship degrees to build out from each of the found
        /// entities on the network, or zero to prevent network build-out.  If this
        /// is non-zero, the size of the network can be limited to a maximum total
        /// number of build-out entities for the whole network.
        /// </param>
        ///
        /// <param name="buildOutMaxEntities">
        /// The maximum number of entities to build out for the entire network.
        /// This limits the size of the build-out network when the build-out
        /// degrees is non-zero.
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
        [SzConfigRetryable]
        string FindNetwork(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags);

        /// <summary>
        /// Describes the ways a record relates to the rest of its respective entity.
        /// </summary>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_WhyRecordInEntity.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_WhyRecordInEntity.xml" path="/*"/>
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
        [SzConfigRetryable]
        string WhyRecordInEntity(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzWhyRecordInEntityDefaultFlags);

        /// <summary>
        /// Describes the ways two records relate to each other.
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_WhyRecords.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_WhyRecords.xml" path="/*"/>
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
        [SzConfigRetryable]
        string WhyRecords(string dataSourceCode1,
                          string recordID1,
                          string dataSourceCode2,
                          string recordID2,
                          SzFlag? flags = SzWhyRecordsDefaultFlags);

        /// <summary>
        /// Describes the ways two entities relate to each other.
        /// </summary>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_WhyEntities.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_WhyEntities.xml" path="/*"/>
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
        [SzConfigRetryable]
        string WhyEntities(long entityID1,
                           long entityID2,
                           SzFlag? flags = SzWhyEntitiesDefaultFlags);

        /// <summary>
        /// Explains how an entity was constructed from its records.
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_HowEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_HowEntity.xml" path="/*"/>
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
        [SzConfigRetryable]
        string HowEntity(long entityID, SzFlag? flags = SzHowEntityDefaultFlags);

        /// <summary>
        /// Describes how an entity would look if composed of a given set of records.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Virtual entities do not have relationships.
        /// </para>
        /// 
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzVirtualEntityFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless they
        /// have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetVirtualEntity.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_GetVirtualEntity.xml" path="/*"/>
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
        /// <seealso cref="SzVirtualEntityDefaultFlags"/>
        /// <seealso cref="SzHowFlags"/>
        /// <seealso cref="HowEntity(long,SzFlag?)"/>
        [SzConfigRetryable]
        string GetVirtualEntity(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            SzFlag? flags = SzVirtualEntityDefaultFlags);

        /// <summary>
        /// Retrieves information about a record.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The returned information contains the original record data that was
        /// loaded and may contain other information depending on the flags parameter.
        /// </para>
        /// 
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only control how the operation is performed but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRecordFlags"/> group will be
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_GetRecord.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_GetRecord.xml" path="/*"/>
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
        [SzConfigRetryable]
        string GetRecord(string dataSourceCode,
                         string recordID,
                         SzFlag? flags = SzRecordDefaultFlags);

        /// <summary>
        /// Initiates an export report of entity data in JSON Lines format.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Used in conjunction with <see cref="FetchNext"/> and 
        /// <see cref="CloseExportReport"/>.  Each <see cref="FetchNext"/>
        /// call returns exported entity data as a JSON object.
        /// </para>
        /// 
        /// <para>
        /// <b>WARNING:</b> This method should only be used on systems containing
        /// less than a few million records.  For larger systems, see 
        /// <see href="https://www.senzing.com/docs/tutorials/advanced_replication/"/>.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_ExportJson.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Complete Export Report:</b>
        /// <include file="../target/examples/results/SzEngineDemo_ExportJson.xml" path="/*"/>
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
        [SzConfigRetryable]
        IntPtr ExportJsonEntityReport(SzFlag? flags = SzExportDefaultFlags);

        /// <summary>
        /// Initiates an export report of entity data in CSV format.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Used in conjunction with <see cref="FetchNext"/> and 
        /// <see cref="CloseExportReport"/>.  The first <see cref="FetchNext"/>
        /// call after calling this method returns the CSV header.  Subsequent
        /// <see cref="FetchNext"/> calls return exported entity data in 
        /// CSV format.
        /// </para>
        /// 
        /// <para>
        /// <b>WARNING:</b> This method should only be used on systems containing
        /// less than a few million records.  For larger systems, see 
        /// <see href="https://www.senzing.com/docs/tutorials/advanced_replication/"/>.
        /// </para>
        /// 
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
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_ExportCsv.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Complete Export Report:</b>
        /// <include file="../target/examples/results/SzEngineDemo_ExportCsv.xml" path="/*"/>
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
        [SzConfigRetryable]
        IntPtr ExportCsvEntityReport(
            string csvColumnList,
            SzFlag? flags = SzExportDefaultFlags);

        /// <summary>
        /// Fetches the next line of entity data from an open export report.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// Used in conjunction with <see cref="ExportJsonEntityReport"/>,
        /// <see cref="ExportCsvEntityReport"/> and <see cref="CloseExportReport"/>.
        /// </para>
        /// 
        /// <para>
        /// If the export handle was obtained from <see cref="ExportCsvEntityReport"/>,
        /// this returns the CSV header on the first call and exported entity data
        /// in CSV format on subsequent calls.
        /// </para>
        /// 
        /// <para>
        /// If the export handle was obtained from <see cref="ExportJsonEntityReport"/>
        /// this returns exported entity data as a JSON object.
        /// </para>
        /// 
        /// <para>
        /// When <c>null</c> is returned, the export report is complete and the 
        /// caller should invoke <see cref="CloseExportReport"/> to free resources.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// Usage (JSON Export):
        /// <include file="../target/examples/SzEngineDemo_ExportJson.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example JSON Line Result:</b>
        /// <include file="../target/examples/results/SzEngineDemo_ExportJson-fetchNext.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// Usage (CSV Export):
        /// <include file="../target/examples/SzEngineDemo_ExportCsv.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example CSV Header Result:</b>
        /// <include file="../target/examples/results/SzEngineDemo_ExportCsv-fetchNext-header.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example CSV Data Result:</b>
        /// <include file="../target/examples/results/SzEngineDemo_ExportCsv-fetchNext-data.xml" path="/*"/>
        /// </example>
        /// 
        /// <param name="exportHandle">
        /// The export handle for the export report from which to retrieve
        /// the next line of data.
        /// </param>
        ///
        /// <returns>
        /// The next line of export data whose format depends on which function
        /// was used to initiate the export, or <c>null</c> if there is no more
        /// data to be exported via the specified handle.
        /// </returns>
        ///
        /// <exception cref="SzException">
        /// If the specified export handle has already been 
        /// <see cref="CloseExportReport">closed</see> or if
        /// any other failure occurs.
        /// </exception>
        ///
        /// <seealso cref="CloseExportReport"/>
        /// <seealso cref="ExportJsonEntityReport"/>
        /// <seealso cref="ExportCsvEntityReport"/>
        [SzConfigRetryable]
        string FetchNext(IntPtr exportHandle);

        /// <summary>
        /// Closes an export report.
        /// </summary>
        ///
        /// <remarks>
        /// Used in conjunction with <see cref="ExportJsonEntityReport"/>,
        /// <see cref="ExportCsvEntityReport"/> and <see cref="FetchNext"/>.
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
        /// The export handle of the export report to close.
        /// </param>
        ///
        /// <exception cref="SzException">
        /// If the specified export handle has already been 
        /// <see cref="CloseExportReport">closed</see> or if
        /// any other failure occurs.
        /// </exception>
        ///
        /// <seealso cref="FetchNext"/>
        /// <seealso cref="ExportJsonEntityReport"/>
        /// <seealso cref="ExportCsvEntityReport"/>
        void CloseExportReport(IntPtr exportHandle);

        /// <summary>
        /// Processes the provided redo record.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This operation performs entity resolution.  Calling this method
        /// has the potential to create more redo records in certain situations.
        /// </para>
        /// 
        /// <para>
        /// This method is used in conjunction with <see cref="GetRedoRecord"/>
        /// and <see cref="CountRedoRecords"/>.
        /// </para>
        ///   
        /// <para>
        /// The optionally specified bitwise-OR'd <see cref="SzFlag"/> values not
        /// only controls how the operation is performed, but also the content of
        /// the response.  Any <see cref="SzFlag"/> value may be included, but only
        /// flags belonging to the <see cref="SzRedoFlags"/> group will be 
        /// recognized (other <see cref="SzFlag"/> values will be ignored unless
        /// they have equivalent bit flags to recognized flags).
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
        /// <include file="../target/examples/SzEngineDemo_ProcessRedos.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Info Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzEngineDemo_AddRecord.xml" path="/*"/>
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
        [SzConfigRetryable]
        string ProcessRedoRecord(string redoRecord, SzFlag? flags = SzRedoDefaultFlags);

        /// <summary>
        /// Retrieves and removes a pending redo record.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// A <c>null</c> value will be returned if there are no pending
        /// redo records.  Use <see cref="ProcessRedoRecord"/> to process
        /// the result of this method.  Once Once a redo record is 
        /// retrieved, it is no longer tracked by Senzing.  The redo 
        /// record may be stored externally for later processing.
        /// </para>
        /// 
        /// <para>
        /// This method is used in conjunction with <see 
        /// cref="ProcessRedoRecord"/> and <see cref="CountRedoRecords"/>.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <b>Usage:</b>
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
        [SzConfigRetryable]
        string GetRedoRecord();

        /// <summary>
        /// Gets the number of redo records pending processing.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// <b>WARNING:</b> When there is a large number of redo records, this
        /// is an expensive call.  Hint: If processing redo records, use result
        /// of <see cref="GetRedoRecord"/> to manage looping.
        /// </para>
        /// 
        /// <para>
        /// This method is used in conjunction with <see cref="GetRedoRecord"/>
        /// and <see cref="ProcessRedoRecord"/>.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// <b>Usage:</b>
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