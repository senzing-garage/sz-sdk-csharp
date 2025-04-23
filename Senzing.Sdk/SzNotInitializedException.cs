using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Extends <see cref="SzUnrecoverableException"/> to define an exceptional
    /// condition triggered by Senzing not being initialized.
    /// </summary>
    public class SzNotInitializedException : SzUnrecoverableException
    {
        /// <summary>Default constructor.</summary>
        public SzNotInitializedException()
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
        public SzNotInitializedException(string message)
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
        public SzNotInitializedException(long? errorCode, string message)
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
        public SzNotInitializedException(Exception cause)
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
        public SzNotInitializedException(string message, Exception cause)
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
        public SzNotInitializedException(long? errorCode, string message, Exception cause)
            : base(errorCode, message, cause)
        {
            // do nothing
        }
    }
}