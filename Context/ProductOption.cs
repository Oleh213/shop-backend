using System;
using System.Text.Json.Serialization;

namespace sushi_backend.Context
{
	public class ProductOption
	{
		public Guid ProductOptionsId { get; set; }

		public string Name { get; set; }

		public string Descriptions { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}

