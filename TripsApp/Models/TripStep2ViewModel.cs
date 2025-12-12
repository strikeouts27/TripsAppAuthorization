using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class TripStep2ViewModel
{
    [Required]
    [StringLength(200)]
    public string Accommodations { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
