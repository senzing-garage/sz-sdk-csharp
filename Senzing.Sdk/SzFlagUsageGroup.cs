namespace Senzing.Sdk {
/// <summary>
/// Enumerates the various classifications of usage groups for the
/// <see cref="SzFlag"/> instances.
/// </summary>
public enum SzFlagUsageGroup {
    /// <summary>
    /// Flags in this usage group can be used for operations that modify the
    /// entity repository by adding records, revaluating records or entities,
    /// deleting records or any similar operations.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzWITH_INFO"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzModifyFlags,

    /// <summary>
    /// Flags in this usage group can be used for operations that retrieve record
    /// data in order to control the level of detail of the returned record.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzRecordFlags,

    /// <summary>
    /// Flags in this usage group can be used for operations that retrieve
    /// entity data in order to control the level of detail of the returned
    /// entity.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzEntityFlags,

    /// <summary>
    /// Flags in this usage group can be used to control the methodology for
    /// finding an entity path, what details to include for the entity 
    /// path and the level of detail for the entities on the path that
    /// are returned.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzFindPathStrictAvoid"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzFindPathIncludeMatchingInfo"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "find-path" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzFindPathDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzFindPathFlags,

    /// <summary>
    /// Flags in this usage group can be used to control the methodology for
    /// finding an entity network, what details to include for the entity 
    /// network and the level of detail for the entities in the network
    /// that are returned.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzFindNetworkIncludeMatchingInfo"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "find-path" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzFindNetworkDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzFindNetworkFlags,

    /// <summary>
    /// Flags in this usage group can be used for operations that search for 
    /// entities to control how the entities are qualified for inclusion
    /// in the search results and the level of detail for the entities
    /// returned in the search results.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzSearchIncludeStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzSearchIncludeResolved"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzSearchIncludePossiblySame"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzSearchIncludePossiblyRelated"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzSearchIncludeNameOnly"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "search" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchIncludeAllEntities"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchByAttributesAll"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchByAttributesStrong"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchByAttributesMinimalAll"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchByAttributesMinimalStrong"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzSearchByAttributesDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzSearchFlags,

    /// <summary>
    /// Flags in this usage group can be used for operations that export 
    /// entities to control how the entities are qualified for inclusion
    /// in the export and the level of detail for the entities returned
    /// in the search results.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludeMultiRecordEntities"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludePossiblySame"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludePossiblyRelated"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludeNameOnly"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludeDisclosed"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzExportIncludeSingleRecordEntities"/></description>
    ///   </item>
    /// </list>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "export" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzExportIncludeAllEntities"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzExportIncludeAllHavingRelationships"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzExportDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzExportFlags,

    /// <summary>
    /// Flags in this usage group can be used to control the methodology for
    /// performing "why analysis", what details to include for the analysis
    /// and the level of detail for the entities in the network that are 
    /// returned.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblySameRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludePossiblyRelatedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeNameOnlyRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeDisclosedRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRelatedRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "why" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzWHY_Entities_DefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzWHY_RECORDS_DefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzWHY_RecordIN_EntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzWhyFlags,

    /// <summary>
    /// Flags in this usage group can be used to control the methodology for
    /// performing "how analysis", what details to include for the analysis
    /// and the level of detail for the entities in the network that are 
    /// returned.
    /// Flags in this usage group can be used to control the methodology for
    /// performing "why analysis", what details to include for the analysis
    /// and the level of detail for the entities in the network that are 
    /// returned.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
    /// group and are defined for "how" operations are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzHowEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
    /// support this group for definining entity or record detail levels are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityIncludeAllRelations"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzEntityBriefDefaultFlags"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzHowFlags,

    /// <summary>
    /// Flags in this usage group can be used for operations that retrieve
    /// virtual entities in order to control the level of detail of the
    /// returned virtual entity.
    /// </summary>
    /// <remarks>
    /// The <see cref="SzFlag"/> instances included in this usage group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeAllFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRepresentativeFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeEntityName"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordSummary"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordTypes"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordMatchingInfo"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureDetails"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeRecordFeatureStats"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="SzFlag.SzEntityIncludeFeatureStats"/></description>
    ///   </item>
    /// </list>
    /// <p>
    /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
    /// <list>
    ///   <item>
    ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
    SzVirtualEntityFlags
}
}