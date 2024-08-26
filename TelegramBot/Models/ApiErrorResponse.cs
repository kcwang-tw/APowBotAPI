namespace TelegramBot.Models
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;

        public int ErrorCode { get; set; }
    }
}
