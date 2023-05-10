using System;
using System.ComponentModel.DataAnnotations;
using WebShop.Main.Conext;

namespace WebShop.Models
{
	public class OrderModel
	{
        public List<CartItemModel> cartItems { get; set; }

		public ContactInfo contactInfo { get; set; }

		public DeliveryOptionsModel deliveryInfo { get; set; }

        public PaymentMethod paymentMethod { get; set; }

        public PromoCodeOrderModel promoCode { get; set; }
    }
}

