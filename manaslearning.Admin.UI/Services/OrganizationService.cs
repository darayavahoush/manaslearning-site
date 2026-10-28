using manaslearning.Admin.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace manaslearning.Admin.UI.Services;

public class OrganizationService : BaseService, IOrganizationService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly IAccessTokenProvider _tokenProvider;

    public OrganizationService(
            ILogger<OrganizationService> logger,
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

    public async Task<IEnumerable<OrganizationInfo>> GetOrganizationsAsync()
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/organization";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<IEnumerable<OrganizationInfo>>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<IEnumerable<OrganizationInfo>> GetAllOrganizationsAsync()
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/organization/all";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<IEnumerable<OrganizationInfo>>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<OrganizationInfo> GetOrganizationByOidAsync(Guid oid)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/organization/getbyoid/{oid}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<OrganizationInfo>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<OrganizationInfo> GetOrganizationAsync(string organizationId)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/organization/{organizationId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<OrganizationInfo>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<ApiResponseModel> CreateOrganizationAsync(string name, string displayName, int type)
    {
        var requestJson = JsonSerializer.Serialize(new { Name = name, DisplayName = displayName, OrganizationTypeId = type });

        var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/organization"))
        {
            Content = new StringContent(requestJson, Encoding.UTF8, StringConstants.MEDIATYPE_APPLICATION_JSON)
        };
        var response = await _httpClient.SendAsync(message);

        return await GetApiResponseModelAsync<ApiResponseModel>(response, HttpContentType.String);
    }

    public async Task<ApiResponseModel> UpdateOrganizationAsync(Guid oid, string name, string displayName, int type)
    {
        var requestJson = JsonSerializer.Serialize(new { Oid = oid, Name = name, DisplayName = displayName, OrganizationTypeId = type });

        var message = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_configuration.GetValue<string>(AppConstants.AdminApiBaseUrl)}/api/organization"))
        {
            Content = new StringContent(requestJson, Encoding.UTF8, StringConstants.MEDIATYPE_APPLICATION_JSON)
        };
        var response = await _httpClient.SendAsync(message);

        return await GetApiResponseModelAsync<ApiResponseModel>(response, HttpContentType.String);
    }
}
