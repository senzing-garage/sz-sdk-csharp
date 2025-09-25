using System;

namespace Senzing.Sdk
{
    /// <summary>
    /// Extends <see cref="InvalidOperationException"/> so the exceptional 
    /// condition of the <see cref="SzEnvironment"/> already being destroyed
    /// can be differentiated from other <see cref="InvalidOperationException"/>
    /// instances that might be thrown.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// This exception can be thrown from almost all Senzing SDK functions if
    /// the associated <see cref="SzEnvironment"/>  has been destroyed.
    /// </para>
    /// 
    /// <para>
    /// <b>NOTE:</b> This class does <b>not</b> extend <see cref="SzException"/>
    /// but rather extends <see cref="InvalidOperationException"/>.
    /// </para>
    /// </remarks>
    public class SzEnvironmentDestroyedException : InvalidOperationException
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SzEnvironmentDestroyedException() : base()
        {
            // do nothing            
        }

        /// <summary>
        /// Constructs with the specified message.
        /// </summary>
        /// 
        /// <param name="message">The message describing what occurred.</param>
        public SzEnvironmentDestroyedException(String message) : base(message)
        {
            // do nothing            
        }

        /// <summary>
        /// Constructs with the specified message and <see cref="Exception"/> cause.
        /// </summary>
        /// 
        /// <param name="message">The message describing what occurred.</param>
        /// 
        /// <param name="cause">The <see cref="Exception"/> cause</param>
        public SzEnvironmentDestroyedException(String message, Exception cause)
            : base(message, cause)
        {
            // do nothing
        }

        /// <summary>
        /// Constructs with the specified <see cref="Exception"/> cause.
        /// </summary>
        /// 
        /// <param name="cause">The <see cref="Exception"/> cause</param>
        public SzEnvironmentDestroyedException(Exception cause)
            : base(null, cause)
        {
            // do nothing
        }
    }
}