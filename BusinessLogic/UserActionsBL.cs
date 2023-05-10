using System;
using Microsoft.EntityFrameworkCore;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DBContext;
using WebShop.Main.DTO;
using WebShop.Main.Interfaces;
using WebShop.Models;

namespace WebShop.Main.BusinessLogic
{
	public class UserActionsBL : IUserActionsBL
    {
        private ShopContext _context;

        public UserActionsBL(ShopContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _context.users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> CheckName(string name)
        {
            return await _context.users.AnyAsync(x => x.Name == name);
        }

        public async Task<bool> CheckEmail(string email)
        {
            return await _context.users.AnyAsync(x => x.Email == email);
        }

        public async Task<string> ChangeUserInfo(UserInfoModel model, User user)
        {
            user.Name = model.Name;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber.ToString();
            await _context.SaveChangesAsync();

            return "Ok";
        }

        public UserDTO UserDTO(User user)
        {
            byte permition = 1;

            if (user.Role == UserRole.Admin)
            {
                permition = 0;
            }
            else if (user.Role == UserRole.Manager)
            {
                permition = 2;
            }

            var userId = new UserDTO { UserId = user.UserId, UserRole = permition };

            return userId;
        }


        public async Task<UserInfoModel> UserInfoDTO(User user)
        {
            if (user.LastName == null)
            {
                user.LastName = "Not specified";
            }
            if (user.Email == null)
            {
                user.Email = "Not specified";
            }
            if (user.PhoneNumber == null)
            {
                user.PhoneNumber = "Not specified";
            }

            var outUser = new UserInfoModel()
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            await _context.SaveChangesAsync();

            return outUser;
        }
    }
}

