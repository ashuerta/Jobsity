using jbx.core.Entities.Messages;
using jbx.core.Interfaces;
using jbx.infrastructure.Contexts;

namespace jbx.infrastructure.Repositories
{
	public class MessageRepository : RepositoryBase<Message>, IMessageRepository
	{
		public MessageRepository(JobsityContext jobsityContext) : base(jobsityContext)
		{
		}

		public  IEnumerable<Message> GetMessages() =>
			 _dbSet
				.OrderByDescending(o => o.Date)
				.Take(50);

    }
}

