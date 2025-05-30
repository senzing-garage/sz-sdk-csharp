namespace Senzing.Sdk.Tests.Core;

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Senzing.Sdk.Core;

using static System.StringComparison;

/// <summary>
/// Provides logging utilities.
/// </summary>
public static class LoggingUtilities
{
    /// <summary>
    /// Used for good measure to avoid interlacing of out debug output.
    /// </summary>
    private static readonly object StdOutMonitor = new object();

    /// <summary>
    /// Used for good measure to avoid interlacing of out debug output.
    /// </summary>
    private static readonly object StdErrMonitor = new object();

    /// <summary>
    ///  The data-time format info provider.
    /// </summary>
    private static readonly DateTimeFormatInfo DateTimeFormatting = new DateTimeFormatInfo();

    /// <summary>
    /// The date-time pattern for the build number.
    /// </summary>
    private const string LogDatePattern = "yyyy-MM-dd HH:mm:ss,SSS";

    /// <summary>
    /// The base product ID to log with if the calling package is not overridden.
    /// </summary>
    public const string BaseProductID = "5025";

    /// <summary>
    /// The last logged exception in this thread.
    /// </summary>
    private static readonly ThreadLocal<long?> LastLoggedException
        = new ThreadLocal<long?>();

    /// <summary>
    /// The dictionary of package prefixes to product ID's.
    /// </summary>
    private static readonly IDictionary<string, string> ProductIDMap
        = new Dictionary<string, string>();

    /// <summary>
    /// Sets the product ID to use when logging messages for classes in the
    /// specified package name.
    /// </summary>
    ///
    /// <param name="namespaceName">The namespace name.</param>
    /// <param name="productId">The product ID to use for the package.</param>
    public static void SetProductIdForPackage(string packageName,
                                                string productId)
    {
        lock (ProductIDMap)
        {
            ProductIDMap.Add(packageName, productId);
        }
    }

    /// <summary>
    /// Produces a single or multi-line error log message with a consistent prefix
    /// and timestamp.
    /// </summary>
    ///
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    public static void LogError(params object[] lines)
    {
        Log(Console.Error, StdErrMonitor, "ERROR", lines, null);
    }

    /// <summary>
    /// Produces a multi-line error log message with a consistent prefix
    /// and timestamp that will conclude with the stack trace for the
    /// specified <see cref="System.Exception"/>.
    /// </summary>
    ///
    /// <param name="exception">
    /// The <see cref="System.Exception"/> whose stack trace should be logged.
    /// </param>
    /// 
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString</c>.
    /// </param>
    public static void LogError(Exception exception, params object[] lines)
    {
        Log(Console.Error, StdErrMonitor, "ERROR", lines, exception);
    }

    /// <summary>
    /// Produces a single or multi-line warning log message with a consistent
    /// prefix and timestamp.
    /// </summary>
    ///
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    public static void LogWarning(params object[] lines)
    {
        Log(Console.Error, StdErrMonitor, "WARNING", lines, null);
    }

    /// <summary>
    /// Produces a multi-line warning log message with a consistent prefix
    /// and timestamp that will conclude with the stack trace for the specified
    /// <see cref="System.Exception"/>.
    /// </summary>
    /// 
    /// <param name="exception">
    /// The <see cref="System.Exception"/> whose stack trace should be logged.
    /// </param>
    /// 
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    public static void LogWarning(Exception exception, params object[] lines)
    {
        Log(Console.Error, StdErrMonitor, "WARNING", lines, exception);
    }

    /// <summary>
    /// Produces a single or multi-line error log message with a consistent prefix
    /// and timestamp.
    /// </summary>
    ///
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    public static void LogInfo(params object[] lines)
    {
        Log(Console.Error, StdOutMonitor, "INFO", lines, null);
    }

    /// <summary>
    /// Gets the product ID for the namespace name of the class performing
    /// the logging.
    /// </summary>
    ///
    /// <param name="namespaceName">The namespace name</param>
    ///
    /// <returns>The product ID with which to log</returns>
    public static string GetProductIdForNamespace(string namespaceName)
    {
        ArgumentNullException.ThrowIfNull(namespaceName, nameof(namespaceName));
        lock (ProductIDMap)
        {
            do
            {
                // check if we have a product ID for the package
                if (ProductIDMap.TryGetValue(namespaceName, out string? found))
                {
                    return found;
                }

                // check if the package name begins with com.senzing and get next part
                int prefixLength = "Senzing.".Length;
                if (namespaceName.StartsWith("Senzing.", Ordinal)
                    && namespaceName.Length > prefixLength)
                {
                    int idx = namespaceName.IndexOf('.', prefixLength);
                    if (idx < 0) idx = namespaceName.Length;
                    return namespaceName.Substring(prefixLength, idx);
                }

                // strip off the last part of the package name
                int index = namespaceName.LastIndexOf('.');
                if (index <= 0) break;
                if (index == (namespaceName.Length - 1)) break;
                namespaceName = namespaceName.Substring(0, index);

            } while (namespaceName.Length > 0 && !namespaceName.Equals("Senzing", Ordinal));

            // return the base product ID if we get here
            return BaseProductID;
        }
    }

