
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Senzing.Sdk
{
    /// <summary>
    /// Provides utilities for the implementation of the core Senzing SDK.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// The default buffer size for hex formatting.
        /// </summary>
        private const int HexBufferSize = 20;

        /// <summary>
        /// The number of bits to initially shift the <c>0xFFFF</c>
        /// mask for hex formatting.
        /// </summary>
        private const int HexMaskInitialShift = 48;

        /// <summary>
        /// The bit mask to use for hex formatting.
        /// </summary>
        private const int HexMask = 0xFFFF;

        /// <summary>
        /// The number of bits to shift the hex bit mask with each 
        /// loop iteration when hex formatting.
        /// </summary>
        private const int HexMaskBitShift = 16;

        /// <summary>
        /// The number of bits in a "nibble" (used for hex formatting).
        /// </summary>
        private const int HexNibbleSize = 4;

        /// <summary>
        /// The radix used for hexadecmial (16).
        /// </summary>
        private const int HexRadix = 16;

        /// <summary>
        /// The number of hex digits that are formatted adjacent before
        /// requiring a space.
        /// </summary>
        private const int HexDigitCount = 4;

        /// <summary>
        /// The number of additional characters required to escape a basic
        /// control character (e.g.: backspace, tab, newline and other whitespace).
        /// </summary>
        private const int JsonEscapeBasicCount = 1;

        /// <summary>
        /// The numeber of additional characters required to escape non-basic
        /// control characters (i.e.: those without shortcut escape sequences).
        /// </summary>
        private const int JsonEscapeControlCount = 6;

        /// <summary>
        /// Formats a <code>long</code> integer value as hexadecimal with 
        /// spaces between each group of for hex digits.
        /// </summary>
        /// 
        /// <param name="value">The value to format</param>
        /// 
        /// <returns>The value formatted as a <c>string</c>.</returns>
        internal static string HexFormat(Int64 value)
        {
            StringBuilder sb = new StringBuilder(HexBufferSize);
            Int64 mask = HexMask << HexMaskInitialShift;
            string prefix = "";

            for (int index = 0; index < HexNibbleSize; index++)
            {
                Int64 masked = value & mask;
                mask = (Int64)(((UInt64)mask) >> HexMaskBitShift);
                masked = (Int64)(((UInt64)masked) >> (((HexNibbleSize - 1) - index) * HexRadix));
                sb.Append(prefix);
                string hex = Convert.ToString(masked, HexRadix);
                for (int zero = hex.Length; zero < HexDigitCount; zero++)
                {
                    sb.Append("0");
                }
                sb.Append(hex);
                prefix = " ";
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes the specified {@link String} into a JSON string with the
        /// the surrounding double quotes.
        /// </summary>
        /// 
        /// <remarks>
        /// If the specified {@link String} is <c>null</c> then <c>"null"</c>
        /// is returned.
        /// </remarks>
        /// 
        /// <param name="text">The <c>string</c> to escape for JSON.</param>
        /// 
        /// <returns>
        /// The quoted escaped <c>string</c> or <c>"null"</c> if the specified
        /// parameter is <c>null</c>.
        /// </returns>
        public static string JsonEscape(string text)
        {
            if (text == null)
            {
                return "null";
            }
            int escapeCount = 0;
            for (int index = 0; index < text.Length; index++)
            {
                char c = text[index];
                int delta = 0;
                switch (c)
                {
                    case '\b':
                    case '\f':
                    case '\n':
                    case '\r':
                    case '\t':
                    case '"':
                    case '\\':
                        delta = JsonEscapeBasicCount;
                        break;
                    default:
                        delta = (c < ' ') ? JsonEscapeControlCount : 0;
                        break;
                };
                escapeCount += delta;
            }
            if (escapeCount == 0)
            {
                return "\"" + text + "\"";
            }
            StringBuilder sb = new StringBuilder(text.Length + escapeCount + 2);
            sb.Append('"');
            for (int index = 0; index < text.Length; index++)
            {
                char c = text[index];
                switch (c)
                {
                    case '"':
                    case '\\':
                        sb.Append('\\').Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        if (c >= ' ')
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            sb.Append("\\u00");
                            string hex = Convert.ToString((Int32)c, HexRadix);
                            if (hex.Length == 1)
                            {
                                sb.Append("0"); // one more zero if single-digit hex
                            }
                            sb.Append(hex);
                        }
                        break;
                }
            }
            sb.Append('"');

            // return the escaped string
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to a <b>null terminated</b> array of UTF-8 encoded bytes.
        /// </summary>
        /// 
        /// <param name="text">
        /// The <c>string</c> to encode.
        /// </param>
        /// 
        /// <returns>
        /// The null-terminated array of bytes.
        /// </returns>
        internal static byte[] StringToUTF8Bytes(string text)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes(text);
            byte[] bytes2 = new byte[bytes1.Length + 1];
            Array.Copy(bytes1, 0, bytes2, 0, bytes1.Length);
            bytes2[bytes2.Length - 1] = 0;
            return bytes2;
        }

        /// <summary>
        /// Converts a pointer a null-terminated array of UTF-8 encoded
        /// bytes to a <c>string</c>.
        /// </summary>
        /// 
        /// <param name="byteArrayPointer">
        /// The pointer to the UTF-8 byte array.
        /// </param>
        /// 
        /// <returns>
        /// The <c>string</c> obtained from decoding the UTF-8 byte array.
        /// </returns>
        internal static string UTF8BytesToString(IntPtr byteArrayPointer)
        {
            if (byteArrayPointer == IntPtr.Zero)
            {
                return null;
            }

            // determine the length of the array (look for null terminator)
            int len = 0;
            while (Marshal.ReadByte(byteArrayPointer, len) != 0)
            {
                ++len;
            }

            // allocate a buffer
            byte[] buffer = new byte[len];

            // copy the data to the byte buffer
            Marshal.Copy(byteArrayPointer, buffer, 0, buffer.Length);

            // decode as a string and return it
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Native method for freeing the buffer allocated by the
        /// native Senzing code.
        /// </summary>
        /// 
        /// <param name="pointer">
        /// The pointer to the buffer to be freed.
        /// </param>
        [DllImport("Sz")]
        private static extern void SzHelper_free(IntPtr pointer);

        /// <summary>
        /// Frees the byte array pointer that was allocated by the 
        /// native code.
        /// </summary>
        /// 
        /// <param name="pointer">
        /// The byte-array pointer to be freed.
        /// </param>
        internal static void FreeSzBuffer(IntPtr pointer)
        {
            SzHelper_free(pointer);
        }

        /// <summary>
        /// A random number generator.
        /// </summary>
        private static readonly Random PRNG = new Random(Environment.TickCount);

        /// <summary>
        /// Shuffles a list.
        /// </summary>
        internal static void Shuffle<T>(this IList<T> list)
        {
            lock (PRNG)
            {
                int count = list.Count;
                int last = count - 1;
                for (int index = 0; index < last; ++index)
                {
                    int randomIndex = PRNG.Next(index, count);
                    T tmp = list[index];
                    list[index] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }
    }
}