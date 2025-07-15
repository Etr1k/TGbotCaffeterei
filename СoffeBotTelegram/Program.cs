    using CoffeBotAPI.Data;
    using CoffeBotAPI.Data.APIdata;
    using CoffeeBot.Telegram.Handlers;
    using CoffeeBot.Telegram.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Telegram.Bot;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql("Host=localhost;Database=coffeebot;Username=postgres;Password=1234"));

    builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("8085652127:AAHUR1d5yoIEo3i9rwKcAHPmDWvkL2V-1bw"));

    builder.Services.AddScoped<OrderService>();
    builder.Services.AddSingleton<UserSessionService>();
    builder.Services.AddScoped<ClientHandler>();
    builder.Services.AddScoped<BaristaHandler>();
    builder.Services.AddSingleton<BotService>();
    builder.Services.AddHostedService<StartupService>();
    builder.Services.AddScoped<IMessageHandler, BaristaHandler>();
    builder.Services.AddScoped<IMessageHandler, ClientHandler>();
    

    var app = builder.Build();

    app.Run();