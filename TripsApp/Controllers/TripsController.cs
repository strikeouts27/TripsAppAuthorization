using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripsApp.Data;
using TripsApp.Models;

namespace TripsApp.Controllers;

[Authorize]
public class TripsController : Controller
{
    private readonly TripsContext _context;

    public TripsController(TripsContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult AddStep1()
    {
        ViewBag.SubHeader = "Step 1 - Destination & Dates";
        KeepFormData();

        var model = new TripStep1ViewModel();

        if (TempData.Peek("Destination") is string destination)
        {
            model.Destination = destination;
        }

        if (TempData.Peek("StartDate") is string startRaw && DateTime.TryParse(startRaw, null, DateTimeStyles.RoundtripKind, out var startDate))
        {
            model.StartDate = startDate;
        }

        if (TempData.Peek("EndDate") is string endRaw && DateTime.TryParse(endRaw, null, DateTimeStyles.RoundtripKind, out var endDate))
        {
            model.EndDate = endDate;
        }

        return View("Step1", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddStep1(TripStep1ViewModel model, string? action)
    {
        if (action == "cancel")
        {
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        ViewBag.SubHeader = "Step 1 - Destination & Dates";

        if (!ModelState.IsValid)
        {
            KeepFormData();
            return View("Step1", model);
        }

        TempData["Destination"] = model.Destination;
        TempData["StartDate"] = model.StartDate?.ToString("O");
        TempData["EndDate"] = model.EndDate?.ToString("O");
        KeepFormData();

        return RedirectToAction(nameof(AddStep2));
    }

    [HttpGet]
    public IActionResult AddStep2()
    {
        if (!HasStep1Data())
        {
            TempData.Clear();
            return RedirectToAction(nameof(AddStep1));
        }

        var model = new TripStep2ViewModel();

        if (TempData.Peek("Accommodations") is string accommodations)
        {
            model.Accommodations = accommodations;
        }

        if (TempData.Peek("Phone") is string phone && !string.IsNullOrWhiteSpace(phone))
        {
            model.Phone = phone;
        }

        if (TempData.Peek("Email") is string email && !string.IsNullOrWhiteSpace(email))
        {
            model.Email = email;
        }

        ViewBag.SubHeader = (TempData.Peek("Accommodations") as string) ?? (TempData.Peek("Destination") as string) ?? "Trip accommodations";
        KeepFormData();

        return View("Step2", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddStep2(TripStep2ViewModel model, string? action)
    {
        if (action == "cancel")
        {
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        if (!HasStep1Data())
        {
            TempData.Clear();
            return RedirectToAction(nameof(AddStep1));
        }

        ViewBag.SubHeader = model.Accommodations;

        if (!ModelState.IsValid)
        {
            KeepFormData();
            return View("Step2", model);
        }

        TempData["Accommodations"] = model.Accommodations;
        TempData["Phone"] = model.Phone ?? string.Empty;
        TempData["Email"] = model.Email ?? string.Empty;
        KeepFormData();

        return RedirectToAction(nameof(AddStep3));
    }

    [HttpGet]
    public IActionResult AddStep3()
    {
        if (!HasStep1Data())
        {
            TempData.Clear();
            return RedirectToAction(nameof(AddStep1));
        }

        var model = new TripStep3ViewModel();

        if (TempData.Peek("Activities") is string activities && !string.IsNullOrWhiteSpace(activities))
        {
            model.Activities = activities;
        }

        ViewBag.SubHeader = TempData.Peek("Destination") as string;
        KeepFormData();

        return View("Step3", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStep3(TripStep3ViewModel model, string? action)
    {
        if (action == "cancel")
        {
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        if (!HasStep1Data())
        {
            TempData.Clear();
            return RedirectToAction(nameof(AddStep1));
        }

        ViewBag.SubHeader = TempData.Peek("Destination") as string;

        if (!ModelState.IsValid)
        {
            TempData["Activities"] = model.Activities ?? string.Empty;
            KeepFormData();
            return View("Step3", model);
        }

        TempData["Activities"] = model.Activities ?? string.Empty;

        var trip = new Trip
        {
            Destination = TempData["Destination"]?.ToString() ?? "Trip",
            StartDate = ParseDate(TempData["StartDate"]),
            EndDate = ParseDate(TempData["EndDate"]),
            Accommodations = TempData["Accommodations"]?.ToString() ?? string.Empty,
            Phone = NormalizeOptional(TempData["Phone"]?.ToString()),
            Email = NormalizeOptional(TempData["Email"]?.ToString()),
            Activities = NormalizeOptional(TempData["Activities"]?.ToString())
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        TempData.Clear();
        TempData["StatusMessage"] = $"Trip to {trip.Destination} added.";

        return RedirectToAction("Index", "Home");
    }

    private bool HasStep1Data()
    {
        return TempData.ContainsKey("Destination") && TempData.ContainsKey("StartDate") && TempData.ContainsKey("EndDate");
    }

    private void KeepFormData()
    {
        TempData.Keep("Destination");
        TempData.Keep("StartDate");
        TempData.Keep("EndDate");
        TempData.Keep("Accommodations");
        TempData.Keep("Phone");
        TempData.Keep("Email");
        TempData.Keep("Activities");
    }

    private static DateTime ParseDate(object? value)
    {
        if (value is string text && DateTime.TryParse(text, null, DateTimeStyles.RoundtripKind, out var parsed))
        {
            return parsed;
        }

        return DateTime.Today;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
