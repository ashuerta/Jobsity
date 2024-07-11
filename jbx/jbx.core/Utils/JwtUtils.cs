using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jbx.core.Entities.Security;
using jbx.core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace jbx.core.Utils
{
	public class JwtUtils : IJwtUtils
    {
        private IConfiguration _configuration;

        public JwtUtils(IConfiguration configuration)
        {
            _configuration = configuration;

            if (string.IsNullOrEmpty(_configuration["AuthSettings:Key"]))
                throw new Exception("JWT secret not configured");
        }

        public JwtSecurityToken GenerateJwtToken(JobsityUser user)
        {
            var claims = new[]
            {
                new Claim("username", user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"] ?? "No valid key present"));
            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        public string? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AuthSettings:Key"]!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["AuthSettings:Issuer"],
                    ValidAudience = _configuration["AuthSettings:Audience"],
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                // return user id from JWT token if validation successful
                return userId;
            }
            catch (Exception ex)
            {
                ex.GetType();
                // return null if validation fails
                return null;
            }
        }
    }
}

