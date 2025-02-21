using Microsoft.EntityFrameworkCore;
using Balancer.Components.Data;
using Balancer.Components.Models;
using Microsoft.Extensions.Logging;

namespace Balancer.Components.Services
{
    internal class DonorEntryService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<DonorService> _logger;
        public async Task<List<DonationEntryModel>> GetDonorsAsync()
        {
            return await _dbContext.Donors.ToListAsync();
        }
    }
}
