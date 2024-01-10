using System.Net.Http.Headers;
using biometricService.Interfaces;
namespace biometricService.Http
{
    public class TokenService : ITokenService
    {
        public TokenService() { }


        //HardCode the token for now
        public string GetToken() => "RElTX2V2YWxfMzQ6SGd3dThnQ2NMdlMyWlVza05lOFlDQVFyNzlpV1dOZ1U=";


        public HttpRequestMessage AddBearerToken(HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}
