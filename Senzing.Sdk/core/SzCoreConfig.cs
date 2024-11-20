using System;

namespace Senzing.Sdk.Core
{
    public class SzCoreConfig : SzConfig
    {

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeConfigExtern"/>.
        /// </summary>
        private NativeConfigExtern nativeApi = null;

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
        public SzCoreConfig(SzCoreEnvironment env)
        {
            this.env = env;
            this.env.Execute<object>(() =>
            {
                // construct the native delegate
                this.nativeApi = new NativeConfigExtern();

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
        /// The package-protected function to destroy the Senzing Config SDK.
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
        /// <see cref="SzConfig_create_helper"/>.
        /// </summary>
        public IntPtr CreateConfig()
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.Create(out IntPtr result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_load_helper"/>. 
        /// </summary>
        public IntPtr ImportConfig(string configDefinition)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.Load(configDefinition,
                                                      out IntPtr result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the config handle
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_save_helper"/>. 
        /// </summary>
        public string ExportConfig(IntPtr configHandle)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.Save(configHandle,
                                                      out string result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the contents of the buffer
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_close_helper"/>. 
        /// </summary>
        public void CloseConfig(IntPtr configHandle)
        {
            this.env.Execute<object>(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.Close(configHandle);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return null;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_listDataSources_helper"/>. 
        /// </summary>
        public string GetDataSources(IntPtr configHandle)
        {
            return this.env.Execute(() =>
            {
                // call the underlying C function
                long returnCode = this.nativeApi.ListDataSources(configHandle,
                                                                 out string result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the contents of the buffer
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_addDataSource_helper"/>.
        /// </summary>
        public string AddDataSource(IntPtr configHandle, string dataSourceCode)
        {
            return this.env.Execute(() =>
            {
                // format the JSON for the native call
                string inputJson = "{\"DSRC_CODE\":"
                                 + Utilities.JsonEscape(dataSourceCode) + "}";

                // call the underlying C function
                long returnCode = this.nativeApi.AddDataSource(
                    configHandle, inputJson, out string result);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_deleteDataSource_helper"/>.
        /// </summary>
        public void DeleteDataSource(IntPtr configHandle, string dataSourceCode)
        {
            this.env.Execute<object>(() =>
            {
                // format the JSON for the JNI call
                string inputJson = "{\"DSRC_CODE\":"
                                 + Utilities.JsonEscape(dataSourceCode) + "}";

                // call the underlying C function
                long returnCode = this.nativeApi.DeleteDataSource(
                    configHandle, inputJson);

                // handle any error code if there is one
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return null
                return null;
            });
        }

    }
}