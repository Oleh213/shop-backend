using System;
using System.Collections.Generic;
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
using sushi_backend.Context;
using sushi_backend.DTO;
using sushi_backend.Models;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Amazon;

namespace WebShop.Main.BusinessLogic
{
	public class ProductActionsBL : IProductActionsBL
    {
        private ShopContext _context;

        private readonly IConfiguration _configuration;

        
        public ProductActionsBL(ShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> GetUser(Guid userId)
            => await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        

        public async Task<Product> GetProduct(Guid productId)
            =>  await _context.products.FirstOrDefaultAsync(x => x.ProductId == productId);
        

        public async Task<bool> CheckCategory(string categoryName)
            => await _context.categories.AnyAsync(x => x.CategoryName == categoryName);
        

        public async Task<Guid> AddProduct(ProductModel model)
        {
            Guid id = Guid.NewGuid();

            var imageName = Guid.NewGuid();

            var category = await _context.categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);


            _context.products.Add(new Product()
            {
                Name = model.ProductName,
                Available = model.Available,
                Price = model.Price,
                ProductId = id,
                Description = model.Description,
                CategoryId = category.CategoryId,
                Image = imageName.ToString(),
            });

            await UploadImage(model.File, imageName.ToString());

            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<string> UpdateProduct(EditProductModel model, Product product)
        {
            var imageName = Guid.NewGuid();

            var category = await _context.categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);

            await DeleteImage(product.Image);

            product.Name = model.ProductName;
            product.Available = model.Available;
            product.CategoryId = category.CategoryId;
            product.Description = model.Description;

            product.ProductOption = await _context.productOptions.FirstOrDefaultAsync(x => x.Name == model.ProductOptionName);
            if (model.File != null)
            {
                product.Image = imageName.ToString();
                await UploadImage(model.File, imageName.ToString());
            }

            product.Price = product.Discount > 0 ?  model.Price - product.Discount : model.Price;

            await _context.SaveChangesAsync();

            return "Ok";
        }

        public async Task<bool> UploadImage(IFormFile file, string imageName)
        {
            using (var client = new AmazonS3Client("AKIATEKBWQQJRIHN2JDQ", "dNdvJlUgnOeq2EswfIuOOSqAr9nkb0iG+wIgBK/a", RegionEndpoint.EUWest1))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = string.IsNullOrEmpty("images") ? imageName : $"{"images"?.TrimEnd('/')}/{imageName}",
                        BucketName = "images-shop-angular",
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
            return true;
        }

        public async Task<bool> DeleteImage(string imageName)
        {
            using (var client = new AmazonS3Client("AKIATEKBWQQJRIHN2JDQ", "dNdvJlUgnOeq2EswfIuOOSqAr9nkb0iG+wIgBK/a", RegionEndpoint.EUWest1))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    var deleteRequest = new DeleteObjectRequest
                    {
                        Key = $"{"images"?.TrimEnd('/')}/{imageName}",
                        BucketName = "images-shop-angular",
                    };

                    var delete = await client.DeleteObjectAsync(deleteRequest);

                }
            }
            return true;
        }

        public List<ProductDTO> AllProductsDTO()
        {
            _context.products.Load();
            _context.categories.Load();
            _context.productOptions.Load();

            var imageSource = _configuration.GetValue<string>("AWS:Image-Source");

            var productDPOs = new List<ProductDTO>();

            foreach (var item in _context.products)
            {
                productDPOs.Add(new ProductDTO
                {
                    ProductId = item.ProductId,
                    Price = item.Price,
                    CategoryName = item.Category.CategoryName,
                    CategoryId = item.CategoryId,
                    ProductName = item.Name,
                    Available = item.Available,
                    Discount = item.Discount,
                    Description = item.Description,
                    Weight = item.Weight,
                    Image = imageSource + item.Image,
                    ProductOption = item.ProductOption
                });
            }
            return productDPOs;
        }

        public async Task<Product> GetOneProductWithAll(Guid productId)
            =>  await _context.products.
                Where(x => x.ProductId == productId).
                Include(x => x.Category).
                FirstOrDefaultAsync();
        

        public ProductDTO OneProductsDTO(Product product)
        {
            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                Price = product.Price,
                CategoryName = product.Category.CategoryName,
                CategoryId = product.CategoryId,
                ProductName = product.Name,
                Available = product.Available,
                Discount = product.Discount,
                Description = product.Description,
                Image = product.Image,
            };
            return productDTO;
        }

        public async Task<List<ProductOptionDTO>> ProductOptionsDTO()
        {
            var productOptionsDTO = new List<ProductOptionDTO>();

            var productOptions = await _context.productOptions.ToListAsync();

            foreach (var item in productOptions)
            {
                productOptionsDTO.Add(new ProductOptionDTO { ProductOptionsName = item.Name});
            }

            return (productOptionsDTO);
        }
    }
}

