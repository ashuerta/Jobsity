using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsity.Core.Entity
{
    public class JobsityUser : IdentityUser, IBase
    {
        [Required(ErrorMessage = "Password is required")]
        [NotMapped]
        public string Password { get; set; }
    }
}
