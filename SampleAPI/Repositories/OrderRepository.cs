using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _context;
        public OrderRepository(SampleApiDbContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetRecentOrdersAsync()
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);
            return await _context.Orders
                .Where(o => o.EntryDate >= oneDayAgo && !o.Deleted)
                .OrderByDescending(o => o.EntryDate)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByNameAsync(string name)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Name == name && !o.Deleted);
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

    }
}
