
namespace biometricService.Http
{
    public class BearerTokenHandler: DelegatingHandler
    {
        private readonly string token = "RElTX3RyaWFsXzYwNzpVdFl3dTJkTlNyMVJKaFRMSVVMZFM0a2doT2gwZzZ5Vg==";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",token);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
