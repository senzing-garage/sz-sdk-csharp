using Senzing.Sdk;

namespace Senzing.Sdk.Core {
public class SzCoreConfigManager: SzConfigManager {
    private SzCoreEnvironment env = null;

    public SzCoreConfigManager(SzCoreEnvironment env) {
        this.env = env;
    }

    internal void Destroy() {
        
    }

}
}