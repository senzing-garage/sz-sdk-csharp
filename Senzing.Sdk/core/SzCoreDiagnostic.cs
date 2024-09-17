
using Senzing.Sdk;

namespace Senzing.Sdk.Core {
public class SzCoreDiagnostic: SzDiagnostic {
    private SzCoreEnvironment env = null;

    public SzCoreDiagnostic(SzCoreEnvironment env) {
        this.env = env;
    }
    
    internal void Destroy() {
        
    }

}
}