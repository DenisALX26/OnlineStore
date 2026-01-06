using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineStoreApp.Data;
using OnlineStoreApp.Extensions;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    public class ProposalsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public ProposalsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Colaborator")]
        public IActionResult Index()
        {
            IQueryable<Proposal> proposals = _db.Proposals;
            if (!User.IsInRole("Admin"))
            {
                proposals = proposals.Where(p => p.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            ViewBag.Statuses = Enum.GetValues(typeof(ProposalStatus))
                .Cast<ProposalStatus>()
                .ToDictionary(status => (int)status, status => EnumExtensions.GetDisplayName(status));
            return View(proposals.ToList());
        }

        [Authorize(Roles = "Colaborator")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "Type", "Type");
            return View();
        }

        [Authorize(Roles = "Colaborator")]
        [HttpPost]
        public async Task<IActionResult> Create(Proposal proposal, IFormFile Image)
        {
            proposal.UserId = _userManager.GetUserId(User)!;
            proposal.Status = ProposalStatus.Pending;
            proposal.Username = _userManager.GetUserName(User) ?? "Unknown User";

            ModelState.Remove(nameof(proposal.Image));
            if (Image != null && Image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".avif" };
                var fileExtension = Path.GetExtension(Image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Image", "Invalid image format. Allowed formats are: .jpg, .jpeg, .png, .gif");
                    return View(proposal);
                }

                var storagePath = Path.Combine(_env.WebRootPath, "images", Image.FileName);
                var databaseFileName = "/images/" + Image.FileName;

                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }
                proposal.Image = databaseFileName;
            }

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine("Se a intamplat o eroare la validare:");
                        Console.WriteLine($"{entry.Key}: {error.ErrorMessage}");
                    }
                }
                return View(proposal);
            }
            _db.Proposals.Add(proposal);
            await _db.SaveChangesAsync(); return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Colaborator")]
        public IActionResult Details(int id)
        {
            var proposal = _db.Proposals.Find(id);
            if (proposal == null)
            {
                return NotFound();
            }
            return View(proposal);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var proposal = await _db.Proposals.FindAsync(id);
            if (proposal == null)
            {
                return NotFound();
            }

            proposal.Status = ProposalStatus.Rejected;
            proposal.RejectionReason = reason;

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        [Authorize(Roles = "Colaborator")]
        public IActionResult Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            var proposal = _db.Proposals
                .FirstOrDefault(p => p.Id == id && p.UserId == userId);

            if (proposal == null)
            {
                return NotFound();
            }

            if (proposal.Status != ProposalStatus.Rejected)
            {
                return Forbid();
            }

            ViewBag.Categories = new SelectList(
                _db.Categories,
                "Type",
                "Type",
                proposal.Category
            );

            return View(proposal);
        }


        [HttpPost]
        [Authorize(Roles = "Colaborator")]
        public async Task<IActionResult> Edit(Proposal model, IFormFile? Image)
        {
            var userId = _userManager.GetUserId(User);

            var proposal = await _db.Proposals
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.UserId == userId);

            if (proposal == null)
                return NotFound();

            // 🔥 FIXUL CHEIE
            ModelState.Remove(nameof(Proposal.Image));

            // VALIDARE MODEL
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    _db.Categories,
                    "Type",
                    "Type",
                    model.Category
                );
                return View(model);
            }

            // 🔁 IMAGE UPDATE
            if (Image != null && Image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".avif" };
                var ext = Path.GetExtension(Image.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Image", "Invalid image format");

                    ViewBag.Categories = new SelectList(
                        _db.Categories,
                        "Type",
                        "Type",
                        model.Category
                    );

                    return View(model);
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var savePath = Path.Combine(_env.WebRootPath, "images", fileName);
                var dbPath = "/images/" + fileName;

                using var stream = new FileStream(savePath, FileMode.Create);
                await Image.CopyToAsync(stream);

                if (!string.IsNullOrEmpty(proposal.Image))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, proposal.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                proposal.Image = dbPath;
            }

            // 🔁 UPDATE DATA
            proposal.Title = model.Title;
            proposal.Description = model.Description;
            proposal.Price = model.Price;
            proposal.Category = model.Category;

            proposal.Status = ProposalStatus.Pending;
            proposal.RejectionReason = null;

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = proposal.Id });
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var proposal = await _db.Proposals.FirstOrDefaultAsync(p => p.Id == id);
            if (proposal == null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Type == proposal.Category);

            if (category == null)
            {
                category = new Category { Type = proposal.Category };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }

            var new_product = new Product
            {
                Title = proposal.Title,
                Description = proposal.Description,
                Price = proposal.Price,
                Image = proposal.Image,
                CategoryId = category.Id,
                Stock = 0,
                Rating = 0.0,
                Type = Models.Type.Left
            };

            _db.Products.Add(new_product);
            proposal.Status = ProposalStatus.Accepted;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}
