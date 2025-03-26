using Balancer.Components.Services;
using Balancer.Components.Models;
using Moq;

namespace Balancer.Tests
{
    public class DonorServiceTest
    {
        private readonly Mock<IDonorService> _mockDonorService;

        public DonorServiceTest()
        { 
            _mockDonorService = new Mock<IDonorService>();
        }

        [Fact]
        public async Task GetDonorsAsync_ReturnsAllDonors()
        {
            var mockDonors = new List<DonorModel>
            {
                new() { DonorNumber = 1, Name = "John Doe", TotalDonations = 150.50m, Address = "123 Main St" },
                new() { DonorNumber = 2, Name = "Jane Smith", TotalDonations = 200.00m, Address = "456 Elm St" },
                new() { DonorNumber = 3, Name = "Mike Johnson", TotalDonations = 75.25m, Address = "789 Oak St" }
            };

            _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(mockDonors);

            var result = await _mockDonorService.Object.GetDonorsAsync();

            Assert.Equal(mockDonors, result);
        }
    }
}
