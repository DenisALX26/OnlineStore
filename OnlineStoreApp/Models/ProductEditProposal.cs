using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineStoreApp.Models;

public class ProductEditProposal
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    [ValidateNever]
    public Product Product { get; set; } = null!;

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Price { get; set; }
    public int? CategoryId { get; set; }

    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";

    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}


