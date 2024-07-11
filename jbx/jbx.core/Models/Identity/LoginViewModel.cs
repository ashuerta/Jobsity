using System.ComponentModel.DataAnnotations;

namespace jbx.core.Models.Identity
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; } = string.Empty;

        public bool Remember { get; set; }
    }
}

