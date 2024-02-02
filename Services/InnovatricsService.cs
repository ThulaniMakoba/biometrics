using Azure.Core;
using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Net.Http;
using biometricService.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;

namespace biometricService.Services
{
    public class InnovatricsService : IInnovatricsService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogService _logService;
        private readonly IUserRepository _userRepository;
        private readonly IFaceDataRepository _faceDataRepository;
        private readonly ITokenService _tokenService;

        public InnovatricsService(
            ILogService logService,
            IUserRepository userRepository,
            IFaceDataRepository faceDataRepository,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService)
        {
            _logService = logService;
            _userRepository = userRepository;
            _faceDataRepository = faceDataRepository;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }
        public async Task<CreateCustomerResponse> CreateInnovatricsCustomer()
        {
            var result = await CreateCustomer();

            if (result.Id == null)
                result = await CreateInnovatricsCustomer();

            return result;
        }

        private async Task<CreateCustomerResponse> CreateCustomer()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/identity/api/v1/customers")
                {
                    Content = new ObjectContent<CreateCustomerResponse>(null, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(request);
                HttpResponseMessage response = await client.SendAsync(request);
                var result = await HandleResponse<CreateCustomerResponse>(response);
                return result ?? new CreateCustomerResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateLivenessResponse> CreateLiveness(Guid customerId)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"/identity/api/v1/customers/{customerId}/liveness")
                {
                    Content = new ObjectContent<object>(null, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(request);
                HttpResponseMessage response = await client.SendAsync(request);

                var result = await HandleResponse<CreateLivenessResponse>(response);
                return result ?? new CreateLivenessResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateLivenessSelfie(Guid customerId, CreateLivenessSelfieRequest request)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {
                var requestData = new HttpRequestMessage(HttpMethod.Post, $"/identity/api/v1/customers/{customerId}/liveness/selfies")
                {
                    Content = new ObjectContent<CreateLivenessSelfieRequest>(request, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(requestData);
                HttpResponseMessage response = await client.SendAsync(requestData);

                var result = await HandleResponse<ErrorMessageModel>(response);

                if (result.ErrorCode != null)
                    throw new Exception(result.ErrorCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CreateReferenceFaceResponse> CreateReferenceFace(CreateReferenceFaceRequest request)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {
                var data = new ReferenceFaceRequest
                {
                    image = request.image,
                    detection = request.detection
                };

                var requestCall = new HttpRequestMessage(HttpMethod.Post, "/identity/api/v1/faces")
                {
                    Content = new ObjectContent<ReferenceFaceRequest>(data, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(requestCall);
                HttpResponseMessage response = await client.SendAsync(requestCall);

                var result = await HandleResponse<CreateReferenceFaceResponse>(response);

                if (result.ErrorCode != null)
                    return new CreateReferenceFaceResponse { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage };

                return result;
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

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");

            var request = new HttpRequestMessage(HttpMethod.Get, $"/identity/api/v1/faces/{faceId}/crop/removed-background")
            {
                Content = new ObjectContent<object>(null, new JsonMediaTypeFormatter())
            };

            AddAuthorizationHeader(request);

            HttpResponseMessage response = await client.SendAsync(request);

            var result = await HandleResponse<CropFaceRemoveBackgroundResponse>(response);

            if (result.ErrorCode != null)
                throw new Exception(result.ErrorCode);

            referenceFaceRequest.image.data = result.data;

            var user = await _userRepository.FindById(referenceFaceRequest.UserId);

            if (user.Id == 0)
            {
                return new CropFaceWithoutBackgroungResult
                {
                    ErrorMessage = "User does not existing for face capture"
                };
            }

            var faceData = new FaceData()
            {
                UserId = user.Id,
                FaceReferenceId = null,
                FaceBase64 = referenceFaceRequest.image.data,
                CreatedDate = DateTime.Now,
                CreatedBy = "SysAdmin",
                TransactionBy = "SysAdmin",
                TransactionDate = DateTime.Now,
            };

            await _faceDataRepository.Add(faceData);

            return new CropFaceWithoutBackgroungResult
            {
                Base64Image = referenceFaceRequest.image.data,
            };
        }

        public async Task<ScoreResponse> EvaluateLivenesSelfie(Guid customerId)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://dot.innovatrics.com");
            try
            {
                var referenceFaceRequest = new PassiveLivenessTypeRequest { type = "PASSIVE_LIVENESS" };

                var request = new HttpRequestMessage(HttpMethod.Post, $"/identity/api/v1/customers/{customerId}/liveness/evaluation")
                {
                    Content = new ObjectContent<PassiveLivenessTypeRequest>(referenceFaceRequest, new JsonMediaTypeFormatter())
                };

                AddAuthorizationHeader(request);

                HttpResponseMessage response = await client.SendAsync(request);

                var result = await HandleResponse<ScoreResponse>(response);

                var convertedScored = double.Parse(result.Score, CultureInfo.InvariantCulture);
                if (convertedScored < 0.89)
                    _logService.Log($"Customer Id:{customerId} failed liveness");

                return result ?? new ScoreResponse();
            }
            catch (Exception)
            {
                throw;
            }
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
