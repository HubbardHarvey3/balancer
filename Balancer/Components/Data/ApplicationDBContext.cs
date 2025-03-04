using Balancer.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace Balancer.Components.Data
{
    internal class ApplicationDBContext : DbContext
    {
        public DbSet<DonorModel> Donors { get; set; }
        public DbSet<DonationEntryModel> Donations { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options) { }
    }
}
