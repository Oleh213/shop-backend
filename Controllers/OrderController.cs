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
using sushi_backend.Models;
using MailKit.Search;
using sushi_backend.Interfaces;

namespace Shop.Main.Actions
{
    [ApiController]
    [Route("api/OrderController")]
    public class OrderController : ControllerBase
    {
        private IOrderActionsBL _orderActionsBL;

        private readonly ILoggerBL _loggerBL;

        private readonly ITimeLinesActionsBL _timeLinesActionsBL;

        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(IOrderActionsBL orderActionsBL, ILoggerBL loggerBL, IHubContext<OrderHub> hubContext, ITimeLinesActionsBL timeLinesActionsBL)
        {
            _orderActionsBL = orderActionsBL;
            _loggerBL = loggerBL;
            _hubContext = hubContext;
            _timeLinesActionsBL = timeLinesActionsBL;
        }

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        [HttpPost("Buy")]
        public async Task<IActionResult> MakeOrder([FromBody] OrderModel orderModel)
        {
            try
            {
                if(await _timeLinesActionsBL.CheckShopWork())
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

                    if(respons != null)
                    {
                        var resOk = new Response<OrderResponsModel>()
                        {
                            IsError = false,
                            ErrorMessage = "",
                            Data = respons,
                        };
                        return Ok(resOk);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                var resError2 = new Response<string>()
                {
                    IsError = true,
                    ErrorMessage = "Error ",
                    Data = $" Вибачте але магазин зараз не працює"
                };

                return NotFound(resError2);


            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ShowBuyList")]
        public async Task<IActionResult> ShowBuyList(Guid orderId)
        {
            try
            {
                var invoiceRequest = new LiqPayRequest
                {
                    OrderId = orderId.ToString(),
                    Action = LiqPayRequestAction.Status,
                    Language = LiqPayRequestLanguage.UK,
                };


                var liqPayClient = new LiqPayClient("sandbox_i35438868943", "sandbox_hk7Vbmn1Li9UOa3P13ZYyOZSnac8JlzWa96IJYZz");

                var response = await liqPayClient.RequestAsync("request", invoiceRequest);

                var orderRespons = new OrderResponsModel { Href = response.Href, OrderId = orderId };



                return Ok(response);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

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
        [Authorize]
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
                            return Unauthorized();
                        }
                        if (order != null)
                        {
                            var orderStaus = order.OrderStatus;
                            await _orderActionsBL.ChangeOrderStatus(order, model.orderStatus);

                            _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' changed order status Order:'{order.OrderId}'(From:{orderStaus} to:'{order.OrderStatus}')");
                            return Ok();
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else
                    {
                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted changed order status(Permission denied)");
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

        [HttpGet("GetNewOrders")]
        [Authorize]
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

                        var dd = await _orderActionsBL.GetNewOrders();

                        return Ok(dd);
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

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById([FromQuery] string orderId)
        {
            try
            {
                var order = await _orderActionsBL.GetOrderById(orderId);

                return order != null ? Ok(order) : NotFound();

            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("GetUserOrders")]
        public async Task<IActionResult> GetUserOrders([FromBody] string[] orders)
        {
            try
            {
                var resultOrders = await _orderActionsBL.GetUserOrders(orders);

                if (resultOrders.Count > 0)
                {
                    return Ok(resultOrders);
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("PaymentStatus")]
        public IActionResult PaymentStatus([FromForm] string data, [FromForm] string signature)
        {
            try
            {
                return (_orderActionsBL.ConfirmPayment(data, signature)) ? Ok() : NotFound();

            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
       

    }
}




