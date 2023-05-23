using System;
using sushi_backend.Context;

namespace sushi_backend.Models
{
	public class EditProductModel
	{
        public Guid ProductId { get; set; }

        public int Price { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public int Available { get; set; }

        public int Discount { get; set; }

        public int Weight { get; set; }

        public string? ProductOptionName { get; set; }

        public string CategoryName { get; set; }

        public IFormFile? File { get; set; }
    }
}

