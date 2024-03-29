﻿using System;
namespace sushi_backend.Context
{
    public class Product
    {
        public Guid ProductId { get; set; }

        public int Price { get; set; }

        public Guid CategoryId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public int Available { get; set; }

        public int Discount { get; set; }

        public int Weight { get; set; }

        public Guid? ProductOptionsId { get; set; }

        public ProductOption? ProductOption { get; set; }

        public Category Category { get; set; }

        public string ImagePreview { get; set; }

        public string Image { get; set; }

        public string? Items { get; set; }
    }
}

