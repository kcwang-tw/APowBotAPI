using System.Text.Json;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public class KernelApiService : IKernelApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        private static readonly JsonSerializerOptions _propertyNameCaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public KernelApiService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["KernelApiSettings:ApiUrl"]!;

            // 增加 header
            var tokenHeader = configuration["KernelApiSettings:TokenHeader"]!;
            var secretToken = configuration["KernelApiSettings:SecretToken"]!;
            httpClient.DefaultRequestHeaders.Add(tokenHeader, secretToken);
        }

        public async Task<ApiResult<IEnumerable<UserProfile>>> GetCandidateListByKeywordAsync(string keyword)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/employee?keyword={keyword}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profiles = JsonSerializer.Deserialize<IEnumerable<UserProfile>>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<IEnumerable<UserProfile>>
                    {
                        Success = true,
                        Data = profiles,
                        StatusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<IEnumerable<UserProfile>>
                    {
                        Success = false,
                        ErrorResponse = errorResponse,
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (Exception)
            {
                return new ApiResult<IEnumerable<UserProfile>>
                {
                    Success = false,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ApiResult<UserProfile>> GetProfileByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/employee/{id}/profile");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<UserProfile>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<UserProfile>
                    {
                        Success = true,
                        Data = profile,
                        StatusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<UserProfile>
                    {
                        Success = false,
                        ErrorResponse = errorResponse,
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (Exception)
            {
                return new ApiResult<UserProfile>
                {
                    Success = false,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ApiResult<UserContact>> GetContactByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/employee/{id}/contact");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var contact = JsonSerializer.Deserialize<UserContact>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<UserContact>
                    {
                        Success = true,
                        Data = contact,
                        StatusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content, _propertyNameCaseInsensitive)!;

                    return new ApiResult<UserContact>
                    {
                        Success = false,
                        ErrorResponse = errorResponse,
                        StatusCode = (int)response.StatusCode
                    };
                }

            }
            catch (Exception)
            {
                return new ApiResult<UserContact>
                {
                    Success = false,
                    Data = null,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }

    public interface IKernelApiService
    {
        Task<ApiResult<IEnumerable<UserProfile>>> GetCandidateListByKeywordAsync(string keyword);

        Task<ApiResult<UserProfile>> GetProfileByIdAsync(string id);

        Task<ApiResult<UserContact>> GetContactByIdAsync(string id);
    }
}
