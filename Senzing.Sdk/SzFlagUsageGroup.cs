using System;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;

namespace Senzing.Sdk
{
    /// <summary>
    /// Enumerates the various classifications of usage groups for the
    /// <see cref="SzFlag"/> instances.
    /// </summary>
    [Flags]
    public enum SzFlagUsageGroup : long
    {
        /// <summary>
        /// Flags in this usage group can be used for operations that add (load)
        /// records to the entity repository.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.AddRecord(string, string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzWithInfo"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzAddRecordDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzAddRecordFlags = (1L << 0),

        /// <summary>
        /// Flags in this usage group can be used for operations that delete
        /// records from the entity repository.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.DeleteRecord(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzWithInfo"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzDeleteRecordDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzDeleteRecordFlags = (1L << 1),

        /// <summary>
        /// Flags in this usage group can be used for operations that reevaluate
        /// records in the entity repository.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.ReevaluateRecord(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzWithInfo"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzReevaluateRecordDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzReevaluateRecordFlags = (1L << 2),

        /// <summary>
        /// Flags in this usage group can be used for operations that reevaluate
        /// entities in the entity repository.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.ReevaluateEntity(long, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzWithInfo"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzReevaluateEntityDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzReevaluateEntityFlags = (1L << 3),

        /// <summary>
        /// Flags in this usage group can be used for operations that process 
        /// redo records in the entity repository.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.ProcessRedoRecord(string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzWithInfo"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzRedoDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzRedoFlags = (1L << 4),

        /// <summary>
        /// Flags in this usage group can be used for operations that retrieve record
        /// data in order to control the level of detail of the returned record.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.GetRecord(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
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
        ///     <description><see cref="SzFlag.SzEntityIncludeRecordDates"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>
        SzRecordFlags = (1L << 5),

        /// <summary>
        /// Flags in this usage group can be used for operations that retrieve record
        /// data in order to control the level of detail of the returned record.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.GetRecord(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlag.SzEntityIncludeInternalFeatures"/></description>
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
        ///     <description><see cref="SzFlag.SzEntityIncludeRecordJsonData"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzEntityIncludeRecordUnmappedData"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzRecordPreviewDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>
        SzRecordPreviewFlags = (1L << 6),

        /// <summary>
        /// Flags in this usage group can be used for operations that retrieve
        /// entity data in order to control the level of detail of the returned
        /// entity.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.GetEntity(long, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzRecordDefaultFlags"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzEntityFlags = (1L << 7),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// finding an entity path, what details to include for the entity 
        /// path and the level of detail for the entities on the path that
        /// are returned.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.FindPath(long, long, int, System.Collections.Generic.ISet{long}, System.Collections.Generic.ISet{string}, SzFlag?)"/></item>
        ///   <item><see cref="SzEngine.FindPath(string, string, string, string, int, System.Collections.Generic.ISet{ValueTuple{string, string}}, System.Collections.Generic.ISet{string}, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "find-path" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzFindPathDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzFindPathFlags = (1L << 8),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// finding an entity network, what details to include for the entity 
        /// network and the level of detail for the entities in the network
        /// that are returned.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.FindNetwork(System.Collections.Generic.ISet{long}, int, int, int, SzFlag?)"/></item>
        ///   <item><see cref="SzEngine.FindNetwork(System.Collections.Generic.ISet{ValueTuple{string, string}}, int, int, int, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "find-path" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzFindNetworkDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzFindNetworkFlags = (1L << 9),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// finding an interesting entities and what details to include for the
        /// results of the operation.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.FindInterestingEntities(string, string, SzFlag?)"/></item>
        ///   <item><see cref="SzEngine.FindInterestingEntities(long, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// Currently, there are no <see cref="SzFlag"/> instances included in this usage group.
        /// However, this is a placeholder for when such flags are defined in a future release.
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "find-path" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzFindInterestingEntitiesDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzFindInterestingEntitiesFlags = (1L << 10),

