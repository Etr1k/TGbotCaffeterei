using CoffeeBot.Telegram;
using CoffeeBot.Telegram.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public class BaristaHandler : IMessageHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly OrderService _orderService;
    private readonly UserSessionService _sessions;

    public BaristaHandler(ITelegramBotClient bot, OrderService orderService, UserSessionService sessions)
    {
        _bot = bot;
        _orderService = orderService;
        _sessions = sessions;
    }

    public bool CanHandle(Message msg)
    {
        if (msg.Text is null)
            return false;

        if (msg.Text == "/barista" || msg.Text == "/orders")
            return true;

        // Теперь — быстрая локальная проверка, без async
        if (_sessions.IsAwaitingPasswordLocal(msg.From.Id))
            return true;

        return false;
    }

    public async Task HandleAsync(Message msg)
    {
        var userId = msg.From.Id;
        var username = msg.From.Username;

        if (msg.Text == "/barista")
        {
            if (await _sessions.IsBaristaAsync(userId))
            {
                await _bot.SendMessage(msg.Chat.Id, "Добро Пожаловать ,Бариста!");
            }
            else
            {
                _sessions.MarkAsAwaitingPassword(userId); // Включили режим ожидания
                await _bot.SendMessage(msg.Chat.Id, "Введите пароль:");
            }
            return;
        }

        // Обработка пароля
        if (_sessions.IsAwaitingPasswordLocal(userId))
        {
            if (msg.Text == BotConfig.BaristaPassword)
            {
                _sessions.ClearAwaitingPassword(userId); // Выключили режим ожидания
                await _sessions.GrantAccessAsync(userId, username);
                await _bot.SendMessage(msg.Chat.Id, "Доступ предоставлен, бариста!");
            }
            else
            {
                await _bot.SendMessage(msg.Chat.Id, "Неверный пароль.");
            }
            return;
        }

        // Просмотр заказов
        if (msg.Text == "/orders")
        {
            var orders = await _orderService.GetNewOrdersAsync();
            foreach (var order in orders)
            {
                var buttons = new InlineKeyboardMarkup(new[]
                {
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Подтвердить", $"accept:{order.Id}"),
                        InlineKeyboardButton.WithCallbackData("Отклонить", $"reject:{order.Id}")
                    }
                });

                await _bot.SendMessage(msg.Chat.Id, $"Заказ от @{order.Username}: {order.Text}", replyMarkup: buttons);
            }
        }
    }

    public bool CanHandle(CallbackQuery query)
    {
        return query.Data?.StartsWith("accept:") == true || query.Data?.StartsWith("reject:") == true;
    }

    public async Task HandleAsync(CallbackQuery query)
    {
        var parts = query.Data.Split(':');
        var orderId = int.Parse(parts[1]);
        await _orderService.UpdateOrderStatusAsync(orderId, parts[0] == "accept" ? "accepted" : "rejected");
        await _bot.AnswerCallbackQuery(query.Id, "Обработано");
    }
}
