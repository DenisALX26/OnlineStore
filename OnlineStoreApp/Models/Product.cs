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
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }   
        public ICollection<WishlistProduct>? WishlistProducts { get; set; }
        public ICollection<CartProduct>? CartProducts { get; set; }

    }
}
