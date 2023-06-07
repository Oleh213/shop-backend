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
using System.Net;
using System.Text;

namespace Shop.Main.Actions
{
    [ApiController]
    [Route("api/AddAdminController")]
    public class AddAdminController : ControllerBase
    {

        private readonly IAdminActionsBL _addAdminActionsBL;

        private readonly ILoggerBL _loggerBL;

        private IEmailSender _emailSender;


        public AddAdminController(IAdminActionsBL addAdminActionsBL, ILoggerBL loggerBL, IEmailSender emailSender)
        {
            _addAdminActionsBL = addAdminActionsBL;
            _loggerBL = loggerBL;
            _emailSender = emailSender;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);


        [HttpPut("AddAdmin")]
        public async Task<IActionResult> AddAdmin()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

