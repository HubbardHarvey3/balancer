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

            testDbContext.Donors.RemoveRange(_dbContext.Donors);
            await _dbContext.SaveChangesAsync();
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

            await _donorService.DeleteDonorAsync(4);
            await _dbContext.SaveChangesAsync();
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

            await _donorService.DeleteDonorAsync(6);
            await _dbContext.SaveChangesAsync();
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

            await _donorService.DeleteDonorAsync(7);
            await _dbContext.SaveChangesAsync();
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

        // This tes was written by Gemini
        [Fact]
        public async Task UpdateDonorAmounts_AddsTransactionIdsAndHandlesMissingDonors()
        {
            // Arrange
            var donor1 = new DonorModel 
            { 
                DonorNumber = 101, 
                Name = "Alice", 
                DonationEntries = null 
            }; 
            var donor2 = new DonorModel 
            { 
                DonorNumber = 102, 
                Name = "Bob", 
                DonationEntries = new List<Guid> 
                { 
                    Guid.NewGuid() 
                } 
            };
            _dbContext.Donors.AddRange(donor1, donor2);
            await _dbContext.SaveChangesAsync();

            // 2. Create donation entries list
            var transactionId1 = Guid.NewGuid();
            var transactionId2 = Guid.NewGuid();
            var transactionId3 = Guid.NewGuid();

            var donationEntries = new List<DonationEntryModel>
            {
                new() 
                { 
                    DonorNumber = 101, 
                    Name = "Alice Entry", 
                    TransactionId = transactionId1, 
                    Date = DateOnly.FromDateTime(DateTime.UtcNow) 
                },
                new() { 
                    DonorNumber = 102, 
                    Name = "Bob Entry", 
                    TransactionId = transactionId2, 
                    Date = DateOnly.FromDateTime(DateTime.UtcNow) 
                },
                new() 
                { 
                    DonorNumber = 999, 
                    Name = "Unknown Entry", 
                    TransactionId = transactionId3, 
                    Date = DateOnly.FromDateTime(DateTime.UtcNow) 
                }
            };

            // Act
            await _donorService.UpdateDonorAmounts(donationEntries);

            var updatedDonor1 = await _dbContext.Donors.FindAsync(101);
            var updatedDonor2 = await _dbContext.Donors.FindAsync(102);
            var nonExistentDonor = await _dbContext.Donors.FindAsync(999);

            // 2. Check donor 1 (was null)
            Assert.NotNull(updatedDonor1);
            Assert.NotNull(updatedDonor1.DonationEntries);
            Assert.Single(updatedDonor1.DonationEntries);
            Assert.Contains(transactionId1, updatedDonor1.DonationEntries);

            // 3. Check donor 2 (was not null)
            Assert.NotNull(updatedDonor2);
            Assert.NotNull(updatedDonor2.DonationEntries);
            Assert.Equal(2, updatedDonor2.DonationEntries.Count);
            Assert.Contains(transactionId2, updatedDonor2.DonationEntries);

            // 4. Check non-existent donor wasn't created
            Assert.Null(nonExistentDonor);

            // 5. Test empty list scenario (covers the loop not running)
            // Arrange
            var initialDonorCount = await _dbContext.Donors.CountAsync();
            var emptyList = new List<DonationEntryModel>();
            // Act
            await _donorService.UpdateDonorAmounts(emptyList);
            // Assert
            var finalDonorCount = await _dbContext.Donors.CountAsync();
            Assert.Equal(initialDonorCount, finalDonorCount);

            // Cleanup
            _dbContext.Donors.RemoveRange(updatedDonor1, updatedDonor2);
            await _dbContext.SaveChangesAsync();
        }
    }

}
