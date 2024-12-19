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
    public interface SzDiagnostic
    {
        /// <summary>
        /// Gathers detailed information on the data store and returns it as a
        /// JSON <c>string</c>.
        /// </summary>
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
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        void PurgeRepository();

        /// <summary>
        /// Experimental/internal method for obtaining diagnostic feature definition
        /// for the specified feature identifier.
        /// </summary>
        ///
        /// <param name="featureID">The identifier for the feature.</param>
        /// 
        /// <returns>
        /// The feature definition describing the feature for the specified feature ID.
        /// </returns>
        /// 
        /// <exception cref="SzException">Thrown if a failure occurs.</exception>
        string GetFeature(long featureID);
    }

}