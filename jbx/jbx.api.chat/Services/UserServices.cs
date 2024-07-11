using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using jbx.api.chat.Interfaces;
using jbx.core.Entities.Security;
using jbx.core.Interfaces;
using jbx.core.Models.Identity;
using jbx.core.Models.Responses;
using jbx.infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace jbx.api.chat.Services
{
	public class UserServices : IUserServices
    {
        private readonly IMapper _mapper;
        private UserManager<JobsityUser> _userManager;
        private readonly IJwtUtils _jwtUtils;
        private JobsityContext _jobsityContext;

        public UserServices(IMapper mapper, UserManager<JobsityUser> userManager, IJwtUtils jwtUtils, JobsityContext jobsityContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtUtils = jwtUtils;
            _jobsityContext = jobsityContext;
        }

        public async Task<JobsityResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return new JobsityResponse
                {
                    Message = "Login user fail(1)",
                    IsSuccess = false,
                };
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return new JobsityResponse
                {
                    Message = "Login user fail(2)",
                    IsSuccess = false,
                };
            }
            var token = _jwtUtils.GenerateJwtToken(user);
            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            return new JobsityResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
            };
        }

        public async Task<JobsityResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register model is null");
            }
            if (model.Password != model.ConfirmPassword)
            {
                return new JobsityResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };
            }

            var user = _mapper.Map<JobsityUser>(model);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return new JobsityResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new JobsityResponse
            {
                Message = "User can not be created",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}

