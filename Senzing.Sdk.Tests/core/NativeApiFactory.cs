using Senzing.Sdk.Tests.Util;
using Senzing.Sdk.Core;
using Senzing.Sdk.Tests.NativeSzApi;

namespace Senzing.Sdk.Tests.Core {
/// <summary>
/// Provides an abstraction for creating instances of the raw Senzing API.
/// </summary>
/// 
/// <remarks>
/// This abstraction allows for alternate implementations to be used especially
/// during auto tests.
/// </remarks>
///
internal class NativeApiFactory {
    /// <summary>
    /// The current <see cref="Senzing.Sdk.Tests.Util.AccessToken"/> required
    /// to authorize uninstalling the <see cref="NativeApiProvider"/>, or
    /// <c>null</c> if no provider is installed.
    /// </summary>
    private static AccessToken? current_token = null;

    /// <summary>
    /// The currently installed <see cref="NativeApiProvider"/>, or
    /// <c>null</c> if no provider is installed.
    /// </summary>
    private static NativeApiProvider? api_provider = null;

    /// <summary>
    /// The <see cref="Senzing.Sdk.Tests.NativeApi.InstallLocations"/>
    /// describing the installation directories.
    /// </summary>
    private static InstallLocations? INSTALL_LOCATIONS = null;

    /// <summary>
    /// Internal object for instance-wide synchronized locking.
    /// </summary>
    private static readonly Object MONITOR = new Object();

    /// <summary>
    /// Gets the install locations.
    /// </summary>
    private static InstallLocations? GetInstallLocations() {
        lock (MONITOR) {
            if (INSTALL_LOCATIONS == null) {
                INSTALL_LOCATIONS = InstallLocations.FindLocations();
            }
            return INSTALL_LOCATIONS;
        }
    }

    /// <summary>
    /// Installs the <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/>
    /// to be used by the factory.
    /// </summary>
    /// 
    /// <remarks>
    /// If none is installed then the default mechanism of constructing new
    /// raw API objects is used.  This returns an
    /// <see cref="Senzing.Sdk.Tests.Util.AccessToken"/> to be used for
    /// uninstalling the provider later.
    /// </remarks>
    ///
    /// <param name="provider">
    /// The non-null <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/>
    /// to install.
    /// </param>
    /// 
    /// <returns>
    /// The <see cref="Senzing.Sdk.Tests.Util.AccessToken"/> to be used
    /// for uninstalling the provider.
    /// </returns>
    /// 
    /// <exception cref="System.ArgumentNullException">
    /// If the specified provider is <c>null</c>.
    /// </exception>
    ///
    /// <exception cref="System.IllegalOperationException">
    /// If a provider is already installed and must first be uninstalled.
    /// </exception>
    public static AccessToken InstallProvider(NativeApiProvider provider) {
        lock(MONITOR) {
            if (provider == null) {
                throw new ArgumentNullException(nameof(provider));
            }
            if (current_token != null) {
                throw new InvalidOperationException(
                    "A provider is already installed and must "
                    + "first be uninstalled.");
            }
            api_provider  = provider;
            current_token = new AccessToken();
            return current_token;
        }
    }

    /// <summary>
    /// Checks if a <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/>
    /// has been installed.
    /// </summary>
    /// 
    /// <returns>
    /// <c>true</c> if a <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/>
    /// has been installed, otherwise <c>false</c>.
    /// </returns>
    public static bool IsProviderInstalled() {
        lock (MONITOR) {
            return (current_token != null);
        }
    }

    /// <summary>
    /// Uninstalls a previously installed
    /// <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/> using the
    /// specified <see cref="Senzing.Sdk.Tests.Util.AccessToken"/> to 
    /// authorize the operation.
    /// </summary>
    /// 
    /// <remarks>
    /// If no provider has been installed then this method does nothing.
    /// </remarks>
    /// 
    /// <param name="token">
    /// The <see cref="Senzing.Sdk.Tests.Util.AccessToken"/> that was
    /// returned when installing the provider.
    /// </param>
    /// 
    /// <exception cref="System.InvalidOperationException">
    /// If the specified token does not match the token that was
    /// returned when the provider was installed.
    /// </exception>
    private static void UninstallProvider(AccessToken token) {
        lock (MONITOR) {
            if (current_token == null) {
                return;
            }
            if (!current_token.Equals(token)) {
                throw new InvalidOperationException(
                    "The specified access token is not the expected access "
                    + "token to authorize unintalling the provider.");
            }
            current_token = null;
            api_provider  = null;
        }
    }

