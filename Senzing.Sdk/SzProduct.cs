namespace Senzing.Sdk
{

    /// <summary>
    /// Defines the C# interface to the Senzing product functions.
    /// </summary>
    /// 
    /// <remarks>
    /// The Senzing product functions provide information regarding the Senzing product
    /// installation and user license.
    /// </remarks>
    /// 
    /// <example>
    /// For example:
    /// </example>
    public interface SzProduct
    {
        ///
        /// <summary>Returns the currently configured license details.</summary>
        ///
        /// <returns>The JSON document describing the license details.</returns>
        /// 
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetLicense();

        /// <summary>Returns the currently installed version details.</summary>
        ///
        /// <returns>The JSON document of version details.</returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetVersion();
    }
}