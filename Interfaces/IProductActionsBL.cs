using System;
using sushi_backend.Context;
using sushi_backend.DTO;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DTO;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IProductActionsBL
	{
        Task<User> GetUser(Guid userId);

        Task<bool> CheckCategory(Guid categoryId);

        Task<Guid> AddProduct(ProductModel model);

        Task<string> UpdateProduct(UpdateProductModel model, Product product);

        Task<Product> GetProduct(Guid productId);

        List<ProductDTO> AllProductsDTO();

        Task<Product> GetOneProductWithAll(Guid productId);

        ProductDTO OneProductsDTO(Product product);

        Task<List<ProductOptionDTO>> ProductOptionsDTO();

    }
}