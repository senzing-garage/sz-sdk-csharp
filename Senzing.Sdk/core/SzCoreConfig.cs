namespace Senzing.Sdk.Core;

using Senzing.Sdk;

public class SzCoreConfig: SzConfig {
    private SzCoreEnvironment env = null;

    public SzCoreConfig(SzCoreEnvironment env) {
        this.env = env;
    }

    internal void Destroy() {
        
    }

}