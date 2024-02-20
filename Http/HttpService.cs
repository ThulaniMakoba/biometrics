using biometricService.Interfaces;
using biometricService.Models;
using System.Net;
using System.Net.Http.Formatting;

namespace biometricService.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;

        public HttpService(HttpClient client, ITokenService tokenService, AppSettings appSettings)
        {
            _tokenService = tokenService;
            _appSettings = appSettings;
            _httpClient = client;
            _httpClient.BaseAddress = new Uri(_appSettings.InnovatricsUrl);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new ObjectContent<TRequest>(data, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return await HandleResponse<TResult>(response);

        }

        public async Task<T> PostAsync<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new ObjectContent<object>(null, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return await HandleResponse<T>(response);
        }
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint)
            {
                Content = new ObjectContent<object>(null, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return await HandleResponse<T>(response);
        }

        public async Task<TResult> PutAsync<TResult>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = new ObjectContent<object>(null, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return await HandleResponse<TResult>(response);
        }

        public async Task<TResult> PutAsync<TRequest, TResult>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = new ObjectContent<TRequest>(data, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return await HandleResponse<TResult>(response);
        }

        public static async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - Details {response.ReasonPhrase}");
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(_appSettings.InnovatricsApiKey))
            {
                _tokenService.AddBearerToken(request, _appSettings.InnovatricsApiKey);
            }
        }
    }
}
