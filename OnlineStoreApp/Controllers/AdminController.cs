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

            return RedirectToAction("Index");
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

    }
}
