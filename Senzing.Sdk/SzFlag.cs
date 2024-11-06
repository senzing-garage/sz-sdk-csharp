using System;

namespace Senzing.Sdk {
/// <summary>
/// Enumerates the Senzing flag values so they can be referred to as 
/// bitwise enumerated flags.
/// </summary>
/// <remarks>
/// Each <c>SzFlag</c> belongs to one or more <see cref="SzFlagUsageGroup"/>
/// instances which can be obtained via the <see cref="SzFlags.GetGroups"/>
/// extension method.  This helps in identifying which flags are applicable
/// to which functions since a function will document which 
/// <see cref="SzFlagUsageGroup"/> to refer to for applicable flags.
/// <p>
/// Passing an <c>SzFlag</c> to a function to which it does not apply will
/// either have no effect or activate an applicable <see cref="SzFlag"/>
/// that happens to have the same bitwise value as the non-applicable
/// <see cref="SzFlag"/>
/// </remarks>
/// 
/// <seealso href="https://docs.senzing.com/flags/index.html"/>
[Flags]
public enum SzFlag : long {
    /// <summary>
    /// The bitwise flag that indicates that the Senzing engine should produce
    /// and return the INFO document describing the affected entities from an
    /// operation.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzModifyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    SzWithInfo = (1L << 62),

    /// <summary>
    /// The bitwise flag for export functionality to indicate "resolved"
    /// relationships (i.e.: entities with multiple records) should be
    /// included in the export.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludeMultiRecordEntities = (1L << 0),

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// "possibly same" relationships should be included in the export.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludePossiblySame = (1L << 1),

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// "possibly related" relationships should be included in the export.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludePossiblyRelated = (1L << 2),

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// "name only" relationships should be included in the export.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludeNameOnly = (1L << 3),

    /// <summary>
    /// The bitwise flag for export functionality to indicate that we
    /// should include "disclosed" relationships.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludeDisclosed = (1L << 4),

    /// <summary>
    /// The bitwise flag for export functionality to indicate that
    /// single-record entities should be included in the export.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzExportIncludeSingleRecordEntities = (1L << 5),
    
    /// <summary>
    /// The bitwise flag for including possibly-same relations for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludePossiblySameRelations = (1L << 6),

    /// <summary>
    /// The bitwise flag for including possibly-related relations for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludePossiblyRelatedRelations = (1L << 7),

    /// <summary>
    /// The bitwise flag for including name-only relations for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeNameOnlyRelations = (1L << 8),

    /// <summary>
    /// The bitwise flag for including disclosed relations for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeDisclosedRelations = (1L << 9),

    /// <summary>
    /// The bitwise flag for including all features for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeAllFeatures = (1L << 10),
    
    /// <summary>
    /// The bitwise flag for including representative features for entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRepresentativeFeatures = (1L << 11),
    
    /// <summary>
    /// The bitwise flag for including the name of the entity.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeEntityName = (1L << 12),
    
    /// <summary>
    /// The bitwise flag for including the record summary of the entity.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordSummary = (1L << 13),
    
    /// <summary>
    /// The bitwise flag for including the record types of the entity or record.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordTypes = (1L << 28),

    /// <summary>
    /// The bitwise flag for including the basic record data for
    /// the entity.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordData = (1L << 14),
    
    /// <summary>
    /// The bitwise flag for including the record matching info for
    /// the entity or record.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordMatchingInfo = (1L << 15),

    /// <summary>
    /// The bitwise flag for including the record json data for the
    /// entity or record.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordJsonData = (1L << 16),

    /// <summary>
    /// The bitwise flag for including the record unmapped data for
    /// the entity or record.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordUnmappedData = (1L << 31),

    /// <summary>
    /// The bitwise flag to include the features identifiers at the
    /// record level, referencing the entity features.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordFeatures = (1L << 18),

    /// <summary>
    /// The bitwise flag for including full feature details at the
    /// record level of an entity response or in a record response.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordFeatureDetails = (1L << 35),

    /// <summary>
    /// The bitwise flag for including full feature statistics at the
    /// record level of an entity response or in a record response.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRecordFeatureStats = (1L << 36),

    /// <summary>
    /// The bitwise flag for including the name of the related entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRelatedEntityName = (1L << 19),

    /// <summary>
    /// The bitwise flag for including the record matching info of the related
    /// entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRelatedMatchingInfo = (1L << 20),

    /// <summary>
    /// The bitwise flag for including the record summary of the
    /// related entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRelatedRecordSummary = (1L << 21),

    /// <summary>
    /// The bitwise flag for including the record types of the
    /// related entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRelatedRecordTypes = (1L << 29),

    /// <summary>
    /// The bitwise flag for including the basic record data of the
    /// related entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeRelatedRecordData = (1L << 22),

    /// <summary>
    /// The bitwise flag for including internal features in entity output.
    /// </summary>
    SzEntityIncludeInternalFeatures = (1L << 23),

    /// <summary>
    /// The bitwise flag for including feature statistics in entity
    /// or record output.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzEntityIncludeFeatureStats = (1L << 24),

    /// <summary>
    /// The bitwise flag for including match key details in addition to the 
    /// standard match key.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzHowFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzIncludeMatchKeyDetails = (1L << 34),

    /// <summary>
    /// The bitwise flag for find-path functionality to indicate
    /// that avoided entities are not allowed under any
    /// circumstance -- even if they are the only means by which
    /// a path can be found between two entities.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzFindPathStrictAvoid = (1L << 25),

    /// <summary>
    /// The bitwise flag for find-path functionality to include
    /// matching info on entity paths.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzFindPathIncludeMatchingInfo = (1L << 30),

    /// <summary>
    /// The bitwise flag for find-network functionality to include
    /// matching info on entity paths of the network.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzFindNetworkIncludeMatchingInfo = (1L << 33),

    /// <summary>
    /// The bitwise flag for including feature scores.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzWhyFlags"/></description>
    ///      <description><see cref="SzFlagUsageGroup.SzHowFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzIncludeFeatureScores = (1L << 26),
    
    /// <summary>
    /// The bitwise flag for including statistics from search results.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzSearchIncludeStats = (1L << 27),

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// "resolved" match level results should be included.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzSearchIncludeResolved = (SzExportIncludeMultiRecordEntities),

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// "possibly same" match level results should be included.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzSearchIncludePossiblySame = (SzExportIncludePossiblySame),

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// "possibly related" match level results should be included.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzSearchIncludePossiblyRelated = (SzExportIncludePossiblyRelated),

    /// <summary>
    /// The bitwise flag for search functionality to indicate that
    /// "name only" match level results should be included.
    /// </summary>
    /// <remarks>
    /// This flag belongs to the following usage groups:
    /// <list>
    ///    <item>
    ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://docs.senzing.com/flags/index.html"/>
    SzSearchIncludeNameOnly = (SzExportIncludeNameOnly)
}
}