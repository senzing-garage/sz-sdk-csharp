using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Defines an exceptional condition when a failure has occurred
    /// pertaining to the Senzing configuration.
    /// </summary>
    public class SzConfigurationException : SzException
    {
        /// <summary>Default constructor.</summary>
        public SzConfigurationException()
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
        public SzConfigurationException(string message)
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
        public SzConfigurationException(long? errorCode, string message)
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
        public SzConfigurationException(Exception cause)
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
        public SzConfigurationException(string message, Exception cause)
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
        public SzConfigurationException(long? errorCode, string message, Exception cause)
            : base(errorCode, message, cause)
        {
            // do nothing
        }
    }
}