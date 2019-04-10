# Contoso CRM

A sample Asp.Net Core Web App using CosmosDB. It emmulates a simple CRM program where the user is able to list, create, edit and delete leads, contacts, and customers.

## Why CosmosDB

- CosmosDB is the first globally distributed database that offers SLA on availability, throughput and latency. 
- If configured with geo-replication, CosmosDB can offer up to 99.999% availability. Furthermore, CosmosDB allows you to work with a number of APIs including SQL (formerly DocumentDB), MongoDB, Cassandra and Gremlin.
- CosmosDB can be configured with different consistency levels which are suitable for a number of scenarios.
- CosmosDB can be used both in hot storage and cold storage scenarios.
- CosmosDB servers most requests in under 10ms. It is so fast it can be used on globally distributted caching.

## Contact Model

```
public class Contact
    {
        // This id is automatically created by cosmosdb if it is not set
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "contactType")]
        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public ContactType ContactType { get; set; }

        [JsonProperty(PropertyName = "company")]
        [Required]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "email")]
        [Required]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        [Required]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
    }
```

## Partition Key

The partition key for this model is: 

**/contactType**

## Indexing

By default, CosmosDB indexes every attribute in the JSON document is indexed. This can have an impact on both performace and RU consumption. 

In the ContosoCRM application, it is not expected that users will search the notes attribute of the document, therefore it has been disabled.

## Performance Optiomizations

The application is based on the following performance optiomizations:

https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips

# Code Optimizations:

```
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
                    client.OpenAsync().Wait();
                }
                return client;
            }
```

  - Direct Mode vs Gateway Mode
  - Preferred Reading Locations
  - Singleton client instance
  - OpenAsync()
  - For local development, consider using the emulator
    - https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator

# Additional Code Concepts

```
while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<T>();

                foreach (var key in result.ResponseHeaders.AllKeys)
                {
                    Debug.WriteLine($"Key: {key} Value: {result.ResponseHeaders[key]}");
                }

                // Information: Calcualte total RUs
                totalRUs += result.RequestCharge;
                results.AddRange(result);
            }

            return new Tuple<double, string, string, IEnumerable<T>>(totalRUs, 
                DocumentDbClientInstance.Client.ReadEndpoint.ToString(), 
                DocumentDbClientInstance.Client.WriteEndpoint.ToString(),
                results);
```

  - Detect and log the RUs
  - Detect and log the read and write regions


# Important Cosmos DB Concepts

## Introduction And SLAs

Azure Cosmos DB is Microsoft’s globally distributed multi-model database service. It offers turnkey global distribution across any number of Azure regions by transparently scaling and replicating your data wherever your users are. The service offers comprehensive **99.99% SLAs which covers the guarantees for throughput, consistency, availability and latency** for the Cosmos DB Database Accounts scoped to a single Azure region configured with any of the five Consistency Levels or Database Accounts spanning multiple Azure regions, configured with any of the four relaxed Consistency Levels. Azure Cosmos DB allows configuring multiple Azure regions as writable endpoints for a Database Account. In this configuration, Cosmos DB **offers 99.999% SLA** for both read and write availability. 

### More About SLAs

https://azure.microsoft.com/en-us/support/legal/sla/cosmos-db/v1_2/

#### Compound Availability

CosmosDB may help in improving compound availability where system A depends on system B. Suppose systems A and B 
have a 99.95% availability. The total compoind availability would be:

```
A * B = 99.95% * 99.95% = 99.90%

Note: That could mean up to 500 minutes of downtime a year.
```

But if system B was CosmosDB at 99.999% then the total compound availability would be:

```
99.95% * 99.999% = 99.949%

Note: That could mean up to 250 minutes of downtime a year.
```

## Preferred Locations and EnableEndpointDiscovery

```
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
                    client.OpenAsync().Wait();
                }
                return client;
            }
        }
```

When EnableEndpointDiscovery is true and the value of this property is non-empty, the SDK uses the locations in the collection in the order they are specified to perform operations, otherwise if the value of this property is not specified, the SDK uses the write region as the preferred location for all operations.

If EnableEndpointDiscovery is set to false, the value of this property is ignored.

## RUs

Azure Cosmos DB is offered in units of solid-state drive (SSD) backed storage and throughput. Request units measure Azure Cosmos DB throughput per second, and request unit consumption varies by operation and JSON document. Use this calculator to determine the number of request units per second (RU/s) and the amount of data storage needed by your application.

### RU Calculator

https://www.documentdb.com/capacityplanner

## Consistency Levels

Distributed databases that rely on replication for high availability, low latency, or both, make the fundamental tradeoff between the read consistency vs. availability, latency, and throughput. Most commercially available distributed databases ask developers to choose between the two extreme consistency models: strong consistency and eventual consistency. The  linearizability or the strong consistency model is the gold standard of data programmability. But it adds a price of higher latency (in steady state) and reduced availability (during failures). On the other hand, eventual consistency offers higher availability and better performance, but makes it hard to program applications.

Azure Cosmos DB approaches data consistency as a spectrum of choices instead of two extremes. Strong consistency and eventual consistency are at the ends of the spectrum, but there are many consistency choices along the spectrum. Developers can use these options to make precise choices and granular tradeoffs with respect to high availability and performance.

With Azure Cosmos DB, developers can choose from five well-defined consistency models on the consistency spectrum. From strongest to more relaxed, the models include strong, bounded staleness, session, consistent prefix, and eventual consistency. The models are well-defined and intuitive and can be used for specific real-world scenarios.

### More About Consistency Levels

https://docs.microsoft.com/en-us/azure/cosmos-db/consistency-levels

## Partitions

Azure Cosmos DB uses partitioning to scale individual containers in a database to meet the performance needs of your application. In partitioning, the items in a container are divided into distinct subsets called logical partitions. Logical partitions are formed based on the value of a partition key that is associated with each item in a container. All items in a logical partition have the same partition key value.

For example, a container holds items. Each item has a unique value for the UserID property. If UserID serves as the partition key for the items in the container and there are 1,000 unique UserID values, 1,000 logical partitions are created for the container.

In addition to a partition key that determines the item’s logical partition, each item in a container has an item ID (unique within a logical partition). Combining the partition key and the item ID creates the item's index, which uniquely identifies the item.

Choosing a partition key is an important decision that will affect your application’s performance.

### More About Paritions

https://docs.microsoft.com/en-us/azure/cosmos-db/partitioning-overview
