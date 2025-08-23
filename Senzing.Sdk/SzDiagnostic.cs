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
        /// Returns overview information about the repository.
        /// </summary>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_GetRepositoryInfo.xml" path="/*"/>
        /// </example>
        ///
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzDiagnosticDemo_GetRepositoryInfo.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>A JSON <c>string</c> describing the datastore.</returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetRepositoryInfo();

        /// <summary>
        /// Conducts a rudimentary repository test to gauge I/O and network performance.
        /// </summary>
        ///
        /// <remarks>
        /// Typically, this is only run when troubleshooting performance.  This is a
        /// non-destructive test.
        /// </remarks>
        /// 
        /// <param name="secondsToRun">
        /// How long to run the database performance test.
        /// </param>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_CheckRepositoryPerformance.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzDiagnosticDemo_CheckRepositoryPerformance.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The JSON <c>string</c> describing the results of the performance test.
        /// </returns>
        ///
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string CheckRepositoryPerformance(int secondsToRun);

        /// <summary>
        /// Permanently deletes all data in the repository, except the configuration.
        /// </summary>
        ///
        /// <remarks>
        /// <b>WARNING:</b> This method is destructive, it will delete all loaded 
        /// records and entity resolution decisions.  Senzing does not provide a 
        /// means to the restore the data.  The only means of recovery would be
        /// restoring from a database backup.
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
        /// Experimental/internal for Senzing support use.
        /// </summary>
        ///
        /// <param name="featureID">The identifier for the feature.</param>
        ///
        /// <example>
        /// Usage:
        /// <include file="../target/examples/SzDiagnosticDemo_GetFeature.xml" path="/*"/>
        /// </example>
        /// 
        /// <example>
        /// <b>Example Result:</b> (formatted for readability)
        /// <include file="../target/examples/results/SzDiagnosticDemo_GetFeature.xml" path="/*"/>
        /// </example>
        /// 
        /// <returns>
        /// The feature definition describing the feature for the specified feature ID.
        /// </returns>
        /// 
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        [SzConfigRetryable]
        string GetFeature(long featureID);
    }

}