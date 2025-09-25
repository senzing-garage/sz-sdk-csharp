namespace Senzing.Sdk
{

    /// <summary>
    /// Provides a factory interface for obtaining the references to the Senzing SDK 
    /// singleton instances that have been initialized.
    /// </summary>
    /// 
    /// <example>
    /// Usage:
    /// <include file="../target/examples/SzProductDemo_SzEnvironment.xml" path="/*"/>
    /// </example>
    /// 
    public interface SzEnvironment
    {
        /// <summary>
        /// Provides a reference to the <see cref="SzProduct" /> singleton associated
        /// with this <c>SzEnvironment</c>.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzProductDemo_GetProduct.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The <see cref="SzProduct" /> instance associated with this <c>SzEnvironment</c>
        /// </returns>
        /// 
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been
        /// <see cref="Destroy">destroyed</see>.
        /// </exception>
        /// 
        /// <exception cref="SzException"> 
        /// If there was a failure in obtaining or initializing the
        /// <see cref="SzProduct" /> instance. 
        /// </exception>
        SzProduct GetProduct();

        /// <summary>
        /// Provides a reference to the <see cref="SzEngine"/> instance associated
        /// with this <c>SzEnvironment</c>.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzEngineDemo_GetEngine.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The <see cref="SzEngine"/> instance associated with
        /// this <c>SzEnvironment</c>.
        /// </returns>
        ///
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been destroyed.
        /// </exception>
        /// 
        /// <exception cref="SzException">
        /// If there was a failure in obtaining or initializing the
        /// <see cref="SzEngine"/> instance.
        /// </exception>
        SzEngine GetEngine();

        /// <summary>
        /// Provides a reference to the <see cref="SzConfigManager"/> instance
        /// associated with this <c>SzEnvironment</c>.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzConfigManagerDemo_GetConfigManager.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The <see cref="SzConfigManager"/> instance associated with
        /// this <c>SzEnvironment</c>.
        /// </returns>
        ///
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been destroyed.
        /// </exception>
        /// 
        /// <exception cref="SzException">
        /// If there was a failure in obtaining or initializing the
        /// <see cref="SzConfigManager"/> instance.
        /// </exception>
        SzConfigManager GetConfigManager();

        /// <summary>
        /// Provides a reference to the <see cref="SzDiagnostic"/> instance
        /// associated with this <c>SzEnvironment</c>.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_GetDiagnostic.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// The <see cref="SzDiagnostic"/> instance associated with
        /// this <c>SzEnvironment</c>.
        /// </returns>
        ///
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been destroyed.
        /// </exception>
        /// 
        /// <exception cref="SzException">
        /// If there was a failure in obtaining or initializing the
        /// <see cref="SzDiagnostic"/> instance.
        /// </exception>
        SzDiagnostic GetDiagnostic();

        /// <summary> 
        /// Gets the currently active configuration ID for this <c>SzEnvironment</c>.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzConfigManagerDemo_GetActiveConfigID.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>The currently active configuration ID.</returns>
        ///
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been destroyed.
        /// </exception>
        /// 
        /// <exception cref="SzException">
        /// If there was a failure in obtaining the active config ID.
        /// </exception>
        long GetActiveConfigID();

        /// <summary>
        /// Reinitializes the <c>SzEnvironment</c> with the specified
        /// configuration ID.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzConfigManagerDemo_GetActiveConfigID.xml" path="/*"/>
        /// </example>
        ///
        /// <param name="configID">
        /// The configuration ID with which to initialize.
        /// </param>
        /// 
        /// <exception cref="SzEnvironmentDestroyedException">
        /// If this <c>SzEnvironment</c> instance has been destroyed.
        /// </exception>
        /// 
        /// <exception cref="SzException">
        /// If there was a failure reinitializing.
        /// </exception>
        void Reinitialize(long configID);

        /// <summary>
        /// Destroys this <c>SzEnvironment</c> and invalidates any SDK singleton
        /// references that has previously provided.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzProductDemo_DestroyEnvironment.xml" path="/*"/>
        /// </example>
        /// 
        /// <remarks>
        /// If this instance has already been destroyed then this method has no effect.
        /// </remarks>
        void Destroy();

        /// <summary>
        /// Checks if this instance has had its <see cref="Destroy" /> method called.
        /// </summary>
        /// 
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzProductDemo_DestroyEnvironment.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>
        /// <c>true</c> if this instance has had its <see cref="Destroy"/>  
        /// method called, otherwise <c>false</c>.
        /// </returns>
        bool IsDestroyed();
    }
}