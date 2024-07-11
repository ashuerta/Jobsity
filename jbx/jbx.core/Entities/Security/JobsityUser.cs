using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using jbx.core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace jbx.core.Entities.Security
{
	public class JobsityUser : IdentityUser, IBase
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [NotMapped]
        public required string Password { get; set; }
    }
}

