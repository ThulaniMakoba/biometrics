using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using Microsoft.AspNetCore.Identity.Data;
using System.Globalization;
using System.Net;
using System.Net.Http.Formatting;

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
        private readonly LdapService _ldapService;
        private readonly AppSettings _appSettings;

        const string defaultUser = "SysAdmin";

        public UserService(
            IUserRepository userRepository,
            ILogService logService,
            IInnovatricsService innovatricsService,
            IFaceDataRepository faceDataRepository,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService,
            LdapService ldapService,
            AppSettings appSettings)
        {
            _userRepository = userRepository;
            _logService = logService;
            _innovatricsService = innovatricsService;
            _faceDataRepository = faceDataRepository;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _ldapService = ldapService;
            _appSettings = appSettings;
        }

        public async Task<UserModel> ValidateAuthenticationOption(AuthenticationOptionRequest request)
        {
            var user = new User();

            if (!string.IsNullOrEmpty(request.Email))
                if (_ldapService.IsUsernameInActiveDirectory(GetUsernameFromEmail(request.Email)))
                    user = await _userRepository.FindUserByEmail(request.Email);

            if (request.EdnaId != null)
                user = await _userRepository.FindUserByEdnaId((int)request.EdnaId);

            if (!string.IsNullOrEmpty(request.IdNumber))
                user = await _userRepository.FindUserByIdNumber(request.IdNumber);

            if (user == null || user?.Id == 0)
                return new UserModel { IsSuccess = false };

            return new UserModel
            {
                IsSuccess = true,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email

            };
        }
        public async Task<UserModel> ProbeReferenceFace(CreateReferenceFaceRequest request)
        {
            try
            {

                var faceData = await _faceDataRepository.FindByUserId(request.UserId);

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

        public async Task<RegisterUserResponse> RegisterUser(UserRegisterRequest registerRequest)
        {
            var user = await _userRepository.FindUserByIdNumber(registerRequest.IdNumber);

            if (user != null)
            {
                return UserAlreadyExistResponse(registerRequest);
            }

            if (user == null)
                user = await _userRepository.FindUserByEmail(registerRequest.Email);

            if (user != null)
            {
                return UserAlreadyExistResponse(registerRequest);
            }

            if (!IsDomainNameCorrect(registerRequest.Email))
            {
                return new RegisterUserResponse
                {
                    Message = "Domain name from the user's email does not match with Active Directory",
                };
            }

            var username = GetUsernameFromEmail(registerRequest.Email);
            bool usernameExists = _ldapService.IsUsernameInActiveDirectory(username);

            if (!usernameExists)
            {
                return new RegisterUserResponse
                {
                    Message = "User's email does not exist in Active Directory",
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
                IdNumber = registerRequest.IdNumber,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
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

        private string GetUsernameFromEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex == -1)
                return email;
            return email.Substring(0, atIndex);
        }

        private bool IsDomainNameCorrect(string email)
        {
            string[] parts = email.Split('@');

            if (parts.Length != 2)
                return false;

            string domain = parts[1];

            return domain == _appSettings.DomainName;
        }

        private RegisterUserResponse UserAlreadyExistResponse(UserRegisterRequest request)
        {
            _logService.Log($"User exist" +
            $"Failed user details FirstName: {request.FirstName}, LastName: {request.LastName}");

            return new RegisterUserResponse
            {
                Message = "User has already registered",
            };
        }
    }
}
