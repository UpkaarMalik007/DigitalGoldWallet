using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DigitalGoldWallet.Tests.UnitTests.Controllers;

public class VendorControllerTests
{
    private readonly Mock<IVendorService> _vendorServiceMock;
    private readonly VendorController _vendorController;

    public VendorControllerTests()
    {
        _vendorServiceMock = new Mock<IVendorService>();
        _vendorController = new VendorController(_vendorServiceMock.Object);
    }

    // ─── POSITIVE TESTS ────────────

    [Fact]
    public async Task GetVendorById_ShouldReturnOkResult_WhenVendorExists()
    {
        var vendor = VendorTestDataFactory.VendorDetailsDto(); 
        _vendorServiceMock
            .Setup(s => s.GetVendorByIdAsync(1))
            .ReturnsAsync(vendor);

        var result = await _vendorController.GetVendorById(1);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(s => s.GetVendorByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task SearchVendors_ShouldReturnOkResult_WhenSearchNameIsValid()
    {
        var vendors = new List<VendorListDto>
        {
            VendorTestDataFactory.VendorListDto() 
        };

        _vendorServiceMock
            .Setup(s => s.SearchVendorsByNameAsync("sona"))
            .ReturnsAsync(vendors);

        var result = await _vendorController.SearchVendors("sona");

        var okResult = result.Should()
            .BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        _vendorServiceMock.Verify(s => s.SearchVendorsByNameAsync("sona"), Times.Once);
    }

    [Fact]
    public async Task CreateVendor_ShouldReturnCreatedAtActionResult_WhenVendorIsCreated()
    {
        var createDto = VendorTestDataFactory.CreateVendorDto(); 
        var createdVendor = VendorTestDataFactory.VendorDetailsDto(); 

        _vendorServiceMock
            .Setup(s => s.CreateVendorAsync(createDto))
            .ReturnsAsync(createdVendor);

        var result = await _vendorController.CreateVendor(createDto);

        var createdResult = result.Should()
            .BeOfType<CreatedAtActionResult>().Subject;

        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(s => s.CreateVendorAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task GetVendorPrice_ShouldReturnOkResult_WhenVendorExists()
    {
        _vendorServiceMock
            .Setup(s => s.GetVendorPriceAsync(1))
            .ReturnsAsync(6500);

        var result = await _vendorController.GetVendorPrice(1);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        _vendorServiceMock.Verify(s => s.GetVendorPriceAsync(1), Times.Once);
    }

   

    [Fact]
    public async Task GetVendorById_ShouldThrowNotFoundException_WhenVendorDoesNotExist()
    {
        _vendorServiceMock
            .Setup(s => s.GetVendorByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Vendor not found."));

        Func<Task> action = async () => await _vendorController.GetVendorById(999);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Vendor not found.");

        _vendorServiceMock.Verify(s => s.GetVendorByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task SearchVendors_ShouldThrowBadRequestException_WhenSearchNameIsEmpty()
    {
        _vendorServiceMock
            .Setup(s => s.SearchVendorsByNameAsync(""))
            .ThrowsAsync(new BadRequestException("Search name is required."));

        Func<Task> action = async () => await _vendorController.SearchVendors("");

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Search name is required.");

        _vendorServiceMock.Verify(s => s.SearchVendorsByNameAsync(""), Times.Once);
    }

    [Fact]
    public async Task CreateVendor_ShouldThrowValidationException_WhenDtoIsInvalid()
    {
        var createDto = new CreateVendorDto
        {
            VendorName = "",
            CurrentGoldPrice = 0,
            Password = ""
        };

        _vendorServiceMock
            .Setup(s => s.CreateVendorAsync(createDto))
            .ThrowsAsync(new ValidationException("Validation failed."));

        Func<Task> action = async () => await _vendorController.CreateVendor(createDto);

        await action.Should().ThrowAsync<ValidationException>();

        _vendorServiceMock.Verify(s => s.CreateVendorAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task GetVendorPrice_ShouldThrowNotFoundException_WhenVendorDoesNotExist()
    {
        _vendorServiceMock
            .Setup(s => s.GetVendorPriceAsync(999))
            .ThrowsAsync(new NotFoundException("Vendor not found."));

        Func<Task> action = async () => await _vendorController.GetVendorPrice(999);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Vendor not found.");

        _vendorServiceMock.Verify(s => s.GetVendorPriceAsync(999), Times.Once);
    }
}