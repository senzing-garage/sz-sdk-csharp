namespace Senzing.Sdk.Tests.Core;

using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Senzing.Sdk.Core;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzCoreEnvironmentTest : AbstractTest 
{
    private static readonly object monitor = new object();

    private const string DefaultSettings = SzCoreEnvironment.DefaultSettings;

    private const string EmployeesDataSource = "EMPLOYEES";
    
    private const string CustomersDataSource = "CUSTOMERS";

    private long configID1 = 0L;

    private long configID2 = 0L;

    private long configID3 = 0L;

    private long defaultConfigID = 0L;

    [OneTimeSetUp]
    public void InitializeEnvironment() {
        this.BeginTests();
        this.InitializeTestEnvironment();
        string settings     = this.GetRepoSettings();
        string instanceName = this.GetInstanceName();
        
        NativeConfig    nativeConfig    = new NativeConfigExtern();
        NativeConfigManager nativeConfigMgr = new NativeConfigManagerExtern();
        try {
            // initialize the native config
            this.Init(nativeConfig, instanceName, settings);
            this.Init(nativeConfigMgr, instanceName, settings);

            string config1 = this.CreateConfig(nativeConfig, CustomersDataSource);
            string config2 = this.CreateConfig(nativeConfig, EmployeesDataSource);
            string config3 = this.CreateConfig(
                nativeConfig, CustomersDataSource, EmployeesDataSource);

            this.configID1 = this.AddConfig(nativeConfigMgr, config1, "Config 1");
            this.configID2 = this.AddConfig(nativeConfigMgr, config2, "Config 2");
            this.configID3 = this.AddConfig(nativeConfigMgr, config3, "Config 3");

            this.defaultConfigID = this.GetDefaultConfigID(nativeConfigMgr);

        } finally {
            this.Destroy(nativeConfig);
            this.Destroy(nativeConfigMgr);
        }   
    }

    [OneTimeTearDown]
    public void TeardownEnvironment()
    {
        try {
            this.TeardownTestEnvironment();
            this.ConditionallyLogCounts(true);
        } finally {
            this.EndTests();
        }
    }

    [Test]
    public void TestNewDefaultBuilder() {
        this.PerformTest(() => {    
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder().Build();
    
                Assert.That(env.GetInstanceName(), 
                            Is.EqualTo(SzCoreEnvironment.DefaultInstanceName), 
                            "Environment instance name is not default instance name");
                Assert.That(env.GetSettings(), Is.EqualTo(DefaultSettings),
                            "Environment settings are not bootstrap settings");
                Assert.IsFalse(env.IsVerboseLogging(),
                    "Environment verbose logging did not default to false");
                Assert.IsNull(env.GetConfigID(), "Environment config ID is not null");
    
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }


    [Test]
    [TestCase(true,"Custom Instance")]
    [TestCase(false,"Custom Instance")]
    [TestCase(true, " ")]
    [TestCase(false, "")]
    public void TestNewCustomBuilder(bool verboseLogging, string instanceName) {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
            
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName(instanceName)
                                        .Settings(settings)
                                        .VerboseLogging(verboseLogging)
                                        .Build();

                string expectedName = (instanceName == null || instanceName.Trim().Length == 0)
                    ? SzCoreEnvironment.DefaultInstanceName : instanceName;
 
                Assert.That(expectedName, Is.EqualTo(env.GetInstanceName()),
                            "Environment instance name is not as expected");
                Assert.That(env.GetSettings(), Is.EqualTo(settings),
                            "Environment settings are not as expected");
                Assert.That(env.IsVerboseLogging(), Is.EqualTo(verboseLogging),
                            "Environment verbose logging did not default to false");
                Assert.IsNull(env.GetConfigID(), "Environment config ID is not null");
    
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    public void TestSingletonViolation() {
        this.PerformTest(() => {
            SzCoreEnvironment? env1 = null;
            SzCoreEnvironment? env2 = null;
            try {
                env1 = SzCoreEnvironment.NewBuilder().Build();
    
                try {
                    env2 = SzCoreEnvironment.NewBuilder().Settings(DefaultSettings).Build();
        
                    // if we get here then we failed
                    Fail("Was able to construct a second factory when first "
                         + "was not yet destroyed");
        
                } catch (InvalidOperationException) {
                    // this exception was expected
                } finally {
                    if (env2 != null) {
                        env2.Destroy();
                    }
                }
            } finally {
                if (env1 != null) {
                    env1.Destroy();
                }
            }    
        });
    }

    [Test]
    public void TestSingletonAdherence() {
        this.PerformTest(() => {
            SzCoreEnvironment? env1 = null;
            SzCoreEnvironment? env2 = null;
            try {
                env1 = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("Instance 1")
                                        .Build();
    
                env1.Destroy();
                env1 = null;
    
                env2 = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("Instance 2")
                                        .Settings(DefaultSettings)
                                        .Build();
    
                env2.Destroy();
                env2 = null;
    
            } finally {
                if (env1 != null) {
                    env1.Destroy();
                }
                if (env2 != null) {
                    env2.Destroy();
                }
            }    
        });
    }

    [Test]
    public void TestDestroy() {
        this.PerformTest(() => {
            SzCoreEnvironment? env1 = null;
            SzCoreEnvironment? env2 = null;
            try {
                // get the first environment
                env1 = SzCoreEnvironment.NewBuilder().InstanceName("Instance 1").Build();
    
                // ensure it is active
                try {
                    env1.EnsureActive();
                } catch (AssertionException) {
                    throw;
                } catch (Exception e) {
                    Fail("First Environment instance is not active.", e);
                }
    
                // destroy the first environment
                env1.Destroy();
    
                // check it is now inactive
                try {
                    env1.EnsureActive();
                    Fail("First Environment instance is still active.");
    
                } catch (AssertionException) {
                    throw;
                } catch (Exception) {
                    // do nothing
                } finally {
                    // clear the env1 reference
                    env1 = null;
                }
    
                // create a second environment instance
                env2 = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("Instance 2")
                                        .Settings(DefaultSettings)
                                        .Build();
    
                // ensure it is active
                try {
                    env2.EnsureActive();
                } catch (AssertionException) {
                    throw;
                } catch (Exception e) {
                    Fail("Second Environment instance is not active.", e);
                }
    
                // destroy the second environment
                env2.Destroy();
    
                // check it is now inactive
                try {
                    env2.EnsureActive();
                    Fail("Second Environment instance is still active.");
    
                } catch (AssertionException) {
                    throw;
                } catch (Exception) {
                    // do nothing
                } finally {
                    // clear the env2 reference
                    env2 = null;
                }
    
                env2 = null;
    
            } finally {
                if (env1 != null) {
                    env1.Destroy();
                }
                if (env2 != null) {
                    env2.Destroy();
                }
            }    
        });
    }


    [Test]
    [TestCase(2,"Foo")]
    [TestCase(3,"Bar")]
    [TestCase(4,"Phoo")]
    [TestCase(5,"Phoox")]
    public void TestExecute(int threadCount, string expected) {
        this.PerformTest(() => {
            SzCoreEnvironment env = SzCoreEnvironment.NewBuilder().Build();

            IList<Task<string>> tasks = new List<Task<string>>(threadCount);

            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxPortThreads);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minPortThreads);
            try {
                if (threadCount <= minWorkerThreads) {
                    // lower the min threads first before lowering the max threads
                    ThreadPool.SetMinThreads(threadCount, minPortThreads);
                    ThreadPool.SetMaxThreads(threadCount, maxPortThreads);

                } else {
                    // set the max threads and then set the min threads
                    ThreadPool.SetMaxThreads(threadCount, maxPortThreads);
                    ThreadPool.SetMinThreads(threadCount, minPortThreads);
                }

                // loop through the threads
                for (int index = 0; index < threadCount; index++) {    
                    Task<string> task = Task<string>.Run( () => {
                        return env.Execute(() => {
                            return expected;
                        });
                    });
                    tasks.Add(task);
                }

                // loop through the tasks
                foreach (Task<string> task in tasks) {
                    try {
                        task.Wait();
                        string actual = task.Result;
                        Assert.That(expected, Is.EqualTo(actual), "Unexpected result from execute()");

                    } catch (AssertionException) {
                        throw;
                    } catch (Exception e) {
                        Fail("Failed execute with exception", e);
                    }
                }

            } finally {
                ThreadPool.SetMaxThreads(maxWorkerThreads, maxPortThreads);
                ThreadPool.SetMinThreads(minWorkerThreads, minPortThreads);    
                if (env != null) {
                    env.Destroy();
                }
            }    
        });
    }

    [Test]
    [TestCase("Foo")]
    [TestCase("Bar")]
    [TestCase("Phoo")]
    [TestCase("Phoox")]
    public void TestExecuteFail(string expected) {
        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            try {
                env  = SzCoreEnvironment.NewBuilder().Build();
    
                try {
                   env.Execute<object>(() => {
                        throw new SzException(expected);
                   });
    
                   Fail("Expected SzException was not thrown");
    
                } catch (SzException e) {
                    Assert.That(e.Message, Is.EqualTo(expected), 
                                "Unexpected exception messasge");
    
                } catch (AssertionException) {
                    throw;
                } catch (Exception e) {
                    Fail("Failed execute with exception", e);
                }
    
            } finally {
                if (env != null) {
                    env.Destroy();
                }
            }    
        });
    }

    [Test]
    [TestCase(1, "Foo")]
    [TestCase(2, "Bar")]
    [TestCase(3, "Phoo")]
    [TestCase(4, "Phoox")]
    public void TestGetExecutingCount(int threadCount, string expected) {
        this.PerformTest(() => {
            int executeCount = threadCount * 3;

            Object[] monitors = new Object[executeCount];
            for (int index = 0; index < executeCount; index++) {
                monitors[index] = new Object();
            }
            SzCoreEnvironment? env  = null;
            try {
                env  = SzCoreEnvironment.NewBuilder().InstanceName(expected).Build();
    
                Thread[]        threads     = new Thread[executeCount];
                string?[]       results     = new string[executeCount];
                Exception?[]    failures    = new Exception[executeCount];
    
                for (int index = 0; index < executeCount; index++) {
                    SzCoreEnvironment coreEnv = env;
                    int threadIndex = index;
                    threads[index] = new Thread(() => {
                        try {
                            string actual = coreEnv.Execute(() => {
                                Object monitor = monitors[threadIndex];
                                lock (monitor) {
                                    Monitor.PulseAll(monitor);
                                    Monitor.Wait(monitor);
                                }
                                return expected + "-" + threadIndex;
                            });
                            results[threadIndex]    = actual;
                            failures[threadIndex]   = null;
                
                        } catch (AssertionException) {
                            throw;
                        } catch (Exception e) {
                            results[threadIndex]    = null;
                            failures[threadIndex]   = e;
                        }
                    });
                }
                int prevExecutingCount = 0;
                for (int index = 0; index < executeCount; index++) {
                    Object monitor = monitors[index];
    
                    lock (monitor) {
    
                        threads[index].Start();
                        Monitor.Wait(monitor);
                    }
                    int executingCount = env.GetExecutingCount();
                    Assert.IsTrue(executingCount > 0, "Executing count is zero");
                    Assert.IsTrue(executingCount > prevExecutingCount, 
                            "Executing count (" + executingCount + ") decremented from previous ("
                            + prevExecutingCount + ")");
                    prevExecutingCount = executingCount;
                }
    
                for (int index = 0; index < executeCount; index++) {
                    Object monitor = monitors[index];
                    lock (monitor) {
                        Monitor.PulseAll(monitor);
                    }
                    threads[index].Join();

                    int executingCount = env.GetExecutingCount();
                    Assert.IsTrue(executingCount >= 0, "Executing count is negative");
                    Assert.IsTrue(executingCount < prevExecutingCount, 
                            "Executing count (" + executingCount + ") incremented from previous ("
                            + prevExecutingCount + ")");
                    prevExecutingCount = executingCount;
                }
                
                // check the basics
                for (int index = 0; index < executeCount; index++) {
                    Assert.That(expected + "-" + index, Is.EqualTo(results[index]),
                                "At least one thread returned an unexpected result");
                    Assert.IsNull(failures[index], 
                                  "At least one thread threw an exception");
                }
                
            } finally {
                for (int index = 0; index < executeCount; index++) {
                    Object monitor = monitors[index];
                    lock (monitor) {
                        Monitor.PulseAll(monitor);
                    }
                }
                if (env != null) {
                    env.Destroy();
                }
            }    
        });
    }

    [Test]
    public void TestDestroyRaceConditions() {
        this.PerformTest(() => {
            SzCoreEnvironment env = SzCoreEnvironment.NewBuilder().Build();

            Object monitor = new Object();
            Exception?[] failures = { null, null, null };
            Thread busyThread = new Thread(() => {
                try {
                    env.Execute<object?>(() => {
                        lock (monitor) {
                            Monitor.Wait(monitor, 15000);
                        }
                        return null;
                    });
                } catch (AssertionException) {
                    throw;
                } catch (Exception e) {
                    failures[0] = e;
                }
            });

            long?[] destroyDuration = [ null ];
            Thread destroyThread = new Thread(() => {
                try {
                    Thread.Sleep(100);
                    long start = Environment.TickCount64;
                    env.Destroy();
                    long end = Environment.TickCount64;
        
                    destroyDuration[0] = (end - start);
                } catch (AssertionException) {
                    throw;
                } catch (Exception e) {
                    failures[1] = e;
                }
            });

            // start the thread that will keep the environment busy
            busyThread.Start();

            // start the thread that will destroy the environment
            destroyThread.Start();

            // sleep for one second to ensure destroy has been called
            Thread.Sleep(1000);

            bool destroyed = env.IsDestroyed();
            Assert.IsTrue(destroyed, "Environment NOT marked as destroyed");
            
            SzCoreEnvironment active = SzCoreEnvironment.GetActiveInstance();

            Assert.IsNull(active, "Active instrance was NOT null when destroying");

            // try to execute after destroy
            try {
                env.Execute<object?>(() => {
                    return null;
                });
                Fail("Unexpectedly managed to execute on a destroyed instance");

            } catch (InvalidOperationException) {
                // all is well
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Failed with unexpected exception", e);
            }

            try {
                busyThread.Join();
                destroyThread.Join();
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Thread joining failed with an exception.", e);
            }

            Assert.IsNotNull(destroyDuration[0], "Destroy duration was not record");
            Assert.IsTrue(destroyDuration[0] > 2000L, "Destroy occurred too quickly: " 
                        + destroyDuration[0] + "ms");

            if (failures[0] != null) {
                Fail("Busy thread got an exception.", failures[0]);
            }
            if (failures[1] != null) {
                Fail("Destroying thread got an exception.", failures[1]);
            }

        });
    }

    [Test]
    public void TestGetActiveInstance() {
        this.PerformTest(() => {
            SzCoreEnvironment? env1 = null;
            SzCoreEnvironment? env2 = null;
            try {
                // get the first environment
                env1 = SzCoreEnvironment.NewBuilder().InstanceName("Instance 1").Build();
    
                SzCoreEnvironment active = SzCoreEnvironment.GetActiveInstance();

                Assert.IsNotNull(active, "No active instance found when it should have been: " 
                                     + env1.GetInstanceName());
                Assert.IsTrue((env1 == active),
                            "Active instance was not as expected: " 
                            + ((active == null) ? null : active.GetInstanceName()));
    
                // destroy the first environment
                env1.Destroy();
    
                active = SzCoreEnvironment.GetActiveInstance();
                Assert.IsNull(active,
                           "Active instance found when there should be none: " 
                           + ((active == null) ? "" : active.GetInstanceName()));
                            
                // create a second Environment instance
                env2 = SzCoreEnvironment.NewBuilder()
                    .InstanceName("Instance 2").Settings(DefaultSettings).Build();
    
                active = SzCoreEnvironment.GetActiveInstance();
                Assert.IsNotNull(active, "No active instance found when it should have been: " 
                                 + env2.GetInstanceName());
                Assert.IsTrue((env2 == active),
                              "Active instance was not as expected: " 
                              + ((active == null) ? null : active.GetInstanceName()));
                    
                // destroy the second environment
                env2.Destroy();
    
                active = SzCoreEnvironment.GetActiveInstance();
                Assert.IsNull(active,
                    "Active instance found when there should be none: " 
                    + ((active == null) ? null : active.GetInstanceName()));
                
                env2 = null;
    
            } finally {
                if (env1 != null) {
                    env1.Destroy();
                }
                if (env2 != null) {
                    env2.Destroy();
                }
            }    
       });
    }

    [Test]
    public void TestGetConfig() {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
            
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("GetConfig Instance")
                                        .Settings(settings)
                                        .VerboseLogging(false)
                                        .Build();

                SzConfig config1 = env.GetConfig();
                SzConfig config2 = env.GetConfig();

                Assert.IsNotNull(config1, "SzConfig was null");
                Assert.IsTrue((config1 == config2), "SzConfig not returning the same object");
                Assert.IsInstanceOf(typeof(SzCoreConfig), config1,
                                    "SzConfig instance is not an instance of SzCoreConfig: "
                                    + config1.GetType().FullName);
                Assert.IsFalse(((SzCoreConfig) config1).IsDestroyed(),
                                   "SzConfig instance reporting that it is destroyed");

                env.Destroy();
                env  = null;

                // ensure we can call destroy twice
                ((SzCoreConfig) config1).Destroy();

                Assert.IsTrue(((SzCoreConfig) config1).IsDestroyed(),
                                  "SzConfig instance reporting that it is NOT destroyed");

            } catch (SzException e) {
                Fail("Got SzException during test", e);

            } finally {
                if (env != null) env.Destroy();
            }    
        });

    }

    [Test]
    public void TestGetConfigManager() {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
            
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("GetConfigManager Instance")
                                        .Settings(settings)
                                        .VerboseLogging(false)
                                        .Build();

                SzConfigManager configMgr1 = env.GetConfigManager();
                SzConfigManager configMgr2 = env.GetConfigManager();

                Assert.IsNotNull(configMgr1, "SzConfigManager was null");
                Assert.IsTrue((configMgr1 == configMgr2),
                              "SzConfigManager not returning the same object");
                Assert.IsInstanceOf(typeof(SzCoreConfigManager), configMgr1,
                                "SzConfigManager instance is not an instance of SzCoreConfigManager: "
                                + configMgr1.GetType().FullName);
                Assert.IsFalse(((SzCoreConfigManager) configMgr1).IsDestroyed(),
                            "SzConfigManager instance reporting that it is destroyed");

                env.Destroy();
                env  = null;

                // ensure we can call destroy twice
                ((SzCoreConfigManager) configMgr1).Destroy();

                Assert.IsTrue(((SzCoreConfigManager) configMgr1).IsDestroyed(),
                            "SzConfigManager instance reporting that it is NOT destroyed");

            } catch (SzException e) {
                Fail("Got SzException during test", e);

            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    public void TestGetDiagnostic() {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
            
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("GetDiagnostic Instance")
                                        .Settings(settings)
                                        .VerboseLogging(false)
                                        .Build();

                SzDiagnostic diagnostic1 = env.GetDiagnostic();
                SzDiagnostic diagnostic2 = env.GetDiagnostic();

                Assert.IsNotNull(diagnostic1, "SzDiagnostic was null");
                Assert.IsTrue((diagnostic1 == diagnostic2),
                              "SzDiagnostic not returning the same object");
                Assert.IsInstanceOf(typeof(SzCoreDiagnostic), diagnostic1,
                                "SzDiagnostic instance is not an instance of SzCoreDiagnostic: "
                                + diagnostic1.GetType().FullName);
                Assert.IsFalse(((SzCoreDiagnostic) diagnostic1).IsDestroyed(),
                            "SzDiagnostic instance reporting that it is destroyed");

                env.Destroy();
                env  = null;

                // ensure we can call destroy twice
                ((SzCoreDiagnostic) diagnostic1).Destroy();

                Assert.IsTrue(((SzCoreDiagnostic) diagnostic1).IsDestroyed(),
                            "SzDiagnostic instance reporting that it is NOT destroyed");

            } catch (SzException e) {
                Fail("Got SzException during test", e);

            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    public void TestGetEngine() {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
            
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("GetEngine Instance")
                                        .Settings(settings)
                                        .VerboseLogging(false)
                                        .Build();

                SzEngine engine1 = env.GetEngine();
                SzEngine engine2 = env.GetEngine();

                Assert.IsNotNull(engine1, "SzEngine was null");
                Assert.IsTrue((engine1 == engine2),
                              "SzEngine not returning the same object");
                Assert.IsInstanceOf(typeof(SzCoreEngine), engine1,
                                "SzEngine instance is not an instance of SzCoreEngine: "
                                + engine1.GetType().FullName);
                Assert.IsFalse(((SzCoreEngine) engine1).IsDestroyed(),
                            "SzEngine instance reporting that it is destroyed");

                env.Destroy();
                env  = null;

                // ensure we can call destroy twice
                ((SzCoreEngine) engine1).Destroy();

                Assert.IsTrue(((SzCoreEngine) engine1).IsDestroyed(),
                            "SzEngine instance reporting that it is NOT destroyed");

            } catch (SzException e) {
                Fail("Got SzException during test", e);

            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    public void TestGetProduct() {
        this.PerformTest(() => {
            string settings = this.GetRepoSettings();
        
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder()
                                        .InstanceName("GetProduct Instance")
                                        .Settings(settings)
                                        .VerboseLogging(false)
                                        .Build();

                SzProduct product1 = env.GetProduct();
                SzProduct product2 = env.GetProduct();

                Assert.IsNotNull(product1, "SzProduct was null");
                Assert.IsTrue((product1 == product2),
                              "SzProduct not returning the same object");
                Assert.IsInstanceOf(typeof(SzCoreProduct), product1,
                                "SzProduct instance is not an instance of SzCoreProduct: "
                                + product1.GetType().FullName);
                Assert.IsFalse(((SzCoreProduct) product1).IsDestroyed(),
                            "SzProduct instance reporting that it is destroyed");

                env.Destroy();
                env  = null;

                // ensure we can call destroy twice
                ((SzCoreProduct) product1).Destroy();

                Assert.IsTrue(((SzCoreProduct) product1).IsDestroyed(),
                            "SzProduct instance reporting that it is NOT destroyed");

            } catch (SzException e) {
                Fail("Got SzException during test", e);

            } finally {
                if (env != null) env.Destroy();
            }        
        });
    }

    [Test]
    [TestCase("Foo")]
    [TestCase("Bar")]
    [TestCase("Phoo")]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\t\t")]
    public void TestGetInstanceName(string instanceName) {
        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            try {
                string? name = instanceName.Length == 0 ? null : instanceName;
                env = SzCoreEnvironment.NewBuilder().InstanceName(name).Build();
    
                string expectedName = (instanceName.Trim().Length == 0) 
                    ? SzCoreEnvironment.DefaultInstanceName : instanceName;

                Assert.That(env.GetInstanceName(), Is.EqualTo(expectedName),
                             "Instance names are not equal after building.");
            
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    [TestCase(10L)]
    [TestCase(12L)]
    [TestCase(0L)]
    public void TestGetConfigID(long? configID) {
        long? initConfigID = (configID == 0L) ? null : configID;

        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;    
            try {
                env  = SzCoreEnvironment.NewBuilder().ConfigID(initConfigID).Build();
    
                Assert.That(env.GetConfigID(), Is.EqualTo(initConfigID),
                             "Config ID's are not equal after building.");
            
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    private static IList<Func<SzCoreEnvironmentTest,string?>> GetSettingsList() {
        IList<Func<SzCoreEnvironmentTest,string?>> result
            = new List<Func<SzCoreEnvironmentTest,string?>>();
        result.Add((test) => DefaultSettings);
        result.Add((test) => test.GetRepoSettings());
        result.Add((test) => null);
        result.Add((test) => "");
        result.Add((test) => "    ");
        result.Add((test) => "\t\t");
        return result;
    }

    [Test, TestCaseSource(nameof(GetSettingsList))]
    public void TestGetSettings(Func<SzCoreEnvironmentTest,string?> settingsFunc) {
        string? settings = settingsFunc(this);
        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder().Settings(settings).Build();
    
                string expected = (settings == null || settings.Trim().Length == 0) 
                    ? SzCoreEnvironment.DefaultSettings : settings;

                Assert.That(env.GetSettings(), Is.EqualTo(expected),
                             "Settings are not equal after building.");
            
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TestIsVerboseLogging(bool verbose) {
        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder().VerboseLogging(verbose).Build();
    
                Assert.That(env.IsVerboseLogging(), Is.EqualTo(verbose),
                            "Verbose logging settings are not equal after building.");
            
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    private class FakeNativeApi : NativeApi {
        private long errorCode;
        private string errorMessage;
        public FakeNativeApi(long errorCode, string errorMessage) {
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
        }        
        public long GetLastExceptionCode() { return this.errorCode; }
        public string GetLastException() { return this.errorMessage; }
        public void ClearLastException() { }
}
    
    [Test]
    [TestCase(1,10,"Foo")]
    [TestCase(0,20,"Bar")]
    [TestCase(2,30,"Phoo")]
    public void TestHandleReturnCode(int returnCode, long errorCode, string errorMessage) {
        NativeApi fakeNativeApi = new FakeNativeApi(errorCode, errorMessage);

        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            try {
                env  = SzCoreEnvironment.NewBuilder().Settings(DefaultSettings).Build();
    
                try {
                    env.HandleReturnCode(returnCode, fakeNativeApi);

                    if (returnCode != 0) {
                        Fail("The handleReturnCode() function did "
                             + "not throw an exception with return code: "
                             + returnCode);
                    }
    
                } catch (SzException e) {
                    if (returnCode == 0) {
                        Fail("Unexpected exception from handleReturnCode() "
                             + "with return code: " + returnCode, e);
                    } else {
                        Assert.IsInstanceOf(typeof(SzException), e, "Type of exception is not as expected");
                        SzException sze = (SzException) e;
                        Assert.That(sze.GetErrorCode(), Is.EqualTo(errorCode),
                                    "Error code of exception is not as expected");
                        Assert.That(e.Message, Is.EqualTo(errorMessage),
                                    "Error message of exception is not as expected");
                    }
                }
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    private static IList<(int,bool)> GetActiveConfigIDParams() {
        IList<(int,bool)> result = new List<(int,bool)>();
        int[] configIDs = [ 1, 2, 3 ];

        bool initEngine = false;
        foreach (int config in configIDs) {
            initEngine = !initEngine;
            result.Add((config, initEngine));
        }

        return result;
    }

    [Test, TestCaseSource(nameof(GetActiveConfigIDParams))]
    public void TestGetActiveConfigID((int configIndex, bool initEngine) args) {
        long configID = this.getConfigID(args.configIndex);
        bool initEngine = args.initEngine;

        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;

            string info = "configID=[ " + configID + " ], initEngine=[ " 
                    + initEngine + " ]";
                
            try {
                string settings = this.GetRepoSettings();
                string instanceName = this.GetInstanceName(
                    "ActiveConfig-" + configID);
    
                env  = SzCoreEnvironment.NewBuilder().Settings(settings)
                                                     .InstanceName(instanceName)
                                                     .ConfigID(configID)
                                                     .Build();
    
                // check the init config ID
                Assert.That(env.GetConfigID(), Is.EqualTo(configID), 
                     "The initialization config ID is not as expected" + info);
            
                // get the active config
                long activeConfigID = env.GetActiveConfigID();
    
                Assert.That(activeConfigID, Is.EqualTo(configID),
                            "The active config ID is not as expected: " + info);
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Got exception in TestGetActiveConfigID: " + info, e);
    
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void TestGetActiveConfigIDDefault(bool initEngine) {
        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
        
            string info = "initEngine=[ " + initEngine + " ]";
    
            try {
                string settings = this.GetRepoSettings();
                string instanceName = this.GetInstanceName(
                    "ActiveConfigDefault");
                    
                env  = SzCoreEnvironment.NewBuilder().Settings(settings)
                                                     .InstanceName(instanceName)
                                                     .Build();
    
                Assert.IsNull(env.GetConfigID(),
                    "The initialziation starting config ID is not null: " + info);
    
                // get the active config
                long activeConfigID = env.GetActiveConfigID();
    
                Assert.That(activeConfigID, Is.EqualTo(this.defaultConfigID), 
                            "The active config ID is not as expected: " + info);
                        
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Got exception in TestGetActiveConfigIDDefault: " + info, e);
                
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    [Test]
    public void TestExecuteException() {
        this.PerformTest(() => {
            SzCoreEnvironment env = SzCoreEnvironment.NewBuilder().Build();
            try {
                env.Execute<object>(() => {
                    throw new IOException("Test exception");
                });
            } catch (SzException e) {
                Exception cause = e.GetBaseException();
                Assert.IsInstanceOf(typeof(IOException), cause, "The cause was not an IOException");
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                Fail("Caught an unexpected exeption", e);
            } finally {
                if (env != null) env.Destroy();
            }
        });
    }

    private static IList<(int?,int,bool,bool)> GetReinitializeParams() {
        IList<(int?,int,bool,bool)> result = new List<(int?,int,bool,bool)>();

        IList<IList<bool?>> booleanCombos = GetBooleanVariants(2, false);

        Random prng = new Random(Environment.TickCount);
        
        IList configIDs = ImmutableList.Create(1, 2, 3);

        IList<IList> configIDCombos = GenerateCombinations(configIDs, configIDs);

        configIDCombos.Shuffle();
        booleanCombos.Shuffle();

        Iterator<IList> configIDIter = GetCircularIterator(configIDCombos);

        foreach (List<bool?> bools in booleanCombos) {
            bool initEngine     = bools[0] ?? false;
            bool initDiagnostic = bools[1] ?? false;
            
            foreach (int configID in configIDs) {
                result.Add((null, configID, initEngine, initDiagnostic));
            }

            IList configs = configIDIter.Next();
            result.Add(((((int?) configs[0]) ?? 0),
                        (((int?) configs[1]) ?? 0),
                        initEngine,
                        initDiagnostic));
        }

        return result;
    }

    [Test, TestCaseSource(nameof(GetReinitializeParams))]
    public void TestReinitialize((int?   startConfigIndex, 
                                  int    endConfigIndex,
                                  bool   initEngine, 
                                  bool   initDiagnostic) args) 
    {
        long? startConfig = (args.startConfigIndex == null)
             ? null : this.getConfigID(args.startConfigIndex ?? 0);
        
        long endConfig = this.getConfigID(args.endConfigIndex);
        bool initEngine = args.initEngine;
        bool initDiagnostic = args.initDiagnostic;

        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            string info = "startConfig=[ " + startConfig + " ], endConfig=[ " 
                 + endConfig + " ], initEngine=[ " + initEngine
                 + " ], initDiagnostic=[ " + initDiagnostic + " ]";
    
            try {
                string settings = this.GetRepoSettings();
                string instanceName = this.GetInstanceName("Reinitialize");
    
                env  = SzCoreEnvironment.NewBuilder().Settings(settings)
                                                     .InstanceName(instanceName)
                                                     .ConfigID(startConfig)
                                                     .Build();
    
                // check the init config ID
                Assert.That(env.GetConfigID(), Is.EqualTo(startConfig),
                    "The initialization stating config ID is not as expected" + info);
    
                // check if we should initialize the engine first
                if (initEngine) env.GetEngine();
    
                // check if we should initialize diagnostics first
                if (initDiagnostic) env.GetDiagnostic();
    
                long? activeConfigID = null;
                if (startConfig != null) {
                    // get the active config
                    activeConfigID = env.GetActiveConfigID();
    
                    Assert.That(activeConfigID, Is.EqualTo(startConfig),
                                "The starting active config ID is not as expected: "
                                + info);
                }
    
                // reinitialize
                env.Reinitialize(endConfig);
            
                // check the initialize config ID
                Assert.That(env.GetConfigID(), Is.EqualTo(endConfig),
                            "The initialization ending config ID is not as expected: "
                            + info);
    
                activeConfigID = env.GetActiveConfigID();
    
                Assert.That(activeConfigID, Is.EqualTo(endConfig),
                            "The ending active config ID is not as expected: "
                            + info);
    
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Got exception in TestReinitialize: " + info, e);
    
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }

    private long getConfigID(int index) {
        switch (index) {
            case 1:
                return this.configID1;
            case 2:
                return this.configID2;
            case 3:
                return this.configID3;
            default:
                throw new ArgumentException("Bad config ID index");
        }
    }

    private static IList<(int,bool,bool)> GetReinitializeDefaultParams() {
        IList<(int,bool,bool)> result = new List<(int,bool,bool)>();
        IList<IList<bool?>> booleanCombos = GetBooleanVariants(2, false);

        int[] configIDs = [ 1, 2 ];
        int count = 0;
        booleanCombos.Shuffle();
        foreach (IList<bool?> bools in booleanCombos) {
            int index = (count++) % configIDs.Length;
            result.Add((configIDs[index], bools[0] ?? false, bools[1] ?? false));
        }
        return result;
    }

    [Test,TestCaseSource(nameof(GetReinitializeDefaultParams))]
    public void TestReinitializeDefault((int     configIndex,
                                         bool    initEngine,
                                         bool    initDiagnostic) args)
    {
        long configID = this.getConfigID(args.configIndex);
        bool initEngine = args.initEngine;
        bool initDiagnostic = args.initDiagnostic;

        this.PerformTest(() => {
            SzCoreEnvironment? env  = null;
            
            string info = "config=[ " + configID + " ], initEngine=[ " + initEngine
                 + " ], initDiagnostic=[ " + initDiagnostic + " ]";
    
            try {
                string settings = this.GetRepoSettings();
                string instanceName = this.GetInstanceName("ReinitializeDefault");
    
                env  = SzCoreEnvironment.NewBuilder().Settings(settings)
                                                     .InstanceName(instanceName)
                                                     .Build();
    
                Assert.IsNull(env.GetConfigID(),
                    "The initialziation starting config ID is not null: " + info);
                
                // check if we should initialize the engine first
                if (initEngine) env.GetEngine();
    
                // check if we should initialize diagnostics first
                if (initDiagnostic) env.GetDiagnostic();
    
                // get the active config ID
                long activeConfigID = env.GetActiveConfigID();
    
                Assert.That(activeConfigID, Is.EqualTo(this.defaultConfigID),
                            "The starting config ID is not the default: " + info);
    
                // reinitialize
                env.Reinitialize(configID);
    
                // check the initialziation config ID again
                Assert.That(env.GetConfigID(), Is.EqualTo(configID),
                            "The initialization config ID is not the "
                            + "reinitialized one: " + info);
    
                // get the active config ID again
                activeConfigID = env.GetActiveConfigID();
    
                Assert.That(activeConfigID, Is.EqualTo(configID),
                            "The reinitialized active config ID is not "
                            + "as expected: " + info);
    
            } catch (AssertionException) {
                throw;
            } catch (Exception e) {
                Fail("Got exception in TestReinitializeDefault: " + info, e);
    
            } finally {
                if (env != null) env.Destroy();
            }    
        });
    }
}