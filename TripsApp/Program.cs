using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TripsApp.Data;
using TripsApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("TripsDb") ?? "Data Source=trips.db";
builder.Services.AddDbContext<TripsContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<TripsContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

await SeedDatabaseAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var provider = scope.ServiceProvider;

    var context = provider.GetRequiredService<TripsContext>();
    await context.Database.EnsureCreatedAsync();

    if (!await context.Trips.AnyAsync())
    {
        await context.Trips.AddRangeAsync(
            new Trip
            {
                Destination = "Seattle",
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(18),
                Accommodations = "Downtown hotel",
                Phone = "206-555-1212",
                Email = "host@example.com",
                Activities = "Pike Place Market, ferry ride, coffee tour"
            },
            new Trip
            {
                Destination = "Austin",
                StartDate = DateTime.Today.AddDays(35),
                EndDate = DateTime.Today.AddDays(40),
                Accommodations = "South Congress rental",
                Activities = "BBQ crawl, live music, Barton Springs"
            });

        await context.SaveChangesAsync();
    }

    var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
    var adminUser = await userManager.FindByNameAsync("admin");

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "admin"
        };

        var createResult = await userManager.CreateAsync(adminUser, "Sesame");

        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
    else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}
