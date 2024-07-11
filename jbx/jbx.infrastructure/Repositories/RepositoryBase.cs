using System.Linq.Expressions;
using jbx.core.Interfaces;
using jbx.infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace jbx.infrastructure.Repositories
{
	public class RepositoryBase<T> : IRepositoryBase<T> where T : class
	{
		protected readonly JobsityContext _ctx;
		protected readonly DbSet<T> _dbSet;

		public RepositoryBase(JobsityContext ctx)
		{
			_ctx = ctx;
			_dbSet = _ctx.Set<T>();
		}

		public void Add(T entity) => _dbSet.Add(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Remove(T entity) => _dbSet.Remove(entity);
        public async Task AddAsync(T entity, CancellationToken cancellationToken) => await _dbSet.AddAsync(entity, cancellationToken);
		public IQueryable<T> GetAll() => _dbSet;
        public IQueryable<T> GetAllByFilter(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);
        public T GetById(Guid id) => _dbSet.Find(id) ?? throw new KeyNotFoundException();
    }
}