    /// <summary>
    /// Internal method for getting the currently installed provider in a
    /// thread-safe manner.
    /// </summary>
    /// 
    /// <returns>
    /// The currently installed provider, or <c>null</c> if no provider
    /// is currently installed.
    /// </returns>
    private static NativeApiProvider? GetInstalledProvider() {
        return api_provider;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Senzing.Sdk.Core.NativeEngine"/>
    /// to use.
    /// </summary>
    /// 
    /// <remarks>
    /// If a <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/> is
    /// installed then it is used to create the instance, otherwise a
    /// new instance of <see cref="Senzing.Sdk.Tests.Core.NativeEngineExtern"/>
    /// is constructed and returned.
    /// </remarks>
    ///
    /// <returns>
    /// A new instance of <see cref="Senzing.Sdk.Core.NativeEngine"/> to use.
    /// </returns>
    public static NativeEngine CreateEngineApi() {
        NativeApiProvider? provider = GetInstalledProvider();
        if (provider != null) {
            return provider.CreateEngineApi();

        } else if (GetInstallLocations() == null) {
            throw new InvalidInstallationException(
                "Unable to find Senzing native installation.");

        } else {
            return new NativeEngineExtern();
        }
    }

    /// <summary>
    /// Provides a new instance of <see cref="Senzing.Sdk.Core.NativeConfig"/> to use.
    /// </summary>
    /// 
    /// <remarks>
    /// If a <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/>
    /// is installed then it is used to create the instance, otherwise a new instance of {@link NativeConfigJni} is
    /// constructed and returned.
    ///
    /// @return A new instance of <see cref="Senzing.Sdk.Core.NativeConfig"/> to use.
    ///
    public static NativeConfig CreateConfigApi() {
        NativeApiProvider? provider = GetInstalledProvider();
        if (provider != null) {
            return provider.CreateConfigApi();

        } else if (GetInstallLocations() == null) {
            throw new InvalidInstallationException(
                "Unable to find Senzing native installation.");

        } else {
            return new NativeConfigExtern();
        }
    }

    /// <summary>
    /// Provides a new instance of <see cref="Senzing.Sdk.Core.NativeProduct"/> to use.  If a
    /// <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/> is installed then it is used to create
    /// the instance, otherwise a new instance of {@link NativeProductJni} is
    /// constructed and returned.
    ///
    /// @return A new instance of <see cref="Senzing.Sdk.Core.NativeProduct"/> to use.
    ///
    public static NativeProduct CreateProductApi() {
        NativeApiProvider? provider = GetInstalledProvider();
        if (provider != null) {
            return provider.CreateProductApi();

        } else if (GetInstallLocations() == null) {
            throw new InvalidInstallationException(
                "Unable to find Senzing native installation.");

        } else {
            return new NativeProductExtern();
        }
    }

    /// <summary>
    /// Provides a new instance of <see cref="Senzing.Sdk.Core.NativeConfigManager"/>
    /// to use.
    /// </summary>
    /// 
    /// <remarks>
    /// If a <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/> is installed
    /// then it is used to create the instance, otherwise a new instance of
    /// <see cref="Senzing.Sdk.Core.NativeConfigManagerExtern"/> is constructed
    /// and returned.
    /// </remarks>
    /// 
    /// <returns>
    /// A new instance of <see cref="Senzing.Sdk.Core.NativeConfigManager"/>
    /// to use.
    /// </returns>
    public static NativeConfigManager CreateConfigMgrApi() {
        NativeApiProvider? provider = GetInstalledProvider();
        if (provider != null) {
            return provider.CreateConfigMgrApi();

        } else if (GetInstallLocations() == null) {
            throw new InvalidInstallationException(
                "Unable to find Senzing native installation.");

        } else {
            return new NativeConfigManagerExtern();
        }
    }

    ///
    /// Provides a new instance of <see cref="Senzing.Sdk.Core.NativeDiagnostic"/> to use.  If a
    /// <see cref="Senzing.Sdk.Tests.Core.NativeApiProvider"/> is installed then it is used to create
    /// the instance, otherwise a new instance of {@link NativeDiagnosticJni} is
    /// constructed and returned.
    ///
    /// @return A new instance of <see cref="Senzing.Sdk.Core.NativeDiagnostic"/> to use.
    ///
    ///
    public static NativeDiagnostic CreateDiagnosticApi() {
        NativeApiProvider? provider = GetInstalledProvider();
        if (provider != null) {
            return provider.CreateDiagnosticApi();

        } else if (GetInstallLocations() == null) {
            throw new InvalidInstallationException(
                "Unable to find Senzing native installation.");

        } else {
            return new NativeDiagnosticExtern();
        }
    }
}
}