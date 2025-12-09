using Microsoft.AspNetCore.Identity;

namespace OnlineStoreApp.Models;

public class ApplicationUser : IdentityUser
{
    public required virtual Wishlist Wishlist { get; set; } = new Wishlist();
    public required virtual Cart Cart { get; set; } = new Cart();
}
