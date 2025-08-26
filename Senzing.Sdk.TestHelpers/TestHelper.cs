namespace Senzing.Sdk.TestHelpers;

using System;

/// <summary>
/// Provides a common interface for all test helpers.
/// </summary>
interface TestHelper
{
    /// <summary>
    /// Implement this method to provide entry point for execution.
    /// </summary>
    /// 
    /// <param name="args">The command-line arguments</param>
    void Execute(string[] args, Action<object?> logger);
}