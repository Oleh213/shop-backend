using System;
using System.Linq;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sushi_backend.Models;
using WebShop.Main.BusinessLogic;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;
using WebShop.Models;

namespace Shop.Main.Actions
{
    [ApiController]
    [Route("api/PromoCodeController")]
    public class PromoCodeController : ControllerBase
    {
        private IPromoCodeActionsBL _promocodeActionsBL;

        private readonly ILoggerBL _loggerBL;

        public PromoCodeController(IPromoCodeActionsBL promocodeActionsBL, ILoggerBL loggerBL)
        {
            _promocodeActionsBL = promocodeActionsBL;
            _loggerBL = loggerBL;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        [HttpPost("AddNewPromoCode")]
        [Authorize]
        public async Task<IActionResult> AddNewPromoCode([FromBody] AddNewPromoModel model)
        {
            try
            {
                var user = await _promocodeActionsBL.GetUser(UserId);

                var response = new Response<string> { IsError = true };
                if (user == null || user.Role != UserRole.Admin)
                {
                    response.ErrorMessage = "Permissions denied";
                    return Unauthorized(response);
                }
                if (await _promocodeActionsBL.CheckPromo(model.Code))
                {
                    response.ErrorMessage = "Promocode name is exist";
                    return Conflict(response);
                }
                if (await _promocodeActionsBL.AddNewPromo(model))
                {
                    response.IsError = false;
                    response.Data = "Promocode added!";
                    return Ok(response);
                }
                else
                {
                    response.ErrorMessage = "Contact with developer!";
                    return Conflict(response);
                }
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetAllPromoCodes")]
        [Authorize]
        public async Task<IActionResult> GetAllPromoCodes()
        {
            try
            {
                var user = await _promocodeActionsBL.GetUser(UserId);
                if (user == null || user.Role != UserRole.Admin)
                {
                    var responseError = new Response<string> {
                        IsError = true,
                        ErrorMessage = "Permissions denied"
                    };
                    return Unauthorized(responseError);
                }

                var promocodes = await _promocodeActionsBL.GetAllPromocodes();

                var response = new Response<List<Promocode>>
                {
                    IsError = false,
                    Data = promocodes,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeletePromoCode")]
        [Authorize]
        public async Task<IActionResult> DeletePromoCode([FromQuery] Guid promoCodeId)
        {
            try
            {
                var user = await _promocodeActionsBL.GetUser(UserId);
                if (user == null || user.Role != UserRole.Admin)
                {
                    var responseError = new Response<string>
                    {
                        IsError = true,
                        ErrorMessage = "Permissions denied"
                    };
                    return Unauthorized(responseError);
                }

                var promocode = await _promocodeActionsBL.GetPromocodeById(promoCodeId);

                if(promocode == null)
                {
                    var responseError2 = new Response<string>
                    {
                        IsError = true,
                        ErrorMessage = "Promo Code id didn't find!",
                    };

                    return NotFound(responseError2);
                }

                var response = new Response<string>
                {
                    IsError = false,
                    Data = "Promo code deleted!"
                };
                await _promocodeActionsBL.DeletePromocode(promocode);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("EditPromoCode")]
        [Authorize]
        public async Task<IActionResult> EditPromoCode([FromBody] Promocode newPromoCode)
        {
            try
            {
                var user = await _promocodeActionsBL.GetUser(UserId);
                if (user == null || user.Role != UserRole.Admin)
                {
                    var responseError = new Response<string>
                    {
                        IsError = true,
                        ErrorMessage = "Permissions denied"
                    };
                    return Unauthorized(responseError);
                }

                var promocode = await _promocodeActionsBL.GetPromocodeById(newPromoCode.PromocodetId);

                if (promocode == null)
                {
                    var responseError2 = new Response<string>
                    {
                        IsError = true,
                        ErrorMessage = "Promo Code id didn't find!",
                    };

                    return NotFound(responseError2);
                }

                //if (await _promocodeActionsBL.CheckPromo(promocode.Code))
                //{
                //    var responseError2 = new Response<string>
                //    {
                //        IsError = true,
                //        ErrorMessage = "Promo Code name is already exist!",
                //    };

                //    return NotFound(responseError2);
                //}

                var response = new Response<string>
                {
                    IsError = false,
                    Data = "Promo code modified!"
                };
                await _promocodeActionsBL.EditPromoCode(promocode, newPromoCode);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("UsePromoCode")]
        public async Task<IActionResult> UsePromoCode([FromBody] PromocodeModel model)
        {
            try
            {
                var promo = await _promocodeActionsBL.GetPromocode(model.Code);

                if (promo != null)
                {
                    var resOk = new Response<int>()
                    {
                        IsError = true,
                        ErrorMessage = "",
                        Data = promo.Discount,
                    };

                    return Ok(resOk);
                }
                else
                {
                    var resError = new Response<int>()
                    {
                        IsError = true,
                        ErrorMessage = "Промокод не активний.",
                        Data = 0,
                    };
                    return NotFound(resError);
                }
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

