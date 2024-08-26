using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    [Route("bot")]
    [ApiController]
    public class BotController(IConfiguration configuration, ILogger<BotController> logger) : ControllerBase
    {
        [HttpGet("setWebhook")]
        public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
        {
            var webhookUrl = configuration["TelegramBotSettings:WebhookUrl"]!;
            var secretToken = configuration["TelegramBotSettings:SecretToken"];
            await bot.SetWebhookAsync(webhookUrl, allowedUpdates: [], secretToken: secretToken, cancellationToken: ct);
            return $"Webhook set to {webhookUrl}";
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] TelegramUpdateHandler handleUpdateService, CancellationToken ct)
        {
            var secretToken = configuration["TelegramBotSettings:SecretToken"];
            if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != secretToken)
            {
                return Forbid();
            }

            try
            {
                logger.LogInformation("Receive update type: {UpdateType}", update.Type);
                await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            }
            catch (Exception exception)
            {
                await handleUpdateService.HandleErrorAsync(bot, exception, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
                logger.LogError(exception, "Error handling update");
            }
            return Ok();
        }
    }
}
