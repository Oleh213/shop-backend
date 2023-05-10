using System;
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

        public CartItemActionsBL(ShopContext context)
        {
            _context = context;
        }

        public List<CartItemDTO> CartItemsDTO(List<CartItemModel> cartOfUser)
        {
            if (cartOfUser != null)
            {
                var newCartOfUsers = new List<CartItemDTO>();

                var product = _context.products;
                foreach (var cartItem in cartOfUser)
                {
                    var item = product.FirstOrDefault(x=> x.ProductId == cartItem.ProductId);
                    newCartOfUsers.Add(new CartItemDTO
                    {
                        ProductName = item.Name,
                        ProductId = item.ProductId,
                        Image = item.Image,
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

