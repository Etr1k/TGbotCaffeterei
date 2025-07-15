using Telegram.Bot.Types;

namespace CoffeeBot.Telegram.Handlers;

public interface IMessageHandler
{
            bool CanHandle(Message message);
            Task HandleAsync(Message message);
            bool CanHandle(CallbackQuery query) => false;
            Task HandleAsync(CallbackQuery query) => Task.CompletedTask;
        }
