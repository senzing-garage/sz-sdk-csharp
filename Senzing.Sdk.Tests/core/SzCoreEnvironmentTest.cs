namespace Senzing.Sdk.Tests.Core;

using NUnit.Framework;
using System;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(ParallelScope.Fixtures)]
internal class SzCoreEnvironmentTest : AbstractTest 
{
    private static readonly SzCoreEnvironmentTest Instance
        = new SzCoreEnvironmentTest();
    
    private const string EmployeeDataSource = "EMPLOYEES";

    private const string CustomerDataSOurce = "CUSTOMERS";

    [OneTimeSetUp]
    public void OneTimeSetup() {
        Instance.BeginTests();
        Instance.InitializeTestEnvironment();

        string settings = Instance.GetRepoSettings();
        string instanceName = Instance.GetInstanceName();

    }

    [OneTimeTearDown]
    public static void OneTimeTearDown() {

    }

    [Test]
    public void Test1()
    {
        if (this == Instance) {

        }
        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        Assert.Pass();
    }
}