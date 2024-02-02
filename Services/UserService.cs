using Azure.Core;
using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Formatting;
using System.Net.Http;
using biometricService.Http;
using System.Net;

namespace biometricService.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IUserRepository _userRepository;
        private readonly ILogService _logService;
        private readonly IInnovatricsService _innovatricsService;
        private readonly IFaceDataRepository _faceDataRepository;
        private readonly ITokenService _tokenService;

        const string defaultUser = "SysAdmin";

        public UserService(
            IUserRepository userRepository,
            ILogService logService,
            IInnovatricsService innovatricsService,
            IFaceDataRepository faceDataRepository,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _logService = logService;
            _innovatricsService = innovatricsService;
            _faceDataRepository = faceDataRepository;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
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
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {

                var data = new ReferenceFaceApi() { referenceFace = $"/api/v1/faces/{referenceFaceId.ToString().ToLower()}" };

                var request = new HttpRequestMessage(HttpMethod.Post, $"/identity/api/v1/faces/{probeFaceId}/similarity")
                {
                    Content = new ObjectContent<ReferenceFaceApi>(data, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(request);

                HttpResponseMessage response = await client.SendAsync(request);
                var result = await HandleResponse<ScoreResponse>(response);

                double score = double.Parse(result.Score, CultureInfo.InvariantCulture);

                result.IsSuccess = score >= 0.89 ? true : false;
                result.Score = string.Empty;
                result.ErrorMessage = score < 0.89 ? "Score is below required threshold" : string.Empty;

                return result;
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
                    Message = "User has already registered",
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

            return new RegisterUserResponse
            {
                UserId = userEntity.Id,
                EdnaId = userEntity.eDNAId,
                Message = "Succefully registered the user"
            };
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            string token = _tokenService.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.AddBearerToken(request, token);
            }
        }

        public static async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.Content.ReadAsAsync<T>();
                //Need to find a solution....
                //throw new HttpRequestException($"Error: {response.StatusCode} - Details {response.ReasonPhrase}");
            }
            else
            {
                throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
}
