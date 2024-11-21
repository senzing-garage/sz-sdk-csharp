namespace Senzing.Sdk.Tests;

using System;

/// <summary>
/// Defines the base exception for Senzing errors.  This adds a property
/// for the numeric Senzing error code which can optionally be set.
/// </summary>
public class TestException : Exception
{
    /// <summary>Default constructor.</summary>
    public TestException()
        : base() 
    {

    }

    /// <summary>
    /// Constructs with a message explaing the reason for the exception.
    /// </summary>
    /// 
    /// <param name="message">The message explaining the reason for the exception.</param>
    public TestException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Constructs with the <see cref="System.Exception"/> that is the
    /// underlying cause for the exception.
    /// </summary>
    /// 
    /// <param name="cause">The underlying cause for the exception.</param>
    public TestException(Exception cause)
        : base(null, cause)
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with a message explaing the reason for the exception and
    /// the <c>Exception</c> that is the underlying cause for the exception.
    /// </summary>
    /// 
    /// <param name="message">The message explaining the reason for the exception.</param>
    ///
    /// <param name="cause">The underlying cause for the exception.</param>
    public TestException(string message, Exception cause)
        : base(message, cause)
    {
        // do nothing
    }
}
