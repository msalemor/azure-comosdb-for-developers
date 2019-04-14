namespace ContosoCrm.DataAccess21.Factories
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;

    public static class DocumentClientFactory
    {
        private static DocumentClient client;
        public static string EndpointUri;
        public static string AuthKey;
        public static string PreferredLocations;
        public static string CONNECTION_MODE;
        public static string PROTOCOL;

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
                        ConnectionProtocol = Protocol.Tcp,
                        EnableEndpointDiscovery = true,
                        UseMultipleWriteLocations = true,
                        RetryOptions = new RetryOptions
                        {
                            // Default
                            MaxRetryAttemptsOnThrottledRequests = 9,
                            // Default
                            MaxRetryWaitTimeInSeconds = 30,
                        }
                    };
                    connectionPolicy.SetCurrentLocation(PreferredLocations);
                    //Set preferred locations
                    //if (!string.IsNullOrEmpty(PreferredLocations))
                    //{
                    //    foreach(var location in PreferredLocations.Split(','))
                    //    {
                    //        connectionPolicy.PreferredLocations.Add(location);
                    //    }
                    //}
                    client = new DocumentClient(new Uri(EndpointUri), AuthKey, connectionPolicy);
                    // Optiomization: OpenAsync()
                    client.OpenAsync().Wait();
                }
                return client;
            }
        }
    }
}
