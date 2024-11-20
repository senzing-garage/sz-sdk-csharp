namespace Senzing.Sdk.Tests;

using NUnit.Framework;
using Senzing.Sdk;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;
using System;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(scope: ParallelScope.All)]
internal class SzFlagsTest : AbstractTest {
    /// <summary>
    /// The dictionary of <c>string</c> constant names from
    /// <see cref="Senzing.Sdk.Core.NativeFlags"/> to their values.
    /// </summary>
    private IDictionary<string, long> nativeFlagsMap = new Dictionary<string, long>();

    /// <summary>
    /// The dictionary of <c>string</c> constant names from
    /// <see cref="Senzing.Sdk.SzFlag"/> to their values.
    /// </summary>
    private IDictionary<string, SzFlag> enumsMap = new Dictionary<string, SzFlag>();

    /// <summary>
    /// The dictionary of <c>string</c> field names for declared 
    /// constants of {@link Set}'s of {@link SzFlag} instance to the
    /// actual {@link Set} of {@link SzFlag} instances.
    /// </summary>
    private IDictionary<string, SzFlag> flagsMap = new Dictionary<string, SzFlag>();

    [OneTimeSetUp]
    public void ReflectFlags() {
        Type nativeFlagsType    = typeof(NativeFlags);
        Type flagsType          = typeof(SzFlags);
        Type enumType           = typeof(SzFlag);
        foreach (FieldInfo fieldInfo in nativeFlagsType.GetFields()) {
            if (fieldInfo.FieldType != typeof(long)) continue;
            if (!fieldInfo.Name.StartsWith("Sz")) continue;
            this.nativeFlagsMap.Add(fieldInfo.Name, ((long?) fieldInfo.GetValue(null)) ?? 0L);
        }

        BindingFlags    bindingFlags    = BindingFlags.Public | BindingFlags.Static;
        // populate the enums
        foreach (FieldInfo fieldInfo in enumType.GetFields(bindingFlags)) {
            this.enumsMap.Add(fieldInfo.Name, ((SzFlag?) fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags);
        }

        // populate the sets
        foreach (FieldInfo fieldInfo in flagsType.GetFields(bindingFlags)) {
            if (fieldInfo.FieldType != typeof(SzFlag)) continue;
            if (!fieldInfo.Name.StartsWith("Sz")) continue;
            
            this.flagsMap.Add(fieldInfo.Name, ((SzFlag?) fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags);
        }
    }

    private static IList<(string, long)> GetNativeFlagMappings() {
        Type            nativeFlagsType = typeof(NativeFlags);

        IList<(string, long)> results = new List<(string, long)>();

        foreach (FieldInfo fieldInfo in nativeFlagsType.GetFields(BindingFlags.Static)) {
            if (fieldInfo.FieldType != typeof(long)) continue;
            if (!fieldInfo.Name.StartsWith("Sz")) continue;
            
            results.Add((fieldInfo.Name, ((long?) fieldInfo.GetValue(null)) ?? 0L));
        }
        return results;
    }

    private static IList<(string, SzFlag)> GetEnumMappings() {
        Type enumType           = typeof(SzFlag);
        BindingFlags    bindingFlags    = BindingFlags.Public | BindingFlags.Static;

        IList<(string, SzFlag)> results = new List<(string, SzFlag)>();
        foreach (FieldInfo fieldInfo in enumType.GetFields(bindingFlags)) {
            results.Add((fieldInfo.Name, ((SzFlag?) fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags ));
        }
        return results;
    }

    private static IList<(string, SzFlag)> GetFlagsMappings() {
        Type flagsType          = typeof(SzFlags);
        BindingFlags    bindingFlags    = BindingFlags.Public | BindingFlags.Static;

        IList<(string, SzFlag)> results = new List<(string, SzFlag)>();

        // populate the sets
        foreach (FieldInfo fieldInfo in flagsType.GetFields(bindingFlags)) {
            if (fieldInfo.FieldType != typeof(SzFlag)) continue;
            if (!fieldInfo.Name.StartsWith("Sz")) continue;
            
            results.Add((fieldInfo.Name, ((SzFlag?) fieldInfo.GetValue(null)) ?? SzFlags.SzNoFlags));
        }

        return results;

    }

    [Test, TestCaseSource(nameof(GetNativeFlagMappings))]
    public void TestNativeFlag((string flagName, long value) args) {
        this.PerformTest(() => {
            string flagName = args.flagName;
            long   value    = args.value;

            Assert.That(this.enumsMap.ContainsKey(flagName) 
                        || this.flagsMap.ContainsKey(flagName),
                "SDK flag constant (" + flagName +") not found for "
                + "native flag constant.");
            if (this.enumsMap.ContainsKey(flagName)) {
                SzFlag enumValue = this.enumsMap[flagName];
                Assert.That(value, Is.EqualTo((long) enumValue),
                            "Enum flag constant (" + flagName
                            + ") has different value ("
                            + Utilities.HexFormat((long) enumValue)
                            + ") than native flag constant: " 
                            + Utilities.HexFormat((long) value));
            } else {
                SzFlag flagsValue = this.flagsMap[flagName];
                Assert.That(value, Is.EqualTo((long) flagsValue),
                            "Flag set constant (" + flagName
                            + ") has different value ("
                            + Utilities.HexFormat((long) flagsValue)
                            + ") than native flag constant: " 
                            + Utilities.HexFormat((long) value));
            }
        });
    }

    [Test, TestCaseSource(nameof(GetFlagsMappings))]
    public void TestFlagsConstant((string name, SzFlag value) args) {
        this.PerformTest(() => {
            string  name    = args.name;
            SzFlag  value   = args.value;
            if (name.EndsWith("AllFlags")) {
                int length          = name.Length;
                string prefix       = name.Substring(0, length - "AllFlags".Length);
                string groupName    = prefix + "Flags";
                
                SzFlagUsageGroup? parsedGroup = null;
                
                try {
                    parsedGroup = (SzFlagUsageGroup?) 
                        Enum.Parse(typeof(SzFlagUsageGroup), groupName, false);

                } catch (Exception e) {
                    Fail("Failed to get SzFlagUsageGroup for AllFlags set: "
                        + "set=[ " + name + "], group=[ " + groupName + "]", e);
                }
                SzFlagUsageGroup group = (parsedGroup ?? ((SzFlagUsageGroup) 0L));

                SzFlag groupFlagsValue = SzFlags.GetFlags(group);
                Assert.That(groupFlagsValue, Is.EqualTo(value),
                            "Value for group (" + group + ") has a different "
                            + "primitive long value ("
                            + Utilities.HexFormat((long) groupFlagsValue)
                            + " / " + group.FlagsToString(SzFlags.GetFlags(group)) 
                            + ") than expected (" + Utilities.HexFormat((long) value) 
                            + "): " + name);
            
            } else if (!nameof(SzFlags.SzNoFlags).Equals(name)) {
                Assert.IsTrue(this.nativeFlagsMap.ContainsKey(name),
                    "Primitive long flag constant not found for "
                    + "aggregate enum constant: " + name);
                long nativeValue = this.nativeFlagsMap[name];
                Assert.That(nativeValue, Is.EqualTo((long) value),
                    "Native flag constant (" + name +") has a different primitive "
                    + "long value (" + Utilities.HexFormat(nativeValue) 
                    + ") than enum flag constant (" + name + "): "
                    + Utilities.HexFormat((long) value));
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestEnumFlag((string name, SzFlag value) args) {
        this.PerformTest(() => {
            string  name    = args.name;
            SzFlag  value   = args.value;

            if (name.Equals(nameof(SzFlag.SzWithInfo))) return;

            Assert.IsTrue(this.nativeFlagsMap.ContainsKey(name),
                        "Primitive long flag constant not found for "
                        + "enum flag constant: " + name);
            
            long nativeValue = this.nativeFlagsMap[name];
            Assert.That(nativeValue, Is.EqualTo((long) value), 
                        "Flag constant (" + name +") has a different primitive "
                        + "long value (" + Utilities.HexFormat(nativeValue) 
                        + ") than enum flag constant (" + name + "): "
                        + Utilities.HexFormat((long) value));
        });
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestNamedMappings((string name, SzFlag value) args) {
        this.PerformTest(() => {
            string  name    = args.name;
            SzFlag  value   = args.value;

            ReadOnlyDictionary<string,SzFlag> flagsByName = SzFlags.GetFlagsByName();

            Assert.IsTrue(flagsByName.ContainsKey(name),
                        "SzFlag symbolic name not found in named flags: " + name);
            
            Assert.That(flagsByName[name], Is.EqualTo(value), 
                        "Flag constant (" + name + ") has a different value  (" 
                        + Utilities.HexFormat((long) flagsByName[name]) 
                        + ") than enum flag constant (" + name + "): "
                        + Utilities.HexFormat((long) value));
        });
    }

    [Test]
    public void TestGetGroupsUnrecognized() {
        this.PerformTest(() => {
            try {
                SzFlagUsageGroup groups = SzFlags.GetGroups("UNRECOGNIZED");

                Fail("Unexpected succeeded in getting groups for an unrecognized "
                    + "flag name.");
                
            } catch (ArgumentException) {
                // expected
            } catch (Exception e) {
                Fail("Got an unexpected exception while getting groups "
                    + "for an unrecognized flag name", e);
            }
        });
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestGetGroups((string name, SzFlag flag) args) {
        this.PerformTest(() => {
            string name = args.name;
            SzFlag flag = args.flag;

            SzFlagUsageGroup groups = SzFlags.GetGroups(name);

            foreach (SzFlagUsageGroup group in Enum.GetValues(typeof(SzFlagUsageGroup))) {
                IDictionary<string,SzFlag> flagsByName = SzFlags.GetFlagsByName(group);
                IDictionary<SzFlag,string> namesByFlag = SzFlags.GetNamesByFlag(group);
                SzFlag flags = SzFlags.GetFlags(group);

                // check if this group is included in groups by flags
                if ((group & groups) == 0L) {
                    Assert.IsFalse(flagsByName.ContainsKey(name),
                                "Groups by flag name (" + name 
                                    + ") does not contain group ("
                                    + group  + ") but group contains flag (" 
                                    + name + " / " + flag.FlagsToString() + ")");
            
                } else {
                    Assert.IsTrue(flagsByName.ContainsKey(name),
                                "Groups by flag name (" + name 
                                + ") contains group ("
                                + group  + ") but group flags by name "
                                + "does not contains flag (" 
                                + name + " / " + flag.FlagsToString() + ")");
                    Assert.IsTrue(namesByFlag.ContainsKey(flag),
                                "Groups by flag name (" + name 
                                + ") contains group ("
                                + group  + ") but group flag names by flag "
                                + "does not contains flag (" 
                                + name + " / " + flag.FlagsToString() + ")");
                    Assert.That((long) (flags & flag), Is.Not.EqualTo(0L),
                                "Groups by flag name (" + name 
                                + ") contains group (" + group 
                                + " ) but group does not contain flag (" 
                                + name + " / " + group.FlagsToString(flag) + ")");                
                }
            }
        });
    }

    private static IList<(SzFlag?,string)> GetToFlagStringParams() {
        IList<(SzFlag?,string)> results = new List<(SzFlag?,string)>();
        results.Add((null, "{ NONE } [0000 0000 0000 0000]"));
        results.Add((SzFlags.SzNoFlags, "{ NONE } [0000 0000 0000 0000]"));

        StringBuilder sb = new StringBuilder(300);
        IDictionary<string,SzFlag> ambiguousFlags
            = new Dictionary<string,SzFlag>();

        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludeResolved), SzFlag.SzSearchIncludeResolved);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludePossiblySame), SzFlag.SzSearchIncludePossiblySame);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludePossiblyRelated), SzFlag.SzSearchIncludePossiblyRelated);
        ambiguousFlags.Add(nameof(SzFlag.SzSearchIncludeNameOnly), SzFlag.SzSearchIncludeNameOnly);

        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludeMultiRecordEntities), SzFlag.SzExportIncludeMultiRecordEntities);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludePossiblySame), SzFlag.SzExportIncludePossiblySame);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludePossiblyRelated), SzFlag.SzExportIncludePossiblyRelated);
        ambiguousFlags.Add(nameof(SzFlag.SzExportIncludeNameOnly), SzFlag.SzExportIncludeNameOnly);
        
        IDictionary<SzFlag,List<string>> ambiguousNames
            = new Dictionary<SzFlag,List<string>>();
        
        foreach (KeyValuePair<string,SzFlag> pair in ambiguousFlags) {
            SzFlag flag = pair.Value;
            string name = pair.Key;
            if (!ambiguousNames.ContainsKey(flag)) {
                ambiguousNames[flag] = new List<string>();
            }
            ambiguousNames[flag].Add(name);
        }
        foreach (KeyValuePair<SzFlag,List<string>> pair in ambiguousNames) {
            pair.Value.Sort();
        }

        List<SzFlag> flags = new List<SzFlag>();
        foreach (SzFlag flag in Enum.GetValues(typeof(SzFlag))) {
            flags.Add(flag);
        }
        flags.Sort();
        int start = 0;
        for (int loop = 0; loop < 3; loop++) {
            for (int count = 1; count < 10; count++) {
                // clear the string builder
                sb.Clear();

                // initialize the flags
                SzFlag aggregateFlags = SzFlags.SzNoFlags;
                
                // reset the start if need be
                if (start > flags.Count - count) {
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
                    if (ambiguousNames.ContainsKey(flag)) {
                        sb.Append("{ ");
                        string prefix2 = "";    
                        foreach (string name in ambiguousNames[flag]) {
                            sb.Append(prefix2);
                            sb.Append(name);
                            prefix2 = " / ";
                        }
                        sb.Append(" }");

                    } else {
                        sb.Append(flag.ToString());
                    }
                    prefix = " | ";
                }
                sb.Append(" [");
                sb.Append(Utilities.HexFormat((long) aggregateFlags));
                sb.Append("]");

                String expected = sb.ToString();
                results.Add((aggregateFlags, expected));
            }
        }

        return results;
    }

    [Test]
    public void TestZeroGetFlags() {
        this.PerformTest(() => {
            try {
                SzFlag flags = SzFlags.GetFlags(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get flags for "
                    + "SzNoFlagUsageGroups: " + SzFlags.FlagsToString(flags));

            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test]
    public void TestZeroGetFlagsByName() {
        this.PerformTest(() => {
            try {
                ReadOnlyDictionary<string,SzFlag> flagsByName
                    = SzFlags.GetFlagsByName(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get flags by name for "
                    + "SzNoFlagUsageGroups: " + flagsByName);
                    
            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test]
    public void TestZeroGetNamesByFlag() {
        this.PerformTest(() => {
            try {
                ReadOnlyDictionary<SzFlag,string> namesByFlag
                    = SzFlags.GetNamesByFlag(SzFlags.SzNoFlagUsageGroups);

                Fail("Was unexpectedly able to get names by flag for "
                    + "SzNoFlagUsageGroups: " + namesByFlag);
                    
            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetFlags() {
        this.PerformTest(() => {
            try {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags 
                    | SzFlagUsageGroup.SzSearchFlags;

                SzFlag flags = SzFlags.GetFlags(aggregateGroups);

                Fail("Was unexpectedly able to get flags for "
                    + "aggregate group: " + SzFlags.FlagsToString(flags));

            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetFlagsByName() {
        this.PerformTest(() => {
            try {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags 
                    | SzFlagUsageGroup.SzSearchFlags;
                
                ReadOnlyDictionary<string,SzFlag> flagsByName
                    = SzFlags.GetFlagsByName(aggregateGroups);

                Fail("Was unexpectedly able to get flags by name for "
                    + "aggregate group: " + flagsByName);
                    
            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test]
    public void TestAggregateGetNamesByFlag() {
        this.PerformTest(() => {
            try {
                SzFlagUsageGroup aggregateGroups
                    = SzFlagUsageGroup.SzEntityFlags 
                    | SzFlagUsageGroup.SzSearchFlags;
                
                ReadOnlyDictionary<SzFlag,string> namesByFlag
                    = SzFlags.GetNamesByFlag(aggregateGroups);

                Fail("Was unexpectedly able to get names by flag for "
                    + "aggregate group: " + namesByFlag);
                    
            } catch (ArgumentException) {
                // expected
            }
        });
    }

    [Test, TestCaseSource(nameof(GetToFlagStringParams))]
    public void TestToFlagString((SzFlag? flag, string expected) args) {
        this.PerformTest(() => {
            SzFlag? flag        = args.flag;
            string  expected    = args.expected;
            string? actual      = SzFlags.FlagsToString(flag);
            Assert.That(actual, Is.EqualTo(expected),
                        "ToFlagString(this SzFlag) did not return as expected.  "
                        + "actual=[ " + actual + " ], expected=[ " + expected + " ]");
        });
    }

     private static IList<SzFlagUsageGroup> GetEnumFlagGroups() {
        Type groupType = typeof(SzFlagUsageGroup);
        IList<SzFlagUsageGroup> result = new List<SzFlagUsageGroup>();

        foreach (SzFlagUsageGroup group in Enum.GetValues(groupType)) {
            result.Add(group);
        }
        return result;
    }

    [Test, TestCaseSource(nameof(GetEnumFlagGroups))]
    public void TestZeroToString(SzFlagUsageGroup group) {
        this.PerformTest(() => {
            string text = group.FlagsToString(SzFlags.SzNoFlags);
            Assert.IsNotNull(text, "The result for zero is null");
            Assert.IsTrue(text.IndexOf("NONE") >= 0,
                        "The result for zero does not contain NONE");
        });
    }

    [Test, TestCaseSource(nameof(GetEnumFlagGroups))]
    public void TestGetFlags(SzFlagUsageGroup group) {
        this.PerformTest(() => {
            ReadOnlyDictionary<string,SzFlag> flagsByName
                = SzFlags.GetFlagsByName(group);
            ReadOnlyDictionary<SzFlag,string> namesByFlag
                = SzFlags.GetNamesByFlag(group);

            SzFlag flags = SzFlags.GetFlags(group);

            Assert.That(flags, Is.Not.EqualTo(SzFlags.SzNoFlags),
                        "Flags for group should not be zero: " + group);

            SzFlag aggregateFlags = SzFlags.SzNoFlags;
            foreach (KeyValuePair<string,SzFlag> pair in flagsByName) {
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
            
            foreach (KeyValuePair<string,SzFlag> pair in flagsByName) {
                Assert.IsTrue(namesByFlag.ContainsKey(pair.Value),
                    "Flag value not found in names by flag map for group, "
                    + "but flag is present in flags by name map for group.  "
                    + "group=[ " + group + " ], flagName=[ " + pair.Key 
                    + " ], flagValue=[ " + Utilities.HexFormat((long) pair.Value)
                    + " ], flagsByName=[ " + flagsByName + " ], namesByFlag=[ "
                    + namesByFlag + " ]");
                Assert.That(namesByFlag[pair.Value], Is.EqualTo(pair.Key),
                    "Flag name for value in names by flag map for group "
                    + "does not match the association in the flags by name map "
                    + "for the group.  group=[ " + group + " ], flagName=[ " 
                    + pair.Key + " ], flagValue=[ "
                    + Utilities.HexFormat((long) pair.Value) + " ], flagsByName=[ "
                    + flagsByName + " ], namesByFlag=[ " + namesByFlag + " ]");
            }

            foreach (KeyValuePair<SzFlag,string> pair in namesByFlag) {
                Assert.IsTrue(flagsByName.ContainsKey(pair.Value),
                    "Flag name not found in flags by name map for group, "
                    + "but flag is present in names by flag map for group.  "
                    + "group=[ " + group + " ], flagName=[ " + pair.Value 
                    + " ], flagValue=[ " + Utilities.HexFormat((long) pair.Key)
                    + " ], flagsByName=[ " + flagsByName + " ], namesByFlag=[ "
                    + namesByFlag + " ]");
                Assert.That(flagsByName[pair.Value], Is.EqualTo(pair.Key),
                    "Flag value for name in flags by name map for group "
                    + "does not match the association in the names by flag map "
                    + "for the group.  group=[ " + group + " ], flagName=[ " 
                    + pair.Value + " ], flagValue=[ "
                    + Utilities.HexFormat((long) pair.Key) + " ], flagsByName=[ "
                    + flagsByName + " ], namesByFlag=[ " + namesByFlag + " ]");
            }

            ReadOnlyDictionary<string,SzFlag> allFlags = SzFlags.GetFlagsByName();

            foreach (KeyValuePair<string, SzFlag> pair in allFlags) {
                string name = pair.Key;
                SzFlag flag = pair.Value;
                SzFlagUsageGroup groups = SzFlags.GetGroups(name);

                Assert.That((long) groups, Is.Not.EqualTo(0L),
                            "Groups for flag should not be zero: " + name);

                if (flagsByName.ContainsKey(name)) {
                    Assert.IsTrue((flags & flag) != SzFlags.SzNoFlags,
                        "Flag name (" + name + ") is in named flags for "
                        + "group (" + group + "), but value is not "
                        + "in the aggregate flag values.");
                    Assert.That((long) (groups & group), Is.Not.EqualTo(0L),
                        "Group (" + group + ") has flag (" + name + ") but the "
                        + "flag does not have the group.  flagsForGroup=[ "
                        + flagsByName.Keys + " / " + SzFlags.FlagsToString(flags)
                        + " ], groupsForFlag=[ " + groups + "]");
                } else {
                    Assert.That((long) (groups & group), Is.EqualTo(0L),
                        "Group (" + group + ") has flag (" + name + ") but the "
                        + "flag does not have the group.  flagsForGroup=[ "
                        + flagsByName.Keys + " / " + SzFlags.FlagsToString(flags)
                        + " ], groupsForFlag=[ " + groups + "]");
                }
            }
        });
    }

    [Test]
    [TestCase("SzSearchIncludeNameOnly",SzFlag.SzSearchIncludeNameOnly)]
    [TestCase("SzSearchIncludeResolved",SzFlag.SzSearchIncludeResolved)]
    [TestCase("SzSearchIncludePossiblySame",SzFlag.SzSearchIncludePossiblySame)]
    [TestCase("SzSearchIncludePossiblyRelated",SzFlag.SzSearchIncludePossiblyRelated)]
    [TestCase("SzExportIncludeNameOnly",SzFlag.SzExportIncludeNameOnly)]
    [TestCase("SzExportIncludeMultiRecordEntities",SzFlag.SzExportIncludeMultiRecordEntities)]
    [TestCase("SzExportIncludePossiblySame",SzFlag.SzExportIncludePossiblySame)]
    [TestCase("SzExportIncludePossiblyRelated",SzFlag.SzExportIncludePossiblyRelated)]
    public void TestGetAmbiguousNamedFlags(String name, SzFlag value) {
        this.PerformTest(() => {
            ReadOnlyDictionary<string,SzFlag> flagsByName = SzFlags.GetFlagsByName();
            Assert.IsTrue(flagsByName.ContainsKey(name),
                "Flag not found in named flags: " + name);
            SzFlag actualValue = flagsByName[name];
            Assert.That(actualValue, Is.EqualTo(value),
                "Named flags value (" + actualValue + ") for " + name 
                + " does not match expected value: " + value);
        });
    }


    private static IList<(SzFlagUsageGroup,SzFlag?,ISet<string>)> GetGroupToStringParameters() {
        IList<(SzFlagUsageGroup,SzFlag?,ISet<string>)> result
            = new List<(SzFlagUsageGroup,SzFlag?,ISet<string>)>();

        Type groupType = typeof(SzFlagUsageGroup);
        Type flagType  = typeof(SzFlag);
        
        ReadOnlyDictionary<string,SzFlag> allFlags = SzFlags.GetFlagsByName();
        List<(string,SzFlag)> allFlagsList = new List<(string,SzFlag)>();
        foreach (KeyValuePair<string,SzFlag> pair in allFlags) {
            allFlagsList.Add((pair.Key,pair.Value));
        }

        result.Add((SzFlags.SzNoFlagUsageGroups,
                    (SzFlag.SzEntityIncludeAllFeatures | SzFlag.SzIncludeFeatureScores),
                    SetOf(Utilities.HexFormat((long) SzFlag.SzEntityIncludeAllFeatures),
                         Utilities.HexFormat((long) SzFlag.SzIncludeFeatureScores))));

        result.Add(((SzFlagUsageGroup.SzEntityFlags | SzFlagUsageGroup.SzSearchFlags),
                    (SzFlag.SzEntityIncludeEntityName | SzFlag.SzSearchIncludeNameOnly),
                    SetOf(Utilities.HexFormat((long) SzFlag.SzEntityIncludeEntityName),
                         Utilities.HexFormat((long) SzFlag.SzSearchIncludeNameOnly))));
                         
        foreach (SzFlagUsageGroup group in Enum.GetValues(groupType)) {
            SzFlag groupFlags = SzFlags.GetFlags(group);

            ReadOnlyDictionary<string,SzFlag> flagsByName = SzFlags.GetFlagsByName(group);
            ReadOnlyDictionary<SzFlag,string> namesByFlag = SzFlags.GetNamesByFlag(group);
            
            List<(string,SzFlag)> namedFlagList = new List<(string,SzFlag)>(flagsByName.Count);
            foreach (KeyValuePair<string,SzFlag> pair in flagsByName) {
                namedFlagList.Add((pair.Key,pair.Value));
            }

            result.Add((group, null, SetOf("{ NONE }")));
            result.Add((group, SzFlags.SzNoFlags, SetOf("{ NONE }")));

            int start = 0;
            for (int loop = 0; loop < 3; loop++) {
                for (int count = 1; count < 10; count++) {
                    ISet<string> expected = new HashSet<string>();
                                    
                    // reset the start if need be
                    if (start > allFlagsList.Count - count) {
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

                        if (flagsByName.ContainsKey(name)) {
                            expected.Add(name);

                        } else if ((groupFlags & flag) != SzFlags.SzNoFlags) {
                            expected.Add(namesByFlag[flag]);

                        } else {
                            expected.Add(Utilities.HexFormat((long) flag));
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
        SzFlagUsageGroup    group       = args.group;
        SzFlag?             flags       = args.flags;
        ISet<string>        expected    = args.expected;
        string testData = "group=[ " + group + " ], flags=[ " + flags + 
                " ], expected=[ " + expected + " ]";
        
        string[] separators = [ " | ", " ["];
        this.PerformTest(() => {
            string actual = group.FlagsToString(flags);
            Assert.IsNotNull(actual, 
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) was null: " + testData);
            Assert.That(actual.Length, Is.Not.EqualTo(0),
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) was empty: " + testData);

            Assert.IsTrue(actual.EndsWith("]"),
                "Result from SzFlagUsageGroup.FlagsToString(SzFlag) does not end with ']': "
                + "actual=[ " + actual + " ], " + testData);
            
            // trim the trailing bracket
            string[] tokens = actual.Substring(0, actual.Length - 1)
                .Split(separators, StringSplitOptions.None);

            Assert.That(tokens[tokens.Length - 1], 
                        Is.EqualTo(Utilities.HexFormat((long) (flags ?? SzFlags.SzNoFlags))),
                        "Hex flag representation is not as expected: " + testData);
            
            ISet<string> actualSet = new HashSet<string>();
            for (int index = 0; index < tokens.Length - 1; index++) {
                actualSet.Add(tokens[index]);
            }

            Assert.That(actualSet.Count, Is.EqualTo(expected.Count),
                "Actual token count does not match expected token count.  actualTokens=[ "
                + actualSet + " ], " + testData);
            
            foreach (string token in actualSet) {
                Assert.IsTrue(expected.Contains(token),
                    "Actual tokens contains unexpected token (" + token + "): " 
                    + "actualTokens=[ " + actualSet + " ], " + testData);
            }
            foreach (string token in expected) {
                Assert.IsTrue(actualSet.Contains(token),
                    "Expected token (" + token + ") missing from actual tokens: "
                    + "actualTokens=[ " + actualSet + " ], " + testData);
            }
        });
    }

    [Test]
    [TestCaseSource(nameof(GetEnumMappings))]
    [TestCaseSource(nameof(GetFlagsMappings))]
    public void TestFlagsToLong((string name, SzFlag flag) args) {
        string name = args.name;
        SzFlag flag = args.flag;
        long value = SzFlags.FlagsToLong(flag);
        Assert.That(value, Is.EqualTo((long) flag),
            "FlagsToLong not as expected for flags: " + name);
    }

    [Test]
    public void TestNullFlagsToLong() {
        long value = SzFlags.FlagsToLong(null);
        Assert.That(value, Is.EqualTo(0L),
            "FlagsToLong not as expected for null flags");
    }
}
