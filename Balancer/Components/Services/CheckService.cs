using Balancer.Components.Models;

namespace Balancer.Components.Services
{
    internal class CheckService
    {
        public List<decimal> _checkEntryAmounts;
        private readonly IDonationEntryService _donationEntryService;
        public CheckService(IDonationEntryService donationEntryService)
        {
            _donationEntryService = donationEntryService;
            _checkEntryAmounts = new List<decimal> { 0.00m, 0.00m, 0.00m };
        }
        public async Task<decimal> GetTotalDonationCheckAmount(DateOnly date)
        {
            decimal total = 0;
            var resultsList = new List<DonationEntryModel>();
            resultsList = await _donationEntryService.GetDonorEntriesByDateAsync(date);
            foreach (var entry in resultsList)
            {
                total = total + entry.Check;
            }
            return total;
        }
    }
}
