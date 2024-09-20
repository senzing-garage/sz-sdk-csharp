namespace Senzing.Sdk.Tests.Core;

using NUnit.Framework;
using System;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(ParallelScope.Fixtures)]
public class SzCoreEnvironmentTest : AbstractTest 
{
    private static int constructCount = 0;
    private static int setupCount = 0;

    public SzCoreEnvironmentTest() {
        constructCount++;
    }

    [OneTimeSetUp]
    public static void OneTimeSetup() {
        Console.WriteLine("One-time setup");
        setupCount++;
    }

    [OneTimeTearDown]
    public static void OneTimeTearDown() {
        Console.WriteLine("One-time teardown");
        setupCount--;
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine($"Pre-Test Setup: {constructCount} / {setupCount}");
    }

    [TearDown]
    public void TearDown() {
        Console.WriteLine($"Post-Test Teardown: {constructCount} / {setupCount}");
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        Assert.Pass();
    }
}