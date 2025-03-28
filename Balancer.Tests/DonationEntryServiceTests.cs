using Balancer.Components.Data;
using Balancer.Components.Models;
using Balancer.Components.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Balancer.Tests
{
    public class DonationEntryServiceTests
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DonationEntryService _donationEntryService;
        private readonly ILogger<DonationEntryService> _logger;

        public DonationEntryServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Donations")
                .Options;

            _dbContext = new ApplicationDBContext(options);

            // Create a mock logger
            var mockLogger = new Mock<ILogger<DonationEntryService>>();
            _logger = mockLogger.Object;

            // Instantiate the service
            _donationEntryService = new DonationEntryService(_dbContext, _logger);
        }

        [Fact]
        public async Task GetDonorEntriesAsync_ReturnsAllEntries()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "GetDonorEntries_DB")
                .Options;

            using var testDbContext = new ApplicationDBContext(options);
            var logger = new LoggerFactory().CreateLogger<DonationEntryService>();
            var donationEntryService = new DonationEntryService(testDbContext, logger);

            testDbContext.Donations.AddRange(new List<DonationEntryModel>
            {
                new() { TransactionId = Guid.NewGuid(), Name = "Elle", DonorNumber = 1, Cash = 100.00m, Date = DateOnly.FromDateTime(DateTime.Today) },
                new() { TransactionId = Guid.NewGuid(), Name = "Susan", DonorNumber = 2, Check = 50.00m, Date = DateOnly.FromDateTime(DateTime.Today) }
            });
            await testDbContext.SaveChangesAsync();

            // Act
            var result = await donationEntryService.GetDonorEntriesAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetDonorEntriesByDateAsync_ReturnsCorrectEntries()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "GetDonorEntriesByDate_DB")
                .Options;

            using var testDbContext = new ApplicationDBContext(options);
            var logger = new LoggerFactory().CreateLogger<DonationEntryService>();
            var donationEntryService = new DonationEntryService(testDbContext, logger);

            // Arrange
            var testDate = DateOnly.FromDateTime(DateTime.Today);
            testDbContext.Donations.AddRange(new List<DonationEntryModel>
            {
                new() { TransactionId = Guid.NewGuid(), Name = "Bob", DonorNumber = 1, Cash = 100.00m, Date = testDate },
                new() { TransactionId = Guid.NewGuid(), Name = "Bobbi", DonorNumber = 2, Cash = 10m, Check = 50.00m, Date = testDate },
                new() { TransactionId = Guid.NewGuid(), Name = "Sue", DonorNumber = 3, Cash = 75.00m, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)) }
            });
            await testDbContext.SaveChangesAsync();

            // Act
            var result = await donationEntryService.GetDonorEntriesByDateAsync(testDate);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SaveDonationEntry_AddsEntryToDatabase()
        {
            // Arrange
            var newEntry = new DonationEntryModel
            {
                TransactionId = Guid.NewGuid(),
                Name = "Lone Survivor",
                DonorNumber = 1,
                Cash = 200.00m,
                Date = DateOnly.FromDateTime(DateTime.Today)
            };

            // Act
            await _donationEntryService.SaveDonationEntry(newEntry);

            // Assert
            var entryInDb = await _dbContext.Donations.FindAsync(newEntry.TransactionId);
            Assert.NotNull(entryInDb);
            Assert.Equal(200.00m, entryInDb.Cash);
        }

        [Fact]
        public async Task DeleteDonorEntryAsync_RemovesEntryFromDatabase()
        {
            var entryToDelete = new DonationEntryModel
            {
                TransactionId = Guid.NewGuid(),
                Name = "Tom",
                DonorNumber = 1,
                Check = 50.00m,
                Date = DateOnly.FromDateTime(DateTime.Today)
            };

            _dbContext.Donations.Add(entryToDelete);
            await _dbContext.SaveChangesAsync();

            await _donationEntryService.DeleteDonorEntryAsync(entryToDelete.TransactionId);

            var deletedEntry = await _dbContext.Donations.FindAsync(entryToDelete.TransactionId);
            Assert.Null(deletedEntry);
        }
    }
}
