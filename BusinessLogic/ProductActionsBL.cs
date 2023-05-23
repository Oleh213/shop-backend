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
using sushi_backend.Context;
using sushi_backend.DTO;
using sushi_backend.Models;
using Amazon.S3.Model;
using Amazon.S3;

namespace WebShop.Main.BusinessLogic
{
	public class ProductActionsBL : IProductActionsBL
    {
        private ShopContext _context;

        private readonly IConfiguration _configuration;

        public IAmazonS3 _s3Client;

        
        public ProductActionsBL(ShopContext context, IConfiguration configuration, IAmazonS3 s3Client)
        {
            _context = context;
            _configuration = configuration;
            _s3Client = s3Client;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            return await _context.products.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<bool> CheckCategory(string categoryName)
        {
            return await _context.categories.AnyAsync(x => x.CategoryName == categoryName);
        }

        public async Task<Guid> AddProduct(ProductModel model)
        {
            Guid id = Guid.NewGuid();
            _context.products.Add(new Product()
            {
                Name = model.Name,
                Available = model.Available,
                Price = model.Price,
                ProductId = id,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Image = model.Img,
            });

            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<string> UpdateProduct(EditProductModel model, Product product)
        {
            var imageName = Guid.NewGuid();

            var imageSource = _configuration.GetValue<string>("AWS:Image-Source");

            var category = await _context.categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);

            product.Name = model.ProductName;
            product.Available = model.Available;
            product.CategoryId = category.CategoryId;
            product.Description = model.Description;
            product.ProductOption = await _context.productOptions.FirstOrDefaultAsync(x => x.Name == model.ProductOptionName);
            if (model.File != null)
            {
                product.Image = imageSource + imageName.ToString();
                await UploadImage(model.File, imageName.ToString());
            }

            if (product.Discount > 0)
            {
                product.Price = model.Price - product.Discount;
            }
            else
            {
                product.Price = model.Price;
            }


            await _context.SaveChangesAsync();

            return "Ok";
        }

        public async Task<bool> UploadImage(IFormFile file, string imageName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync("images-shop-angular");
            if (!bucketExists)
                return false;
            var request = new PutObjectRequest()
            {
                BucketName = "images-shop-angular",
                Key = string.IsNullOrEmpty("images") ? imageName : $"{"images"?.TrimEnd('/')}/{imageName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
            return true;
        }

        public List<ProductDTO> AllProductsDTO()
        {
            _context.products.Load();
            _context.categories.Load();
            _context.productOptions.Load();

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
                    Image = item.Image,
                    ProductOption = item.ProductOption
                });
            }
            return productDPOs;
        }

        public async Task<Product> GetOneProductWithAll(Guid productId)
        {
            return  await _context.products.
                Where(x => x.ProductId == productId).
                Include(x => x.Category).
                FirstOrDefaultAsync();
        }

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