    /// <summary>
    /// Produces a single or multi-line log message with a consistent prefix
    /// and timestamp using the specified <see cref="System.Text.TextWriter"/>,
    /// <c>object</c> to lock, <c>string</c> log type and lines of text to log.
    /// </summary>
    ///
    /// <param name="textWriter">
    /// The <see cref="System.Text.TextWriter"/> to which to write the log message.
    /// </param>
    /// 
    /// <param name="monitor">
    /// The <c>object</c> to lock on when writing and flushing the
    /// <see cref="System.Text.TextWriter"/>
    /// </param>
    ///                
    /// <param name="logType">
    /// The logging type such as <c>"DEBUG"</c> or <c>"ERROR"</c>.
    /// </param>
    /// 
    /// <param name="lines">
    /// The lines of text to log, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    /// 
    /// <param name="exception">
    /// The <see cref="System.Exception"/> whose stack trace should be logged.
    /// </param>
    private static void Log(TextWriter textWriter,
                            object monitor,
                            string logType,
                            object[] lines,
                            Exception? exception)
    {
        StackTrace stackTrace = new StackTrace(3, true);
        StackFrame? caller = stackTrace.GetFrame(0);
        MethodBase? method = (caller == null) ? null : caller.GetMethod();
        Type? callingType = (method == null) ? null : method.DeclaringType;
        string? typeName = (callingType == null) ? "" : callingType.FullName;
        string callerClass = (typeName == null) ? "" : typeName;

        int index = callerClass.LastIndexOf('.');

        string namespaceName = callerClass.Substring(0, index);
        callerClass = callerClass.Substring(index + 1);

        string productId = GetProductIdForNamespace(namespaceName);

        StringBuilder sb = new StringBuilder();
        DateTime now = DateTime.UtcNow;
        string timestamp = now.ToString(LogDatePattern, DateTimeFormatting);

        sb.Append(timestamp).Append(" senzing-").Append(productId)
            .Append(" (").Append(logType).Append(')')
            .Append(" [").Append(Thread.CurrentThread.Name)
            .Append('|').Append(callerClass).Append('.')
            .Append(caller?.GetMethod()?.Name).Append(':')
            .Append(caller?.GetFileLineNumber()).Append("] ")
            .Append(MultilineFormat(lines));

        // handle the stack trace if a throwable is provided
        if (exception != null)
        {
            sb.AppendLine(exception.ToString());
        }

        lock (monitor)
        {
            textWriter.Write(sb);
            textWriter.Flush();
        }
    }

    /// <summary>
    /// Formats a multiline message.
    /// </summary>
    ///
    /// <param name="lines">
    /// The lines of text to be formatted, which may be objects that will be converted
    /// to text via <c>ToString()</c>.
    /// </param>
    ///
    /// <returns>The formatted multiline <c>string</c></returns>
    public static string MultilineFormat(params object[] lines)
    {
        ArgumentNullException.ThrowIfNull(lines, nameof(lines));
        StringBuilder sb = new StringBuilder();
        foreach (object line in lines)
        {
            sb.AppendLine("" + line);
        }
        return sb.ToString();
    }


    /// <summary>
    /// Formats an error message from a <see cref="NativeApi"/> instance,
    /// including the details (which may contain sensitive PII information)
    /// as part of the result.
    /// </summary>
    /// 
    /// <param name="operation">
    /// The name of the operation from the native API interface that was
    /// attempted but failed.
    /// </param>
    /// 
    /// <param name="fallible">
    /// The <see cref="Senzing.Sdk.Core.NativeApi"/> from which to extract
    /// the error message.
    /// </param>
    /// 
    /// <returns>The multi-line formatted log message.</returns>
    internal static string FormatError(string operation, NativeApi fallible)
    {
        return FormatError(operation, fallible, true);
    }

