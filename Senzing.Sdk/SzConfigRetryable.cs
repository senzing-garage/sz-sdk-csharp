namespace Senzing.Sdk
{
    /// <summary>
    /// Used to annotate which Senzing SDK methods are dependent upon the
    /// active configuration being current.  
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Methods annotated with this annotation are those methods
    /// that may experience a failure if the <see 
    /// cref="SzEnvironment.GetActiveConfigID">active configuration</see>
    /// is <b>not</b> the most recent configuration for the repository.
    /// Such failures can occur when retrieved data references a
    /// configuration element (e.g.: data source, feature type or match type)
    /// that is not found in the <see cref="SzEnvironment.GetActiveConfigID"
    /// >active configuration</see>.
    /// </para>
    /// 
    /// <para>
    /// There are various ways this can happen, but typically it means that
    /// data has been loaded through another <see cref="SzEnvironment"/>
    /// using a newer configuration (usually in another system process) and
    /// then that data is retrieved by an <see cref="SzEnvironment"/> that
    /// was initialized prior to that configuration being <see 
    /// cref="SzConfigManager.RegisterConfig(string, string)">registered in
    /// the repository</see> and being set as the <see 
    /// cref="SzConfigManager.SetDefaultConfigID">default configuration</see>.
    /// </para>
    ///
    /// <para>
    /// Such failures can usually be resolved by retrying after checking if the 
    /// <see cref="SzEnvironment.GetActiveConfigID">active configuration ID</see>
    /// differs from the current <see cref="SzConfigManager.GetDefaultConfigID"
    /// >default configuration ID</see>, and if so <see 
    /// cref="SzEnvironment.Reinitialize">reinitializing</see> using the current
    /// <see cref="SzConfigManager.GetDefaultConfigID">default configuration ID</see>
    /// </para>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class SzConfigRetryable : System.Attribute
    {
        public SzConfigRetryable()
        {

        }
    }
}