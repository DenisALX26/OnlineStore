using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreApp.Models
{
    public class FAQ
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Answer { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

