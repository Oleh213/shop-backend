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
                ProductName = model.ProductName,
                Available = model.Available,
                Price = model.Price,
                ProductId = id,
                Description = model.Description,
                CategoryId = category.CategoryId,
                Image = model.File.FileName,
                ImagePreview = model.File.FileName,
                Weight = model.Weight
            });

            await UploadImage(model.File, model.File.FileName, "products-main");

            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<string> UpdateProduct(EditProductModel model, Product product)
        {
            var imageName = Guid.NewGuid();

            var category = await _context.categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);

            await DeleteImage(product.Image);

            var option = await _context.productOptions.FirstOrDefaultAsync(x => x.Name == model.ProductOptionName);

            if (option != null)
            {
                product.ProductOptionsId = option.ProductOptionsId;
                product.ProductOption = option;
            }
            else
            {
                product.ProductOption = option;
                product.ProductOptionsId = null;
            }
            product.ProductName = model.ProductName;
            product.Available = model.Available;
            product.CategoryId = category.CategoryId;
            product.Description = model.Description;
            product.Weight = model.Weight;

            product.ProductOption = await _context.productOptions.FirstOrDefaultAsync(x => x.Name == model.ProductOptionName);
            if (model.File != null)
            {
                product.Image = model.File.FileName;
                await UploadImage(model.File, model.File.FileName, "products-main");
            }

            product.Price = product.Discount > 0 ?  model.Price - product.Discount : model.Price;

            await _context.SaveChangesAsync();

            return "Ok";
        }

        public async Task<bool> UploadImage(IFormFile file, string imageName,string folderName)
        {
            using (var client = new AmazonS3Client("AKIATEKBWQQJRIHN2JDQ", "dNdvJlUgnOeq2EswfIuOOSqAr9nkb0iG+wIgBK/a", RegionEndpoint.EUWest1))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = string.IsNullOrEmpty(folderName) ? imageName : $"{folderName?.TrimEnd('/')}/{imageName}",
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

            var imageSource = _configuration.GetValue<string>("AWS:Image-Source") + "products-main/";

            var productDPOs = new List<ProductDTO>();

            foreach (var item in _context.products)
            {
                productDPOs.Add(new ProductDTO
                {
                    ProductId = item.ProductId,
                    Price = item.Price,
                    CategoryName = item.Category.CategoryName,
                    CategoryId = item.CategoryId,
                    ProductName = item.ProductName,
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
        

        public async Task<ProductDTO> OneProductsDTO(Product product)
        {
            var imageSource = _configuration.GetValue<string>("AWS:Image-Source") + "products-main/";

            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                Price = product.Price,
                CategoryName = product.Category.CategoryName,
                CategoryId = product.CategoryId,
                ProductName = product.ProductName,
                Weight = product.Weight,
                ProductOption = product.ProductOption,
                Available = product.Available,
                Discount = product.Discount,
                Description = product.Description,
                Image = imageSource + product.Image,
            };
            if(product.Items != null && product.Items.Length> 0)
            {
                productDTO.Items = await GetProductItems(product.Items);
            }
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

        public async Task<List<Product>> GetProductItems(string items)
        {
            var guidArray = items.Split('|', StringSplitOptions.RemoveEmptyEntries);
            var imageSource = _configuration.GetValue<string>("AWS:Image-Source") + "products-main/";

            var itemsList = new List<Guid>();
            var itemsProducts = new List<Product>();
            var products = await _context.products.ToListAsync();

            foreach (string guid in guidArray)
            {
                if (Guid.TryParse(guid.Trim(), out Guid parsedGuid))
                {
                    itemsList.Add(parsedGuid);
                    var product = products.FirstOrDefault(x => x.ProductId == parsedGuid);
                    product.Image = imageSource + product.Image;
                    itemsProducts.Add(product);
                }
            }
            return itemsProducts;
        }

        public async Task<bool> AddItemToProduct(Guid itemId, Guid productId)
        {
            var product = await _context.products.FirstOrDefaultAsync(x=> x.ProductId == productId);



            if (product.Items == null)
            {
                product.Items = itemId.ToString();
            }
            else
            {
                product.Items += " | " + itemId.ToString();
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DellItemToProduct(Guid itemId, Guid productId)
        {
            var product = await _context.products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product.Items != null)
            {
                var guidArray = product.Items.Split('|', StringSplitOptions.RemoveEmptyEntries);

                var itemsList = new List<Guid>();

                foreach (string guid in guidArray)
                {
                    if (Guid.TryParse(guid.Trim(), out Guid parsedGuid))
                    {
                        itemsList.Add(parsedGuid);
                    }
                }
                if (itemsList.Any(x => x == itemId))
                {
                    itemsList.Remove(itemId);

                    product.Items = string.Join("|", itemsList);

                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            return false;
        }
    }
}

