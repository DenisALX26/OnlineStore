namespace OnlineStoreApp.Models;

public class WishlistProduct
{
    public int ProductId { get; set; }
    public int WishlistId { get; set; }
    public virtual Product? Product{ get; set; }
    public virtual Wishlist? Wishlist { get; set; }
}
