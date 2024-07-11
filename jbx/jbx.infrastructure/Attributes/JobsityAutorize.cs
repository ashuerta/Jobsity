using jbx.core.Interfaces;
using jbx.core.Utils;
using jbx.infrastructure.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace jbx.infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JobsityAutorize : Attribute, IAuthorizationFilter
    {
        /*private readonly IJwtUtils _jwtUtils;

        public JobsityAutorize(IJwtUtils jwtUtils)
        {
            _jwtUtils = jwtUtils;
        }*/

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var token = context.HttpContext.Session.GetString("jwtToken");

            if (token.IsNullOrEmpty())
            {
                // not logged in or role not authorized
                context.Result = new UnauthorizedObjectResult(string.Empty);
                return;
            }
            var jwtUtils =
            context.HttpContext.RequestServices.GetService(typeof(IJwtUtils))
                as JwtUtils;
            if (jwtUtils == null)
                throw new NullReferenceException();
            // Verify the token using the JwtSecurityTokenHandlerWrapper
            var userId = jwtUtils.ValidateJwtToken(token);
            context.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        }
    }
}

