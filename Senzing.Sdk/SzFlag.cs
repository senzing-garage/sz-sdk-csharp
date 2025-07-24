using System;

namespace Senzing.Sdk
{
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
    /// <para>
    /// Passing an <c>SzFlag</c> to a function to which it does not apply will
    /// either have no effect or activate an applicable <see cref="SzFlag"/>
    /// that happens to have the same bitwise value as the non-applicable
    /// <see cref="SzFlag"/>
    /// </para>
    /// </remarks>
    /// 
    /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
    [Flags]
    public enum SzFlag : long
    {
        /// <summary>
        /// The bitwise flag that indicates that the Senzing engine should produce
        /// and return the INFO document describing the affected entities from an
        /// operation.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzAddRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzDeleteRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzReevaluateRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzReevaluateEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRedoFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzModifySet)]
        SzWithInfo = (1L << 62),

        /// <summary>
        /// The bitwise flag for export functionality to indicate "resolved"
        /// relationships (i.e.: entities with multiple records) should be
        /// included in the export.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludeMultiRecordEntities = (1L << 0),

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// "possibly same" relationships should be included in the export.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludePossiblySame = (1L << 1),

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// "possibly related" relationships should be included in the export.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludePossiblyRelated = (1L << 2),

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// "name only" relationships should be included in the export.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludeNameOnly = (1L << 3),

        /// <summary>
        /// The bitwise flag for export functionality to indicate that we
        /// should include "disclosed" relationships.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludeDisclosed = (1L << 4),

        /// <summary>
        /// The bitwise flag for export functionality to indicate that
        /// single-record entities should be included in the export.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzExportSet)]
        SzExportIncludeSingleRecordEntities = (1L << 5),

        /// <summary>
        /// The bitwise flag for including possibly-same relations for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludePossiblySameRelations = (1L << 6),

        /// <summary>
        /// The bitwise flag for including possibly-related relations for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludePossiblyRelatedRelations = (1L << 7),

        /// <summary>
        /// The bitwise flag for including name-only relations for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeNameOnlyRelations = (1L << 8),

        /// <summary>
        /// The bitwise flag for including disclosed relations for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeDisclosedRelations = (1L << 9),

        /// <summary>
        /// The bitwise flag for including all features for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeAllFeatures = (1L << 10),

        /// <summary>
        /// The bitwise flag for including representative features for entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeRepresentativeFeatures = (1L << 11),

        /// <summary>
        /// The bitwise flag for including the name of the entity.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeEntityName = (1L << 12),

        /// <summary>
        /// The bitwise flag for including the record summary of the entity.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeRecordSummary = (1L << 13),

        /// <summary>
        /// The bitwise flag for including the record types of the entity or record.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeRecordTypes = (1L << 28),

        /// <summary>
        /// The bitwise flag for including the basic record data for
        /// the entity.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeRecordData = (1L << 14),

        /// <summary>
        /// The bitwise flag for including the record matching info for
        /// the entity or record.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeRecordMatchingInfo = (1L << 15),

        /// <summary>
        /// The bitwise flag for including the record matching info for
        /// the entity or record.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntityRecordSet)]
        SzEntityIncludeRecordDates = (1L << 39),

        /// <summary>
        /// The bitwise flag for including the record json data for the
        /// entity or record.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeRecordJsonData = (1L << 16),

        /// <summary>
        /// The bitwise flag for including the record unmapped data for
        /// the entity or record.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeRecordUnmappedData = (1L << 31),

        /// <summary>
        /// The bitwise flag to include the features identifiers at the
        /// record level, referencing the entity features.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeRecordFeatures = (1L << 18),

        /// <summary>
        /// The bitwise flag for including full feature details at the
        /// record level of an entity response or in a record response.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeRecordFeatureDetails = (1L << 35),

        /// <summary>
        /// The bitwise flag for including full feature statistics at the
        /// record level of an entity response or in a record response.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeRecordFeatureStats = (1L << 36),

        /// <summary>
        /// The bitwise flag for including the name of the related entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeRelatedEntityName = (1L << 19),

