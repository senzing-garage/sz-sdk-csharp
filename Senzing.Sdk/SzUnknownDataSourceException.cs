using System;

namespace Senzing.Sdk {
/// <summary>
/// Extends <see cref="SzBadInputException"/> to define an exceptional
/// condition where the provided bad input to a Senzing operation is an
/// identifier that could not be used to successfully locate required
/// data for that operation.
/// </summary>
public class SzUnknownDataSourceException : SzBadInputException 
{
    /// <summary>Default constructor.</summary>
    public SzUnknownDataSourceException()
        : base()
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with a message explaing the reason for the exception.
    /// </summary>
    /// 
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    public SzUnknownDataSourceException(string message) 
        : base(message)
    { 
        // do nothing
    }

    /// <summary>
    /// Constructs with a message explaing the reason for the exception.
    /// </summary>
    /// 
    /// <param name="errorCode">The underlying Senzing error code.</param>
    ///
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    public SzUnknownDataSourceException(long? errorCode, string message) 
        : base(message)
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with the {@link Throwable} that is the underlying cause
    /// for the exception.
    /// </summary>
    /// 
    /// <param name="cause">The underlying cause for the exception.</param>
    public SzUnknownDataSourceException(Exception cause) 
        : base(null, cause)
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with a message explaing the reason for the exception and
    /// the <c>Exception</c> that is the underlying cause for the exception.
    /// </summary>
    /// 
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    ///
    /// <param name="cause">The underlying cause for the exception.</param>
    public SzUnknownDataSourceException(string message, Exception cause) 
        : base(message, cause)
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with the Senzing error code, the message explaing the reason
    /// for the exception and the <c>Exception</c> that is the underlying cause
    /// for the exception.
    /// </summary>
    ///
    /// <param name="errorCode">The underlying Senzing error code.</param>
    ///
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    ///
    /// <param name="cause">The underlying cause for the exception.</param>
    public SzUnknownDataSourceException(long?        errorCode, 
                                        string      message, 
                                        Exception   cause) 
        : base(message, cause)
    {
        // do nothing
    }
}
}