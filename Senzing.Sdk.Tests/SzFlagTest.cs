namespace Senzing.Sdk.Tests;

using NUnit.Framework;
using Senzing.Sdk;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.Core;
using System;
using System.Text;
using System.Reflection;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzFlagTest : AbstractTest {
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
    }

    [Test, TestCaseSource(nameof(GetFlagsMappings))]
    public void TestFlagsConstant((string name, SzFlag value) args) {
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

            SzFlag groupValue = group.GetFlags();
            Assert.That(groupValue, Is.EqualTo(value),
                         "Value for group (" + group + ") has a different "
                        + "primitive long value (" + Utilities.HexFormat((long) groupValue)
                        + " / " + group.ToString(group.GetFlags()) 
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
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestEnumFlag((string name, SzFlag value) args) {
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
    }

    [Test, TestCaseSource(nameof(GetEnumMappings))]
    public void TestGetGroups((string name, SzFlag flag) args) {
        string name = args.name;
        SzFlag flag = args.flag;

        SzFlagUsageGroup groups1 = flag.GetGroups();
        SzFlagUsageGroup groups2 = SzFlags.GetGroups(name);

        foreach (SzFlagUsageGroup group in Enum.GetValues(typeof(SzFlagUsageGroup))) {
            SzFlag flags = group.GetFlags();

            // check if this group is included in groups by flags
            if ((group & groups1) == 0L) {
                Assert.That((long) (flags & flag), Is.EqualTo(0L),
                            "Groups by flag does not contain group (" + group 
                            + " ) but group contains flag (" 
                            + name + " / " + flag.ToFlagString() + ")");
                Assert.That((long) (group & groups2), Is.EqualTo(0L),
                            "Groups by flag (" + groups1 + ") does not contain group (" + group
                            + ") but groups by name (" + groups2 + ") does: " 
                            + name + " / " + flag.ToFlagString() + ")");
        
            } else {
                Assert.That((long) (flags & flag), Is.Not.EqualTo(0L),
                            "Groups by flag contains group (" + group 
                            + " ) but group does not contain flag (" 
                            + name + " / " + group.ToString(flag) + ")");                
            }

            // check if this group is included in groups by name
            if ((group & groups2) != 0L) {
                Assert.That((long) (flags & flag), Is.Not.EqualTo(0L),
                            "Groups by name contains group (" + group 
                            + " ) but group does not contain flag (" 
                            + name + " / " + group.ToString(flag) + ")");
                
                Assert.That((long) (group & groups1), Is.Not.EqualTo(0L),
                            "Groups by name (" + groups2 + ") contains group ("
                            + group + ") but groups by flag (" + groups1 + ") does not: " 
                            + name + " / " + flag.ToFlagString() + ")");
            }
        }
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

    [Test, TestCaseSource(nameof(GetToFlagStringParams))]
    public void TestToFlagString((SzFlag? flag, string expected) args) {
        SzFlag? flag        = args.flag;
        string  expected    = args.expected;
        string? actual      = SzFlags.ToString(flag);
        Assert.That(actual, Is.EqualTo(expected),
                    "ToFlagString(this SzFlag) did not return as expected.  "
                    + "actual=[ " + actual + " ], expected=[ " + expected + " ]");
    }
}
