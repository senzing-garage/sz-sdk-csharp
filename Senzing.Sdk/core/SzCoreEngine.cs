namespace Senzing.Sdk.Core;

using Senzing.Sdk;

public class SzCoreEngine: SzEngine {
    private SzCoreEnvironment env = null;

    public SzCoreEngine(SzCoreEnvironment env) {
        this.env = env;
    }
    
    internal void Destroy() {
        
    }

}