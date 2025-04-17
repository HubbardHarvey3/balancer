using Balancer.Components.Models;
using Balancer.Components.Services;
using Moq;

namespace Balancer.Tests
{
    public class CheckServiceTests
    {
        private readonly Mock<IDonationEntryService> _mockDonationEntryService;
        private readonly CheckService _checkService;

        public CheckServiceTests()
        {
            _mockDonationEntryService = new Mock<IDonationEntryService>();
            _checkService = new CheckService(_mockDonationEntryService.Object);
        }

        [Fact]
        public async Task GetTotalDonationCheckAmount_ReturnsCorrectSum()
        {
            var testDate = new DateOnly(2025, 1, 1);
            var mockDonorList = new List<DonationEntryModel>
            {
                new() { Check = 100m },
                new() { Check = 150m },
                new() { Check = 300m }
            };
            var resultList = new List<DonationEntryModel>();
            _mockDonationEntryService
                .Setup(s => s.GetDonorEntriesByDateAsync(testDate))
                .ReturnsAsync(mockDonorList);

            var total = await _checkService.GetTotalDonationCheckAmount(testDate);

            Assert.Equal(550m, total);

        }

        [Fact]
        public void IsCheckAmountInError_ReturnsTrue()
        {
            var checkAmounts = 11;
            var donorCheckAmounts = 10;

            Assert.True(_checkService.IsCheckAmountsInError(checkAmounts, donorCheckAmounts));
        }

        [Fact]
        public void IsCheckAmountInError_ReturnsFalse()
        {
            var checkAmounts = 11;
            var donorCheckAmounts = 11;

            Assert.False(_checkService.IsCheckAmountsInError(checkAmounts, donorCheckAmounts));
        }

    }
}
