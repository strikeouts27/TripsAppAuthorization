using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class TripStep1ViewModel
{
    [Required]
    [StringLength(100)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }
}
