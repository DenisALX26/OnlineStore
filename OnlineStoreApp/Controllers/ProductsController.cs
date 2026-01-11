using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize(Roles = "Admin, Colaborator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Category);
            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                products = products.Where(p => p.CreatedByUserId == userId);
            }
            return View(products.ToList());
        }

        // GET: /Products/Details/5
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews!)
                    .ThenInclude(r => r.User)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Pre-populate dropdown with available categories
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Type })
                .ToList();
            return View();
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedByUserId = _userManager.GetUserId(User)!;
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Type })
                .ToList();
            return View(product);
        }

        // GET Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = _context.Products.Find(id);
            
            if (product == null)
                return NotFound();

            // Verifică dacă utilizatorul are permisiunea de a edita produsul
            // Admin poate edita orice produs, Colaborator doar propriile produse
            if (!User.IsInRole("Admin") && product.CreatedByUserId != userId)
            {
                return Forbid();
            }

            // Populează lista de categorii pentru dropdown
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Type })
                .ToList();

            return View(product);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var userId = _userManager.GetUserId(User);
            var existingProduct = _context.Products.Find(id);
            
            if (existingProduct == null)
                return NotFound();

            // Verifică dacă utilizatorul are permisiunea de a edita produsul
            if (!User.IsInRole("Admin") && existingProduct.CreatedByUserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Păstrează CreatedByUserId original (nu permite schimbarea proprietarului)
                product.CreatedByUserId = existingProduct.CreatedByUserId;
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Dacă modelul nu e valid, trebuie să repopulăm ViewBag
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Type })
                .ToList();

            return View(product);
        }

        // GET Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Verifică dacă utilizatorul are permisiunea de a șterge produsul
            // Admin poate șterge orice produs, Colaborator doar propriile produse
            if (!User.IsInRole("Admin") && product.CreatedByUserId != userId)
            {
                return Forbid();
            }

            return View(product);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = _context.Products.Find(id);
            
            if (product == null) 
                return NotFound();

            // Verifică dacă utilizatorul are permisiunea de a șterge produsul
            if (!User.IsInRole("Admin") && product.CreatedByUserId != userId)
            {
                return Forbid();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Colaborator")]
        public IActionResult EditColaboratorProducts(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var product = _context.Products
                .FirstOrDefault(p => p.Id == productId);

            if (product == null) return NotFound();

            if (product.CreatedByUserId != userId)
            {
                return Forbid();
            }

            var model = new ProductEditProposal
            {
                ProductId = product.Id,

                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,

                UserId = userId,
                Username = _userManager.GetUserName(User)!
            };

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Type
                }).ToList();

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Colaborator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditColaboratorProducts(ProductEditProposal model)
        {
            var userId = _userManager.GetUserId(User);

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (existingProduct == null)
                return NotFound();

            if (existingProduct.CreatedByUserId != userId)
                return Forbid();

            if (model.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Select a category");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Type
                    })
                    .ToList();

                return View(model);
            }

            model.Status = ProposalStatus.Pending;
            model.UserId = userId;
            model.Username = _userManager.GetUserName(User)!;

            _context.ProductEditProposals.Add(model);
            existingProduct.Status = ProductStatus.PendingEdit; 
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
