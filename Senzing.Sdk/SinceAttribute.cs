namespace Senzing.Sdk
{
    /// <summary>
    /// Indicates the Senzing SDK version in which the annotated element was
    /// first introduced.  This is used to identify flags and constants that
    /// may not be present in the installed Senzing runtime if the installed
    /// version predates the annotated version.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal class SinceAttribute : System.Attribute
    {
        internal readonly string version;
        public SinceAttribute(string version)
        {
            this.version = version;
        }
    }
}
