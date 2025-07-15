using CoffeeBot.Telegram.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class BotService
{
    private readonly ITelegramBotClient _bot;
    private readonly IEnumerable<IMessageHandler> _handlers;

    public BotService(ITelegramBotClient bot, IEnumerable<IMessageHandler> handlers)
    {
        _bot = bot;
        _handlers = handlers;
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        var opts = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _bot.StartReceiving(OnUpdate, OnError, opts, token);
        var me = await _bot.GetMe(token);
        Console.WriteLine($"Bot started: @{me.Username}");
        await Task.Delay(-1, token);
    }

    private async Task OnUpdate(ITelegramBotClient bot, Update upd, CancellationToken ct)
    {
        if (upd.Type == UpdateType.CallbackQuery && upd.CallbackQuery is { } callback)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(callback));
            if (handler != null)
                await handler.HandleAsync(callback);
            return;
        }

        if (upd.Type == UpdateType.Message && upd.Message is { } msg)
        {
            foreach (var handler in _handlers)
            {
                if (handler.CanHandle(msg))
                {
                    await handler.HandleAsync(msg);
                    return; // не даём другим обработчикам забирать сообщение
                }
            }
        }
    }


    private Task OnError(ITelegramBotClient b, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}