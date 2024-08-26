using Telegram.Bot;
using TelegramBot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("tgwebhook")
    .RemoveAllLoggers()
    .AddTypedClient<ITelegramBotClient>(httpClient =>
    {
        var botToken = builder.Configuration["TelegramBotSettings:Token"];
        return new TelegramBotClient(botToken!, httpClient);
    });

builder.Services.AddScoped<IKernelApiService, KernelApiService>();
builder.Services.AddSingleton<TelegramUpdateHandler>();
builder.Services.ConfigureTelegramBotMvc();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
