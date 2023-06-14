using System;
using System.CodeDom;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using sushi_backend.Hubs;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Models;

namespace WebShop.Main.Hubs
{
	public class OrderHub:Hub<IOrderHub>
    {
        //public async Task Enter(string username, string orderId)
        //{
        //    if (String.IsNullOrEmpty(username))
        //    {
        //        await Clients.Caller.SendAsync("Notify", "Для входа в чат введите логин");
        //    }
        //    else
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        //        await Clients.Group(orderId).SendAsync("Notify", $"{username} вошел в чат");
        //    }
        //}

        public async Task MakeOrder(Order orderModel)
        {
            await Clients.Group(orderModel.OrderId.ToString()).MakeOrder(orderModel);

            await Clients.Group("admins").MakeOrder(orderModel);
        }

        public async Task AddToGroup(OrderTest order)
            => await Groups.AddToGroupAsync(Context.ConnectionId, order.OrderId);

        public async Task AddToAdmins()
            => await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
    }

    public class OrderTest
    {
        public string OrderId { get; set; }
    }
}

