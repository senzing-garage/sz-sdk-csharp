namespace Senzing.Sdk {
/// <summary>
/// Provides aggregate <see cref="SzFlag"/> constants as well as
/// extension methods and utility methods pertaining to 
/// <see cref="SzFlag"/> and <see cref="SzFlagUsageGroup"/>.
/// </summary>
public static class SzFlags {
    /// The value representing no flags are being passed.
    /// </summary>
    ///
    /// <remarks>
    /// Alternatively, a <c>null</c> value will indicate
    /// no flags as well.
    /// </remarks>
    public const SzFlag SzNoFlags = 0L;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzModifyFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzModifyFlags"/>
    public const SzFlag SzModifyAllFlags = SzFlag.SzWithInfo;
        
    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzRecordFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzRecordFlags"/>
    public const SzFlag SzRecordAllFlags
        = SzFlag.SzEntityIncludeInternalFeatures 
            | SzFlag.SzEntityIncludeRecordTypes
            | SzFlag.SzEntityIncludeRecordFeatureDetails
            | SzFlag.SzEntityIncludeRecordFeatureStats
            | SzFlag.SzEntityIncludeRecordMatchingInfo
            | SzFlag.SzEntityIncludeRecordJsonData
            | SzFlag.SzEntityIncludeRecordUnmappedData;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzEntityFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzEntityFlags"/>
    public const SzFlag SzEntityAllFlags
        = SzFlag.SzEntityIncludePossiblySameRelations
            | SzFlag.SzEntityIncludePossiblyRelatedRelations
            | SzFlag.SzEntityIncludeNameOnlyRelations
            | SzFlag.SzEntityIncludeDisclosedRelations
            | SzFlag.SzEntityIncludeAllFeatures
            | SzFlag.SzEntityIncludeRepresentativeFeatures
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary
            | SzFlag.SzEntityIncludeRecordTypes
            | SzFlag.SzEntityIncludeRecordData
            | SzFlag.SzEntityIncludeRecordMatchingInfo
            | SzFlag.SzEntityIncludeRecordJsonData
            | SzFlag.SzEntityIncludeRecordUnmappedData
            | SzFlag.SzEntityIncludeRecordFeatures
            | SzFlag.SzEntityIncludeRecordFeatureDetails
            | SzFlag.SzEntityIncludeRecordFeatureStats
            | SzFlag.SzEntityIncludeRelatedEntityName
            | SzFlag.SzEntityIncludeRelatedMatchingInfo
            | SzFlag.SzEntityIncludeRelatedRecordSummary
            | SzFlag.SzEntityIncludeRelatedRecordTypes
            | SzFlag.SzEntityIncludeRelatedRecordData
            | SzFlag.SzEntityIncludeInternalFeatures
            | SzFlag.SzEntityIncludeFeatureStats
            | SzFlag.SzIncludeMatchKeyDetails;
    
    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzFindPathFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzFindPathFlags"/>
    public const SzFlag SzFindPathAllFlags
        = SzEntityAllFlags 
            | SzFlag.SzFindPathStrictAvoid
            | SzFlag.SzFindPathIncludeMatchingInfo;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzFindNetworkFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzFindNetworkFlags"/>
    public const SzFlag SzFindNetworkAllFlags
        = SzEntityAllFlags | SzFlag.SzFindNetworkIncludeMatchingInfo;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzSearchFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzSearchFlags"/>
    public const SzFlag SzSearchAllFlags
        = SzEntityAllFlags
            | SzFlag.SzSearchIncludeStats
            | SzFlag.SzSearchIncludeResolved
            | SzFlag.SzSearchIncludePossiblySame
            | SzFlag.SzSearchIncludePossiblyRelated
            | SzFlag.SzSearchIncludeNameOnly;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzExportFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzExportFlags"/>
    public const SzFlag SzExportAllFlags
        = SzEntityAllFlags
            | SzFlag.SzExportIncludeMultiRecordEntities
            | SzFlag.SzExportIncludePossiblySame
            | SzFlag.SzExportIncludePossiblyRelated
            | SzFlag.SzExportIncludeNameOnly
            | SzFlag.SzExportIncludeDisclosed
            | SzFlag.SzExportIncludeSingleRecordEntities;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzWhyFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzWhyFlags"/>
    public const SzFlag SzWhyFlags
        = SzEntityAllFlags | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzHowFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzHowFlags"/>
    public const SzFlag SzHowFlags
        = SzFlag.SzIncludeMatchKeyDetails | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzVirtualEntityFlags"/>
    public const SzFlag SzVirtualEntityAllFlags
        = SzFlag.SzEntityIncludeAllFeatures
              | SzFlag.SzEntityIncludeAllFeatures
              | SzFlag.SzEntityIncludeRepresentativeFeatures
              | SzFlag.SzEntityIncludeEntityName
              | SzFlag.SzEntityIncludeRecordSummary
              | SzFlag.SzEntityIncludeRecordTypes
              | SzFlag.SzEntityIncludeRecordData
              | SzFlag.SzEntityIncludeRecordMatchingInfo
              | SzFlag.SzEntityIncludeRecordJsonData
              | SzFlag.SzEntityIncludeRecordUnmappedData
              | SzFlag.SzEntityIncludeRecordFeatures
              | SzFlag.SzEntityIncludeRecordFeatureDetails
              | SzFlag.SzEntityIncludeRecordFeatureStats
              | SzFlag.SzEntityIncludeInternalFeatures
              | SzFlag.SzEntityIncludeFeatureStats;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> for export functionality to indicate
    /// that all entities should be included in an export.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludeMultiRecordEntities"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludeSingleRecordEntities"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzExportIncludeAllEntities 
        = SzFlag.SzExportIncludeMultiRecordEntities 
            | SzFlag.SzExportIncludeSingleRecordEntities;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> for export functionality to indicate that
    /// entities having relationships of any kind should be included in an export.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludePossiblySame"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludePossiblyRelated"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludeNameOnly"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzExportIncludeDisclosed"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzExportIncludeAllHavingRelationships 
        = SzFlag.SzExportIncludePossiblySame
            | SzFlag.SzExportIncludePossiblyRelated
            | SzFlag.SzExportIncludeNameOnly 
            | SzFlag.SzExportIncludeDisclosed;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> for including all relations for entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the 
    /// <see cref="SzFlagUsageGroup.SzEntityFlags"/> usage group, and by extension 
    /// belong to the following usage groups which are super sets:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzEntityIncludeAllRelations 
        = SzFlag.SzEntityIncludePossiblySameRelations
            | SzFlag.SzEntityIncludePossiblyRelatedRelations
            | SzFlag.SzEntityIncludeNameOnlyRelations
            | SzFlag.SzEntityIncludeDisclosedRelations;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> for search functionality to indicate that
    /// search results should include entities that are related to the search 
    /// attributes at any match level.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludeResolved"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludePossiblySame"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludePossiblyRelated"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludeNameOnly"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzSearchIncludeAllEntities
        = SzFlag.SzSearchIncludeResolved 
            | SzFlag.SzSearchIncludePossiblySame
            | SzFlag.SzSearchIncludePossiblyRelated
            | SzFlag.SzSearchIncludeNameOnly;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> to produce the default level
    /// of detail when retrieving records.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the 
    /// <see cref="SzFlagUsageGroup.SzRecordFlags"/> usage group, and by extension 
    /// belong to the following usage groups which are super sets:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzRecordDefaultFlags
        = (SzFlag.SzEntityIncludeRecordJsonData);

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> to produce the default level of
    /// detail when retrieving entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the 
    /// <see cref="SzFlagUsageGroup.SzEntityFlags"/> usage group, and by extension 
    /// belong to the following usage groups which are super sets:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzEntityDefaultFlags 
        = SzEntityIncludeAllRelations
            | SzFlag.SzEntityIncludeRepresentativeFeatures
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary
            | SzFlag.SzEntityIncludeRecordData
            | SzFlag.SzEntityIncludeRecordMatchingInfo
            | SzFlag.SzEntityIncludeRelatedEntityName
            | SzFlag.SzEntityIncludeRelatedRecordSummary
            | SzFlag.SzEntityIncludeRelatedMatchingInfo;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> to produce the default level of
    /// detail when retrieving entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the 
    /// <see cref="SzFlagUsageGroup.SzEntityFlags"/> usage group, and by extension 
    /// belong to the following usage groups which are super sets:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzEntityBriefDefaultFlags 
        = SzEntityIncludeAllRelations
            | SzFlag.SzEntityIncludeRecordMatchingInfo
            | SzFlag.SzEntityIncludeRelatedMatchingInfo;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for exporting entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzExportIncludeAllEntities"/></description>
    ///   </item>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzExportDefaultFlags 
        = (SzExportIncludeAllEntities | SzEntityDefaultFlags);

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for finding entity paths.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzFindPathIncludeMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzFindPathDefaultFlags 
        = (SzFlag.SzFindPathIncludeMatchingInfo
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary);

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for finding entity networks.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzFindNetworkIncludeMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzFindNetworkDefaultFlags 
        = (SzFlag.SzFindNetworkIncludeMatchingInfo
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary);

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings for
    /// "why entity" operations.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzWhyEntitiesDefaultFlags 
        = SzEntityDefaultFlags
            | SzFlag.SzEntityIncludeInternalFeatures
            | SzFlag.SzEntityIncludeFeatureStats
            | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for "why records" operations.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzWhyRecordsDefaultFlags 
        = SzEntityDefaultFlags
            | SzFlag.SzEntityIncludeInternalFeatures
            | SzFlag.SzEntityIncludeFeatureStats
            | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for "why record in entity" operations.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzWhyRecordInEntityDefaultFlags 
        = SzEntityDefaultFlags
            | SzFlag.SzEntityIncludeInternalFeatures
            | SzFlag.SzEntityIncludeFeatureStats
            | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// for "how entity" operations.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzHowFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzHowEntityDefaultFlags 
        = (SzFlag.SzIncludeFeatureScores);

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default settings
    /// when retrieving virtual entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzVirtualEntityDefaultFlags 
        = SzFlag.SzEntityIncludeRepresentativeFeatures
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary
            | SzFlag.SzEntityIncludeRecordData
            | SzFlag.SzEntityIncludeRecordMatchingInfo;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> indicating that search results
    /// should include all matching entities regardless of match level.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzSearchIncludeAllEntities"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzSearchByAttributesAll 
        = SzSearchIncludeAllEntities
            | SzFlag.SzEntityIncludeRepresentativeFeatures
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary
            | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> indicating that search results
    /// should only include strongly matching entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludeResolved"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludePossiblySame"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzSearchByAttributesStrong 
        = SzFlag.SzSearchIncludeResolved
            | SzFlag.SzSearchIncludePossiblySame
            | SzFlag.SzEntityIncludeRepresentativeFeatures
            | SzFlag.SzEntityIncludeEntityName
            | SzFlag.SzEntityIncludeRecordSummary
            | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> indicating that search results
    /// should include all matching entities regardless of match level
    /// while returning minimal data for those entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzSearchIncludeAllEntities"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzSearchByAttributesMinimalAll = SzSearchIncludeAllEntities;

    /// <summary>
    /// The aggregate <see cref="SzFlag"/> indicating that search results
    /// should only include strongly matching entities while returning 
    /// miniaml data for those matching entities.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludeResolved"/></description>
    ///   </item>
    ///   <item>
    ///      <description><see cref="SzFlag.SzSearchIncludePossiblySame"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    public const SzFlag SzSearchByAttributesMinimalStrong 
        = SzFlag.SzSearchIncludeResolved | SzFlag.SzSearchIncludePossiblySame;
        
    /// <summary>
    /// The aggregate <see cref="SzFlag"/> representing the default flags for
    /// "search by attribute" operations.
    /// </summary>
    /// <remarks>
    /// The included <see cref="SzFlag"/> values are:
    /// <list>
    ///   <item>
    ///      <description>All flags from <see cref="SzSearchByAttributesAll"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// All the flags in this constant belong to the following usage groups:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
   public const SzFlag SzSearchByAttributesDefaultFlags = SzSearchByAttributesAll;
}
}