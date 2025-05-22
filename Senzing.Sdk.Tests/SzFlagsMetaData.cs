namespace Senzing.Sdk.Tests;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

using Senzing.Sdk.Tests.NativeSzApi;

/// <summary>
/// Loads the flags JSON meta data file and makes its properties available.
/// </summary>
internal class SzFlagsMetaData
{
    private const string SymbolKey = "altSymbol";
    private const string BitsKey = "bits";
    private const string ValueKey = "value";
    private const string DefinitionKey = "definition";
    private const string GroupsKey = "altGroups";
    private const string FlagsKey = "altFlags";

    /**
     * Provides the meta data describing a specific flag.
     */
    public class SzFlagMetaData
    {
        private readonly string symbol;
        private readonly IReadOnlyList<int> bits;
        private readonly long value;
        private readonly bool aggregate;
        private readonly IReadOnlyList<string> definition;
        private readonly IReadOnlyList<string> groups;
        private readonly IReadOnlyList<string>? flags;

        internal SzFlagMetaData(JsonObject jsonObject)
        {
            // get the symbol
            this.symbol = jsonObject[SymbolKey]?.GetValue<string>() ?? "";
            if (this.symbol.Length == 0)
            {
                throw new ArgumentException(
                    "Property (" + SymbolKey + ") not found: " + jsonObject);
            }

            // get the bits
            JsonArray? bitsArray = jsonObject[BitsKey]?.AsArray();
            if (bitsArray == null)
            {
                throw new ArgumentException(
                    "Property (" + BitsKey + ") not found: " + jsonObject);
            }
            List<int> bitsList = new List<int>(bitsArray.Count);
            for (int index = 0; index < bitsArray.Count; index++)
            {
                int? bit = bitsArray[index]?.GetValue<int>();
                if (bit == null)
                {
                    throw new ArgumentException(
                        "Property (" + BitsKey + ") has a null element at index ("
                        + index + "): " + jsonObject);
                }
                bitsList.Add((int)bit);
            }
            this.bits = new ReadOnlyCollection<int>(bitsList);

            // get the value
            long? val = jsonObject[ValueKey]?.GetValue<long>();
            if (val == null)
            {
                throw new ArgumentException(
                    "Property (" + ValueKey + ") not found: " + jsonObject);
            }
            else
            {
                this.value = (long)val;
            }

            // get the definition
            JsonValueKind? kind = jsonObject[DefinitionKey]?.GetValueKind();
            if (kind == null)
            {
                throw new ArgumentException(
                    "Property (" + DefinitionKey + ") not found: " + jsonObject);
            }
            else
            {
                if (kind == JsonValueKind.Array)
                {
                    JsonArray? defArray = jsonObject[DefinitionKey]?.AsArray();
                    if (defArray == null)
                    {
                        throw new ArgumentException(
                            "Property (" + DefinitionKey + ") not found: "
                            + jsonObject);
                    }
                    List<string> defList = new List<string>(defArray.Count);
                    for (int index = 0; index < defArray.Count; index++)
                    {
                        string? element = defArray[index]?.GetValue<string>();
                        if (element == null)
                        {
                            throw new ArgumentException(
                                "Property (" + DefinitionKey + ") has a null "
                                + "element at index (" + index + "): "
                                + jsonObject);
                        }
                        defList.Add(element);
                    }
                    this.definition = new ReadOnlyCollection<string>(defList);

                }
                else
                {
                    string? def = jsonObject[DefinitionKey]?.GetValue<string>();
                    if (def == null)
                    {
                        throw new ArgumentException(
                            "Property (" + DefinitionKey + ") has a value "
                            + "that is neither a string nor a string array: "
                            + jsonObject);
                    }
                    List<string> defList = [def];
                    this.definition = new ReadOnlyCollection<string>(defList);
                }
            }

            // get the groups
            JsonArray? groupsArray = jsonObject[GroupsKey]?.AsArray();
            if (groupsArray == null)
            {
                throw new ArgumentException(
                    "Property (" + GroupsKey + ") not found: " + jsonObject);
            }
            List<string> groupsList = new List<string>(groupsArray.Count);
            for (int index = 0; index < groupsArray.Count; index++)
            {
                string? group = groupsArray[index]?.GetValue<string>();
                if (group == null)
                {
                    throw new ArgumentException(
                        "Property (" + GroupsKey + ") has a null element at index ("
                        + index + "): " + jsonObject);
                }
                groupsList.Add(group);
            }
            this.groups = new ReadOnlyCollection<string>(groupsList);

            // get the sub-flags (if any)
            if (jsonObject.ContainsKey(FlagsKey))
            {
                JsonArray? flagsArray = jsonObject[FlagsKey]?.AsArray();
                if (flagsArray == null)
                {
                    throw new ArgumentException(
                        "Property (" + FlagsKey + ") not found: " + jsonObject);
                }
                List<string> subFlags = new List<string>(flagsArray.Count);
                for (int index = 0; index < flagsArray.Count; index++)
                {
                    string? flag = flagsArray[index]?.GetValue<string>();
                    if (flag == null)
                    {
                        throw new ArgumentException(
                            "Property (" + FlagsKey + ") has a null element at index ("
                            + index + "): " + jsonObject);
                    }
                    subFlags.Add(flag);
                }
                this.flags = new ReadOnlyCollection<string>(subFlags);
            }
            else
            {
                this.flags = null;
            }
            this.aggregate = (this.flags != null
                              || (this.definition.Count == 1
                                  && this.value == 0
                                  && this.groups.Count == 1));
        }

