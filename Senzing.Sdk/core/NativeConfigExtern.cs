using System;
using System.Runtime.InteropServices;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides an implementation of <see cref="NativeConfig"/> that will
    /// call the native equivalent <c>extern</c> functions.
    /// </summary>
    internal class NativeConfigExtern : NativeConfig
    {
        [DllImport("Sz")]
        private static extern int SzConfig_init(byte[] moduleName, byte[] iniParams, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_init()</c>.
        /// </summary>
        public long Init(string moduleName, string iniParams, bool verboseLogging)
        {
            return SzConfig_init(Utilities.StringToUTF8Bytes(moduleName),
                                 Utilities.StringToUTF8Bytes(iniParams),
                                 (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern long SzConfig_destroy();

        /// <summary>
        /// Implemented to call the external native function <c>SzConfig_destroy()</c>.
        /// </summary>
        public long Destroy()
        {
            return SzConfig_destroy();
        }

        [DllImport("Sz")]
        private static extern long SzConfig_getLastException([MarshalAs(UnmanagedType.LPArray)] byte[] buf, long length);

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
            long length = SzConfig_getLastException(buf, buf.Length);
            if (length == 0L)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(buf, 0, (int)(length - 1));
        }

        [DllImport("Sz")]
        private static extern long SzConfig_getLastExceptionCode();

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
            return SzConfig_getLastExceptionCode();
        }

        [DllImport("Sz")]
        private static extern void SzConfig_clearLastException();

        /// <summary>
        /// Implemented to clear the information pertaining to the last
        /// error encountered with the Senzing config functions.
        /// </summary>
        public void ClearLastException()
        {
            SzConfig_clearLastException();
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

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfig_create_helper();

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_create_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long Create(out IntPtr configHandle)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;

            result = SzConfig_create_helper();
            configHandle = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfig_load_helper(byte[] bytes);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_load_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long Load(string jsonConfig, out IntPtr configHandle)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;

            byte[] bytes = Utilities.StringToUTF8Bytes(jsonConfig);
            result = SzConfig_load_helper(bytes);
            configHandle = result.response;
            return result.returnCode;
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfig_save_helper(IntPtr handle);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_save_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long Save(IntPtr configHandle, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzConfig_save_helper(configHandle);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long SzConfig_close_helper(IntPtr handle);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_close_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long Close(IntPtr configHandle)
        {
            return SzConfig_close_helper(configHandle);
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfig_listDataSources_helper(
            IntPtr handle);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_listDataSources_helper"/>. 
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long ListDataSources(IntPtr configHandle, out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                result = SzConfig_listDataSources_helper(configHandle);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern SzPointerResult SzConfig_addDataSource_helper(
            IntPtr handle, byte[] bytes);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_addDataSource_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long AddDataSource(IntPtr configHandle,
                                  string inputJson,
                                  out string response)
        {
            SzPointerResult result;
            result.response = IntPtr.Zero;
            result.returnCode = 0L;
            try
            {
                byte[] bytes = Utilities.StringToUTF8Bytes(inputJson);
                result = SzConfig_addDataSource_helper(configHandle, bytes);
                response = Utilities.UTF8BytesToString(result.response);
                return result.returnCode;
            }
            finally
            {
                Utilities.FreeSzBuffer(result.response);
            }
        }

        [DllImport("Sz")]
        private static extern long SzConfig_deleteDataSource_helper(IntPtr handle,
                                                                    byte[] bytes);

        /// <summary>
        /// Implemented to call the external native helper function 
        /// <see cref="SzConfig_deleteDataSource_helper"/>.
        /// </summary>
        ///
        /// <returns>
        /// Zero (0) on success and non-zero on failure.
        /// </returns>
        public long DeleteDataSource(IntPtr configHandle, string inputJson)
        {
            byte[] bytes = Utilities.StringToUTF8Bytes(inputJson);
            return SzConfig_deleteDataSource_helper(configHandle, bytes);
        }

    }
}