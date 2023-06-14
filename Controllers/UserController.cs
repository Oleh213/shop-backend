using System;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Models;
using Microsoft.AspNetCore.Authorization;
using WebShop.Main.DTO;
using WebShop.Main.Interfaces;
using WebShop.Main.BusinessLogic;
using WebShop.Main.Context;
using Firebase;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace WebShop.Main.Actions
{
    [ApiController]
    [Route("api/UserController")]
    public class UserController : ControllerBase
    {
        private IUserActionsBL _userActionsBL;

        private readonly ILoggerBL _loggerBL;

        public UserController(IUserActionsBL userActionsBL, ILoggerBL loggerBL)
        {
            _userActionsBL = userActionsBL;
            _loggerBL = loggerBL;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        [HttpPost("AddUserInfo")]
        [Authorize]
        public async Task<IActionResult> AddUserInfo([FromBody] UserInfoModel model)
        {
            try
            {
                var user = await _userActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (!await _userActionsBL.CheckName(model.Name))
                    {
                        if (!await _userActionsBL.CheckEmail(model.Email))
                        {

                            await _userActionsBL.ChangeUserInfo(model, user);

                            var res = new Response<string>()
                            {
                                IsError = false,
                                ErrorMessage = "",
                                Data = "Your information successful update"
                            };

                            _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' update information");
                            return Ok(res);
                        }
                        else
                        {
                            var resError = new Response<string>()
                            {
                                IsError = true,
                                ErrorMessage = "",
                                Data = "Please enter another email!"
                            };

                            return NotFound(resError);
                        }
                    }
                    else
                    {
                        var resError = new Response<string>()
                        {
                            IsError = true,
                            ErrorMessage = "",
                            Data = "Please enter another name!"
                        };
                        return NotFound(resError);
                    }
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetNameOfUser")]
        [Authorize]
        public async Task<IActionResult> GetNameOfUser()
        {
            try
            {
                var user = await _userActionsBL.GetUser(UserId);

                if (user != null)
                {
                    var res = new Response<string>()
                    {
                        IsError = false,
                        ErrorMessage = "",
                        Data = user.Name,
                    };
                    return Ok(res);
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

       

        [HttpGet("GetUser")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var user = await _userActionsBL.GetUser(UserId);

                var userDTO = _userActionsBL.UserDTO(user);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {

                string accountSid = "AC68eaa65890dc3174e00fb6c17ffdbc03";
                string authToken = "c19bf0cea77ca297888a292c642387a3";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "Hi Yohan, We are from Twilio Code: 13",
                    from: new Twilio.Types.PhoneNumber("+14302434786"), // virtual Twilio number
                    to: new Twilio.Types.PhoneNumber("+380666638032")
                );

                return Ok(message);

                //var user = await _userActionsBL.GetUser(UserId);

                //if (user != null)
                //{
                //    var outUser = await _userActionsBL.UserInfoDTO(user);

                //    return Ok(outUser);
                //}
                //else
                //    return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
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