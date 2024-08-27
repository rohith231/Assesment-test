using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Tests.Repositories
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly SampleApiDbContext _context;
        private readonly OrderRepository _repository;

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<SampleApiDbContext>()
                .UseInMemoryDatabase("mock_SampleApiDbContext")
                .Options;

            _context = new SampleApiDbContext(options);
            _repository = new OrderRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetRecentOrders_ReturnsOrdersWithinLastDay()
        {
            
            _context.Orders.Add(new Order { EntryDate = DateTime.UtcNow.AddHours(-1), Name = "Recent Order", Description = "Recent Order Description" });
            _context.Orders.Add(new Order { EntryDate = DateTime.UtcNow.AddDays(-2), Name = "Old Order", Description = "Old Order Description", Deleted = true });
            await _context.SaveChangesAsync();

            
            var result = await _repository.GetRecentOrdersAsync();

            
            Assert.Single(result);
            Assert.Equal("Recent Order", result.First().Name);
        }

        [Fact]
        public async Task AddOrderAsync_AddsOrderToDatabase()
        {
            
            var order = new Order { Name = "New Order", Description = "New Order Description" };

            
            await _repository.AddOrderAsync(order);
            var result = await _context.Orders.FindAsync(order.Id);

            
            Assert.NotNull(result);
            Assert.Equal("New Order", result.Name);
        }

    }
}