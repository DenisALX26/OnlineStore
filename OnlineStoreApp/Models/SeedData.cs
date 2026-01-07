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

            var products = new List<Product>
            {
                // Sneakers
                new()
                {
                    Title = "Classic White Sneakers",
                    Description = "Aceste sneakers albe clasice sunt perfecte pentru uz zilnic, oferind un echilibru perfect între confort și stil. Realizate din materiale premium: talpă exterioară din cauciuc rezistent, talpă interioară cu suport pentru arcada piciorului, și partea superioară din material textil respirabil. Disponibile în mărimi de la 36 la 46. Garanție de 2 ani pentru defecte de fabricație. Potrivite pentru mers pe jos, activități zilnice și stil casual. Curățare ușoară cu cârpă umedă. Livrare gratuită pentru comenzi peste 200 RON.",
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
                    Description = "Sneakers negre high-top moderne cu suport excelent pentru gleznă, perfecte pentru stil urban și ieșiri casuale. Caracteristici: partea superioară din piele sintetică de înaltă calitate, talpă exterioară din cauciuc cu aderență îmbunătățită, sistem de legare rapidă, și căptușeală confortabilă. Disponibile în mărimi 36-46. Garanție 2 ani. Potrivite pentru skateboarding, mers pe jos și activități urbane. Materiale durabile și ușor de întreținut. Politică de returnare 14 zile.",
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
                    Description = "Sneakers cu design retro inspirat din anii '80-'90, cu culori îndrăznețe și design clasic. Perfecte pentru a face o declarație de stil. Detalii: partea superioară din material textil colorat, talpă exterioară din cauciuc cu pattern clasic, logo retro pe lateral, și confort premium. Mărimi disponibile: 36-46. Garanție 2 ani. Potrivite pentru stil casual, evenimente și colecționari. Materiale de calitate superioară. Livrare în 2-3 zile lucrătoare.",
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
                    Description = "Design curat și simplu cu confort maxim. Perfecte pentru cei care preferă eleganță subtilă. Caracteristici: partea superioară din piele naturală albă, talpă exterioară din cauciuc alb, design minimalist fără logo vizibil, și confort excepțional pentru toată ziua. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru birou casual, evenimente și uz zilnic. Materiale premium și durabile. Returnare gratuită în 14 zile.",
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
                    Title = "Leather Ankle Boots",
                    Description = "Ghete de gleznă din piele premium cu design clasic. Ideale pentru sezonul de toamnă și iarnă. Caracteristici: piele naturală de vită de înaltă calitate, căptușeală termică pentru izolare, talpă exterioară din cauciuc cu aderență excelentă pe zăpadă și gheață, și fermoar lateral pentru ușurință în purtare. Disponibile în mărimi 36-46. Garanție 2 ani. Impermeabile, potrivite pentru ploaie și zăpadă ușoară. Întreținere: curățare cu produse speciale pentru piele. Livrare gratuită.",
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
                    Description = "Ghete combat durabile cu design robust. Perfecte pentru activități în aer liber și teren dificil. Detalii: partea superioară din piele și material textil rezistent, talpă exterioară din cauciuc cu aderență agresivă, sistem de legare rapidă, și suport excelent pentru gleznă. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru hiking, camping, și activități outdoor. Rezistente la apă și durabile. Returnare în 14 zile.",
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
                    Description = "Ghete Chelsea elegante cu panouri elastice laterale. Versatil și stilat pentru orice ocazie. Caracteristici: piele naturală premium, panouri elastice pentru ușurință în purtare, talpă exterioară din cauciuc, și design clasic britanic. Mărimi disponibile: 36-46. Garanție 2 ani. Potrivite pentru birou, evenimente și stil smart casual. Materiale de calitate superioară. Livrare în 2-3 zile.",
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
                new()
                {
                    Title = "Ultra Lightweight Running Shoes",
                    Description = "Încălțăminte de alergare profesională cu tehnologie avansată de amortizare. Proiectate pentru performanță și confort maxim. Caracteristici: talpă exterioară din cauciuc de înaltă calitate, sistem de amortizare cu tehnologie de returnare a energiei, partea superioară din material mesh respirabil, și greutate ultra-redusă (doar 220g per pereche). Mărimi: 36-46. Garanție 2 ani. Potrivite pentru alergare pe asfalt, track și distanțe medii. Tehnologie anti-sudare și suport pentru arcada piciorului. Livrare gratuită.",
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
                    Description = "Încălțăminte de alergare pe trasee cu aderență excelentă și stabilitate. Perfecte pentru aventuri off-road. Detalii: talpă exterioară cu crampoane agresive pentru aderență pe pământ, piatră și noroi, protecție pentru degete, sistem de amortizare pentru teren accidentat, și materiale rezistente la apă. Mărimi disponibile: 36-46. Garanție 2 ani. Potrivite pentru trail running, hiking rapid și teren dificil. Rezistente și durabile. Returnare în 14 zile.",
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
                    Description = "Încălțăminte de performanță înaltă proiectate pentru alergare pe distanțe lungi. Ușoare cu returnare superioară a energiei. Caracteristici: tehnologie de carbon fiber pentru rigiditate și eficiență, sistem de amortizare premium pentru distanțe lungi, partea superioară ultra-respirabilă, și greutate redusă pentru performanță maximă. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru maratoane, semi-maratoane și distanțe lungi. Tehnologie de vârf pentru atleți. Livrare în 2-3 zile.",
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
                    Title = "Canvas Slip-On Shoes",
                    Description = "Încălțăminte confortabilă slip-on din canvas în diverse culori. Ușor de purtat și perfectă pentru ocazii casuale. Caracteristici: partea superioară din canvas de calitate, talpă interioară confortabilă, design fără șireturi pentru ușurință, și talpă exterioară din cauciuc. Disponibile în mărimi 36-46 și multiple culori. Garanție 2 ani. Potrivite pentru plimbări, shopping și activități zilnice. Ușor de curățat și întreținut. Livrare gratuită pentru comenzi peste 150 RON.",
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
                    Description = "Loafers clasice din piele cu design atemporal. Perfecte pentru ocazii smart casual. Detalii: piele naturală premium, design fără șireturi elegant, talpă exterioară din cauciuc, și confort pentru toată ziua. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru birou, întâlniri și evenimente casual elegante. Materiale de calitate superioară. Returnare gratuită în 14 zile.",
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
                    Description = "Sandale ușoare și respirabile perfecte pentru vară. Rezistente la apă și confortabile pentru purtare toată ziua. Caracteristici: material EVA ușor și flexibil, talpă exterioară din cauciuc cu aderență pe nisip și piatră, curele ajustabile, și design rapid de uscare. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru plajă, piscină și activități de vară. Rezistente la apă și ușor de curățat. Livrare gratuită.",
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
                    Description = "Sandale sport durabile cu aderență excelentă. Perfecte pentru hiking și activități în aer liber. Detalii: curele din material sintetic rezistent, talpă exterioară cu crampoane pentru aderență, suport pentru arcada piciorului, și design ergonomic. Mărimi disponibile: 36-46. Garanție 2 ani. Potrivite pentru hiking, camping și activități outdoor. Rezistente și durabile. Returnare în 14 zile.",
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
                    Description = "Încălțăminte elegantă Oxford din piele naturală autentică. Perfectă pentru afaceri și ocazii formale. Caracteristici: piele de vită de înaltă calitate, legare Oxford clasică, talpă exterioară din piele, și finisare lustruită profesională. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru interviuri, evenimente formale și birou. Materiale premium și durabile. Livrare în 2-3 zile lucrătoare.",
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
                    Description = "Încălțăminte sofisticată Derby cu sistem de legare deschis. Confortabilă dar elegantă pentru purtare formală. Detalii: piele naturală premium, legare Derby pentru confort sporit, talpă exterioară din cauciuc, și design clasic britanic. Mărimi: 36-46. Garanție 2 ani. Potrivite pentru evenimente formale, birou și ocazii speciale. Materiale de calitate superioară. Returnare gratuită în 14 zile.",
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
            
            // FAQs specifice pentru fiecare categorie
            var sneakersFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Garanția acoperă defecte de materiale și de fabricație." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale sau consultări despre măsură, vă rugăm să ne contactați." },
                new { Q = "Este potrivit pentru alergare?", A = "Acest produs este proiectat pentru uz zilnic și confort. Pentru alergare profesională, recomandăm produsele specializate din categoria Running Shoes care oferă suport și amortizare superioară." },
                new { Q = "Este impermeabil?", A = "Produsul oferă protecție de bază împotriva apei datorită materialelor utilizate, dar nu este complet impermeabil. Pentru condiții de ploaie intensă, recomandăm produse specializate." },
                new { Q = "Cum trebuie să îl curăț?", A = "Recomandăm curățarea cu o cârpă umedă și un detergent blând. Pentru partea superioară din material textil, puteți folosi o perie moale. Evitați mașina de spălat și uscarea la soare direct pentru a preveni deteriorarea." },
                new { Q = "Este potrivit pentru iarnă?", A = "Acest produs oferă confort și protecție de bază, dar pentru condiții de iarnă cu zăpadă și temperaturi scăzute, recomandăm produse specializate din categoria Boots cu căptușeală termică." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale (nevăzut, cu etichetele atașate), cu bonul fiscal. Returnarea este gratuită pentru comenzi peste 200 RON." },
                new { Q = "Cât durează livrarea?", A = "Livrarea standard durează 2-3 zile lucrătoare. Pentru comenzi peste 200 RON, livrarea este gratuită. Oferim și opțiune de livrare express în 24 de ore (cu cost suplimentar)." },
                new { Q = "Este potrivit pentru copii?", A = "Acest produs este recomandat pentru adulți. Mărimile disponibile (36-46) sunt pentru adulți. Pentru copii, vă recomandăm să consultați secțiunea dedicată produselor pentru copii." },
                new { Q = "Ce materiale sunt folosite?", A = "Produsul este realizat din materiale premium: partea superioară din material textil respirabil sau piele sintetică de calitate, talpă exterioară din cauciuc rezistent, și talpă interioară cu suport pentru arcada piciorului." }
            };
            
            var bootsFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Garanția acoperă defecte de materiale, cusături și componente." },
                new { Q = "Este impermeabil?", A = "Da, acest produs este impermeabil datorită materialelor premium utilizate și tratamentelor speciale aplicate. Oferă protecție excelentă împotriva apei, ploii și zăpezii ușoare." },
                new { Q = "Este potrivit pentru iarnă?", A = "Da, acest produs este perfect pentru iarnă. Are căptușeală termică pentru izolare, talpă exterioară cu aderență excelentă pe zăpadă și gheață, și oferă protecție completă împotriva elementelor." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale sau consultări, vă rugăm să ne contactați." },
                new { Q = "Cum trebuie să îl curăț?", A = "Pentru piele naturală, folosiți produse speciale de curățare și condiționare pentru piele. Curățați cu o cârpă umedă după fiecare utilizare, mai ales pe zăpadă sau noroi. Evitați uscarea la căldură directă." },
                new { Q = "Este potrivit pentru hiking?", A = "Da, acest produs este potrivit pentru hiking și activități outdoor. Oferă suport excelent pentru gleznă, aderență pe teren dificil, și protecție împotriva elementelor." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Returnarea este gratuită." },
                new { Q = "Cât durează livrarea?", A = "Livrarea standard durează 2-3 zile lucrătoare. Oferim livrare gratuită pentru comenzi peste 200 RON și opțiune de livrare express." }
            };
            
            var runningFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Garanția acoperă defecte de materiale, talpă și componente." },
                new { Q = "Este potrivit pentru alergare?", A = "Da, acest produs este special proiectat pentru alergare. Are tehnologie avansată de amortizare, suport pentru arcada piciorului, și design optimizat pentru performanță." },
                new { Q = "Este potrivit pentru maratoane?", A = "Da, acest produs este potrivit pentru maratoane și distanțe lungi. Are tehnologie de carbon fiber pentru eficiență, sistem de amortizare premium, și greutate redusă pentru performanță maximă." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru consultări despre măsură sau mărimi speciale, vă rugăm să ne contactați." },
                new { Q = "Cum trebuie să îl curăț?", A = "Recomandăm curățarea după fiecare utilizare cu o cârpă umedă. Pentru partea superioară din mesh, puteți folosi o perie moale. Evitați mașina de spălat și uscarea la căldură directă pentru a păstra proprietățile tehnice." },
                new { Q = "Este potrivit pentru trail running?", A = "Acest produs este optimizat pentru alergare pe asfalt și track. Pentru trail running și teren dificil, recomandăm produsele specializate din categoria Trail Running Shoes cu crampoane și protecție suplimentară." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Returnarea este gratuită pentru comenzi peste 200 RON." },
                new { Q = "Ce tehnologie de amortizare folosește?", A = "Produsul folosește tehnologie avansată de amortizare cu returnare a energiei, sistem de suport pentru arcada piciorului, și materiale premium pentru confort și performanță maximă." }
            };
            
            var casualFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru consultări despre măsură, vă rugăm să ne contactați." },
                new { Q = "Este potrivit pentru uz zilnic?", A = "Da, acest produs este perfect pentru uz zilnic. Oferă confort excelent, design versatil, și este ușor de purtat pentru activități zilnice." },
                new { Q = "Cum trebuie să îl curăț?", A = "Pentru canvas, curățați cu o cârpă umedă și detergent blând. Pentru piele, folosiți produse speciale pentru piele. Evitați mașina de spălat și uscarea la soare direct." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Returnarea este gratuită pentru comenzi peste 150 RON." },
                new { Q = "Cât durează livrarea?", A = "Livrarea standard durează 2-3 zile lucrătoare. Oferim livrare gratuită pentru comenzi peste 150 RON." }
            };
            
            var sandalsFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație." },
                new { Q = "Este rezistent la apă?", A = "Da, acest produs este rezistent la apă datorită materialelor utilizate (EVA sau material sintetic). Se usucă rapid și este perfect pentru activități de vară." },
                new { Q = "Este potrivit pentru plajă?", A = "Da, acest produs este perfect pentru plajă. Este rezistent la apă, se usucă rapid, are aderență excelentă pe nisip, și este ușor de curățat." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru consultări despre măsură, vă rugăm să ne contactați." },
                new { Q = "Cum trebuie să îl curăț?", A = "Curățarea este foarte simplă: spălați cu apă și săpun, apoi lăsați să se usuce la aer. Materialele utilizate sunt ușor de întreținut și rezistente." },
                new { Q = "Este potrivit pentru hiking?", A = "Pentru hiking și activități outdoor, recomandăm produsele specializate din categoria Sport Sandals care oferă suport suplimentar, crampoane pentru aderență, și design ergonomic." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Returnarea este gratuită." }
            };
            
            var formalFAQs = new[]
            {
                new { Q = "Are garanție?", A = "Da, toate produsele noastre beneficiază de garanție de 2 ani pentru defecte de fabricație. Garanția acoperă defecte de materiale, cusături și componente." },
                new { Q = "Care sunt măsurile disponibile?", A = "Produsele noastre sunt disponibile în mărimi standard de la 36 la 46. Pentru mărimi speciale sau consultări, vă rugăm să ne contactați." },
                new { Q = "Este potrivit pentru evenimente formale?", A = "Da, acest produs este perfect pentru evenimente formale, interviuri, și ocazii de afaceri. Design clasic și elegant, realizat din piele naturală premium." },
                new { Q = "Cum trebuie să îl curăț?", A = "Pentru piele naturală, folosiți produse speciale de curățare și lustruire pentru piele. Curățați regulat cu o cârpă moale și aplicați cremă pentru piele pentru a menține aspectul și durabilitatea." },
                new { Q = "Care este politica de returnare?", A = "Puteți returna produsul în termen de 14 zile de la cumpărare, în condiții originale, cu bonul fiscal. Returnarea este gratuită." },
                new { Q = "Cât durează livrarea?", A = "Livrarea standard durează 2-3 zile lucrătoare. Oferim și opțiune de livrare express în 24 de ore pentru evenimente urgente." },
                new { Q = "Ce materiale sunt folosite?", A = "Produsul este realizat din piele naturală de vită de înaltă calitate, talpă exterioară din piele sau cauciuc, și finisare lustruită profesională pentru aspect elegant." }
            };
            
            // FAQs comune pentru toate produsele
            var commonFAQs = new[]
            {
                new { Q = "Care este politica de livrare?", A = "Oferim livrare standard în 2-3 zile lucrătoare. Livrarea este gratuită pentru comenzi peste 200 RON (sau 150 RON pentru produse casual). Oferim și livrare express în 24 de ore cu cost suplimentar." },
                new { Q = "Pot plăti cu cardul?", A = "Da, acceptăm plăți cu cardul (Visa, Mastercard), transfer bancar, și plata la livrare (ramburs) pentru anumite zone." },
                new { Q = "Oferiți servicii de personalizare?", A = "Momentan nu oferim servicii de personalizare, dar lucrăm la implementarea acestora. Pentru cereri speciale, vă rugăm să ne contactați." },
                new { Q = "Cum pot contacta suportul?", A = "Puteți ne contacta prin email la support@onlinestore.ro, telefon la 0800-123-456 (luni-vineri, 9-18), sau prin formularul de contact de pe site." }
            };

            foreach (var product in savedProducts)
            {
                // Select FAQs based on product category
                var categoryFAQs = product.Category?.Type switch
                {
                    "Sneakers" => sneakersFAQs,
                    "Boots" => bootsFAQs,
                    "Running Shoes" => runningFAQs,
                    "Casual Shoes" => casualFAQs,
                    "Sandals" => sandalsFAQs,
                    "Formal Shoes" => formalFAQs,
                    _ => sneakersFAQs // Default to sneakers if category not found
                };
                
                // Add 5-7 category-specific FAQs per product
                int faqCount = random.Next(5, 8);
                var selectedCategoryFAQs = categoryFAQs.OrderBy(x => random.Next()).Take(Math.Min(faqCount, categoryFAQs.Length)).ToList();
                
                // Add 1-2 common FAQs
                int commonFaqCount = random.Next(1, 3);
                var selectedCommonFAQs = commonFAQs.OrderBy(x => random.Next()).Take(commonFaqCount).ToList();
                
                // Combine and add all FAQs
                foreach (var faq in selectedCategoryFAQs)
                {
                    faqs.Add(new FAQ
                    {
                        ProductId = product.Id,
                        Question = faq.Q,
                        Answer = faq.A,
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30))
                    });
                }
                
                foreach (var faq in selectedCommonFAQs)
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
