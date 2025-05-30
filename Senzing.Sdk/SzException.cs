using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Defines the base exception for Senzing errors.  This adds a property
    /// for the numeric Senzing error code which can optionally be set.
    /// </summary>
    public class SzException : Exception
    {
        /// <summary>
        /// Gets the underlying Senzing error code associated with the
        /// exception or <c>null</c> if no error code was associated
        /// with the exception.
        /// </summary>
        public long? ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        /// <summary>The underlying Senzing error code.</summary>
        private readonly long? errorCode;

        /// <summary>Default constructor.</summary>
        public SzException()
        {
            this.errorCode = null;
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception.
        /// </summary>
        /// 
        /// <param name="message">The message explaining the reason for the exception.</param>
        public SzException(string message)
            : base(message)
        {
            this.errorCode = null;
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception.
        /// </summary>
        /// 
        /// <param name="errorCode">The underlying Senzing error code.</param>
        ///
        /// <param name="message">The message explaining the reason for the exception.</param>
        public SzException(long? errorCode, string message)
            : base(message)
        {
            this.errorCode = errorCode;
        }

        /// <summary>
        /// Constructs with the <see cref="System.Exception"/> that is the
        /// underlying cause for the exception.
        /// </summary>
        /// 
        /// <param name="cause">The underlying cause for the exception.</param>
        public SzException(Exception cause)
            : base(null, cause)
        {
            this.errorCode = null;
        }

        /// <summary>
        /// Constructs with a message explaining the reason for the exception and
        /// the <c>Exception</c> that is the underlying cause for the exception.
        /// </summary>
        /// 
        /// <param name="message">The message explaining the reason for the exception.</param>
        ///
        /// <param name="cause">The underlying cause for the exception.</param>
        public SzException(string message, Exception cause)
            : base(message, cause)
        {
            this.errorCode = null;
        }

        /// <summary>
        /// Constructs with the Senzing error code, the message explaining the reason
        /// for the exception and the <c>Exception</c> that is the underlying cause
        /// for the exception.
        /// </summary>
        ///
        /// <param name="errorCode">The underlying Senzing error code.</param>
        ///
        /// <param name="message">The message explaining the reason for the exception.</param>
        ///
        /// <param name="cause">The underlying cause for the exception.</param>
        public SzException(long? errorCode, string message, Exception cause)
            : base(message, cause)
        {
            this.errorCode = errorCode;
        }

    }
}