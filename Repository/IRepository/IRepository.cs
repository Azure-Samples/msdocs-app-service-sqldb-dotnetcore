using System.Linq.Expressions;

namespace DotNetCoreSqlDb.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        void Add(T entity);
        void Remove(T entity);
        void Remove(int id);
        void RemoveRange(IEnumerable<T> entity);
        T Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orederBy = null,
            string includeProperties = null
            );
        T FirstorDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );
        void Update(T entity);
    }
}
