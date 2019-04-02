namespace ContosoCrm.DataAccess.Interfaces
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
        Task<Tuple<double, T>> GetItemAsync(string id, string partitionKey);
        Task<Tuple<double, IEnumerable<T>>> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector = null, string partitionKey = null);
        void Initialize(string dbId, string colId);
        Task<Document> UpdateItemAsync(string id, T item);
    }
}