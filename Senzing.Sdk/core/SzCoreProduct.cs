
using Senzing.Sdk;

namespace Senzing.Sdk.Core {
/// <summary>
/// The internal implementation of <see cref="Senzing.Sdk.SzProduct"/>
/// that works with the <see cref="SzCoreEnvironment"/>. 
/// </summary>
internal class SzCoreProduct: SzProduct {

    /// <summary>
    /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
    /// </summary>
    private SzCoreEnvironment env = null;

    /// <summary>
    /// Constructs with the specified <see cref="SzCoreEnvironment"/>.
    /// </summary>
    /// 
    /// <param name="env">
    /// The <see cref="SzCoreEnvironment"/> that is constructing this instance
    /// and will be used by this instance.
    /// </param>
    public SzCoreProduct(SzCoreEnvironment env) {
        this.env = env;
    }

    /// <summary>
    /// Implemented to call the native <c>G2Product_license()</c> function.
    /// </summary>
    /// 
    /// <seealso cref="Senzing.Sdk.SzProduct.GetLicense"/>
    public string GetLicense() 
    {
        return "License";
    }

    /// <summary>
    /// Implemented to call the native <c>G2Product_version()</c> function.
    /// </summary>
    /// 
    /// <seealso cref="Senzing.Sdk.SzProduct.GetLicense"/>
    public string GetVersion() 
    {
        return "version";
    }

    internal void Destroy() {

    }
}
}