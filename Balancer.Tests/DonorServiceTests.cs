using Balancer.Components.Data;
using Balancer.Components.Models;
using Balancer.Components.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Balancer.Tests
{
    public class DonorServiceTests
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DonorService _donorService;
        private readonly ILogger<DonorService> _logger;

        public DonorServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            var mockLogger = new Mock<ILogger<DonorService>>();
            _logger = mockLogger.Object;

            _dbContext = new ApplicationDBContext(options);
            _donorService = new DonorService(_dbContext, _logger);
        }

        [Fact]
        public async Task GetDonorsAsync_ReturnsAllDonors()
        {

             // Use a unique database name for this test
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "GetDonorsAsync_DB")
                .Options;

            // Create a fresh DbContext and Service instance for this test
            using var testDbContext = new ApplicationDBContext(options);
            var logger = new LoggerFactory().CreateLogger<DonorService>();
            var donorService = new DonorService(testDbContext, logger);

            testDbContext.Donors.AddRange(new List<DonorModel>
            {
                new() { DonorNumber = 1, Name = "John Doe", TotalDonations = 150.50m, Address = "123 Main St" },
                new() { DonorNumber = 2, Name = "Jane Smith", TotalDonations = 200.00m, Address = "456 Elm St" },
                new() { DonorNumber = 3, Name = "Mike Johnson", TotalDonations = 75.25m, Address = "789 Oak St" }
            });

            await testDbContext.SaveChangesAsync();

            var result = await donorService.GetDonorsAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetSingleDonorAsync_ReturnsOneDonor()
        {
            var donor = new DonorModel
            {
                DonorNumber = 4,
                Name = "Lone Survivor",
                TotalDonations = 100.50m,
                Address = "12 Sanctuary Hills"
            };

            _dbContext.Donors.Add(donor);
            await _dbContext.SaveChangesAsync();

            var result = await _donorService.GetSingleDonorAsync(4);

            Assert.NotNull(result);
            Assert.Equal(donor.Name, result.Name);
        }

        [Fact]
        public async Task AddDonorsAsync_AddsNewDonor()
        {
            var newDonor = new DonorModel
            {
                DonorNumber = 6,
                Name = "John Doe",
                TotalDonations = 150.50m,
                Address = "123 Main St"
            };

            await _donorService.AddDonorsAsync(newDonor);

            var donorInDb = await _dbContext.Donors.FindAsync(6);
            Assert.NotNull(donorInDb);
            Assert.Equal("John Doe", donorInDb.Name);
        }

        [Fact]
        public async Task UpdateDonorAsync_UpdatesExistingDonor()
        {
            var donor = new DonorModel
            {
                DonorNumber = 7,
                Name = "John Doe",
                TotalDonations = 150.50m,
                Address = "123 Main St"
            };

            _dbContext.Donors.Add(donor);
            await _dbContext.SaveChangesAsync();

            donor.Name = "Updated Name";
            await _donorService.UpdateDonorAsync(donor);

            var updatedDonor = await _dbContext.Donors.FindAsync(7);

            Assert.NotNull(updatedDonor);
            Assert.Equal("Updated Name", updatedDonor.Name);
        }

        [Fact]
        public async Task DeleteDonorAsync_RemovesDonor()
        {
            var donor = new DonorModel
            {
                DonorNumber = 8,
                Name = "John Doe",
                TotalDonations = 150.50m,
                Address = "123 Main St"
            };

            _dbContext.Donors.Add(donor);
            await _dbContext.SaveChangesAsync();

            await _donorService.DeleteDonorAsync(8);
            var deletedDonor = await _dbContext.Donors.FindAsync(8);

            Assert.Null(deletedDonor);
        }
    }

}
