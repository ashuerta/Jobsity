using jbx.core.Entities.Messages;

namespace jbx.core.Interfaces
{
	public interface IMessageRepository : IRepositoryBase<Message>
	{
		IEnumerable<Message> GetMessages();
	}
}

