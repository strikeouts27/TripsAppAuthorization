using System.ComponentModel.DataAnnotations;

namespace TripsApp.Models;

public class EmployeeViewModel
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Title { get; set; } = string.Empty;
}
