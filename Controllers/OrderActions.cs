using System;
using System.Collections.Generic;
// using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using Microsoft.EntityFrameworkCore;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebShop.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using WebShop.Main.Hubs;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using Telegram.Bot;
using System.Threading;
using Telegram.Bot.Types;
using System.Text;
using LiqPay.SDK.Dto;
using LiqPay.SDK;
using LiqPay.SDK.Dto.Enums;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Shop.Main.Actions
{
    [ApiController]
    [Route("OrderActions")]
    public class OrderActions : ControllerBase
    {
        private IOrderActionsBL _orderActionsBL;

        private readonly ILoggerBL _loggerBL;

        private readonly IHubContext<OrderHub> _hubContext;

        public OrderActions(IOrderActionsBL orderActionsBL, ILoggerBL loggerBL, IHubContext<OrderHub> hubContext)
        {
            _orderActionsBL = orderActionsBL;
            _loggerBL = loggerBL;
            _hubContext = hubContext;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        [HttpPost("Buy")]
        public async Task<IActionResult> MakeOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                if (!_orderActionsBL.CheckCountOfProducts(orderModel.cartItems))
                {
                    var resError = new Response<string>()
                    {
                        IsError = true,
                        ErrorMessage = "Error ",
                        Data = $" Один із товарів у вашому кошику на даний сонент недоступний"
                    };

                    return NotFound(resError);
                }

                var respons = await _orderActionsBL.CreateNewOrder(orderModel.cartItems, orderModel.deliveryInfo, orderModel.paymentMethod, orderModel.contactInfo, orderModel.promoCode);

                var resOk = new Response<string>()
                {
                    IsError = false,
                    ErrorMessage = "",
                    Data = respons,
                };


                return Ok(resOk);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Authorize]
        //[HttpGet("ShowBuyList")]
        //public async Task<IActionResult> ShowBuyList()
        //{
        //    try
        //    {
        //        var orderedProductIds = await _orderActionsBL.ShowOrders(UserId);

        //        return Ok(orderedProductIds);
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPatch("GetTotalPrice")]
        public async Task<IActionResult> GetTotalPrice(List<CartItemModel> cartItems)
        {
            try
            {
                if (cartItems != null)
                {
                    var totalPrice = _orderActionsBL.GetTotalPrice(cartItems);

                    var resOk = new Response<int>()
                    {
                        IsError = false,
                        ErrorMessage = " ",
                        Data = totalPrice,
                    };

                    return Ok(resOk);
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

        [HttpPost("ChangeOrderStatus")]
        public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusModel model)
        {
            try
            {
                var user = await _orderActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Manager || model.orderStatus == OrderStatus.Canceled)
                    {
                        var order = await _orderActionsBL.GetOrder(model.OrderId);

                        if (order.OrderStatus != OrderStatus.AwaitingConfirm && model.orderStatus == OrderStatus.Canceled)
                        {
                            _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted changed order status Order:'{order.OrderId}'(From:{model.orderStatus} to:'{order.OrderStatus}')");
                            return NotFound();
                        }
                        if (order != null)
                        {
                            await _orderActionsBL.ChangeOrderStatus(order, model.orderStatus);

                            _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' changed order status Order:'{order.OrderId}'(From:{model.orderStatus} to:'{order.OrderStatus}')");
                            return Ok();
                        }
                    }
                    else
                    {
                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted changed order status(Permission denied)");
                        return NotFound();
                    }
                    return NotFound();
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetNewOrders")]
        public async Task<IActionResult> GetNewOrders()
        {
            try
            {
                var user = await _orderActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Manager)
                    {
                        _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' get new orders");

                        return Ok(await _orderActionsBL.GetNewOrders());
                    }
                    else
                    {
                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted get new orders(Permission denied)");
                        return NotFound();
                    }
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("MakeTestPayment")]
        public async Task<IActionResult> MakeTestPayment()
        {
            try
            {
                //var response = await _orderActionsBL.GoToPayment("",);

                return Ok();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("PaymentStatus")]
        public IActionResult PaymentStatus([FromBody] LiqPayCheckoutFormModel data)
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
    public class LiqPayCheckoutFormModel
    {
        public string request_data { get; set; }
    }

}




