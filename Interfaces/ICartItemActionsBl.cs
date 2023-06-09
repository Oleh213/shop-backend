using System;
using sushi_backend.Context;
using WebShop.Main.Conext;
using WebShop.Main.DTO;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface ICartItemActionsBL
	{
        Task<List<CartItemDTO>> CartItemsDTO(List<CartItemModel> cartOfUser);
    }
}

