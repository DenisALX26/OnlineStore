using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public WishlistController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
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
                return RedirectToAction("Login", "Account");
            }
            var wishlist = _db.Wishlists.Include(w => w.WishlistProducts).ThenInclude(wp => wp.Product)
                                        .FirstOrDefault(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    WishlistProducts = new List<WishlistProduct>()
                };
                _db.Wishlists.Add(wishlist);
                await _db.SaveChangesAsync();
            }
            return View(wishlist);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Add(int productId, string returnUrl = null)
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

            var wishlist = _db.Wishlists.Include(w => w.WishlistProducts)
                                        .FirstOrDefault(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    WishlistProducts = new List<WishlistProduct>()
                };
                _db.Wishlists.Add(wishlist);
                await _db.SaveChangesAsync();
            }

            if (!wishlist.WishlistProducts.Any(wp => wp.ProductId == productId))
            {
                var wishlistProduct = new WishlistProduct
                {
                    WishlistId = wishlist.Id,
                    ProductId = productId
                };
                _db.WishlistProducts.Add(wishlistProduct);
                await _db.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var wishlist = await _db.Wishlists
                .Include(w => w.WishlistProducts)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return NotFound();
            }

            var wishlistProduct = wishlist.WishlistProducts?.FirstOrDefault(wp => wp.ProductId == productId);
            if (wishlistProduct != null)
            {
                _db.WishlistProducts.Remove(wishlistProduct);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
