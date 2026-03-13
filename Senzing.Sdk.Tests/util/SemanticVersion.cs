namespace Senzing.Sdk.Tests.Util;

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Represents a semantic version described as a <c>string</c> containing
/// integer numbers separated by decimal points (e.g.: "1.4.5"), optionally
/// followed by a pre-release suffix of <c>"-alpha"</c>, <c>"-beta"</c>,
/// or <c>"-rc"</c> with additional dot-separated version parts (e.g.:
/// "2.0.0-alpha.2.0", "2.0.0-beta.3.2", "2.0.0-rc.2.1").  Pre-release
/// suffixes are normalized to lowercase on construction (e.g.: <c>"RC"</c>
/// becomes <c>"rc"</c>).
/// </summary>
internal sealed class SemanticVersion : IComparable<SemanticVersion>
{
    /// <summary>
    /// The value used to represent a release candidate pre-release suffix.
    /// </summary>
    private const int RcValue = -1;

    /// <summary>
    /// The value used to represent a beta pre-release suffix.
    /// </summary>
    private const int BetaValue = -2;

    /// <summary>
    /// The value used to represent an alpha pre-release suffix.
    /// </summary>
    private const int AlphaValue = -3;

    /// <summary>
    /// The separator characters used to split the version string.
    /// </summary>
    private static readonly char[] Separators = new char[] { '-', '.' };

    /// <summary>
    /// A read-only dictionary of pre-release integer sentinel values to
    /// their corresponding string suffix representations for use in
    /// reconstructing the version string via <see cref="ToString()"/>.
    /// </summary>
    private static readonly Dictionary<int, string> SuffixMap
        = new Dictionary<int, string>
        {
            { RcValue,    "-rc" },
            { BetaValue,  "-beta" },
            { AlphaValue, "-alpha" }
        };

    /// <summary>
    /// The <c>List</c> of version parts.
    /// </summary>
    private readonly List<int> versionParts;

    /// <summary>
    /// The version string representation.
    /// </summary>
    private readonly string versionString;

    /// <summary>
    /// The normalized version of the <c>string</c> for calculating the
    /// hash code.
    /// </summary>
    private readonly string normalizedString;

    /// <summary>
    /// Constructs with the specified version string (e.g.: "1.4.5").
    /// Pre-release suffixes (<c>"alpha"</c>, <c>"beta"</c>, <c>"rc"</c>)
    /// are supported and normalized to lowercase (e.g.: <c>"RC"</c> becomes
    /// <c>"rc"</c>).
    /// </summary>
    ///
    /// <param name="versionString">
    /// The version string with which to construct.
    /// </param>
    ///
    /// <exception cref="ArgumentNullException">
    /// If the specified parameter is <c>null</c>.
    /// </exception>
    ///
    /// <exception cref="ArgumentException">
    /// If the specified parameter is not properly formatted.
    /// </exception>
    public SemanticVersion(string versionString)
    {
        ArgumentNullException.ThrowIfNull(
            versionString, "Version string cannot be null");

        try
        {
            string[] tokens = versionString.Split(Separators);
            this.versionParts = new List<int>(tokens.Length);
            bool hasSuffix = false;
            foreach (string token in tokens)
            {
                string upperToken = token.ToUpperInvariant();
                switch (upperToken)
                {
                    case "ALPHA":
                    case "BETA":
                    case "RC":
                        if (hasSuffix)
                        {
                            throw new ArgumentException(
                                "Multiple pre-release suffixes are not "
                                + "allowed: " + versionString);
                        }
                        if (this.versionParts.Count == 0)
                        {
                            throw new ArgumentException(
                                "Pre-release suffix must follow at least "
                                + "one numeric version part: "
                                + versionString);
                        }
                        hasSuffix = true;
                        this.versionParts.Add(
                            upperToken == "ALPHA" ? AlphaValue
                            : upperToken == "BETA" ? BetaValue
                            : RcValue);
                        break;
                    default:
                        int part = Convert.ToInt32(token, 10);
                        if (part < 0)
                        {
                            throw new ArgumentOutOfRangeException(
                                "Negative version part is not allowed: " + part);
                        }
                        this.versionParts.Add(part);
                        break;
                }
            }

            // create a normalized list of version parts by removing trailing zeroes
            int count = 0;
            for (int index = this.versionParts.Count - 1; index >= 0; index--)
            {
                if (this.versionParts[index] != 0)
                {
                    break;
                }
                count++;
            }
            List<int> normalized = new List<int>(this.versionParts.Count - count);
            for (int index = 0; index < this.versionParts.Count - count; index++)
            {
                normalized.Add(this.versionParts[index]);
            }

            // set the version strings
            this.versionString = BuildVersionString(this.versionParts);
            this.normalizedString = BuildVersionString(normalized);
        }
        catch (Exception e) when (e is not ArgumentException)
        {
            throw new ArgumentException(
                "Invalid semantic version string: " + versionString, e);
        }
    }

