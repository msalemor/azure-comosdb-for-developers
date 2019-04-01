namespace ContosoCrm.DataAccess.Helpers
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;

    public static class DocumentDbClientInstance
    {
        private static DocumentClient client;
        public static string EndpointUri;
        public static string AuthKey;
        public static string PreferredLocations;

        // Optimization: reuse the client instance for the life of the application
        public static DocumentClient Client
        {
            get
            {
                if (client is null)
                {
                    var connectionPolicy = new ConnectionPolicy
                    {
                        // Optionmizations: Use Directing Mode
                        // Gateway mode adds more compatibility but adds and extra hop
                        ConnectionMode = ConnectionMode.Direct,
                        EnableEndpointDiscovery = true
                    };
                    // Set preferred locations
                    if (!string.IsNullOrEmpty(PreferredLocations))
                    {
                        foreach(var location in PreferredLocations.Split(','))
                        {
                            connectionPolicy.PreferredLocations.Add(location);
                        }
                    }
                    client = new DocumentClient(new Uri(EndpointUri), AuthKey, connectionPolicy);
                    // Optiomization: OpenAsync()
                    client.OpenAsync().ConfigureAwait(true).GetAwaiter();
                }
                return client;
            }
        }
    }
}
