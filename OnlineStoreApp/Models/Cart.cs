using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStoreApp.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
