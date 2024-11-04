using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Senzing.Sdk.Core {
/// <summary>
/// Provides an implementation of <see cref="NativeEngine"/>
/// that will call the native equivalent <c>extern</c> functions.
/// </summary>
internal class NativeFlags {
    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include "resolved" relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_MULTI_RECORD_ENTITIES = (1L << 0);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include "possibly same" relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_POSSIBLY_SAME = (1L << 1);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include "possibly related" relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_POSSIBLY_RELATED = (1L << 2);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include "name only" relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_NAME_ONLY = (1L << 3);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include "disclosed" relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_DISCLOSED = (1L << 4);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include singleton entities.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_SINGLE_RECORD_ENTITIES = (1L << 5);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include all entities.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_ALL_ENTITIES
      = (SZ_EXPORT_INCLUDE_MULTI_RECORD_ENTITIES 
        | SZ_EXPORT_INCLUDE_SINGLE_RECORD_ENTITIES);

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// we should include all relationships.
    /// </summary>
    internal const long SZ_EXPORT_INCLUDE_ALL_HAVING_RELATIONSHIPS
      = (SZ_EXPORT_INCLUDE_POSSIBLY_SAME
         | SZ_EXPORT_INCLUDE_POSSIBLY_RELATED
         | SZ_EXPORT_INCLUDE_NAME_ONLY 
         | SZ_EXPORT_INCLUDE_DISCLOSED);

    /// <summary>
    /// The bitwise flag for including possibly-same relations for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_POSSIBLY_SAME_RELATIONS = (1L << 6);

    /// <summary>
    /// The bitwise flag for including possibly-related relations for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_POSSIBLY_RELATED_RELATIONS = (1L << 7);

    /// <summary>
    /// The bitwise flag for including name-only relations for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_NAME_ONLY_RELATIONS = (1L << 8);

    /// <summary>
    /// The bitwise flag for including disclosed relations for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_DISCLOSED_RELATIONS = (1L << 9);

    /// <summary>
    /// The bitwise flag for including all relations for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_ALL_RELATIONS
      = (SZ_ENTITY_INCLUDE_POSSIBLY_SAME_RELATIONS
         | SZ_ENTITY_INCLUDE_POSSIBLY_RELATED_RELATIONS
         | SZ_ENTITY_INCLUDE_NAME_ONLY_RELATIONS
         | SZ_ENTITY_INCLUDE_DISCLOSED_RELATIONS);

    /// <summary>
    /// The bitwise flag for including all features for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_ALL_FEATURES = (1L << 10);

    /// <summary>
    /// The bitwise flag for including representative features for entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_REPRESENTATIVE_FEATURES = (1L << 11);

    /// <summary>
    /// The bitwise flag for including the name of the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_ENTITY_NAME = (1L << 12);

    /// <summary>
    /// The bitwise flag for including the record summary of the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_SUMMARY = (1L << 13);

    /// <summary>
    /// The bitwise flag for including the record types of the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_TYPES = (1L << 28);

    /// <summary>
    /// The bitwise flag for including the basic record data for the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_DATA = (1L << 14);

    /// <summary>
    /// The bitwise flag for including the record matching info for the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_MATCHING_INFO = (1L << 15);

    /// <summary>
    /// The bitwise flag for including the record json data for the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_JSON_DATA = (1L << 16);

    /// <summary>
    /// The bitwise flag for including the record unmapped data for the entity.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_UNMAPPED_DATA = (1L << 31);

    /// <summary>
    /// The bitwise flag for the features identifiers for the records.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_FEATURES = (1L << 18);

    /// <summary>
    /// The bitwise flag for the features identifiers for the records.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_FEATURE_DETAILS = (1L << 35);

    /// <summary>
    /// The bitwise flag for the features identifiers for the records.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RECORD_FEATURE_STATS = (1L << 36);

    /// <summary>
    /// The bitwise flag for including the name of the related entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RELATED_ENTITY_NAME = (1L << 19);

    /// <summary>
    /// The bitwise flag for including the record matching info of the related.
    /// entities
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RELATED_MATCHING_INFO = (1L << 20);

    /// <summary>
    /// The bitwise flag for including the record summary of the related entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RELATED_RECORD_SUMMARY = (1L << 21);

    /// <summary>
    /// The bitwise flag for including the record types of the related entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RELATED_RECORD_TYPES = (1L << 29);

    /// <summary>
    /// The bitwise flag for including the basic record data of the related.
    /// entities.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_RELATED_RECORD_DATA = (1L << 22);

    /// <summary>
    /// The bitwise flag for including internal features in entity output.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_INTERNAL_FEATURES = (1L << 23);

    /// <summary>
    /// The bitwise flag for including feature statistics in entity output.
    /// </summary>
    internal const long SZ_ENTITY_INCLUDE_FEATURE_STATS = (1L << 24);

    /// <summary>
    /// The bitwise flag for including internal features.
    /// </summary>
    internal const long SZ_INCLUDE_MATCH_KEY_DETAILS = (1L << 34);

    /// <summary>
    /// The bitwise flag for find-path functionality to indicate that
    /// avoided entities are strictly forbidden.
    /// </summary>
    internal const long SZ_FIND_PATH_STRICT_AVOID = (1L << 25);

    /// <summary>
    /// The bitwise flag for find-path functionality to include
    /// matching info on entity paths.
    /// </summary>
    internal const long SZ_FIND_PATH_INCLUDE_MATCHING_INFO = (1L << 30);

    /// <summary>
    /// The bitwise flag for find-path functionality to include
    /// matching info on entity paths.
    /// </summary>
    internal const long SZ_FIND_NETWORK_INCLUDE_MATCHING_INFO = (1L << 33);

    /// <summary>
    /// The bitwise flag for including feature scores.
    /// </summary>
    internal const long SZ_INCLUDE_FEATURE_SCORES = (1L << 26);

    /// <summary>
    /// The bitwise flag for including statistics from search results.
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_STATS = (1L << 27);

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// we should include "resolved" match level results.
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_RESOLVED = (SZ_EXPORT_INCLUDE_MULTI_RECORD_ENTITIES);

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// we should include "possibly same" match level results.
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_POSSIBLY_SAME
      = (SZ_EXPORT_INCLUDE_POSSIBLY_SAME);

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// we should include "possibly related" match level results.
    ///
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_POSSIBLY_RELATED
      = (SZ_EXPORT_INCLUDE_POSSIBLY_RELATED);

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// we should include "name only" match level results.
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_NAME_ONLY = (SZ_EXPORT_INCLUDE_NAME_ONLY);

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// we should include all match level results.
    /// </summary>
    internal const long SZ_SEARCH_INCLUDE_ALL_ENTITIES
      = (SZ_SEARCH_INCLUDE_RESOLVED 
         | SZ_SEARCH_INCLUDE_POSSIBLY_SAME
         | SZ_SEARCH_INCLUDE_POSSIBLY_RELATED
         | SZ_SEARCH_INCLUDE_NAME_ONLY);

    /// <summary>
    /// The default recommended bitwise flag values for getting records.
    /// </summary>
    internal const long SZ_RECORD_DEFAULT_FLAGS = (SZ_ENTITY_INCLUDE_RECORD_JSON_DATA);

    /// <summary>
    /// The default recommended bitwise flag values for getting entities.
    /// </summary>
    internal const long SZ_ENTITY_DEFAULT_FLAGS
      = (SZ_ENTITY_INCLUDE_ALL_RELATIONS
         | SZ_ENTITY_INCLUDE_REPRESENTATIVE_FEATURES
         | SZ_ENTITY_INCLUDE_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RECORD_SUMMARY
         | SZ_ENTITY_INCLUDE_RECORD_DATA
         | SZ_ENTITY_INCLUDE_RECORD_MATCHING_INFO
         | SZ_ENTITY_INCLUDE_RELATED_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RELATED_RECORD_SUMMARY
         | SZ_ENTITY_INCLUDE_RELATED_MATCHING_INFO);

    /// <summary>
    /// The default recommended bitwise flag values for getting entities.
    /// </summary>
    internal const long SZ_ENTITY_BRIEF_DEFAULT_FLAGS
      = (SZ_ENTITY_INCLUDE_RECORD_MATCHING_INFO
         | SZ_ENTITY_INCLUDE_ALL_RELATIONS
         | SZ_ENTITY_INCLUDE_RELATED_MATCHING_INFO);

    /// <summary>
    /// The default recommended bitwise flag values for exporting entities.
    /// </summary>
    internal const long SZ_EXPORT_DEFAULT_FLAGS
      = (SZ_EXPORT_INCLUDE_ALL_ENTITIES
         | SZ_ENTITY_DEFAULT_FLAGS);

    /// <summary>
    /// The default recommended bitwise flag values for finding entity paths.
    /// </summary>
    internal const long SZ_FIND_PATH_DEFAULT_FLAGS
      = (SZ_FIND_PATH_INCLUDE_MATCHING_INFO
         | SZ_ENTITY_INCLUDE_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RECORD_SUMMARY);

    /// <summary>
    /// The default recommended bitwise flag values for finding entity networks.
    /// </summary>
    internal const long SZ_FIND_NETWORK_DEFAULT_FLAGS
      = (SZ_FIND_NETWORK_INCLUDE_MATCHING_INFO
         | SZ_ENTITY_INCLUDE_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RECORD_SUMMARY);

    /// <summary>
    /// The default recommended bitwise flag values for why-entities analysis 
    /// on entities.
    /// </summary>
    internal const long SZ_WHY_ENTITIES_DEFAULT_FLAGS
      = (SZ_ENTITY_DEFAULT_FLAGS
         | SZ_ENTITY_INCLUDE_INTERNAL_FEATURES
         | SZ_ENTITY_INCLUDE_FEATURE_STATS
         | SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for why-records analysis 
    /// on entities.
    /// </summary>
    internal const long SZ_WHY_RECORDS_DEFAULT_FLAGS
      = (SZ_ENTITY_DEFAULT_FLAGS
         | SZ_ENTITY_INCLUDE_INTERNAL_FEATURES
         | SZ_ENTITY_INCLUDE_FEATURE_STATS
         | SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for why-record-in analysis
    /// on entities.
    /// </summary>
    internal const long SZ_WHY_RECORD_IN_ENTITY_DEFAULT_FLAGS
      = (SZ_ENTITY_DEFAULT_FLAGS
         | SZ_ENTITY_INCLUDE_INTERNAL_FEATURES
         | SZ_ENTITY_INCLUDE_FEATURE_STATS
         | SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for how-analysis on entities.
    /// </summary>
    internal const long SZ_HOW_ENTITY_DEFAULT_FLAGS
      = (SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for virtual-entity-analysis
    /// on entities.
    /// </summary>
    internal const long SZ_VIRTUAL_ENTITY_DEFAULT_FLAGS
      = (SZ_ENTITY_DEFAULT_FLAGS);

    /// <summary>
    /// The default recommended bitwise flag values for searching by attributes,
    /// returning all matching entities.
    /// </summary>
    internal const long SZ_SEARCH_BY_ATTRIBUTES_ALL
      = (SZ_SEARCH_INCLUDE_ALL_ENTITIES
         | SZ_ENTITY_INCLUDE_REPRESENTATIVE_FEATURES
         | SZ_ENTITY_INCLUDE_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RECORD_SUMMARY
         | SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for searching by attributes,
    /// returning only strongly matching entities.
    /// </summary>
    internal const long SZ_SEARCH_BY_ATTRIBUTES_STRONG
      = (SZ_SEARCH_INCLUDE_RESOLVED
         | SZ_SEARCH_INCLUDE_POSSIBLY_SAME
         | SZ_ENTITY_INCLUDE_REPRESENTATIVE_FEATURES
         | SZ_ENTITY_INCLUDE_ENTITY_NAME
         | SZ_ENTITY_INCLUDE_RECORD_SUMMARY
         | SZ_INCLUDE_FEATURE_SCORES);

    /// <summary>
    /// The default recommended bitwise flag values for searching by attributes,
    /// returning minimal data with all matches.
    /// </summary>
    internal const long SZ_SEARCH_BY_ATTRIBUTES_MINIMAL_ALL = (SZ_SEARCH_INCLUDE_ALL_ENTITIES);

    /// <summary>
    /// The default recommended bitwise flag values for searching by attributes,
    /// returning the minimal data, and returning only the strongest matches.
    /// </summary>
    internal const long SZ_SEARCH_BY_ATTRIBUTES_MINIMAL_STRONG
      = (SZ_SEARCH_INCLUDE_RESOLVED | SZ_SEARCH_INCLUDE_POSSIBLY_SAME);

    /// <summary>
    /// The default recommended bitwise flag values for searching by attributes.
    /// </summary>
    internal const long SZ_SEARCH_BY_ATTRIBUTES_DEFAULT_FLAGS = (SZ_SEARCH_BY_ATTRIBUTES_ALL);
}
}
