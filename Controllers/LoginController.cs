﻿using System;
using System.Collections.Generic;
using Shop.Main.Actions;
using System.Linq;
using System.IO;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;
using Microsoft.Extensions.Options;
using Authenticate;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebShop.Main.Context;

namespace Shop.Main.Actions
{

    [ApiController]
    [Route("api/LogInController")]
    public class LogInController : ControllerBase  
    {
        private readonly ILoggerBL _loggerBL;

        private ILogInActionsBL _logInActionsBL;

        public LogInController(ILogInActionsBL logInActionsBL, ILoggerBL loggerBL)
        {
            _logInActionsBL = logInActionsBL;
            _loggerBL = loggerBL;
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginModule model)
        {
            try
            {
                var user = await _logInActionsBL.AuthenticateUser(model.Name, model.Password);

                if (user != null)
                {
                    var token = _logInActionsBL.GenerateJWT(user);

                    _loggerBL.AddLog(LoggerLevel.Info, $"User:'{user.Name}' login to account");

                    return Ok(new
                    {
                        access_token = token
                    });
                }
                else
                {
                    var resEr = new Response<string>()
                    {
                        IsError = true,
                        ErrorMessage = "401",
                        Data = "Check your name or password!"
                    };
                    _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{model.Name}' enter incorrect information when tried login");
                    return Unauthorized(resEr);
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


