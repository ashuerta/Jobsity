using System.Linq.Expressions;
using jbx.core.Entities;

namespace jbx.core.Interfaces
{
	public interface IRepositoryBase<T> where T : class
	{
		void Add(T entity);
		Task AddAsync(T Entity, CancellationToken cancellationToken);
		void Update(T entity);
		void Remove(T Entity);
		IQueryable<T> GetAll();
        IQueryable<T> GetAllByFilter(Expression<Func<T, bool>> predicate);
		T GetById(Guid Id);
    }
}

