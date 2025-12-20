using System.ComponentModel.DataAnnotations;

namespace OnlineStoreApp.Models;

public enum ProposalStatus
{
    [Display(Name = "În așteptare")]
    Pending,
    [Display(Name = "Acceptată")]
    Accepted,
    [Display(Name = "Respinsă")]
    Rejected
}
