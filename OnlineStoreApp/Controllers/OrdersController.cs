using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, ILogger<OrdersController> logger)
        {
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
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

            if (cart == null || cart.CartProducts == null || !cart.CartProducts.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Add products before placing an order.";
                return RedirectToAction("Index", "Cart");
            }

            // Validate stock for all products in cart
            var outOfStockItems = new List<string>();
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product == null)
                {
                    continue;
                }

                if (cartProduct.Product.Stock < cartProduct.Quantity)
                {
                    outOfStockItems.Add($"{cartProduct.Product.Title} (Available: {cartProduct.Product.Stock}, Requested: {cartProduct.Quantity})");
                }
            }

            if (outOfStockItems.Any())
            {
                TempData["ErrorMessage"] = "Some products are no longer available in the requested quantity:\n" + string.Join("\n", outOfStockItems);
                return RedirectToAction("Index", "Cart");
            }

            // Calculate total
            var totalPrice = cart.CartProducts.Sum(cp => (cp.Product?.Price ?? 0) * cp.Quantity);

            ViewBag.TotalPrice = totalPrice;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
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

            if (cart == null || cart.CartProducts == null || !cart.CartProducts.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Validate stock again before placing order
            var outOfStockItems = new List<string>();
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product == null)
                {
                    continue;
                }

                if (cartProduct.Product.Stock < cartProduct.Quantity)
                {
                    outOfStockItems.Add($"{cartProduct.Product.Title} (Available: {cartProduct.Product.Stock}, Requested: {cartProduct.Quantity})");
                }
            }

            if (outOfStockItems.Any())
            {
                TempData["ErrorMessage"] = "Some products are no longer available in the requested quantity:\n" + string.Join("\n", outOfStockItems);
                return RedirectToAction("Index", "Cart");
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Completed",
                TotalPrice = cart.CartProducts.Sum(cp => (cp.Product?.Price ?? 0) * cp.Quantity),
                OrderItems = new List<OrderItem>()
            };

            // Create order items and update stock
            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product == null)
                {
                    continue;
                }

                var orderItem = new OrderItem
                {
                    ProductId = cartProduct.ProductId,
                    Quantity = cartProduct.Quantity,
                    Price = cartProduct.Product.Price
                };
                order.OrderItems.Add(orderItem);

                // Decrease stock
                cartProduct.Product.Stock -= cartProduct.Quantity;
                if (cartProduct.Product.Stock < 0)
                {
                    cartProduct.Product.Stock = 0; // Ensure stock doesn't go negative
                }
            }

            _db.Orders.Add(order);

            // Clear cart
            _db.CartProducts.RemoveRange(cart.CartProducts);
            cart.TotalPrice = 0;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Your order has been placed successfully! Total: {order.TotalPrice:F2} RON";
            return RedirectToAction("Index", "Cart");
        }
    }
}

