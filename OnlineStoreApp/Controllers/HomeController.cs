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

    public IActionResult Index(string category, string search, string price, string rating)
    {
        var products = _db.Products
        .Include(p => p.Category)
        .AsQueryable();

    if (!string.IsNullOrEmpty(category))
        products = products.Where(p => p.Category.Type == category);

    if (!string.IsNullOrEmpty(search))
        products = products.Where(p => p.Title.Contains(search));

    if (price == "high")
        products = products.OrderByDescending(p => p.Price);
    else if (price == "low")
        products = products.OrderBy(p => p.Price);

    if (rating == "high")
        products = products.OrderByDescending(p => p.Rating);
    else if (rating == "low")
        products = products.OrderBy(p => p.Rating);

    ViewBag.Categories = _db.Categories.ToList();
    ViewBag.SelectedCategory = category;
    ViewBag.Search = search;

    return View(products.ToList());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        ViewBag.CategoriesCount = _db.Categories.Count();
        ViewBag.ProductsCount = _db.Products.Count();
        ViewBag.ProposalsCount = _db.Proposals.Count();
        ViewBag.UsersCount = _db.Users.Count();
        ViewBag.ProductEditProposalsCount = _db.ProductEditProposals.Count();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
