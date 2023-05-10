using System;
using System.Data.Entity;
using Authenticate;
using Microsoft.Extensions.Options;
using sushi_backend.DTO;
using sushi_backend.Interfaces;
using WebShop.Main.DBContext;

namespace sushi_backend.BusinessLogic
{
	public class ImageSliderActionsBL : IImageSliderActionsBL
    {
        private ShopContext _context;

        public ImageSliderActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<List<ImagesSliderDTO>> GetImages()
        {
            var images = _context.imagesSliders;

            if (images != null)
            {
                var imagerDTO = new List<ImagesSliderDTO>();

                foreach (var item in images)
                {
                    imagerDTO.Add(new ImagesSliderDTO
                    {
                        Description = item.Description,
                        Image = item.Image
                    });
                }
                return imagerDTO;
            }
            else return null;
        }
    }
}

