using System.Collections.Generic;
using System.Threading.Tasks;

namespace Songhay.Cloud.BlobStorage.Models
{
    public interface IRepositoryAsync
    {
        Task DeleteEntityAsync<TEntity>(object key) where TEntity : class, new();
        Task<bool> HasEntityAsync<TEntity>(object key) where TEntity : class, new();
        Task<IEnumerable<TEntity>> LoadAllAsync<TEntity>() where TEntity : class, new();
        Task<TEntity> LoadSingleAsync<TEntity>(object key) where TEntity : class, new();
        Task SaveEntityAsync<TEntity>(TEntity item) where TEntity : class, new();
    }
}
