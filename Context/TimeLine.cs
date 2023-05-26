using System;
namespace sushi_backend.Context
{
	public class TimeLine
	{
		public Guid TimeLineId { get; set; }

		public DateTime From { get; set; }

		public DateTime To { get; set; }

		public TimeConfig TimeConfig { get; set; }

		public int Priority { get; set; }

        public bool IsOpen { get; set; }

		public string Note { get; set; }
    }

	public enum TimeConfig
	{
        Repeat,
		Once,
	}
}

