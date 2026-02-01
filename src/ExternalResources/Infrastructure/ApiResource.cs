namespace AuroraScienceHub.SWeather.PreliminaryDataImport.ExternalResources.Infrastructure;

/// <summary>
/// Relative paths to target data
/// </summary>
public static class ApiResource
{
    /// <summary>
    /// Folder with php scripts for Pc index data
    /// </summary>
    public static class PcIndex
    {
        /// <summary>
        /// Base path to php scripts
        /// </summary>
        public const string BasePath = "ovation_test/";

        /// <summary>
        /// Relative path to PcN data
        /// </summary>
        public const string PcN = "make_json_pc.php?ns=pcn";

        /// <summary>
        /// Relative path to PcS data
        /// </summary>
        public const string PcS = "make_json_pc.php?ns=pcs";
    }
}
