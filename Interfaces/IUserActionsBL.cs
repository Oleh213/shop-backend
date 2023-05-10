using System;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Main.DTO;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IUserActionsBL
	{
        Task<User> GetUser(Guid userId);

        Task<bool> CheckName(string name);

        Task<bool> CheckEmail(string email);

        Task<string> ChangeUserInfo(UserInfoModel model, User user);

        UserDTO UserDTO(User user);

        Task<UserInfoModel> UserInfoDTO(User user);

    }
}

