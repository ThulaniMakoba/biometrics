
namespace biometricService.Http
{
    public class BearerTokenHandler: DelegatingHandler
    {
        private readonly string token = "Token";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
