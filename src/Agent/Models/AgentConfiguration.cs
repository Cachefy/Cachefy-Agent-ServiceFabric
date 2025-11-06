namespace Agent.Models
{
    public class AgentConfiguration
    {
        /// <summary>
        /// The base URL for the cache server endpoint.
        /// Example: "https://cache-server.example.com" or "https://localhost:5001"
        /// </summary>
        public string CacheServerUrl { get; set; } = string.Empty;

        /// <summary>
        /// API key for authenticating with external services.
        /// This value should be kept secure and not logged.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}