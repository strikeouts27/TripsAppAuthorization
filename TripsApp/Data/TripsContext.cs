using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TripsApp.Models;

namespace TripsApp.Data;

public class TripsContext : IdentityDbContext<IdentityUser>
{
    public TripsContext(DbContextOptions<TripsContext> options) : base(options)
    {
    }

    public DbSet<Trip> Trips => Set<Trip>();
}
