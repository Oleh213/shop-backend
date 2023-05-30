using System;
using sushi_backend.DTO;
using sushi_backend.Models;
using WebShop.Main.Conext;

namespace sushi_backend.Interfaces
{
	public interface IImageSliderActionsBL
	{
        Task<User> GetUser(Guid userId);

        Task<List<ImagesSliderDTO>> GetImages();

        Task<bool> ChangeImage(EditSlideModel model);

        Task<bool> AddNewSlide(EditSlideModel model);

        Task<bool> DeleteImage(EditSlideModel model);

    }
}

