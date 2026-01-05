using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var products = _db.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        ViewBag.CategoriesCount = _db.Categories.Count();
        ViewBag.ProductsCount = _db.Products.Count();
        ViewBag.ProposalsCount = _db.Proposals.Count();
        ViewBag.UsersCount = _db.Users.Count();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
