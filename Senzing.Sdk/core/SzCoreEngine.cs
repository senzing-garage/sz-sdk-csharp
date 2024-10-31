using Senzing.Sdk;

namespace Senzing.Sdk.Core {
public class SzCoreEngine: SzEngine {

    /// <summary>
    /// THe <see cref="SzCoreEnvironment"/> that constructed this instance.
    /// </summary>
    private SzCoreEnvironment env = null;

    /// <summary>
    /// The underlying <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>.
    /// </summary>
    private NativeEngineExtern nativeApi = null;

    /// <summary>
    /// Internal object for instance-wide synchronized locking.
    /// </summary>
    private readonly object monitor = new object();

    /// <summary>
    /// Constructs with the specified <see cref="SzCoreEnvironment"/>.
    /// </summary>
    /// 
    /// <param name="env">
    /// The <see cref="SzCoreEnvironment"/> that is constructing this instance
    /// and will be used by this instance.
    /// </param>
    public SzCoreEngine(SzCoreEnvironment env) {
        this.env = env;
        this.env.Execute<object>(() => {
            // construct the native delegate
            this.nativeApi = new NativeEngineExtern();

            // initialize the native delegate
            long returnCode = this.nativeApi.Init(this.env.GetInstanceName(),
                                                  this.env.GetSettings(),
                                                  this.env.IsVerboseLogging());
            
            // handle the return code
            this.env.HandleReturnCode(returnCode, this.nativeApi);

            // no return value so return null
            return null;
        });
    }

    /// <summary>
    /// Gets the associated <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>
    /// instance.
    /// </summary>
    ///
    /// <returns>
    /// The associated <see cref="Senzing.Sdk.Core.NativeEngineExtern"/>
    /// instance.
    /// </returns>
    internal NativeEngineExtern GetNativeApi() {
        return this.nativeApi;
    }

    /// <summary>
    /// The package-protected function to destroy the Senzing Engine SDK.
    /// </summary>
    internal void Destroy() {
        lock (this.monitor) {
            if (this.nativeApi == null) {
                return;
            }
            this.nativeApi.Destroy();
            this.nativeApi = null;
        }
    }

    /// <summary>
    /// Checks if this instance has been destroyed by the associated
    /// <see cref="Senzing.Sdk.Core.SzCoreEnvironment"/>.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> if this instance has been destroyed, otherwise <c>false</c>.
    /// </returns>
    internal bool IsDestroyed() {
        lock (this.monitor) {
            return (this.nativeApi == null);
        }
    }

}
}