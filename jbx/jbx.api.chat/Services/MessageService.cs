using AutoMapper;
using jbx.api.chat.Interfaces;
using jbx.core.Entities.Messages;
using jbx.core.Interfaces;
using jbx.core.Models.Message;
using jbx.core.Models.Rabbitmq;
using jbx.core.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace jbx.api.chat.Services
{
	public class MessageServices : IMessageServices
    {
        private readonly IMapper _mapper;
        private IMessageRepository _repository;

        public MessageServices(IMapper mapper, IMessageRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<JobsityResponse> AddMessageAsync(JobsityMessage model)
        {
            try
            {
                var msg = _mapper.Map<Message>(model);
                msg.Id = Guid.NewGuid();
                msg.Consumer = "C";
                await _repository.AddAsync(msg, CancellationToken.None);
                await _repository.SaveAsync();

                return new JobsityResponse
                {
                    Message = "Success",
                    IsSuccess = true,
                    Data = msg,
                };
            }
            catch (Exception ex)
            {
                ex.GetType();
                throw;
            }
        }

        public async Task<JobsityResponse> GetMessagesAsync()
        {
            try
            {
                var messages = _repository.GetMessages();
                if (messages.Any())
                {
                    return await Task.Run(() => new JobsityResponse
                    {
                        Message = "Success",
                        IsSuccess = true,
                        Data = messages,
                    });
                }
                return await Task.Run(() => new JobsityResponse
                {
                    Message = "No items to show",
                    Data = null,
                    IsSuccess = true,
                });
            }
            catch (Exception ex)
            {
                ex.GetType();
                throw;
            }
        }
    }
}

