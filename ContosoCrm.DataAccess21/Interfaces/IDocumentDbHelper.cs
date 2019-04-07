namespace ContosoCrm.DataAccess21.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    public interface IDocumentDbHelper<T> where T : class
    {
        Task<Document> CreateItemAsync(T item);
        Task DeleteItemAsync(string id, string partionKey);
        Task<Tuple<double, string, string, string, T>> GetItemAsync(string id, string partitionKey);
        Task<Tuple<double, string, string, string, IEnumerable<T>>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector = null, string partitionKey = null);
        void Initialize(string dbId, string colId, int offerThroughput = 1000, ConsistencyLevel consistencyLevel = ConsistencyLevel.Session, string partitionKey = null);
        Task<Document> UpdateItemAsync(string id, T item);

        Task<Offer> UpdateOfferForCollectionAsync(string collectionSelfLink, int newOfferThroughput);
    }
}