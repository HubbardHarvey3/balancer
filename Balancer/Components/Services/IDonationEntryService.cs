using Balancer.Components.Models;

namespace Balancer.Components.Services
{
    internal interface IDonationEntryService
    {
        Task<List<DonationEntryModel>> GetDonorEntriesAsync();
        Task<List<DonationEntryModel>> GetDonorEntriesByDateAsync(DateOnly entryDate);
        Task SaveDonationEntry(DonationEntryModel donation);
        Task DeleteDonorEntryAsync(Guid entryId);

    }
}
