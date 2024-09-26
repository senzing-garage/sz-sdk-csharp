using Senzing.Sdk;
using System;
using System.Threading;


namespace Senzing.Sdk.Core {
/// <summary>
/// Provides the core implementation of <see cref="Senzing.Sdk.SzEnvironment"/>
/// that directly initializes the Senzing SDK modules and provides management
/// of the Senzing environment in this process.
/// </summary>
///
/// <seealso cref="Senzing.Sdk.SzEnvironment"/> 
public class SzCoreEnvironment: SzEnvironment {
    /// <summary>
    ///  The defaut instance name to use for the Senzing initialization.
    ///  The value is <c>"Senzing Instance"</c>.
    /// </summary>
    /// <remarks>
    /// An explicit value can be provided via <see cref="Builder.InstanceName"/> 
    /// during initialization.
    /// </remarks>
    /// 
    /// <seealso cref="Builder.InstanceName" />
    public const string DEFAULT_INSTANCE_NAME = "Senzing Instance";

    /// <summary>
    /// The default "bootstrap" settings with which to initialize the
    /// <c>SzCoreEnvironment</c> when an explicit settings value has not been
    /// provided via <see cref="Builder.Settings(string)" />.
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
    /// </remarks>
    /// 
    /// <seealso cref="Builder.Settings" />
    public const string DEFAULT_SETTINGS = "{ }";

    /// <summary>
    /// The number of milliseconds to delay (if not notified) until checking
    /// if we are ready to destroy.
    /// </summary>
    /// 
    private const int DestroyDelay = 5000;

    /// <summary>
    /// Internal object for class-wide synchronized locking.
    /// </summary>
    private static readonly Object CLASS_MONITOR = new Object();

    /// <summary>
    /// Enumerates the possible states for an instance of <c>SzCoreEnvironment</c>.
    /// </summary>
    private enum State {
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
    public static Builder NewBuilder() {
        return new Builder();
    }

