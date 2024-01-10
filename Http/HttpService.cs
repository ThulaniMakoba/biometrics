using biometricService.Interfaces;
using System.Net;
using System.Net.Http.Formatting;

namespace biometricService.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public HttpService(string baseUrl, ITokenService tokenService)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string endpoint, TRequest data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new ObjectContent<TRequest>(data, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

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
            else if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                var details = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {response.StatusCode} - Details {details}");
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            string token = _tokenService.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.AddBearerToken(request, token);
            }
        }
    }
}
