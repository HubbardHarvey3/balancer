using Bunit;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using Microsoft.Extensions.Logging;
using Balancer.Components.Pages;
using Balancer.Components.Services;
using Balancer.Components.Models;
using FluentAssertions;

namespace Balancer.Tests;

public class DonorsPageTests : TestContext
{
    private readonly Mock<IDonorService> _mockDonorService;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly Mock<ILogger<Donors>> _mockLogger;
    private readonly Mock<ISnackbar> _mockSnackbar;
    public DonorsPageTests()
    {
        _mockDonorService = new Mock<IDonorService>();
        _mockDialogService = new Mock<IDialogService>();
        _mockLogger = new Mock<ILogger<Donors>>(); 
        _mockSnackbar = new Mock<ISnackbar>();

        Services.AddMudServices();

        Services.AddSingleton(_mockDonorService.Object);
        Services.AddSingleton(_mockDialogService.Object);
        Services.AddSingleton(_mockLogger.Object);
        Services.AddSingleton(_mockSnackbar.Object);

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void DonorsPage_RendersCorrectly_AndLoadsInitialDonors()
    {
        var initialDonors = new List<DonorModel>
        {
            new() { DonorNumber = 1, Name = "Donor One", Address = "1 First St", TotalDonations = 100 },
            new() { DonorNumber = 2, Name = "Donor Two", Address = "2 Second St", TotalDonations = 200 }
        };
        _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(initialDonors);

        var cut = RenderComponent<Donors>(); 

        cut.Find("label:contains('First, Last')"); 
        cut.Find("button:contains('Save')"); 

        cut.Find("th:contains('Donor Number')");
        cut.Find("th:contains('Donor Name')");
        cut.Find("th:contains('Address')");
        cut.Find("th:contains('Total Donations')");
        cut.Find("th:contains('Actions')");

        var rows = cut.FindAll("tbody tr:contains('St')"); 
        rows.Should().HaveCount(initialDonors.Count); 

        var firstRow = rows[0]; 
        var cells = firstRow.QuerySelectorAll("td");

        cells.Should().HaveCount(5); 

        cells[0].TextContent.Should().Be(initialDonors[0].DonorNumber.ToString());
        cells[1].TextContent.Should().Be(initialDonors[0].Name);
        cells[2].TextContent.Should().Be(initialDonors[0].Address);
        cells[3].TextContent.Should().Be(initialDonors[0].TotalDonations.ToString());

        var actionCell = cells[4];
        actionCell.QuerySelector("button:contains('Edit')").Should().NotBeNull(); 
        actionCell.QuerySelector("button:contains('Delete')").Should().NotBeNull();

        _mockDonorService.Verify(s => s.GetDonorsAsync(), Times.Once);
    }
    [Fact]
    public void AddDonor_WhenValidInput_CallsServiceAndReloadsList()
    {
        var initialDonors = new List<DonorModel>(); 
        var addedDonor = new DonorModel(); 

        _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(() => new List<DonorModel>(initialDonors)); 
        _mockDonorService.Setup(s => s.AddDonorsAsync(It.IsAny<DonorModel>()))
            .Callback<DonorModel>(donor =>
            {
                addedDonor = donor;
                donor.DonorNumber = 99; 
                initialDonors.Add(donor);
            })
            .Returns(Task.CompletedTask);

        var cut = RenderComponent<Donors>();

        var nameLabel = cut.Find("label:contains('First, Last')"); 
        var nameInputId = nameLabel.Attributes["for"]?.Value;
        var nameInput = cut.Find($"input#{nameInputId}");
        
        var addressLabel = cut.Find("label:contains('Address')");
        var addressInputId = addressLabel.Attributes["for"]?.Value;
        var addressInput = cut.Find($"input#{addressInputId}");
        
        nameInput.Change("New Donor");
        addressInput.Change("123 New Ave");

        cut.InvokeAsync(() => cut.FindAll("button:contains('Save')").First().Click());

        _mockDonorService.Verify(s => s.AddDonorsAsync(It.Is<DonorModel>(d =>
            d.Name == "New Donor" &&
            d.Address == "123 New Ave"
        )), Times.Once);

        _mockDonorService.Verify(s => s.GetDonorsAsync(), Times.Exactly(2));

        cut.WaitForAssertion(() =>
        {
            var donorListTable = cut.Find("th:contains('Donor Number')").Closest("table");
            donorListTable.Should().NotBeNull("Donor list table should be found");

            var rows = donorListTable.QuerySelectorAll("tbody tr");
            rows.Should().HaveCount(1, "because one donor should have been added");

            var cells = rows[0].QuerySelectorAll("td");
            cells.Should().HaveCount(5); 

            cells[0].TextContent.Trim().Should().Be(addedDonor.DonorNumber.ToString());
            cells[1].TextContent.Trim().Should().Be(addedDonor.Name);
            cells[2].TextContent.Trim().Should().Be(addedDonor.Address);
            cells[3].TextContent.Trim().Should().Be(addedDonor.TotalDonations.ToString());

            var actionCell = cells[4];
            actionCell.QuerySelector("button:contains('Edit')").Should().NotBeNull();
            actionCell.QuerySelector("button:contains('Delete')").Should().NotBeNull();

        }, TimeSpan.FromSeconds(2)); 

        var nameLabelAfter = cut.Find("label:contains('First, Last')");
        var nameInputIdAfter = nameLabelAfter.Attributes["for"]?.Value;
        var nameInputAfter = cut.Find($"input#{nameInputIdAfter}");
        nameInputAfter.GetAttribute("value").Should().BeEmpty("Name input should be cleared after save");

        var addressLabelAfter = cut.Find("label:contains('Address')");
        var addressInputIdAfter = addressLabelAfter.Attributes["for"]?.Value;
        var addressInputAfter = cut.Find($"input#{addressInputIdAfter}");
        addressInputAfter.GetAttribute("value").Should().BeEmpty("Address input should be cleared after save");
    }

    [Fact]
    public void DeleteDonor_WhenConfirmed_CallsServiceAndReloads()
    {
        var donorToDelete = new DonorModel { DonorNumber = 1, Name = "Donor One", Address = "1 First St" };
        var initialDonors = new List<DonorModel> { donorToDelete };

        _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(() => new List<DonorModel>(initialDonors));
        _mockDonorService.Setup(s => s.DeleteDonorAsync(donorToDelete.DonorNumber))
            .Callback<int>(_ => initialDonors.Remove(donorToDelete))
            .Returns(Task.CompletedTask);

        var mockDialogReference = new Mock<IDialogReference>();
        mockDialogReference.Setup(r => r.Result).Returns(Task.FromResult<DialogResult?>(DialogResult.Ok(true)));
        _mockDialogService.Setup(d => d.ShowAsync<ConfirmDeleteDialog>(
                It.IsAny<string>(), 
                It.IsAny<DialogParameters>() 
            ))
            .Returns(Task.FromResult(mockDialogReference.Object));

        var cut = RenderComponent<Donors>();

        var donorListTable = cut.Find("th:contains('Donor Number')").Closest("table");
        donorListTable.Should().NotBeNull("Donor list table should be found");

        var row = donorListTable.QuerySelector("tbody tr");
        row.Should().NotBeNull("The table row for the donor should be found.");

        var deleteButton = row.QuerySelector("button:contains('Delete')");
        deleteButton.Should().NotBeNull("Delete button should be found in the row");

        deleteButton.Click();

        _mockDialogService.Verify(d => d.ShowAsync<ConfirmDeleteDialog>(
            "Confirm",
            It.Is<DialogParameters>(p => p.Get<string>("ContentText") == "Are you sure you want to delete this donor")
            ), Times.Once);

        _mockDonorService.Verify(s => s.DeleteDonorAsync(donorToDelete.DonorNumber), Times.Once);
        _mockDonorService.Verify(s => s.GetDonorsAsync(), Times.Exactly(2));

        cut.WaitForAssertion(() =>
        {
            var updatedTable = cut.Find("th:contains('Donor Number')").Closest("table");
            updatedTable.Should().NotBeNull("Donor list table should still exist after delete"); 
            updatedTable.QuerySelectorAll("tbody tr").Should().BeEmpty("because the donor should have been removed");
        });
    }

    [Fact]
    public void DeleteDonor_WhenCancelled_DoesNotCallService()
    {
        var donorToDelete = new DonorModel { DonorNumber = 1, Name = "Donor One", Address = "1 First St" };
        var initialDonors = new List<DonorModel> { donorToDelete };

        _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(initialDonors);

        var mockDialogReference = new Mock<IDialogReference>();
        mockDialogReference.Setup(r => r.Result).Returns(Task.FromResult<DialogResult?>(DialogResult.Cancel()));
        _mockDialogService.Setup(d => d.ShowAsync<ConfirmDeleteDialog>(
                It.IsAny<string>(), 
                It.IsAny<DialogParameters>()
            ))
             .Returns(Task.FromResult(mockDialogReference.Object));

        var cut = RenderComponent<Donors>();

        var donorListTable = cut.Find("th:contains('Donor Number')").Closest("table");
        donorListTable.Should().NotBeNull("Donor list table should be found");

        var row = donorListTable.QuerySelector("tbody tr");
        row.Should().NotBeNull("The table row for the donor should be in the right table");

        var deleteButton = row.QuerySelector("button:contains('Delete')");
        deleteButton.Should().NotBeNull("Delete button should be found in the row");

        deleteButton.Click(); 

        _mockDialogService.Verify(d => d.ShowAsync<ConfirmDeleteDialog>(It.IsAny<string>(), It.IsAny<DialogParameters>()), Times.Once);
        _mockDonorService.Verify(s => s.DeleteDonorAsync(It.IsAny<int>()), Times.Never);
        _mockDonorService.Verify(s => s.GetDonorsAsync(), Times.Once);

        cut.WaitForAssertion(() =>
        {
            var updatedTable = cut.Find("th:contains('Donor Number')").Closest("table");
            updatedTable.Should().NotBeNull("Donor list table should still exist after delete"); 
            updatedTable.QuerySelectorAll("tbody tr").Should().NotBeEmpty("because the donor should not have been removed");
        });
    }

  [Fact]
  public async Task DeleteDonor_DirectCall_CompletesWithoutError()
  {
      var donorToDelete = new DonorModel { DonorNumber = 1, Name = "Donor One", Address = "1 First St" };
      var initialDonors = new List<DonorModel> { donorToDelete };

      _mockDonorService.Setup(s => s.GetDonorsAsync()).ReturnsAsync(() => new List<DonorModel>(initialDonors));
      _mockDonorService.Setup(s => s.DeleteDonorAsync(donorToDelete.DonorNumber))
          .Callback<int>(_ => initialDonors.Remove(donorToDelete))
          .Returns(Task.CompletedTask);

      var cut = RenderComponent<Donors>();

      var donorListTable = cut.Find("th:contains('Donor Number')").Closest("table");
      donorListTable.Should().NotBeNull("Donor list table should be found initially");
      donorListTable.QuerySelectorAll("tbody tr").Should().HaveCount(1, "because the initial donor should be present");


      await cut.InvokeAsync(() => cut.Instance.DeleteDonor(donorToDelete.DonorNumber));

      _mockDonorService.Verify(s => s.DeleteDonorAsync(donorToDelete.DonorNumber), Times.Once);
      _mockDonorService.Verify(s => s.GetDonorsAsync(), Times.Exactly(2)); 

      cut.WaitForAssertion(() =>
      {
          var updatedDonorListTable = cut.Find("th:contains('Donor Number')").Closest("table");
          updatedDonorListTable.Should().NotBeNull("Donor list table should still exist after delete");
          updatedDonorListTable.QuerySelectorAll("tbody tr").Should().BeEmpty("because the donor should have been removed");
      });
    }
}
