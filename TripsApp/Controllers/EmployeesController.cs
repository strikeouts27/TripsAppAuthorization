using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripsApp.Models;

namespace TripsApp.Controllers;

[Authorize(Roles = "Admin")]
public class EmployeesController : Controller
{
    [HttpGet]
    public IActionResult Add(string? statusMessage = null)
    {
        ViewBag.StatusMessage = statusMessage;
        return View(new EmployeeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(EmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        return RedirectToAction(nameof(Add), new { statusMessage = $"Employee {model.Name} added (demo only)." });
    }
}
