using CoffeBotAPI.Data.APIdata;
using CoffeBotAPI.Model;
using CoffeeBot.Telegram.Handlers;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = CoffeBotAPI.Model.User;

public class ClientHandler : IMessageHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly OrderService _orders;
    private readonly AppDbContext _db;


    public ClientHandler(ITelegramBotClient bot, OrderService orders, AppDbContext db)
    {
        _bot = bot;
        _orders = orders;
        _db = db;
    }

    public bool CanHandle(Message msg)
        => msg.Text is not null &&          // всегда текст
           !msg.Text.StartsWith("/barista") // не бариста‑команды
           && !msg.Text.StartsWith("/orders");

    public async Task HandleAsync(Message msg)
    {
        if (msg.Text == "/start")
        {
            await _bot.SendMessage(msg.Chat.Id, "Привет! Меню появится позже.");
            return;
        }

        var tgId = msg.From.Id;

        // 🔍 Находим пользователя
        var user = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == tgId);

        if (user == null)
        {
            user = new User
            {
                TelegramId = tgId,
                Username = msg.From.Username ?? $"user_{tgId}",
                IsBarista = false
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(); // 🔄 сохраняем user.Id
        }


        var order = new Orders
        {
            TelegramId = tgId,
            Username   = msg.From.Username ?? $"user_{tgId}",
            Text       = msg.Text!,
            Status     = StatusOrder.New,
            CreatedAt  = DateTime.UtcNow,
            UserId     = user.Id // ✅ Устанавливаем UserId
        };

        await _orders.CreateOrderAsync(order);
        await _bot.SendMessage(msg.Chat.Id, "Ваш заказ принят.");
    }

}