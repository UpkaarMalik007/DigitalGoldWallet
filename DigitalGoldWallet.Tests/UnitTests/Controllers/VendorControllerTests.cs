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


    [Fact]
    public async Task GetVendorById_ShouldReturnOkResult_WhenVendorExists()
    {
        VendorDto vendor = VendorTestDataFactory.VendorDto();

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

    [Fact]
    public async Task SearchVendors_ShouldReturnOkResult_WhenSearchNameIsValid()
    {
        List<VendorDto> vendors =
        [
            VendorTestDataFactory.VendorDto()
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


    [Fact]
    public async Task CreateVendor_ShouldReturnCreatedAtActionResult_WhenVendorIsCreated()
    {
        VendorDto createDto = VendorTestDataFactory.CreateVendorDto();

        VendorDto createdVendor = VendorTestDataFactory.VendorDto();
        createdVendor.VendorId = 10;
        createdVendor.VendorName = createDto.VendorName;
        createdVendor.ContactEmail = createDto.ContactEmail;
        createdVendor.CurrentGoldPrice = createDto.CurrentGoldPrice;
        createdVendor.Password = null;

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

    [Fact]
    public async Task CreateVendor_ShouldThrowValidationException_WhenDtoIsInvalid()
    {
        VendorDto createDto = new()
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

    [Fact]
    public async Task GetVendorPrice_ShouldReturnOkResult_WhenVendorExists()
    {
        _vendorServiceMock
            .Setup(service => service.GetVendorPriceAsync(1))
            .ReturnsAsync(6500);

        IActionResult result = await _vendorController.GetVendorPrice(1);

        OkObjectResult okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().NotBeNull();

        _vendorServiceMock.Verify(
            service => service.GetVendorPriceAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetVendorPrice_ShouldThrowNotFoundException_WhenVendorDoesNotExist()
    {
        _vendorServiceMock
            .Setup(service => service.GetVendorPriceAsync(999))
            .ThrowsAsync(new NotFoundException("Vendor not found."));

        Func<Task> action = async () => await _vendorController.GetVendorPrice(999);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Vendor not found.");

        _vendorServiceMock.Verify(
            service => service.GetVendorPriceAsync(999),
            Times.Once);
    }
}