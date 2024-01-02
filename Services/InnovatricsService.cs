using biometricService.Http;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using System.Globalization;

namespace biometricService.Services
{
    public class InnovatricsService : IInnovatricsService
    {
        private readonly IHttpService _httpService;
        private readonly ILogService _logService;
        public InnovatricsService(IHttpService httpService, ILogService logService)
        {
            _httpService = httpService;
            _logService = logService;
        }
        public async Task<CreateCustomerResponse> CreateInnovatricsCustomer()
        {
            try
            {
                var response = await _httpService.PostAsync<CreateCustomerResponse>("/identity/api/v1/customers");
                return response ?? new CreateCustomerResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateLivenessResponse> CreateLiveness(Guid customerId)
        {
            try
            {
                var response = await _httpService.PutAsync<CreateLivenessResponse>($"/identity/api/v1/customers/{customerId}/liveness");
                return response ?? new CreateLivenessResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateLivenessSelfie(Guid customerId, CreateLivenessSelfieRequest request)
        {
            try
            {
                var response = await _httpService.PostAsync<CreateLivenessSelfieRequest, ErrorMessageModel>($"/identity/api/v1/customers/{customerId}/liveness/selfies", request);
                if (response.ErrorCode != null)
                    throw new Exception(response.ErrorCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateReferenceFaceResponse> CreateReferenceFace(CreateReferenceFaceRequest request)
        {
            try
            {
                var response = await _httpService.PostAsync<CreateReferenceFaceRequest, CreateReferenceFaceResponse>("/identity/api/v1/faces", request);
                return response;
            }
            catch (Exception e)
            {
                return new CreateReferenceFaceResponse { ErrorMessage = e.Message };
            }
        }

        public async Task<ScoreResponse> EvaluateLivenesSelfie(Guid customerId)
        {
            try
            {
                var referenceFaceRequest = new PassiveLivenessTypeRequest { type = "PASSIVE_LIVENESS" };
                var response = await _httpService.PostAsync<PassiveLivenessTypeRequest, ScoreResponse>($"/identity/api/v1/customers/{customerId}/liveness/evaluation", referenceFaceRequest);

                var convertedScored = double.Parse(response.Score, CultureInfo.InvariantCulture);
                if (convertedScored < 0.89)
                    _logService.Log($"Customer Id:{customerId} failed liveness");

                return response ?? new ScoreResponse();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
