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
    private readonly ApplicationDBContext _dbContext;
    private readonly DonorService _donorService;
    private readonly ILogger<DonorService> _logger;

    public DonorServiceTest()
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
        // Arrange - Seed the database
        _dbContext.Donors.AddRange(new List<DonorModel>
        {
            new() { DonorNumber = 1, Name = "John Doe", TotalDonations = 150.50m, Address = "123 Main St" },
            new() { DonorNumber = 2, Name = "Jane Smith", TotalDonations = 200.00m, Address = "456 Elm St" },
            new() { DonorNumber = 3, Name = "Mike Johnson", TotalDonations = 75.25m, Address = "789 Oak St" }
        });

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _donorService.GetDonorsAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetSingleDonorAsync_ReturnsOneDonor()
    {
        // Arrange
        var donor = new DonorModel
        {
            DonorNumber = 1,
            Name = "Lone Survivor",
            TotalDonations = 100.50m,
            Address = "12 Sanctuary Hills"
        };

        _dbContext.Donors.Add(donor);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _donorService.GetSingleDonorAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(donor.Name, result.Name);
    }

    [Fact]
    public async Task AddDonorsAsync_AddsNewDonor()
    {
        // Arrange
        var newDonor = new DonorModel
        {
            DonorNumber = 1,
            Name = "John Doe",
            TotalDonations = 150.50m,
            Address = "123 Main St"
        };

        // Act
        await _donorService.AddDonorsAsync(newDonor);

        // Assert
        var donorInDb = await _dbContext.Donors.FindAsync(1);
        Assert.NotNull(donorInDb);
        Assert.Equal("John Doe", donorInDb.Name);
    }

    [Fact]
    public async Task UpdateDonorAsync_UpdatesExistingDonor()
    {
        // Arrange
        var donor = new DonorModel
        {
            DonorNumber = 1,
            Name = "John Doe",
            TotalDonations = 150.50m,
            Address = "123 Main St"
        };

        _dbContext.Donors.Add(donor);
        await _dbContext.SaveChangesAsync();

        // Modify the donor
        donor.Name = "Updated Name";
        await _donorService.UpdateDonorAsync(donor);

        // Act
        var updatedDonor = await _dbContext.Donors.FindAsync(1);

        // Assert
        Assert.NotNull(updatedDonor);
        Assert.Equal("Updated Name", updatedDonor.Name);
    }

    [Fact]
    public async Task DeleteDonorAsync_RemovesDonor()
    {
        // Arrange
        var donor = new DonorModel
        {
            DonorNumber = 1,
            Name = "John Doe",
            TotalDonations = 150.50m,
            Address = "123 Main St"
        };

        _dbContext.Donors.Add(donor);
        await _dbContext.SaveChangesAsync();

        // Act
        await _donorService.DeleteDonorAsync(1);
        var deletedDonor = await _dbContext.Donors.FindAsync(1);

        // Assert
        Assert.Null(deletedDonor);
    }
}

}
