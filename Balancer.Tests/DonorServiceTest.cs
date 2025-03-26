using Balancer.Components.Services;
using Balancer.Components.Models;
using Moq;
using System.Numerics;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Balancer.Components.Data;
using Microsoft.Extensions.Logging;

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

        [Fact]
        public async Task GetSingleDonorAsync_ReturnOneDonor()
        {
            var mockDonor = new DonorModel()
            {
                DonorNumber = 1,
                Name = "Lone Survivor",
                TotalDonations = 100.50m,
                Address = "12 Sanctuary Hills"
            };

            _mockDonorService.Setup(s => s.GetSingleDonorAsync(1)).ReturnsAsync(mockDonor);

            var testResult = await _mockDonorService.Object.GetSingleDonorAsync(1);

            Assert.Equal(mockDonor, testResult);
        }

        [Fact]
        public async Task AddDonorsAsync_CallsAddMethod()
        {
            var mockDonorService = new Mock<IDonorService>(); // Mock the interface
            var newDonor = new DonorModel
            {
                DonorNumber = 1,
                Name = "John Doe",
                TotalDonations = 150.50m,
                Address = "123 Main St"
            };

            await mockDonorService.Object.AddDonorsAsync(newDonor);

            mockDonorService.Verify(s => s.AddDonorsAsync(It.Is<DonorModel>(d => d.DonorNumber == 1)), Times.Once);
        }

    }
}
