using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public CartController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = await _db.Carts
                .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    TotalPrice = 0,
                    CartProducts = new List<CartProduct>()
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            // Calculate total price
            cart.TotalPrice = cart.CartProducts?.Sum(cp => cp.Product?.Price ?? 0) ?? 0;
            await _db.SaveChangesAsync();

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = await _db.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    TotalPrice = 0,
                    CartProducts = new List<CartProduct>()
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }

            // Check if product is already in cart
            if (!cart.CartProducts!.Any(cp => cp.ProductId == productId))
            {
                var cartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = productId
                };
                _db.CartProducts.Add(cartProduct);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = await _db.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            var cartProduct = cart.CartProducts?.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct != null)
            {
                _db.CartProducts.Remove(cartProduct);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}

