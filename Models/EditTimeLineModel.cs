using System;
using sushi_backend.Context;

namespace sushi_backend.Models
{
	public class EditTimeLineModel
	{
        public Guid TimeLineId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public TimeConfig TimeConfig { get; set; }

        public bool IsOpen { get; set; }

        public string Note { get; set; }

        public int Priority { get; set; }
    }
}

