
using CoffeBotAPI.Data.APIdata;
using CoffeBotAPI.Model;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task CreateOrderAsync(Orders order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Orders>> GetNewOrdersAsync()
    {
        return await _db.Orders.Where(o => o.Status == StatusOrder.New).ToListAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = StatusOrder.New;
            await _db.SaveChangesAsync();
        }
    }
}
