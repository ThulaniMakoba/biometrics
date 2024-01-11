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
        private readonly IUserService _userService;
        public InnovatricsService(IHttpService httpService, ILogService logService, IUserService userService)
        {
            _httpService = httpService;
            _logService = logService;
            _userService = userService;
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
                var response = await _httpService.PostAsync<ReferenceFaceRequest, CreateReferenceFaceResponse>("/identity/api/v1/faces", new ReferenceFaceRequest
                {
                    image = request.image,
                    detection = request.detection
                });

                if (response.ErrorCode != null)
                    return new CreateReferenceFaceResponse { ErrorCode = response.ErrorCode, ErrorMessage = response.ErrorMessage };

                return response;
            }
            catch (Exception e)
            {
                return new CreateReferenceFaceResponse { ErrorMessage = e.Message };
            }
        }
        public async Task<CropFaceWithoutBackgroungResult> CreateReferenceFaceWithOutBackGround(CreateReferenceFaceRequest request)
        {
            try
            {
                var response = await CreateReferenceFace(request);
                return await FaceCropWithoutBackground(response.id, request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<CropFaceWithoutBackgroungResult> FaceCropWithoutBackground(Guid faceId, CreateReferenceFaceRequest referenceFaceRequest)
        {
            if (faceId == Guid.Empty)
                return new CropFaceWithoutBackgroungResult
                {
                    ErrorMessage = "Failed creating crop face"

                };


            var response = await _httpService.GetAsync<CropFaceRemoveBackgroundResponse>($"/identity/api/v1/faces/{faceId}/crop/removed-background");
            if (response.ErrorCode != null)
                throw new Exception(response.ErrorCode);

            referenceFaceRequest.image.data = response.data;

            Thread.Sleep(5000);
            var createReferenceFace = await CreateReferenceFace(referenceFaceRequest);

            if (createReferenceFace.ErrorCode != null || createReferenceFace.ErrorMessage != null)
                return new CropFaceWithoutBackgroungResult
                {
                    ErrorMessage = createReferenceFace.ErrorMessage,
                    ErrorCode = createReferenceFace.ErrorCode
                };

            var updateUserWithReferenceFace = new UpdateUserFaceDataRequest
            {
                FaceImageBase64 = response.data,
                FaceReferenceId = createReferenceFace.id,
                UserId = referenceFaceRequest.UserId,
                ComputerSerialNumber = referenceFaceRequest.ComputerSerialNumber
            };

            await _userService.UpdateUserWithReferenceFace(updateUserWithReferenceFace);

            return new CropFaceWithoutBackgroungResult
            {
                Base64Image = referenceFaceRequest.image.data,
                Id = createReferenceFace.id
            };
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
