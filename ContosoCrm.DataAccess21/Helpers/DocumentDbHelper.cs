namespace ContosoCrm.DataAccess21.Helpers
{
    using ContosoCrm.DataAccess21.Interfaces;
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
        private string SelfLink;

        #region Crud Operations

        public virtual async Task<Tuple<double, T>> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                var result = await DocumentDbClientInstance.Client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionId, id),
                    new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                var response = (T)(dynamic)result.Resource;
                var tuple = new Tuple<double, T>(result.RequestCharge, response);
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

        #endregion

        #region Management Operations

        public virtual void Initialize(string dbId, string colId, int offerThroughput = 1000, ConsistencyLevel consistencyLevel = ConsistencyLevel.Session, string partitionKey = null)
        {
            databaseId = dbId;
            collectionId = colId;

            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync(offerThroughput, consistencyLevel, partitionKey).Wait();
        }

        public async Task<Offer> UpdateOfferForCollectionAsync(string collectionSelfLink, int newOfferThroughput)
        {
            // Create an asynchronous query to retrieve the current offer for the document collection
            // Notice that the current version of the API only allows to use the SelfLink for the collection
            // to retrieve its associated offer
            Offer existingOffer = null;
            var offerQuery = DocumentDbClientInstance.Client.CreateOfferQuery()
                .Where(o => o.ResourceLink == collectionSelfLink)
                .AsDocumentQuery();
            while (offerQuery.HasMoreResults)
            {
                foreach (var offer in await offerQuery.ExecuteNextAsync<Offer>())
                {
                    existingOffer = offer;
                }
            }
            if (existingOffer == null)
            {
                throw new Exception("I couldn't retrieve the offer for the collection.");
            }
            // Set the desired throughput to newOfferThroughput RU/s for the new offer built based on the current offer
            var newOffer = new OfferV2(existingOffer, newOfferThroughput);
            var replaceOfferResponse = await DocumentDbClientInstance.Client.ReplaceOfferAsync(newOffer);
            return replaceOfferResponse.Resource;
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

        private async Task CreateCollectionIfNotExistsAsync(int offerThroughput = 1000, ConsistencyLevel consistencyLevel = ConsistencyLevel.Session, string partitionKey = null)
        {
            try
            {
                await DocumentDbClientInstance.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                DocumentCollection documentCollection = new DocumentCollection
                {
                    Id = collectionId
                };

                var options = new RequestOptions
                {
                    OfferThroughput = offerThroughput,
                    ConsistencyLevel = consistencyLevel
                };

                if (!string.IsNullOrEmpty(partitionKey))
                {
                    documentCollection.PartitionKey.Paths.Add(partitionKey);
                }

                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await DocumentDbClientInstance.Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        documentCollection,
                        options);
                }
                else
                {
                    throw;
                }
            }


        }

        #endregion
    }
}
