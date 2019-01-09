using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.EFData
{
    public class GenericRepository<T> : IDisposable, IGenericRepository<T> where T : class
    {
        JiraContext context;
        DbSet<T> dbSet;

        public GenericRepository()
        {
            context = new JiraContext();
            dbSet = context.Set<T>();
        }

        public void Add(T entity) => Save(entity);
        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate) => await context.Set<T>().Where(predicate).ToListAsync();
        public IQueryable<T> GetAll() => dbSet;
        public async Task<IQueryable<T>> GetAllAsync() => await Task.Run(() => dbSet);
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate) => context.Set<T>().Where(predicate).AsQueryable();
        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate) => await context.Set<T>().SingleOrDefaultAsync(predicate);
        public async Task<T> AddAsync(T entity)
        {
            context.Set<T>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        void Save(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
        }

        public void Edit(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Deleted;
            return await context.SaveChangesAsync();
        }
                       
        public async Task<int> EditAsync(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (context != null)
                context.Dispose();            
            GC.SuppressFinalize(this);
        }

    }
}
