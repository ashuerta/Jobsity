using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsity.Core.Entity
{
    public class JwToken : IBase
    {
        [NotMapped]
        public string Token { get; set; }

        [NotMapped]
        public DateTime Expiration { get; set; }
    }
}
