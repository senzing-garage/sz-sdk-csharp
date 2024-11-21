using Senzing.Sdk.Core;

namespace Senzing.Sdk.Tests.Core
{
    internal interface NativeApiProvider
    {
        /// <summary>
        /// Provides a new instance of
        /// <see cref="Senzing.Sdk.Core.NativeEngine"/> to use.
        /// </summary>
        ///
        /// <returns>
        /// A new instance of
        /// <see cref="Senzing.Sdk.Core.NativeEngine"/> to use.
        /// </returns>
        NativeEngine CreateEngineApi();

        /// <summary>
        /// Provides a new instance of
        /// <see cref="Senzing.Sdk.Core.NativeConfig"/> to use.
        /// </summary>
        ///
        /// <returns>
        /// A new instance of
        /// <see cref="Senzing.Sdk.Core.NativeConfig"/> to use.
        /// </returns>
        NativeConfig CreateConfigApi();

        /// <summary>
        /// Provides a new instance of
        /// <see cref="Senzing.Sdk.Core.NativeProduct"/> to use.
        /// </summary>
        ///
        /// <returns>
        /// A new instance of
        /// <see cref="Senzing.Sdk.Core.NativeProduct"/> to use.
        /// </returns>
        NativeProduct CreateProductApi();

        /// <summary>
        /// Provides a new instance of
        /// <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to use.
        /// </summary>
        ///
        /// <returns>
        /// A new instance of
        /// <see cref="Senzing.Sdk.Core.NativeConfigManager"/> to use.
        /// </returns>
        NativeConfigManager CreateConfigMgrApi();

        /// <summary>
        /// Provides a new instance of
        /// <see cref="Senzing.Sdk.Core.NativeDiagnostic"/> to use.
        /// </summary>
        ///
        /// <returns>
        /// A new instance of
        /// <see cref="Senzing.Sdk.Core.NativeDiagnostic"/> to use.
        /// </returns>
        NativeDiagnostic CreateDiagnosticApi();
    }
}