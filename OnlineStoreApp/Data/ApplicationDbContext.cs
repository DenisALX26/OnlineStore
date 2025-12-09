using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configure string properties for MySQL compatibility
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    var maxLength = property.GetMaxLength();
                    if (maxLength == null)
                    {
                        // Set default max length for string primary keys and foreign keys
                        if (property.IsKey() || property.IsForeignKey())
                        {
                            property.SetMaxLength(255);
                        }
                    }
                }
            }
        }

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().HasOne<Wishlist>(a => a.Wishlist)
            .WithOne(b => b.User)
            .HasForeignKey<Wishlist>(b => b.UserId);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().HasOne<Cart>(a => a.Cart)
            .WithOne(b => b.User)
            .HasForeignKey<Cart>(b => b.UserId);
    }

}
