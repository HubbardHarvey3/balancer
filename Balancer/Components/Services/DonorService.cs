using Balancer.Components.Data;
using Balancer.Components.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Balancer.Components.Services
{
    internal class DonorService : IDonorService
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

        public async Task<DonorModel?> GetSingleDonorAsync(int donorNumber)
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

        public async Task UpdateDonorAmounts(List<DonationEntryModel> donorList)
        {
            foreach (var donor in donorList)
            {
                _logger.LogInformation("Updating {Name}", donor.Name);
                var donorToUpdate = await _dbContext.Donors.FindAsync(donor.DonorNumber);
                if (donorToUpdate == null)
                {
                    _logger.LogInformation("Donor {Name} Not Found", donor.Name);
                }
                else
                {
                    donorToUpdate.DonationEntries ??= new List<Guid>();
                    _logger.LogInformation("Adding new transaction id to {Name}", donor.Name);
                    donorToUpdate.DonationEntries.Add(donor.TransactionId);
                }
            }
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Finished updating donor amounts and saved changes.");
        }
    }
}
