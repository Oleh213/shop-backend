using System;
using sushi_backend.DTO;

namespace sushi_backend.Interfaces
{
	public interface IImageSliderActionsBL
	{
		Task<List<ImagesSliderDTO>> GetImages();
	}
}

