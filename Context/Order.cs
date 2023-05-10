using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text.Json.Serialization;
using WebShop.Main.Context;
using WebShop.Models;

namespace WebShop.Main.Conext
{
    public class Order 
    {
        public Guid OrderId { get; set; }

        public double TotalPrice { get; set; }

        public bool PromoUsed { get; set; }

        public string UsedPromoCode { get; set; }

        public DateTime OrderTime { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public int OrderNumber { get; set; }

        public string? OrderMessage { get; set; }

        public DeliveryOptions DeliveryOptions { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public ICollection<OrderList> OrderLists { get; set; }
    }

    public class PromoCodeOrderModel
    {
        public bool PromoUsed { get; set; }

        public string UsedPromoCode { get; set; }
    }
    
    public enum PaymentMethod
    {
        Cash,
        CardInStore,
        CardOnline,
    }


    public class ContactInfo
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

    }

    public enum OrderStatus
    {
        AwaitingConfirm,
        Cooking,
        Delivered,
        AwaitingPicUp,
        Completed,
        Declined,
        Refunded,
        Canceled,
        AwaitingPayment,
    }

}





