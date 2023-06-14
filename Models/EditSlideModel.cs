using System;
namespace sushi_backend.Models
{
	public class EditSlideModel
	{
		public Guid ImageSliderId { get; set; }

        public int ImageNumber { get; set; }

		public string? Description { get; set; }

        public IFormFile? File { get; set; }
    }
}

