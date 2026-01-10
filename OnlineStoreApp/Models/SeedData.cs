using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using System.IO;

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
                    new ApplicationUser { UserName = "colaborator@test.com", Wishlist = new Wishlist(), Cart = new Cart() }, "Colaborator123!"),
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
                new() { Type = "Sneakers" },
                new() { Type = "Boots" },
                new() { Type = "Sandals" },
                new() { Type = "Running Shoes" },
                new() { Type = "Casual Shoes" },
                new() { Type = "Formal Shoes" }
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

            var shoesSides = new[] { Type.Left, Type.Right };

            var products = new List<Product>
            {
                // Real shoes data
                new ()
                {
                    Title = "Nike Air Force 1 Low '07 White",
                    Description = "The Nike Air Force 1 Low '07 in White is a timeless classic that combines style and comfort. Featuring a clean white leather upper, this iconic sneaker offers durability and a sleek look. The encapsulated Air-Sole unit provides cushioning for all-day wear, while the rubber outsole ensures traction on various surfaces. Perfect for everyday wear, the Air Force 1 Low '07 White is a versatile addition to any sneaker collection.",
                    Price = 299.99,
                    Type = Type.Left,
                    Image = "/images/seed/af1_low_white_07.avif",
                    Rating = 4.8,
                    Stock = 100,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },new ()
                {
                    Title = "Nike Air Force 1 Low '07 White",
                    Description = "The Nike Air Force 1 Low '07 in White is a timeless classic that combines style and comfort. Featuring a clean white leather upper, this iconic sneaker offers durability and a sleek look. The encapsulated Air-Sole unit provides cushioning for all-day wear, while the rubber outsole ensures traction on various surfaces. Perfect for everyday wear, the Air Force 1 Low '07 White is a versatile addition to any sneaker collection.",
                    Price = 299.99,
                    Type = Type.Right,
                    Image = "/images/seed/af1_low_white_07.avif",
                    Rating = 4.8,
                    Stock = 100,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new ()
                {
                    Title = "Crocs Classic Clog Lightning McQueen",
                    Description = "The Crocs Classic Clog Lightning McQueen edition brings the fun and excitement of Disney Pixar's Cars to your feet. Featuring a vibrant red design with Lightning McQueen graphics, these clogs are perfect for kids and fans of the movie. Made from Croslite™ material, they provide lightweight comfort and durability. The ventilated design ensures breathability, while the slip-on style makes them easy to wear. Whether for casual outings or playtime, these Crocs add a touch of racing fun to any outfit.",
                    Price = 149.99,
                    Type = Type.Left,
                    Image = "/images/seed/lighting_mcqueen.avif",
                    Rating = 4.5,
                    Stock = 80,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new ()
                {
                    Title = "Crocs Classic Clog Lightning McQueen",
                    Description = "The Crocs Classic Clog Lightning McQueen edition brings the fun and excitement of Disney Pixar's Cars to your feet. Featuring a vibrant red design with Lightning McQueen graphics, these clogs are perfect for kids and fans of the movie. Made from Croslite™ material, they provide lightweight comfort and durability. The ventilated design ensures breathability, while the slip-on style makes them easy to wear. Whether for casual outings or playtime, these Crocs add a touch of racing fun to any outfit.",
                    Price = 149.99,
                    Type = Type.Right,
                    Image = "/images/seed/lighting_mcqueen.avif",
                    Rating = 4.5,
                    Stock = 80,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "adidas Yeezy Boost 350 V2 MX Dark Salt",
                    Description = "The adidas Yeezy Boost 350 V2 MX Dark Salt is a stylish and comfortable sneaker designed in collaboration with Kanye West. Featuring a unique blend of dark and light grey tones, this sneaker offers a modern and versatile look. The Primeknit upper provides a snug and adaptive fit, while the Boost midsole delivers exceptional cushioning and energy return. With its distinctive design and premium materials, the Yeezy Boost 350 V2 MX Dark Salt is perfect for sneaker enthusiasts looking to make a statement.",
                    Price = 279.99,
                    Type = Type.Left,
                    Image = "/images/seed/yeezy_boost_350.avif",
                    Rating = 4.5,
                    Stock = 50,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "adidas Yeezy Boost 350 V2 MX Dark Salt",
                    Description = "The adidas Yeezy Boost 350 V2 MX Dark Salt is a stylish and comfortable sneaker designed in collaboration with Kanye West. Featuring a unique blend of dark and light grey tones, this sneaker offers a modern and versatile look. The Primeknit upper provides a snug and adaptive fit, while the Boost midsole delivers exceptional cushioning and energy return. With its distinctive design and premium materials, the Yeezy Boost 350 V2 MX Dark Salt is perfect for sneaker enthusiasts looking to make a statement.",
                    Price = 279.99,
                    Type = Type.Right,
                    Image = "/images/seed/yeezy_boost_350.avif",
                    Rating = 4.5,
                    Stock = 50,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "adidas Yeezy Foam RNR Onyx",
                    Description = "The adidas Yeezy Foam RNR Onyx is a bold and innovative footwear option designed in collaboration with Kanye West. Featuring a sleek black design, these foam runners offer a futuristic look combined with exceptional comfort. Made from lightweight EVA foam, they provide cushioning and support for all-day wear. The slip-on style ensures easy on and off, while the unique silhouette makes a statement wherever you go. Perfect for casual outings or lounging, the Yeezy Foam RNR Onyx combines style and functionality in one eye-catching package.",
                    Price = 179.99,
                    Type = Type.Right,
                    Image = "/images/seed/yeezy_foam.avif",
                    Rating = 4.2,
                    Stock = 30,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "adidas Yeezy Foam RNR Onyx",
                    Description = "The adidas Yeezy Foam RNR Onyx is a bold and innovative footwear option designed in collaboration with Kanye West. Featuring a sleek black design, these foam runners offer a futuristic look combined with exceptional comfort. Made from lightweight EVA foam, they provide cushioning and support for all-day wear. The slip-on style ensures easy on and off, while the unique silhouette makes a statement wherever you go. Perfect for casual outings or lounging, the Yeezy Foam RNR Onyx combines style and functionality in one eye-catching package.",
                    Price = 179.99,
                    Type = Type.Left,
                    Image = "/images/seed/yeezy_foam.avif",
                    Rating = 4.2,
                    Stock = 30,
                    CategoryId = casualCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "ASICS Gel-1130 White Black",
                    Description = "The ASICS Gel-1130 in White Black is a high-performance running shoe designed for comfort and support. Featuring a breathable white mesh upper with black accents, this shoe offers a sleek and modern look. The GEL cushioning system provides excellent shock absorption, while the durable rubber outsole ensures traction on various surfaces. With its lightweight design and responsive fit, the ASICS Gel-1130 White Black is perfect for runners seeking both style and functionality.",
                    Price = 219.99,
                    Type = Type.Left,
                    Image = "/images/seed/asics.avif",
                    Rating = 4.3,
                    Stock = 40,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "ASICS Gel-1130 White Black",
                    Description = "The ASICS Gel-1130 in White Black is a high-performance running shoe designed for comfort and support. Featuring a breathable white mesh upper with black accents, this shoe offers a sleek and modern look. The GEL cushioning system provides excellent shock absorption, while the durable rubber outsole ensures traction on various surfaces. With its lightweight design and responsive fit, the ASICS Gel-1130 White Black is perfect for runners seeking both style and functionality.",
                    Price = 219.99,
                    Type = Type.Right,
                    Image = "/images/seed/asics.avif",
                    Rating = 4.3,
                    Stock = 40,
                    CategoryId = sneakersCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "Nike Kobe 6 Protro Total Orange",
                    Description = "The Nike Kobe 6 Protro Total Orange is a striking basketball shoe that pays homage to the legendary Kobe Bryant. Featuring a vibrant orange upper with black accents, this shoe offers a bold and dynamic look on the court. The Flywire technology provides a secure fit, while the Zoom Air unit delivers responsive cushioning for quick movements. With its lightweight design and excellent traction, the Nike Kobe 6 Protro Total Orange is perfect for players looking to elevate their game with style and performance.",
                    Price = 799.99,
                    Type = Type.Right,
                    Image = "/images/seed/nike_kobe.avif",
                    Rating = 4.9,
                    Stock = 20,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "Nike Kobe 6 Protro Total Orange",
                    Description = "The Nike Kobe 6 Protro Total Orange is a striking basketball shoe that pays homage to the legendary Kobe Bryant. Featuring a vibrant orange upper with black accents, this shoe offers a bold and dynamic look on the court. The Flywire technology provides a secure fit, while the Zoom Air unit delivers responsive cushioning for quick movements. With its lightweight design and excellent traction, the Nike Kobe 6 Protro Total Orange is perfect for players looking to elevate their game with style and performance.",
                    Price = 799.99,
                    Type = Type.Left,
                    Image = "/images/seed/nike_kobe.avif",
                    Rating = 4.9,
                    Stock = 20,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                // Sneakers
                new()
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
                new()
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
                new()
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
                new()
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
                new()
                {
                    Title = "Timberland 6 Boot Black Nubuck Premium",
                    Description = "The Timberland 6 Boot Black Nubuck Premium is a rugged and stylish boot designed for durability and comfort. Crafted from premium black nubuck leather, these boots feature a waterproof construction to keep your feet dry in wet conditions. The padded collar provides additional ankle support, while the anti-fatigue technology ensures all-day comfort. With its iconic design and sturdy rubber outsole, the Timberland 6 Boot is perfect for outdoor adventures and urban wear alike.",
                    Price = 449.99,
                    Type = Type.Right,
                    Image = "/images/seed/timberland_boot.avif",
                    Rating = 4.9,
                    Stock = 75,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
                {
                    Title = "Timberland 6 Boot Black Nubuck Premium",
                    Description = "The Timberland 6 Boot Black Nubuck Premium is a rugged and stylish boot designed for durability and comfort. Crafted from premium black nubuck leather, these boots feature a waterproof construction to keep your feet dry in wet conditions. The padded collar provides additional ankle support, while the anti-fatigue technology ensures all-day comfort. With its iconic design and sturdy rubber outsole, the Timberland 6 Boot is perfect for outdoor adventures and urban wear alike.",
                    Price = 449.99,
                    Type = Type.Left,
                    Image = "/images/seed/timberland_boot.avif",
                    Rating = 4.9,
                    Stock = 75,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new ()
                {
                    Title = "UGG Mini Bailey Bow II Chestnut",
                    Description = "The UGG Mini Bailey Bow II in Chestnut is a stylish and cozy boot perfect for cooler weather. Featuring a soft sheepskin lining and a durable suede upper, these boots provide warmth and comfort all day long. The signature UGG sole offers excellent traction, while the decorative bow adds a touch of femininity to the classic design. Ideal for casual wear, the Mini Bailey Bow II combines fashion and function in one versatile package.",
                    Price = 309.99,
                    Type = Type.Right,
                    Image = "/images/seed/ugg.avif",
                    Rating = 4.7,
                    Stock = 60,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new ()
                {
                    Title = "UGG Mini Bailey Bow II Chestnut",
                    Description = "The UGG Mini Bailey Bow II in Chestnut is a stylish and cozy boot perfect for cooler weather. Featuring a soft sheepskin lining and a durable suede upper, these boots provide warmth and comfort all day long. The signature UGG sole offers excellent traction, while the decorative bow adds a touch of femininity to the classic design. Ideal for casual wear, the Mini Bailey Bow II combines fashion and function in one versatile package.",
                    Price = 309.99,
                    Type = Type.Left,
                    Image = "/images/seed/ugg.avif",
                    Rating = 4.7,
                    Stock = 60,
                    CategoryId = bootsCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
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
                new()
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
                new()
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
                new ()
                {
                    Title = "Nike Ja 3 Scratch 3.0",
                    Description = "The Nike Ja 3 Scratch 3.0 is a stylish and comfortable running shoe designed for athletes and casual runners alike. Featuring a breathable mesh upper and responsive cushioning, these shoes provide excellent support and comfort during your runs. The durable rubber outsole offers superior traction on various surfaces, making them ideal for both indoor and outdoor workouts. With its sleek design and vibrant colors, the Nike Ja 3 Scratch 3.0 is perfect for those who want to combine performance with style.",
                    Price = 399.99,
                    Type = Type.Right,
                    Image = "/images/seed/niek_ja_3.avif",
                    Rating = 4.9,
                    Stock = 45,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new ()
                {
                    Title = "Nike Ja 3 Scratch 3.0",
                    Description = "The Nike Ja 3 Scratch 3.0 is a stylish and comfortable running shoe designed for athletes and casual runners alike. Featuring a breathable mesh upper and responsive cushioning, these shoes provide excellent support and comfort during your runs. The durable rubber outsole offers superior traction on various surfaces, making them ideal for both indoor and outdoor workouts. With its sleek design and vibrant colors, the Nike Ja 3 Scratch 3.0 is perfect for those who want to combine performance with style.",
                    Price = 399.99,
                    Type = Type.Left,
                    Image = "/images/seed/niek_ja_3.avif",
                    Rating = 4.9,
                    Stock = 45,
                    CategoryId = runningCat.Id,
                    CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    Status = ProductStatus.Active
                },
                new()
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
                new()
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
                new()
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
                new()
                {
                Title = "adidas Yeezy Slide Onyx",
                Description = "The adidas Yeezy Slide Onyx is a sleek and modern slide sandal designed for comfort and style. Featuring a minimalist black design, these slides are made from lightweight EVA foam that provides cushioning and support for all-day wear. The contoured footbed ensures a secure fit, while the textured outsole offers traction on various surfaces. Perfect for casual outings or lounging at home, the Yeezy Slide Onyx combines fashion and function in one versatile package.",
                Price = 269.99,
                Type = Type.Right,
                Image = "/images/seed/adidas_yeezy_slide_onyx.avif",
                Rating = 3.7,
                Stock = 12,
                CategoryId = casualCat.Id,
                CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Status = ProductStatus.Active
                },new()
                {
                Title = "adidas Yeezy Slide Onyx",
                Description = "The adidas Yeezy Slide Onyx is a sleek and modern slide sandal designed for comfort and style. Featuring a minimalist black design, these slides are made from lightweight EVA foam that provides cushioning and support for all-day wear. The contoured footbed ensures a secure fit, while the textured outsole offers traction on various surfaces. Perfect for casual outings or lounging at home, the Yeezy Slide Onyx combines fashion and function in one versatile package.",
                Price = 269.99,
                Type = Type.Left,
                Image = "/images/seed/adidas_yeezy_slide_onyx.avif",
                Rating = 3.7,
                Stock = 12,
                CategoryId = casualCat.Id,
                CreatedByUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Status = ProductStatus.Active
                },
                new()
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
                new()
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
                new()
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
                new()
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
                new()
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
                new()
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
            var customers = context.Users.Where(u => u.UserName != null && (u.UserName.Contains("customer") || u.UserName.Contains("guest"))).ToList();

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
                if (productReviews.Count > 0)
                {
                    product.Rating = productReviews.Average(r => r.Rating);
                }
            }

            context.SaveChanges();

            // Seed FAQs for products
            var faqs = new List<FAQ>();
            var commonFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație." },
                new { Q = "Este potrivit pentru copii?", A = "Acest produs este recomandat pentru adulți. Pentru copii, vă recomandăm să consultați măsurile disponibile." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale, vă rugăm să ne contactați." },
                new { Q = "Este potrivit pentru alergare?", A = "Acest produs este proiectat pentru uz zilnic și confort. Pentru alergare, recomandăm produsele din categoria Running Shoes." },
                new { Q = "Este impermeabil?", A = "Produsul oferă protecție de bază împotriva apei, dar nu este complet impermeabil. Pentru condiții extreme, recomandăm produse specializate." },
                new { Q = "Cum trebuie să îl curăț?", A = "Recomandăm curățarea cu o cârpă umedă și un detergent blând. Evitați mașina de spălat și uscarea la soare direct." },
                new { Q = "Este potrivit pentru iarnă?", A = "Acest produs oferă confort și protecție de bază, dar pentru condiții de iarnă extreme, recomandăm produse specializate din categoria Boots." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal." }
            };

            foreach (var product in savedProducts)
            {
                // Add 3-5 FAQs per product
                int faqCount = random.Next(3, 6);
                var selectedFAQs = commonFAQs.OrderBy(x => random.Next()).Take(faqCount).ToList();

                foreach (var faq in selectedFAQs)
                {
                    faqs.Add(new FAQ
                    {
                        ProductId = product.Id,
                        Question = faq.Q,
                        Answer = faq.A,
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30))
                    });
                }
            }

            context.FAQs.AddRange(faqs);
            context.SaveChanges();
        }
    }
}
