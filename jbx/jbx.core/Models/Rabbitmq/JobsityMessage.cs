using System.ComponentModel.DataAnnotations.Schema;

namespace jbx.core.Models.Rabbitmq
{
	public class JobsityMessage
	{
        public string User { get; set; } = string.Empty;

        public string Msg { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}

