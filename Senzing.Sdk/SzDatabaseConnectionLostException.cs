using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Extends <see cref="SzRetryableException"/> to define an exceptional
    /// condition where a database connection was lost causing a Senzing 
    /// operation to fail.  Retrying the operation would likely result in
    /// the connection being reestablished and the operation succeeding.
    /// </summary>
    public class SzDatabaseConnectionLostException : SzRetryableException
    {
        /// <summary>Default constructor.</summary>
        public SzDatabaseConnectionLostException()
            : base()
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message explaining the reason for the exception.
        /// </param>
        public SzDatabaseConnectionLostException(string message)
            : base(message)
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception.
        /// </summary>
        /// 
        /// <param name="errorCode">The underlying Senzing error code.</param>
        ///
        /// <param name="message">
        /// The message explaining the reason for the exception.
        /// </param>
        public SzDatabaseConnectionLostException(long? errorCode, string message)
            : base(errorCode, message)
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with the {@link Throwable} that is the underlying cause
        /// for the exception.
        /// </summary>
        /// 
        /// <param name="cause">The underlying cause for the exception.</param>
        public SzDatabaseConnectionLostException(Exception cause)
            : base(null, cause)
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception and
        /// the <c>Exception</c> that is the underlying cause for the exception.
        /// </summary>
        /// 
        /// <param name="message">
        /// The message explaining the reason for the exception.
        /// </param>
        ///
        /// <param name="cause">The underlying cause for the exception.</param>
        public SzDatabaseConnectionLostException(string message, Exception cause)
            : base(message, cause)
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with the Senzing error code, the message explaining the reason
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
        public SzDatabaseConnectionLostException(long? errorCode, string message, Exception cause)
            : base(errorCode, message, cause)
        {
            // do nothing
        }
    }
}