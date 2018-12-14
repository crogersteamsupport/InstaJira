using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TeamSupport.EFData
{
    public interface IGenericRepository<T> where T : class
    {
        //Synchronous methods
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);

        //Asynchronous methods
        Task<IQueryable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<int> EditAsync(T entity);
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

    }
}