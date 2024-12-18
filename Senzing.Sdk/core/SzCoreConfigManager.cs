using Senzing.Sdk;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzConfigManager"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreConfigManager : SzConfigManager
    {

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfigManagerExtern"/>.
        /// </summary>
        private NativeConfigManagerExtern nativeApi = null;

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
        public SzCoreConfigManager(SzCoreEnvironment env)
        {
            this.env = env;
            this.env.Execute<object>(() =>
            {
                // construct the native delegate
                this.nativeApi = new NativeConfigManagerExtern();

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
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeConfigManagerExtern"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeConfigManagerExtern"/>
        /// instance.
        /// </returns>
        internal NativeConfigManagerExtern GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// The package-protected function to destroy the Senzing ConfigManager SDK.
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
        /// <c>SzConfigMgr_addConfig_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.AddConfig(string, string, out long)"/>. 
        /// </summary>
        public long AddConfig(string configDefinition, string configComment)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.AddConfig(
                    configDefinition, configComment, out long configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return configID;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getConfig_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.GetConfig(long, out string)"/>. 
        /// </summary>
        public string GetConfig(long configID)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.GetConfig(
                    configID, out string config);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return config;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getConfigList_helper"</c> via
        /// <see cref="NativeConfigManager.GetConfigList(out string)"/>. 
        /// </summary>
        public string GetConfigs()
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.GetConfigList(out string configList);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return configList;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getDefaultConfigID_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.GetDefaultConfigID(out long)"/>. 
        /// </summary>
        public long GetDefaultConfigID()
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.GetDefaultConfigID(out long configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return configID;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_replaceDefaultConfigID"</c> via
        /// <see cref="NativeConfigManagerExtern.ReplaceDefaultConfigID(long, long)"/>. 
        /// </summary>
        public void ReplaceDefaultConfigID(long currentDefaultConfigID,
                                           long newDefaultConfigID)
        {
            this.env.Execute<object>(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.ReplaceDefaultConfigID(
                    currentDefaultConfigID, newDefaultConfigID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return null;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_setDefaultConfigID_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.SetDefaultConfigID(long)"/>. 
        /// </summary>
        public void SetDefaultConfigID(long configID)
        {
            this.env.Execute<object>(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.SetDefaultConfigID(configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return null;
            });
        }

    }
}