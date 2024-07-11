using jbx.core.Models.Identity;
using jbx.core.Models.Responses;

namespace jbx.api.chat.Interfaces
{
	public interface IUserServices
	{
        Task<JobsityResponse> RegisterUserAsync(RegisterViewModel model);

        Task<JobsityResponse> LoginUserAsync(LoginViewModel model);
    }
}

