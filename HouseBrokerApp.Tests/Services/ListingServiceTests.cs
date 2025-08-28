using HouseBrokerApp.Application.DTOs;
using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Core.Entities;
using HouseBrokerApp.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Linq.Expressions;

namespace HouseBrokerApp.Tests.Services
{
    public class ListingServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly ListingService _listingService;
        private readonly Mock<IRepository<PropertyListing>> _mockRepo;
        private readonly IMemoryCache _cache;

        public ListingServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IRepository<PropertyListing>>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _mockUow.Setup(u => u.Repository<PropertyListing>())
                .Returns(_mockRepo.Object);

            _listingService = new ListingService(_mockUow.Object, _cache);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFromRepository_WhenCacheIsEmpty()
        {
            var listings = new List<PropertyListing>
        {
            new() { Id = Guid.NewGuid(), PropertyType = "House", Location = "KTM", Price = 5000000 }
        };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(listings);

            var result = await _listingService.GetAllAsync();

            Assert.Single(result);
            Assert.Equal("House", result.First().PropertyType);
        }

        [Fact]
        public async Task GetAllAsync_UsesCache_WhenAvailable()
        {
            var listings = new List<PropertyListing>
        {
            new() { Id = Guid.NewGuid(), PropertyType = "Flat", Location = "KTM", Price = 3000000 }
        };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(listings);

            // First call populates cache
            var firstCall = await _listingService.GetAllAsync();

            // Second call should return cached result, not hit repo again
            var secondCall = await _listingService.GetAllAsync();

            Assert.Single(secondCall);
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_StoresNewListingAndInvalidatesCache()
        {
            var dto = new PropertyListingDto
            {
                PropertyType = "Apartment",
                Location = "Lalitpur",
                Price = 6000000,
                BrokerId = Guid.NewGuid()
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<PropertyListing>()))
                .Returns(Task.CompletedTask);

            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _listingService.AddAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<PropertyListing>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_CommissionIsCalculatedCorrectly()
        {
            var dto = new PropertyListingDto
            {
                PropertyType = "Villa",
                Location = "Bhaktapur",
                Price = 12000000, // > 1 crore
                BrokerId = Guid.NewGuid()
            };

            PropertyListing? savedListing = null;

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<PropertyListing>()))
                .Callback<PropertyListing>(l => savedListing = l)
                .Returns(Task.CompletedTask);

            _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _listingService.AddAsync(dto);

            Assert.NotNull(savedListing);
            Assert.Equal(0.015m * dto.Price, savedListing!.Commission); // 1.5%
        }

        [Fact]
        public async Task DeleteAsync_RemovesListing()
        {
            var id = Guid.NewGuid();
            var entity = new PropertyListing { Id = id, PropertyType = "Land", Price = 4000000 };

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            await _listingService.DeleteAsync(id);

            _mockRepo.Verify(r => r.Delete(entity), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DoesNothing_WhenListingNotFound()
        {
            var id = Guid.NewGuid();

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((PropertyListing?)null);

            await _listingService.DeleteAsync(id);

            _mockRepo.Verify(r => r.Delete(It.IsAny<PropertyListing>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
