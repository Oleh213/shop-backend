using System;
using System.Text.Json.Serialization;

namespace sushi_backend.Context
{
	public class Category
	{
		public Guid CategoryId { get; set; }

		public string CategoryName { get; set; }

		public string CategoryImage { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
    }
}

