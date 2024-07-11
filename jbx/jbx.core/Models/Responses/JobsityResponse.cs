namespace jbx.core.Models.Responses
{
	public class JobsityResponse
	{
		public bool IsSuccess { get; set; }

		public string Message { get; set; } = string.Empty;

		public object? Data { get; set; }

		public IEnumerable<string>? Errors { get; set; }
    }
}

