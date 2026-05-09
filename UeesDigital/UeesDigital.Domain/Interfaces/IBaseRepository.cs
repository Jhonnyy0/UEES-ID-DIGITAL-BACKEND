using System.Linq.Expressions;

namespace UeesDigital.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T> Update(T entity);
        Task<bool> Delete(int id);
        Task<T?> FindFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate, int take, int page, string search);
    }
}