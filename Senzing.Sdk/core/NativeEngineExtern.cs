using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides an implementation of <see cref="NativeEngine"/>
    /// that will call the native equivalent <c>extern</c> functions.
    /// </summary>
    internal class NativeEngineExtern : NativeEngine
    {
        [DllImport("Sz")]
        private static extern int Sz_init(
            byte[] moduleName, byte[] iniParams, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function
        /// <see cref="SzConfig_init"/>
        /// </summary>
        public long Init(string moduleName, string iniParams, bool verboseLogging)
        {
            return Sz_init(Utilities.StringToUTF8Bytes(moduleName),
                           Utilities.StringToUTF8Bytes(iniParams),
                           (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern int Sz_initWithConfigID(
            byte[] moduleName, byte[] iniParams, long initConfigID, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function
        /// <see cref="SzConfig_initWithConfigID"/>.
        /// </summary>
        public long InitWithConfigID(string moduleName,
                                     string iniParams,
                                     long initConfigID,
                                     bool verboseLogging)
        {
            return Sz_initWithConfigID(
                Utilities.StringToUTF8Bytes(moduleName),
                Utilities.StringToUTF8Bytes(iniParams),
                initConfigID,
                (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern int Sz_reinit(long initConfigID);

        /// <summary>
        /// Implemented to call the external native function
        /// <see cref="SzConfig_reinit"/>.
        /// </summary>
        public long Reinit(long initConfigID)
        {
            return Sz_reinit(initConfigID);
        }

        [DllImport("Sz")]
        private static extern long Sz_destroy();

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_destroy()</c>.
        /// </summary>
        public long Destroy()
        {
            return Sz_destroy();
        }

        [DllImport("Sz")]
        private static extern long Sz_getLastException(
            [MarshalAs(UnmanagedType.LPArray)] byte[] buf, long length);

        /// <summary>
        /// Implemented to return the last exception message associated
        /// with the Senzing config funtions.
        /// </summary>
        ///
        /// <remarks>
        /// This is most commonly called after a Senzing config function
        /// returnbs a failure code (non-zero or NULL).
        /// </remarks>
        ///
        /// <returns>An error message</returns>
        public string GetLastException()
        {
            byte[] buf = new byte[4096];
            long length = Sz_getLastException(buf, buf.Length);
            if (length == 0L)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(buf, 0, (int)(length - 1));
        }

        [DllImport("Sz")]
        private static extern long Sz_getLastExceptionCode();

        /// <summary>
        /// Implemented to return the last error code associated with the 
        /// Senzing config funtions.
        /// </summary>
        ///
        /// <remarks>
        /// This is most commonly called after a Senzing config function
        /// returns a failure code (non-zero or NULL).
        /// </remarks>
        ///
        /// <returns>An error code</returns>
        ///
        public long GetLastExceptionCode()
        {
            return Sz_getLastExceptionCode();
        }

        [DllImport("Sz")]
        private static extern void Sz_clearLastException();

        /// <summary>
        /// Implemented to clear the information pertaining to the last
        /// error encountered with the Senzing config funtions.
        /// </summary>
        public void ClearLastException()
        {
            Sz_clearLastException();
        }

        /// <summary>
        /// Combines an <see cref="System.IntPtr""/> response with a
        /// <c>long</c> return code to handle the result from the 
        /// engine helper functions.
        /// </summary>
        private struct SzPointerResult
        {
            public IntPtr response;
            public long returnCode;
        }

        /// <summary>
        /// Combines a <c>long</c> response with a <c>long</c> return code
        /// to handle the result from the engine helper functions.
        /// </summary>
        private struct SzLongResult
        {
            public long response;
            public long returnCode;
        }

        [DllImport("Sz")]
        private static extern long Sz_primeEngine();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_primeEngine"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long PrimeEngine()
        {
            return Sz_primeEngine();
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_stats_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_stats_helper"/>. 
        /// </summary>
        ///
        ///
        /// <returns>JSON document of workload statistics</returns>
        public string Stats()
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_stats_helper();
                if (result.returnCode == 0L)
                {
                    return Utilities.UTF8BytesToString(result.response);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzLongResult Sz_getActiveConfigID_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getActiveConfigID_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetActiveConfigID(out long configID)
        {
            SzLongResult result;
            result.response = 0L;
            result.returnCode = 0L;
            result = Sz_getActiveConfigID_helper();
            configID = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern long Sz_addRecord(
            byte[] dataSourceCode, byte[] recordID, byte[] jsonData);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_addRecord"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long AddRecord(string dataSourceCode,
                              string recordID,
                              string jsonData)
        {
            byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
            byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);
            byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);

            return Sz_addRecord(codeBytes, idBytes, jsonBytes);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_addRecordWithInfo_helper(
            byte[] dataSourceCode, byte[] recordID, byte[] jsonData, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_addRecordWithInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long AddRecordWithInfo(string dataSourceCode,
                                     string recordID,
                                     string jsonData,
                                     long flags,
                                     out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);
                byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);

                result = Sz_addRecordWithInfo_helper(codeBytes,
                                                     idBytes,
                                                     jsonBytes,
                                                     flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_preprocessRecord_helper(
            byte[] jsonData, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_preprocessRecord_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long PreprocessRecord(string jsonData,
                                     long flags,
                                     out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);

                result = Sz_preprocessRecord_helper(jsonBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long Sz_deleteRecord(
            byte[] dataSourceCode, byte[] recordID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_deleteRecord"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long DeleteRecord(string dataSourceCode, string recordID)
        {
            byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
            byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

            return Sz_deleteRecord(codeBytes, idBytes);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_deleteRecordWithInfo_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_deleteRecordWithInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long DeleteRecordWithInfo(string dataSourceCode,
                                         string recordID,
                                         long flags,
                                         out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_deleteRecordWithInfo_helper(codeBytes,
                                                        idBytes,
                                                        flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long Sz_reevaluateRecord(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_reevaluateRecord"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ReevaluateRecord(string dataSourceCode,
                                     string recordID,
                                     long flags)
        {
            byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
            byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

            return Sz_reevaluateRecord(codeBytes, idBytes, flags);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_reevaluateRecordWithInfo_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_reevaluateRecordWithInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ReevaluateRecordWithInfo(string dataSourceCode,
                                             string recordID,
                                             long flags,
                                             out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_reevaluateRecordWithInfo_helper(codeBytes,
                                                            idBytes,
                                                            flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long Sz_reevaluateEntity(long entityID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_reevaluateEntity"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ReevaluateEntity(long entityID, long flags)
        {
            return Sz_reevaluateEntity(entityID, flags);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_reevaluateEntityWithInfo_helper(
            long entityID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_reevaluateEntityWithInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ReevaluateEntityWithInfo(long entityID,
                                             long flags,
                                             out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_reevaluateEntityWithInfo_helper(entityID, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_searchByAttributes_helper(byte[] jsonData);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_searchByAttributes_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long SearchByAttributes(string jsonData, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);

                result = Sz_searchByAttributes_helper(jsonBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_searchByAttributes_V2_helper(
            byte[] jsonData, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_searchByAttributes_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long SearchByAttributes(string jsonData, long flags, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);

                result = Sz_searchByAttributes_V2_helper(jsonBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_searchByAttributes_V3_helper(
            byte[] jsonData, byte[] searchProfile, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_searchByAttributes_V3_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long SearchByAttributes(string jsonData,
                                       string searchProfile,
                                       long flags,
                                       out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] jsonBytes = Utilities.StringToUTF8Bytes(jsonData);
                byte[] profBytes = Utilities.StringToUTF8Bytes(searchProfile);

                result = Sz_searchByAttributes_V3_helper(jsonBytes, profBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getEntityByEntityID_helper(long entityID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getEntityByEntityID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetEntityByEntityID(long entityID, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_getEntityByEntityID_helper(entityID);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getEntityByEntityID_V2_helper(
            long entityID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getEntityByEntityID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetEntityByEntityID(long entityID,
                                        long flags,
                                        out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_getEntityByEntityID_V2_helper(entityID, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getEntityByRecordID_helper(
            byte[] dataSourceCode, byte[] recordID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getEntityByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetEntityByRecordID(string dataSourceCode,
                                        string recordID,
                                        out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_getEntityByRecordID_helper(codeBytes, idBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getEntityByRecordID_V2_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getEntityByRecordID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetEntityByRecordID(string dataSourceCode,
                                       string recordID,
                                       long flags,
                                       out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_getEntityByRecordID_V2_helper(codeBytes,
                                                          idBytes,
                                                          flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findInterestingEntitiesByEntityID_helper(
            long entityID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findInterestingEntitiesByEntityID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindInterestingEntitiesByEntityID(long entityID,
                                                      long flags,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_findInterestingEntitiesByEntityID_helper(entityID,
                                                                     flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findInterestingEntitiesByRecordID_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findInterestingEntitiesByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindInterestingEntitiesByRecordID(string dataSourceCode,
                                                      string recordID,
                                                      long flags,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_findInterestingEntitiesByRecordID_helper(codeBytes,
                                                                     idBytes,
                                                                     flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityID_helper(
            long entityID1, long entityID2, long maxDegrees);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityID(long entityID1,
                                       long entityID2,
                                       int maxDegrees,
                                       out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_findPathByEntityID_helper(entityID1,
                                                      entityID2,
                                                      maxDegrees);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityID_V2_helper(
            long entityID1, long entityID2, long maxDegrees, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityID(long entityID1,
                                       long entityID2,
                                       int maxDegrees,
                                       long flags,
                                       out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_findPathByEntityID_V2_helper(entityID1,
                                                         entityID2,
                                                         maxDegrees,
                                                         flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordID_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordID(string dataSourceCode1,
                                       string recordID1,
                                       string dataSourceCode2,
                                       string recordID2,
                                       int maxDegrees,
                                       out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);

                result = Sz_findPathByRecordID_helper(codeBytes1,
                                                      idBytes1,
                                                      codeBytes2,
                                                      idBytes2,
                                                      maxDegrees);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordID_V2_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordID(string dataSourceCode1,
                                      string recordID1,
                                      string dataSourceCode2,
                                      string recordID2,
                                      int maxDegrees,
                                      long flags,
                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);

                result = Sz_findPathByRecordID_V2_helper(codeBytes1,
                                                         idBytes1,
                                                         codeBytes2,
                                                         idBytes2,
                                                         maxDegrees,
                                                         flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityIDWithAvoids_helper(
            long entityID1, long entityID2, long maxDegrees, byte[] avoidedEntities);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityIDWithAvoids_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityIDWithAvoids(long entityID1,
                                                 long entityID2,
                                                 int maxDegrees,
                                                 string avoidedEntities,
                                                 out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);

                result = Sz_findPathByEntityIDWithAvoids_helper(entityID1,
                                                                entityID2,
                                                                maxDegrees,
                                                                avoidBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityIDWithAvoids_V2_helper(
            long entityID1,
            long entityID2,
            long maxDegrees,
            byte[] avoidedEntities,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityIDWithAvoids_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityIDWithAvoids(long entityID1,
                                                 long entityID2,
                                                 int maxDegrees,
                                                 string avoidedEntities,
                                                 long flags,
                                                 out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);

                result = Sz_findPathByEntityIDWithAvoids_V2_helper(entityID1,
                                                                   entityID2,
                                                                   maxDegrees,
                                                                   avoidBytes,
                                                                   flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordIDWithAvoids_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees,
            byte[] avoidedEntities);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordIDWithAvoids(string dataSourceCode1,
                                                 string recordID1,
                                                 string dataSourceCode2,
                                                 string recordID2,
                                                 int maxDegrees,
                                                 string avoidedEntities,
                                                 out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);

                result = Sz_findPathByRecordIDWithAvoids_helper(codeBytes1,
                                                                idBytes1,
                                                                codeBytes2,
                                                                idBytes2,
                                                                maxDegrees,
                                                                avoidBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordIDWithAvoids_V2_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees,
            byte[] avoidedEntities,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordIDWithAvoids_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordIDWithAvoids(string dataSourceCode1,
                                                 string recordID1,
                                                 string dataSourceCode2,
                                                 string recordID2,
                                                 int maxDegrees,
                                                 string avoidedEntities,
                                                 long flags,
                                                 out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);

                result = Sz_findPathByRecordIDWithAvoids_V2_helper(codeBytes1,
                                                                   idBytes1,
                                                                   codeBytes2,
                                                                   idBytes2,
                                                                   maxDegrees,
                                                                   avoidBytes,
                                                                   flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityIDIncludingSource_helper(
            long entityID1,
            long entityID2,
            long maxDegrees,
            byte[] avoidedEntities,
            byte[] requiredSources);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityIDIncludingSource_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityIDIncludingSource(long entityID1,
                                                      long entityID2,
                                                      int maxDegrees,
                                                      string avoidedEntities,
                                                      string requiredSources,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);
                byte[] srcBytes = Utilities.StringToUTF8Bytes(requiredSources);

                result = Sz_findPathByEntityIDIncludingSource_helper(entityID1,
                                                                     entityID2,
                                                                     maxDegrees,
                                                                     avoidBytes,
                                                                     srcBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByEntityIDIncludingSource_V2_helper(
            long entityID1,
            long entityID2,
            long maxDegrees,
            byte[] avoidedEntities,
            byte[] requiredSources,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByEntityIDIncludingSource_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByEntityIDIncludingSource(long entityID1,
                                                      long entityID2,
                                                      int maxDegrees,
                                                      string avoidedEntities,
                                                      string requiredSources,
                                                      long flags,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);
                byte[] srcBytes = Utilities.StringToUTF8Bytes(requiredSources);

                result = Sz_findPathByEntityIDIncludingSource_V2_helper(entityID1,
                                                                        entityID2,
                                                                        maxDegrees,
                                                                        avoidBytes,
                                                                        srcBytes,
                                                                        flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordIDIncludingSource_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees,
            byte[] avoidedEntities,
            byte[] requiredSources);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordIDIncludingSource_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordIDIncludingSource(string dataSourceCode1,
                                                      string recordID1,
                                                      string dataSourceCode2,
                                                      string recordID2,
                                                      int maxDegrees,
                                                      string avoidedEntities,
                                                      string requiredSources,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);
                byte[] srcBytes = Utilities.StringToUTF8Bytes(requiredSources);

                result = Sz_findPathByRecordIDIncludingSource_helper(codeBytes1,
                                                                     idBytes1,
                                                                     codeBytes2,
                                                                     idBytes2,
                                                                     maxDegrees,
                                                                     avoidBytes,
                                                                     srcBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findPathByRecordIDIncludingSource_V2_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long maxDegrees,
            byte[] avoidedEntities,
            byte[] requiredSources,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findPathByRecordIDIncludingSource_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindPathByRecordIDIncludingSource(string dataSourceCode1,
                                                      string recordID1,
                                                      string dataSourceCode2,
                                                      string recordID2,
                                                      int maxDegrees,
                                                      string avoidedEntities,
                                                      string requiredSources,
                                                      long flags,
                                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);
                byte[] avoidBytes = Utilities.StringToUTF8Bytes(avoidedEntities);
                byte[] srcBytes = Utilities.StringToUTF8Bytes(requiredSources);

                result = Sz_findPathByRecordIDIncludingSource_V2_helper(codeBytes1,
                                                                        idBytes1,
                                                                        codeBytes2,
                                                                        idBytes2,
                                                                        maxDegrees,
                                                                        avoidBytes,
                                                                        srcBytes,
                                                                        flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findNetworkByEntityID_helper(
            byte[] entityList,
            long maxDegrees,
            long buildOutDegrees,
            long maxEntities);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findNetworkByEntityID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindNetworkByEntityID(string entityList,
                                          int maxDegrees,
                                          int buildOutDegrees,
                                          int maxEntities,
                                          out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] entityBytes = Utilities.StringToUTF8Bytes(entityList);

                result = Sz_findNetworkByEntityID_helper(entityBytes,
                                                         maxDegrees,
                                                         buildOutDegrees,
                                                         maxEntities);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findNetworkByEntityID_V2_helper(
            byte[] entityList,
            long maxDegrees,
            long buildOutDegrees,
            long maxEntities,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findNetworkByEntityID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindNetworkByEntityID(string entityList,
                                          int maxDegrees,
                                          int buildOutDegrees,
                                          int maxEntities,
                                          long flags,
                                          out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] entityBytes = Utilities.StringToUTF8Bytes(entityList);

                result = Sz_findNetworkByEntityID_V2_helper(entityBytes,
                                                            maxDegrees,
                                                            buildOutDegrees,
                                                            maxEntities,
                                                            flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findNetworkByRecordID_helper(
            byte[] recordList,
            long maxDegrees,
            long buildOutDegrees,
            long maxEntities);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findNetworkByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindNetworkByRecordID(string recordList,
                                          int maxDegrees,
                                          int buildOutDegrees,
                                          int maxEntities,
                                          out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] recordBytes = Utilities.StringToUTF8Bytes(recordList);

                result = Sz_findNetworkByRecordID_helper(recordBytes,
                                                         maxDegrees,
                                                         buildOutDegrees,
                                                         maxEntities);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_findNetworkByRecordID_V2_helper(
            byte[] recordList,
            long maxDegrees,
            long buildOutDegrees,
            long maxEntities,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_findNetworkByRecordID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long FindNetworkByRecordID(string recordList,
                                          int maxDegrees,
                                          int buildOutDegrees,
                                          int maxEntities,
                                          long flags,
                                          out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] recordBytes = Utilities.StringToUTF8Bytes(recordList);

                result = Sz_findNetworkByEntityID_V2_helper(recordBytes,
                                                            maxDegrees,
                                                            buildOutDegrees,
                                                            maxEntities,
                                                            flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyRecordInEntity_helper(
            byte[] dataSourceCode, byte[] recordID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyRecordInEntity_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyRecordInEntity(string dataSourceCode,
                                      string recordID,
                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_whyRecordInEntity_helper(codeBytes, idBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyRecordInEntity_V2_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyRecordInEntity_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyRecordInEntity(string dataSourceCode,
                                      string recordID,
                                      long flags,
                                      out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_whyRecordInEntity_V2_helper(codeBytes, idBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyRecords_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyRecords_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyRecords(string dataSourceCode1,
                               string recordID1,
                               string dataSourceCode2,
                               string recordID2,
                               out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);

                result = Sz_whyRecords_helper(codeBytes1,
                                              idBytes1,
                                              codeBytes2,
                                              idBytes2);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyRecords_V2_helper(
            byte[] dataSourceCode1,
            byte[] recordID1,
            byte[] dataSourceCode2,
            byte[] recordID2,
            long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyRecords_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyRecords(string dataSourceCode1,
                               string recordID1,
                               string dataSourceCode2,
                               string recordID2,
                               long flags,
                               out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes1 = Utilities.StringToUTF8Bytes(dataSourceCode1);
                byte[] idBytes1 = Utilities.StringToUTF8Bytes(recordID1);
                byte[] codeBytes2 = Utilities.StringToUTF8Bytes(dataSourceCode2);
                byte[] idBytes2 = Utilities.StringToUTF8Bytes(recordID2);

                result = Sz_whyRecords_V2_helper(codeBytes1,
                                                 idBytes1,
                                                 codeBytes2,
                                                 idBytes2,
                                                 flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyEntities_helper(
            long entityID1, long entityID2);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyEntities_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyEntities(long entityID1,
                                long entityID2,
                                out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_whyEntities_helper(entityID1, entityID2);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_whyEntities_V2_helper(
            long entityID1, long entityID2, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_whyEntities_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long WhyEntities(long entityID1,
                                long entityID2,
                                long flags,
                                out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_whyEntities_V2_helper(entityID1, entityID2, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_howEntityByEntityID_helper(long entityID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_howEntityByEntityID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long HowEntityByEntityID(long entityID, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_howEntityByEntityID_helper(entityID);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_howEntityByEntityID_V2_helper(
            long entityID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_howEntityByEntityID_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long HowEntityByEntityID(long entityID,
                                        long flags,
                                        out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_howEntityByEntityID_V2_helper(entityID, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }

        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getVirtualEntityByRecordID_helper(
            byte[] recordList);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getVirtualEntityByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetVirtualEntityByRecordID(string recordList,
                                               out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] recordBytes = Utilities.StringToUTF8Bytes(recordList);

                result = Sz_getVirtualEntityByRecordID_helper(recordBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getVirtualEntityByRecordID_V2_helper(
            byte[] recordList, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getVirtualEntityByRecordID_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetVirtualEntityByRecordID(string recordList,
                                               long flags,
                                               out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] recordBytes = Utilities.StringToUTF8Bytes(recordList);

                result = Sz_getVirtualEntityByRecordID_V2_helper(recordBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getRecord_helper(
            byte[] dataSourceCode, byte[] recordID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getRecord_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetRecord(string dataSourceCode,
                              string recordID,
                              out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_getRecord_helper(codeBytes, idBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getRecord_V2_helper(
            byte[] dataSourceCode, byte[] recordID, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getRecord_V2_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetRecord(string dataSourceCode,
                              string recordID,
                              long flags,
                              out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] codeBytes = Utilities.StringToUTF8Bytes(dataSourceCode);
                byte[] idBytes = Utilities.StringToUTF8Bytes(recordID);

                result = Sz_getRecord_V2_helper(codeBytes, idBytes, flags);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_exportJSONEntityReport_helper(long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_exportJSONEntityReport_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long ExportJSONEntityReport(long flags, out IntPtr exportHandle)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;

            result = Sz_exportJSONEntityReport_helper(flags);
            exportHandle = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_exportCSVEntityReport_helper(
            byte[] csvColumnList, long flags);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_exportCSVEntityReport_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long ExportCSVEntityReport(string csvColumnList,
                                          long flags,
                                          out IntPtr exportHandle)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;

            byte[] columnBytes = Utilities.StringToUTF8Bytes(csvColumnList);
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < columnBytes.Length; index++)
            {
                sb.Append((char)columnBytes[index]);
            }
            Console.Error.WriteLine("***** COLUMNS: " + columnBytes + " / " + sb.ToString());

            result = Sz_exportCSVEntityReport_helper(columnBytes, flags);
            exportHandle = result.response;
            return result.returnCode;

        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_fetchNext_helper(IntPtr exportHandle);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_fetchNext_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long FetchNext(IntPtr exportHandle, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_fetchNext_helper(exportHandle);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long Sz_closeExport_helper(IntPtr exportHandle);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_closeExport_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long CloseExport(IntPtr exportHandle)
        {
            return Sz_closeExport_helper(exportHandle);
        }

        [DllImport("Sz")]
        private static extern long Sz_processRedoRecord(byte[] redoRecord);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_processRedoRecord"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ProcessRedoRecord(string redoRecord)
        {
            byte[] redoBytes = Utilities.StringToUTF8Bytes(redoRecord);

            return Sz_processRedoRecord(redoBytes);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_processRedoRecordWithInfo_helper(
            byte[] redoReord);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_processRedoRecordWithInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long ProcessRedoRecordWithInfo(string redoRecord,
                                              out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] redoBytes = Utilities.StringToUTF8Bytes(redoRecord);

                result = Sz_processRedoRecordWithInfo_helper(redoBytes);

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult Sz_getRedoRecord_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_getRedoRecord_helper"/>. 
        /// </summary>
        ///
        /// <returns>Zero (0) on success and non-zero on failure.</returns>
        public long GetRedoRecord(out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = Sz_getRedoRecord_helper();

                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long Sz_countRedoRecords();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="Sz_countRedoRecords"/>. 
        /// </summary>
        ///
        /// <returns>
        /// The number of redo records waiting to be processed, or a negative
        /// number if an error occurred.
        /// </returns>
        public long CountRedoRecords()
        {
            return Sz_countRedoRecords();
        }

    }
}