    /// <summary>
    /// Formats an error message from a <see cref="Senzing.Sdk.Core.NativeApi"/>
    /// instance, optionally including the details (which may contain sensitive
    /// PII information) as part of the result.
    /// </summary>
    ///
    /// <param name="operation">
    /// The name of the operation from the native API interface that was
    /// attempted but failed.
    /// </param>
    /// 
    /// <param name="fallible">
    /// The <see cref="Senzing.Sdk.Core.NativeApi"/> from which to extract the
    /// error message.
    /// </param>
    /// 
    /// <param name="includeDetails">
    /// <c>true</c> to include the details of the failure failure in the
    /// resultant message, and <c>false</c> to exclude them (usually to
    /// avoid logging sensitive information).
    /// </param>
    /// 
    /// <returns>The multi-line formatted log message.</returns>
    internal static string FormatError(string operation,
                                       NativeApi fallible,
                                       bool includeDetails)
    {
        long errorCode = fallible.GetLastExceptionCode();
        StringBuilder sb = new StringBuilder();

        sb.AppendLine();
        sb.AppendLine("Operation Failed : " + operation);
        sb.AppendLine("Error Code       : " + errorCode);

        if (includeDetails)
        {
            string message = fallible.GetLastException();
            sb.AppendLine("Reason           : " + message);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Logs an error message from a <see cref="Senzing.Sdk.Core.NativeApi"/>
    /// instance, including the details (which may contain sensitive PII
    /// information) as part of the result.
    /// </summary>
    /// 
    /// <param name="operation">
    /// The name of the operation from the native API interface that was
    /// attempted but failed.
    /// </param>
    /// 
    /// <param name="fallible">
    /// The <see cref="Senzing.Sdk.Core.NativeApi"/> from which to extract the
    /// error message.
    /// </param>
    internal static void LogError(string operation, NativeApi fallible)
    {
        LogError(operation, fallible, true);
    }

    /// <summary>
    /// Logs an error message from a <see cref="Senzing.Sdk.Core.NativeApi"/>
    /// instance, optionally including the details (which may contain
    /// sensitive PII information) as part of the result.
    /// </summary>
    ///
    /// <param name="operation">
    /// The name of the operation from the native API interface that was
    /// attempted but failed.
    /// </param>
    /// 
    /// <param name="fallible">
    /// The <see cref="Senzing.Sdk.Core.NativeApi"/> from which to extract the
    /// error message.
    /// </param>
    /// 
    /// <param name="includeDetails">
    /// <c>true</c> to include the details of the failure failure in the
    /// resultant message, and <c>false</c> to exclude them (usually to
    /// avoid logging sensitive information).
    /// </param>
    internal static void LogError(string operation,
                                  NativeApi fallible,
                                  bool includeDetails)
    {
        string message = FormatError(operation, fallible, includeDetails);
        Console.Error.WriteLine(message);
    }

    /// <summary>
    /// Convert a throwable to a <c>long</c> value so we don't keep a reference
    /// to what could be a complex exception object.
    /// </summary>
    /// 
    /// <param name="e">The exception to convert</param>
    /// 
    /// <returns>
    /// The long hash representation to identify the throwable instance.
    /// </returns>
    private static long? ExceptionToLong(Exception? e)
    {
        if (e == null) return null;
        long hash1 = (long)RuntimeHelpers.GetHashCode(e);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(e.ToString());

        long hash2 = (long)sb.ToString().GetHashCode(Ordinal);
        return ((hash1 << 32) | hash2);
    }

    /// <summary>
    /// Checks if the specified <see cref="System.Exception"/> is the last
    /// logged exception.
    /// </summary>
    /// 
    /// <remarks>
    /// This is handy for telling if the exception has already been logged by a
    /// deeper level of the stack trace.
    /// </remarks>
    /// 
    /// <param name="e">The <see cref="System.Exception"/> to check</param>
    /// 
    /// <returns>
    /// <c>true</c> if it is the last logged exception, otherwise <c>false</c>.
    /// </returns>
    public static bool IsLastLoggedException(Exception e)
    {
        if (e == null) return false;
        if (LastLoggedException.Value == null) return false;
        long? value = ExceptionToLong(e);
        return (LastLoggedException.Value == value);
    }

    /// <summary>
    /// Sets the specified <see cref="System.Exception"/> as the last logged
    /// exception.
    /// </summary>
    /// 
    /// <remarks>
    /// This is handy for telling if the exception has already been logged by a
    /// deeper level of the stack trace.
    /// </remarks>
    /// 
    /// <param name="e">
    /// The <see cref="System.Exception"/> to set as the last logged exception.
    /// </param>
    public static void SetLastLoggedException(Exception e)
    {
        LastLoggedException.Value = ExceptionToLong(e);
    }

    /// <summary>
    /// Sets the last logged exception and rethrows the specified exception.
    /// </summary>
    /// 
    /// <param name="e">
    /// The <see cref="System.Exception"/> to set as the last logged exception.
    /// </param>
    public static void SetLastLoggedAndThrow(Exception e)
    {
        SetLastLoggedException(e);
        throw e;
    }

    /// <summary>
    /// Conditionally logs to stderr the specified <see cref="System.Exception"/>
    /// is <b>not</b> the last logged exception and then rethrows it.
    /// </summary>
    ///
    /// <param name="e">
    /// The <see cref="System.Exception"/> to set as the last logged exception.
    /// </param>
    /// 
    /// <returns>
    /// Never returns anything since it always throws.
    /// </returns>
    public static Exception LogOnceAndThrow(Exception e)
    {
        if (!IsLastLoggedException(e))
        {
            Console.Error.WriteLine(e);
        }
        SetLastLoggedAndThrow(e);
        return new TestException(); // we never get here
    }

    /// <summary>
    /// Formats the specified exception as a <c>string</c>.
    /// </summary>
    public static string FormatException(string? msg, Exception e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        StringBuilder sb = new StringBuilder();
        if (msg != null)
        {
            sb.AppendLine(msg);
            sb.AppendLine("-----------------------------------------");
        }
        sb.AppendLine(e.ToString());
        sb.AppendLine(e.StackTrace);
        return sb.ToString();
    }
}
