namespace Senzing.Sdk.Core;

using Senzing.Sdk;

public class SzCoreProduct: SzProduct {

    private SzCoreEnvironment env = null;

    public SzCoreProduct(SzCoreEnvironment env) {
        this.env = env;
    }

    public string GetLicense() 
    {
        return "License";
    }

    public string GetVersion() 
    {
        return "version";
    }

    internal void Destroy() {

    }
}