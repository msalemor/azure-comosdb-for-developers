namespace ContosoCrm.DataAccess.Helpers
{
    using ContosoCrm.DataAccess.Interfaces;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class DocumentDbHelper<T> : IDocumentDbHelper<T> where T : class
    {
        private string databaseId;
        private string collectionId;

        public virtual async Task<Tuple<double, T>> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                var result = await DocumentDbClientInstance.Client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionId, id),
                    new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                var tuple = new Tuple<double, T>(result.RequestCharge, (T)(dynamic)result.Resource);
                //return (T)(dynamic)document;
                return tuple;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public virtual async Task<Tuple<double, string, string, string, IEnumerable<T>>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector = null, string partitionKey = null)
        {
            double totalRUs = 0;
            IDocumentQuery<T> query;
            FeedOptions options = new FeedOptions { MaxItemCount = -1 };

            if (!string.IsNullOrEmpty(partitionKey))
            {
                options.PartitionKey = new PartitionKey(partitionKey);
                options.EnableCrossPartitionQuery = false;
            }
            else
            {
                options.EnableCrossPartitionQuery = true;
            }

            if (selector is null)
            {
                query = DocumentDbClientInstance.Client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    options)
                    .Where(predicate)
                    .AsDocumentQuery();
            }
            else
            {
                query = DocumentDbClientInstance.Client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    options)
                    .Where(predicate)
                    .Select(selector)
                    .AsDocumentQuery();
            }

            List<T> results = new List<T>();
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

            return new Tuple<double, string, string, string, IEnumerable<T>>(totalRUs, 
                DocumentDbClientInstance.Client.ReadEndpoint.ToString(), 
                DocumentDbClientInstance.Client.WriteEndpoint.ToString(),
                DocumentDbClientInstance.Client.ConsistencyLevel.ToString(),
                results);
        }

        public virtual async Task<Document> CreateItemAsync(T item)
        {
            return await DocumentDbClientInstance.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), item);
        }

        public virtual async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await DocumentDbClientInstance.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id), item);
        }

        public virtual async Task DeleteItemAsync(string id, string partionKey)
        {
            var options = new RequestOptions { PartitionKey = new PartitionKey(partionKey) };
            await DocumentDbClientInstance.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id), options);
        }

        public virtual void Initialize(string dbId, string colId)
        {
            databaseId = dbId;
            collectionId = colId;

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await DocumentDbClientInstance.Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var options = new RequestOptions
                    {
                        ConsistencyLevel = ConsistencyLevel.Session
                    };
                    await DocumentDbClientInstance.Client.CreateDatabaseAsync(new Database { Id = databaseId }, options);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await DocumentDbClientInstance.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await DocumentDbClientInstance.Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
