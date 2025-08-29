using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Interfaces;
using HouseBrokerApp.Core.Enums;
using HouseBrokerApp.Infrastructure.Identity;
using HouseBrokerApp.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace HouseBrokerApp.Tests.Controllers
{
    public class ListingsControllerTests
    {
        private readonly Mock<IListingService> _mockListingService;
        private readonly Mock<IBrokerService> _mockBrokerService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ListingsController _controller;

        public ListingsControllerTests()
        {
            _mockListingService = new Mock<IListingService>();
            _mockBrokerService = new Mock<IBrokerService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // Always return a fake broker user
            _mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "broker1",
                    Email = "broker@test.com",
                    Role = "Broker"
                });

            _controller = new ListingsController(
                _mockListingService.Object,
                _mockBrokerService.Object,
                _mockUserManager.Object
            );
        }

        // INDEX
        [Fact]
        public async Task Index_ReturnsViewWithListings()
        {
            // Arrange
            var listings = new List<PropertyListingDto>
                        {
                            new() { Id = Guid.NewGuid(), PropertyType = PropertyType.House, Location = "KTM", Price = 5000000 },
                            new() { Id = Guid.NewGuid(), PropertyType = PropertyType.Apartment, Location = "Lalitpur", Price = 6000000 }
                        };

            _mockListingService.Setup(s => s.SearchAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<decimal?>(),
                    It.IsAny<PropertyType?>()))  
                .ReturnsAsync(listings);

            var searchModel = new ListingSearchModel
            {
                Location = "KTM",
                MinPrice = null,
                MaxPrice = null,
                PropertyType = null
            };

            // Act
            var result = await _controller.Index(searchModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ListingSearchModel>(viewResult.Model);

            Assert.Equal(2, model.Listings.Count());
            Assert.Contains(model.Listings, l => l.PropertyType == PropertyType.House);
            Assert.Contains(model.Listings, l => l.PropertyType == PropertyType.Apartment);
        }



        // DETAILS
        [Fact]
        public async Task Details_ReturnsView_WhenListingExists()
        {
            var id = Guid.NewGuid();
            var dto = new PropertyListingDto
            {
                Id = id,
                PropertyType = PropertyType.House // enum instead of string
            };

            _mockListingService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);

            var result = await _controller.Details(id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PropertyListingDto>(viewResult.Model);
            Assert.Equal(PropertyType.House, model.PropertyType); // enum check
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenListingDoesNotExist()
        {
            var id = Guid.NewGuid();
            _mockListingService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((PropertyListingDto?)null);

            var result = await _controller.Details(id);

            Assert.IsType<NotFoundResult>(result);
        }

        // CREATE
        [Fact]
        public async Task Create_Post_RedirectsOnSuccess()
        {
            var dto = new PropertyListingDto
            {
                Id = Guid.NewGuid(),
                PropertyType = PropertyType.House, // enum
                Location = "KTM",
                Price = 5000000
            };

            _mockListingService.Setup(s => s.AddAsync(It.IsAny<PropertyListingDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.Create(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _mockListingService.Verify(s => s.AddAsync(It.IsAny<PropertyListingDto>()), Times.Once);
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new PropertyListingDto
            {
                Id = Guid.NewGuid(),
                PropertyType = PropertyType.House, 
                Location = "KTM",
                Price = 5000000
            };

            // Force validation error (simulate missing/invalid property type in ModelState)
            _controller.ModelState.AddModelError("PropertyType", "Required");

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PropertyListingDto>(viewResult.Model);

            Assert.Equal(dto, model); // same dto returned to the view
            _mockListingService.Verify(s => s.AddAsync(It.IsAny<PropertyListingDto>()), Times.Never);
        }

        // EDIT
        [Fact]
        public async Task Edit_Post_RedirectsOnSuccess()
        {
            var dto = new PropertyListingDto
            {
                Id = Guid.NewGuid(),
                PropertyType = PropertyType.House,
                Location = "KTM",
                Price = 7000000
            };

            _mockListingService.Setup(s => s.UpdateAsync(dto)).Returns(Task.CompletedTask);

            var result = await _controller.Edit(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new PropertyListingDto
            {
                Id = Guid.NewGuid(),
                PropertyType = PropertyType.House,
                Location = "KTM",
                Price = 7000000
            };

            // Force a validation error as if PropertyType were missing
            _controller.ModelState.AddModelError("PropertyType", "Required");

            // Act
            var result = await _controller.Edit(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PropertyListingDto>(viewResult.Model);

            Assert.Equal(dto, model); // should return same dto back to view
            _mockListingService.Verify(s => s.UpdateAsync(It.IsAny<PropertyListingDto>()), Times.Never);
        }

        // DELETE
        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex_WhenSuccessful()
        {
            var id = Guid.NewGuid();

            _mockListingService.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(id);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _mockListingService.Verify(s => s.DeleteAsync(id), Times.Once);
        }
    }
}
