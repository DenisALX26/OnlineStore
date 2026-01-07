namespace OnlineStoreApp.Models;

public class CartProduct
{
    public int ProductId { get; set; }
    public int CartId { get; set; }
    public int Quantity { get; set; } = 1; // Default quantity is 1
    public virtual Product? Product{ get; set; }
    public virtual Cart? Cart { get; set; }
}
