using CoffeBotAPI.Data.APIdata;
using CoffeBotAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

public class UserSessionService
{
    private readonly AppDbContext _db;

    // Локальное отслеживание состояния ожидания пароля
    private readonly ConcurrentDictionary<long, bool> _awaitingPassword = new();

    public UserSessionService(AppDbContext db)
    {
        _db = db;
    }

    public void MarkAsAwaitingPassword(long userId)
    {
        _awaitingPassword[userId] = true;
    }

    public void ClearAwaitingPassword(long userId)
    {
        _awaitingPassword.TryRemove(userId, out _);
    }

    public bool IsAwaitingPasswordLocal(long userId)
    {
        return _awaitingPassword.ContainsKey(userId);
    }

    public async Task RequirePasswordAsync(long userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
        if (user != null)
        {
            user.IsBarista = false;
            await _db.SaveChangesAsync();
        }
    }

    public async Task GrantAccessAsync(long userId, string username)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
        if (user == null)
        {
            user = new User()
            {
                TelegramId = userId,
                Username = username,
                IsBarista = true
            };
            _db.Users.Add(user);
        }
        else
        {
            user.IsBarista = true;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsBaristaAsync(long userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
        return user != null && user.IsBarista;
    }
}