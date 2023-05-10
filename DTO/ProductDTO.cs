using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebShop.Main.Context;

namespace WebShop.Main.DTO
{
	public class ProductDTO
	{
        public Guid ProductId { get; set; }

        public int Price { get; set; }

        public int Weight { get; set; }

        public string CategoryName { get; set; }

        public Guid CategoryId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public int Available { get; set; }

        public int Discount { get; set; }

        public string? Image { get; set; }
    }
}

