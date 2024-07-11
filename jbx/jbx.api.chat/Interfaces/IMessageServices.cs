using jbx.core.Models.Message;
using jbx.core.Models.Responses;

namespace jbx.api.chat.Interfaces
{
	public interface IMessageServices
	{
        Task<JobsityResponse> AddMessageAsync(MessageViewModel model);

        Task<JobsityResponse> GetMessagesAsync();
    }
}

