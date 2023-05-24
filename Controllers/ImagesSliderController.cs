using System;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.Interfaces;
using WebShop.Models;
using Microsoft.AspNet.SignalR;
using sushi_backend.Interfaces;

namespace sushi_backend.Controllers
{
    [ApiController]
    [Route("api/ImagesSliderController")]
    public class ImagesSliderController : ControllerBase
    { 
    
        private IImageSliderActionsBL _imageSliderActionsBL;

        private readonly ILoggerBL _loggerBL;

        public ImagesSliderController(IImageSliderActionsBL imageSliderActionsBL, ILoggerBL loggerBL)
        {
            _imageSliderActionsBL = imageSliderActionsBL;
            _loggerBL = loggerBL;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        [HttpGet("GetImagesSlider")]
        [Authorize]
        public async Task<IActionResult> GetImagesSlider()
        {
            try
            {
                var images = await _imageSliderActionsBL.GetImages();

                if (images != null)
                {
                    return Ok(images);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
	}
}

