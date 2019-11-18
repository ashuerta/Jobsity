using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsity.Core.Entity
{
    public class JobsityMessage : IBase
    {
        [NotMapped]
        public string User { get; set; }

        [NotMapped]
        public string Msg { get; set; }

        [NotMapped]
        public DateTime Date { get; set; }
    }
}
