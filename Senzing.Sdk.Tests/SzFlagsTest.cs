namespace Senzing.Sdk.Tests;

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Senzing.Sdk;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;

using static System.StringComparison;
using static Senzing.Sdk.Tests.SzFlagsMetaData;
using static Senzing.Sdk.Utilities;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(scope: ParallelScope.All)]
internal class SzFlagsTest : AbstractTest
{
    /// <summary>
    /// The dictionary of <c>string</c> constant names from
    /// <see cref="Senzing.Sdk.SzFlag"/> to their values.
    /// </summary>
    private readonly Dictionary<string, SzFlag> enumsMap = new Dictionary<string, SzFlag>();

    /// <summary>
    /// The dictionary of <c>string</c> field names for declared 
    /// constants of aggregate <see cref="SzFlag"/> values to the
    /// actual aggregate <see cref="SzFlag"/> value.
    /// </summary>
    private readonly Dictionary<string, SzFlag> flagsMap = new Dictionary<string, SzFlag>();

    /// <summary>
    /// The <see cref="SzFlagsMetaData"/> describing all the flags. 
    /// </summary>
    private SzFlagsMetaData? flagsMetaData;

    [OneTimeSetUp]
    public void ReflectFlags()
    {
        Type flagsType = typeof(SzFlags);
        Type enumType = typeof(SzFlag);

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
        // populate the enums
        foreach (FieldInfo fieldInfo in enumType.GetFields(bindingFlags))
        {
            this.enumsMap.Add(fieldInfo.Name, ((SzFlag?)fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags);
        }

        // populate the sets
        foreach (FieldInfo fieldInfo in flagsType.GetFields(bindingFlags))
        {
            if (fieldInfo.FieldType != typeof(SzFlag)) continue;
            if (!fieldInfo.Name.StartsWith("Sz", Ordinal)) continue;

            this.flagsMap.Add(fieldInfo.Name, ((SzFlag?)fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags);
        }

        // get the flags meta data
        try
        {
            this.flagsMetaData = new SzFlagsMetaData();

        }
        catch (Exception e)
        {
            Fail(e);
            throw;
        }
    }

    private static IList<(string, SzFlag)> GetEnumMappings()
    {
        Type enumType = typeof(SzFlag);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

        IList<(string, SzFlag)> results = new List<(string, SzFlag)>();
        foreach (FieldInfo fieldInfo in enumType.GetFields(bindingFlags))
        {
            results.Add((fieldInfo.Name, ((SzFlag?)fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags));
        }
        return results;
    }

    private static IList<(string, SzFlag)> GetFlagsMappings()
    {
        Type flagsType = typeof(SzFlags);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

        IList<(string, SzFlag)> results = new List<(string, SzFlag)>();

        // populate the sets
        foreach (FieldInfo fieldInfo in flagsType.GetFields(bindingFlags))
        {
            if (fieldInfo.FieldType != typeof(SzFlag)) continue;
            if (!fieldInfo.Name.StartsWith("Sz", Ordinal)) continue;

            results.Add((fieldInfo.Name, ((SzFlag?)fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags));
        }

        return results;

    }

    private static IList<(string, long)> GetMetaMappings()
    {
        List<(string, long)> results = new List<(string, long)>();
        SzFlagsMetaData flagsMetaData = new SzFlagsMetaData();

        IReadOnlyDictionary<string, SzFlagMetaData> metaMap = flagsMetaData.Flags;

        foreach (SzFlagMetaData metaData in metaMap.Values)
        {
            results.Add((metaData.Symbol, metaData.Value));
        }
        return results;
    }

    private static IList<SzFlagUsageGroup> GetEnumGroups()
    {
        List<SzFlagUsageGroup> results = new List<SzFlagUsageGroup>();
        foreach (SzFlagUsageGroup group in Enum.GetValues(typeof(SzFlagUsageGroup)))
        {
            results.Add(group);
        }
        return results;
    }

    private static IList<string> GetMetaGroups()
    {
        List<string> results = new List<string>();
        SzFlagsMetaData flagsMetaData = new SzFlagsMetaData();
        foreach (string group in flagsMetaData.Groups)
        {
            results.Add(group);
        }
        return results;
    }

    [Test, TestCaseSource(nameof(GetFlagsMappings))]
    public void TestFlagsConstant((string name, SzFlag value) args)
    {
        this.PerformTest(() =>
        {
            string name = args.name;
            SzFlag value = args.value;
            if (name.EndsWith("AllFlags", Ordinal))
            {
                int length = name.Length;
                string prefix = name.Substring(0, length - "AllFlags".Length);
                string groupName = prefix + "Flags";

                SzFlagUsageGroup? parsedGroup = null;

                try
                {
                    parsedGroup = (SzFlagUsageGroup?)
                        Enum.Parse(typeof(SzFlagUsageGroup), groupName, false);

                }
                catch (Exception e)
                {
                    Fail("Failed to get SzFlagUsageGroup for AllFlags set: "
                        + "set=[ " + name + "], group=[ " + groupName + "]", e);
                    throw;
                }
                SzFlagUsageGroup group = (parsedGroup ?? ((SzFlagUsageGroup)0L));


                IReadOnlyDictionary<string, SzFlagMetaData>? metaMap
                    = this.flagsMetaData?.GetBaseFlagsByGroup("" + group);

                Assert.That(metaMap, Is.Not.Null,
                    "No meta group found for group name: " + group);
                long metaGroupValue = 0L;
                StringBuilder sb = new StringBuilder();
                string conjunction = "";
                foreach (SzFlagMetaData metaData in metaMap.Values)
                {
                    metaGroupValue |= metaData.Value;
                    sb.Append(conjunction);
                    sb.Append(metaData.Symbol);
                    conjunction = " | ";
                }

                SzFlag groupFlagsValue = SzFlags.GetFlags(group);


                Assert.That(((long)value), Is.EqualTo(metaGroupValue),
                         "Meta value for group (" + group + ") has a different "
                        + "primitive long value (" + HexFormat(metaGroupValue)
                        + " / " + sb.ToString()
                        + ") than expected (" + HexFormat((long)value)
                        + " / " + group.FlagsToString(value) + "): " + name);

                Assert.That(groupFlagsValue, Is.EqualTo(value),
                            "Value for group (" + group + ") has a different "
                            + "primitive long value ("
                            + HexFormat((long)groupFlagsValue)
                            + " / " + group.FlagsToString(SzFlags.GetFlags(group))
                            + ") than expected (" + HexFormat((long)value)
                            + "): " + name);

            }
            else if (!nameof(SzFlags.SzNoFlags).Equals(name, Ordinal)
                     && !nameof(SzFlags.SzRedoDefaultFlags).Equals(name, Ordinal))
            {
                SzFlagMetaData? metaData = this.flagsMetaData?.GetFlag(name);
                Assert.That(metaData, Is.Not.Null,
                            "Aggregate flag constant (" + name + ") not found "
                            + "in meta-data.");
                Assert.That((long)value, Is.EqualTo(metaData?.Value),
                    "Aggregate flag constant (" + name + ") has a different "
                    + "value (" + HexFormat((long)value) + ") than found in "
                    + "meta-data: " + HexFormat(metaData?.Value ?? 0L));
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestEnumFlag((string name, SzFlag value) args)
    {
        this.PerformTest(() =>
        {
            string name = args.name;
            SzFlag value = args.value;

            SzFlagMetaData? metaData = this.flagsMetaData?.GetFlag(name);

            Assert.That(metaData, Is.Not.Null,
                "Enum flag constant (" + name + ") not found in meta-data");

            Assert.That((long)value, Is.EqualTo(metaData?.Value),
                "Enum flag constant (" + name + ") has different value ("
                + HexFormat((long)value) + ") than found in meta-data: "
                + HexFormat(metaData?.Value ?? 0L));
        });
    }

    [Test, TestCaseSource(nameof(GetMetaMappings))]
    public void TestMetaFlag((string name, long value) args)
    {
        this.PerformTest(() =>
        {
            string name = args.name;
            long value = args.value;

            SzFlagMetaData? metaData = this.flagsMetaData?.GetFlag(name);

            if (metaData == null)
            {
                Fail("Meta data flag name not found in meta data: " + name);
                throw new ArgumentNullException(nameof(args));
            }
            if (!metaData.Aggregate && metaData.Value != 0)
            {
                bool enumFound = this.enumsMap.TryGetValue(name, out SzFlag enumValue);
                Assert.IsTrue(enumFound, "SDK Enum Flag constant not found for "
                    + "meta-data flag: " + name);
                Assert.That((long)enumValue, Is.EqualTo(value),
                            "SDK Enum Flag constant (" + name + ") has differnet value ("
                            + HexFormat((long)enumValue) + ") than found in meta-data: "
                            + HexFormat(value));
            }
            else
            {
                bool flagsFound = this.flagsMap.TryGetValue(name, out SzFlag flagsValue);
                Assert.IsTrue(flagsFound, "SDK Aggregate Enum Flag constant not found for "
                              + "meta-data flag: " + name);
                Assert.That((long)flagsValue, Is.EqualTo(value),
                            "SDK Aggregate Enum Flag constant (" + name + ") has differnet"
                            + "value (" + HexFormat((long)flagsValue) + ") than found in "
                            + "meta-data: " + HexFormat(value));
            }
        });
    }


    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestNamedMappings((string name, SzFlag value) args)
    {
        this.PerformTest(() =>
        {
            string name = args.name;
            SzFlag value = args.value;

            ReadOnlyDictionary<string, SzFlag> flagsByName = SzFlags.GetFlagsByName();

            Assert.IsTrue(flagsByName.ContainsKey(name),
                        "SzFlag symbolic name not found in named flags: " + name);

            Assert.That(flagsByName[name], Is.EqualTo(value),
                        "Flag constant (" + name + ") has a different value  ("
                        + HexFormat((long)flagsByName[name])
                        + ") than enum flag constant (" + name + "): "
                        + HexFormat((long)value));
        });
    }

    [Test]
    public void TestGetGroupsUnrecognized()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzFlagUsageGroup groups = SzFlags.GetGroups("UNRECOGNIZED");

                Fail("Unexpected succeeded in getting groups for an unrecognized "
                    + "flag name.");

            }
            catch (ArgumentException)
            {
                // expected
            }
            catch (Exception e)
            {
                Fail("Got an unexpected exception while getting groups "
                    + "for an unrecognized flag name", e);
                throw;
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestGetGroups((string name, SzFlag flag) args)
    {
        this.PerformTest(() =>
        {
            string name = args.name;
            SzFlag flag = args.flag;

            SzFlagMetaData? metaData = this.flagsMetaData?.GetFlag(name);
            if (metaData == null)
            {
                Fail("Meta data not found for SDK enum flag: " + name);
                throw new InvalidOperationException();
            }

            IReadOnlyList<string> metaGroupNames = metaData.Groups;

            SzFlagUsageGroup groups = SzFlags.GetGroups(name);

            ISet<string> groupNames = new HashSet<string>();
            foreach (SzFlagUsageGroup group in Enum.GetValues(typeof(SzFlagUsageGroup)))
            {
                if ((group & groups) != 0L)
                {
                    groupNames.Add("" + group);
                }
            }

            SzFlagUsageGroup metaGroups = (SzFlagUsageGroup)0L;
            foreach (string groupName in metaGroupNames)
            {
                SzFlagUsageGroup? parsedGroup = (SzFlagUsageGroup?)
                    Enum.Parse(typeof(SzFlagUsageGroup), groupName, false);
                if (parsedGroup == null)
                {
                    Fail("Group (" + groupName + ") found in meta-data for "
                         + "flag (" + name + ") is not found in SDK groups: "
                         + metaGroupNames);
                }
                else
                {
                    metaGroups |= (SzFlagUsageGroup)parsedGroup;
                }
            }

            Assert.That(groups, Is.EqualTo(metaGroups),
                "SDK flag (" + name + ") has different groups (" + groups
                + " / " + groupNames + ") than is found in meta-data groups "
                + "for flag: " + metaGroups + " / " + metaGroupNames);

            foreach (SzFlagUsageGroup group in Enum.GetValues(typeof(SzFlagUsageGroup)))
            {
                IReadOnlyDictionary<string, SzFlagMetaData>? metaFlags
                    = this.flagsMetaData?.GetBaseFlagsByGroup("" + group);
                if (metaFlags == null)
                {
                    Fail("Group name not found in meta-data: " + group);
                    throw new InvalidOperationException();
                }
                IDictionary<string, SzFlag> flagsByName = SzFlags.GetFlagsByName(group);
                IDictionary<SzFlag, string> namesByFlag = SzFlags.GetNamesByFlag(group);
                SzFlag flags = SzFlags.GetFlags(group);

                // check if this group is included in groups by flags
                if ((group & groups) == 0L)
                {
                    Assert.IsFalse(metaFlags.ContainsKey(name),
                                "Groups by flag name (" + name
                                    + ") does not contain group ("
                                    + group + ") but meta data has flags "
                                    + " for group containing flag ("
                                    + name + " / " + debug(metaFlags.Keys) + ")");

                    Assert.IsFalse(flagsByName.ContainsKey(name),
                                "Groups by flag name (" + name
                                    + ") does not contain group ("
                                    + group + ") but group contains flag ("
                                    + name + " / " + flag.FlagsToString() + ")");

                }
                else
                {
                    Assert.IsTrue(metaFlags.ContainsKey(name),
                                "Groups by flag name (" + name
                                    + ") contains group ("
                                    + group + ") but meta data has flags "
                                    + " for group NOT containing flag ("
                                    + name + " / " + debug(metaFlags.Keys) + ")");
                    Assert.IsTrue(flagsByName.ContainsKey(name),
                                "Groups by flag name (" + name
                                + ") contains group ("
                                + group + ") but group flags by name "
                                + "does not contains flag ("
                                + name + " / " + flag.FlagsToString() + ")");
                    Assert.IsTrue(namesByFlag.ContainsKey(flag),
                                "Groups by flag name (" + name
                                + ") contains group ("
                                + group + ") but group flag names by flag "
                                + "does not contains flag ("
                                + name + " / " + flag.FlagsToString() + ")");
                    Assert.That((long)(flags & flag), Is.Not.EqualTo(0L),
                                "Groups by flag name (" + name
                                + ") contains group (" + group
                                + " ) but group does not contain flag ("
                                + name + " / " + group.FlagsToString(flag) + ")");
                }
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEnumGroups))]
    public void TestEnumGroup(SzFlagUsageGroup group)
    {
        this.PerformTest(() =>
        {
            IReadOnlyList<string>? metaGroups = this.flagsMetaData?.Groups;
            if (metaGroups == null)
            {
                Fail("Flags meta data was null");
                throw new InvalidOperationException();
            }

            string groupName = "" + group;
            Assert.That(metaGroups.Contains(groupName),
                        "SDK group not found in meta data: " + groupName);

            IReadOnlyDictionary<string, SzFlagMetaData>? metaFlags
                = this.flagsMetaData?.GetBaseFlagsByGroup(groupName);

            if (metaFlags == null)
            {
                Fail("Meta data flags not found for group: " + group);
                throw new InvalidOperationException();
            }
            ReadOnlyDictionary<string, SzFlag> flagsByName = SzFlags.GetFlagsByName(group);
            foreach (string flagName in metaFlags.Keys)
            {
                Assert.That(flagsByName.ContainsKey(flagName),
                         "SDK flags for group name (" + group + ") does not contain "
                         + "flag (" + flagName + ") found in meta data: "
                         + debug(metaFlags.Keys));
            }
            foreach (string flagName in flagsByName.Keys)
            {
                Assert.That(metaFlags.ContainsKey(flagName),
                         "SDK flags for group name (" + group + ") contains flag ("
                         + flagName + ") NOT found in meta data: "
                         + debug(metaFlags.Keys));
            }
        });
    }

    [Test, TestCaseSource(nameof(GetMetaGroups))]
    public void TestMetaGroup(string groupName)
    {
        this.PerformTest(() =>
        {
            SzFlagUsageGroup? parsedGroup = (SzFlagUsageGroup?)
                        Enum.Parse(typeof(SzFlagUsageGroup), groupName, false);

            if (parsedGroup == null)
            {
                Fail("Group found in meta data was not found in SDK "
                     + "enumerated groups: " + groupName);
            }
        });
    }

    private static IList<(SzFlag?, string)> GetToFlagStringParams()
    {
        IList<(SzFlag?, string)> results = new List<(SzFlag?, string)>();
        results.Add((null, "{ NONE } [0000 0000 0000 0000]"));
        results.Add((SzFlags.SzNoFlags, "{ NONE } [0000 0000 0000 0000]"));

        StringBuilder sb = new StringBuilder(300);
        IDictionary<string, SzFlag> ambiguousFlags
            = new Dictionary<string, SzFlag>();

        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludeResolved), SzFlag.SzSearchIncludeResolved);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludePossiblySame), SzFlag.SzSearchIncludePossiblySame);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludePossiblyRelated), SzFlag.SzSearchIncludePossiblyRelated);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludeNameOnly), SzFlag.SzSearchIncludeNameOnly);

        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludeMultiRecordEntities), SzFlag.SzExportIncludeMultiRecordEntities);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludePossiblySame), SzFlag.SzExportIncludePossiblySame);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludePossiblyRelated), SzFlag.SzExportIncludePossiblyRelated);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludeNameOnly), SzFlag.SzExportIncludeNameOnly);

        IDictionary<SzFlag, List<string>> ambiguousNames
            = new Dictionary<SzFlag, List<string>>();

        foreach (KeyValuePair<string, SzFlag> pair in ambiguousFlags)
        {
            SzFlag flag = pair.Value;
            string name = pair.Key;
            if (!ambiguousNames.ContainsKey(flag))
            {
                ambiguousNames[flag] = new List<string>();
            }
            ambiguousNames[flag].Add(name);
        }
        foreach (KeyValuePair<SzFlag, List<string>> pair in ambiguousNames)
        {
            pair.Value.Sort();
        }

        List<SzFlag> flags = new List<SzFlag>();
        SzFlag allFlags = SzFlags.SzNoFlags;
        foreach (SzFlag flag in Enum.GetValues(typeof(SzFlag)))
        {
            flags.Add(flag);
            allFlags |= flag;
        }
        unchecked
        {
            flags.Add((SzFlag)(1L << 45));
        }
        flags.Sort();
        int start = 0;
        for (int loop = 0; loop < 3; loop++)
        {
            for (int count = 1; count < 10; count++)
            {
                // clear the string builder
                sb.Clear();

                // initialize the flags
                SzFlag aggregateFlags = SzFlags.SzNoFlags;

                // reset the start if need be
                if (start > flags.Count - count)
                {
                    start = count;
                }

                string prefix = "";

                int end = start + count;

                // loop through flags and add them in
                for (int index = start; index < end; index++, start++)
                {
                    SzFlag flag = flags[index];
                    SzFlag before = aggregateFlags;
                    aggregateFlags |= flag;
                    if (aggregateFlags == before) continue;

                    sb.Append(prefix);
                    if (ambiguousNames.ContainsKey(flag))
                    {
                        sb.Append("{ ");
                        string prefix2 = "";
                        foreach (string name in ambiguousNames[flag])
                        {
                            sb.Append(prefix2);
                            sb.Append(name);
                            prefix2 = " / ";
                        }
                        sb.Append(" }");

                    }
                    else if ((flag & allFlags) == SzFlags.SzNoFlags)
                    {
                        sb.Append(HexFormat((long)flag));
                    }
                    else
                    {
                        sb.Append(flag.ToString());
                    }
                    prefix = " | ";
                }
                sb.Append(" [");
                sb.Append(HexFormat((long)aggregateFlags));
                sb.Append(']');

                String expected = sb.ToString();
                results.Add((aggregateFlags, expected));
            }
        }

        return results;
    }

    [Test]
    public void TestZeroGetFlags()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzFlag flags = SzFlags.GetFlags(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get flags for "
                    + "SzNoFlagUsageGroups: " + SzFlags.FlagsToString(flags));

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestZeroGetFlagsByName()
    {
        this.PerformTest(() =>
        {
            try
            {
                ReadOnlyDictionary<string, SzFlag> flagsByName
                    = SzFlags.GetFlagsByName(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get flags by name for "
                    + "SzNoFlagUsageGroups: " + flagsByName);

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestZeroGetNamesByFlag()
    {
        this.PerformTest(() =>
        {
            try
            {
                ReadOnlyDictionary<SzFlag, string> namesByFlag
                    = SzFlags.GetNamesByFlag(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get names by flag for "
                    + "SzNoFlagUsageGroups: " + namesByFlag);

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetFlags()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags
                    | SzFlagUsageGroup.SzSearchFlags;

                SzFlag flags = SzFlags.GetFlags(aggregateGroups);

                Fail("Was unexpectedly able to get flags for "
                    + "aggregate group: " + SzFlags.FlagsToString(flags));

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetFlagsByName()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags
                    | SzFlagUsageGroup.SzSearchFlags;

                ReadOnlyDictionary<string, SzFlag> flagsByName
                    = SzFlags.GetFlagsByName(aggregateGroups);

                Fail("Was unexpectedly able to get flags by name for "
                    + "aggregate group: " + flagsByName);

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetNamesByFlag()
    {
        this.PerformTest(() =>
        {
            try
            {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags
                    | SzFlagUsageGroup.SzSearchFlags;

                ReadOnlyDictionary<SzFlag, string> namesByFlag
                    = SzFlags.GetNamesByFlag(aggregateGroups);

                Fail("Was unexpectedly able to get names by flag for "
                    + "aggregate group: " + namesByFlag);

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test, TestCaseSource(nameof(GetToFlagStringParams))]
    public void TestToFlagString((SzFlag? flag, string expected) args)
    {
        this.PerformTest(() =>
        {
            SzFlag? flag = args.flag;
            string expected = args.expected;
            string? actual = SzFlags.FlagsToString(flag);
            Assert.That(actual, Is.EqualTo(expected),
                        "ToFlagString(this SzFlag) did not return as expected.  "
                        + "actual=[ " + actual + " ], expected=[ " + expected + " ]");
        });
    }

    private static IList<SzFlagUsageGroup> GetEnumFlagGroups()
    {
        Type groupType = typeof(SzFlagUsageGroup);
        IList<SzFlagUsageGroup> result = new List<SzFlagUsageGroup>();

        foreach (SzFlagUsageGroup group in Enum.GetValues(groupType))
        {
            result.Add(group);
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetEnumFlagGroups))]
    public void TestZeroToString(SzFlagUsageGroup group)
    {
        this.PerformTest(() =>
        {
            string text = group.FlagsToString(SzFlags.SzNoFlags);
            Assert.IsNotNull(text, "The result for zero is null");
            Assert.IsTrue(text.Contains("NONE", Ordinal),
                        "The result for zero does not contain NONE");
        });
    }

    [Test, TestCaseSource(nameof(GetEnumFlagGroups))]
    public void TestGetFlags(SzFlagUsageGroup group)
    {
        this.PerformTest(() =>
        {
            ReadOnlyDictionary<string, SzFlag> flagsByName
                = SzFlags.GetFlagsByName(group);
            ReadOnlyDictionary<SzFlag, string> namesByFlag
                = SzFlags.GetNamesByFlag(group);

            SzFlag flags = SzFlags.GetFlags(group);

            if (group != SzFlagUsageGroup.SzFindInterestingEntitiesFlags)
            {
                Assert.That(flags, Is.Not.EqualTo(SzFlags.SzNoFlags),
                            "Flags for group should not be zero: " + group);
            }

            SzFlag aggregateFlags = SzFlags.SzNoFlags;
            foreach (KeyValuePair<string, SzFlag> pair in flagsByName)
            {
                string name = pair.Key;
                SzFlag flag = pair.Value;
                aggregateFlags |= flag;
                Assert.IsTrue((flags & flag) != SzFlags.SzNoFlags,
                    "Flag (" + name + " ) missing from aggregate flag value "
                    + "of group: " + group);
            }
            Assert.That(flags, Is.EqualTo(aggregateFlags),
                "Group flags value and aggregate flag values differ: "
                + group);

            Assert.That(flagsByName.Count, Is.EqualTo(namesByFlag.Count),
                "Group flags by name map does not have the same size as "
                + "group flag names by flag: group=[ " + group
                + " ], flagsByName=[ " + flagsByName + " ], namesByFlag=[ "
                + namesByFlag + " ]");

            foreach (KeyValuePair<string, SzFlag> pair in flagsByName)
            {
                Assert.IsTrue(namesByFlag.ContainsKey(pair.Value),
                    "Flag value not found in names by flag map for group, "
                    + "but flag is present in flags by name map for group.  "
                    + "group=[ " + group + " ], flagName=[ " + pair.Key
                    + " ], flagValue=[ " + HexFormat((long)pair.Value)
                    + " ], flagsByName=[ " + flagsByName + " ], namesByFlag=[ "
                    + namesByFlag + " ]");
                Assert.That(namesByFlag[pair.Value], Is.EqualTo(pair.Key),
                    "Flag name for value in names by flag map for group "
                    + "does not match the association in the flags by name map "
                    + "for the group.  group=[ " + group + " ], flagName=[ "
                    + pair.Key + " ], flagValue=[ "
                    + HexFormat((long)pair.Value) + " ], flagsByName=[ "
                    + flagsByName + " ], namesByFlag=[ " + namesByFlag + " ]");
            }

            foreach (KeyValuePair<SzFlag, string> pair in namesByFlag)
            {
                Assert.IsTrue(flagsByName.ContainsKey(pair.Value),
                    "Flag name not found in flags by name map for group, "
                    + "but flag is present in names by flag map for group.  "
                    + "group=[ " + group + " ], flagName=[ " + pair.Value
                    + " ], flagValue=[ " + HexFormat((long)pair.Key)
                    + " ], flagsByName=[ " + flagsByName + " ], namesByFlag=[ "
                    + namesByFlag + " ]");
                Assert.That(flagsByName[pair.Value], Is.EqualTo(pair.Key),
                    "Flag value for name in flags by name map for group "
                    + "does not match the association in the names by flag map "
                    + "for the group.  group=[ " + group + " ], flagName=[ "
                    + pair.Value + " ], flagValue=[ "
                    + HexFormat((long)pair.Key) + " ], flagsByName=[ "
                    + flagsByName + " ], namesByFlag=[ " + namesByFlag + " ]");
            }

            ReadOnlyDictionary<string, SzFlag> allFlags = SzFlags.GetFlagsByName();

            foreach (KeyValuePair<string, SzFlag> pair in allFlags)
            {
                string name = pair.Key;
                SzFlag flag = pair.Value;
                SzFlagUsageGroup groups = SzFlags.GetGroups(name);

                Assert.That((long)groups, Is.Not.EqualTo(0L),
                            "Groups for flag should not be zero: " + name);

                if (flagsByName.ContainsKey(name))
                {
                    Assert.IsTrue((flags & flag) != SzFlags.SzNoFlags,
                        "Flag name (" + name + ") is in named flags for "
                        + "group (" + group + "), but value is not "
                        + "in the aggregate flag values.");
                    Assert.That((long)(groups & group), Is.Not.EqualTo(0L),
                        "Group (" + group + ") has flag (" + name + ") but the "
                        + "flag does not have the group.  flagsForGroup=[ "
                        + debug(flagsByName.Keys) + " / "
                        + SzFlags.FlagsToString(flags)
                        + " ], groupsForFlag=[ " + groups + "]");
                }
                else
                {
                    Assert.That((long)(groups & group), Is.EqualTo(0L),
                        "Group (" + group + ") has flag (" + name + ") but the "
                        + "flag does not have the group.  flagsForGroup=[ "
                        + debug(flagsByName.Keys) + " / "
                        + SzFlags.FlagsToString(flags)
                        + " ], groupsForFlag=[ " + groups + "]");
                }
            }
        });
    }

    [Test]
    [TestCase("SzSearchIncludeNameOnly", SzFlag.SzSearchIncludeNameOnly)]
    [TestCase("SzSearchIncludeResolved", SzFlag.SzSearchIncludeResolved)]
    [TestCase("SzSearchIncludePossiblySame", SzFlag.SzSearchIncludePossiblySame)]
    [TestCase("SzSearchIncludePossiblyRelated", SzFlag.SzSearchIncludePossiblyRelated)]
    [TestCase("SzExportIncludeNameOnly", SzFlag.SzExportIncludeNameOnly)]
    [TestCase("SzExportIncludeMultiRecordEntities", SzFlag.SzExportIncludeMultiRecordEntities)]
    [TestCase("SzExportIncludePossiblySame", SzFlag.SzExportIncludePossiblySame)]
    [TestCase("SzExportIncludePossiblyRelated", SzFlag.SzExportIncludePossiblyRelated)]
    public void TestGetAmbiguousNamedFlags(String name, SzFlag value)
    {
        this.PerformTest(() =>
        {
            ReadOnlyDictionary<string, SzFlag> flagsByName = SzFlags.GetFlagsByName();
            Assert.IsTrue(flagsByName.ContainsKey(name),
                "Flag not found in named flags: " + name);
            SzFlag actualValue = flagsByName[name];
            Assert.That(actualValue, Is.EqualTo(value),
                "Named flags value (" + actualValue + ") for " + name
                + " does not match expected value: " + value);
        });
    }


    private static IList<(SzFlagUsageGroup, SzFlag?, ISet<string>)> GetGroupToStringParameters()
    {
        IList<(SzFlagUsageGroup, SzFlag?, ISet<string>)> result
            = new List<(SzFlagUsageGroup, SzFlag?, ISet<string>)>();

        Type groupType = typeof(SzFlagUsageGroup);
        Type flagType = typeof(SzFlag);

        ReadOnlyDictionary<string, SzFlag> allFlags = SzFlags.GetFlagsByName();
        List<(string, SzFlag)> allFlagsList = new List<(string, SzFlag)>();
        foreach (KeyValuePair<string, SzFlag> pair in allFlags)
        {
            allFlagsList.Add((pair.Key, pair.Value));
        }

        result.Add((SzFlags.SzNoFlagUsageGroups,
                    (SzFlag.SzEntityIncludeAllFeatures | SzFlag.SzIncludeFeatureScores),
                    SetOf(HexFormat((long)SzFlag.SzEntityIncludeAllFeatures),
                         HexFormat((long)SzFlag.SzIncludeFeatureScores))));

        result.Add(((SzFlagUsageGroup.SzEntityFlags | SzFlagUsageGroup.SzSearchFlags),
                    (SzFlag.SzEntityIncludeEntityName | SzFlag.SzSearchIncludeNameOnly),
                    SetOf(HexFormat((long)SzFlag.SzEntityIncludeEntityName),
                         HexFormat((long)SzFlag.SzSearchIncludeNameOnly))));

        foreach (SzFlagUsageGroup group in Enum.GetValues(groupType))
        {
            SzFlag groupFlags = SzFlags.GetFlags(group);

            ReadOnlyDictionary<string, SzFlag> flagsByName = SzFlags.GetFlagsByName(group);
            ReadOnlyDictionary<SzFlag, string> namesByFlag = SzFlags.GetNamesByFlag(group);

            List<(string, SzFlag)> namedFlagList = new List<(string, SzFlag)>(flagsByName.Count);
            foreach (KeyValuePair<string, SzFlag> pair in flagsByName)
            {
                namedFlagList.Add((pair.Key, pair.Value));
            }

            result.Add((group, null, SetOf("{ NONE }")));
            result.Add((group, SzFlags.SzNoFlags, SetOf("{ NONE }")));

            int start = 0;
            for (int loop = 0; loop < 3; loop++)
            {
                for (int count = 1; count < 10; count++)
                {
                    ISet<string> expected = new HashSet<string>();

                    // reset the start if need be
                    if (start > allFlagsList.Count - count)
                    {
                        start = count;
                    }

                    int end = start + count;
                    SzFlag aggregateFlags = SzFlags.SzNoFlags;

                    // loop through flags and add them in
                    for (int index = start; index < end; index++, start++)
                    {
                        (string name, SzFlag flag) tuple = allFlagsList[index];
                        string name = tuple.name;
                        SzFlag flag = tuple.flag;

                        aggregateFlags |= flag;

                        if (flagsByName.ContainsKey(name))
                        {
                            expected.Add(name);

                        }
                        else if ((groupFlags & flag) != SzFlags.SzNoFlags)
                        {
                            expected.Add(namesByFlag[flag]);

                        }
                        else
                        {
                            expected.Add(HexFormat((long)flag));
                        }
                    }
                    result.Add((group, aggregateFlags, expected));
                }
            }
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetGroupToStringParameters))]
    public void TestGetAmbiguousNamedFlags(
        (SzFlagUsageGroup group, SzFlag? flags, ISet<string> expected) args)
    {
        SzFlagUsageGroup group = args.group;
        SzFlag? flags = args.flags;
        ISet<string> expected = args.expected;
        string testData = "group=[ " + group + " ], flags=[ " + flags +
                " ], expected=[ " + expected + " ]";

        string[] separators = [" | ", " ["];
        this.PerformTest(() =>
        {
            string actual = group.FlagsToString(flags);
            Assert.IsNotNull(actual,
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) was null: " + testData);
            Assert.That(actual.Length, Is.Not.EqualTo(0),
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) was empty: " + testData);

            Assert.IsTrue(actual.EndsWith(']'),
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) does not end with ']': "
                + "actual=[ " + actual + " ], " + testData);

            // trim the trailing bracket
            string[] tokens = actual.Substring(0, actual.Length - 1)
                .Split(separators, StringSplitOptions.None);

            Assert.That(tokens[tokens.Length - 1],
                        Is.EqualTo(HexFormat((long)(flags ?? SzFlags.SzNoFlags))),
                        "Hex flag representation is not as expected: " + testData);

            ISet<string> actualSet = new HashSet<string>();
            for (int index = 0; index < tokens.Length - 1; index++)
            {
                actualSet.Add(tokens[index]);
            }

            Assert.That(actualSet.Count, Is.EqualTo(expected.Count),
                "Actual token count does not match expected token count.  actualTokens=[ "
                + actualSet + " ], " + testData);

            foreach (string token in actualSet)
            {
                Assert.IsTrue(expected.Contains(token),
                    "Actual tokens contains unexpected token (" + token + "): "
                    + "actualTokens=[ " + actualSet + " ], " + testData);
            }
            foreach (string token in expected)
            {
                Assert.IsTrue(actualSet.Contains(token),
                    "Expected token (" + token + ") missing from actual tokens: "
                    + "actualTokens=[ " + actualSet + " ], " + testData);
            }
        });
    }

    [Test]
    [TestCaseSource(nameof(GetEnumMappings))]
    [TestCaseSource(nameof(GetFlagsMappings))]
    public void TestFlagsToLong((string name, SzFlag flag) args)
    {
        string name = args.name;
        SzFlag flag = args.flag;
        long value = SzFlags.FlagsToLong(flag);
        Assert.That(value, Is.EqualTo((long)flag),
            "FlagsToLong not as expected for flags: " + name);
    }

    [Test]
    public void TestNullFlagsToLong()
    {
        long value = SzFlags.FlagsToLong(null);
        Assert.That(value, Is.EqualTo(0L),
            "FlagsToLong not as expected for null flags");
    }

    [Test]
    public void TestFlagInfo()
    {
        this.PerformTest(() =>
        {
            Type enumType = typeof(SzFlag);
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
            SzFlagInfo? previous = null;
            foreach (FieldInfo fieldInfo in enumType.GetFields(bindingFlags))
            {
                SzFlagInfo flagInfo1 = new SzFlagInfo(fieldInfo);
                SzFlagInfo flagInfo2 = new SzFlagInfo(fieldInfo);

                Assert.That(flagInfo1, Is.EqualTo(flagInfo2),
                    "Flag info for same field (" + fieldInfo.Name + ") are not equal: "
                    + "flagInfo1=[ " + flagInfo1 + " ], flagInfo2=[ " + flagInfo2 + " ]");

                Assert.IsNotNull(flagInfo1.ToString(),
                    "SzFlagInfo.ToString() returned null for field: " + fieldInfo.Name);

                string expectedString = fieldInfo.Name + " ("
                    + HexFormat((long)(((SzFlag?)fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags))
                    + ")";

                Assert.That(flagInfo1.ToString(), Is.EqualTo(expectedString),
                    "SzFlagInfo.ToString() not as expected: " + fieldInfo.Name);

                Assert.That(flagInfo1.CompareTo(flagInfo2), Is.EqualTo(0),
                    "SzFlagInfo.CompareTo() not returning zero for equal elements: "
                    + fieldInfo.Name);

                Assert.That(flagInfo1.GetHashCode(), Is.EqualTo(flagInfo2.GetHashCode()),
                    "SzFlagInfo.GetHashCode() values not equal for equal objects: " + fieldInfo.Name);

                Assert.IsTrue(flagInfo1.Equals(flagInfo1),
                    "SzFlagInfo.Equals(self) expectedly returned false: " + fieldInfo.Name);

                Assert.That(flagInfo1.CompareTo(flagInfo1), Is.EqualTo(0),
                    "SzFlagInfo.CompareTo(self) expectedly returned non-zero: " + fieldInfo.Name);

                Assert.IsTrue(flagInfo1.CompareTo(null) > 0,
                    "SzFlagInfo.CompareTo(null) expectedly returned non-positive: " + fieldInfo.Name);

                try
                {
                    int compareResult = flagInfo1.CompareTo("Hello");
                    Assert.Fail("Compared SzFlagInfo to string without an exception: " + fieldInfo.Name);
                }
                catch (ArgumentException)
                {
                    // expected
                }
                if (previous != null)
                {
                    Assert.That(flagInfo1, Is.Not.EqualTo(previous),
                        "SzFlagInfo for " + fieldInfo.Name + " unexpectedly compared equal to "
                        + "SzFlagInfo for " + previous.name + " via EqualTo()");

                    int compare = flagInfo1.CompareTo(previous);
                    int reverseCompare = previous.CompareTo(flagInfo1);
                    int expectedCompare = String.Compare(flagInfo1.name,
                                                        previous.name,
                                                        StringComparison.Ordinal);

                    Assert.That(compare, Is.Not.EqualTo(0),
                        "SzFlagInfo for " + fieldInfo.Name + " unexpectedly compared equal to "
                        + "SzFlagInfo for " + previous.name + "via CompareTo()");
                    Assert.That(reverseCompare, Is.Not.EqualTo(0),
                        "SzFlagInfo for " + fieldInfo.Name + " unexpectedly reverse compared "
                        + "equal to SzFlagInfo for " + previous.name + "via CompareTo()");
                    Assert.That(compare, Is.Not.EqualTo(reverseCompare),
                        "Reverse comparison for " + flagInfo1.name + " versus "
                        + previous.name + " unexpectedly equal to forward comparison");
                    Assert.That((compare < 0), Is.EqualTo(expectedCompare < 0),
                        "Comparison ordering for " + flagInfo1.name + " versus "
                        + previous.name + "not as expected");
                    Assert.That((compare < 0), Is.Not.EqualTo(reverseCompare < 0),
                        "Reverse comparison ordering for " + flagInfo1.name
                        + " versus " + previous.name
                        + " unexpectedly equal to forward comparison");
                }
                previous = flagInfo1;
            }
        });
    }


    [Test]
    public void TestBadFlagUsageGroupInfo1()
    {
        this.PerformTest(() =>
        {

            Type enumType = typeof(SzFlag);
            try
            {
                SzFlagUsageGroupInfo badGroup = new SzFlagUsageGroupInfo(
                    SzFlagUsageGroup.SzAddRecordFlags,
                    ListOf(new SzFlagInfo(
                        enumType.GetField(
                            nameof(SzFlag.SzEntityIncludeEntityName)))));
                Assert.Fail("Successfully constructed errant SzFlagUsageGroupInfo");

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    [Test]
    public void TestBadFlagUsageGroupInfo2()
    {
        this.PerformTest(() =>
        {
            Type enumType = typeof(SzFlag);
            try
            {
                SzFlagUsageGroupInfo badGroup = new SzFlagUsageGroupInfo(
                    SzFlagUsageGroup.SzExportFlags,
                    ListOf(new SzFlagInfo(
                            enumType.GetField(
                                nameof(SzFlag.SzExportIncludePossiblyRelated))),
                        new SzFlagInfo(
                            enumType.GetField(
                                nameof(SzFlag.SzExportIncludePossiblyRelated)))));

                Assert.Fail("Successfully constructed errant SzFlagUsageGroupInfo");

            }
            catch (ArgumentException)
            {
                // expected
            }
        });
    }

    private static string debug(IEnumerable<string> strings)
    {
        StringBuilder sb = new StringBuilder();
        string prefix = "[ ";
        foreach (string s in strings)
        {
            sb.Append(prefix).Append(s);
            prefix = ", ";
        }
        sb.Append(" ]");
        return sb.ToString();
    }
}
