using System.Net.Http.Headers;
using biometricService.Interfaces;
namespace biometricService.Http
{
    public class TokenService : ITokenService
    {
        public TokenService() { }


        //HardCode the token for now
        public async Task<string> GetToken()
        {
            return "RElTX3RyaWFsXzYwNzpVdFl3dTJkTlNyMVJKaFRMSVVMZFM0a2doT2gwZzZ5Vg==";
        }

        public HttpRequestMessage AddBearerToken(HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}
