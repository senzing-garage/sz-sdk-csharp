using Senzing.Sdk;

namespace Senzing.Sdk.Core {
public class SzCoreConfig: SzConfig {
    private SzCoreEnvironment env = null;

    public SzCoreConfig(SzCoreEnvironment env) {
        this.env = env;
    }

    internal void Destroy() {
        
    }
}
}