using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Shoppers.Data.Repositories
{
    public class DataRepository<T> : IRepository<T> where T : class
    {
        private readonly ShoppersDbContext _context;
        private readonly DbSet<T> _dbSet;

        public DataRepository(ShoppersDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            SaveChanges();
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}