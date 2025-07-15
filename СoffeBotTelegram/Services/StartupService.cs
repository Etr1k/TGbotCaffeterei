using Microsoft.Extensions.Hosting;

namespace CoffeeBot.Telegram.Services
{
    public class StartupService : IHostedService
    {
        private readonly BotService _botService;

        public StartupService(BotService botService)
        {
            _botService = botService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _botService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}