using Balancer.Components.Data;
using Balancer.Components.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Balancer.Components.Services
{
    internal class DonorService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<DonorService> _logger;

        public DonorService(ApplicationDBContext dbContext, ILogger<DonorService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _logger.LogInformation("DB Initialized");
        }

        public async Task<List<DonorModel>> GetDonorsAsync()
        {
            return await _dbContext.Donors.ToListAsync();
        }

        public async Task<DonorModel?> GetSingleDonor(int donorNumber)
        {
            var donor = await _dbContext.Donors.FindAsync(donorNumber);
            return donor;
        }

        public async Task AddDonorsAsync(DonorModel donor)
        {
            _logger.LogInformation("Adding Donors");
            _dbContext.Donors.Add(donor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDonorAsync(DonorModel donor)
        {
            _logger.LogInformation("Updating Donor");
            _dbContext.Donors.Update(donor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDonorAsync(int donorNumber)
        {
            var donor = await _dbContext.Donors.FindAsync(donorNumber);
            if (donor != null)
            {
                _logger.LogInformation("Deleting Donor");
                _dbContext.Donors.Remove(donor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
