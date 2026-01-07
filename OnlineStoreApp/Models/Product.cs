using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreApp.Models
{
    public enum Type
    {
        [Display(Name = "Left Foot")]
        Left = 1,

        [Display(Name = "Right Foot")]
        Right = 2
    }
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public double Price { get; set; }
        public Type Type { get; set; }
        public string Image { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Stock { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }   
        public ICollection<WishlistProduct>? WishlistProducts { get; set; }
        public ICollection<CartProduct>? CartProducts { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByUser { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
    }
}
