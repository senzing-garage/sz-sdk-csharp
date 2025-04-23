using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Defines an exceptional condition where the failure is an 
    /// intermittent condition and the operation may be retried with
    /// the same parameters with an expectation of success.
    /// </summary>
    public class SzRetryableException : SzException
    {
        /// <summary>Default constructor.</summary>
        public SzRetryableException()
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
        public SzRetryableException(string message)
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
        public SzRetryableException(long? errorCode, string message)
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
        public SzRetryableException(Exception cause)
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
        public SzRetryableException(string message, Exception cause)
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
        public SzRetryableException(long? errorCode, string message, Exception cause)
            : base(errorCode, message, cause)
        {
            // do nothing
        }
    }
}