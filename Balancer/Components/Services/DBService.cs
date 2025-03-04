using Balancer.Components.Data;
using Microsoft.EntityFrameworkCore;

namespace Balancer.Components.Services
{
    class DBService
    {
        private readonly ApplicationDBContext _context;

        public DBService(ApplicationDBContext context)
        {
            _context = context;
        }

        public void InitializeDatabase()
        {
            _context.Database.EnsureCreated();
        }
    }

    public static class ServiceExtensions
    {
        public static void AddDatabaseServices(this IServiceCollection services)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "balancer.db");
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            services.AddTransient<DBService>();
        }
    }
}