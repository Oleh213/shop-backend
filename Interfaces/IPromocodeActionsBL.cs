using System;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IPromoCodeActionsBL
	{
        Task<Promocode> GetPromocode(string code);

        Task<Promocode> GetPromocodeById(Guid promoCodeId);
        Task<User> GetUser(Guid userId);

        Task<List<Promocode>> GetAllPromocodes();

        Task<bool> AddNewPromo(AddNewPromoModel model);

        Task<bool> CheckPromo(string promoName);

        Task<bool> EditPromoCode(Promocode promocode, Promocode newPromo);

        Task<bool> DeletePromocode(Promocode promocode);
    }
}

