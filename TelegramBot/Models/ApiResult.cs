namespace TelegramBot.Models
{
    public class ApiResult<T> where T : class
    {
        public bool Success { get; set; }

        public T? Data { get; set; }

        public ApiErrorResponse? ErrorResponse { get; set; }

        public int StatusCode { get; set; }
    }
}
