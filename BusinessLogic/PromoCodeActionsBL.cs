using System;
using Google.Rpc;
using Microsoft.EntityFrameworkCore;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;

namespace WebShop.Main.BusinessLogic
{
    public class PromoCodeActionsBL : IPromoCodeActionsBL
    {
        private ShopContext _context;

        public PromoCodeActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<Promocode> GetPromocode(string code)
            =>  await _context.promocodes.FirstOrDefaultAsync(x => x.Code == code);

        public async Task<Promocode> GetPromocodeById(Guid promoCodeId)
            => await _context.promocodes.FirstOrDefaultAsync(x => x.PromocodetId == promoCodeId);

        public async Task<List<Promocode>> GetAllPromocodes()
            => await _context.promocodes.ToListAsync();

        public async Task<User> GetUser(Guid userId)
            => await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<bool> CheckPromo(string promoName)
            => await _context.promocodes.AnyAsync(x => x.Code == promoName);


        public async Task<bool> DeletePromocode(Promocode promocode)
        {
            _context.promocodes.Remove(promocode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditPromoCode(Promocode promocode, Promocode newPromo)
        {
            promocode.Code = newPromo.Code;
            promocode.Count = newPromo.Count;
            promocode.Discount = newPromo.Discount;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddNewPromo(AddNewPromoModel model)
        {
            await _context.promocodes.AddAsync(new Promocode
            {
                Code = model.Code,
                Count = model.Count,
                Discount = model.Discount,
            });

            await _context.SaveChangesAsync();

            return true;
        }

    }
}

