using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzConfigManager"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreConfigManager : SzConfigManager
    {
        /// <summary>
        /// The <b>unmodifiable</b> collection of default data sources.
        /// </summary>
        private static readonly ReadOnlyCollection<string> DefaultSources
            = new ReadOnlyCollection<string>(
                new List<string>(new[] { "TEST", "SEARCH" }));

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
                    returnCode = this.configApi.Save(configHandle, out string configDef);

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
                    returnCode = this.configApi.Save(configHandle, out string configDef);

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
        /// <c>SzConfigMgr_addConfig_helper"</c> via
        /// <see cref="NativeConfigManagerExtern.AddConfig(string, string, out long)"/>. 
        /// </summary>
        public long RegisterConfig(string configDefinition, string configComment)
        {
            return this.env.Execute(() =>
            {
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.AddConfig(
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
            string configComment = this.CreateConfigComment(configDefinition);

            // return the result from the base method
            return this.RegisterConfig(configDefinition, configComment);
        }

        /// <summary>
        /// Finds the index of the first non-whitespace character after the
        /// specified index from the specified character array.
        /// </summary>
        ///
        /// <param name="charArray">The character array.</param>
        /// <param name="fromIndex">The starting index.</param>
        ///
        /// <returns>
        /// The index of the first non-whitespace character or the length of
        /// the character array if no non-whitespace character is found.
        /// </returns>
        internal static int EatWhiteSpace(char[] charArray, int fromIndex)
        {
            int index = fromIndex;

            // advance past any whitespace
            while (index < charArray.Length && Char.IsWhiteSpace(charArray[index]))
            {
                index++;
            }

            // return the index
            return index;
        }

        /// <summary>
        /// Produce an auto-generated configuration comment for the 
        /// configuration manager registry.  This does a pseudo-JSON 
        /// parse to avoid a third-party JSON parser dependency.
        /// </summary>
        /// 
        /// <param name="configDefinition">
        /// The <c>string</c> configuration definition.
        /// </param>
        ///
        /// <returns>
        /// The auto-generated comment, which may be empty-string
        /// if an auto-generated comment could not otherwise be produced.
        /// </returns>
        internal string CreateConfigComment(string configDefinition)
        {
            int index = configDefinition.IndexOf("\"CFG_DSRC\"");
            if (index < 0)
            {
                return "";
            }
            char[] charArray = configDefinition.ToCharArray();

            // advance past any whitespace
            index = EatWhiteSpace(charArray, index + "\"CFG_DSRC\"".Length);

            // check for the colon
            if (index >= charArray.Length || charArray[index++] != ':')
            {
                return "";
            }

            // advance past any whitespace
            index = EatWhiteSpace(charArray, index);

            // check for the open bracket
            if (index >= charArray.Length || charArray[index++] != '[')
            {
                return "";
            }

            // find the end index
            int endIndex = configDefinition.IndexOf("]", index);
            if (endIndex < 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Data Sources: ");
            string prefix = "";
            int dataSourceCount = 0;
            ISet<string> defaultSources = new SortedSet<string>();
            while (index > 0 && index < endIndex)
            {
                index = configDefinition.IndexOf("\"DSRC_CODE\"", index);
                if (index < 0 || index >= endIndex)
                {
                    continue;
                }
                index = EatWhiteSpace(charArray, index + "\"DSRC_CODE\"".Length);

                // check for the colon
                if (index >= endIndex || charArray[index++] != ':')
                {
                    return "";
                }

                index = EatWhiteSpace(charArray, index);

                // check for the quote
                if (index >= endIndex || charArray[index++] != '"')
                {
                    return "";
                }
                int start = index;

                // find the ending quote
                while (index < endIndex && charArray[index] != '"')
                {
                    index++;
                }
                if (index >= endIndex)
                {
                    return "";
                }

                // get the data source code
                string dataSource = configDefinition.Substring(start, (index - start));
                if (DefaultSources.Contains(dataSource))
                {
                    defaultSources.Add(dataSource);
                    continue;
                }
                sb.Append(prefix);
                sb.Append(dataSource);
                dataSourceCount++;
                prefix = ", ";
            }

            // check if only the default data sources
            if (dataSourceCount == 0 && defaultSources.Count == 0)
            {
                sb.Append("[ NONE ]");
            }
            else if (dataSourceCount == 0
                    && defaultSources.Count == DefaultSources.Count)
            {
                sb.Append("[ ONLY DEFAULT ]");

            }
            else if (dataSourceCount == 0)
            {

                sb.Append("[ SOME DEFAULT (");
                prefix = "";
                foreach (String source in defaultSources)
                {
                    sb.Append(prefix);
                    sb.Append(source);
                    prefix = ", ";
                }
                sb.Append(") ]");
            }

            // return the constructed string
            return sb.ToString();
        }

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <c>SzConfigMgr_getConfigList_helper"</c> via
        /// <see cref="NativeConfigManager.GetConfigList(out string)"/>. 
        /// </summary>
        public string GetConfigRegistry()
        {
            return this.env.Execute(() =>
            {
                // get the config manager API
                NativeConfigManager nativeApi = this.GetConfigManagerApi();

                // call the underlying C function
                long returnCode = nativeApi.GetConfigList(out string configList);

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