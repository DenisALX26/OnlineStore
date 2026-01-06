using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4))));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations and seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    // Check if FAQs table exists, if not apply migration
    try
    {
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            context.Database.Migrate(); // create/update tables if needed
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error applying migrations. Attempting to create FAQs table manually...");
        
        // Try to create FAQs table if it doesn't exist
        try
        {
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS FAQs (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    ProductId INT NOT NULL,
                    Question VARCHAR(500) NOT NULL,
                    Answer VARCHAR(2000) NOT NULL,
                    CreatedAt DATETIME(6) NOT NULL,
                    INDEX IX_FAQs_ProductId (ProductId),
                    CONSTRAINT FK_FAQs_Products_ProductId FOREIGN KEY (ProductId) 
                        REFERENCES Products(Id) ON DELETE CASCADE
                ) CHARACTER SET utf8mb4;
            ");
        }
        catch (Exception createEx)
        {
            logger.LogError(createEx, "Failed to create FAQs table manually");
        }
    }
    
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Must be before UseRouting to serve static files
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
