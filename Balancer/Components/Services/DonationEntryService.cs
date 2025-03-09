using Balancer.Components.Data;
using Balancer.Components.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Balancer.Components.Services
{
    internal class DonationEntryService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<DonationEntryService> _logger;

        public DonationEntryService(ApplicationDBContext dbContext, ILogger<DonationEntryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<DonationEntryModel>> GetDonorEntriesAsync()
        {
            _logger.LogInformation("Getting Donor Entries");
            return await _dbContext.Donations.ToListAsync();
        }

        public async Task<List<DonationEntryModel>> GetDonorEntriesByDateAsync(DateOnly entryDate)
        {
            _logger.LogInformation("Getting Entries for {0}", entryDate);
            return await _dbContext.Donations
                .Where(entry => entry.Date.Equals(entryDate))
                .ToListAsync();
        }

        public async Task SaveDonationEntry(DonationEntryModel donation)
        {
            _logger.LogInformation("Saving Donation Entry");
            _dbContext.Donations.Add(donation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDonorEntryAsync(Guid entryId)
        {
            var result = await _dbContext.Donations.FindAsync(entryId);
            if (result != null)
            {
                _logger.LogInformation("Deleting Donation Entry : {id}", entryId);
                _dbContext.Donations.Remove(result);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
