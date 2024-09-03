using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class TelegramUpdateHandler(IConfiguration configuration, ITelegramBotClient bot, ILogger<TelegramUpdateHandler> logger, IKernelApiService apiService) : IUpdateHandler
    {
        private readonly Dictionary<long, string> _userCommands = [];

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await (update switch
            {
                { Message: { } message } => OnMessage(message),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            logger.LogInformation("HandleError: {Exception}", exception);
            // Cooldown in case of network connection error
            if (exception is RequestException)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        private async Task OnMessage(Message msg)
        {
            logger.LogInformation("Receive message type: {MessageType}", msg.Type);
            if (msg.Text is not { } messageText)
            {
                return;
            }

            var command = messageText.Split(' ')[0];
            var keyword = string.Empty;
            if (messageText.Split(' ').Length > 1)
            {
                keyword = messageText.Split(' ')[1];
            }
            await (command switch
            {
                "/start" => SendGreetingMessage(msg),
                "/help" => SendHelpMessage(msg),
                "/whois" => HandleWhoisRequest(msg, keyword),
                "/apow" => SendBotInfoMessage(msg),
                _ => HandleOtherReplyAsync(msg)
            });
        }

        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            logger.LogInformation("Receive callback data: {CallbackData}", callbackQuery.Data);

            if (callbackQuery.Data is null || callbackQuery.Message is null)
            {
                return;
            }

            if (callbackQuery.Data.StartsWith("whois"))
            {
                var targetId = callbackQuery.Data.Replace("whois", string.Empty);
                await GetProfileAndContactAndReplyAsync(callbackQuery.Message, targetId);
                return;
            }
        }

        private async Task<Message> SendGreetingMessage(Message msg)
        {
            var reply = string.Concat("對我許願是沒用的，你只能利用以下的指令：\n", GetBotMenu());
            return await SendTextMessageAsync(msg, reply);
        }

        private async Task<Message> SendHelpMessage(Message msg)
        {
            return await SendTextMessageAsync(msg, GetBotMenu());
        }

        private async Task<Message> HandleWhoisRequest(Message msg, string keyword)
        {
            // 如果有關鍵字，直接查詢
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // 從關鍵字取得候選列表
                return await GetCanditateAndReplyAsync(msg, keyword);
            }

            if (!_userCommands.TryAdd(msg.Chat.Id, "/whois"))
            {
                // 紀錄使用者指令到字典中
                _userCommands[msg.Chat.Id] = "/whois";
            }

            // 與使用者互動，請使用者提供關鍵字
            return await SendTextMessageAsync(msg, "你要找誰啊？");
        }

        private async Task<Message> HandleOtherReplyAsync(Message msg)
        {
            if (string.IsNullOrWhiteSpace(msg.Text))
            {
                return await SendDefaultMessage(msg);
            }

            if (_userCommands.TryGetValue(msg.Chat.Id, out var command))
            {
                // 從字典中移除使用者指令
                _userCommands.Remove(msg.Chat.Id);

                return command switch
                {
                    "/whois" => await GetCanditateAndReplyAsync(msg, msg.Text),
                    _ => await SendDefaultMessage(msg),
                };
            }

            return await SendDefaultMessage(msg);
        }

        private async Task<Message> SendBotInfoMessage(Message msg)
        {
            var info = configuration["TelegramBotSettings:BotInfo"] ?? "我壞掉了，抓不到資料...";
            info = info.Split('|').Aggregate(string.Empty, (current, s) => current + s + Environment.NewLine);

            return await SendTextMessageAsync(msg, info);
        }

        private async Task<Message> SendDefaultMessage(Message msg)
        {
            var reply = string.Concat("洗勒工殺小？講人話好嗎！不知道怎麼講就用下面的指令啦：\n", GetBotMenu());
            return await SendTextMessageAsync(msg, reply);
        }

        private async Task<Message> GetCanditateAndReplyAsync(Message msg, string keyword)
        {
            var response = await apiService.GetCandidateListByKeywordAsync(keyword);

            if (response.Success)
            {
                if (response.Data is null || !response.Data.Any())
                {
                    return await SendTextMessageAsync(msg, "沒這個人啦，你確定字有打對嗎？");
                }

                // 如果只有一筆資料，直接回傳聯絡資訊
                if (response.Data.Count() == 1)
                {
                    var userProfile = response.Data.First();
                    return await GetContactAndReplyAsync(msg, userProfile);
                }

                // 有多筆資料，回傳候選列表
                var condidates = response.Data;
                var inlineMarkup = new InlineKeyboardMarkup();

                foreach (var candidate in condidates)
                {
                    inlineMarkup.AddNewRow().AddButton(
                        InlineKeyboardButton.WithCallbackData(
                            $"{candidate.Name} {candidate.DepartmentName} {candidate.TitleName}",
                            $"whois{candidate.Id}"
                        )
                    );
                }

                return await bot.SendTextMessageAsync(msg.Chat, $"條件有點粗欸，挑一個尬意的吧：", replyMarkup: inlineMarkup);
            }

            return await SendApiErrorMessage(msg, response.StatusCode, response.ErrorResponse);
        }

        private async Task<Message> GetContactAndReplyAsync(Message msg, UserProfile userProfile)
        {
            logger.LogInformation("人員 ID: {Id}", userProfile.Id);
            var response = await apiService.GetContactByIdAsync(userProfile.Id);

            if (response.Success)
            {
                if (response.Data is null)
                {
                    return await SendTextMessageAsync(msg, "這傢伙很懶，什麼聯絡資料都沒有留下喔！");

                }

                var contact = response.Data;
                var reply =
                    $"{userProfile.Name}\n" +
                    $"{userProfile.OrganizationId} {userProfile.DepartmentId}\n" +
                    $"{userProfile.DepartmentName} {userProfile.TitleName}\n" +
                    $"年資：{userProfile.YearsOfExperience}\n" +
                    $"Email：{contact.Email}\n" +
                    $"分機：{contact.PhoneNumber}\n" +
                    $"公務機：{contact.MobileNumber}";

                return await SendTextMessageAsync(msg, reply);
            }

            if (response.StatusCode == StatusCodes.Status404NotFound)
            {
                var reply =
                    $"{userProfile.Name}\n" +
                    $"{userProfile.OrganizationId} {userProfile.DepartmentId}\n" +
                    $"{userProfile.DepartmentName} {userProfile.TitleName}\n" +
                    $"年資：{userProfile.YearsOfExperience}\n" +
                    $"Email：\n" +
                    $"分機：\n" +
                    $"公務機：";

                return await SendTextMessageAsync(msg, reply);
            }

            return await SendApiErrorMessage(msg, response.StatusCode, response.ErrorResponse);
        }

        private async Task<Message> GetProfileAndContactAndReplyAsync(Message msg, string id)
        {
            var response = await apiService.GetProfileByIdAsync(id);

            if (response.Success)
            {
                if (response.Data is null)
                {
                    return await SendTextMessageAsync(msg, "這傢伙很懶，什麼聯絡資料都沒有留下喔！");
                }

                var userProfile = response.Data;
                return await GetContactAndReplyAsync(msg, userProfile);
            }

            return await SendApiErrorMessage(msg, response.StatusCode, response.ErrorResponse);
        }

        private async Task<Message> SendApiErrorMessage(Message msg, int StatusCode, ApiErrorResponse? errorResponse)
        {
            if (StatusCode == StatusCodes.Status500InternalServerError || errorResponse is null)
            {
                return await SendTextMessageAsync(msg, "我壞掉了，找我的老闆處理吧...");
            }

            switch (errorResponse.ErrorCode)
            {
                case 1:
                    return await SendTextMessageAsync(msg, "沒有給我關鍵字是要我通靈喔！");
                case 2:
                    return await SendTextMessageAsync(msg, "條件太粗了啦！講清楚一點好嗎？");
                default:
                    return await SendTextMessageAsync(msg, "我壞掉了，找我的老闆處理吧...");
            }
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
            return Task.CompletedTask;
        }

        private async Task<Message> SendTextMessageAsync(Message msg, string reply)
        {
            return await bot.SendTextMessageAsync(msg.Chat, reply, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
        }

        private static string GetBotMenu()
        {
            const string menu =
                "/start     - 召喚阿炮\n" +
                "/help      - 顯示阿炮指令\n" +
                "/whois     - 進入員工資料查詢互動模式\n" +
                "/apow      - 顯示機器人資訊\n\n" +
                "目前提供姓名、OPID、分機、公務機、Email 作為關鍵字查詢\n" +
                "使用 /whois 空一格加關鍵字可以跳過查詢互動模式直接查詢";

            return menu;
        }
    }
}
