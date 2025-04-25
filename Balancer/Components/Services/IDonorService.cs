using Balancer.Components.Models;

namespace Balancer.Components.Services
{
    internal interface IDonorService
    {
        Task<List<DonorModel>> GetDonorsAsync();
        Task<DonorModel?> GetSingleDonorAsync(int donorNumber);
        Task AddDonorsAsync(DonorModel donor);
        Task UpdateDonorAsync(DonorModel donor);
        Task DeleteDonorAsync(int donorNumber);
        Task UpdateDonorAmounts(List<DonationEntryModel> donorEntryList);

    }
}
