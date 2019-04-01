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
                        ConnectionMode = ConnectionMode.Direct
                    };
                    // Set preferred locations
                    connectionPolicy.PreferredLocations.Add(LocationNames.EastUS);
                    connectionPolicy.PreferredLocations.Add(LocationNames.WestUS);
                    client = new DocumentClient(new Uri(EndpointUri), AuthKey, connectionPolicy);
                    // Optiomization: OpenAsync()
                    client.OpenAsync().ConfigureAwait(true).GetAwaiter();
                }
                return client;
            }
        }
    }
}
