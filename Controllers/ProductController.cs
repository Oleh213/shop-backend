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
using sushi_backend.DTO;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using sushi_backend.Models;

namespace Shop.Main.Actions
{

    [ApiController]
    [Route("api/ProductController")]
    public class ProductController : ControllerBase
    {
        private IProductActionsBL _productActionsBL;

        private readonly ILoggerBL _loggerBL;

        private Guid UserId => Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);

        public IAmazonS3 _s3Client;

        public ProductController(IProductActionsBL productActionsBL, ILoggerBL loggerBL, IAmazonS3 s3Client)
        {
            _productActionsBL = productActionsBL;
            _loggerBL = loggerBL;
            _s3Client = s3Client;

        }

        [HttpPost("AddProduct")]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromForm] ProductModel model)
        {
            try
            {
                var user = await _productActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        if (await _productActionsBL.CheckCategory(model.CategoryName))
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

                            _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted add new product! (CategoryId:'{model.CategoryName}' dont't found)");
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
        public async Task<IActionResult> UpdateProduct([FromForm] EditProductModel model)
        {
            try
            {
                var user = await _productActionsBL.GetUser(UserId);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        if (await _productActionsBL.CheckCategory(model.CategoryName))
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

                            _loggerBL.AddLog(LoggerLevel.Warn, $"User:'{UserId}' wanted update informations about ProductId:'{model.ProductId}' (CategoryId:'{model.CategoryName}' don't found )");
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
                    var productDTO = await _productActionsBL.OneProductsDTO(product);

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

        [Authorize]
        [HttpGet("GetProductOptions")]
        public async Task<IActionResult> GetProductOptions()
        {
            try
            {
                var productOptions = await _productActionsBL.ProductOptionsDTO();

                return productOptions != null ? Ok(productOptions) : NotFound();
            }
            catch (Exception ex)
            {
                _loggerBL.AddLog(LoggerLevel.Error, $"Message: '{ex.Message}', Source: '{ex.Source}', InnerException: '{ex.InnerException}' ");

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploudFileModel uploudFileModel)
        {
            var file = uploudFileModel.File;
            var bucketExists = await _s3Client.DoesS3BucketExistAsync("images-shop-angular");
            if (!bucketExists) return NotFound($"Bucket images-shop-angularInfo does not exist.");
            var request = new PutObjectRequest()
            {
                BucketName = "images-shop-angular",
                Key = string.IsNullOrEmpty("images") ? Guid.NewGuid().ToString() : $"{"images"?.TrimEnd('/')}/{Guid.NewGuid().ToString()}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return Ok($"File prefix/{file.FileName} uploaded to S3 successfully!");
        }

    }
}