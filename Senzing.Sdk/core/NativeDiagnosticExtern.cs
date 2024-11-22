using System;
using System.Runtime.InteropServices;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides an implementation of <see cref="NativeDiagnostic"/>
    /// that will call the native equivalent <c>extern</c> functions.
    /// </summary>
    internal class NativeDiagnosticExtern : NativeDiagnostic
    {
        [DllImport("Sz")]
        private static extern int SzDiagnostic_init(
            byte[] moduleName, byte[] iniParams, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function
        /// <see cref="SzConfig_init"/>
        /// </summary>
        public long Init(string moduleName, string iniParams, bool verboseLogging)
        {
            return SzDiagnostic_init(Utilities.StringToUTF8Bytes(moduleName),
                                     Utilities.StringToUTF8Bytes(iniParams),
                                     (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern int SzDiagnostic_initWithConfigID(
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
            return SzDiagnostic_initWithConfigID(
                Utilities.StringToUTF8Bytes(moduleName),
                Utilities.StringToUTF8Bytes(iniParams),
                initConfigID,
                (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern int SzDiagnostic_reinit(long initConfigID);

        /// <summary>
        /// Implemented to call the external native function
        /// <see cref="SzConfig_reinit"/>.
        /// </summary>
        public long Reinit(long initConfigID)
        {
            return SzDiagnostic_reinit(initConfigID);
        }

        [DllImport("Sz")]
        private static extern long SzDiagnostic_destroy();

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_destroy()</c>.
        /// </summary>
        public long Destroy()
        {
            return SzDiagnostic_destroy();
        }

        [DllImport("Sz")]
        private static extern long SzDiagnostic_getLastException(
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
            long length = SzDiagnostic_getLastException(buf, buf.Length);
            if (length == 0L)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(buf, 0, (int)(length - 1));
        }

        [DllImport("Sz")]
        private static extern long SzDiagnostic_getLastExceptionCode();

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
            return SzDiagnostic_getLastExceptionCode();
        }

        [DllImport("Sz")]
        private static extern void SzDiagnostic_clearLastException();

        /// <summary>
        /// Implemented to clear the information pertaining to the last
        /// error encountered with the Senzing config funtions.
        /// </summary>
        public void ClearLastException()
        {
            SzDiagnostic_clearLastException();
        }

        /// <summary>
        /// Combines an <see cref="System.IntPtr""/> response with a
        /// <c>long</c> return code to handle the result from the 
        /// config helper functions.
        /// </summary>
        private struct SzPointerResult
        {
            public IntPtr response;
            public long returnCode;
        }


        [DllImport("Sz")]
        private static extern SzPointerResult SzDiagnostic_getDatastoreInfo_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzDiagnostic_getDatastoreInfo_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetDatastoreInfo(out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzDiagnostic_getDatastoreInfo_helper();
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzDiagnostic_checkDatastorePerformance_helper(
            long secondsToRun);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzDiagnostic_checkDatastorePerformance_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long CheckDatastorePerformance(int secondsToRun, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzDiagnostic_checkDatastorePerformance_helper(secondsToRun);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long SzDiagnostic_purgeRepository();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzDiagnostic_purgeRepository"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long PurgeRepository()
        {
            return SzDiagnostic_purgeRepository();
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzDiagnostic_getFeature_helper(long libFeatID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzDiagnostic_checkDatastorePerformance_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetFeature(long libFeatID, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzDiagnostic_getFeature_helper(libFeatID);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;

            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

    }
}