        /// <summary>
        /// Gets the symbol for this flag.
        /// </summary>
        ///
        /// <returns>The sumbol for this flag.</returns>
        public string Symbol
        {
            get
            {
                return this.symbol;
            }
        }

        /// <summary>
        /// Gets the 64-bit <code>long</code> value for this flag.
        /// </summary>
        /// 
        /// <returns>The 64-bit <code>long</code> value for this flag.</returns>
        public long Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Checks if this represents an aggregate flag or a base flag.
        /// </summary>
        /// 
        /// <remarks>
        /// Aggregate flags are bitmasks defined as pre-defined defaults and 
        /// are meant to contain other flags even if the default sometimes 
        /// contains no flags.
        /// </remarks>
        /// 
        /// <returns>
        /// <c>true</c> if this is an aggregate flag, otherwise <c>falce</c>
        /// </returns>
        public bool Aggregate
        {
            get
            {
                return this.aggregate;
            }
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList"/> containing the bit 
        /// indices that are active for this flag.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="IReadOnlyList"/> containing the bit 
        /// indices that are active for this flag.
        /// </returns>
        public IReadOnlyList<int> Bits
        {
            get
            {
                return this.bits;
            }
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList"/> of <c>string</c> definitions
        /// for this flag.
        /// </summary>
        /// 
        /// <remarks>
        /// If this flag is defined in terms of its actual value, then there
        /// is a single member of the form <c>"1 << N"</c> where <c>N</c> is
        /// the bit index that is set.  If this flag is defined in terms of
        /// one or more other flags then the members of the returned 
        /// <see cref="IReadOnlyList"/> are the names of those flags that
        /// are used to define this flag through bitwise-OR operations.
        /// </remarks>
        /// 
        /// <returns>
        /// The <see cref="IReadOnlyList"/> of <c>string</c>
        /// definitions for this flag.
        /// </returns>
        public IReadOnlyList<string> Definition
        {
            get
            {
                return this.definition;
            }
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList"/> of <c>string</c> base flag
        /// names that identify the single-bit base flags aggregated by this flag,
        /// or <c>null</c> if this flag is itself a base flag.
        /// </summary>
        /// 
        /// <remarks>
        /// <b>NOTE:</b> A flag may still be a base flag even if it is defined 
        /// in terms of another flag because some flags have the same underlying
        /// value.  Several of the "search" related flags are examples of this as
        /// they are defined in terms of the "export" flags that perform a similar
        /// function.
        /// </remarks>
        ///
        /// <returns>
        /// The <see cref="IReadOnlyList"/> of <c>string</c> base flag
        /// names that identify the single-bit base flags aggregated by this flag,
        /// or <c>null</c> if this flag is itself a base flag.
        /// </returns>
        public IReadOnlyList<string>? BaseFlags
        {
            get
            {
                return this.flags;
            }
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyList"/> of flag usage group names
        /// identifying the usage groups to which this flag is applicable.
        /// </summary>
        /// 
        /// <returns>
        /// The the <see cref="IReadOnlyList"/> of flag usage group names
        /// identifying the usage groups to which this flag is applicable.
        /// </returns>
        public IReadOnlyList<string> Groups
        {
            get
            {
                return this.groups;
            }
        }
    }

    private readonly IReadOnlyList<string> groups;

    private readonly IReadOnlyDictionary<string, SzFlagMetaData> flagsByName;

    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>> flagsByGroup;

    private readonly IReadOnlyDictionary<string, SzFlagMetaData> baseFlagsByName;

    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>> baseFlagsByGroup;

    private readonly IReadOnlyDictionary<string, SzFlagMetaData> aggrFlagsByName;

    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>> aggrFlagsByGroup;

    private static FileInfo GetFlagsMetaDataFile()
    {
        InstallLocations? locations = InstallLocations.FindLocations();
        if (locations == null)
        {
            throw new InvalidOperationException(
                "Failed to find Senzing install location.");
        }
        DirectoryInfo? baseDir = locations.InstallDirectory;
        if (baseDir == null)
        {
            throw new InvalidOperationException(
                "Failed to find base Senzing install directory.");
        }
        DirectoryInfo sdkDir = new DirectoryInfo(
            Path.Combine(baseDir.FullName, "sdk"));

        if (!sdkDir.Exists)
        {
            throw new InvalidOperationException(
                "The SDK directory does not exist: " + sdkDir.FullName);
        }

        FileInfo flagsFile
            = new FileInfo(Path.Combine(sdkDir.FullName, "szflags.json"));

        if (!flagsFile.Exists)
        {
            throw new InvalidOperationException(
                "The szflags.json file does not exist: " + flagsFile.FullName);
        }
        return flagsFile;
    }

    public SzFlagsMetaData() : this(GetFlagsMetaDataFile())
    {
        // do nothing
    }

    public SzFlagsMetaData(FileInfo flagsFile)
    {
        IDictionary<string, SzFlagMetaData> flagsMap
            = new Dictionary<string, SzFlagMetaData>();
        string jsonText = File.ReadAllText(flagsFile.FullName, Encoding.UTF8);

        JsonArray? jsonArray = JsonNode.Parse(jsonText)?.AsArray();
        if (jsonArray == null)
        {
            throw new JsonException(
                "Failed to parse szflags.json: " + flagsFile.FullName);
        }
        for (int index = 0; index < jsonArray.Count; index++)
        {
            JsonObject? jsonObject = jsonArray[index]?.AsObject();
            if (jsonObject == null)
            {
                throw new JsonException(
                    "Failed to parse szflags.json: " + flagsFile.FullName);
            }
            SzFlagMetaData info = new SzFlagMetaData(jsonObject);
            flagsMap.Add(info.Symbol, info);
        }
        this.flagsByName = new ReadOnlyDictionary<string, SzFlagMetaData>(flagsMap);

        SortedSet<string> groupSet = new SortedSet<string>();
        Dictionary<string, SzFlagMetaData> baseMap = new Dictionary<string, SzFlagMetaData>();
        Dictionary<string, SzFlagMetaData> aggrMap = new Dictionary<string, SzFlagMetaData>();
        Dictionary<string, Dictionary<string, SzFlagMetaData>> groupMap
            = new Dictionary<string, Dictionary<string, SzFlagMetaData>>();
        Dictionary<string, Dictionary<string, SzFlagMetaData>> baseGroupMap
            = new Dictionary<string, Dictionary<string, SzFlagMetaData>>();
        Dictionary<string, Dictionary<string, SzFlagMetaData>> aggrGroupMap
            = new Dictionary<string, Dictionary<string, SzFlagMetaData>>();

        foreach (SzFlagMetaData fmd in this.flagsByName.Values)
        {
            if (fmd.Aggregate)
            {
                aggrMap.Add(fmd.Symbol, fmd);
            }
            else
            {
                baseMap.Add(fmd.Symbol, fmd);
            }

            // get the groups
            foreach (string group in fmd.Groups)
            {
                groupSet.Add(group);
                bool found = groupMap.TryGetValue(
                    group, out Dictionary<string, SzFlagMetaData>? groupFlags);
                if (groupFlags == null)
                {
                    groupFlags = new Dictionary<string, SzFlagMetaData>();
                    groupMap.Add(group, groupFlags);
                }
                groupFlags.Add(fmd.Symbol, fmd);

                Dictionary<string, Dictionary<string, SzFlagMetaData>> parentMap
                    = (fmd.Aggregate) ? aggrGroupMap : baseGroupMap;

                parentMap.TryGetValue(
                    group, out Dictionary<string, SzFlagMetaData>? parentFlags);
                if (parentFlags == null)
                {
                    parentFlags = new Dictionary<string, SzFlagMetaData>();
                    parentMap.Add(group, parentFlags);
                }
                if (!parentFlags.TryGetValue(fmd.Symbol, out SzFlagMetaData? existing))
                {
                    parentFlags.Add(fmd.Symbol, fmd);
                }
                else if (!fmd.Equals(existing))
                {
                    throw new InvalidDataException(
                        "Differing meta-data (" + fmd
                        + ") for same flag symbol ("
                        + fmd.Symbol + "): " + existing);
                }

            }
        }

        // handle the groups that have no flags
        Dictionary<string, SzFlagMetaData> emptyDictionary
            = new Dictionary<string, SzFlagMetaData>();

        foreach (string group in groupSet)
        {
            groupMap.TryAdd(group, emptyDictionary);
            baseGroupMap.TryAdd(group, emptyDictionary);
            aggrGroupMap.TryAdd(group, emptyDictionary);
        }

        //  make all sub-maps unmodifiable
        List<Dictionary<string, Dictionary<string, SzFlagMetaData>>> parentMaps
            = new List<Dictionary<string, Dictionary<string, SzFlagMetaData>>>
                { groupMap, baseGroupMap, aggrGroupMap };

        List<Dictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>> roParentMaps
            = new List<Dictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>>(
                parentMaps.Count);

        foreach (Dictionary<string, Dictionary<string, SzFlagMetaData>> parent in parentMaps)
        {
            Dictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>> roParent
                = new Dictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>();
            roParentMaps.Add(roParent);

            foreach (KeyValuePair<string, Dictionary<string, SzFlagMetaData>> pair in parent)
            {
                Dictionary<string, SzFlagMetaData> map = pair.Value;
                roParent.Add(pair.Key,
                             new ReadOnlyDictionary<string, SzFlagMetaData>(map));
            }
        }

        this.groups = new ReadOnlyCollection<string>([.. groupSet]);
        this.baseFlagsByName = new ReadOnlyDictionary<string, SzFlagMetaData>(baseMap);
        this.aggrFlagsByName = new ReadOnlyDictionary<string, SzFlagMetaData>(aggrMap);
        this.flagsByGroup
            = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>(
                roParentMaps[0]);

        this.baseFlagsByGroup
            = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>(
                roParentMaps[1]);

        this.aggrFlagsByGroup
            = new ReadOnlyDictionary<string, IReadOnlyDictionary<string, SzFlagMetaData>>(
                roParentMaps[2]);
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary"/> of <c>string</c> flag
    /// name keys to <see cref="SzFlagMetaData"/> values for all flags.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="IReadOnlyDictionary"/> of <c>string</c> flag
    /// name keys to <see cref="SzFlagMetaData"/> values for all flags.
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData> Flags
    {
        get
        {
            return this.flagsByName;
        }
    }

    /// <summary>
    /// Gets the <see cref="SzFlagMetaData"/> for the flag with the
    /// specified name.  This returns <c>null</c> if the specified name
    /// is not recognized.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="SzFlagMetaData"/> for the flag with the specified
    /// name, or <c>null</c> if the specified name is not recognized.
    /// </returns>
    public SzFlagMetaData? GetFlag(string name)
    {
        this.flagsByName.TryGetValue(name, out SzFlagMetaData? result);
        return result;
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyList{string}"/> of <c>string</c> group
    /// names for all groups that are found for the flags.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="IReadOnlyList{string}"/> of <c>string</c> group names
    /// for all groups that are found for the flags.
    /// </returns>
    public IReadOnlyList<string> Groups
    {
        get
        {
            return this.groups;
        }
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all flags belonging to the group identified by the specified name.
    /// </summary>
    /// 
    /// <remarks>
    /// If the specified group name is not recognized then <c>null</c> is
    /// returned.
    /// </remarks>
    /// 
    /// <param name="group">
    /// The group name identifying the group.
    /// </param>
    /// 
    /// <returns>
    /// An <<see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all flags belonging to the group identified by the specified name,
    /// or <c>null</c> if the specified group name is not recognized.
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData>? GetFlagsByGroup(string group)
    {
        this.flagsByGroup.TryGetValue(
            group, out IReadOnlyDictionary<string, SzFlagMetaData>? result);
        return result;
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all base flags (i.e.: those flags that do <b>not</b> have composite
    /// base flags).
    /// </summary>
    /// 
    /// <returns>
    /// Am <see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all base flags.
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData> BaseFlags
    {
        get
        {
            return this.baseFlagsByName;
        }
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/>
    /// of <c>string</c> flag name keys to <see cref="SzFlagMetaData"/>
    /// values for all aggregate flags (i.e.: those flags that do have
    /// composite base flags).
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="IReadOnlyDictionary{string,SzFlagMetaData}"/>
    /// of <c>string</c> flag name keys to <see cref="SzFlagMetaData"/>
    /// values for all aggregate flags
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData> AggregateFlags
    {
        get
        {
            return this.aggrFlagsByName;
        }
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{string,SzFlagMetaData}" of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all base flags belonging to the group identified by the specified
    /// name.  Base flags are those that do <b>not</b> have composite base
    /// flags.  If the specified group name is not recognized then <c>null</c>
    /// is returned.
    /// </summary>
    /// 
    /// <param name="group">
    /// The group name identifying the group.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="IReadOnlyDictionary{string,SzFlagMetaData}" of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all base flags belonging to the group identified by the specified
    /// name, or <c>null</c> if the specified group name is not recognized.
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData>? GetBaseFlagsByGroup(string group)
    {
        this.baseFlagsByGroup.TryGetValue(
            group, out IReadOnlyDictionary<string, SzFlagMetaData>? result);
        return result;
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyDictionary{string, SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all aggregate flags belonging to the group identified by the
    /// specified name.
    /// </summary>
    /// 
    /// <remarks>
    /// Aggregate flags are those that do have composite base flags.  If the
    /// specified group name is not recognized then <c>null</c> is returned.
    /// </remarks>
    /// 
    /// <param name="group">
    /// The group name identifying the group.
    /// </param>
    /// 
    /// <returns>
    /// An <see cref="IReadOnlyDictionary{string, SzFlagMetaData}"/> of
    /// <c>string</c> flag name keys to <see cref="SzFlagMetaData"/> values
    /// for all aggregate flags belonging to the group identified by the
    /// specified name.
    /// </returns>
    public IReadOnlyDictionary<string, SzFlagMetaData>? GetAggregateFlagsByGroup(string group)
    {
        this.aggrFlagsByGroup.TryGetValue(
            group, out IReadOnlyDictionary<string, SzFlagMetaData>? result);
        return result;
    }
}
