using manaslearning.Admin.UI.Models;
using System.Net.Http.Json;

namespace manaslearning.Admin.UI.Services
{
    public class BaseService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        protected string BaseUrl { get; init; }

        public BaseService(IConfiguration configuration, ILogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;

            this.BaseUrl = $"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}";
        }

        public async Task<ApiResponseModel> GetApiResponseModelAsync<T>(HttpResponseMessage response, HttpContentType httpContentType = HttpContentType.Json) // where T: class  // TODO: will be implemented later
        {
            var apiResponse = new ApiResponseModel();
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    apiResponse.Succeeded = true;
                    apiResponse.StatusCode = response.StatusCode;
                    apiResponse.Data = await ReadAsync<T>(response.Content, httpContentType);

                    return apiResponse;
                }
                apiResponse.Succeeded = false;
                apiResponse.StatusCode = response.StatusCode;

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    apiResponse.Data = await ReadAsync<T>(response.Content, HttpContentType.String);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"ErrorMessage={ex.Message}");

                apiResponse.Succeeded = false;
                apiResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                apiResponse.Data = "JSON parse error";
            }

            return apiResponse;
        }

        public async Task<object> ReadAsync<T>(HttpContent httpContent, HttpContentType httpContentType)
        {
            if (httpContent == null)
                return null;

            return httpContentType switch
            {
                HttpContentType.Empty => string.Empty,
                HttpContentType.Json => await httpContent.ReadFromJsonAsync<T>(),
                HttpContentType.String => await httpContent.ReadAsStringAsync(),
                _ => null,
            };
        }
    }
}
