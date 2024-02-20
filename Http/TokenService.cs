using System.Net.Http.Headers;
using biometricService.Interfaces;
namespace biometricService.Http
{
    public class TokenService : ITokenService
    {
        public TokenService() { }

        public HttpRequestMessage AddBearerToken(HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}
