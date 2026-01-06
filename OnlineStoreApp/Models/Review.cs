using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreApp.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; } 
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
