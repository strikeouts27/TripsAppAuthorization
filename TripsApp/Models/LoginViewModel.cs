using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class LoginViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
