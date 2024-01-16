using biometricService.Models;
using biometricService.Models.Responses;

namespace biometricService.Interfaces
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUser(UserRegisterRequest user); 
        Task<UserModel> ProbeReferenceFace(ProbeFaceRequest request);
        Task<VerificationResponse> VerifyUser(VerificationRequest request);
        Task UpdateUserWithReferenceFace(UpdateUserFaceDataRequest request);
        Task<RegisterFaceRequestResponse> RegisterFace(RegisterFaceRequest faceRequest);

    }
}
