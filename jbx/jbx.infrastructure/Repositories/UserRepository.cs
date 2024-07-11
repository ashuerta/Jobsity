using jbx.core.Entities.Security;
using jbx.core.Interfaces;
using jbx.infrastructure.Contexts;

namespace jbx.infrastructure.Repositories
{
	public class UserRepository : RepositoryBase<JobsityUser>, IUserRepository
	{
		public UserRepository(JobsityContext jobsityContext) : base(jobsityContext)
		{
		}
	}
}

