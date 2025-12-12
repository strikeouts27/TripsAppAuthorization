using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class Trip
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    [Required]
    [StringLength(200)]
    public string Accommodations { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? Activities { get; set; }
}
