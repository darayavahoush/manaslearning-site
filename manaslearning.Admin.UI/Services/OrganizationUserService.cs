using manaslearning.Admin.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace manaslearning.Admin.UI.Services;

public class OrganizationUserService : BaseService, IOrganizationUserService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly IAccessTokenProvider _tokenProvider;

    public OrganizationUserService(
            ILogger<OrganizationUserService> logger,
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

    public async Task<IEnumerable<OrganizationUserInfo>> GetAllUsersByOrganizationOidAsync(Guid Oid)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            try
            {
                string url = $"{BaseUrl}/api/OrganizationUser/GetAllByOrganizationOid?organizationOid={Oid}";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                return await _httpClient.GetFromJsonAsync<IEnumerable<OrganizationUserInfo>>(url);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<OrganizationUserInfo> GetUserAsync(Guid OrganizationOid, long userId)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/OrganizationUser/{OrganizationOid}/{userId}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<OrganizationUserInfo>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }

    public async Task<IEnumerable<OrganizationUserInfo>> GetUsersByOrganizationOidAsync(Guid Oid)
    {
        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            string url = $"{BaseUrl}/api/OrganizationUser/GetByOrganizationOid?organizationOid={Oid}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            return await _httpClient.GetFromJsonAsync<IEnumerable<OrganizationUserInfo>>(url);
        }
        else
        {
            throw new AccessTokenNotAvailableException(_navigationManager, tokenResult, null);
        }
    }
}
