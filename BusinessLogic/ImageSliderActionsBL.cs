using System;
using Amazon.S3;
using Authenticate;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using sushi_backend.DTO;
using sushi_backend.Interfaces;
using WebShop.Main.DBContext;
using sushi_backend.Models;
using WebShop.Main.Interfaces;
using WebShop.Main.Conext;
using sushi_backend.Context;

namespace sushi_backend.BusinessLogic
{
    public class ImageSliderActionsBL : IImageSliderActionsBL
    {
        private ShopContext _context;

        private IProductActionsBL _productActionsBL;

        private readonly IConfiguration _configuration;

        public ImageSliderActionsBL(ShopContext context, IConfiguration configuration, IProductActionsBL productActionsBL)
        {
            _context = context;
            _configuration = configuration;
            _productActionsBL = productActionsBL;
        }

        public async Task<User> GetUser(Guid userId)
            => await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<bool> AddNewSlide(EditSlideModel model)
        {
            var number = await _context.imagesSliders.MaxAsync(x=> x.ImageNumber);
            var imageNumber = Guid.NewGuid().ToString();

            if(await _productActionsBL.UploadImage(model.File, imageNumber, "sliders"))
            {
                await _context.imagesSliders.AddAsync(new ImagesSlider
                {
                    Image = imageNumber,
                    ImageNumber = number + 1,
                    Description = "/menu"
                }) ;

                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<List<ImagesSliderDTO>> GetImages()
        {
            var images = await _context.imagesSliders.ToListAsync();

            var imageSource = _configuration.GetValue<string>("AWS:Image-Source") + "sliders/";

            if (images != null)
            {
                var imagerDTO = new List<ImagesSliderDTO>();

                foreach (var item in images)
                {
                    imagerDTO.Add(new ImagesSliderDTO
                    {
                        Description = item.Description,
                        Image = imageSource + item.Image,
                        ImageNumber = item.ImageNumber,
                        ImagesSliderId = item.ImagesSliderId

                    });;
                }
                return imagerDTO;
            }
            else return null;
        }

        public async Task<bool> ChangeImage(EditSlideModel model)
        {
            var slide = await _context.imagesSliders.FirstOrDefaultAsync(x=> x.ImagesSliderId == model.ImageSliderId);

            var imageId = Guid.NewGuid().ToString();

            await _productActionsBL.DeleteImage(slide.Image);

            if (slide == null)
            {
                return false;
            }

            slide.Description = model.Description;
            slide.ImageNumber = model.ImageNumber;

            await _context.SaveChangesAsync();
            return true;

            if (model.File != null)
            {
                slide.Image = imageId;
                await _productActionsBL.UploadImage(model.File, model.File.FileName, "products-main");
            }
        }

        public async Task<bool> DeleteImage(EditSlideModel model)
        {
            var slide = await _context.imagesSliders.FirstOrDefaultAsync(x => x.ImageNumber == model.ImageNumber);

            if (slide == null)
            {
                return false;
            }
            if (await _productActionsBL.DeleteImage(slide.Image))
            {
                _context.imagesSliders.Remove(slide);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

