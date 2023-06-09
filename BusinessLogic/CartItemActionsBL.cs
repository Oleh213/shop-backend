using System;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using sushi_backend.Context;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.DTO;
using WebShop.Main.Interfaces;
using WebShop.Models;

namespace WebShop.Main.BusinessLogic
{
    public class CartItemActionsBL : ICartItemActionsBL
    {
        private readonly ShopContext _context;

        private readonly IConfiguration _configuration;

        public CartItemActionsBL(ShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<CartItemDTO>> CartItemsDTO(List<CartItemModel> cartOfUser)
        {
            var imageSource = _configuration.GetValue<string>("AWS:Image-Source") + "products-main/";

            if (cartOfUser != null)
            {
                var newCartOfUsers = new List<CartItemDTO>();

                var product = await _context.products.ToListAsync();
                foreach (var cartItem in cartOfUser)
                {
                    var item = product.FirstOrDefault(x=> x.ProductId == cartItem.ProductId);
                    newCartOfUsers.Add(new CartItemDTO
                    {
                        ProductName = item.Name,
                        ProductId = item.ProductId,
                        Image = imageSource + item.Image,
                        Price = item.Price,
                        Available = item.Available,
                        Weight = item.Weight,
                        Count = cartItem.Count,
                    });
                }
                return (newCartOfUsers);
            }
            else
                return null;
        }
    }
}

