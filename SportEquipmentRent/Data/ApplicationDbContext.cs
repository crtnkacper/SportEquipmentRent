using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportEquipmentRent.Models;

namespace SportEquipmentRent.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SportEquipmentRent.Models.SportEquipment> SportEquipment { get; set; } = default!;
        public DbSet<SportEquipmentRent.Models.Renting> Renting { get; set; } = default!;
    }
}
