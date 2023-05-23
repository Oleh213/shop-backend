using System;
using Microsoft.EntityFrameworkCore;
using sushi_backend.Context;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;

namespace WebShop.Main.BusinessLogic
{
	public class DiscountActionsBL : IDiscountActionsBL
    {
        private readonly ShopContext _context;

        public DiscountActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            return await _context.products.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<bool> ApplyDiscount(Product product, int discount)
        {
            if (product.Discount > 0)
            {
                product.Price += product.Discount;
            }

            if (product.Price > discount)
            {
                product.Price -= discount;
                product.Discount = discount;
            }
            else
            {
                product.Discount = product.Price - 1;
                product.Price = 1;
            }

            await _context.SaveChangesAsync();
            return true;



        }

        public async Task<string> ClearDiscount(Product product)
        {
            product.Price += product.Discount;
            product.Discount = 0;

            await _context.SaveChangesAsync();

            return "Ok";
        }
    }
}

