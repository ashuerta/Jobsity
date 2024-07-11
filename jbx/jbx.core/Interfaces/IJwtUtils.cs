using System.IdentityModel.Tokens.Jwt;
using jbx.core.Entities.Security;

namespace jbx.core.Interfaces
{
	public interface IJwtUtils
	{
        public JwtSecurityToken GenerateJwtToken(JobsityUser user);
        public string? ValidateJwtToken(string? token);
    }
}

