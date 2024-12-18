
using Senzing.Sdk;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// The internal core implementation of <see cref="Senzing.Sdk.SzProduct"/>
    /// that works with the <see cref="SzCoreEnvironment"/>. 
    /// </summary>
    internal class SzCoreProduct : SzProduct
    {

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeProductExtern"/>.
        /// </summary>
        private NativeProductExtern nativeApi = null;

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
        public SzCoreProduct(SzCoreEnvironment env)
        {
            this.env = env;
            this.env.Execute<object>(() =>
            {
                // construct the native delegate
                this.nativeApi = new NativeProductExtern();

                // initialize the native delegate
                long returnCode = this.nativeApi.Init(this.env.GetInstanceName(),
                                                      this.env.GetSettings(),
                                                      this.env.IsVerboseLogging());

                // handle the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // no return value so return null
                return null;
            });
        }

        /// <summary>
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeProductExtern"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeProductExtern"/>
        /// instance.
        /// </returns>
        internal NativeProductExtern GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// The package-protected function to destroy the Senzing Product SDK.
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
        /// Implemented to call the native <c>G2Product_license()</c>
        /// function via <see cref="NativeProductExtern.License"/> 
        /// </summary>
        /// 
        /// <seealso cref="Senzing.Sdk.SzProduct.GetLicense"/>
        public string GetLicense()
        {
            return this.env.Execute(() =>
            {
                return this.nativeApi.License();
            });
        }

        /// <summary>
        /// Implemented to call the native <c>G2Product_version()</c>
        /// function via <see cref="NativeProductExtern.Version"/>.
        /// </summary>
        /// 
        /// <seealso cref="Senzing.Sdk.SzProduct.GetLicense"/>
        public string GetVersion()
        {
            return this.env.Execute(() =>
            {
                return this.nativeApi.Version();
            });
        }
    }
}