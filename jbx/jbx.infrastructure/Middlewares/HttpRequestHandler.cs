using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using jbx.core.Interfaces;
using System.Net;

namespace jbx.infrastructure.Middlewares
{
	public class HttpRequestHandler : IMiddleware
	{
        private readonly ILogger _logger;
        private readonly IJwtUtils _jwtUtils;

        public HttpRequestHandler(
            ILoggerFactory loggerFactory,
            IJwtUtils jwtUtils)
        {
            _logger = loggerFactory.CreateLogger<HttpRequestHandler>(); ;
            _jwtUtils = jwtUtils;
        }

        //public async Task Invoke(HttpContext context)
        //{
            //_logger.LogInformation("Before request");

            ///await _next.Invoke(context);

            // Clean up.
        //}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _logger.LogInformation("Before request");
            // Get the token from the Authorization header

            var token = context.Session.GetString("jwtToken");

            if (!token.IsNullOrEmpty())
            {
                try
                {
                    // Verify the token using the JwtSecurityTokenHandlerWrapper
                    var userId = _jwtUtils.ValidateJwtToken(token);

                    // Store the user ID in the HttpContext items for later use
                    context.Items["UserId"] = userId;
                    context.Request.Headers.Add("Authorization", $"Bearer {token}");
                    // You can also do the for same other key which you have in JWT token.
                }
                catch (Exception)
                {
                    // If the token is invalid, throw an exception
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    await next(context);
                }
            }
            // Continue processing the request
            await next(context);
        }
    }
}

