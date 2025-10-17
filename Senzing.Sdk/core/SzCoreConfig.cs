using System;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzConfig"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreConfig : SzConfig
    {
        // 
        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfig"/>.
        /// </summary>
        private readonly NativeConfig nativeApi = null;

        /// <summary>
        /// The backing config definition.
        /// </summary>
        private string configDefinition;

        /// <summary>
        /// Constructs with the specified <see cref="SzCoreEnvironment"/>,
        /// <see cref="NativeConfig"/> and <c>string</c> config definition. 
        /// </summary>
        /// 
        /// <remarks>
        /// This constructor is called by <see cref="SzCoreConfigManager"/>
        /// </remarks>
        ///  
        /// <param name="env">
        /// The <see cref="SzCoreEnvironment"/> that is constructing this instance
        /// and will be used by this instance.
        /// </param>
        /// 
        /// <param name="nativeConfig">
        /// The <see cref="NativeConfig"/> instance provided by the 
        /// <see cref="SzCoreConfigManager"/>.  
        /// </param>
        /// 
        /// <param name="configDefinition">
        /// The <c>string</c> config definition describing the configuration
        /// represented by this instance.
        /// </param>
        public SzCoreConfig(SzCoreEnvironment env,
                            NativeConfig nativeConfig,
                            string configDefinition)
        {
            if (env == null)
            {
                throw new ArgumentNullException(
                    nameof(env), "The specified environment cannot be null");
            }
            if (nativeConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(nativeConfig), "The NativeConfig cannot be null");
            }
            if (configDefinition == null)
            {
                throw new ArgumentNullException(
                    nameof(configDefinition),
                    "The specified config definition cannot be null");
            }

            // set the environment and config definition
            this.env = env;
            this.configDefinition = configDefinition;
            this.nativeApi = nativeConfig;
        }

        /// <summary>
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeConfig"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeConfig"/>
        /// instance.
        /// </returns>
        internal NativeConfig GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// Implemented to return the config definition for this instance.
        /// </summary>
        public string Export()
        {
            return this.env.Execute(() => this.configDefinition);
        }

        /// <summary>
        /// Implemented to call the <see cref="SzCoreUtilities.ConfigToString(SzConfig)"/>
        /// function and return the result.
        /// </summary>
        public override string ToString()
        {
            return SzCoreUtilities.ConfigToString(this);
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_listDataSources_helper"</c> via 
        /// <see cref="NativeConfig.GetDataSourceRegistry(IntPtr, out string)"/>.
        /// </summary>
        public string GetDataSourceRegistry()
        {
            return this.env.Execute(() =>
            {
                // load the configuration
                long returnCode = this.nativeApi.Load(this.configDefinition,
                                                      out IntPtr configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                try
                {
                    // call the underlying C function
                    returnCode = this.nativeApi.GetDataSourceRegistry(configHandle,
                                                                out string result);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // return the contents of the buffer
                    return result;

                }
                finally
                {
                    // close the config handle
                    returnCode = this.nativeApi.Close(configHandle);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);
                }
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_registerDataSource_helper"</c> via
        /// <see cref="NativeConfig.RegisterDataSource(IntPtr, string, out string)"/>. 
        /// </summary>
        public string RegisterDataSource(string dataSourceCode)
        {
            return this.env.Execute(() =>
            {
                // load the configuration
                long returnCode = this.nativeApi.Load(this.configDefinition,
                                                      out IntPtr configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                try
                {
                    // format the JSON for the native call
                    string inputJson = "{\"DSRC_CODE\":"
                                    + Utilities.JsonEscape(dataSourceCode) + "}";

                    // call the underlying C function
                    returnCode = this.nativeApi.RegisterDataSource(
                        configHandle, inputJson, out string result);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // export the new config
                    returnCode = this.nativeApi.Export(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // store the new config definition
                    this.configDefinition = configDef;

                    // return null
                    return result;

                }
                finally
                {
                    // close the config handle
                    returnCode = this.nativeApi.Close(configHandle);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);
                }
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_unregisterDataSource_helper"</c> via
        /// <see cref="NativeConfig.UnregisterDataSource(IntPtr, string)"/>. 
        /// </summary>
        public void UnregisterDataSource(string dataSourceCode)
        {
            this.env.Execute<object>(() =>
            {
                // load the configuration
                long returnCode = this.nativeApi.Load(this.configDefinition,
                                                      out IntPtr configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                try
                {
                    // format the JSON for the JNI call
                    string inputJson = "{\"DSRC_CODE\":"
                        + Utilities.JsonEscape(dataSourceCode) + "}";

                    // call the underlying C function
                    returnCode = this.nativeApi.UnregisterDataSource(
                        configHandle, inputJson);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // export the new config
                    returnCode = this.nativeApi.Export(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // store the new config definition
                    this.configDefinition = configDef;

                    // return null
                    return null;

                }
                finally
                {
                    // close the config handle
                    returnCode = this.nativeApi.Close(configHandle);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);
                }
            });
        }

    }
}