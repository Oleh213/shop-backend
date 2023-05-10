using System;
using Microsoft.EntityFrameworkCore;
using WebShop.Main.Conext;
using WebShop.Main.DBContext;
using WebShop.Main.Interfaces;

namespace WebShop.Main.BusinessLogic
{
    public class PromocodeActionsBL : IPromocodeActionsBL
    {
        private ShopContext _context;

        public PromocodeActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<Promocode> GetPromocode(string code)
        {
            return await _context.promocodes.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}

