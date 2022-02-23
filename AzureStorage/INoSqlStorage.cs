using System.Linq.Expressions;

namespace AzureStorageLibrary
{
  public interface INoSqlStorage<TEntity>
  {
    Task<TEntity> Add(TEntity entity);
    Task Delete(string rowKey, string partitionKey);
    Task<TEntity> Update(TEntity entity);
    Task<TEntity> Get(string rowKey, string partitionKey);
    IQueryable<TEntity> All();
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query);
  }
}
