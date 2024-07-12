using jbx.core.Models.Message;
using jbx.core.Models.Rabbitmq;
using jbx.core.Models.Responses;

namespace jbx.api.chat.Interfaces
{
	public interface IMessageServices
	{
        Task<JobsityResponse> AddMessageAsync(JobsityMessage model);

        Task<JobsityResponse> GetMessagesAsync();
    }
}

