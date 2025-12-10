using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Categories()
        {
            var categories = _db.Categories.ToList();
            return View(categories);
        }

        [HttpGet("create-category")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost("create-category")]
        public IActionResult CreateCategory(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.Categories.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Categories.Update(category);
            _db.SaveChanges();

            return RedirectToAction("Categories");
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteCategoryMethod(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Categories");
        }

        public ActionResult Products()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

    }
}
