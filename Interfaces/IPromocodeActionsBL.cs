using System;
using WebShop.Main.Conext;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IPromocodeActionsBL
	{
        Task<Promocode> GetPromocode(string code);
    }
}

