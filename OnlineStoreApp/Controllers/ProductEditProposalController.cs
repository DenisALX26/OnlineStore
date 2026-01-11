using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreApp.Data;
using OnlineStoreApp.Models;

namespace OnlineStoreApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductEditProposalController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductEditProposalController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var proposals = _db.ProductEditProposals.Include(p => p.Product)
                                .Where(p => p.Status == ProposalStatus.Pending).ToList();
            return View(proposals);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var proposal = _db.ProductEditProposals
                .Include(p => p.Product)
                .FirstOrDefault(p => p.Id == id);

            if (proposal == null)
                return NotFound();

            return View(proposal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var proposal = await _db.ProductEditProposals
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proposal == null)
                return NotFound();

            var product = proposal.Product;

            // APPLY CHANGES 🔥
            product.Title = proposal.Title;
            product.Description = proposal.Description;
            product.Price = proposal.Price;
            product.CategoryId = proposal.CategoryId;

            proposal.Status = ProposalStatus.Accepted;

            _db.ProductEditProposals.Remove(proposal);


            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var proposal = await _db.ProductEditProposals.FindAsync(id);
            if (proposal == null)
                return NotFound();

            proposal.Status = ProposalStatus.Rejected;
            proposal.RejectionReason = reason;

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }
    }
}
