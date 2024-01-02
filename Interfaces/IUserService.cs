using biometricService.Models;
using biometricService.Models.Responses;

namespace biometricService.Interfaces
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUser(UserRegisterRequest user);
        //Task<CreateCustomerResponse> CreateInnovatricsCustomer();
        Task<CreateReferenceFaceResponse> CreateReferenceFace(CreateReferenceFaceRequest request);
        Task<ScoreResponse> ProbeReferenceFace(ProbeFaceRequest request);
        Task<VerificationResponse> VerifyUser(VerificationRequest request);
        //Task<CreateLivenessResponse> CreateLiveness(Guid customerId);
        //Task CreateLivenessSelfie(Guid customerId, CreateLivenessSelfieRequest request);
        //Task<ScoreResponse> EvaluateLivenesSelfie(Guid customerId);

    }
}
