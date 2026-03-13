namespace Senzing.Sdk.Tests;

using System;
using System.Reflection;

using Senzing.Sdk;
using Senzing.Sdk.Tests.Util;

/// <summary>
/// Provides utility methods for flag-related tests to determine
/// the <see cref="SinceAttribute"/> version of flag constants
/// via reflection.
/// </summary>
internal static class SzFlagTestUtilities
{
    /// <summary>
    /// The default <see cref="SemanticVersion"/> assumed for flags
    /// without a <see cref="SinceAttribute"/> annotation.
    /// </summary>
    public static readonly SemanticVersion DefaultSince
        = new SemanticVersion("4.0.0");

    /// <summary>
    /// Gets the <see cref="SemanticVersion"/> from the
    /// <see cref="SinceAttribute"/> on the specified <see cref="SzFlags"/>
    /// field, or <see cref="DefaultSince"/> if no annotation is present.
    /// </summary>
    ///
    /// <param name="fieldName">
    /// The name of the field on <see cref="SzFlags"/>.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="SemanticVersion"/> from the
    /// <see cref="SinceAttribute"/>, or <see cref="DefaultSince"/>
    /// if not annotated.
    /// </returns>
    public static SemanticVersion GetSinceVersion(string fieldName)
    {
        try
        {
            Type flagsType = typeof(SzFlags);
            FieldInfo? field = flagsType.GetField(
                fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return DefaultSince;
            }
            object[] attrs = field.GetCustomAttributes(
                typeof(SinceAttribute), false);
            if (attrs.Length > 0)
            {
                return new SemanticVersion(((SinceAttribute)attrs[0]).version);
            }
        }
        catch (Exception)
        {
            // fall through
        }
        return DefaultSince;
    }

    /// <summary>
    /// Gets the <see cref="SemanticVersion"/> from the
    /// <see cref="SinceAttribute"/> on the specified <see cref="SzFlag"/>
    /// field (enum constant), or <see cref="DefaultSince"/> if no
    /// annotation is present.
    /// </summary>
    ///
    /// <param name="name">
    /// The name of the field on <see cref="SzFlag"/>.
    /// </param>
    ///
    /// <returns>
    /// The <see cref="SemanticVersion"/> from the
    /// <see cref="SinceAttribute"/>, or <see cref="DefaultSince"/>
    /// if not annotated.
    /// </returns>
    public static SemanticVersion GetSinceVersionForSzFlag(string name)
    {
        try
        {
            Type enumType = typeof(SzFlag);
            FieldInfo? field = enumType.GetField(
                name, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return DefaultSince;
            }
            object[] attrs = field.GetCustomAttributes(
                typeof(SinceAttribute), false);
            if (attrs.Length > 0)
            {
                return new SemanticVersion(((SinceAttribute)attrs[0]).version);
            }
        }
        catch (Exception)
        {
            // fall through
        }
        return DefaultSince;
    }
}
