using System;

namespace Senzing.Sdk {
/// <summary>
/// Defines an exceptional condition when an invalid input value is
/// provided to a Senzing operation preventing the successful
/// completion of that operation.
/// </summary>
public class SzReplaceConflictException : SzException 
{
    /// <summary>Default constructor.</summary>
    public SzReplaceConflictException()
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
    public SzReplaceConflictException(string message) 
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
    public SzReplaceConflictException(long? errorCode, string message) 
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
    public SzReplaceConflictException(Exception cause) 
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
    public SzReplaceConflictException(string message, Exception cause) 
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
    public SzReplaceConflictException(long? errorCode, string message, Exception cause) 
        : base(message, cause)
    {
        // do nothing
    }
}
}