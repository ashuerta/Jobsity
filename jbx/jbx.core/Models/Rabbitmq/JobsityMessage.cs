using System.ComponentModel.DataAnnotations.Schema;

namespace jbx.core.Models.Rabbitmq
{
	public class JobsityMessage
	{
        [NotMapped]
        public string User { get; set; } = string.Empty;

        [NotMapped]
        public string Msg { get; set; } = string.Empty;

        [NotMapped]
        public DateTime Date { get; set; }
    }
}

