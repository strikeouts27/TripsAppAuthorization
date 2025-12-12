using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripsApp.Data;
using TripsApp.Models;

namespace TripsApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TripsContext _context;

    public HomeController(ILogger<HomeController> logger, TripsContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Trips Log";
        var trips = await _context.Trips
            .OrderBy(t => t.StartDate)
            .ToListAsync();

        ViewData["StatusMessage"] = TempData["StatusMessage"] as string;
        return View(trips);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