        /// <summary>
        /// Flags in this usage group can be used for operations that search for 
        /// entities to control how the entities are qualified for inclusion
        /// in the search results and the level of detail for the entities
        /// returned in the search results.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.SearchByAttributes(string, SzFlag?)"/></item>
        ///   <item><see cref="SzEngine.SearchByAttributes(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        ///   <item>
        ///     <description><see cref="SzFlag.SzSearchIncludeAllCandidates"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzSearchIncludeRequest"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzSearchIncludeRequestDetails"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzSearchFlags = (1L << 11),

        /// <summary>
        /// Flags in this usage group can be used for operations that export 
        /// entities to control how the entities are qualified for inclusion
        /// in the export and the level of detail for the entities returned
        /// in the search results.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.ExportJsonEntityReport(SzFlag?)"/></item>
        ///   <item><see cref="SzEngine.ExportCsvEntityReport(string, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
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
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzExportFlags = (1L << 12),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// performing "why analysis", what details to include for the analysis
        /// and the level of detail for the entities in the network that are 
        /// returned.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.WhyRecordInEntity(string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "why" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzWhyRecordInEntityDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzWhyRecordInEntityFlags = (1L << 13),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// performing "why analysis", what details to include for the analysis
        /// and the level of detail for the entities in the network that are 
        /// returned.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.WhyRecords(string, string, string, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "why" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzWhyRecordsDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzWhyRecordsFlags = (1L << 14),

        /// <summary>
        /// Flags in this usage group can be used to control the methodology for
        /// performing "why analysis", what details to include for the analysis
        /// and the level of detail for the entities in the network that are 
        /// returned.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.WhyEntities(long, long, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "why" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzWhyEntitiesDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzWhyEntitiesFlags = (1L << 15),

        /// <summary>
        /// Flags in this usage group can be used for operations that search for 
        /// entities to control how the entities are qualified for inclusion
        /// in the search results and the level of detail for the entities
        /// returned in the search results.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.WhySearch(string, long, string, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        ///     <description><see cref="SzFlag.SzSearchIncludeRequest"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzSearchIncludeRequestDetails"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "search" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzWhySearchDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzWhySearchFlags = (1L << 16),

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
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.HowEntity(long, SzFlag?)"/></item>
        /// </list>
        /// <para>
        /// The <see cref="SzFlag"/> instances included in this usage group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlag.SzIncludeMatchKeyDetails"/></description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="SzFlag.SzIncludeFeatureScores"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that use this
        /// group and are defined for "how" operations are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzHowEntityDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants that also 
        /// support this group for defining entity or record detail levels are:
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
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzHowFlags = (1L << 17),

        /// <summary>
        /// Flags in this usage group can be used for operations that retrieve
        /// virtual entities in order to control the level of detail of the
        /// returned virtual entity.
        /// </summary>
        /// <remarks>
        /// Applicable methods include:
        /// <list>
        ///   <item><see cref="SzEngine.GetVirtualEntity(System.Collections.Generic.ISet{ValueTuple{string, string}}, SzFlag?)"/></item>
        /// </list>
        /// <para>
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
        /// </para>
        /// <para>
        /// The pre-defined <see cref="SzFlag"/> aggregate constants for this group are:
        /// <list>
        ///   <item>
        ///     <description><see cref="SzFlags.SzVirtualEntityDefaultFlags"/></description>
        ///   </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <seealso href="https://docs.senzing.com/flags/index.html"/>>
        SzVirtualEntityFlags = (1L << 18)
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal class SzFlagUsageGroupsAttribute : System.Attribute
    {
        internal readonly SzFlagUsageGroup groups;
        public SzFlagUsageGroupsAttribute(SzFlagUsageGroup groups)
        {
            this.groups = groups;
        }
    }

