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
using WebShop.Main.DTO;
using WebShop.Main.BusinessLogic;

namespace Shop.Main.Actions
{

    [ApiController]
    [Route("[controller]")]
    public class ProductActions : ControllerBase
    {
        private IProductActionsBL _productActionsBL;

        private readonly ILoggerBL _loggerBL;

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public ProductActions(IProductActionsBL productActionsBL, ILoggerBL loggerBL)
        {
            _productActionsBL = productActionsBL;
            _loggerBL = loggerBL;
        }

        [HttpPost("AddProduct")]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel model)
        {
            try
            {
                var user = await _productActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        if (await _productActionsBL.CheckCategory(model.CategoryId))
                        {
                            var product = await _productActionsBL.AddProduct(model);

                            var resOk = new Response<string>()
                            {
                                IsError = false,
                                ErrorMessage = "",
                                Data = $"Product successfully added!"
                            };

                            _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' add new product! (ProductId:'{product}')");
                            return Ok(resOk);
                        }
                        else
                        {
                            var resEr = new Response<string>()
                            {
                                IsError = true,
                                ErrorMessage = "401",
                                Data = $"* Error, category dont't found *"
                            };

                            _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted add new product! (CategoryId:'{model.CategoryId}' dont't found)");
                            return NotFound(resEr);
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

        [HttpPatch("UpdateProduct")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)
        {
            try
            {
                var user = await _productActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        if (await _productActionsBL.CheckCategory(model.CategoryId))
                        {
                            var product = await _productActionsBL.GetProduct(model.ProductId);

                            if (product != null)
                            {
                                await _productActionsBL.UpdateProduct(model, product);

                                var resOk = new Response<string>()
                                {
                                    IsError = false,
                                    ErrorMessage = "",
                                    Data = $"Information successfully updated!"
                                };

                                _loggerBL.AddLog(LoggerLevel.Info, $"User:'{UserId}' update informations about ProductId:'{product.ProductId}'");
                                return Ok(resOk);
                            }
                            else
                            {
                                var resEr = new Response<string>()
                                {
                                    IsError = true,
                                    ErrorMessage = "401",
                                    Data = $"* Error, product dont't found *"
                                };

                                _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted update informations about ProductId:'{model.ProductId}' (ProductId don't found)");
                                return NotFound(resEr);
                            }
                        }
                        else
                        {
                            var resEr = new Response<string>()
                            {
                                IsError = true,
                                ErrorMessage = "401",
                                Data = $"* Error, category dont't found *"
                            };

                            _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted update informations about ProductId:'{model.ProductId}' (CategoryId:'{model.CategoryId}' don't found )");
                            return NotFound(resEr);
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

                        _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted update informations about Product (Permission denied)");
                        return Unauthorized(resEr);
                    }
                }
                else return Unauthorized();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ShowProducts")]
        public IActionResult ShowProducts()
        {
            try
            {
                var productDPOs = _productActionsBL.AllProductsDTO();

                return Ok(productDPOs);
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetOneProduct")]
        public async Task<IActionResult> GetOneProduct([FromQuery] GetProductModel model)
        {
            try
            {
                var product = await _productActionsBL.GetOneProductWithAll(model.ProductId);

                if (product != null)
                {
                    var productDTO = _productActionsBL.OneProductsDTO(product);

                    return Ok(productDTO);
                }
                else return NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}