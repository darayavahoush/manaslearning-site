using manaslearning.Admin.UI.Models;
using manaslearning.Admin.UI.ResponseModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace manaslearning.Admin.UI.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IAccessTokenProvider _tokenProvider;

        public UserService(
            ILogger<UserService> logger,
            IConfiguration configuration,
            HttpClient httpClient,
            NavigationManager navigationManager,
            IAccessTokenProvider tokenProvider) : base(configuration, logger)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._httpClient = httpClient;
            this._navigationManager = navigationManager;
            this._tokenProvider = tokenProvider;
        }

        public async Task<IEnumerable<UserInfo>> GetUsersAsync()
        {
            var tokenResult = await _tokenProvider.RequestAccessToken();
            if (tokenResult.TryGetToken(out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                return await _httpClient.GetFromJsonAsync<IEnumerable<UserInfo>>($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/user");
            }
            else
            {
                throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
                //throw new ArgumentNullException("token");
            }
        }

        //public async Task<ApiResponseModel> GetUsersAsync()
        //{
        //    var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/user"));
        //    var tokenResult = await _tokenProvider.RequestAccessToken();
        //    if (tokenResult.TryGetToken(out var token))
        //    {
        //        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        //    }
        //    var response = await _httpClient.SendAsync(message);
        //    return await GetApiResponseModelAsync<IEnumerable<UserInfo>>(response);
        //}

        public async Task<ApiResponseModel> CreateBulkUsersAsync(IEnumerable<SchoolUserAccount> schoolAccounts)
        {
            var requestJson = JsonSerializer.Serialize(schoolAccounts);

            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/schoolaccount"))
            {
                Content = new StringContent(requestJson, Encoding.UTF8, StringConstants.MEDIATYPE_APPLICATION_JSON)
            };
            var response = await _httpClient.SendAsync(message);

            return await GetApiResponseModelAsync<BulkUsersResponseModel>(response);
        }

        public async Task<ApiResponseModel> ResetPasswordAsync(string email)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/user/resetpassword?emailId={email}"));
            var response = await _httpClient.SendAsync(message);

            return await GetApiResponseModelAsync<string>(response, HttpContentType.String);
        }
    }
}
