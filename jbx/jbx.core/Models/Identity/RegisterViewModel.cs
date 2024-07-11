using System.ComponentModel.DataAnnotations;

namespace jbx.core.Models.Identity
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}

