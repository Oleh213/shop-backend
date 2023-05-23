using System;
using sushi_backend.Context;
using WebShop.Main.Conext;

namespace WebShop.Main.Interfaces
{
	public interface IDiscountActionsBL
	{
        Task<User> GetUser(Guid userId);

        Task<Product> GetProduct(Guid productId);

        Task<bool> ApplyDiscount(Product product, int discount);

        Task<string> ClearDiscount(Product product);
    }
}