    internal static class SzFlagUsageGroupSets
    {
        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant
        /// to use for <see cref="SzFlag"/> instances that can only be used
        /// for "modify" operations.
        /// </summary>
        internal const SzFlagUsageGroup SzModifySet
            = SzFlagUsageGroup.SzAddRecordFlags | SzFlagUsageGroup.SzDeleteRecordFlags
            | SzFlagUsageGroup.SzReevaluateRecordFlags | SzFlagUsageGroup.SzReevaluateEntityFlags
            | SzFlagUsageGroup.SzRedoFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply
        /// to <see cref="SzFlag"/> instances that retrieve entity data pertaining
        /// to inclusion of related entities and the details of related entities.
        /// </summary>
        internal const SzFlagUsageGroup SzRelationSet
            = SzFlagUsageGroup.SzEntityFlags | SzFlagUsageGroup.SzSearchFlags
            | SzFlagUsageGroup.SzExportFlags | SzFlagUsageGroup.SzFindPathFlags
            | SzFlagUsageGroup.SzFindNetworkFlags | SzFlagUsageGroup.SzWhySearchFlags
            | SzFlagUsageGroup.SzWhyRecordsFlags | SzFlagUsageGroup.SzWhyEntitiesFlags
            | SzFlagUsageGroup.SzWhyRecordInEntityFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply
        /// to apply to <see cref="SzFlag"/> instances that retrieve entity data since
        /// they are used by most operations.
        /// </summary>
        /// <remarks>
        /// Flags that use this aggregate constant should not affect inclusion of or
        /// details of related entities.
        /// </remarks>
        internal const SzFlagUsageGroup SzEntitySet
            = SzRelationSet | SzFlagUsageGroup.SzVirtualEntityFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply
        /// to <see cref="SzFlag"/> instances that retrieve entity data <b>and</b>
        /// also apply to "how" operations.
        /// </summary>
        /// <remarks>
        /// Flags that use this aggregate constant should not affect inclusion of
        /// or details of related entities.
        /// </remarks>
        internal const SzFlagUsageGroup SzEntityHowSet
            = SzRelationSet | SzFlagUsageGroup.SzHowFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that retrieve record data since they are used
        /// by most other groups and can be specifically used for retrieving a single
        /// record.
        /// </summary>
        internal const SzFlagUsageGroup SzEntityRecordSet
            = SzEntitySet | SzFlagUsageGroup.SzRecordFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that are applicable to getting a record
        /// preview.  These are subset of those records applicable to retrieving record
        /// data and therefore an be used by any group that can retrieve entity data.
        /// </summary>
        internal const SzFlagUsageGroup SzRecordPreviewSet
            = SzEntityRecordSet | SzFlagUsageGroup.SzRecordPreviewFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can be used only for "how analysis" and
        /// "why analysis" operations.
        /// </summary>
        internal const SzFlagUsageGroup SzHowWhySearchSet
            = SzFlagUsageGroup.SzWhyRecordInEntityFlags
            | SzFlagUsageGroup.SzWhyRecordsFlags
            | SzFlagUsageGroup.SzWhyEntitiesFlags
            | SzFlagUsageGroup.SzWhySearchFlags
            | SzFlagUsageGroup.SzHowFlags
            | SzFlagUsageGroup.SzSearchFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can be used only for "why search"
        /// and "search" operations.
        /// </summary>
        internal const SzFlagUsageGroup SzWhySearchSet
            = SzFlagUsageGroup.SzSearchFlags | SzFlagUsageGroup.SzWhySearchFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can only be used for "search" operations.
        /// </summary>
        internal const SzFlagUsageGroup SzSearchSet = SzFlagUsageGroup.SzSearchFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can only be used for "export" operations.
        /// </summary>
        internal const SzFlagUsageGroup SzExportSet = SzFlagUsageGroup.SzExportFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can only be used for "find path"
        /// operations.
        /// </summary>
        internal const SzFlagUsageGroup SzFindPathSet
            = SzFlagUsageGroup.SzFindPathFlags;

        /// <summary>
        /// The internal aggregate <see cref="SzFlagUsageGroup"/> constant to apply to
        /// <see cref="SzFlag"/> instances that can only be used for "find network"
        /// operations.
        /// </summary>
        internal const SzFlagUsageGroup SzFindNetworkSet
            = SzFlagUsageGroup.SzFindNetworkFlags;
    }

}