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
        
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [Display(Name = "Titlu")]
        public required string Title { get; set; }
        
        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [Display(Name = "Descriere")]
        public required string Description { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Prețul trebuie să fie mai mare decât 0")]
        [Display(Name = "Preț")]
        public double Price { get; set; }
        
        [Display(Name = "Tip")]
        public Type Type { get; set; }
        
        [Display(Name = "Imagine")]
        public string Image { get; set; } = string.Empty;
        
        [Display(Name = "Rating")]
        public double Rating { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stocul nu poate fi negativ")]
        [Display(Name = "Stoc")]
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
