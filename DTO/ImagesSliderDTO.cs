using System;
namespace sushi_backend.DTO
{
	public class ImagesSliderDTO
	{
        public Guid ImagesSliderId { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public int ImageNumber { get; set; }
    }
}

