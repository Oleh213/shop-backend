using System;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    public class ProductModel
    {
        public string ProductName { get; set; }

        public int Available { get; set; }

        public int Price { get; set; }

        public int Weight { get; set; }

        public string Description { get; set; }
                      
        public string CategoryName { get; set; }

        public IFormFile File { get; set; }

        public string ProductOptionName { get; set; }
    }
}

