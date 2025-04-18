using Balancer.Components.Models;
using Balancer.Components.Services;
using Moq;

namespace Balancer.Tests
{
    public class CashServiceTests
    {
        private readonly Mock<IDonationEntryService> _mockDonationEntryService;
        private readonly CashService _cashService;

        public CashServiceTests()
        {
            _mockDonationEntryService = new Mock<IDonationEntryService>();
            _cashService = new CashService(_mockDonationEntryService.Object);
        }

        [Fact]
        public void UpdateDenomination_ValidDenomination_UpdatesAmount()
        {
            _cashService.UpdateDenomination("$20", 5);

            Assert.Equal(5, _cashService.DenominationAmounts["$20"]);
        }

        [Fact]
        public void UpdateDenomination_InvalidDenomination_DoesNotThrow()
        {
            var exception = Record.Exception(() => _cashService.UpdateDenomination("Invalid", 5));

            Assert.Null(exception);
        }

        [Fact]
        public void GetTotalCashAmount_CalculatesCorrectTotal()
        {
            _cashService.UpdateDenomination("$100", 1);
            _cashService.UpdateDenomination("$50", 2);
            _cashService.UpdateDenomination("�25", 4); // 4 quarters = $1

            var total = _cashService.GetTotalCashAmount();

            Assert.Equal(201m, total); // 100 + (50*2) + 1
        }

        [Fact]
        public async Task GetTotalDonationCashAmount_ReturnsCorrectSum()
        {
            var testDate = new DateOnly(2025, 3, 17);
            var mockDonations = new List<DonationEntryModel>
        {
            new() { Cash = 50m },
            new() { Cash = 20m },
            new() { Cash = 30m }
        };

            _mockDonationEntryService
                .Setup(s => s.GetDonorEntriesByDateAsync(testDate))
                .ReturnsAsync(mockDonations);

            var total = await _cashService.GetTotalDonationCashAmount(testDate);

            Assert.Equal(100m, total);
        }

        [Fact]
        public void IsCashAmountInError_ReturnsTrue()
        {
            var denomCashAmounts = 11;
            var donorCashAmounts = 10;

            Assert.True(_cashService.IsCashAmountsInError(denomCashAmounts, donorCashAmounts));
        }

        [Fact]
        public void IsCashAmountInError_ReturnsFalse()
        {
            var denomCashAmounts = 11;
            var donorCashAmounts = 11;

            Assert.False(_cashService.IsCashAmountsInError(denomCashAmounts, donorCashAmounts));
        }
    }
}