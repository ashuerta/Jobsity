using AutoMapper;
using jbx.core.Entities.Security;
using jbx.core.Models.Identity;

namespace jbx.api.chat.MapperProfiles
{
	public class JobsityUserProfile : Profile
    {
		public JobsityUserProfile()
		{
            CreateMap<JobsityUser, RegisterViewModel>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.ConfirmPassword,
                opt => opt.Ignore());
            CreateMap<RegisterViewModel, JobsityUser>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.Email));
        }
	}
}

