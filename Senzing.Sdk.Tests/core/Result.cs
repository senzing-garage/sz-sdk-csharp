namespace Senzing.Sdk.Tests.Core;

/// <summary>
/// Provides a simple wrapper for optional handling "out parameters" of any type.
/// </summary>
public class Result<T>
{
    /// <summary>
    /// The underlying value.
    /// </summary>
    private T? value;

    /// <summary>
    /// Default constructor.  This will construct with a <code>null</code> value.
    /// </summary>
    Result()
    {
        // do nothing
    }

    /// <summary>
    /// Constructs with the specified value.
    /// </summary>
    /// 
    /// <param name="value">
    /// The value with which to construct.
    /// </param>
    Result(T value)
    {
        this.value = value;
    }

    /// <summary>
    /// Sets the underlying value to the specified value.
    /// </summary>
    /// 
    /// <param name="value">The value to be set.</param>
    public void SetValue(T? value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the underlying value.
    /// </summary>
    /// 
    /// <returns>The underlying value.</returns>
    public T? Value
    {
        get
        {
            return this.value;
        }
    }
}

