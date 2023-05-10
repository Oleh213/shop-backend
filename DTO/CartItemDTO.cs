using System;
using System.Text.Json.Serialization;
using WebShop.Main.Conext;

namespace WebShop.Main.DTO
{
    public class CartItemDTO
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string Image { get; set; }

        public int Price { get; set; }

        public int Weight { get; set; }

        public int Available { get; set; }

        public int Count { get; set; }
    }
}