namespace Senzing.Sdk
{

    /// <summary>
    /// Defines the interface to the Senzing diagnostic functions.
    /// </summary>
    ///
    /// <remarks>
    /// The Senzing diagnostic functions provide diagnostics and
    /// statistics pertaining to the host system and the Senzing repository.
    /// </remarks>
    /// 
    /// <example>
    /// An <c>SzDiagnostic</c> instance is typically obtained from an 
    /// <see cref="SzEnvironment"/> instance via the
    /// <see cref="SzEnvironment.GetDiagnostic"/> method as follows.
    /// 
    /// For example:
    /// <include file="../target/examples/SzDiagnosticDemo_GetDiagnostic.xml" path="/*"/>
    /// </example>
    public interface SzDiagnostic
    {
        /// <summary>
        /// Gathers detailed information on the data store and returns it as a
        /// JSON <c>string</c>.
        /// </summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_GetDatastoreInfo.xml" path="/*"/>
        /// </example>
        ///
        /// <returns>A JSON <c>string</c> describing the datastore.</returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetDatastoreInfo();

        /// <summary>
        /// Runs non-destruction DB performance tests and returns detail of the 
        /// result as a JSON <c>string</c>.
        /// </summary>
        ///
        /// <param name="secondsToRun">
        /// How long to run the database performance test.
        /// </param>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_CheckDatastorePerformance.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The JSON <c>string</c> describing the results of the performance test.
        /// </returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string CheckDatastorePerformance(int secondsToRun);

        /// <summary>
        /// Purges all data in the configured repository.
        /// </summary>
        ///
        /// <remarks>
        /// <b>WARNING:</b> There is no undoing from this.  Make sure your
        /// repository is regularly backed up.
        /// </remarks>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_PurgeRepository.xml" path="/*"/>
        /// </example>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        ///
        /// <seealso href="https://raw.githubusercontent.com/Senzing/code-snippets-v4/refs/heads/main/csharp/snippets/initialization/PurgeRepository/Program.cs">Purge Repository Code Snippet</seealso> 
        void PurgeRepository();

        /// <summary>
        /// Experimental/internal method for obtaining diagnostic feature definition
        /// for the specified feature identifier.
        /// </summary>
        ///
        /// <param name="featureID">The identifier for the feature.</param>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_GetFeature.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The feature definition describing the feature for the specified feature ID.
        /// </returns>
        /// 
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetFeature(long featureID);
    }

}