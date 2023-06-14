using System;
using WebShop.Main.Conext;
using WebShop.Main.Hubs;

namespace sushi_backend.Hubs
{
	public interface IOrderHub
	{
        Task MakeOrder(Order orderModel);

        Task AddToGroup(OrderTest order);

        Task AddToAdmins();
    }
}

