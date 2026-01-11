using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId, string comment, int rating, string returnUrl = null)
        {
            // Check if user is Admin - Admins cannot leave reviews
            if (User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Adminii nu pot lăsa review-uri.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            // Always get rating directly from form data to ensure correct value
            var ratingFromForm = Request.Form["rating"].ToString();
            if (!string.IsNullOrEmpty(ratingFromForm) && int.TryParse(ratingFromForm, out int parsedRating))
            {
                rating = parsedRating;
            }

            // Validare
            if (string.IsNullOrWhiteSpace(comment) || rating < 1 || rating > 5)
            {
                TempData["ErrorMessage"] = "Te rugăm să completezi toate câmpurile corect.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("Details", "Products", new { id = productId }) });
            }

            // Verifică dacă produsul există
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Verifică dacă utilizatorul a mai adăugat un review pentru acest produs
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "Ai adăugat deja un review pentru acest produs. Poți edita review-ul existent.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            // Creează review-ul
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Comment = comment,
                Rating = rating, // Ensure rating is set correctly
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Reload the review to ensure it's saved correctly
            await _context.Entry(review).ReloadAsync();

            // Recalculează rating-ul produsului
            await RecalculateProductRating(productId);

            TempData["SuccessMessage"] = "Review-ul tău a fost adăugat cu succes!";
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Products", new { id = productId });
        }

        [HttpPost]
        [Authorize(Roles = "Customer, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string returnUrl = null)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            // Verifică dacă utilizatorul este proprietarul review-ului sau este admin
            if (review.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Nu ai permisiunea de a șterge acest review.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Details", "Products", new { id = review.ProductId });
            }

            var productId = review.ProductId;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            // Recalculează rating-ul produsului
            await RecalculateProductRating(productId);

            TempData["SuccessMessage"] = "Review-ul a fost șters cu succes.";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Products", new { id = productId });
        }

        private async Task RecalculateProductRating(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return;

            if (product.Reviews != null && product.Reviews.Any())
            {
                product.Rating = product.Reviews.Average(r => r.Rating);
            }
            else
            {
                product.Rating = 0;
            }

            await _context.SaveChangesAsync();
        }
    }
}

