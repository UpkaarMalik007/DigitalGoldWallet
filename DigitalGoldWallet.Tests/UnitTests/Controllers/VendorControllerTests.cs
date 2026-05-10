using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
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

    // CASE 1: Vendor Login
    // Positive test
    [Fact]
    public async Task LoginVendor_ShouldReturnOkResult_WhenCredentialsAreValid()
    {
        VendorLoginDto loginDto = new()
        {
            ContactEmail = "rohit.sona@example.com",
            Password = "Vendor@123"
        };

        VendorLoginResponseDto loginResponse = new()
        {
            VendorId = 1,
            VendorName = "Sona Jewellers",
            ContactEmail = "rohit.sona@example.com",
            Role = "Vendor",
            Token = "fake-jwt-token"
        };

        _vendorServiceMock
            .Setup(service => service.LoginVendorAsync(loginDto))
            .ReturnsAsync(loginResponse);

        IActionResult result = await _vendorController.LoginVendor(loginDto);

        OkObjectResult okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(
            service => service.LoginVendorAsync(loginDto),
            Times.Once);
    }

    // CASE 1: Vendor Login
    // Negative test
    [Fact]
    public async Task LoginVendor_ShouldThrowBadRequestException_WhenCredentialsAreInvalid()
    {
        VendorLoginDto loginDto = new()
        {
            ContactEmail = "wrong@example.com",
            Password = "WrongPassword"
        };

        _vendorServiceMock
            .Setup(service => service.LoginVendorAsync(loginDto))
            .ThrowsAsync(new BadRequestException("Invalid email or password."));

        Func<Task> action = async () => await _vendorController.LoginVendor(loginDto);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Invalid email or password.");

        _vendorServiceMock.Verify(
            service => service.LoginVendorAsync(loginDto),
            Times.Once);
    }

    // CASE 2: Get Vendor By ID
    // Positive test
    [Fact]
    public async Task GetVendorById_ShouldReturnOkResult_WhenVendorExists()
    {
        VendorDetailsDto vendor = new()
        {
            VendorId = 1,
            VendorName = "Sona Jewellers",
            Description = "Trusted gold vendor",
            ContactPersonName = "Rohit Verma",
            ContactEmail = "rohit.sona@example.com",
            ContactPhone = "+91 9876541230",
            WebsiteUrl = "https://www.sonajewellers.com",
            TotalGoldQuantity = 2200,
            CurrentGoldPrice = 6500,
            CreatedAt = DateTime.UtcNow
        };

        _vendorServiceMock
            .Setup(service => service.GetVendorByIdAsync(1))
            .ReturnsAsync(vendor);

        IActionResult result = await _vendorController.GetVendorById(1);

        OkObjectResult okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(
            service => service.GetVendorByIdAsync(1),
            Times.Once);
    }

    // CASE 2: Get Vendor By ID
    // Negative test
    [Fact]
    public async Task GetVendorById_ShouldThrowNotFoundException_WhenVendorDoesNotExist()
    {
        _vendorServiceMock
            .Setup(service => service.GetVendorByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Vendor not found."));

        Func<Task> action = async () => await _vendorController.GetVendorById(999);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Vendor not found.");

        _vendorServiceMock.Verify(
            service => service.GetVendorByIdAsync(999),
            Times.Once);
    }

    // CASE 3: Search Vendors
    // Positive test
    [Fact]
    public async Task SearchVendors_ShouldReturnOkResult_WhenSearchNameIsValid()
    {
        List<VendorListDto> vendors =
        [
            new VendorListDto
            {
                VendorId = 1,
                VendorName = "Sona Jewellers",
                Description = "Trusted gold vendor",
                ContactEmail = "rohit.sona@example.com",
                ContactPhone = "+91 9876541230",
                WebsiteUrl = "https://www.sonajewellers.com",
                TotalGoldQuantity = 2200,
                CurrentGoldPrice = 6500
            }
        ];

        _vendorServiceMock
            .Setup(service => service.SearchVendorsByNameAsync("sona"))
            .ReturnsAsync(vendors);

        IActionResult result = await _vendorController.SearchVendors("sona");

        OkObjectResult okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(
            service => service.SearchVendorsByNameAsync("sona"),
            Times.Once);
    }

    // CASE 3: Search Vendors
    // Negative test
    [Fact]
    public async Task SearchVendors_ShouldThrowBadRequestException_WhenSearchNameIsEmpty()
    {
        _vendorServiceMock
            .Setup(service => service.SearchVendorsByNameAsync(""))
            .ThrowsAsync(new BadRequestException("Search name is required."));

        Func<Task> action = async () => await _vendorController.SearchVendors("");

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("Search name is required.");

        _vendorServiceMock.Verify(
            service => service.SearchVendorsByNameAsync(""),
            Times.Once);
    }

    // CASE 4: Create Vendor
    // Positive test
    [Fact]
    public async Task CreateVendor_ShouldReturnCreatedAtActionResult_WhenVendorIsCreated()
    {
        CreateVendorDto createDto = new()
        {
            VendorName = "Test Gold Vendor",
            Description = "Test vendor for authentication and registration",
            ContactPersonName = "Test Person",
            ContactEmail = "test.vendor@example.com",
            ContactPhone = "+91 9876543210",
            WebsiteUrl = "https://www.testgoldvendor.com",
            CurrentGoldPrice = 6500,
            Password = "Vendor@123"
        };

        VendorDetailsDto createdVendor = new()
        {
            VendorId = 10,
            VendorName = "Test Gold Vendor",
            Description = "Test vendor for authentication and registration",
            ContactPersonName = "Test Person",
            ContactEmail = "test.vendor@example.com",
            ContactPhone = "+91 9876543210",
            WebsiteUrl = "https://www.testgoldvendor.com",
            TotalGoldQuantity = 0,
            CurrentGoldPrice = 6500,
            CreatedAt = DateTime.UtcNow
        };

        _vendorServiceMock
            .Setup(service => service.CreateVendorAsync(createDto))
            .ReturnsAsync(createdVendor);

        IActionResult result = await _vendorController.CreateVendor(createDto);

        CreatedAtActionResult createdResult = result.Should()
            .BeOfType<CreatedAtActionResult>()
            .Subject;

        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.ActionName.Should().Be(nameof(VendorController.GetVendorById));
        createdResult.RouteValues!["id"].Should().Be(10);
        createdResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(
            service => service.CreateVendorAsync(createDto),
            Times.Once);
    }

    // CASE 4: Create Vendor
    // Negative test
    [Fact]
    public async Task CreateVendor_ShouldThrowValidationException_WhenDtoIsInvalid()
    {
        CreateVendorDto createDto = new()
        {
            VendorName = "",
            CurrentGoldPrice = 0,
            Password = ""
        };

        _vendorServiceMock
            .Setup(service => service.CreateVendorAsync(createDto))
            .ThrowsAsync(new ValidationException("Validation failed."));

        Func<Task> action = async () => await _vendorController.CreateVendor(createDto);

        await action.Should()
            .ThrowAsync<ValidationException>();

        _vendorServiceMock.Verify(
            service => service.CreateVendorAsync(createDto),
            Times.Once);
    }
}