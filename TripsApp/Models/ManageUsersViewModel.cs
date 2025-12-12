using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class ManageUsersViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string NewUserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;

    public string? StatusMessage { get; set; }

    public List<ManageUserRow> Users { get; set; } = new();
}

public class ManageUserRow
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}