    /// <summary>
    /// Builds a version string from the specified list of version parts,
    /// converting pre-release sentinel values back to their string suffixes.
    /// </summary>
    ///
    /// <param name="parts">The list of version parts.</param>
    /// <returns>The version string.</returns>
    private static string BuildVersionString(List<int> parts)
    {
        StringBuilder sb = new StringBuilder();
        string prefix = "";
        foreach (int part in parts)
        {
            if (SuffixMap.TryGetValue(part, out string? suffix))
            {
                sb.Append(suffix);
                prefix = ".";
            }
            else
            {
                sb.Append(prefix).Append(part);
                prefix = ".";
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Overridden to return <c>true</c> if and only if the specified parameter
    /// is a non-null reference to an object of the same class with equivalent
    /// version parts.
    /// </summary>
    ///
    /// <param name="obj">The object to compare with.</param>
    ///
    /// <returns>
    /// <c>true</c> if the objects are equal, otherwise <c>false</c>
    /// </returns>
    public override sealed bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (this == obj)
        {
            return true;
        }
        if (this.GetType() != obj.GetType())
        {
            return false;
        }
        SemanticVersion version = (SemanticVersion)obj;
        return (this.CompareTo(version) == 0);
    }

    /// <summary>
    /// Implemented to return a hash code that is consistent with the
    /// <see cref="Equals"/> implementation.
    /// </summary>
    ///
    /// <returns>The hash code for this instance.</returns>
    public override sealed int GetHashCode()
    {
        return this.normalizedString.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Implemented to compare the parts of the semantic version in order.
    /// </summary>
    public int CompareTo(SemanticVersion? other)
    {
        ArgumentNullException.ThrowIfNull(
            other, "The specified parameter cannot be null");

        // iterate over the parts
        int max = Math.Max(this.versionParts.Count, other.versionParts.Count);
        for (int index = 0; index < max; index++)
        {
            int part1 = (index < this.versionParts.Count)
                ? this.versionParts[index] : 0;
            int part2 = (index < other.versionParts.Count)
                ? other.versionParts[index] : 0;

            // compare the parts
            int diff = part1.CompareTo(part2);

            // if the diff is non-zero then return it
            if (diff != 0)
            {
                return diff;
            }
        }

        // if we have exhausted all version parts without a difference then we have
        // equality
        return 0;
    }

    /// <summary>
    /// Returns the version string for this instance. This is equivalent to
    /// calling <see cref="ToString(bool)"/> with <c>false</c> as the parameter.
    /// </summary>
    ///
    /// <returns>The version string for this instance.</returns>
    public override string ToString()
    {
        return this.ToString(false);
    }

    /// <summary>
    /// Returns a version string for this instance that is optionally normalized
    /// to remove trailing zeroes.
    /// </summary>
    ///
    /// <param name="normalized">
    /// <c>true</c> if trailing zeroes should be stripped, otherwise <c>false</c>.
    /// </param>
    ///
    /// <returns>
    /// A <c>string</c> representation of this instance that is optionally
    /// normalized to remove trailing zeroes.
    /// </returns>
    public string ToString(bool normalized)
    {
        return (normalized) ? this.normalizedString : this.versionString;
    }

}
