using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sushi_backend.Interfaces;
using sushi_backend.Models;
using Telegram.Bot.Types;
using WebShop.Main.BusinessLogic;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.Interfaces;
using WebShop.Models;

namespace sushi_backend.Controllers
{
    [ApiController]
    [Route("api/TimeLineController")]
    public class TimeLineController : ControllerBase
    {
        private ITimeLinesActionsBL _timeLinesActionsBL;

        private readonly ILoggerBL _loggerBL;

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public TimeLineController(ITimeLinesActionsBL timeLinesActionsBL, ILoggerBL loggerBL)
        {
            _timeLinesActionsBL = timeLinesActionsBL;
            _loggerBL = loggerBL;
        }

        [HttpPost("AddTimeLine")]
        [Authorize]
        public async Task<IActionResult> AddTimeLine([FromBody] AddNewTimeLineModel model)
        {
            try
            {
                var user = await _timeLinesActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        var check = await _timeLinesActionsBL.AddTimeLine(model.From, model.To, model.TimeConfig, model.IsOpen, model.Note, model.Priority);

                        if (check)
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        var resEr = new Response<string>()
                        {
                            IsError = true,
                            ErrorMessage = "401",
                            Data = $"* Error, you dont have permissions! *"
                        };

                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted add new Product (Permission denied)");
                        return Unauthorized(resEr);
                    }
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CloseShopTillToday")]
        [Authorize]
        public async Task<IActionResult> CloseShopTillToday()
        {
            try
            {
                var user = await _timeLinesActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        var check = await _timeLinesActionsBL.CloseShopTillToday();

                        if (check)
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        var resEr = new Response<string>()
                        {
                            IsError = true,
                            ErrorMessage = "401",
                            Data = $"* Error, you dont have permissions! *"
                        };

                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted add new Product (Permission denied)");
                        return Unauthorized(resEr);
                    }
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPatch("EditTimeLine")]
        [Authorize]
        public async Task<IActionResult> EditTimeLine([FromBody] EditTimeLineModel model)
        {
            try
            {
                var user = await _timeLinesActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        var check = await _timeLinesActionsBL.EditTimeLine(model);

                        if (check)
                        {
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }

                    }
                    else
                    {
                        var resEr = new Response<string>()
                        {
                            IsError = true,
                            ErrorMessage = "401",
                            Data = $"* Error, you dont have permissions! *"
                        };

                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted add new Product (Permission denied)");
                        return Unauthorized(resEr);
                    }
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("GetTimeLines")]
        [Authorize]
        public async Task<IActionResult> GetTimeLines()
        {
            try
            {
                var response = await _timeLinesActionsBL.ShowTimeLines();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("CheckShopStatus")]
        public async Task<IActionResult> CheckShopStatus()
        {
            try
            {
                var response = await _timeLinesActionsBL.CheckShopWork();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteTimeLine")]
        [Authorize]
        public async Task<IActionResult> DeleteTimeLine([FromQuery] Guid timeLineId)
        {
            try
            {
                var response = await _timeLinesActionsBL.DeleteTimeLine(timeLineId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

