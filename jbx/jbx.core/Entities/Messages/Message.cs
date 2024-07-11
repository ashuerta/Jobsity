namespace jbx.core.Entities.Messages
{
	public class Message : EntityBase
	{
		public Message()
		{
		}
		public Guid Id { get; set; }
		public string TypedMessage { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string Consumer { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}

