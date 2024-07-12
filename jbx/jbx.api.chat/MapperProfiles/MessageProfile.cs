using AutoMapper;
using jbx.core.Entities.Messages;
using jbx.core.Models.Rabbitmq;

namespace jbx.api.chat.MapperProfiles
{
	public class MessageProfile : Profile
    {
		public MessageProfile()
		{
            CreateMap<Message, JobsityMessage>()
                .ForMember(dest => dest.Msg,
                opt => opt.MapFrom(src => src.TypedMessage))
                .ForMember(dest => dest.User,
                opt => opt.MapFrom(src => src.Sender));
            CreateMap<JobsityMessage, Message>()
                .ForMember(dest => dest.Sender,
                opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.TypedMessage,
                opt => opt.MapFrom(src => src.Msg));
        }
	}
}

