namespace jbx.core.Models.Bot
{
	public class StooqDetail
	{
        public string Symbol { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;

        public string Time { get; set; } = string.Empty;

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public int Volume { get; set; }
    }
}

