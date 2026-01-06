using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("categories")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var categories = _db.Categories.ToList();
            return View(categories);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_db.Categories.Any(c => c.Type == model.Type))
            {
                ModelState.AddModelError("Type", "O categorie cu acest tip există deja.");
                return View(model);
            }

            _db.Categories.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("create-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("edit-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("edit-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Categories.Update(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("delete-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost("delete-category")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteMethod(int id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var products = _db.Products.Where(p => p.CategoryId == id).ToList();
            _db.Products.RemoveRange(products);
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
