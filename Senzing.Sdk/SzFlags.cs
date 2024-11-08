using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Senzing.Sdk {

/// <summary>
/// Encapsulates a flag value with its name since some flags have the
/// same values, but different usages depending on the symbol which which
/// they are referenced.
/// </summary>
internal class SzFlagInfo : IComparable {
    /// <summary>The read-only symbolic name</summary>
    internal readonly string name;
    /// <summary>The read-only bitwise flag value</summary>
    internal readonly SzFlag value;
    /// <summary>The usage groups for this flag</summary>
    internal readonly SzFlagUsageGroup groups;

    /// <summary>Constructs with the field info</summary>
    internal SzFlagInfo(FieldInfo fieldInfo) {
        this.name   = fieldInfo.Name;
        this.value  = (SzFlag) fieldInfo.GetValue(null);

        object[] attrs = fieldInfo.GetCustomAttributes(
            typeof(SzFlagUsageGroupsAttribute), false);

        this.groups = (attrs.Length > 0) 
            ? ((SzFlagUsageGroupsAttribute) attrs[0]).groups
            : ((SzFlagUsageGroup) 0L);
    }

    /// <summary>Provides a sensible equality implementation.</summary>
    public override bool Equals(object obj) {
        if (obj == null) return false;
        if (this.GetType() != obj.GetType()) return false;
        SzFlagInfo info = (obj as SzFlagInfo);
        return (Object.Equals(this.name, info.name)
                && Object.Equals(this.value, info.value));
    }
    /// <summary>Provides a sensible hash code implementation.</summary>
    public override int GetHashCode() {
        return this.name.GetHashCode() ^ this.value.GetHashCode();
    }
    /// <summary>Provides a sensible comparison implementation.</summary>
    public int CompareTo(object obj) {
        if (obj == null) return 1;
        if (this.GetType() != obj.GetType()) {
            throw new ArgumentException(
                "Cannot compare " + this.GetType() + " to " + obj.GetType());
        }
        SzFlagInfo info = (obj as SzFlagInfo);
        int result = this.name.CompareTo(info.name);
        if (result != 0) return result;
        long diff = ((long) this.value) - ((long) info.value);
        if (diff == 0L) return 0;
        return (diff < 0L) ? -1 : 1;
    }
    /// <summary>Provides a sensible conversion to string</summary>
    public override string ToString() {
        return this.name + " (" + Utilities.HexFormat((Int64) this.value) + ")";
    }
}

/// <summary>
/// Internal class for encapsulating info pertaining to flag usage groups.
/// </summary>
internal class SzFlagUsageGroupInfo {
    internal readonly SzFlagUsageGroup group;
    internal readonly SzFlag flags;

    internal readonly IDictionary<string,SzFlagInfo> infoByName;
    internal readonly IDictionary<SzFlag,SzFlagInfo> infoByFlag;
    internal SzFlagUsageGroupInfo(SzFlagUsageGroup group, ICollection<SzFlagInfo> flagInfos) {
        this.group = group;
        this.infoByName = new Dictionary<string,SzFlagInfo>();
        this.infoByFlag = new Dictionary<SzFlag,SzFlagInfo>();
        
        SzFlag aggregateFlags = (SzFlag) 0L;

        foreach (SzFlagInfo flagInfo in flagInfos) {
            if ((flagInfo.groups & group) == 0) {
                throw new ArgumentException(
                    "The specified group (" + group + " ) is not found in the flag info "
                    + "groups.  flagInfo=[ " + flagInfo + " ], flagInfoGroups=[ "
                    + flagInfo.groups + " ]");
            }
            aggregateFlags |= flagInfo.value;
            this.infoByName.Add(flagInfo.name, flagInfo);
            if (this.infoByFlag.ContainsKey(flagInfo.value)) {
                throw new ArgumentException(
                    "Cannot have the same flag by differnet names in the same group: "
                        + "group=[ " + this.group + " ], flag=[ " + flagInfo
                        + " ], existing=[ " + this.infoByFlag[flagInfo.value] + " ]");
            }
            this.infoByFlag.Add(flagInfo.value, flagInfo);
        }

        // set the flags
        this.flags = aggregateFlags;
    }
}

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
    public const SzFlag SzWhyAllFlags
        = SzEntityAllFlags | SzFlag.SzIncludeFeatureScores;

    /// <summary>
    /// The <see cref="SzFlag"/> value that aggregates all <see cref="SzFlag"/> 
    /// constants belonging to the <see cref="SzFlagUsageGroup.SzHowFlags"/>
    /// usage group.
    /// </summary>
    ///
    /// <seealso cref="SzFlagUsageGroup.SzHowFlags"/>
    public const SzFlag SzHowAllFlags
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

    /// <summary>
    /// Mapping of individual flag values to the possible names to which they belong.
    /// </summary>
    private static readonly IDictionary<SzFlag,List<SzFlagInfo>> FlagInfoByFlag
        = new Dictionary<SzFlag,List<SzFlagInfo>>();

    /// <summary>
    /// Mapping of string flag names to flag values.
    /// </summary>
    private static readonly IDictionary<string,SzFlagInfo> FlagInfoByName
        = new Dictionary<string,SzFlagInfo>();

    /// <summary>
    /// Mapping of individual group values to the group info objects.
    /// </summary>
    private static readonly IDictionary<SzFlagUsageGroup,SzFlagUsageGroupInfo> GroupInfoByGroup
        = new Dictionary<SzFlagUsageGroup,SzFlagUsageGroupInfo>();

    /// <summary>
    /// The number of possible flags bits that can be set.
    /// </summary>
    private const int FlagsBitCount = 64;

    /// <summary>
    /// The class initializer.
    /// </summary>
    static SzFlags() {
        Type flagType = typeof(SzFlag);
        Type groupType = typeof(SzFlagUsageGroup);

        Array allFlags = Enum.GetValues(flagType);
        Array allGroups = Enum.GetValues(groupType);

        IDictionary<SzFlagUsageGroup,IList<SzFlagInfo>> groupFlagInfos
            = new Dictionary<SzFlagUsageGroup,IList<SzFlagInfo>>();

        foreach (SzFlagUsageGroup group in allGroups) {
            groupFlagInfos[group] = new List<SzFlagInfo>();
        }

        foreach (SzFlag flag in allFlags) {
            FlagInfoByFlag[flag] = new List<SzFlagInfo>();
        }

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
        foreach (FieldInfo fieldInfo in flagType.GetFields(bindingFlags)) {
            SzFlagInfo flagInfo = new SzFlagInfo(fieldInfo);
            
            FlagInfoByFlag[flagInfo.value].Add(flagInfo);
            FlagInfoByName[flagInfo.name] = flagInfo;
            foreach (SzFlagUsageGroup group in allGroups) {
                if ((group & flagInfo.groups) != 0L) {
                    groupFlagInfos[group].Add(flagInfo);
                }
            }
        }

        foreach (KeyValuePair<SzFlag,List<SzFlagInfo>> pair in FlagInfoByFlag) {
            pair.Value.Sort((info1,info2) => info1.name.CompareTo(info2.name));
        }

        foreach (SzFlagUsageGroup group in allGroups) {
            IList<SzFlagInfo> flagInfos = groupFlagInfos[group];
            SzFlagUsageGroupInfo groupInfo = new SzFlagUsageGroupInfo(group, flagInfos);
            GroupInfoByGroup.Add(group, groupInfo);
        }
    }

    /// <summary>
    /// Extension method for <see cref="SzFlagUsageGroup"/> to obtain the
    /// aggregate <see cref="SzFlag"/> value for all flags associated with
    /// the group.
    /// </summary>
    ///
    /// <remarks>
    /// If the subject <see cref="SzFlagUsageGroup"/> is itself an aggregate
    /// value of multiple groups then the aggregate flag values for all
    /// flags belonging to all the individual groups is returned (i.e.: the
    /// union of the flags for each groups).  If the respective 
    /// <see cref="SzFlagUsageGroup"/> is zero (0) then <see cref="SzNoFlags"/>
    /// (zero) is returned.
    /// </remarks>
    ///
    /// <param name="group">
    /// The <see cref="SzFlagUsageGroup"/> on which to operate.
    /// </param>
    ///
    /// <returns>
    /// The aggregate <see cref="SzFlag"/> value with the bits set for all
    /// flags belonging to all the <see cref="SzFlagUsageGroup"/> values.
    /// </returns>
    public static SzFlag GetFlags(this SzFlagUsageGroup group) {
        if (GroupInfoByGroup.ContainsKey(group)) {
            SzFlagUsageGroupInfo info = GroupInfoByGroup[group];
            return info.flags;
        }
        SzFlag  result  = SzNoFlags;
        foreach (SzFlagUsageGroupInfo info in GroupInfoByGroup.Values) {
            if ((group & info.group) != 0L) {
                result |= info.flags;
            }
        }
        return result;
    }

    /// <summary>
    /// Formats a <c>string</c> representation of the specified 
    /// <see cref="SzFlag"/> according to the <see cref="SzFlag"/>
    /// instances associated with the respective <see cref="SzFlagUsageGroup"/>.
    /// </summary>
    ///
    /// <remarks>
    /// <p>
    /// <b>NOTE:</b> This method is useful in logging which flags were past to a
    /// particular method using the <see cref="SzFlagUsageGroup"/> for the flags that
    /// are accepted by that method.
    /// <p>
    /// Some <see cref="SzFlag"/> values have the same underlying bitwise flag value
    /// which can lead to ambiguity with the default <c>ToString()</c> implementation
    /// for enums.  However, none of the <see cref="SzFlag"/> instances for a single
    /// <see cref="SzFlagUsageGroup"/> should overlap in bitwise values and this
    /// method will prefer the <see cref="SzFlag"/> symbol belonging to the respective
    /// <see cref="SzFlagUsageGroup"/> for formatting the <c>string</c>.
    /// <p>
    /// If the respective <see cref="SzFlagUsageGroup"/> is an aggregate group value
    /// representing multiple groups (or no groups) then this method simply defaults 
    /// to formatting the set bit values using numeric representation, foregoing the
    /// use of symbolic flag names altogether.
    /// </remarks>
    ///
    /// <param name="group">
    /// The <see cref="SzFlagUsageGroup"/> on which to operate.
    /// </param>
    ///
    /// <param name="flag">
    /// The <see cref="SzFlag"/> value (which may be an aggregate value) to format
    /// as a <c>string</c>.
    /// </param>
    /// 
    /// <returns>
    /// The <c>string</c> describing the specified flags.
    /// </returns>
    public static string ToString(this SzFlagUsageGroup group, SzFlag flag) {
        SzFlagUsageGroupInfo groupInfo = (GroupInfoByGroup.ContainsKey(group))
            ? GroupInfoByGroup[group]
            : null;

        StringBuilder sb = new StringBuilder();
        string prefix = "";
        if (flag == ((SzFlag) 0L)) {
            // handle the zero
            sb.Append("{ NONE }");

        } else {
            for (int index = 0; index < FlagsBitCount; index++) {
                long    singleBit   = (1L << index);
                SzFlag  singleFlag  = (SzFlag) singleBit;

                // check if the flag bit is represented
                if ((singleFlag & flag) != 0L) {
                    // if we have only one then use it directly
                    if (groupInfo == null
                        || (!groupInfo.infoByFlag.ContainsKey(singleFlag)))
                    {
                        sb.Append(prefix);
                        sb.Append(Utilities.HexFormat(singleBit));
                        prefix = " | ";
                    } else {
                        SzFlagInfo flagInfo = groupInfo.infoByFlag[singleFlag];
                        sb.Append(prefix);
                        sb.Append(flagInfo.name);
                        prefix = " | ";
                    }
                }
            }
        }
        sb.Append(" [");
        sb.Append(Utilities.HexFormat((long) flag));
        sb.Append("]");
        return sb.ToString();
    }

    /// <summary>
    /// Gets the aggregate <see cref="SzFlagUsageGroup"/> value representing
    /// the one or more usage groups associated with the respective
    /// <see cref="SzFlag"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// If the respective <see cref="SzFlag"/> is an aggregate value of the 
    /// defined <see cref="SzFlag"/> constants then the returned 
    /// <see cref="SzFlagUsageGroup"/> value will aggregate all those 
    /// individual group flags that are associated with <b>all</b> of the
    /// individual flags.
    /// <p>
    /// <b>NOTE:</b> If a flag value is ambiguous because of a shared bit value
    /// (e.g.: some search and export flags) then the groups for all are 
    /// returned.  To get a disambiguated result, use the static 
    /// <c>GetGroups(string)</c> method.
    /// </remarks>
    ///
    /// <param name="flag">
    /// The <see cref="SzFlag"/> instance on which to operate.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="SzFlagUsageGroup"/> value that aggregates all the
    /// groups associated with every flag in the respective <see cref="SzFlag"/>.
    /// </returns>
    public static SzFlagUsageGroup GetGroups(this SzFlag flag) {
        SzFlagUsageGroup result = (SzFlagUsageGroup) 0L;
        if (FlagInfoByFlag.ContainsKey(flag)) {
            IList<SzFlagInfo> infoList = FlagInfoByFlag[flag];
            foreach (SzFlagInfo flagInfo in infoList) {
                result |= flagInfo.groups;
            }
        } else if (flag != SzNoFlags) {
            for (int index = 0; index < FlagsBitCount; index++) {
                SzFlag singleFlag = (SzFlag) (1L << index);
                if ((flag & singleFlag) != 0L) {
                    if (FlagInfoByFlag.ContainsKey(singleFlag)) {
                        IList<SzFlagInfo> infoList = FlagInfoByFlag[singleFlag];
                        foreach (SzFlagInfo flagInfo in infoList) {
                            result |= flagInfo.groups;
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Gets the aggregate <see cref="SzFlagUsageGroup"/> value representing
    /// the one or more usage groups associated with the specified symbolic
    /// <see cref="SzFlag"/> name.
    /// </summary>
    /// 
    /// <remarks>
    /// If the specified name is not recognized as a symbolic name from 
    /// the <see cref="SzFlag"/> <c>enum</c> then an
    /// <see cref="System.ArgumentException"/> is thrown.
    /// </remarks>
    ///
    /// <param name="symbolicFlagName">
    /// The symbolic flag name from the <see cref="SzFlag"/> <c>enum</c>.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="SzFlagUsageGroup"/> value that aggregates all the
    /// groups associated with the specified <see cref="SzFlag"/> symbolic
    /// name.
    /// </returns>
    ///
    /// <exception cref="System.ArgumentException">
    /// If the specified symbolic flag name is not recognized as an enum
    /// symbol from the <see cref="SzFlag"/> <c>enum</c>.
    /// </exception>
    public static SzFlagUsageGroup GetGroups(string symbolicFlagName) {
        if (!FlagInfoByName.ContainsKey(symbolicFlagName)) {
            throw new ArgumentException(
                "Unrecognized symbolic SzFlag name: " + symbolicFlagName);
        } else {
            return FlagInfoByName[symbolicFlagName].groups;
        }
    }

    /// <summary>
    /// Returns the <c>string</c> representation describing the respective
    /// <see cref="SzFlag"/> value.
    /// </summary>
    ///
    /// <param name="flag">
    /// The <see cref="SzFlag"/> on which to operate.
    /// </param>
    /// 
    /// <returns>
    /// The <c>string</c> representation describing the respective 
    /// <see cref="SzFlag"/> which may be an aggregate value.
    /// </returns>
    public static string ToFlagString(this SzFlag flag) {
        return ToString(flag);
    }

    /// <summary>
    /// Returns the <c>string</c> representation describing the specified
    /// <see cref="SzFlag"/> value.
    /// </summary>
    ///
    /// <param name="flag">
    /// The <see cref="SzFlag"/> to format as a <c>string</c>.
    /// </param>
    /// 
    /// <returns>
    /// The <c>string</c> representation describing the specified 
    /// <see cref="SzFlag"/> which may be an aggregate value.
    /// </returns>
    public static string ToString(SzFlag? flag) {
        StringBuilder sb = new StringBuilder();
        string prefix  = "";
        SzFlag szFlag = (flag ?? SzFlags.SzNoFlags);

        if (flag == null || flag == SzNoFlags) {
            sb.Append("{ NONE }");
        } else if (FlagInfoByFlag.ContainsKey(szFlag)) {
            IList<SzFlagInfo> infoList = FlagInfoByFlag[szFlag];
            if (infoList.Count == 1) {
                sb.Append(prefix);
                sb.Append(infoList[0].name);
                prefix = " | ";
            } else {
                sb.Append(prefix);
                sb.Append("{ ");                
                string prefix2 = "";
                foreach (SzFlagInfo flagInfo in infoList) {
                    sb.Append(prefix2);
                    sb.Append(flagInfo.name);
                    prefix2 = " / ";
                }
                sb.Append(" }");
                prefix = " | ";
            }
        } else {
            for (int index = 0; index < FlagsBitCount; index++) {
                long singleBit = 1L << index;
                SzFlag singleFlag = (SzFlag) singleBit;

                // skip the bits that are not set
                if ((szFlag & singleFlag) == 0L) continue;

                if (!FlagInfoByFlag.ContainsKey(singleFlag)) {
                    sb.Append(prefix);
                    sb.Append(Utilities.HexFormat(singleBit));
                    prefix = " | ";
                } else {
                    IList<SzFlagInfo> infoList = FlagInfoByFlag[singleFlag];
                    if (infoList.Count == 1) {
                        sb.Append(prefix);
                        sb.Append(infoList[0].name);
                        prefix = " | ";
                    } else {
                        sb.Append(prefix);
                        sb.Append("{ ");                
                        string prefix2 = "";
                        foreach (SzFlagInfo flagInfo in infoList) {
                            sb.Append(prefix2);
                            sb.Append(flagInfo.name);
                            prefix2 = " / ";
                        }
                        sb.Append(" }");
                        prefix = " | ";
                     }
                }
            }
        }
        sb.Append(" [");
        sb.Append(Utilities.HexFormat((long) szFlag));
        sb.Append("]");
        return sb.ToString();
    }
}
}