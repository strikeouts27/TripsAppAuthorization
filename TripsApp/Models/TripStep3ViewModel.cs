using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class TripStep3ViewModel
{
    [StringLength(500)]
    public string? Activities { get; set; }
}
