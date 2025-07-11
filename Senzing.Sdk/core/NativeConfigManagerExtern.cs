using System;
using System.Runtime.InteropServices;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides an implementation of <see cref="NativeConfigManager"/>
    /// that will call the native equivalent <c>extern</c> functions.
    /// </summary>
    internal class NativeConfigManagerExtern : NativeConfigManager
    {
        [DllImport("Sz")]
        private static extern int SzConfigMgr_init(
            byte[] moduleName, byte[] iniParams, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_init()</c>.
        /// </summary>
        public long Init(string moduleName, string iniParams, bool verboseLogging)
        {
            return SzConfigMgr_init(Utilities.StringToUTF8Bytes(moduleName),
                                    Utilities.StringToUTF8Bytes(iniParams),
                                    (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern long SzConfigMgr_destroy();

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_destroy()</c>.
        /// </summary>
        public long Destroy()
        {
            return SzConfigMgr_destroy();
        }

        [DllImport("Sz")]
        private static extern long SzConfigMgr_getLastException(
            [MarshalAs(UnmanagedType.LPArray)] byte[] buf, long length);

        /// <summary>
        /// Implemented to return the last exception message associated
        /// with the Senzing config functions.
        /// </summary>
        ///
        /// <remarks>
        /// This is most commonly called after a Senzing config function
        /// returns a failure code (non-zero or NULL).
        /// </remarks>
        ///
        /// <returns>An error message</returns>
        public string GetLastException()
        {
            byte[] buf = new byte[4096];
            long length = SzConfigMgr_getLastException(buf, buf.Length);
            if (length == 0L)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(buf, 0, (int)(length - 1));
        }

        [DllImport("Sz")]
        private static extern long SzConfigMgr_getLastExceptionCode();

        /// <summary>
        /// Implemented to return the last error code associated with the 
        /// Senzing config functions.
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
            return SzConfigMgr_getLastExceptionCode();
        }

        [DllImport("Sz")]
        private static extern void SzConfigMgr_clearLastException();

        /// <summary>
        /// Implemented to clear the information pertaining to the last
        /// error encountered with the Senzing config functions.
        /// </summary>
        public void ClearLastException()
        {
            SzConfigMgr_clearLastException();
        }

        /// <summary>
        /// Combines an <see cref="System.IntPtr"/> response with a
        /// <c>long</c> return code to handle the result from the 
        /// config helper functions.
        /// </summary>
        private struct SzPointerResult
        {
            public IntPtr response;
            public long returnCode;
        }

        /// <summary>
        /// Combines a <c>long</c> response with a <c>long</c> return code
        /// to handle the result from the config helper functions.
        /// </summary>
        private struct SzLongResult
        {
            public long response;
            public long returnCode;
        }

        [DllImport("Sz")]
        private static extern SzLongResult SzConfigMgr_registerConfig_helper(
            byte[] config, byte[] comments);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_registerConfig_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long RegisterConfig(string configStr, string configComments, out long configID)
        {
            SzLongResult result;
            result.response = 0L;
            result.returnCode = 0L;

            byte[] configBytes = Utilities.StringToUTF8Bytes(configStr);
            byte[] commentBytes = Utilities.StringToUTF8Bytes(configComments);

            result = SzConfigMgr_registerConfig_helper(configBytes, commentBytes);
            configID = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfigMgr_getConfig_helper(long configID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_getConfig_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetConfig(long configID, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzConfigMgr_getConfig_helper(configID);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfigMgr_getConfigRegistry_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_getConfigRegistry_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetConfigRegistry(out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzConfigMgr_getConfigRegistry_helper();
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long SzConfigMgr_setDefaultConfigID(long configID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_setDefaultConfigID"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long SetDefaultConfigID(long configID)
        {
            return SzConfigMgr_setDefaultConfigID(configID);
        }

        [DllImport("Sz")]
        private static extern SzLongResult SzConfigMgr_getDefaultConfigID_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_getDefaultConfigID_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long GetDefaultConfigID(out long configID)
        {
            SzLongResult result;
            result.response = 0L;
            result.returnCode = 0L;
            result = SzConfigMgr_getDefaultConfigID_helper();
            configID = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern long SzConfigMgr_replaceDefaultConfigID(
            long oldConfigID, long newConfigID);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfigMgr_replaceDefaultConfigID"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long ReplaceDefaultConfigID(long oldConfigID, long newConfigID)
        {
            return SzConfigMgr_replaceDefaultConfigID(oldConfigID, newConfigID);
        }
    }
}