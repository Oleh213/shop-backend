using System;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.Interfaces;
using WebShop.Models;
using Microsoft.AspNetCore.Authorization;
using sushi_backend.Interfaces;
using WebShop.Main.BusinessLogic;
using sushi_backend.Models;

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
        public async Task<IActionResult> GetImagesSlider()
        {
            try
            {
                var images = await _imageSliderActionsBL.GetImages();

                return images != null ? Ok(images) : NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("UpdateImageSlider")]
        [Authorize]
        public async Task<IActionResult> UpdateImageSlider([FromForm] EditSlideModel model)
        {
            try
            {
                var user = await _imageSliderActionsBL.GetUser(UserId);
                if (user == null)
                {
                    return Unauthorized();
                }
                if (user.Role == UserRole.Admin)
                {
                    var response = await _imageSliderActionsBL.ChangeImage(model);
                    return response ? Ok() : NotFound();
                }

                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteImageSlide")]
        [Authorize]
        public async Task<IActionResult> DeleteImageSlide([FromQuery] EditSlideModel model)
        {
            try
            {

                var user = await _imageSliderActionsBL.GetUser(UserId);

                if (user == null)
                {
                    return Unauthorized();
                }
                if (user.Role == UserRole.Admin)
                {
                    var response = await _imageSliderActionsBL.DeleteImage(model);
                    return response ? Ok() : NotFound();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("AddNewSlide")]
        [Authorize]
        public async Task<IActionResult> AddNewSlide([FromForm] EditSlideModel model)
        {
            try
            {
                var user = await _imageSliderActionsBL.GetUser(UserId);

                if (user == null)
                {
                    return Unauthorized();
                }
                if (user.Role == UserRole.Admin)
                {
                    var response = await _imageSliderActionsBL.AddNewSlide(model);
                    return response ? Ok() : NotFound();
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

