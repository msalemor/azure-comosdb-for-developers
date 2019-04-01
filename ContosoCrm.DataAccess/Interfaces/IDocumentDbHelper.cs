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
        Task DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<Tuple<double, IEnumerable<T>>> GetItemsAsync(Expression<Func<T, bool>> predicate);
        void Initialize(string dbId, string colId);
        Task<Document> UpdateItemAsync(string id, T item);
    }
}