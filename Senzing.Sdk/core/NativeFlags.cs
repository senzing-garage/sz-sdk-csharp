namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides an implementation of <see cref="NativeEngine"/>
    /// that will call the native equivalent <c>extern</c> functions.
    /// </summary>
    internal static class NativeFlags
    {
        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include "resolved" relationships.
        /// </summary>
        public const long SzExportIncludeMultiRecordEntities = (1L << 0);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include "possibly same" relationships.
        /// </summary>
        public const long SzExportIncludePossiblySame = (1L << 1);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include "possibly related" relationships.
        /// </summary>
        public const long SzExportIncludePossiblyRelated = (1L << 2);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include "name only" relationships.
        /// </summary>
        public const long SzExportIncludeNameOnly = (1L << 3);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include "disclosed" relationships.
        /// </summary>
        public const long SzExportIncludeDisclosed = (1L << 4);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include singleton entities.
        /// </summary>
        public const long SzExportIncludeSingleRecordEntities = (1L << 5);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include all entities.
        /// </summary>
        public const long SzExportIncludeAllEntities
          = (SzExportIncludeMultiRecordEntities
            | SzExportIncludeSingleRecordEntities);

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// we should include all relationships.
        /// </summary>
        public const long SzExportIncludeAllHavingRelationships
          = (SzExportIncludePossiblySame
             | SzExportIncludePossiblyRelated
             | SzExportIncludeNameOnly
             | SzExportIncludeDisclosed);

        /// <summary>
        /// The bitwise flag for including possibly-same relations for entities.
        /// </summary>
        public const long SzEntityIncludePossiblySameRelations = (1L << 6);

        /// <summary>
        /// The bitwise flag for including possibly-related relations for entities.
        /// </summary>
        public const long SzEntityIncludePossiblyRelatedRelations = (1L << 7);

        /// <summary>
        /// The bitwise flag for including name-only relations for entities.
        /// </summary>
        public const long SzEntityIncludeNameOnlyRelations = (1L << 8);

        /// <summary>
        /// The bitwise flag for including disclosed relations for entities.
        /// </summary>
        public const long SzEntityIncludeDisclosedRelations = (1L << 9);

        /// <summary>
        /// The bitwise flag for including all relations for entities.
        /// </summary>
        public const long SzEntityIncludeAllRelations
          = (SzEntityIncludePossiblySameRelations
             | SzEntityIncludePossiblyRelatedRelations
             | SzEntityIncludeNameOnlyRelations
             | SzEntityIncludeDisclosedRelations);

        /// <summary>
        /// The bitwise flag for including all features for entities.
        /// </summary>
        public const long SzEntityIncludeAllFeatures = (1L << 10);

        /// <summary>
        /// The bitwise flag for including representative features for entities.
        /// </summary>
        public const long SzEntityIncludeRepresentativeFeatures = (1L << 11);

        /// <summary>
        /// The bitwise flag for including the name of the entity.
        /// </summary>
        public const long SzEntityIncludeEntityName = (1L << 12);

        /// <summary>
        /// The bitwise flag for including the record summary of the entity.
        /// </summary>
        public const long SzEntityIncludeRecordSummary = (1L << 13);

        /// <summary>
        /// The bitwise flag for including the record types of the entity.
        /// </summary>
        public const long SzEntityIncludeRecordTypes = (1L << 28);

        /// <summary>
        /// The bitwise flag for including the basic record data for the entity.
        /// </summary>
        public const long SzEntityIncludeRecordData = (1L << 14);

        /// <summary>
        /// The bitwise flag for including the record matching info for the entity.
        /// </summary>
        public const long SzEntityIncludeRecordMatchingInfo = (1L << 15);

        /// <summary>
        /// The bitwise flag for including the record json data for the entity.
        /// </summary>
        public const long SzEntityIncludeRecordJsonData = (1L << 16);

        /// <summary>
        /// The bitwise flag for including the record unmapped data for the entity.
        /// </summary>
        public const long SzEntityIncludeRecordUnmappedData = (1L << 31);

        /// <summary>
        /// The bitwise flag for the features identifiers for the records.
        /// </summary>
        public const long SzEntityIncludeRecordFeatures = (1L << 18);

        /// <summary>
        /// The bitwise flag for the features identifiers for the records.
        /// </summary>
        public const long SzEntityIncludeRecordFeatureDetails = (1L << 35);

        /// <summary>
        /// The bitwise flag for the features identifiers for the records.
        /// </summary>
        public const long SzEntityIncludeRecordFeatureStats = (1L << 36);

        /// <summary>
        /// The bitwise flag for including the name of the related entities.
        /// </summary>
        public const long SzEntityIncludeRelatedEntityName = (1L << 19);

        /// <summary>
        /// The bitwise flag for including the record matching info of the related.
        /// entities
        /// </summary>
        public const long SzEntityIncludeRelatedMatchingInfo = (1L << 20);

        /// <summary>
        /// The bitwise flag for including the record summary of the related entities.
        /// </summary>
        public const long SzEntityIncludeRelatedRecordSummary = (1L << 21);

        /// <summary>
        /// The bitwise flag for including the record types of the related entities.
        /// </summary>
        public const long SzEntityIncludeRelatedRecordTypes = (1L << 29);

        /// <summary>
        /// The bitwise flag for including the basic record data of the related.
        /// entities.
        /// </summary>
        public const long SzEntityIncludeRelatedRecordData = (1L << 22);

        /// <summary>
        /// The bitwise flag for including internal features in entity output.
        /// </summary>
        public const long SzEntityIncludeInternalFeatures = (1L << 23);

        /// <summary>
        /// The bitwise flag for including feature statistics in entity output.
        /// </summary>
        public const long SzEntityIncludeFeatureStats = (1L << 24);

        /// <summary>
        /// The bitwise flag for including internal features.
        /// </summary>
        public const long SzIncludeMatchKeyDetails = (1L << 34);

        /// <summary>
        /// The bitwise flag for find-path functionality to indicate that
        /// avoided entities are strictly forbidden.
        /// </summary>
        public const long SzFindPathStrictAvoid = (1L << 25);

        /// <summary>
        /// The bitwise flag for find-path functionality to include
        /// matching info on entity paths.
        /// </summary>
        public const long SzFindPathIncludeMatchingInfo = (1L << 30);

        /// <summary>
        /// The bitwise flag for find-path functionality to include
        /// matching info on entity paths.
        /// </summary>
        public const long SzFindNetworkIncludeMatchingInfo = (1L << 33);

        /// <summary>
        /// The bitwise flag for including feature scores.
        /// </summary>
        public const long SzIncludeFeatureScores = (1L << 26);

        /// <summary>
        /// The bitwise flag for including statistics from search results.
        /// </summary>
        public const long SzSearchIncludeStats = (1L << 27);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// we should include "resolved" match level results.
        /// </summary>
        public const long SzSearchIncludeResolved
          = (SzExportIncludeMultiRecordEntities);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// we should include "possibly same" match level results.
        /// </summary>
        public const long SzSearchIncludePossiblySame
          = (SzExportIncludePossiblySame);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// we should include "possibly related" match level results.
        ///
        /// </summary>
        public const long SzSearchIncludePossiblyRelated
          = (SzExportIncludePossiblyRelated);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// we should include "name only" match level results.
        /// </summary>
        public const long SzSearchIncludeNameOnly = (SzExportIncludeNameOnly);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// we should include all match level results.
        /// </summary>
        public const long SzSearchIncludeAllEntities
          = (SzSearchIncludeResolved
             | SzSearchIncludePossiblySame
             | SzSearchIncludePossiblyRelated
             | SzSearchIncludeNameOnly);

        /// <summary>
        /// The default recommended bitwise flag values for getting records.
        /// </summary>
        public const long SzRecordDefaultFlags = (SzEntityIncludeRecordJsonData);

        /// <summary>
        /// The default recommended bitwise flag values for basic entity output.
        /// </summary>
        public const long SzEntityCoreFlags
          = (SzEntityIncludeRepresentativeFeatures
             | SzEntityIncludeEntityName
             | SzEntityIncludeRecordSummary
             | SzEntityIncludeRecordData
             | SzEntityIncludeRecordMatchingInfo);

        /// <summary>
        /// The default recommended bitwise flag values for getting entities.
        /// </summary>
        public const long SzEntityDefaultFlags
          = (SzEntityCoreFlags
             | SzEntityIncludeAllRelations
             | SzEntityIncludeRelatedEntityName
             | SzEntityIncludeRelatedRecordSummary
             | SzEntityIncludeRelatedMatchingInfo);

        /// <summary>
        /// The default recommended bitwise flag values for getting entities.
        /// </summary>
        public const long SzEntityBriefDefaultFlags
          = (SzEntityIncludeRecordMatchingInfo
             | SzEntityIncludeAllRelations
             | SzEntityIncludeRelatedMatchingInfo);

        /// <summary>
        /// The default recommended bitwise flag values for exporting entities.
        /// </summary>
        public const long SzExportDefaultFlags
          = (SzExportIncludeAllEntities
             | SzEntityDefaultFlags);

        /// <summary>
        /// The default recommended bitwise flag values for finding entity paths.
        /// </summary>
        public const long SzFindPathDefaultFlags
          = (SzFindPathIncludeMatchingInfo
             | SzEntityIncludeEntityName
             | SzEntityIncludeRecordSummary);

        /// <summary>
        /// The default recommended bitwise flag values for finding entity networks.
        /// </summary>
        public const long SzFindNetworkDefaultFlags
          = (SzFindNetworkIncludeMatchingInfo
             | SzEntityIncludeEntityName
             | SzEntityIncludeRecordSummary);

        /// <summary>
        /// The default recommended bitwise flag values for why-entities analysis 
        /// on entities.
        /// </summary>
        public const long SzWhyEntitiesDefaultFlags = SzIncludeFeatureScores;

        /// <summary>
        /// The default recommended bitwise flag values for why-records analysis 
        /// on entities.
        /// </summary>
        public const long SzWhyRecordsDefaultFlags = SzIncludeFeatureScores;

        /// <summary>
        /// The default recommended bitwise flag values for why-record-in analysis
        /// on entities.
        /// </summary>
        public const long SzWhyRecordInEntityDefaultFlags = SzIncludeFeatureScores;

        /// <summary>
        /// The default recommended bitwise flag values for how-analysis on entities.
        /// </summary>
        public const long SzHowEntityDefaultFlags
          = (SzIncludeFeatureScores);

        /// <summary>
        /// The default recommended bitwise flag values for virtual-entity-analysis
        /// on entities.
        /// </summary>
        public const long SzVirtualEntityDefaultFlags = SzEntityCoreFlags;

        /// <summary>
        /// The default recommended bitwise flag values for searching by attributes,
        /// returning all matching entities.
        /// </summary>
        public const long SzSearchByAttributesAll
          = (SzSearchIncludeAllEntities
             | SzSearchIncludeStats
             | SzEntityIncludeRepresentativeFeatures
             | SzEntityIncludeEntityName
             | SzEntityIncludeRecordSummary
             | SzIncludeFeatureScores);

        /// <summary>
        /// The default recommended bitwise flag values for searching by attributes,
        /// returning only strongly matching entities.
        /// </summary>
        public const long SzSearchByAttributesStrong
          = (SzSearchIncludeResolved
             | SzSearchIncludePossiblySame
             | SzSearchIncludeStats
             | SzEntityIncludeRepresentativeFeatures
             | SzEntityIncludeEntityName
             | SzEntityIncludeRecordSummary
             | SzIncludeFeatureScores);

        /// <summary>
        /// The default recommended bitwise flag values for searching by attributes,
        /// returning minimal data with all matches.
        /// </summary>
        public const long SzSearchByAttributesMinimalAll
          = (SzSearchIncludeAllEntities | SzSearchIncludeStats);

        /// <summary>
        /// The default recommended bitwise flag values for searching by attributes,
        /// returning the minimal data, and returning only the strongest matches.
        /// </summary>
        public const long SzSearchByAttributesMinimalStrong
          = (SzSearchIncludeResolved
             | SzSearchIncludeStats
             | SzSearchIncludePossiblySame);

        /// <summary>
        /// The default recommended bitwise flag values for searching by attributes.
        /// </summary>
        public const long SzSearchByAttributesDefaultFlags
          = (SzSearchByAttributesAll);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// search results should not only include those entities that
        /// satisfy resolution rule, but also those that present on the
        /// candidate list but fail to satisfy a resolution rule.
        /// </summary>
        public const long SzSearchIncludeAllCandidates = (1L << 32);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// the search response should include the basic feature
        /// information for the search criteria features.
        /// </summary>
        public const long SzSearchIncludeRequest = (1L << 37);

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// the search response should include detailed feature 
        /// information for search criteria features (including feature
        /// stats and generic status).
        /// </summary>
        /// 
        /// <remarks>
        /// This flag has no effect unless
        /// <see cref="SzSearchIncludeRequest"/> is also specified.
        /// </remarks>
        public const long SzSearchIncludeRequestDetails = (1L << 38);

        /// <summary>
        /// The default recommended bitwise flag values for performing 
        /// "why search" operations.
        /// </summary>
        public const long SzWhySearchDefaultFlags
          = (SzIncludeFeatureScores
             | SzSearchIncludeRequestDetails
             | SzSearchIncludeStats);
    }
}
