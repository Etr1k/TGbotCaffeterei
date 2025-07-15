using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.EntityFrameworkCore;
using CoffeBotAPI.Data.APIdata;
using CoffeBotAPI.Model;
using User = CoffeBotAPI.Model.User;

namespace CoffeBotAPI.Handlers
{
    public class OrderHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly AppDbContext _context;

        public OrderHandler(ITelegramBotClient bot, AppDbContext context)
        {
            _bot = bot;
            _context = context;
        }

        public async Task HandleOrderMessage(Message msg)
        {
            if (msg.Text?.StartsWith("/order") == true)
            {
                var orderText = msg.Text.Replace("/order", "").Trim();
                if (string.IsNullOrEmpty(orderText))
                {
                    await _bot.SendMessage(msg.Chat.Id, "Укажите заказ, например: /order латте");
                    return;
                }

                await CreateOrder(msg.From.Id, msg.From.Username, orderText);
            }
            else if (msg.Text == "/orders")
            {
                await HandleOrdersCommand(msg);
            }
        }

        public async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            // Предполагаем, что callbackQuery.Data содержит текст заказа, например "latte"
            var orderText = callbackQuery.Data;
            if (string.IsNullOrEmpty(orderText))
            {
                await _bot.AnswerCallbackQuery(callbackQuery.Id, "Ошибка: пустой заказ.");
                return;
            }

            await CreateOrder(callbackQuery.From.Id, callbackQuery.From.Username, orderText);
            await _bot.AnswerCallbackQuery(callbackQuery.Id, "Заказ принят!");
            await _bot.SendMessage(callbackQuery.Message.Chat.Id, "Заказ принят!");
        }

        private async Task CreateOrder(long telegramId, string username, string orderText)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
            if (user == null)
            {
                user = new User
                {
                    Username = username ?? $"User_{telegramId}",
                    TelegramId = telegramId,
                    IsBarista = false
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var order = new Orders
            {
                UserId = user.Id,
                TelegramId = telegramId,
                Username = username ?? $"User_{telegramId}",
                Text = orderText,
                Status = StatusOrder.New, // Должно быть StatusOrder.New, а не строка
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        private async Task HandleOrdersCommand(Message msg)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == msg.From.Id);
            if (user == null || !user.IsBarista)
            {
                await _bot.SendMessage(msg.Chat.Id, "Только бариста может просматривать заказы.");
                return;
            }

            var orders = await _context.Orders
                .Where(o => o.Status == StatusOrder.New)
                .ToListAsync();

            if (!orders.Any())
            {
                await _bot.SendMessage(msg.Chat.Id, "Нет новых заказов.");
                return;
            }

            var message = "Новые заказы:\n";
            foreach (var order in orders)
            {
                string statusText = order.Status switch
                {
                    StatusOrder.New => "Новый",
                    StatusOrder.Inprogress => "Готовится",
                    StatusOrder.Done => "Готов",
                    StatusOrder.Cancelled => "Отменён",
                    _ => "Неизвестно"
                };

                message += $"Заказ #{order.Id}: {order.Text} от {order.Username} ({statusText})\n";
            }

            await _bot.SendMessage(msg.Chat.Id, message);
        }
    }
}