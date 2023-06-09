using System;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Shop.Main.BusinessLogic;
using WebShop.Main.BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebShop.Main.Context;
using sushi_backend.Interfaces;
using WebShop.Models;

namespace Shop.Main.Actions
{
    [ApiController]
    [Route("api/CartItemController")]
    public class CartItemController : ControllerBase
    {

        private readonly ICartItemActionsBL _cartItemActionsBL;

        private readonly ILoggerBL _loggerBL;

        private IEmailSender _emailSender;


        public CartItemController(ICartItemActionsBL cartItemActionsBL, ILoggerBL loggerBL, IEmailSender emailSender)
        {
            _cartItemActionsBL = cartItemActionsBL;
            _loggerBL = loggerBL;
            _emailSender = emailSender;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);


        [HttpPatch("ShowCart")]
        public async Task<IActionResult> ShowCart([FromBody] List<CartItemModel> cartItems)
        {
            try
            {
                if(cartItems.Count() > 0)
                {
                   var cartItemsDTO = await _cartItemActionsBL.CartItemsDTO(cartItems);

                    return Ok(cartItemsDTO);

                }
                else
                {
                    return NotFound();
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

