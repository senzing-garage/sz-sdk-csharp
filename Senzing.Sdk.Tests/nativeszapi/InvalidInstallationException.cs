namespace Senzing.Sdk.Tests.NativeSzApi;

/// <summary>
/// This exception is thrown when the Senzing native installation is invalid.
/// </summary>
public class InvalidInstallationException : Exception
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public InvalidInstallationException() : base()
    {
        // do nothing more
    }

    /// <summary>
    /// Constructs with the specified message.
    /// </summary>
    /// 
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    public InvalidInstallationException(string message) : base(message)
    {
        // do nothing more
    }

    /// <summary>
    /// Constructs with the specified message and cause.
    /// </summary>
    ///
    /// <param name="message">
    /// The message explaining the reason for the exception.
    /// </param>
    ///
    /// <param name="cause">
    /// The underlying cause for the exception.
    /// </param>
    public InvalidInstallationException(string message, Exception cause)
        : base(message, cause)
    {
        // do nothing more
    }

    /// <summary>
    /// Constructs with the specified cause.
    /// </summary>
    /// 
    /// <param name="cause">
    /// The underlying cause for the exception.
    /// </param>
    public InvalidInstallationException(Exception cause) : base(null, cause)
    {
        // do nothing more
    }
}