        /// <summary>
        /// The bitwise flag for including the record matching info of the related
        /// entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeRelatedMatchingInfo = (1L << 20),

        /// <summary>
        /// The bitwise flag for including the record summary of the
        /// related entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeRelatedRecordSummary = (1L << 21),

        /// <summary>
        /// The bitwise flag for including the record types of the
        /// related entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeRelatedRecordTypes = (1L << 29),

        /// <summary>
        /// The bitwise flag for including the basic record data of the
        /// related entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRelationSet)]
        SzEntityIncludeRelatedRecordData = (1L << 22),

        /// <summary>
        /// The bitwise flag for including internal features in an entity
        /// response or record response.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordPreviewFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzRecordFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzRecordPreviewSet)]
        SzEntityIncludeInternalFeatures = (1L << 23),

        /// <summary>
        /// The bitwise flag for including feature statistics in entity responses.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzVirtualEntityFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntitySet)]
        SzEntityIncludeFeatureStats = (1L << 24),

        /// <summary>
        /// The bitwise flag for including match key details in addition to the 
        /// standard match key.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzExportFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzHowFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzEntityHowSet)]
        SzIncludeMatchKeyDetails = (1L << 34),

        /// <summary>
        /// The bitwise flag for find-path functionality to indicate
        /// that avoided entities are not allowed under any
        /// circumstance -- even if they are the only means by which
        /// a path can be found between two entities.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzFindPathSet)]
        SzFindPathStrictAvoid = (1L << 25),

        /// <summary>
        /// The bitwise flag for find-path functionality to include
        /// matching info on entity paths.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindPathFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzFindPathSet)]
        SzFindPathIncludeMatchingInfo = (1L << 30),

        /// <summary>
        /// The bitwise flag for find-network functionality to include
        /// matching info on entity paths of the network.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzFindNetworkFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzFindNetworkSet)]
        SzFindNetworkIncludeMatchingInfo = (1L << 33),

        /// <summary>
        /// The bitwise flag for including feature scores.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordInEntityFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyRecordsFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhyEntitiesFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhySearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzHowFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzHowWhySearchSet)]
        SzIncludeFeatureScores = (1L << 26),

        /// <summary>
        /// The bitwise flag for including statistics from search results.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhySearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzWhySearchSet)]
        SzSearchIncludeStats = (1L << 27),

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// "resolved" match level results should be included.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzSearchSet)]
        SzSearchIncludeResolved = SzExportIncludeMultiRecordEntities,

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// "possibly same" match level results should be included.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzSearchSet)]
        SzSearchIncludePossiblySame = SzExportIncludePossiblySame,

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// "possibly related" match level results should be included.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzSearchSet)]
        SzSearchIncludePossiblyRelated = SzExportIncludePossiblyRelated,

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// "name only" match level results should be included.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzSearchSet)]
        SzSearchIncludeNameOnly = SzExportIncludeNameOnly,

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// search results should not only include those entities that
        /// satisfy resolution rule, but also those that present on the
        /// candidate list but fail to satisfy a resolution rule.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzSearchSet)]
        SzSearchIncludeAllCandidates = (1L << 32),

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// the search response should include the basic feature
        /// information for the search criteria features.
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhySearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzWhySearchSet)]
        SzSearchIncludeRequest = (1L << 37),

        /// <summary>
        /// The bitwise flag for search functionality to indicate that
        /// the search response should include detailed feature 
        /// information for search criteria features (including feature
        /// stats and generic status).
        /// <para>
        /// This flag has no effect unless
        /// <see cref="SzSearchIncludeRequest"/> is also specified.
        /// </para>
        /// <para>
        /// This flag belongs to the following usage groups:
        /// <list type="bullet">
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzSearchFlags"/></description>
        ///    </item>
        ///    <item>
        ///      <description><see cref="SzFlagUsageGroup.SzWhySearchFlags"/></description>
        ///    </item>
        /// </list>
        /// </para>
        /// See also: <seealso href="https://docs.senzing.com/flags/index.html"/>
        /// </summary>
        [SzFlagUsageGroups(SzFlagUsageGroupSets.SzWhySearchSet)]
        SzSearchIncludeRequestDetails = (1L << 38)
    }
}