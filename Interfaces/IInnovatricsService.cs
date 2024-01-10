using biometricService.Models.Responses;
using biometricService.Models;

namespace biometricService.Interfaces
{
    public interface IInnovatricsService
    {
        Task<CreateCustomerResponse> CreateInnovatricsCustomer();
        Task<CreateReferenceFaceResponse> CreateReferenceFace(CreateReferenceFaceRequest request);
        Task<CropFaceWithoutBackgroungResult> CreateReferenceFaceWithOutBackGround(CreateReferenceFaceRequest request);
        Task<CreateLivenessResponse> CreateLiveness(Guid customerId);
        Task CreateLivenessSelfie(Guid customerId, CreateLivenessSelfieRequest request);
        Task<ScoreResponse> EvaluateLivenesSelfie(Guid customerId);
    }
}
