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
        [Authorize(Roles = "Customer")]
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

            // Calculate total price based on quantity
            cart.TotalPrice = cart.CartProducts?.Sum(cp => (cp.Product?.Price ?? 0) * cp.Quantity) ?? 0;
            await _db.SaveChangesAsync();

            return View(cart);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add(int productId, int quantity = 1, string returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = returnUrl });
                }
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("Details", "Products", new { id = productId }) });
            }

            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index");
            }

            // Validate stock
            if (product.Stock < quantity)
            {
                TempData["ErrorMessage"] = $"Insufficient stock. Available: {product.Stock} items.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index");
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than 0.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index");
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
            var existingCartProduct = cart.CartProducts!.FirstOrDefault(cp => cp.ProductId == productId);
            if (existingCartProduct != null)
            {
                // Update quantity if product already in cart
                var newQuantity = existingCartProduct.Quantity + quantity;
                if (product.Stock < newQuantity)
                {
                    TempData["ErrorMessage"] = $"Insufficient stock. Available: {product.Stock} items. In cart: {existingCartProduct.Quantity} items.";
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index");
                }
                existingCartProduct.Quantity = newQuantity;
            }
            else
            {
                var cartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _db.CartProducts.Add(cartProduct);
            }
            
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added to cart successfully!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = await _db.Carts
                .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            var cartProduct = cart.CartProducts?.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                // Remove product if quantity is 0 or less
                _db.CartProducts.Remove(cartProduct);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product removed from cart.";
                return RedirectToAction("Index");
            }

            // Validate stock
            if (cartProduct.Product!.Stock < quantity)
            {
                TempData["ErrorMessage"] = $"Insufficient stock. Available: {cartProduct.Product.Stock} items.";
                return RedirectToAction("Index");
            }

            cartProduct.Quantity = quantity;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Quantity updated successfully!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
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
                TempData["SuccessMessage"] = "Product removed from cart.";
            }

            return RedirectToAction("Index");
        }
    }
}

