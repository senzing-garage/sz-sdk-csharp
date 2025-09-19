
using Senzing.Sdk;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzDiagnostic"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreDiagnostic : SzDiagnostic
    {

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeDiagnostic"/>.
        /// </summary>
        private NativeDiagnostic nativeApi = null;

        /// <summary>
        /// Internal object for instance-wide synchronized locking.
        /// </summary>
        private readonly object monitor = new object();

        /// <summary>
        /// Constructs with the specified <see cref="SzCoreEnvironment"/>.
        /// </summary>
        /// 
        /// <param name="env">
        /// The <see cref="SzCoreEnvironment"/> that is constructing this instance
        /// and will be used by this instance.
        /// </param>
        public SzCoreDiagnostic(SzCoreEnvironment env)
        {
            this.env = env;
            this.env.Execute<object>(() =>
            {
                // construct the native delegate
                this.nativeApi = new NativeDiagnosticExtern();

                long? configID = this.env.GetConfigID();
                // check if we are initializing with a config ID
                if (configID == null)
                {
                    // initialize the native delegate
                    long returnCode = this.nativeApi.Init(
                        this.env.GetInstanceName(),
                        this.env.GetSettings(),
                        this.env.IsVerboseLogging());

                    // handle the return code
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                }
                else
                {
                    // initialize the native delegate
                    long returnCode = this.nativeApi.InitWithConfigID(
                        this.env.GetInstanceName(),
                        this.env.GetSettings(),
                        configID ?? 0L,
                        this.env.IsVerboseLogging());

                    // handle the return code
                    this.env.HandleReturnCode(returnCode, this.nativeApi);
                }

                // no return value so return null
                return null;
            });
        }

        /// <summary>
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeDiagnostic"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeDiagnostic"/>
        /// instance.
        /// </returns>
        internal NativeDiagnostic GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// The package-protected function to destroy the Senzing Diagnostic SDK.
        /// </summary>
        internal void Destroy()
        {
            lock (this.monitor)
            {
                if (this.nativeApi == null)
                {
                    return;
                }
                this.nativeApi.Destroy();
                this.nativeApi = null;
            }
        }

        /// <summary>
        /// Checks if this instance has been destroyed by the associated
        /// <see cref="Senzing.Sdk.Core.SzCoreEnvironment"/>.
        /// </summary>
        ///
        /// <returns>
        /// <c>true</c> if this instance has been destroyed, otherwise <c>false</c>.
        /// </returns>
        internal bool IsDestroyed()
        {
            lock (this.monitor)
            {
                return (this.nativeApi == null);
            }
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzDiagnostic_getDatastoreInfo_helper"</c> via
        /// <see cref="NativeDiagnostic.GetRepositoryInfo(out string)"/>. 
        /// </summary>
        public string GetRepositoryInfo()
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.GetRepositoryInfo(out string info);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the JSON from the string buffer
                return info;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzDiagnostic_checkDatastorePerformance_helper"</c> via
        /// <see cref="NativeDiagnostic.CheckRepositoryPerformance(int, out string)"/>.
        /// </summary>
        public string CheckRepositoryPerformance(int secondsToRun)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.CheckRepositoryPerformance(
                    secondsToRun, out string result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the JSON from the string buffer
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzDiagnostic_checkDatastorePerformance_helper"</c> via
        /// <see cref="NativeDiagnostic.GetFeature(long, out string)"/>.
        /// </summary>
        public string GetFeature(long featureID)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.GetFeature(
                    featureID, out string result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the JSON from the string buffer
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzDiagnostic_purgeRepository"</c> via
        /// <see cref="NativeDiagnostic.PurgeRepository"/>.
        /// </summary>
        public void PurgeRepository()
        {
            this.env.Execute<object>(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.PurgeRepository();

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return null;
            });

        }
    }
}