namespace Senzing.Sdk.Core;

using Senzing.Sdk;

public class SzCoreDiagnostic: SzDiagnostic {
    private SzCoreEnvironment env = null;

    public SzCoreDiagnostic(SzCoreEnvironment env) {
        this.env = env;
    }
    
    internal void Destroy() {
        
    }

}