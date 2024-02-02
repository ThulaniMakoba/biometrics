using System.Net.Http.Headers;
using biometricService.Interfaces;
namespace biometricService.Http
{
    public class TokenService : ITokenService
    {
        public TokenService() { }


        //HardCode the token for now
        public string GetToken() => "RElTX2V2YWxfNjI6TE10QUNHZzhMMUdnQ3g4N05JZjRPUUZFS0ZOZ3JHSnY=";


        public HttpRequestMessage AddBearerToken(HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }
    }
}
