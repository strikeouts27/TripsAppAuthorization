using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripsApp.Models;

namespace TripsApp.Controllers;

[Authorize(Roles = "Admin")]
public class ManageUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private const string AdminRole = "Admin";

    public ManageUsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? statusMessage = null)
    {
        var model = await BuildModelAsync();
        model.StatusMessage = statusMessage;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUser(ManageUsersViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildModelAsync();
            invalidModel.NewUserName = model.NewUserName;
            invalidModel.NewPassword = model.NewPassword;
            return View("Index", invalidModel);
        }

        var user = new IdentityUser { UserName = model.NewUserName };
        var result = await _userManager.CreateAsync(user, model.NewPassword);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { statusMessage = "User created." });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        var errorModel = await BuildModelAsync();
        errorModel.NewUserName = model.NewUserName;
        return View("Index", errorModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return RedirectToAction(nameof(Index), new { statusMessage = "User not found." });
        }

        if (string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Index), new { statusMessage = "The admin user cannot be deleted." });
        }

        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index), new { statusMessage = "User deleted." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (!await _roleManager.RoleExistsAsync(AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            await _userManager.AddToRoleAsync(user, AdminRole);
        }

        return RedirectToAction(nameof(Index), new { statusMessage = "User promoted to Admin." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromAdmin(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && !string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
        {
            await _userManager.RemoveFromRoleAsync(user, AdminRole);
        }

        return RedirectToAction(nameof(Index), new { statusMessage = "Admin role removed." });
    }

    private async Task<ManageUsersViewModel> BuildModelAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var rows = new List<ManageUserRow>();

        foreach (var user in users)
        {
            rows.Add(new ManageUserRow
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                IsAdmin = await _userManager.IsInRoleAsync(user, AdminRole)
            });
        }

        return new ManageUsersViewModel
        {
            Users = rows
        };
    }
}
