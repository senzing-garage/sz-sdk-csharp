using System;
using System.Collections.Generic;
using System.Text;

using static Senzing.Sdk.SzFlag;
using static Senzing.Sdk.SzFlags;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the internal core implementation of <see cref="SzEngine"/>
    /// that works with the <see cref="SzCoreEnvironment"/> class. 
    /// </summary>
    internal class SzCoreEngine : SzEngine
    {
        /// <summary>
        /// The mask for removing SDK-specific flags that don't go downstream.
        /// </summary>
        internal const long SdkFlagMask = ~((long)SzWithInfo);

        /// <summary>
        /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
        /// </summary>
        private readonly SzCoreEnvironment env;

        /// <summary>
        /// The underlying <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>.
        /// </summary>
        private NativeEngineExtern nativeApi = null;

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
        public SzCoreEngine(SzCoreEnvironment env)
        {
            this.env = env;
            this.env.Execute<object>(() =>
            {
                // construct the native delegate
                this.nativeApi = new NativeEngineExtern();

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
        /// Gets the associated <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>
        /// instance.
        /// </summary>
        ///
        /// <returns>
        /// The associated <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>
        /// instance.
        /// </returns>
        internal NativeEngineExtern GetNativeApi()
        {
            return this.nativeApi;
        }

        /// <summary>
        /// The package-protected function to destroy the Senzing Engine SDK.
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
        /// Implemented to call the <c>Sz_addRecord</c> native function
        /// via <see cref="NativeEngineExtern.AddRecord"/> or
        /// <c>Sz_addRecordWithInfo_helper</c> native function via
        /// <see cref="NativeEngineExtern.AddRecordWithInfo(string, string, string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string AddRecord(string dataSourceCode,
                                string recordID,
                                string recordDefinition,
                                SzFlag? flags = SzAddRecordDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                long returnCode = 0;
                string result = null;
                // check if we have flags to pass downstream or need info
                if (downstreamFlags == 0L
                    && (flags == null || ((flags & SzWithInfo) == SzNoFlags)))
                {
                    // no info needed, no flags to pass, go simple
                    returnCode = this.nativeApi.AddRecord(dataSourceCode,
                                                          recordID,
                                                          recordDefinition);

                }
                else
                {
                    // we either need info or have flags or both
                    returnCode = this.nativeApi.AddRecordWithInfo(
                        dataSourceCode,
                        recordID,
                        recordDefinition,
                        downstreamFlags,
                        out string info);

                    // set the info result if requested
                    if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                    {
                        result = info;
                    }
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getRecordPreview_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetRecordPreview(string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetRecordPreview(
            string recordDefinition,
            SzFlag? flags = SzRecordPreviewDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                long returnCode = this.nativeApi.GetRecordPreview(
                        recordDefinition, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_closeExport_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.CloseExportReport(IntPtr)"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public void CloseExportReport(IntPtr exportHandle)
        {
            this.env.Execute<object>(() =>
            {
                long returnCode = this.nativeApi.CloseExportReport(exportHandle);

                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // no return value, return null
                return null;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_countRedoRecords</c> 
        /// external native helper function via
        /// <see cref="NativeEngineExtern.CountRedoRecords"/>.
        /// </summary>
        ///
        /// <returns>
        /// The number of redo records waiting to be processed, or a negative
        /// number if an error occurred.
        /// </returns>
        public long CountRedoRecords()
        {
            return this.env.Execute(() =>
            {
                long count = this.nativeApi.CountRedoRecords();

                if (count < 0L)
                {
                    this.env.HandleReturnCode(count, this.nativeApi);
                }

                // return the count
                return count;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_deleteRecord</c> 
        /// external native helper function via
        /// <see cref="NativeEngineExtern.DeleteRecord(string, string)"/>
        /// or <c>Sz_deleteRecordWithInfo_helper</c> function via
        /// <see cref="NativeEngineExtern.DeleteRecordWithInfo(string, string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string DeleteRecord(string dataSourceCode,
                                   string recordID,
                                   SzFlag? flags = SzDeleteRecordDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                long returnCode = 0;
                string result = null;
                // check if we have flags to pass downstream or need info
                if (downstreamFlags == 0L
                    && (flags == null || ((flags & SzWithInfo) == SzNoFlags)))
                {
                    // no info needed, no flags to pass, go simple
                    returnCode = this.nativeApi.DeleteRecord(dataSourceCode,
                                                             recordID);

                }
                else
                {
                    // we either need info or have flags or both
                    returnCode = this.nativeApi.DeleteRecordWithInfo(
                        dataSourceCode,
                        recordID,
                        downstreamFlags,
                        out string info);

                    // set the info result if requested
                    if (flags != null && ((flags & SzWithInfo) != SzNoFlags))
                    {
                        result = info;
                    }
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_exportCSVEntityReport_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.ExportCSVEntityReport(string, long, out IntPtr)"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public IntPtr ExportCsvEntityReport(
            string csvColumnList,
            SzFlag? flags = SzExportDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

            return this.env.Execute(() =>
            {
                long returnCode = this.nativeApi.ExportCSVEntityReport(
                    csvColumnList, downstreamFlags, out IntPtr result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the export handle
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_exportJSONEntityReport_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.ExportJSONEntityReport(long, out IntPtr)"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public IntPtr ExportJsonEntityReport(
            SzFlag? flags = SzExportDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

            return this.env.Execute(() =>
            {
                long returnCode = this.nativeApi.ExportJSONEntityReport(
                    downstreamFlags, out IntPtr result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the export handle
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_fetchNext_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.FetchNext(IntPtr, out string)"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public string FetchNext(IntPtr exportHandle)
        {
            return this.env.Execute(() =>
            {
                long returnCode = this.nativeApi.FetchNext(
                    exportHandle, out string result);

                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the next export chunk
                return result;
            });
        }

        /// <summary>
        /// Encodes the <see cref="ISet{T}"/> of <c>long</c>
        /// entity ID's as JSON.
        /// </summary>
        ///
        /// <remarks>
        /// The JSON is formatted as:
        /// <code>
        /// {
        ///    "ENTITIES": [
        ///       { "ENTITY_ID": &lt;entity_id1&gt; },
        ///       { "ENTITY_ID": &lt;entity_id2&gt; },
        ///       . . .
        ///       { "ENTITY_ID": &lt;entity_idN&gt; }
        ///    ]
        /// }
        /// </code>
        /// </remarks>
        ///
        /// <param name="entityIDs">
        /// The non-null <see cref="ISet{T}"/> of non-null
        /// <c>long</c> entity ID's.
        /// </param>
        ///
        /// <returns>
        /// The encoded JSON string of entity ID's.
        /// </returns>
        internal static string EncodeEntityIDs(ISet<long> entityIDs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"ENTITIES\":[");
            if (entityIDs != null)
            {
                string prefix = "";
                foreach (long entityID in entityIDs)
                {
                    sb.Append(prefix);
                    sb.Append("{\"ENTITY_ID\":").Append(entityID).Append("}");
                    prefix = ",";
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// Encodes the <see cref="ISet{T}"/> of tuples of
        /// data source code and record ID pairs as JSON.
        /// </summary>
        ///
        /// <remarks>
        /// The JSON is formatted as:
        /// <code>
        ///   {
        ///     "RECORDS": [
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source1&gt;",
        ///          "RECORD_ID":  "&lt;record_id1&gt;"
        ///        },
        ///        {
        ///          "DATA_SOURCE": "&lt;data_source2&gt;",
        ///          "RECORD_ID":  "&lt;record_id2&gt;"
        ///        },
        ///        . . .
        ///        {
        ///          "DATA_SOURCE": "&lt;data_sourceN&gt;",
        ///          "RECORD_ID":  "&lt;record_idN&gt;"
        ///        }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        ///
        /// <param name="recordKeys">
        /// The non-null <see cref="ISet{T}"/> of non-null
        /// tuples of data source code and record ID pairs.
        /// </param>
        /// 
        /// <returns>The encoded JSON string of record keys.</returns>
        internal static string EncodeRecordKeys(
            ISet<(string dataSourceCode, string recordID)> recordKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"RECORDS\":[");
            if (recordKeys != null)
            {
                string prefix = "";
                foreach ((string dataSourceCode, string recordID) recordKey in recordKeys)
                {
                    string dataSourceCode = recordKey.dataSourceCode;
                    string recordId = recordKey.recordID;
                    sb.Append(prefix);
                    sb.Append("{\"DATA_SOURCE\":");
                    sb.Append(Utilities.JsonEscape(dataSourceCode));
                    sb.Append(",\"RECORD_ID\":");
                    sb.Append(Utilities.JsonEscape(recordId));
                    sb.Append("}");
                    prefix = ",";
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// Encodes the <see cref="ISet{T}"/> of
        /// <c>string</c> data source codes as JSON.
        /// </summary>
        /// <remarks>
        /// The JSON is formatted as:
        /// <code>
        ///    { "DATA_SOURCES": [
        ///        "&lt;data_source_code1&gt;",
        ///        "&lt;data_source_code2&gt;",
        ///        . . .
        ///        "&lt;data_source_codeN&gt;"
        ///      ]
        ///    }
        /// </code>
        /// </remarks>
        ///
        /// <param name="dataSources">
        /// The <see cref="ISet{T}"/> of <c>string</c>
        /// data source codes.
        /// </param>
        ///
        /// <returns>The encoded JSON string of record keys.</returns>
        internal static string EncodeDataSources(ISet<string> dataSources)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"DATA_SOURCES\":[");
            if (dataSources != null)
            {
                string prefix = "";
                foreach (string dataSourceCode in dataSources)
                {
                    sb.Append(prefix);
                    sb.Append(Utilities.JsonEscape(dataSourceCode));
                    prefix = ",";
                }
            }
            sb.Append("]}");
            return sb.ToString();
        }

        /// <summary>
        /// Implemented to call the <c>Sz_findNetworkByEntityID_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.FindNetworkByEntityID(string, int, int, int, long, out string)"/> 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindNetwork(
            ISet<long> entityIDs,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

            return this.env.Execute(() =>
            {
                string jsonEntityIDs = EncodeEntityIDs(entityIDs);

                long returnCode = this.nativeApi.FindNetworkByEntityID(
                    jsonEntityIDs,
                    maxDegrees,
                    buildOutDegrees,
                    buildOutMaxEntities,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_findNetworkByRecordID_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.FindNetworkByRecordID(string, int, int, int, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindNetwork(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            int maxDegrees,
            int buildOutDegrees,
            int buildOutMaxEntities,
            SzFlag? flags = SzFindNetworkDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

            return this.env.Execute(() =>
            {
                string jsonRecordKeys = EncodeRecordKeys(recordKeys);

                long returnCode = this.nativeApi.FindNetworkByRecordID(
                    jsonRecordKeys,
                    maxDegrees,
                    buildOutDegrees,
                    buildOutMaxEntities,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external
        /// <c>Sz_findPathByEntityID_V2_helper</c>,
        /// <c>Sz_findPathByEntityIDWithAvoids_V2_helper</c> or
        /// <c>Sz_findPathByEntityIDIncludingSource_V2_helper</c>
        /// native helper functions via
        /// <see cref="NativeEngineExtern.FindPathByEntityID(long, long, int, long, out string)"/>,
        /// <see cref="NativeEngineExtern.FindPathByEntityIDWithAvoids(long, long, int, string, long, out string)"/> or
        /// <see cref="NativeEngineExtern.FindPathByEntityIDIncludingSource(long, long, int, string, string, long, out string)"/>,
        /// respectively.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindPath(
            long startEntityId,
            long endEntityId,
            int maxDegrees,
            ISet<long> avoidEntityIDs = null,
            ISet<string> requiredDataSources = null,
            SzFlag? flags = SzFindPathDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

            return this.env.Execute(() =>
            {
                long returnCode = 0;
                string result = "";

                if ((avoidEntityIDs == null || avoidEntityIDs.Count == 0)
                    && (requiredDataSources == null || requiredDataSources.Count == 0))
                {
                    // call the base function
                    returnCode = this.nativeApi.FindPathByEntityID(
                        startEntityId, endEntityId, maxDegrees,
                        downstreamFlags, out string path);

                    // set the result
                    result = path;

                }
                else if (requiredDataSources == null || requiredDataSources.Count == 0)
                {
                    // encode the entity ID's
                    string avoidanceJson = EncodeEntityIDs(avoidEntityIDs);

                    // call the variant with avoidances, but without required data sources
                    returnCode = this.nativeApi.FindPathByEntityIDWithAvoids(
                        startEntityId, endEntityId, maxDegrees, avoidanceJson,
                        downstreamFlags, out string path);

                    // set the result
                    result = path;

                }
                else
                {
                    // encode the entity ID's
                    string avoidanceJson = EncodeEntityIDs(avoidEntityIDs);

                    // encode the data sources
                    string dataSourceJson = EncodeDataSources(requiredDataSources);

                    // we have to call the full-blown variant of the function
                    returnCode = this.nativeApi.FindPathByEntityIDIncludingSource(
                        startEntityId, endEntityId, maxDegrees, avoidanceJson,
                        dataSourceJson, downstreamFlags, out string path);

                    // set the result
                    result = path;
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the external
        /// <c>Sz_findPathByRecordID_V2_helper</c>,
        /// <c>Sz_findPathByRecordIDWithAvoids_V2_helper</c> or
        /// <c>Sz_findPathByRecordIDIncludingSource_V2_helper</c>
        /// native helper function via
        /// <see cref="NativeEngineExtern.FindPathByRecordID(string, string, string, string, int, long, out string)"/>,
        /// <see cref="NativeEngineExtern.FindPathByRecordIDWithAvoids(string, string, string, string, int, string, long, out string)"/> or
        /// <see cref="NativeEngineExtern.FindPathByRecordIDIncludingSource(string, string, string, string, int, string, string, long, out string)"/>,
        /// respectively.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindPath(
            string startDataSourceCode,
            string startRecordID,
            string endDataSourceCode,
            string endRecordID,
            int maxDegrees,
            ISet<(string dataSourceCode, string recordID)> avoidRecordKeys = null,
            ISet<string> requiredDataSources = null,
            SzFlag? flags = SzFindPathDefaultFlags)
        {
            // clear out the SDK-specific flags
            long downstreamFlags = (FlagsToLong(flags) & SdkFlagMask);

            return this.env.Execute(() =>
            {
                string result = "";
                long returnCode = 0;

                if ((avoidRecordKeys == null || avoidRecordKeys.Count == 0)
                    && (requiredDataSources == null || requiredDataSources.Count == 0))
                {
                    // call the base function
                    returnCode = this.nativeApi.FindPathByRecordID(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        downstreamFlags,
                        out string path);

                    // set the result
                    result = path;

                }
                else if (requiredDataSources == null || requiredDataSources.Count == 0)
                {
                    // encode the entity ID's
                    string avoidanceJson = EncodeRecordKeys(avoidRecordKeys);

                    // call the variant with avoidances, but without required data sources
                    returnCode = this.nativeApi.FindPathByRecordIDWithAvoids(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        avoidanceJson,
                        downstreamFlags,
                        out string path);

                    // set the result
                    result = path;

                }
                else
                {
                    // encode the entity ID's
                    string avoidanceJson = EncodeRecordKeys(avoidRecordKeys);

                    // encode the data sources
                    string dataSourceJson = EncodeDataSources(requiredDataSources);

                    // we have to call the full-blown variant of the function
                    returnCode = this.nativeApi.FindPathByRecordIDIncludingSource(
                        startDataSourceCode,
                        startRecordID,
                        endDataSourceCode,
                        endRecordID,
                        maxDegrees,
                        avoidanceJson,
                        dataSourceJson,
                        downstreamFlags,
                        out string path);

                    // set the result
                    result = path;
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getEntityByEntityID_V2_helper"</c> 
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetEntityByEntityID(long, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetEntity(long entityId,
                                SzFlag? flags = SzEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.GetEntityByEntityID(
                    entityId, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getEntityByEntityID_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetEntityByRecordID(string, string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetEntity(string dataSourceCode,
                                string recordID,
                                SzFlag? flags = SzEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.GetEntityByRecordID(
                    dataSourceCode,
                    recordID,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_findInterestingEntitiesByEntityID_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.FindInterestingEntitiesByEntityID(long, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindInterestingEntities(
            long entityId,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.FindInterestingEntitiesByEntityID(
                    entityId, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_findInterestingEntitiesByRecordID_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.FindInterestingEntitiesByRecordID(string, string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string FindInterestingEntities(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzFindInterestingEntitiesDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.FindInterestingEntitiesByRecordID(
                    dataSourceCode,
                    recordID,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getRecord_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetRecord(string, string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetRecord(string dataSourceCode,
                                string recordID,
                                SzFlag? flags = SzRecordDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.GetRecord(
                    dataSourceCode,
                    recordID,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getRedoRecord_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetRedoRecord(out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetRedoRecord()
        {
            return this.env.Execute(() =>
            {
                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.GetRedoRecord(out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return (result.Length == 0) ? null : result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_stats_helper</c> 
        /// external native helper function via
        /// <see cref="NativeEngineExtern.Stats"/>.
        /// </summary>
        ///
        ///
        /// <returns>JSON document of workload statistics</returns>
        public string GetStats()
        {
            return this.env.Execute(() =>
            {
                return this.nativeApi.Stats();
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_getVirtualEntityByRecordID_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.GetVirtualEntityByRecordID(string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string GetVirtualEntity(
            ISet<(string dataSourceCode, string recordID)> recordKeys,
            SzFlag? flags = SzVirtualEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // get the record ID JSON
                string jsonRecordstring = EncodeRecordKeys(recordKeys);

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.GetVirtualEntityByRecordID(
                    jsonRecordstring, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }


        /// <summary>
        /// Implemented to call the <c>Sz_howEntityByEntityID_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.HowEntityByEntityID(long, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string HowEntity(long entityId,
                                SzFlag? flags = SzHowEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.HowEntityByEntityID(
                    entityId, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_primeEngine</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.PrimeEngine"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public void PrimeEngine()
        {
            this.env.Execute<object>(() =>
            {
                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.PrimeEngine();

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return null;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_processRedoRecord</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.ProcessRedoRecord(string)"/>
        /// or <c>Sz_processRedoRecordWithInfo_helper</c> function via
        /// <see cref="NativeEngineExtern.ProcessRedoRecordWithInfo(string, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string ProcessRedoRecord(
            string redoRecord,
            SzFlag? flags = SzRedoDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                long returnCode = 0;
                string result = null;
                // check if we have flags to pass downstream
                if (flags == null || ((flags & SzWithInfo) == SzNoFlags))
                {
                    returnCode = this.nativeApi.ProcessRedoRecord(redoRecord);

                }
                else
                {
                    returnCode = this.nativeApi.ProcessRedoRecordWithInfo(
                        redoRecord, out string info);
                    result = info;
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });

        }

        /// <summary>
        /// Implemented to call the <c>Sz_reevaluateEntity</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.ReevaluateEntity(long, long)"/>
        /// or <c>Sz_reevaluateEntityWithInfo_helper</c> function via
        /// <see cref="NativeEngineExtern.ReevaluateEntityWithInfo(long, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string ReevaluateEntity(
            long entityId,
            SzFlag? flags = SzReevaluateEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                long returnCode = 0;
                string result = null;

                // check if we have flags to pass downstream or need info
                if (flags == null || ((flags & SzWithInfo) == SzNoFlags))
                {
                    // no info needed, no flags to pass, go simple
                    returnCode = this.nativeApi.ReevaluateEntity(
                        entityId, downstreamFlags);

                }
                else
                {
                    // we either need info or have flags or both
                    returnCode = this.nativeApi.ReevaluateEntityWithInfo(
                        entityId, downstreamFlags, out string info);

                    // set the info result if requested
                    result = info;
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_reevaluateRecord</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.ReevaluateRecord(string, string, long)"/>
        /// or the <c>Sz_reevaluateRecordWithInfo_helper</c> function via
        /// <see cref="NativeEngineExtern.ReevaluateRecordWithInfo(string, string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string ReevaluateRecord(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzReevaluateRecordDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                long returnCode = 0;
                string result = null;

                // check if we have flags to pass downstream or need info
                if (flags == null || ((flags & SzWithInfo) == SzNoFlags))
                {
                    // no info needed, no flags to pass, go simple
                    returnCode = this.nativeApi.ReevaluateRecord(dataSourceCode,
                                                                 recordID,
                                                                 downstreamFlags);

                }
                else
                {
                    // we either need info or have flags or both
                    returnCode = this.nativeApi.ReevaluateRecordWithInfo(
                        dataSourceCode,
                        recordID,
                        downstreamFlags,
                        out string info);

                    // set the info result
                    result = info;

                    // TODO(bcaceres): remove this if not-found records produce an error
                    // check if record not found yields empty INFO
                    if (result != null && result.Length == 0)
                    {
                        result = null;
                    }
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_searchByAttributes_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.SearchByAttributes(string, long, out string)"/> 
        /// or <c>Sz_searchByAttributes_V3_helper</c> external native function via
        /// <see cref="NativeEngineExtern.SearchByAttributes(string, string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string SearchByAttributes(
            string attributes,
            string searchProfile,
            SzFlag? flags = SzSearchByAttributesDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // declare the result variables
                long returnCode = 0L;
                string result = "";

                // check if have a search profile
                if (searchProfile == null)
                {
                    // search with the default search profile
                    returnCode = this.nativeApi.SearchByAttributes(
                        attributes, downstreamFlags, out string entities);

                    // set the result
                    result = entities;

                }
                else
                {
                    returnCode = this.nativeApi.SearchByAttributes(
                        attributes, searchProfile, downstreamFlags, out string entities);

                    // see the result
                    result = entities;
                }

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the
        /// <see cref="SearchByAttributes(string,string,SzFlag?)"/>
        /// with a <c>null</c> search profile parameter.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string SearchByAttributes(
            string attributes,
            SzFlag? flags = SzSearchByAttributesDefaultFlags)
        {
            return this.SearchByAttributes(attributes, null, flags);
        }

        /// <summary>
        /// Implemented to call the <c>Sz_whySearch_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.WhySearch(string, long, string, out string)"/> 
        /// or <c>Sz_whySearch_V2_helper</c> external native function via
        /// <see cref="NativeEngineExtern.WhySearch(string, long, string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string WhySearch(
            string attributes,
            long entityID,
            string searchProfile = null,
            SzFlag? flags = SzWhySearchDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // call the underlying native function
                long returnCode = this.nativeApi.WhySearch(
                    attributes, entityID, searchProfile, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_whyEntities_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.WhyEntities(long, long, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string WhyEntities(
            long entityId1,
            long entityId2,
            SzFlag? flags = SzWhyEntitiesDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.WhyEntities(
                    entityId1, entityId2, downstreamFlags, out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_whyRecordInEntity_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.WhyRecordInEntity(string, string, long, out string)"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string WhyRecordInEntity(
            string dataSourceCode,
            string recordID,
            SzFlag? flags = SzWhyRecordInEntityDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.WhyRecordInEntity(
                    dataSourceCode,
                    recordID,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }

        /// <summary>
        /// Implemented to call the <c>Sz_whyRecords_V2_helper</c>
        /// external native helper function via
        /// <see cref="NativeEngineExtern.WhyRecords(string, string, string, string, long, out string)"/>.
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public string WhyRecords(
            string dataSourceCode1,
            string recordID1,
            string dataSourceCode2,
            string recordID2,
            SzFlag? flags = SzWhyRecordsDefaultFlags)
        {
            return this.env.Execute(() =>
            {
                // clear out the SDK-specific flags
                long downstreamFlags = FlagsToLong(flags) & SdkFlagMask;

                // check if we have flags to pass downstream
                long returnCode = this.nativeApi.WhyRecords(
                    dataSourceCode1,
                    recordID1,
                    dataSourceCode2,
                    recordID2,
                    downstreamFlags,
                    out string result);

                // check the return code
                this.env.HandleReturnCode(returnCode, this.nativeApi);

                // return the result
                return result;
            });
        }
    }
}