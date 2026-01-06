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
    public DbSet<Category> Categories { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<WishlistProduct> WishlistProducts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

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

        // 1:1 For Wishlist and ApplicationUser
        builder.Entity<ApplicationUser>().HasOne<Wishlist>(a => a.Wishlist)
            .WithOne(b => b.User)
            .HasForeignKey<Wishlist>(b => b.UserId);

        // 1:1 For Cart and ApplicationUser
        builder.Entity<ApplicationUser>().HasOne<Cart>(a => a.Cart)
            .WithOne(b => b.User)
            .HasForeignKey<Cart>(b => b.UserId);

        // 1:M For Category and Product
        builder.Entity<Product>().HasOne<Category>(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        // M:1 For Review and Product
        builder.Entity<Review>().HasOne<Product>(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId);

        // M:1 For FAQ and Product
        builder.Entity<FAQ>().HasOne<Product>(f => f.Product)
            .WithMany(p => p.FAQs)
            .HasForeignKey(f => f.ProductId);

        // M:N For Wishlist and Product through WishlistProduct
        builder.Entity<WishlistProduct>()
            .HasKey(wp => new { wp.WishlistId, wp.ProductId });

        builder.Entity<WishlistProduct>()
            .HasOne<Wishlist>(wp => wp.Wishlist)
            .WithMany(w => w.WishlistProducts)
            .HasForeignKey(wp => wp.WishlistId);

        builder.Entity<WishlistProduct>()
            .HasOne<Product>(wp => wp.Product)
            .WithMany(p => p.WishlistProducts)
            .HasForeignKey(wp => wp.ProductId);

        // M:N For Cart and Product through CartProduct
        builder.Entity<CartProduct>()
            .HasKey(cp => new { cp.CartId, cp.ProductId });

        builder.Entity<CartProduct>()
            .HasOne<Cart>(cp => cp.Cart)
            .WithMany(c => c.CartProducts)
            .HasForeignKey(cp => cp.CartId);

        builder.Entity<CartProduct>()
            .HasOne<Product>(cp => cp.Product)
            .WithMany(p => p.CartProducts)
            .HasForeignKey(cp => cp.ProductId);

        builder.Entity<Proposal>()
            .HasOne<ApplicationUser>(p => p.User)
            .WithMany(u => u.Proposals)
            .HasForeignKey(p => p.UserId);

        // 1:M For Order and ApplicationUser
        builder.Entity<Order>()
            .HasOne<ApplicationUser>(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        // 1:M For OrderItem and Order
        builder.Entity<OrderItem>()
            .HasOne<Order>(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        // M:1 For OrderItem and Product
        builder.Entity<OrderItem>()
            .HasOne<Product>(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
    }

}
