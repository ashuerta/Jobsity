using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.Core.Entity
{
    public abstract class ExternalRepository<TEntity, TContext> : IReadExternalRepository
        where TEntity : class
        where TContext : IdentityDbContext<JobsityUser>
    {
        protected TContext _ctx;

        public async Task<IQueryable<TEntity>> GetAll()
        {
            IQueryable<TEntity> query = _ctx.Set<TEntity>();
            return await Task.Run(() => query);
        }

        public async Task<TEntity> FindBy(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {

            var query = _ctx.Set<TEntity>().Where(predicate).FirstOrDefault<TEntity>();
            return await Task.Run(() => query);
        }

        public async Task<TEntity> GetById(int id)
        {
            var query = _ctx.Set<TEntity>().Find(id);
            return await Task.Run(() => query);
        }
    }
}
