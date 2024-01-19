using Azure.Core;
using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace biometricService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpService _httpService;
        private readonly ILogService _logService;
        private readonly IInnovatricsService _innovatricsService;
        private readonly IFaceDataRepository _faceDataRepository;

        const string defaultUser = "SysAdmin";

        public UserService(IUserRepository userRepository, IHttpService httpService, ILogService logService,
            IInnovatricsService innovatricsService, IFaceDataRepository faceDataRepository)
        {
            _userRepository = userRepository;
            _httpService = httpService;
            _logService = logService;
            _innovatricsService = innovatricsService;
            _faceDataRepository = faceDataRepository;
        }

        public async Task<UserModel> ProbeReferenceFace(ProbeFaceRequest request)
        {
            try
            {
                var user = new User();
                if (request.EdnaId != null)
                    user = await _userRepository.FindUserByEdnaId((int)request.EdnaId);

                if (!string.IsNullOrEmpty(request.IdNumber))
                    user = await _userRepository.FindUserByIdNumber(request.IdNumber);

                if (user == null)
                    return new UserModel { IsSuccess = false };

                var faceData = await _faceDataRepository.FindByUserId(user.Id);

                if (faceData == null)
                    return new UserModel { IsSuccess = false };

                var existingFace = await _innovatricsService.CreateReferenceFace(new CreateReferenceFaceRequest
                {
                    detection = request.detection,
                    image = new Image { data = faceData.FaceBase64 },
                });

                var probeFace = await _innovatricsService.CreateReferenceFace(new CreateReferenceFaceRequest
                {
                    image = request.image,
                    detection = request.detection,
                });

                var probeReferenceFaceResult = await ProbeFaceToReferenceFace(probeFace.id, existingFace.id);
                var userDetails = new UserModel();

                if (probeReferenceFaceResult.IsSuccess)
                {
                    userDetails.FirstName = user.FirstName;
                    userDetails.LastName = user.LastName;
                    userDetails.Email = user.Email;
                }

                userDetails.IsSuccess = probeReferenceFaceResult.IsSuccess;

                return userDetails;
            }
            catch (Exception e)
            {
                return new UserModel { ErrorMessage = e.Message };
            }
        }

        private async Task<ScoreResponse> ProbeFaceToReferenceFace(Guid probeFaceId, Guid referenceFaceId)
        {
            try
            {
                var request = new ReferenceFaceApi() { referenceFace = $"/api/v1/faces/{referenceFaceId.ToString().ToLower()}" };

                var response = await _httpService.PostAsync<ReferenceFaceApi, ScoreResponse>($"/identity/api/v1/faces/{probeFaceId}/similarity", request);
                _logService.Log($"The combination of ProbeFaceId: {probeFaceId} and ReferenceFaceId: {referenceFaceId},has this score result: {response.Score}");

                double score = double.Parse(response.Score, CultureInfo.InvariantCulture);
                response.IsSuccess = score >= 0.89 ? true : false;
                response.Score = string.Empty;
                response.ErrorMessage = score < 0.89 ? "Score is below required threshold" : string.Empty;

                return response;
            }
            catch (Exception e)
            {
                _logService.Log($"The combination of ProbeFaceId: {probeFaceId} and ReferenceFaceId: {referenceFaceId},has this message: {e.Message}");
                return new ScoreResponse { ErrorMessage = e.Message };

            }
        }

        public async Task<RegisterUserResponse> RegisterUser(UserRegisterRequest user)
        {
            var query = await _userRepository.FindUserByIdNumber(user.IdNumber);

            if (query != null)
            {
                string message = $"User exist" +
                    $"Failed user details FirstName: {user.FirstName}, LastName: {user.LastName}";
                _logService.Log(message);

                return new RegisterUserResponse
                {
                    Message = "User exist",
                };
            }

            const int defaultEdnaId = 100001;

            var latestEdnaId = await _userRepository.LatestEdnId();

            if (latestEdnaId != 0)
                latestEdnaId += 1;
            else
                latestEdnaId = defaultEdnaId;

            var userEntity = new User
            {
                IdNumber = user.IdNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ComputerMotherboardSerialNumber = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CreatedBy = defaultUser,
                TransactionBy = defaultUser,
                TransactionDate = DateTime.Now,
                eDNAId = latestEdnaId,
                Deleted = false,
            };

            await _userRepository.Add(userEntity);

            _logService.Log($"\"Succefully register the user with id\": {userEntity.Id}");

            return new RegisterUserResponse
            {
                UserId = userEntity.Id,
                EdnaId = userEntity.eDNAId,
                Message = "Succefully register the user"
            };
        }
    }
}
