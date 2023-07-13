namespace KWFFunctionalTests.Abstractions
{
    using KWFCommon.Abstractions.Constants;

    using KWFJson.Configuration;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Net.Http.Headers;

    using System.Net;
    using System.Text;
    using System.Text.Json;

    public abstract class KwfBaseFunctionalTests<TProgram> : IDisposable
        where TProgram : class
    {
        private readonly WebApplicationFactory<TProgram> _webApplicationFactory;
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonSerializerOptions = new KWFJsonConfiguration().GetJsonSerializerOptions();

        protected abstract string BearerToken { get; }
        protected abstract string BearerTokenHeader { get; }
        protected abstract string UrlFormat { get; }

        public KwfBaseFunctionalTests()
        {
            var (webApplicationFactory, httpClient) = KwfTestApplicationBuilderExtensions.CreateKwfTestApplicationClient<TProgram>(AddMockServices);
            _webApplicationFactory = webApplicationFactory;
            _httpClient = httpClient;
        }

        protected async Task<(HttpStatusCode, string)> GetAsync(params string[] args)
        {
            AddToken();

            var serviceResponse = args.Length == 0
                ? await _httpClient.GetAsync(UrlFormat)
                : await _httpClient.GetAsync(string.Format(UrlFormat, args));

            return (serviceResponse.StatusCode, await serviceResponse.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpStatusCode, string)> PostAsync<TReq>(TReq request, params string[] args)
            where TReq : class
        {
            AddToken();
            var serviceRequest = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, RestConstants.JsonContentType);
            var serviceResponse = args.Length == 0
                ? await _httpClient.PostAsync(UrlFormat, serviceRequest)
                : await _httpClient.PostAsync(string.Format(UrlFormat, args), serviceRequest);
            return (serviceResponse.StatusCode, await serviceResponse.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpStatusCode, string)> PutAsync<TReq>(TReq request, params string[] args)
            where TReq : class
        {
            AddToken();
            var serviceRequest = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, RestConstants.JsonContentType);
            var serviceResponse = args.Length == 0
                ? await _httpClient.PutAsync(UrlFormat, serviceRequest)
                : await _httpClient.PutAsync(string.Format(UrlFormat, args), serviceRequest);
            return (serviceResponse.StatusCode, await serviceResponse.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpStatusCode, string)> DeleteAsync(params string[] args)
        {
            AddToken();

            var serviceResponse = args.Length == 0
                ? await _httpClient.DeleteAsync(UrlFormat)
                : await _httpClient.DeleteAsync(string.Format(UrlFormat, args));
            return (serviceResponse.StatusCode, await serviceResponse.Content.ReadAsStringAsync());
        }

        protected async Task<(HttpStatusCode, T?)> GetAsync<T>(params string[] args)
            where T : class
        {
            AddToken();

            var serviceResponse = args.Length == 0 
                ? await _httpClient.GetAsync(UrlFormat) 
                : await _httpClient.GetAsync(string.Format(UrlFormat, args));
            try
            {
                var parsedResponse = JsonSerializer.Deserialize<T>(await serviceResponse.Content.ReadAsStringAsync(), _jsonSerializerOptions);
                return (serviceResponse.StatusCode, parsedResponse);
            }
            catch
            {
                return (serviceResponse.StatusCode, default(T));
            }
        }

        protected async Task<(HttpStatusCode, TResp?)> PostAsync<TReq, TResp>(TReq request, params string[] args)
            where TReq : class
            where TResp : class
        {
            AddToken();
            var serviceRequest = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, RestConstants.JsonContentType);
            var serviceResponse = args.Length == 0
                ? await _httpClient.PostAsync(UrlFormat, serviceRequest)
                : await _httpClient.PostAsync(string.Format(UrlFormat, args), serviceRequest);
            try
            {
                var parsedResponse = JsonSerializer.Deserialize<TResp>(await serviceResponse.Content.ReadAsStringAsync(), _jsonSerializerOptions);
                return (serviceResponse.StatusCode, parsedResponse);
            }
            catch
            {
                return (serviceResponse.StatusCode, default(TResp));
            }
        }

        protected async Task<(HttpStatusCode, TResp?)> PutAsync<TReq, TResp>(TReq request, params string[] args)
            where TReq : class
            where TResp : class
        {
            AddToken();
            var serviceRequest = new StringContent(JsonSerializer.Serialize(request, _jsonSerializerOptions), Encoding.UTF8, RestConstants.JsonContentType);
            var serviceResponse = args.Length == 0
                ? await _httpClient.PutAsync(UrlFormat, serviceRequest)
                : await _httpClient.PutAsync(string.Format(UrlFormat, args), serviceRequest);
            try
            {
                var parsedResponse = JsonSerializer.Deserialize<TResp>(await serviceResponse.Content.ReadAsStringAsync(), _jsonSerializerOptions);
                return (serviceResponse.StatusCode, parsedResponse);
            }
            catch
            {
                return (serviceResponse.StatusCode, default(TResp));
            }
        }

        protected async Task<(HttpStatusCode, T?)> DeleteAsync<T>(params string[] args)
            where T : class
        {
            AddToken();

            var serviceResponse = args.Length == 0
                ? await _httpClient.DeleteAsync(UrlFormat)
                : await _httpClient.DeleteAsync(string.Format(UrlFormat, args));
            try
            {
                var parsedResponse = JsonSerializer.Deserialize<T>(await serviceResponse.Content.ReadAsStringAsync(), _jsonSerializerOptions);
                return (serviceResponse.StatusCode, parsedResponse);
            }
            catch
            {
                return (serviceResponse.StatusCode, default(T));
            }
        }

        protected abstract void AddMockServices(IServiceCollection services);

        private void AddToken()
        {
            if (!string.IsNullOrEmpty(BearerToken))
            {
                var tokenHeader = string.IsNullOrEmpty(BearerTokenHeader) ? HeaderNames.Authorization : BearerTokenHeader;
                if (!_httpClient.DefaultRequestHeaders.Contains(tokenHeader))
                {
                    _httpClient.DefaultRequestHeaders.Add(tokenHeader, BearerToken);
                }
            }
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }

            if (_webApplicationFactory != null)
            {
                _webApplicationFactory.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
