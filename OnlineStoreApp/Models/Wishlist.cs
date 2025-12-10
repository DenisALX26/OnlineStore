using System.ComponentModel.DataAnnotations;

namespace OnlineStoreApp.Models;

public class Wishlist
{
    [Key]
    public int Id { get; set; }
    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<WishlistProduct>? WishlistProducts { get; set; }

}
