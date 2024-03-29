﻿using System;
using WebShop.Main.Conext;
using WebShop.Main.Context;

namespace WebShop.Models
{
	public class DeliveryOptionsModel
	{
        public string Address { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string DeliveryTime { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public DeliveryTimeOptions DeliveryTimeOptions { get; set; }

    }
}

