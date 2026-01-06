using System.ComponentModel.DataAnnotations;

namespace OnlineStoreApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public double TotalPrice { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
        
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}

