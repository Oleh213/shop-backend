using System;
using WebShop.Main.Conext;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IPromoCodeActionsBL
	{
        Task<Promocode> GetPromocode(string code);
    }
}

