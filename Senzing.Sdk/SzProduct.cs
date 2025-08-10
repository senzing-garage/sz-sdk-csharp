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
    /// An `SzProduct` instance is typically obtained from an
    /// <see cref="SzEnvironment"/> instance via the
    /// <see cref="SzEnvironment.GetProduct"/> method as follows.
    /// 
    /// For example:
    /// <include file="../target/examples/SzProductDemo_GetProduct.xml" path="/*"/>
    /// </example>
    public interface SzProduct
    {
        /// <summary>Gets the details and entitlements of the applied product license.</summary>
        ///
        /// <remarks>
        /// <b>NOTE:</b> The details do not include the license key.
        /// </remarks>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzProductDemo_GetLicense.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzProductDemo_GetLicense.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>The JSON document describing the license details.</returns>
        /// 
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetLicense();

        /// <summary>Gets the product version details.</summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzProductDemo_GetVersion.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzProductDemo_GetVersion.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>The JSON document of version details.</returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetVersion();
    }
}