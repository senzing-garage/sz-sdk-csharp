namespace Senzing.Sdk.Core {
/// <summary>
/// Provides a base interface for Senzing native SDK's can have failures occur.
/// </summary>
internal interface NativeApi {
    /// <summary>
    /// Returns a string about the last error the system received.
    /// </summary>
    ///
    /// <remarks>
    /// This is most commonly called after an Senzing function returns
    /// a failure code (non-zero or NULL).
    /// </remarks>
    ///
    /// <returns>An error message</returns>
    ///
    string GetLastException();

    /// <summary>
    /// Returns the exception code of the last error the system received.
    /// </summary>
    ///
    /// <remarks>
    /// This is most commonly called after a Senzing funtion returns
    /// a failure code (non-zero or NULL).
    /// </remarks>
    ///
    /// <returns>An error code</returns>
    ///
    long GetLastExceptionCode();

    /// <summary>
    /// Clears the information about the last error the system received.
    /// </summary>
    void ClearLastException();
}
}