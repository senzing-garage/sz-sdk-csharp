using System;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzConfig"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreConfig : SzConfig
    {

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfigExtern"/>.
        /// </summary>
        private readonly NativeConfigExtern nativeApi = null;

        /// <summary>
        /// The backing config definition.
        /// </summary>
        private string configDefinition;

        /// <summary>
        /// Constructs with the specified <see cref="SzCoreEnvironment"/>.
        /// </summary>
        /// 
        /// <param name="env">
        /// The <see cref="SzCoreEnvironment"/> that is constructing this instance
        /// and will be used by this instance.
        /// </param>
        /// 
        /// <param name="configDefinition">
        /// The <c>string</c> config definition describing the configuration
        /// represented by this instance.
        /// </param>
        public SzCoreConfig(SzCoreEnvironment env, string configDefinition)
        {
            this.env = env;
            if (configDefinition == null)
            {
                throw new ArgumentNullException(
                    "The specified config definition cannot be null");
            }

            // set the environment and config definition
            this.configDefinition = configDefinition;

            // construct the native delegate
            SzCoreConfigManager configMgr = (SzCoreConfigManager)this.env.GetConfigManager();

            this.nativeApi = configMgr.GetConfigApi();
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
        internal NativeConfigExtern GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// Implemented to return the config definition for this instance.
        /// </summary>
        public string Export()
        {
            return this.configDefinition;
        }

        /// <summary>
        /// Implemented to call the <see cref="Export"/> function and
        /// return the result.
        /// </summary>
        public override string ToString()
        {
            return this.Export();
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfig_listDataSources_helper"</c> via 
        /// <see cref="NativeConfigExtern.ListDataSources(IntPtr, out string)"/>.
        /// </summary>
        public string GetDataSources()
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
                    returnCode = this.nativeApi.ListDataSources(configHandle,
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
        /// <c>SzConfig_addDataSource_helper"</c> via
        /// <see cref="NativeConfigExtern.AddDataSource(IntPtr, string, out string)"/>. 
        /// </summary>
        public string AddDataSource(string dataSourceCode)
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
                    returnCode = this.nativeApi.AddDataSource(
                        configHandle, inputJson, out string result);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // export the new config
                    returnCode = this.nativeApi.Save(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // store the new confg definition
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
        /// <c>SzConfig_deleteDataSource_helper"</c> via
        /// <see cref="NativeConfigExtern.DeleteDataSource(IntPtr, string)"/>. 
        /// </summary>
        public void DeleteDataSource(string dataSourceCode)
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
                    returnCode = this.nativeApi.DeleteDataSource(
                        configHandle, inputJson);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // export the new config
                    returnCode = this.nativeApi.Save(configHandle, out string configDef);

                    // handle any error code if there is one
                    this.env.HandleReturnCode(returnCode, this.nativeApi);

                    // store the new confg definition
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