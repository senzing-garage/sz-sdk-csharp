using System;
using System.Runtime.InteropServices;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides the external native implementation of <see cref="NativeProduct"/>.
    /// </summary>
    internal class NativeProductExtern : NativeProduct
    {
        [DllImport("Sz")]
        private static extern int SzProduct_init(byte[] moduleName, byte[] iniParams, long verboseLogging);

        /// <summary>
        /// Implemented to call the external native function <c>SzProduct_init()</c>.
        /// </summary>
        public long Init(string moduleName, string iniParams, bool verboseLogging)
        {
            return SzProduct_init(Utilities.StringToUTF8Bytes(moduleName),
                                  Utilities.StringToUTF8Bytes(iniParams),
                                  (verboseLogging) ? 1 : 0);
        }

        [DllImport("Sz")]
        private static extern long SzProduct_destroy();

        /// <summary>
        /// Implemented to call the external native function <c>SzProduct_destroy()</c>.
        /// </summary>
        public long Destroy()
        {
            return SzProduct_destroy();
        }

        [DllImport("Sz")]
        private static extern IntPtr SzProduct_getLicense();

        /// <summary>
        /// Implemented to call the external native function <c>SzProduct_getLicense()</c>.
        /// </summary>
        public string GetLicense()
        {
            return Utilities.UTF8BytesToString(SzProduct_getLicense());
        }

        [DllImport("Sz")]
        private static extern IntPtr SzProduct_getVersion();

        /// <summary>
        /// Returns the currently installed version details.
        /// </summary>
        ///
        /// <returns>A JSON document describing version details.</returns>
        public string GetVersion()
        {
            return Utilities.UTF8BytesToString(SzProduct_getVersion());
        }

        [DllImport("Sz")]
        private static extern long SzProduct_getLastException([MarshalAs(UnmanagedType.LPArray)] byte[] buf, long length);

        /// <summary>
        /// Implemented to return the last exception message associated
        /// with the Senzing product functions.
        /// </summary>
        ///
        /// <remarks>
        /// This is most commonly called after a Senzing product function
        /// returns a failure code (non-zero or NULL).
        /// </remarks>
        ///
        /// <returns>An error message</returns>
        public string GetLastException()
        {
            byte[] buf = new byte[4096];
            long length = SzProduct_getLastException(buf, buf.Length);
            if (length == 0L)
            {
                return "";
            }
            return System.Text.Encoding.UTF8.GetString(buf, 0, (int)(length - 1));
        }

        [DllImport("Sz")]
        private static extern long SzProduct_getLastExceptionCode();

        /// <summary>
        /// Implemented to return the last error code associated with the 
        /// Senzing product functions.
        /// </summary>
        ///
        /// <remarks>
        /// This is most commonly called after a Senzing product function
        /// returns a failure code (non-zero or NULL).
        /// </remarks>
        ///
        /// <returns>An error code</returns>
        ///
        public long GetLastExceptionCode()
        {
            return SzProduct_getLastExceptionCode();
        }

        [DllImport("Sz")]
        private static extern void SzProduct_clearLastException();

        /// <summary>
        /// Implemented to clear the information pertaining to the last
        /// error encountered with the Senzing product functions.
        /// </summary>
        public void ClearLastException()
        {
            SzProduct_clearLastException();
        }

    }
}