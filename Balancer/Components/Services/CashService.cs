using Balancer.Components.Models;

namespace Balancer.Components.Services
{
    internal class CashService 
    {
        public Dictionary<string, int> DenominationAmounts { get; private set; }
        private readonly IDonationEntryService _donationEntryService;

        public CashService(IDonationEntryService donationEntryService)
        {
            _donationEntryService = donationEntryService;
            DenominationAmounts = new()
            {
                { "$100", 0 }, { "$50", 0 }, { "$20", 0 }, { "$10", 0 },
                { "$5", 0 }, { "$1", 0 }, { "¢50", 0 }, { "¢25", 0 },
                { "¢10", 0 }, { "¢5", 0 }, { "¢1", 0 }
            };
        }
        public void UpdateDenomination(string denomination, int amount)
        {
            if (DenominationAmounts.ContainsKey(denomination))
            {
                DenominationAmounts[denomination] = amount;
            }
        }

        public decimal GetTotalCashAmount()
        {
            var values = new Dictionary<string, decimal>
            {
                { "$100", 100m }, { "$50", 50m }, { "$20", 20m }, { "$10", 10m },
                { "$5", 5m }, { "$1", 1m }, { "¢50", 0.50m }, { "¢25", 0.25m },
                { "¢10", 0.10m }, { "¢5", 0.05m }, { "¢1", 0.01m }
            };

            decimal total = 0;
            foreach (var kvp in DenominationAmounts)
            {
                total += values[kvp.Key] * kvp.Value;
            }
            return total;
        }

        public async Task<decimal> GetTotalDonationCashAmount(DateOnly date)
        {
            decimal total = 0;
            var resultsList = new List<DonationEntryModel>();
            resultsList = await _donationEntryService.GetDonorEntriesByDateAsync(date);
            foreach (var entry in resultsList)
            {
                total = total + entry.Cash;
            }
            return total;
        }
    }

}
