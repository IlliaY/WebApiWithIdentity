using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(int id);

        Task AddAsync(TEntity entity);

        void Delete(TEntity entity);

        Task DeleteByIdAsync(int id);

        void Update(TEntity entity);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}