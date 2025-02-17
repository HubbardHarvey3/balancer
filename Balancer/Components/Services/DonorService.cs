using Microsoft.EntityFrameworkCore;
using Balancer.Components.Data;
using Balancer.Components.Models;

namespace Balancer.Components.Services
{
    internal class DonorService
    {
        private readonly ApplicationDBContext _dbContext;

        public DonorService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DonorModel>> GetDonorsAsync()
        {
            return await _dbContext.Donors.ToListAsync();
        }

        public async Task AddDonorsAsync(DonorModel donor)
        {
            _dbContext.Donors.Add(donor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateDonorAsync(DonorModel donor)
        {
            _dbContext.Donors.Update(donor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDonorAsync(int donorNumber)
        {
            var donor = await _dbContext.Donors.FindAsync(donorNumber);
            if (donor != null)
            {
                _dbContext.Donors.Remove(donor);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
