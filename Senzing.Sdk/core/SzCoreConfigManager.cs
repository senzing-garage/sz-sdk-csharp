namespace Senzing.Sdk.Core;

using Senzing.Sdk;

public class SzCoreConfigManager: SzConfigManager {
    private SzCoreEnvironment env = null;

    public SzCoreConfigManager(SzCoreEnvironment env) {
        this.env = env;
    }

    internal void Destroy() {
        
    }

}