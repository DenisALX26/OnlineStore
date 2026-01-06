using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineStoreApp.Data;

namespace OnlineStoreApp.Models;

public class Proposal
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Propunerea trebuie sa aiba un titlu"), StringLength(100)]
    public string Title { get; set; } = "";
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public ProposalStatus Status { get; set; } = ProposalStatus.Pending;
    public string Description { get; set; } = "";
    public double Price { get; set;}
    public string Image { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Username { get; set; } = "";
    public string? RejectionReason { get; set; }
}
