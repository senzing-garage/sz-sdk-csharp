using Senzing.Sdk;

namespace Senzing.Sdk.Core {
public class SzCoreEngine: SzEngine {
    private SzCoreEnvironment env = null;

    public SzCoreEngine(SzCoreEnvironment env) {
        this.env = env;
    }
    
    internal void Destroy() {
        
    }

}
}