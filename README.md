# Contoso CRM
A sample Asp.Net Core Web App using CosmosDB. It emmulates a simple CRM program where contacts, of different types can be viewed, created, edited and deleted.

It is based on the following performance optiomizations:

https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips

# Code Optimizations:

  - Direct Mode vs Gateway Mode
  - Preferred Reading Locations
  - Singleton client instance
  - OpenAsync()

# Addition Code Concepts
  - Detect and log the RUs
  - Detect and log the reading region location


# Important Cosmos DB Concepts

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