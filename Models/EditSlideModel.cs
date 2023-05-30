using System;
namespace sushi_backend.Models
{
	public class EditSlideModel
	{
		public int? ImageNumber { get; set; }

        public IFormFile? File { get; set; }
    }
}

