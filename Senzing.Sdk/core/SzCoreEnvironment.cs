using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

using static Senzing.Sdk.Core.SzCoreUtilities;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the core implementation of <see cref="Senzing.Sdk.SzEnvironment"/>
    /// that directly initializes the Senzing SDK modules and provides management
    /// of the Senzing environment in this process.
    /// </summary>
    /// 
    /// <example>
    /// Usage:
    /// <include file="../target/examples/SzProductDemo_SzEnvironment.xml" path="/*"/>
    /// </example>
    ///
    /// <seealso cref="Senzing.Sdk.SzEnvironment"/> 
    public class SzCoreEnvironment : SzEnvironment
    {
        /// <summary>
        /// The default instance name to use for the Senzing initialization.
        /// The value is <c>"Senzing Instance"</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// An explicit value can be provided via the <see cref="Builder"/>'s
        /// <see cref="AbstractBuilder{E,B}.InstanceName"><c>InstanceName</c></see>
        /// method.
        /// </remarks>
        /// 
        /// <seealso cref="NewBuilder"/> 
        /// <seealso cref="Builder"/> 
        /// <seealso cref="AbstractBuilder{E,B}.InstanceName" />
        public const string DefaultInstanceName = "Senzing Instance";

        /// <summary>
        /// The default "bootstrap" settings with which to initialize
        /// the <c>SzCoreEnvironment</c> when an explicit settings
        /// value has not been provided via the <see cref="Builder"/>'s 
        /// <see cref="AbstractBuilder{E,B}.Settings"><c>Settings</c></see>
        /// method.
        /// </summary>
        /// 
        /// <remarks>
        /// If this is used it will initialize Senzing for access to only the
        /// <see cref="SzProduct" /> and <see cref="SzConfig" /> interfaces
        /// when Senzing is installed in the default location for the
        /// platform.  The value of this constant is <c>"{ }"</c>.
        /// 
        /// <para>
        /// <b>NOTE:</b> Using these settings is only useful for accessing the
        /// <see cref="SzProduct" /> and  <see cref="SzConfig" /> interfaces since
        /// <see cref="SzEngine" />, <see cref="SzConfigManager" /> and
        /// <see cref="SzDiagnostic"/> require database configuration to connect
        /// to the Senzing repository.
        /// </para>
        /// </remarks>
        /// 
        /// <seealso cref="NewBuilder"/> 
        /// <seealso cref="Builder"/> 
        /// <seealso cref="AbstractBuilder{E,B}.Settings" />
        public const string DefaultSettings = "{ }";

        /// <summary>
        /// The number of milliseconds to delay (if not notified) until checking
        /// if we are ready to destroy.
        /// </summary>
        private const int DestroyDelay = 5000;

        /// <summary>
        /// Internal object for class-wide synchronized locking.
        /// </summary>
        private static readonly Object ClassMonitor = new Object();

        /// <summary>
        /// Enumerates the possible states for an instance of <c>SzCoreEnvironment</c>.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// If an <c>SzCoreEnvironment</c> instance is in the "active" state then it
            /// is initialized and ready to use.
            /// </summary>
            /// 
            /// <remarks>
            /// Only one instance of <c>SzCoreEnvironment</c> can exist in the
            /// <see cref="Active" /> or <see cref="Destroying" /> state because Senzing
            /// environment cannot be initialized heterogeneously within a single process.
            /// </remarks>
            /// 
            /// <seealso cref="SzCoreEnvironment.GetActiveInstance" />
            /// <seealso cref="SzCoreEnvironment.Destroy" />
            Active,

            /// <summary>
            /// An instance <c>SzCoreEnvironment</c> moves to the "destroying" state when
            /// the <see cref="SzCoreEnvironment.Destroy" /> method has been called but has
            /// not yet completed any Senzing operations that are already in-progress.
            /// </summary>
            /// 
            /// <remarks>
            /// In this state, the <c>SzCoreEnvironment</c> will fast-fail any newly
            /// attempted operations with an <see cref="System.InvalidOperationException" />,
            /// but will complete those Senzing operations that were initiated before
            /// <see cref="Destroy"/> was called.
            /// </remarks>
            /// 
            /// <seealso cref="SzCoreEnvironment.GetActiveInstance" />
            /// <seealso cref="SzCoreEnvironment.Destroy" />
            Destroying,

            /// <summary>
            /// An instance of <c>SzCoreEnvironment</c> moves to the "destroyed" state
            /// when the <see cref="Destroy" /> method has completed and there are no
            /// more Senzing operations that are in-progress.
            /// </summary>
            /// 
            /// <remarks>
            /// Once an <see cref="Active" /> instance moves to <see cref="Destroyed" />
            /// then a new active instance can be created and initialized.
            /// </remarks>
            /// 
            /// <seealso cref="SzCoreEnvironment.GetActiveInstance" />
            /// <seealso cref="SzCoreEnvironment.Destroy" />
            Destroyed
        }

        /// <summary>
        /// Creates a new instance of <see cref="Builder"/> for setting up an
        /// instance of <c>SzCoreEnvironment</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// Keep in mind that while multiple <see cref="Builder"/> instances can
        /// exists, <b>only one active instance</b> of <c>SzCoreEnvironment</c>
        /// can exist at time.  An active instance is one that has not yet been
        /// destroyed.
        /// </remarks>
        /// 
        /// <returns>
        /// The <see cref="Builder"/> for configuring and initializing the
        /// <c>SzCoreEnvironment</c>.
        /// </returns>
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        /// <summary> 
        /// The current instance of the <c>SzCoreEnvironment</c>
        /// </summary>
        private static volatile SzCoreEnvironment currentInstance = null;

        /// <summary>
        /// Gets the current active instance of <c>SzCoreEnvironment</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// An active instance is one that has been constructed and has not yet
        /// been destroyed.  There can be at most one active instance.  If no
        /// active instance exists then <c>null</c> is returned.
        /// </remarks>
        /// 
        /// <returns>
        /// The current active instance of <c>SzCoreEnvironment</c>, or <c>null</c>
        /// if there is no active instance.
        /// </returns>
        public static SzCoreEnvironment GetActiveInstance()
        {
            lock (ClassMonitor)
            {
                if (currentInstance == null)
                {
                    return null;
                }
                Interlocked.MemoryBarrier();
                lock (currentInstance.monitor)
                {
                    State state = currentInstance.state;
                    switch (state)
                    {
                        case State.Destroying:
                            // wait until destroyed and fall through
                            WaitUntilDestroyed(currentInstance);
                            goto case State.Destroyed;
                        case State.Destroyed:
                            // if still set but destroyed, clear it and fall through
                            currentInstance = null;
                            goto case State.Active;

                        case State.Active:
                            // return the current instance
                            return currentInstance;
                        default:
                            throw new InvalidOperationException(
                                "Unrecognized SzCoreEnvironment state: " + state);
                    }
                }
            }
        }


        /// <summary>The instance name to initialize the API's with.</summary>
        private readonly string instanceName;

        /// <summary>The settings to initialize the API's with.</summary>
        private readonly string settings;

        /// <summary>The flag indicating if verbose logging is enabled.</summary>
        private readonly bool verboseLogging;

        /// <summary>
        /// The explicit configuration ID to initialize with or <c>null</c> if
        /// using the default configuration.
        /// </summary>
        private long? configID = null;

        /// <summary>
        /// The <see cref="SzCoreProduct"/> instance to use.
        /// </summary>
        private SzCoreProduct coreProduct = null;

        /// <summary>
        /// The <see cref="SzCoreEngine"/> instance to use.
        /// </summary>
        private SzCoreEngine coreEngine = null;

        /// <summary>
        /// The <see cref="SzCoreConfigManager"/> instance to use.
        /// </summary>
        private SzCoreConfigManager coreConfigMgr = null;

        /// <summary>
        /// The <see cref="SzCoreDiagnostic"/> singleton instance to use.
        /// </summary>
        private SzCoreDiagnostic coreDiagnostic = null;

        /// <summary>The <see cref="State"/> for this instance.</summary>
        private volatile State state;

        /// <summary>The number of currently executing operations.</summary>
        private volatile int executingCount = 0;

        /// <summary>The <c>ReaderWriterLock</c> for this instance.</summary>
        private readonly ReaderWriterLockSlim readWriteLock;

        /// <summary>
        /// Internal object for instance-wide synchronized locking.
        /// </summary>
        private readonly Object monitor = new Object();

        /// <summary>
        /// Protected constructor used by the <see cref="Builder"/> to 
        /// construct the instance.
        /// </summary>
        ///  
        /// <param name="initializer">
        /// The <see cref="Initializer"/> with which to construct.
        /// (typically an instance of <see cref="AbstractBuilder{E, B}"/>)
        /// </param>
        protected SzCoreEnvironment(Initializer initializer)
        {
            // set the fields
            this.readWriteLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            this.instanceName = initializer.GetInstanceName();
            this.settings = initializer.GetSettings();
            this.verboseLogging = initializer.IsVerboseLogging();
            this.configID = initializer.GetConfigID();

            lock (ClassMonitor)
            {
                SzCoreEnvironment activeEnvironment = GetActiveInstance();
                if (activeEnvironment != null)
                {
                    throw new InvalidOperationException(
                        "At most one active instance of SzCoreEnvironment can be "
                        + "initialized.  Another instance was previously initialized "
                        + "and has not yet been destroyed.");
                }

                // set the state
                this.state = State.Active;

                // set the current instance
                currentInstance = this;
            }
        }

        /// <summary>
        /// Waits until the specified <c>SzCoreEnvironment</c> instance has
        /// been destroyed.
        /// </summary>
        /// 
        /// <remarks>
        /// Use this when obtaining an instance of <c>SzCoreEnvironment</c> in
        /// the <see cref="State.Destroying"/> state and you want to wait until
        /// it is fully destroyed.
        /// </remarks>
        /// 
        /// <param name="environment">
        /// The non-null <c>SzCoreEnvironment</c> instance to wait on.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        /// If the specified parameter is <c>null</c>.
        /// </exception>
        private static void WaitUntilDestroyed(SzCoreEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("The specified instance cannot be null");
            }
            lock (environment.monitor)
            {
                while (environment.state != State.Destroyed)
                {
                    try
                    {
                        Monitor.Wait(environment.monitor, DestroyDelay);
                    }
                    catch (ThreadInterruptedException)
                    {
                        // ignore the exception
                    }
                }
            }
        }

        /// <summary>
        /// Gets the associated Senzing instance name for initialization.
        /// </summary>
        /// 
        /// <returns>The associated Senzing instance name for initialization.</returns>
        internal string GetInstanceName()
        {
            return this.instanceName;
        }


        /// <summary>
        /// Gets the associated Senzing settings for initialization.
        /// </summary>
        /// 
        /// <returns>The associated Senzing settings for initialization.</returns>
        internal string GetSettings()
        {
            return this.settings;
        }


        /// <summary>
        /// Gets the verbose logging setting to indicating if verbose logging
        /// should be enabled (<c>true</c>) or disabled (<c>false</c>).
        /// </summary>
        /// 
        /// <returns>
        /// <c>true</c> if verbose logging should be enabled, otherwise <c>false</c>.
        /// </returns>
        internal bool IsVerboseLogging()
        {
            return this.verboseLogging;
        }

        /// <summary>
        /// Gets the explicit configuration ID with which to initialize the Senzing 
        /// environment.
        /// </summary>
        /// 
        /// <remarks>
        /// This returns <c>null</c> if the default configuration ID configured
        /// in the repository should be used.
        /// </remarks>
        /// 
        /// <returns>q
        /// The explicit configuration ID with which to initialize the Senzing
        /// environment, or <c>null</c> if the default configuration ID
        /// configured in the repository should be used.
        /// </returns>
        internal long? GetConfigID()
        {
            return this.configID;
        }

        /// <summary>
        /// Executes the specified task (<c>Func</c>) via <see cref="DoExecute"/>
        /// after ensuring that this instance is not <see cref="Destroy">destroyed</see>
        /// and will not be <see cref="Destroy">destroyed</see> before the task's
        /// completion.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This is used by the core implementations of <see cref="SzEngine"/>,
        /// <see cref="SzProduct"/>, <see cref="SzConfigManager"/>, 
        /// <see cref="SzConfig"/> and <see cref="SzDiagnostic"/> to execute their
        /// functionality and ensure the environment is stable during execution.
        /// </para>
        /// 
        /// <para>
        /// If successful, this will return the result of the specified task.
        /// If not successful, this will throw any exception produced by the
        /// specified task, wrapping it in an <see cref="Senzing.Sdk.SzException"/>
        /// if it is a that is not of type <see cref="SzException"/>.
        /// </para>
        /// </remarks>
        /// 
        /// <typeparam name="T">
        /// The return type of the specified function and the type returned by this function.
        /// If your function does not really need a return value, then have it return <c>null</c>.
        /// </typeparam>
        /// 
        /// <param name="task">
        /// The no-argument <c>Func</c> to execute.
        /// </param>
        /// 
        /// <returns>The result from the specified task.</returns>
        /// 
        /// <exception cref="Senzing.Sdk.SzException">
        /// If the specified task triggers a failure.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        /// If this <c>SzCoreEnvironment</c> instance has already been destroyed.
        /// </exception>
        protected internal virtual T Execute<T>(Func<T> task)
        {
            ReaderWriterLockSlim localLock = null;
            try
            {
                // acquire a write lock while checking if active 
                localLock = this.AcquireReadLock();
                lock (this.monitor)
                {
                    if (this.state != State.Active)
                    {
                        throw new InvalidOperationException(
                            "SzEnvironment has been destroyed");
                    }

                    // increment the executing count
                    Interlocked.Increment(ref this.executingCount);
                }
                try
                {
                    return DoExecute(task);
                }
                catch (SzException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new SzException(e);
                }

            }
            finally
            {
                lock (this.monitor)
                {
                    Interlocked.Decrement(ref this.executingCount);
                    Monitor.PulseAll(this.monitor);
                }
                localLock = this.ReleaseLock(localLock);
            }
        }

        /// <summary>
        /// Called by <see cref="Execute"/> after ensuring that this instance
        /// is not <see cref="Destroy">destroyed</see> and ensuring any calls
        /// to <see cref="Destroy"/> will block until after this method's 
        /// invocation is completed.
        /// </summary>
        /// 
        /// <remarks>
        /// If successful, this will return the result of the specified task.
        /// If not successful, this will throw any exception produced by the
        /// specified task, wrapping it in an <see cref="Senzing.Sdk.SzException"/>
        /// if it is a that is not of type <see cref="SzException"/>.
        /// </remarks>
        /// 
        /// <typeparam name="T">
        /// The return type of the specified function and the type returned by this function.
        /// If your function does not really need a return value, then have it return <c>null</c>.
        /// </typeparam>
        /// 
        /// <param name="task">
        /// The no-argument <c>Func</c> to execute.
        /// </param>
        /// 
        /// <returns>The result from the specified task.</returns>
        protected internal virtual T DoExecute<T>(Func<T> task)
        {
            return task();
        }

        /// <summary>
        /// Gets the number of currently executing operations.
        /// </summary>
        /// 
        /// <returns>
        /// The number of currently executing operations.
        /// </returns>
        internal int GetExecutingCount()
        {
            lock (this.monitor)
            {
                return this.executingCount;
            }
        }

        /// <summary>
        /// Ensures this instance is still active and if not will throw 
        /// an <see cref="InvalidOperationException"/>.
        /// </summary>
        ///
        /// <exception cref="System.InvalidOperationException">
        /// If this instance is not active.
        /// </exception>
        internal void EnsureActive()
        {
            lock (ClassMonitor)
            {
                if (this.state != State.Active)
                {
                    throw new InvalidOperationException(
                        "The SzCoreEnvironment instance has already been destroyed.");
                }
            }
        }

        /// <summary>
        /// Handles the Senzing native return code and coverts it to the proper
        /// <see cref="Senzing.Sdk.SzException"/> if it is not zero (0).
        /// </summary>
        ///
        /// <param name="returnCode">
        /// The return code to handle.
        /// </param>
        ///
        /// <param name="nativeApi">
        /// The <see cref="Senzing.Sdk.Core.NativeApi"/> implementation that
        /// produced the return code on this current thread.
        /// </param>
        internal void HandleReturnCode(long returnCode, NativeApi nativeApi)
        {
            if (returnCode == 0L)
            {
                return;
            }
            // TODO(barry): Map the exception properly
            long errorCode = nativeApi.GetLastExceptionCode();
            string message = nativeApi.GetLastException();
            nativeApi.ClearLastException();

            // create the appropriate exception
            throw CreateSzException(errorCode, message);
        }

        /// <summary>
        /// Implemented to return the <see cref="SzCoreConfigManager"/>
        /// associated with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="SzCoreConfigManager"/> associated with this instance.
        /// </returns>
        public SzConfigManager GetConfigManager()
        {
            lock (this.monitor)
            {
                this.EnsureActive();
                if (this.coreConfigMgr == null)
                {
                    this.coreConfigMgr = new SzCoreConfigManager(this);
                }

                // return the configured instance
                return this.coreConfigMgr;
            }
        }

        /// <summary>
        /// Implemented to return the <see cref="SzCoreDiagnostic"/>
        /// associated with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="SzCoreDiagnostic"/> associated with this instance.
        /// </returns>
        public SzDiagnostic GetDiagnostic()
        {
            lock (this.monitor)
            {
                this.EnsureActive();
                if (this.coreDiagnostic == null)
                {
                    this.coreDiagnostic = new SzCoreDiagnostic(this);
                }

                // return the configured instance
                return this.coreDiagnostic;
            }
        }

        /// <summary>
        /// Implemented to return the <see cref="SzCoreEngine"/>
        /// associated with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="SzCoreEngine"/> associated with this instance.
        /// </returns>
        public SzEngine GetEngine()
        {
            lock (this.monitor)
            {
                this.EnsureActive();
                if (this.coreEngine == null)
                {
                    this.coreEngine = new SzCoreEngine(this);
                }

                // return the configured instance
                return this.coreEngine;
            }
        }

        /// <summary>
        /// Implemented to return the <see cref="SzCoreProduct"/> associated
        /// with this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="SzCoreProduct"/> associated with this instance.
        /// </returns>
        public SzProduct GetProduct()
        {
            lock (this.monitor)
            {
                this.EnsureActive();
                if (this.coreProduct == null)
                {
                    this.coreProduct = new SzCoreProduct(this);
                }

                // return the configured instance
                return this.coreProduct;
            }
        }

        /// <summary>
        /// Implemented to destroy each of the underlying core API objects.
        /// </summary>
        /// 
        /// <remarks>
        /// This method has no effect if it has previously been called for this instance.
        /// </remarks>
        public void Destroy()
        {
            ReaderWriterLockSlim localLock = null;
            try
            {
                lock (this.monitor)
                {
                    // check if this has already been called
                    if (this.state != State.Active)
                    {
                        return;
                    }

                    // set the flag for destroying
                    this.state = State.Destroying;
                    Monitor.PulseAll(this.monitor);
                }

                // acquire an exclusive lock for destroying to ensure
                // all executing tasks have completed
                localLock = this.AcquireWriteLock();

                // ensure completion of in-flight executions
                int exeCount = this.GetExecutingCount();
                if (exeCount > 0)
                {
                    throw new InvalidOperationException(
                        "Acquired write lock for destroying environment while tasks "
                        + "still executing: " + exeCount);
                }

                // once we get here we can really shut things down
                if (this.coreEngine != null)
                {
                    this.coreEngine.Destroy();
                    this.coreEngine = null;
                }
                if (this.coreDiagnostic != null)
                {
                    this.coreDiagnostic.Destroy();
                    this.coreDiagnostic = null;
                }
                if (this.coreConfigMgr != null)
                {
                    this.coreConfigMgr.Destroy();
                    this.coreConfigMgr = null;
                }
                if (this.coreProduct != null)
                {
                    this.coreProduct.Destroy();
                    this.coreProduct = null;
                }

                // set the state
                lock (this.monitor)
                {
                    this.state = State.Destroyed;
                    Monitor.PulseAll(this.monitor);
                }
            }
            finally
            {
                localLock = this.ReleaseLock(localLock);
            }
        }

        /// <summary>
        /// Implemented to return <c>true</c> if this instance is 
        /// the currently active instance, otherwise <c>false</c>.
        /// </summary>
        /// 
        /// <returns>
        /// <c>true</c> if this instance is the currently active
        /// instance, otherwise <c>false</c>.
        /// </returns>
        public bool IsDestroyed()
        {
            lock (this.monitor)
            {
                return this.state != State.Active;
            }
        }

        /// <summary>
        /// Implemented to call the underlying native method to obtain
        /// the active config ID.
        /// </summary>
        public long GetActiveConfigID()
        {
            ReaderWriterLockSlim localLock = null;
            try
            {
                // get a read lock to ensure we remain active while
                // executing the operation
                localLock = this.AcquireReadLock();

                // ensure we have initialized the engine or diagnostic
                lock (this.monitor)
                {
                    this.EnsureActive();

                    // check if the core engine has been initialized
                    if (this.coreEngine == null)
                    {
                        // initialize the engine if not yet initialized
                        this.GetEngine();
                    }
                }

                // get the active config ID from the native engine
                long configID = this.Execute(() =>
                {
                    NativeEngine nativeEngine = this.coreEngine.GetNativeApi();
                    long returnCode = nativeEngine.GetActiveConfigID(out long result);
                    this.HandleReturnCode(returnCode, nativeEngine);
                    return result;
                });

                // return the config ID
                return configID;

            }
            finally
            {
                localLock = this.ReleaseLock(localLock);
            }
        }

        /// <summary>
        /// Implemented to call the underlying native method to reinitialize.
        /// </summary>
        public void Reinitialize(long configID)
        {
            ReaderWriterLockSlim localLock = null;
            try
            {
                // get an exclusive write lock
                localLock = this.AcquireWriteLock();

                lock (this.monitor)
                {
                    // set the config ID for future native initializations
                    this.configID = configID;

                    // check if we have already initialized the engine or diagnostic
                    if (this.coreEngine != null)
                    {
                        // engine already initialized so we need to reinitialize
                        this.Execute<object>(() =>
                        {
                            long returnCode = this.coreEngine.GetNativeApi().Reinit(configID);
                            this.HandleReturnCode(returnCode, this.coreEngine.GetNativeApi());
                            return null;
                        });

                    }
                    else if (this.coreDiagnostic != null)
                    {
                        // diagnostic already initialized so we need to reinitialize
                        // NOTE: we do not need to do this if we reinitialized the
                        // engine since the configuration ID is globally set
                        this.Execute<object>(() =>
                        {
                            long returnCode = this.coreDiagnostic.GetNativeApi().Reinit(configID);
                            this.HandleReturnCode(returnCode, this.coreDiagnostic.GetNativeApi());
                            return null;
                        });
                    }
                    else
                    {
                        // force initialization to ensure the configuration ID is valid
                        this.GetEngine();
                    }
                }
            }
            finally
            {
                localLock = this.ReleaseLock(localLock);
            }
        }

        /// <summary>
        /// Provides an interface for initializing an instance of
        /// <see cref="SzCoreEnvironment"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This interface is not needed to use <see cref="SzCoreEnvironment"/>.
        /// It is only needed if you are extending <see cref="SzCoreEnvironment"/>.
        /// </para>
        /// 
        /// <para>
        /// This is provided for derived classes of <see cref="SzCoreEnvironment"/>
        /// to initialize their super class and is typically implemented by
        /// extending <see cref="AbstractBuilder{E,B}"/> in creating a derived
        /// builder implementation.
        /// </para>
        /// </remarks>
        public interface Initializer
        {
            /// <summary>
            /// Gets the Senzing settings which which to initialize 
            /// the <see cref="SzCoreEnvironment" />.
            /// </summary>
            /// 
            /// <returns>
            /// The Senzing settings which which to initialize the 
            /// <see cref="SzCoreEnvironment" />.
            /// </returns>
            string GetSettings();

            /// <summary>
            /// Gets the Senzing instance name with which to initialize
            /// the <see cref="SzCoreEnvironment" />.
            /// </summary>
            /// 
            /// <returns>
            /// The Senzing instance name with which to initialize
            /// the <see cref="SzCoreEnvironment" />.
            /// </returns>
            string GetInstanceName();

            /// <summary>
            /// Checks if initializing the <see cref="SzCoreEnvironment"/>
            /// with verbose logging.
            /// </summary>
            /// 
            /// <returns>
            /// <c>true</c> if verbose logging will be enabled, otherwise
            /// <c>false</c>.
            /// </returns>
            bool IsVerboseLogging();

            /// <summary>
            /// Gets the explicit configuration ID (if any) with which to 
            /// initialize the <see cref="SzCoreEnvironment"/>.
            /// </summary>
            /// 
            /// <remarks>
            /// This returns <c>null</c> if no explicit configuration ID has
            /// been provided and the default configuration ID from the
            /// Senzing repository should be used.
            /// </remarks>
            /// 
            /// <returns>
            /// The explicit configuration ID with which to initialize the 
            /// <see cref="SzCoreEnvironment"/>, or <c>null</c> if no explicit 
            /// configuration ID has been provided and the the default
            /// configuration ID from the Senzing repository should be used.
            /// </returns>
            long? GetConfigID();
        }

        /// <summary>
        /// Provides a base class for builder implementations of
        /// <see cref="SzCoreEnvironment"/> and its derived classes.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This interface is not needed to use <see cref="SzCoreEnvironment"/>.
        /// It is only needed if you are extending <see cref="SzCoreEnvironment"/>.
        /// </para>
        /// 
        /// <para>
        /// This allows the derived builder to return references to its own 
        /// environment class and to its own builder class rather than
        /// base classes.  When extending <see cref="SzCoreEnvironment"/> you 
        /// should provide an implementation of this class that is specific to
        /// your derived class.
        /// </para>
        /// </remarks>
        /// 
        /// <typeparam name="E">
        /// The <see cref="SzCoreEnvironment"/>-derived class build by
        /// instances of this class.
        /// </typeparam>
        /// 
        /// <typeparam name="B">
        /// The <see cref="AbstractBuilder{E,B}"/>-derived class of the
        /// implementation.
        /// </typeparam>
        public abstract class AbstractBuilder<E, B> : Initializer
            where E : SzCoreEnvironment
            where B : AbstractBuilder<E, B>
        {
            /// <summary>
            /// The settings for the builder which default to
            /// <see cref="SzCoreEnvironment.DefaultSettings" />
            /// </summary>
            private string settings = SzCoreEnvironment.DefaultSettings;

            /// <summary>
            /// The instance name for the builder which defaults to
            /// <see cref="SzCoreEnvironment.DefaultInstanceName" />
            /// </summary>
            private string instanceName = SzCoreEnvironment.DefaultInstanceName;

            /// <summary>
            /// The verbose logging setting for the builder which defaults
            /// to <c>false</c>.
            /// </summary>
            private bool verboseLogging = false;

            /// <summary>
            /// The config ID for the builder.
            /// </summary>
            /// 
            /// <remarks>
            /// If not provided explicitly then the configured default configuration
            /// in the Senzing repository is used.
            /// </remarks>
            private long? configID = null;

            /// <summary>Protected default constructor.</summary>
            protected internal AbstractBuilder()
            {
                this.settings = DefaultSettings;
                this.instanceName = DefaultInstanceName;
                this.verboseLogging = false;
                this.configID = null;
            }

            /// <summary>
            /// Provides the Senzing settings to initialize the
            /// <see cref="SzCoreEnvironment" />.
            /// </summary>
            /// 
            /// <remarks>
            /// If this is set to <c>null</c> or empty-string then
            /// <see cref="SzCoreEnvironment.DefaultSettings" /> will 
            /// be used to provide limited functionality.
            /// </remarks>
            /// 
            /// <param name="settings">
            /// The Senzing settings, or <c>null</c> or empty-string 
            /// to restore the default value.
            /// </param>
            /// 
            /// <returns>A reference to this instance.</returns>
            /// 
            /// <seealso cref="SzCoreEnvironment.DefaultSettings" />
            public B Settings(string settings)
            {
                if (settings != null && settings.Trim().Length == 0)
                {
                    settings = null;
                }
                this.settings = (settings == null)
                    ? DefaultSettings : settings.Trim();
                return ((B)this);
            }

            /// <summary>
            /// Gets the Senzing settings which which to initialize 
            /// the <see cref="SzCoreEnvironment" />.
            /// </summary>
            /// 
            /// <returns>
            /// The Senzing settings which which to initialize the 
            /// <see cref="SzCoreEnvironment" />.
            /// </returns>
            public string GetSettings()
            {
                return this.settings;
            }

            /// <summary>
            /// Provides the Senzing instance name to initialize the
            /// <see cref="SzCoreEnvironment" />
            /// </summary>
            /// 
            /// <remarks>
            /// Call this method to override the default value of
            /// <see cref="SzCoreEnvironment.DefaultInstanceName" />
            /// </remarks>
            /// 
            /// <param name="instanceName">
            /// The instance name to initialize the <see cref="SzCoreEnvironment"/>
            /// or <c>null</c> or empty-string to restore the default.
            /// </param>
            /// 
            /// <returns>A reference to this instance</returns>
            /// 
            /// <seealso cref="SzCoreEnvironment.DefaultInstanceName"/>
            public B InstanceName(string instanceName)
            {
                if (instanceName != null && instanceName.Trim().Length == 0)
                {
                    instanceName = null;
                }
                this.instanceName = (instanceName == null)
                    ? DefaultInstanceName : instanceName.Trim();
                return ((B)this);
            }

            /// <summary>
            /// Gets the Senzing instance name with which to initialize
            /// the <see cref="SzCoreEnvironment" />.
            /// </summary>
            /// 
            /// <returns>
            /// The Senzing instance name with which to initialize
            /// the <see cref="SzCoreEnvironment" />.
            /// </returns>
            public string GetInstanceName()
            {
                return this.instanceName;
            }

            /// <summary>
            /// Sets the verbose logging flag for initializing the 
            /// <see cref="SzCoreEnvironment"/>.
            /// </summary>
            /// 
            /// <remarks>
            /// Call this method to explicitly set the value.  If not
            /// called, the default value is <c>false</c>.
            /// </remarks>
            /// 
            /// <param name="verboseLogging">
            /// <c>true</c> if verbose logging should be enabled, 
            /// otherwise <c>false</c>.
            /// </param>
            /// 
            /// <returns>A reference to this instance</returns>
            public B VerboseLogging(bool verboseLogging)
            {
                this.verboseLogging = verboseLogging;
                return ((B)this);
            }

            /// <summary>
            /// Checks if initializing the <see cref="SzCoreEnvironment"/>
            /// with verbose logging.
            /// </summary>
            /// 
            /// <returns>
            /// <c>true</c> if verbose logging will be enabled, otherwise
            /// <c>false</c>.
            /// </returns>
            public bool IsVerboseLogging()
            {
                return this.verboseLogging;
            }

            /// <summary>
            /// Sets the explicit configuration ID to use to initialize the 
            /// <c>SzCoreEnvironment</c>.
            /// </summary>
            /// 
            /// <remarks>
            /// If not specified then the default configuration ID obtained
            /// from the Senzing repository is used.
            /// </remarks>
            /// 
            /// <param name="configID">
            /// The explicit configuration ID to use to initialize the
            /// <c>SzCoreEnvironment</c>, or <c>null</c> if the default
            /// configuration ID from the Senzing repository should be used.
            /// </param>
            /// 
            /// <returns>A reference to this instance.</returns>
            public B ConfigID(long? configID)
            {
                this.configID = configID;
                return ((B)this);
            }

            /// <summary>
            /// Gets the explicit configuration ID (if any) with which to 
            /// initialize the <see cref="SzCoreEnvironment"/>.
            /// </summary>
            /// 
            /// <remarks>
            /// This returns <c>null</c> if no explicit configuration ID has
            /// been provided and the default configuration ID from the
            /// Senzing repository should be used.
            /// </remarks>
            /// 
            /// <returns>
            /// The explicit configuration ID with which to initialize the 
            /// <see cref="SzCoreEnvironment"/>, or <c>null</c> if no explicit 
            /// configuration ID has been provided and the the default
            /// configuration ID from the Senzing repository should be used.
            /// </returns>
            public long? GetConfigID()
            {
                return this.configID;
            }

            /// <summary>
            /// Creates a new <see cref="SzCoreEnvironment"/> instance of
            /// type <c>E</c> based on this builder instance.
            /// </summary>
            /// 
            /// <remarks>
            /// This method will throw an <see cref="InvalidOperationException"/>
            /// if another active <see cref="SzCoreEnvironment"/> instance exists
            /// since only one active instance can exist within a process at any
            /// given time.  An active instance is one that has been constructed,
            /// but has not yet been destroyed.
            /// </remarks>
            /// 
            /// <returns>
            /// The newly created <see cref="SzCoreEnvironment"/> instance of
            /// type <c>E</c>.
            /// </returns>
            /// 
            /// <exception cref="InvalidOperationException">
            /// If another active <see cref="SzCoreEnvironment"/> instance exists
            /// when this method is invoked.
            /// </exception>
            public abstract E Build();
        }

        /// <summary>
        /// The builder class for creating an instance of <see cref="SzCoreEnvironment"/>.
        /// </summary>
        public class Builder : AbstractBuilder<SzCoreEnvironment, Builder>
        {
            /// <summary>Protected default constructor.</summary>
            protected internal Builder() : base()
            {
                // do nothing
            }

            /// <summary>
            /// Creates a new <see cref="SzCoreEnvironment"/> instance based
            /// this <c>Builder</c> instance.
            /// </summary>
            /// 
            /// <remarks>
            /// This method will throw an <see cref="InvalidOperationException"/>
            /// if another active <see cref="SzCoreEnvironment"/> instance exists since
            /// only one active instance can exist within a process at any given time.
            /// An active instance is one that has been constructed, but has not yet
            /// been destroyed.
            /// </remarks>
            /// 
            /// <returns>
            /// The newly created <see cref="SzCoreEnvironment"/> instance.
            /// </returns>
            /// 
            /// <exception cref="InvalidOperationException">
            /// If another active <see cref="SzCoreEnvironment"/> instance exists when
            /// this method is invoked.
            /// </exception>
            public override SzCoreEnvironment Build()
            {
                return new SzCoreEnvironment(this);
            }
        }

        /// <summary>
        /// Attempts to acquire an exclusive write lock from this
        /// instance's <see cref="System.Threading.ReaderWriterLockSlim"/>
        /// if the lock is not already held.
        /// </summary>
        /// 
        /// <returns>
        /// This returns the <see cref="System.Threading.ReaderWriterLockSlim"/>
        /// for this instance if the lock was not held and was obtained by this
        /// call, otherwise it returns <c>null</c> to indicate the lock was 
        /// already held.
        /// </returns>
        private ReaderWriterLockSlim AcquireWriteLock()
        {
            if (this.readWriteLock.IsWriteLockHeld)
            {
                return null;
            }
            if (this.readWriteLock.IsReadLockHeld)
            {
                throw new InvalidOperationException(
                    "Attempt to obtain write lock while read lock was held.");
            }
            this.readWriteLock.EnterWriteLock();
            return this.readWriteLock;
        }

        /// <summary>
        /// Attempts to acquire a shared read lock from this
        /// instance's <see cref="System.Threading.ReaderWriterLockSlim"/>
        /// if the lock is not already held.
        /// </summary>
        /// 
        /// <returns>
        /// This returns the <see cref="System.Threading.ReaderWriterLockSlim"/>
        /// for this instance if the lock was not held and was obtained by this
        /// call, otherwise it returns <c>null</c> to indicate the lock was 
        /// already held.
        /// </returns>
        private ReaderWriterLockSlim AcquireReadLock()
        {
            if (this.readWriteLock.IsWriteLockHeld
                || this.readWriteLock.IsReadLockHeld)
            {
                return null;
            }
            this.readWriteLock.EnterReadLock();
            return this.readWriteLock;
        }

        /// <summary>
        /// Releases any locks held on the specified 
        /// <see cref="System.Threading.ReaderWriterLockSlim"/> providing it is
        /// not <c>null</c>.
        /// </summary>
        /// 
        /// <remarks>
        /// If the specified parameter is <c>null</c> then this method does
        /// nothing and simply returns <c>null</c>.
        /// </remarks>
        /// 
        /// <param name="localLock">
        /// The <see cref="System.Threading.ReaderWriterLockSlim"/> to release the
        /// lock on, or <c>null</c> if no lock was actually obtained by the calling
        /// stack frame.
        /// </param>
        /// 
        /// <returns>
        /// Always returns <c>null</c> so the calling stack frame can set its 
        /// local variable to the result of this function on a single line.
        /// </returns>.
        private ReaderWriterLockSlim ReleaseLock(ReaderWriterLockSlim localLock)
        {
            if (localLock == null)
            {
                return null;
            }
            if (localLock.IsWriteLockHeld)
            {
                localLock.ExitWriteLock();
            }
            else if (localLock.IsReadLockHeld)
            {
                localLock.ExitReadLock();
            }
            return null;
        }

    }
}