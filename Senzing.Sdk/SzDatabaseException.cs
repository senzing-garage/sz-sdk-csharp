using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Extends <see cref="SzUnrecoverableException"/> to define an exceptional
    /// condition triggered by a database error from which we cannot recover
    /// (e.g.: missing or unexpected schema definition).
    /// </summary>
    public class SzDatabaseException : SzUnrecoverableException
    {
        /// <summary>Default constructor.</summary>
        public SzDatabaseException()
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
        public SzDatabaseException(string message)
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
        public SzDatabaseException(long? errorCode, string message)
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
        public SzDatabaseException(Exception cause)
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
        public SzDatabaseException(string message, Exception cause)
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
        public SzDatabaseException(long? errorCode, string message, Exception cause)
            : base(errorCode, message, cause)
        {
            // do nothing
        }
    }
}