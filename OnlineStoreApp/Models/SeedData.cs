using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;

namespace OnlineStoreApp.Models;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
    serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
        
        // Seed Roles and Users only if they don't exist
        if (!context.Roles.Any())
        {

            context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    Name = "Colaborator",
                    NormalizedName = "Colaborator".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper()
                }
            );

            var hasher = new PasswordHasher<ApplicationUser>();

            context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                PasswordHash = hasher.HashPassword(
                    new ApplicationUser { UserName = "admin@test.com", Wishlist = new Wishlist(), Cart = new Cart() }, "Admin123!"),
                    Wishlist = new Wishlist(),
                    Cart = new Cart()
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb10",
                    UserName = "colaborator@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "COLABORATOR@TEST.COM",
                    Email = "colaborator@test.com",
                    NormalizedUserName = "COLABORATOR@TEST.COM",
                PasswordHash = hasher.HashPassword(
                    new ApplicationUser { UserName = "colaborator@test.com", Wishlist = new Wishlist(), Cart = new Cart()    }, "Colaborator123!"),
                    Wishlist = new Wishlist(),
                    Cart = new Cart()
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb11",
                    UserName = "customer@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "CUSTOMER@TEST.COM",
                    Email = "customer@test.com",
                    NormalizedUserName = "CUSTOMER@TEST.COM",
                PasswordHash = hasher.HashPassword(
                    new ApplicationUser { UserName = "customer@test.com", Wishlist = new Wishlist(), Cart = new Cart() }, "Customer123!"),
                    Wishlist = new Wishlist(),
                    Cart = new Cart()

                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb12",
                    UserName = "guest@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "GUEST@TEST.COM",
                    Email = "guest@test.com",
                    NormalizedUserName = "GUEST@TEST.COM",
                PasswordHash = hasher.HashPassword(
                    new ApplicationUser { UserName = "guest@test.com", Wishlist = new Wishlist(), Cart = new Cart() }, "Guest123!"),
                    Wishlist = new Wishlist(),
                    Cart = new Cart()
                }
            // new ApplicationUser
            // {
            //     Id = "8e445865-a24d-4543-a6c6-9443d048cdb12",
            //     UserName = "guest@test.com",
            //     EmailConfirmed = true,
            //     NormalizedEmail = "GUEST@TEST.COM",
            //     Email = "guest@test.com",
            //     NormalizedUserName = "GUEST@TEST.COM",
            //     PasswordHash = hasher.HashPassword(null, "Guest123!"),
            //     Wishlist = new Wishlist(),
            //     Cart = new Cart()
            // }
            );

            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb10"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb11"
            }
        );

            context.SaveChanges();
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Type = "Sneakers" },
                new Category { Type = "Boots" },
                new Category { Type = "Sandals" },
                new Category { Type = "Running Shoes" },
                new Category { Type = "Casual Shoes" },
                new Category { Type = "Formal Shoes" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        // Seed Products
        if (!context.Products.Any())
        {
            var categories = context.Categories.ToList();
            var sneakersCat = categories.FirstOrDefault(c => c.Type == "Sneakers");
            var bootsCat = categories.FirstOrDefault(c => c.Type == "Boots");
            var sandalsCat = categories.FirstOrDefault(c => c.Type == "Sandals");
            var runningCat = categories.FirstOrDefault(c => c.Type == "Running Shoes");
            var casualCat = categories.FirstOrDefault(c => c.Type == "Casual Shoes");
            var formalCat = categories.FirstOrDefault(c => c.Type == "Formal Shoes");

            if (sneakersCat == null || bootsCat == null || sandalsCat == null ||
                runningCat == null || casualCat == null || formalCat == null)
            {
                throw new InvalidOperationException("One or more required categories are missing.");
            }

            var products = new List<Product>
            {
                // Sneakers
                new Product
                {
                    Title = "Classic White Sneakers",
                    Description = "Comfortable and stylish white sneakers perfect for everyday wear. Made with premium materials for durability and comfort.",
                    Price = 299.99,
                    Type = Type.Left,
                    Image = "/images/classic-white-sneakers.jpg",
                    Rating = 4.5,
                    Stock = 50,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Black High-Top Sneakers",
                    Description = "Modern black high-top sneakers with excellent ankle support. Perfect for urban style and casual outings.",
                    Price = 349.99,
                    Type = Type.Left,
                    Image = "/images/black-hightop-sneakers.jpg",
                    Rating = 4.7,
                    Stock = 35,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Retro Style Sneakers",
                    Description = "Vintage-inspired sneakers with bold colors and classic design. Perfect for making a statement.",
                    Price = 279.99,
                    Type = Type.Left,
                    Image = "/images/retro-sneakers.jpg",
                    Rating = 4.6,
                    Stock = 42,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Minimalist White Sneakers",
                    Description = "Clean and simple design with maximum comfort. Perfect for those who prefer understated elegance.",
                    Price = 249.99,
                    Type = Type.Left,
                    Image = "/images/minimalist-white-sneakers.jpg",
                    Rating = 4.4,
                    Stock = 55,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Boots
                new Product
                {
                    Title = "Leather Ankle Boots",
                    Description = "Premium leather ankle boots with classic design. Ideal for autumn and winter seasons.",
                    Price = 599.99,
                    Type = Type.Left,
                    Image = "/images/leather-ankle-boots.jpg",
                    Rating = 4.8,
                    Stock = 25,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Combat Boots",
                    Description = "Durable combat boots with rugged design. Perfect for outdoor activities and tough terrain.",
                    Price = 449.99,
                    Type = Type.Left,
                    Image = "/images/combat-boots.jpg",
                    Rating = 4.6,
                    Stock = 30,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Chelsea Boots",
                    Description = "Elegant Chelsea boots with elastic side panels. Versatile and stylish for any occasion.",
                    Price = 529.99,
                    Type = Type.Left,
                    Image = "/images/chelsea-boots.jpg",
                    Rating = 4.7,
                    Stock = 28,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Running Shoes
                new Product
                {
                    Title = "Ultra Lightweight Running Shoes",
                    Description = "Professional running shoes with advanced cushioning technology. Designed for maximum performance and comfort.",
                    Price = 499.99,
                    Type = Type.Left,
                    Image = "/images/ultra-lightweight-running.jpg",
                    Rating = 4.9,
                    Stock = 40,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Trail Running Shoes",
                    Description = "Rugged trail running shoes with excellent grip and stability. Perfect for off-road running adventures.",
                    Price = 549.99,
                    Type = Type.Left,
                    Image = "/images/trail-running-shoes.jpg",
                    Rating = 4.7,
                    Stock = 28,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Marathon Running Shoes",
                    Description = "High-performance shoes designed for long-distance running. Lightweight with superior energy return.",
                    Price = 579.99,
                    Type = Type.Left,
                    Image = "/images/marathon-running-shoes.jpg",
                    Rating = 4.8,
                    Stock = 32,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Casual Shoes
                new Product
                {
                    Title = "Canvas Slip-On Shoes",
                    Description = "Comfortable canvas slip-on shoes in various colors. Easy to wear and perfect for casual occasions.",
                    Price = 199.99,
                    Type = Type.Left,
                    Image = "/images/canvas-slipon.jpg",
                    Rating = 4.3,
                    Stock = 60,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Loafers",
                    Description = "Classic leather loafers with timeless design. Perfect for smart casual occasions.",
                    Price = 379.99,
                    Type = Type.Left,
                    Image = "/images/loafers.jpg",
                    Rating = 4.5,
                    Stock = 38,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Sandals
                new Product
                {
                    Title = "Summer Beach Sandals",
                    Description = "Lightweight and breathable sandals perfect for summer. Water-resistant and comfortable for all-day wear.",
                    Price = 149.99,
                    Type = Type.Left,
                    Image = "/images/beach-sandals.jpg",
                    Rating = 4.4,
                    Stock = 45,
                    CategoryId = sandalsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Sport Sandals",
                    Description = "Durable sport sandals with excellent grip. Perfect for hiking and outdoor activities.",
                    Price = 229.99,
                    Type = Type.Left,
                    Image = "/images/sport-sandals.jpg",
                    Rating = 4.5,
                    Stock = 33,
                    CategoryId = sandalsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Formal Shoes
                new Product
                {
                    Title = "Classic Oxford Dress Shoes",
                    Description = "Elegant oxford dress shoes made from genuine leather. Perfect for business and formal occasions.",
                    Price = 699.99,
                    Type = Type.Left,
                    Image = "/images/oxford-dress-shoes.jpg",
                    Rating = 4.8,
                    Stock = 20,
                    CategoryId = formalCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new Product
                {
                    Title = "Derby Shoes",
                    Description = "Sophisticated derby shoes with open lacing system. Comfortable yet elegant for formal wear.",
                    Price = 649.99,
                    Type = Type.Left,
                    Image = "/images/derby-shoes.jpg",
                    Rating = 4.7,
                    Stock = 22,
                    CategoryId = formalCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Seed Reviews with text and ratings
            var savedProducts = context.Products.ToList();
            var customers = context.Users.Where(u => u.UserName.Contains("customer") || u.UserName.Contains("guest")).ToList();

            var reviews = new List<Review>();
            var random = new Random();
            
            var reviewComments = new[]
            {
                "Excellent quality! Very comfortable and durable. Highly recommend!",
                "Great product, exactly as described. Fast shipping too.",
                "Good value for money. Comfortable fit and nice design.",
                "Amazing shoes! Perfect for my needs. Will buy again.",
                "Very satisfied with this purchase. Quality is outstanding.",
                "Good product but could be better. Still worth the price.",
                "Comfortable and stylish. Perfect for everyday use.",
                "Love these shoes! Great quality and perfect fit.",
                "Not bad, but expected a bit more for this price.",
                "Excellent craftsmanship. Very happy with my purchase.",
                "Great shoes, very comfortable. Would recommend to friends.",
                "Good quality product. Met my expectations.",
                "Perfect fit and great design. Very satisfied!",
                "Nice product, good quality materials used.",
                "Excellent purchase! Exceeded my expectations.",
                "Very comfortable and stylish. Great value!",
                "Good product overall. Happy with the quality.",
                "Amazing quality! Worth every penny.",
                "Nice design and comfortable. Recommended!",
                "Great shoes, perfect for my lifestyle."
            };

            foreach (var product in savedProducts)
            {
                // Add 3-6 reviews per product
                int reviewCount = random.Next(3, 7);
                var usedCustomers = new HashSet<int>();
                
                for (int i = 0; i < reviewCount && usedCustomers.Count < customers.Count; i++)
                {
                    int customerIndex;
                    do
                    {
                        customerIndex = random.Next(customers.Count);
                    } while (usedCustomers.Contains(customerIndex));
                    
                    usedCustomers.Add(customerIndex);
                    
                    int rating = random.Next(3, 6); // Rating between 3-5
                    string comment = reviewComments[random.Next(reviewComments.Length)];
                    
                    reviews.Add(new Review
                    {
                        ProductId = product.Id,
                        UserId = customers[customerIndex].Id,
                        Comment = comment,
                        Rating = rating,
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 90))
                    });
                }
            }

            context.Reviews.AddRange(reviews);
            context.SaveChanges();

            // Update product ratings based on actual review ratings
            foreach (var product in savedProducts)
            {
                var productReviews = context.Reviews.Where(r => r.ProductId == product.Id).ToList();
                if (productReviews.Any())
                {
                    product.Rating = productReviews.Average(r => r.Rating);
                }
            }

            context.SaveChanges();
        }
    }
}
