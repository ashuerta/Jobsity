using System.ComponentModel.DataAnnotations.Schema;
namespace jbx.core.Entities.Security
{
	public class JwToken
	{
        public JwToken(bool isAuthenticated, string token)
        {
            IsAuthenticated = isAuthenticated;
            Token = token;
            RefreshToken = string.Empty;
        }
        public bool IsAuthenticated { get; set; }

        [NotMapped]
        public required string Token { get; set; }

        [NotMapped]
        public DateTime Expiration { get; set; }

        public string RefreshToken { get; set; }
    }
}

