namespace Senzing.Sdk.Tests.Util;

using System;

using NUnit.Framework;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(scope: ParallelScope.All)]
internal class SemanticVersionTest
{
    // ---- Basic parsing tests ----

    [Test]
    [TestCase("1.4.5")]
    [TestCase("0.0.0")]
    [TestCase("10.20.30")]
    [TestCase("1.0")]
    [TestCase("3")]
    public void TestBasicParsing(string version)
    {
        SemanticVersion sv = new SemanticVersion(version);
        Assert.That(sv.ToString(), Is.EqualTo(version));
    }

    // ---- Pre-release parsing tests ----

    [Test]
    [TestCase("2.0.0-alpha", "2.0.0-alpha")]
    [TestCase("2.0.0-beta.3.2", "2.0.0-beta.3.2")]
    [TestCase("2.0.0-rc.2.1", "2.0.0-rc.2.1")]
    [TestCase("1.0.0-alpha", "1.0.0-alpha")]
    [TestCase("3.1.0-RC.1", "3.1.0-rc.1")]
    [TestCase("2.0.0-ALPHA.2", "2.0.0-alpha.2")]
    [TestCase("2.0.0-Beta.3", "2.0.0-beta.3")]
    public void TestPreReleaseParsing(string input, string expected)
    {
        SemanticVersion sv = new SemanticVersion(input);
        Assert.That(sv.ToString(), Is.EqualTo(expected));
    }

    // ---- ToString normalized tests ----

    [Test]
    [TestCase("1.4.0", "1.4")]
    [TestCase("1.0.0", "1")]
    [TestCase("2.0.0-alpha.2.0", "2.0.0-alpha.2")]
    [TestCase("2.0.0-beta.3.0", "2.0.0-beta.3")]
    [TestCase("2.0.0-rc.1", "2.0.0-rc.1")]
    public void TestToStringNormalized(string input, string expected)
    {
        SemanticVersion sv = new SemanticVersion(input);
        Assert.That(sv.ToString(true), Is.EqualTo(expected));
    }

    // ---- Equality tests ----

    [Test]
    [TestCase("1.4.5", "1.4.5")]
    [TestCase("1.4.0", "1.4")]
    [TestCase("1.0.0", "1")]
    [TestCase("2.0.0-alpha.2.0", "2.0.0-alpha.2.0")]
    [TestCase("2.0.0-alpha.2", "2.0.0-alpha.2.0")]
    [TestCase("2.0.0-beta.3.2", "2.0.0-beta.3.2")]
    [TestCase("2.0.0-rc.1", "2.0.0-rc.1")]
    public void TestEquality(string v1, string v2)
    {
        SemanticVersion sv1 = new SemanticVersion(v1);
        SemanticVersion sv2 = new SemanticVersion(v2);
        Assert.That(sv1, Is.EqualTo(sv2));
        Assert.That(sv1.GetHashCode(), Is.EqualTo(sv2.GetHashCode()));
    }

    [Test]
    [TestCase("2.0.0-alpha.1", "2.0.0-beta.1")]
    [TestCase("2.0.0-rc.1", "2.0.0")]
    [TestCase("1.0.0", "2.0.0")]
    [TestCase("2.0.0-alpha", "2.0.0-rc")]
    public void TestInequality(string v1, string v2)
    {
        SemanticVersion sv1 = new SemanticVersion(v1);
        SemanticVersion sv2 = new SemanticVersion(v2);
        Assert.That(sv1, Is.Not.EqualTo(sv2));
    }

    // ---- Ordering tests ----

    [Test]
    [TestCase("1.0.0", "2.0.0")]
    [TestCase("1.0.0", "1.1.0")]
    [TestCase("1.0.0", "1.0.1")]
    [TestCase("2.0.0-alpha.2.0", "2.0.0-beta.3.2")]
    [TestCase("2.0.0-beta.3.2", "2.0.0-rc.2.1")]
    [TestCase("2.0.0-rc.2.1", "2.0.0")]
    [TestCase("2.0.0-alpha.1", "2.0.0-alpha.2")]
    [TestCase("2.0.0-rc.1", "2.0.0-rc.2")]
    [TestCase("2.0.0-alpha", "2.0.0")]
    [TestCase("4.2.999", "4.3.0-alpha.1")]
    public void TestOrdering(string smaller, string larger)
    {
        SemanticVersion sv1 = new SemanticVersion(smaller);
        SemanticVersion sv2 = new SemanticVersion(larger);
        Assert.That(sv1.CompareTo(sv2), Is.LessThan(0),
            smaller + " should be less than " + larger);
        Assert.That(sv2.CompareTo(sv1), Is.GreaterThan(0),
            larger + " should be greater than " + smaller);
    }

    [Test]
    public void TestCompareToEqual()
    {
        SemanticVersion sv1 = new SemanticVersion("4.3.0");
        SemanticVersion sv2 = new SemanticVersion("4.3.0");
        Assert.That(sv1.CompareTo(sv2), Is.EqualTo(0));
    }

    // ---- Invalid input tests ----

    [Test]
    public void TestNullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new SemanticVersion(null!));
    }

    [Test]
    [TestCase("alpha.1.2.3")]
    [TestCase("1.0.alpha.beta.2")]
    [TestCase("")]
    [TestCase("abc")]
    [TestCase("1.2.-3")]
    public void TestInvalidInputThrows(string input)
    {
        Assert.That(() => new SemanticVersion(input),
            Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void TestCompareToNullThrows()
    {
        SemanticVersion sv = new SemanticVersion("1.0.0");
        Assert.Throws<ArgumentNullException>(() => sv.CompareTo(null));
    }
}
