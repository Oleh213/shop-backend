using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebShop.Main.Conext;

namespace WebShop.Main.Context
{
    public class DeliveryOptions
    {
        public Guid OrderId { get; set; }

        public Guid DeliveryOptionsId { get; set; }

        public string Address { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public DeliveryTimeOptions DeliveryTimeOptions { get; set; }

        public string DeliveryTime { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
    }

    public enum DeliveryType
    {
        OnAddress,
        PicUp,
    }

    public enum DeliveryTimeOptions
    {
        Asap,
        OnTime,
    }
}