    /// The current instance of the <c>SzCoreEnvironment</c>
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
    public static SzCoreEnvironment GetActiveInstance() {
        lock (CLASS_MONITOR) {
            if (currentInstance == null) {
                return null;
            }
            Interlocked.MemoryBarrier();
            lock (currentInstance.monitor) {
                State state = currentInstance.state;
                switch (state) {
                    case State.Destroying:
                        // wait until destroyed and fall through
                        WaitUntilDestroyed(currentInstance);
                        goto case State.Destroying;
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
    private string instanceName = null;
    
    /// <summary>The settings to initialize the API's with.</summary>
    private string settings = null;
    
    /// <summary>The flag indicating if verbose logging is enabled.</summary>
    private bool verboseLogging = false;
    
    /// <summary>
    /// The explicit configuration ID to initialize with or <c>null</c> if
    /// using the default configuration.
    /// </summary>
    private long? configId = null;

    /// <summary>
    /// The <see cref="SzCoreProduct"/> instance to use.
    /// </summary>
    private SzCoreProduct coreProduct = null;
    
    /// <summary>
    /// The <see cref="SzCoreConfig"/> instance to use.
    /// </summary>
    private SzCoreConfig coreConfig = null;

    /// <summary>
    /// The <see cref="SzCoreEngine"/> intance to use.
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
    
    /// The <c>ReaderWriterLock</c> for this instance.
    private readonly ReaderWriterLockSlim readWriteLock;

    /// <summary>
    /// Internal object for instance-wide synchronized locking.
    /// </summary>
    private readonly Object monitor = new Object();
    
    /// <summary>
    /// Private constructor used by the builder to construct the instance.
    /// </summary>
    ///  
    /// <param name="instanceName">The Senzing instance name.</param>
    /// 
    /// <param name="settings">The Senzing core settings.</param>
    /// 
    /// <param name="verboseLogging">
    /// The verbose logging setting for Senzing environment.
    /// </param>
    /// 
    /// <param name="configId">
    /// The explicit config ID for the Senzing environment initialization,
    /// or <code>null</code> if using the default configuration.
    /// </param>
    private SzCoreEnvironment(string    instanceName,
                              string    settings,
                              bool      verboseLogging,
                              long?     configId) 
    {
        // set the fields
        this.readWriteLock  = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        this.instanceName   = instanceName;
        this.settings       = settings;
        this.verboseLogging = verboseLogging;
        this.configId       = configId;

        lock (CLASS_MONITOR) {
            SzCoreEnvironment activeEnvironment = GetActiveInstance();
            if (activeEnvironment != null) {
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
        if (environment == null) {
            throw new ArgumentNullException("The specified instance cannot be null");
        }
        lock (environment.monitor) {
            while (environment.state != State.Destroyed) {
                try {
                    Monitor.Wait(environment, DestroyDelay);
                } catch (ThreadInterruptedException) {
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
    internal string GetInstanceName() {
        return this.instanceName;
    }

    
    /// <summary>
    /// Gets the associated Senzing settings for initialization.
    /// </summary>
    /// 
    /// <returns>The associated Senzing settings for initialization.</returns>
    internal string GetSettings() {
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
    internal bool IsVerboseLogging() {
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
    /// <returns>
    /// The explicit configuration ID with which to initialize the Senzing
    /// environment, or <c>null</c> if the default configuration ID
    /// configured in the repository should be used.
    /// </returns>
    internal long? GetConfigId() {
        return this.configId;
    }
    
    /// <summary>
    /// Executes the specified task (<c>Func</c>) and returns the result if successful.
    /// </summary>
    /// 
    /// <remarks>
    /// This will throw any exception produced by the specified task, wrapping it in
    /// an <see cref="Senzing.Sdk.SzException"/> if it is a that is not of type {@link SzException}.
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
    internal T execute<T>(Func<T> task)
    {
        ReaderWriterLockSlim localLock = null;
        try {
            // acquire a wrie lock while checking if acive
            localLock = this.AcquireReadLock();
            lock (this.monitor) {
                if (this.state != State.Active) {
                    throw new InvalidOperationException(
                        "SzEnvironment has been destroyed");
                }

                // increment the executing count
                Interlocked.Increment(ref this.executingCount);
            }
        
            return task();

        } finally {
            lock (this.monitor) {
                Interlocked.Decrement(ref this.executingCount);
                Monitor.PulseAll(this);
            }
            localLock = this.ReleaseLock(localLock);
        }
    }
    
    /// <summary>
    /// Gets the number of currently executing operations.
    /// </summary>
    /// 
    /// <returns>
    /// The number of currently executing operations.
    /// </returns>
    internal int GetExecutingCount() {
        lock (this.monitor) {
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
    internal void EnsureActive() {
        lock (CLASS_MONITOR) {
            if (this.state != State.Active) {
                throw new InvalidOperationException(
                    "The SzCoreEnvironment instance has already been destroyed.");
            }
        }
    }

    /// <summary>
    /// Implemented to return the <see cref="SzCoreConfig"/> associated
    /// with this instance.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="SzCoreConfig"/> associated with this instance.
    /// </returns>
    public SzConfig GetConfig() {
        lock (this.monitor) {
            this.EnsureActive();
            if (this.coreConfig == null) {
                this.coreConfig = new SzCoreConfig(this);
            }

            // return the configured instance
            return this.coreConfig;
        }
    }

    /// <summary>
    /// Implemented to return the <see cref="SzCoreConfigManager"/>
    /// associated with this instance.
    /// </summary>
    /// 
    /// <returns>
    /// The <see cref="SzCoreConfigManager"/> associated with this instance.
    /// </returns>
    public SzConfigManager GetConfigManager() {
        lock (this.monitor) {
            this.EnsureActive();
            if (this.coreConfigMgr == null) {
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
    public SzDiagnostic GetDiagnostic() {
        lock (this.monitor) {
            this.EnsureActive();
            if (this.coreDiagnostic == null) {
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
    public SzEngine GetEngine() {
        lock (this.monitor) {
            this.EnsureActive();
            if (this.coreEngine == null) {
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
    public SzProduct GetProduct() {
        lock (this.monitor) {
            this.EnsureActive();
            if (this.coreProduct == null) {
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
    public void Destroy() {
        ReaderWriterLockSlim localLock = null;
        try {
            lock (this.monitor) {
                // check if this has already been called
                if (this.state != State.Active) {
                    return;
                }

                // set the flag for destroying
                this.state = State.Destroying;
                Monitor.PulseAll(this);
            }

            // acquire an exclusive lock for destroying to ensure
            // all executing tasks have completed
            localLock = this.AcquireWriteLock();

            // ensure completion of in-flight executions
            int exeCount = this.GetExecutingCount();
            if (exeCount > 0) {
                throw new InvalidOperationException(
                    "Acquired write lock for destroying environment while tasks "
                    + "still exuecting: " + exeCount);
            }

            // once we get here we can really shut things down
            if (this.coreEngine != null) {
                this.coreEngine.Destroy();
                this.coreEngine = null;
            }
            if (this.coreDiagnostic != null) {
                this.coreDiagnostic.Destroy();
                this.coreDiagnostic = null;
            }
            if (this.coreConfigMgr != null) {
                this.coreConfigMgr.Destroy();
                this.coreConfigMgr = null;
            }
            if (this.coreConfig != null) {
                this.coreConfig.Destroy();
                this.coreConfig = null;
            }
            if (this.coreProduct != null) {
                this.coreProduct.Destroy();
                this.coreProduct = null;
            }

            // set the state
            lock (this.monitor) {
                this.state = State.Destroyed;
                Monitor.PulseAll(this);
            }
        } finally {
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
    public bool IsDestroyed() {
        lock (this.monitor) {
            return this.state != State.Active;
        }
    }

    /// <summary>
    /// The builder class for creating an instance of <see cref="SzCoreEnvironment"/>.
    /// </summary>
    public class Builder 
    {
        /// <summary>
        /// The settings for the builder which default to
        /// <see cref="SzCoreEnvironment.DEFAULT_SETTINGS" />
        /// </summary>
        private string settings = SzCoreEnvironment.DEFAULT_SETTINGS;

        /// <summary>
        /// The instance name for the builder which defaults to
        /// <see cref="SzCoreEnvironment.DEFAULT_INSTANCE_NAME" />
        /// </summary>
        private string instanceName = SzCoreEnvironment.DEFAULT_INSTANCE_NAME;

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
        private long? configId = null;

        /// <summary>Default constructor.</summary>
        public Builder() {
            this.settings       = DEFAULT_SETTINGS;
            this.instanceName   = DEFAULT_INSTANCE_NAME;
            this.verboseLogging = false;
            this.configId       = null;
        }

        /// <summmary>
        /// Provides the Senzing settings to configure the
        /// <see cref="SzCoreEnvironment" />.
        /// </summary>
        /// 
        /// <remarks>
        /// If this is set to <c>null</c> or empty-string then
        /// <see cref="SzCoreEnvironment.DEFAULT_SETTINGS" /> will be used to provide
        /// limited funtionality.
        /// </remarks>
        /// 
        /// <param name="settings">
        /// The Senzing settings, or <c>null</c> or empty-string to restore the default value.
        /// </param>
        /// 
        /// <returns>A reference to this instance.</returns>
        /// 
        /// <seealso cref="SzCoreEnvironment.DEFAULT_SETTINGS" />
        public Builder Settings(string settings) {
            if (settings != null && settings.Trim().Length == 0) {
                settings = null;
            }
            this.settings = (settings == null)
                ? DEFAULT_SETTINGS : settings.Trim();
            return this;
        }

        /// <summary>
        /// Provides the Senzing instance name to configure the
        /// <see cref="SzCoreEnvironment" />
        /// </summary>
        /// 
        /// <remarks>
        /// Call this method to override the default value of
        /// <see cref="SzCoreEnvironment.DEFAULT_INSTANCE_NAME" />
        /// </remarks>
        /// 
        /// <param name="instanceName">
        /// The instance name to initialize the <see cref="SzCoreEnvironment"/>
        /// or <c>null</c> or empty-string to restore the default.
        /// </param>
        /// 
        /// <returns>A reference to this instance</returns>
        /// 
        /// <seealso cref="SzCoreEnvironment.DEFAULT_INSTANCE_NAME"/>
        public Builder InstanceName(string instanceName) {
            if (instanceName != null && instanceName.Trim().Length == 0) {
                instanceName = null;
            }
            this.instanceName = (instanceName == null) 
                ? DEFAULT_INSTANCE_NAME : instanceName.Trim();
            return this;
        }

        /// <summary>
        /// Sets the verbose logging flag for configuring the 
        /// <see cref="SzCoreEnvironment"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// Call this method to explicitly set the value.  If not called, the
        /// default value is <c>false</c>.
        /// </remarks>
        /// 
        /// <param name="verboseLogging">
        /// <c>true</c> if verbose logging should be enabled, otherwise <c>false</c>.
        /// </param>
        /// 
        /// <returns>A reference to this instance</returns>
        public Builder VerboseLogging(bool verboseLogging) {
            this.verboseLogging = verboseLogging;
            return this;
        }

        /// Sets the explicit configuration ID to use to initialize the {@link
        /// SzCoreEnvironment}.  If not specified then the default configuration
        /// ID obtained from the Senzing repository is used.
        /// 
        /// @param configId The explicit configuration ID to use to initialize the
        ///                 {@link SzCoreEnvironment}, or <code>null</code> if the
        ///                 default configuration ID from the Senzing repository
        ///                 should be used.
        /// 
        /// @return A reference to this instance.
        
        public Builder ConfigId(long? configId) {
            this.configId = configId;
            return this;
        }

        /// <summary>
        /// This method creates a new <see cref="SzCoreEnvironment"/> instance
        /// based on this <c>Builder</c> instance.
        /// </summary>
        /// 
        /// <remarks>
        /// This method will throw an <see cref="System.IllegalOperationExeption"/>
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
        /// <exception cref="System.InvalidOperationException">
        /// If another active <see cref="SzCoreEnvironment"/> instance exists when
        /// this method is invoked.
        /// </exception>
        public SzCoreEnvironment Build()
        {
            return new SzCoreEnvironment(this.instanceName,
                                         this.settings,
                                         this.verboseLogging,
                                         this.configId);
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
    private ReaderWriterLockSlim AcquireWriteLock() {
        if (this.readWriteLock.IsWriteLockHeld) {
            return null;
        }
        if (this.readWriteLock.IsReadLockHeld) {
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
    private ReaderWriterLockSlim AcquireReadLock() {
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
    /// <param name="lock">
    /// The <see cref="System.Threading.ReaderWriterLockSlim"/> to release the
    /// lock on, or <c>null</c> if no lock was actually obtained by the calling
    /// stack frame.
    /// </param>
    /// 
    /// <returns>
    /// Always returns <c>null</c> so the calling stack frame can set its 
    /// local variable to the result of this function on a single line.
    /// </returns>.
    private ReaderWriterLockSlim ReleaseLock(ReaderWriterLockSlim localLock) {
        if (localLock == null) {
            return null;
        }
        if (localLock.IsWriteLockHeld) {
            localLock.ExitWriteLock();
        } else if (localLock.IsReadLockHeld) {
            localLock.ExitReadLock();
        }
        return null;
    }

}
}