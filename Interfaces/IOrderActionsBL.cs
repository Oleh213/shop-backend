using System;
using LiqPay.SDK.Dto;
using sushi_backend.Context;
using sushi_backend.Models;
using WebShop.Main.Conext;
using WebShop.Main.Context;
using WebShop.Models;

namespace WebShop.Main.Interfaces
{
	public interface IOrderActionsBL
	{

        Task<OrderResponsModel> CreateNewOrder(List<CartItemModel> cartItems, DeliveryOptionsModel deliveryOptions, PaymentMethod paymentMethod, ContactInfo contactInfo, PromoCodeOrderModel promocode);

        int GetTotalPrice(List<CartItemModel> cartItems);

        bool CheckCountOfProducts(List<CartItemModel> cartItems);

        Task<List<Order>> GetNewOrders();

        Task<Order> GetOrderById(string orderId);

        Task<User> GetUser(Guid userId);

        Task<List<Order>> GetUserOrders(string[] orders);

        Task<Order> GetOrder(Guid orderId);

        Task<string> ChangeOrderStatus(Order order, OrderStatus orderStatus);

        Task<OrderResponsModel> GoToPayment(string email, double amount, Guid orderId);

        bool ConfirmPayment(string data, string signature);
    }
}

