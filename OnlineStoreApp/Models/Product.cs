using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }

    }
}
