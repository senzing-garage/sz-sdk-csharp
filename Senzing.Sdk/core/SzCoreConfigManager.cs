using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using static Senzing.Sdk.Core.SzCoreUtilities;

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
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfigExtern"/>.
        /// </summary>
        private NativeConfigExtern configApi = null;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfigManagerExtern"/>.
        /// </summary>
        private NativeConfigManagerExtern configMgrApi = null;

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
                this.configApi = new NativeConfigExtern();

                // initialize the native delegate
                long returnCode = this.configApi.Init(this.env.GetInstanceName(),
                                                      this.env.GetSettings(),
                                                      this.env.IsVerboseLogging());

                // handle the return code
                this.env.HandleReturnCode(returnCode, this.configApi);

                // no return value so return null
                return null;
            });
        }

        /// <summary>
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeConfigExtern"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeConfigExtern"/>
        /// instance.
        /// </returns>
        internal NativeConfigExtern GetConfigApi()
        {
            return this.configApi;
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
        internal NativeConfigManagerExtern GetConfigManagerApi()
        {
            lock (this.monitor)
            {
                if (this.configMgrApi == null)
                {
                    this.env.Execute<object>(() =>
                    {
                        this.configMgrApi = new NativeConfigManagerExtern();

                        // initialize the native delegate
                        long returnCode = this.configMgrApi.Init(
                            this.env.GetInstanceName(),
                            this.env.GetSettings(),
                            this.env.IsVerboseLogging());

                        // handle the return code
                        this.env.HandleReturnCode(returnCode, this.configMgrApi);

                        // no return value, so return null
                        return null;
                    });
                }

                // return the config manager API
                return this.configMgrApi;
            }
        }

        /// <summary>
        /// The package-protected function to destroy the Senzing ConfigManager SDK.
        /// </summary>
        internal void Destroy()
        {
            lock (this.monitor)
            {
                // destroy the config manager API
                if (this.configMgrApi != null)
                {
                    this.configMgrApi.Destroy();
                    this.configMgrApi = null;
                }

                // destroy the config API
                if (this.configApi != null)
                {
                    this.configApi.Destroy();
                    this.configApi = null;
                }
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
                return (this.configApi == null);
            }
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_create_helper"</c> via
        /// <see cref="NativeConfigExtern.Create(out IntPtr)"/>.
        /// </summary>
        public SzConfig CreateConfig()
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.configApi.Create(out IntPtr configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.configApi);

                try
                {
                    // export the config definition
                    returnCode = this.configApi.Export(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.configApi);

                    // create and return a new SzConfig
                    return new SzCoreConfig(this.env, configDef);

                }
                finally
                {
                    // close the config handle
                    returnCode = this.configApi.Close(configHandle);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.configApi);
                }
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_load_helper"</c> via
        /// <see cref="NativeConfigExtern.Load(string, out IntPtr)"/>.
        /// </summary>
        public SzConfig CreateConfig(string configDefinition)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.configApi.Load(configDefinition,
                                                      out IntPtr configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.configApi);

                try
                {
                    // export the config definition
                    returnCode = this.configApi.Export(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.configApi);

                    // create and return a new SzConfig
                    return new SzCoreConfig(this.env, configDef);

                }
                finally
                {
                    // close the config handle
                    returnCode = this.configApi.Close(configHandle);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.configApi);
                }
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getConfig_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.GetConfig(long, out string)"/>. 
        /// </summary>
        public SzConfig CreateConfig(long configID)
        {
            return this.env.Execute(() =>
            {
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.GetConfig(
                    configID, out string configDef);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

                // return the config handle
                return new SzCoreConfig(this.env, configDef);
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_registerConfig_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.RegisterConfig(string, string, out long)"/>. 
        /// </summary>
        public long RegisterConfig(string configDefinition, string configComment)
        {
            return this.env.Execute(() =>
            {
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.RegisterConfig(
                    configDefinition, configComment ?? "", out long configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

                // return the config handle
                return configID;
            });
        }

        /// <summary>
        /// Implemented to call the <see cref="RegisterConfig(string,string)"/> 
        /// function with an auto-generated comment.
        /// </summary>
        public long RegisterConfig(string configDefinition)
        {
            // generate a configuration comment
            string configComment = CreateConfigComment(configDefinition);

            // return the result from the base method
            return this.RegisterConfig(configDefinition, configComment);
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getConfigList_helper"</c> via
        /// <see cref="NativeConfigManager.GetConfigRegistry(out string)"/>. 
        /// </summary>
        public string GetConfigRegistry()
        {
            return this.env.Execute(() =>
            {
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.GetConfigRegistry(out string configList);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

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
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.GetDefaultConfigID(out long configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

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
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.ReplaceDefaultConfigID(
                    currentDefaultConfigID, newDefaultConfigID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

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
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.SetDefaultConfigID(configID);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, nativeApi);

                // return null
                return null;
            });
        }


        /// <summary>
        /// Implemented to call <see cref="RegisterConfig(string,string)"/> and
        /// <see cref="SetDefaultConfigID(long)"/> functions.
        /// </summary> 
        public long SetDefaultConfig(string configDefinition, string configComment)
        {
            // register the configuration
            long configId = this.RegisterConfig(configDefinition, configComment);

            // set it as the default config ID
            this.SetDefaultConfigID(configId);

            // return the config ID
            return configId;
        }

        /// <summary>
        /// Implemented to call the <see cref="RegisterConfig(string)"/> and
        /// <see cref="SetDefaultConfigID(long)"/> functions.
        /// </summary>
        public long SetDefaultConfig(String configDefinition)
        {
            // register the configuration
            long configId = this.RegisterConfig(configDefinition);

            // set it as the default config ID
            this.SetDefaultConfigID(configId);

            // return the config ID
            return configId;
        }
    }
}