using System;
using sushi_backend.Context;
using sushi_backend.DTO;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DTO;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IProductActionsBL
	{
        Task<User> GetUser(Guid userId);

        Task<bool> CheckCategory(string categoryName);

        Task<Guid> AddProduct(ProductModel model);

        Task<string> UpdateProduct(EditProductModel model, Product product);

        Task<Product> GetProduct(Guid productId);

        List<ProductDTO> AllProductsDTO();

        Task<Product> GetOneProductWithAll(Guid productId);

        Task<ProductDTO> OneProductsDTO(Product product);

        Task<List<ProductOptionDTO>> ProductOptionsDTO();

        Task<bool> UploadImage(IFormFile file, string imageName, string folderName);

        Task<bool> DeleteImage(string imageName);

        Task<List<Product>> GetProductItems(string items);

        Task<bool> DellItemToProduct(Guid itemId, Guid productId);

        Task<bool> AddItemToProduct(Guid itemId, Guid productId);
    }
}