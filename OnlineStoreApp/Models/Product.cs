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
        
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title")]
        public required string Title { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public required string Description { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public double Price { get; set; }
        
        [Display(Name = "Type")]
        public Type Type { get; set; }
        
        [Display(Name = "Image")]
        public string Image { get; set; } = string.Empty;
        
        [Display(Name = "Rating")]
        public double Rating { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }   
        public ICollection<WishlistProduct>? WishlistProducts { get; set; }
        public ICollection<CartProduct>? CartProducts { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public ApplicationUser? CreatedByUser { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public ICollection<FAQ>? FAQs { get; set; }

    }
}
