using System.Linq.Expressions;

namespace Shoppers.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        T GetById(int id);
        T Get(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(T entity);
        bool Any(Expression<Func<T, bool>> predicate);
        int SaveChanges();
    }
}