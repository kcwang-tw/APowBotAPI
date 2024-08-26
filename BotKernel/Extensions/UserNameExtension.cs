using CgmhNameStringConverter;

namespace BotKernel.Extensions
{
    public static class UserNameExtension
    {
        public static string ToUtf8(this string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                return hexString;
            }

            try
            {
                // 呼叫難字轉換 WebService
                var ws = new WebService1SoapClient(WebService1SoapClient.EndpointConfiguration.WebService1Soap);
                ws.Endpoint.Binding.SendTimeout = TimeSpan.FromSeconds(5); // 設置 5 秒超時

                var task = ws.Big5Hex2UTF8Async(hexString);
                if (Task.WhenAny(task, Task.Delay(5000)).Result == task)
                {
                    return task.Result;
                }
                else
                {
                    return hexString; // 超時時返回原始字符串
                }
            }
            catch (Exception)
            {
                return hexString; // 發生異常時返回原始字符串
            }
        }
    }
}
