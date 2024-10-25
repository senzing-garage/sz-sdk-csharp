using Senzing.Sdk.Core;

namespace Senzing.Sdk.Tests.Core
{
internal interface NativeApiProvider
{
    /**
        * Provides a new instance of {@link NativeEngine} to use.
        *
        * @return A new instance of {@link NativeEngine} to use.
        */
    NativeEngine CreateEngineApi();

    /**
        * Provides a new instance of {@link NativeConfig} to use.
        *
        * @return A new instance of {@link NativeConfig} to use.
        */
    NativeConfig CreateConfigApi();

    /**
        * Provides a new instance of {@link NativeProduct} to use.
        *
        * @return A new instance of {@link NativeProduct} to use.
        */
    NativeProduct CreateProductApi();

    /**
        * Provides a new instance of {@link NativeConfigManager} to use.
        *
        * @return A new instance of {@link NativeConfigManager} to use.
        *
        */
    NativeConfigManager CreateConfigMgrApi();

    /**
        * Provides a new instance of {@link NativeDiagnostic} to use.
        *
        * @return A new instance of {@link NativeDiagnostic} to use.
        */
    NativeDiagnostic CreateDiagnosticApi();
}
}