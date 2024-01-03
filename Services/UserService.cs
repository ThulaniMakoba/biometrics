using Azure.Core;
using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Interfaces;
using biometricService.Models;
using biometricService.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace biometricService.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IHttpService _httpService;
        private readonly ILogService _logService;

        public UserService(AppDbContext context, IHttpService httpService, ILogService logService)
        {
            _context = context;
            _httpService = httpService;
            _logService = logService;
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

        public async Task<ScoreResponse> ProbeReferenceFace(ProbeFaceRequest request)
        {
            try
            {
                var referenceFaceRequest = new CreateReferenceFaceRequest { detection = request.detection, image = request.image };
                var response = await _httpService.PostAsync<CreateReferenceFaceRequest, CreateReferenceFaceResponse>("/identity/api/v1/faces", referenceFaceRequest);

                var result = await ProbeFaceToReferenceFace(response.id, request.ReferenceFaceId);

                return result;
            }
            catch (Exception e)
            {
                return new ScoreResponse { ErrorMessage = e.Message };
            }

        }

        private async Task<ScoreResponse> ProbeFaceToReferenceFace(Guid probeFaceId, Guid referenceFaceId)
        {
            try
            {
                var request = new ReferenceFaceApi() { referenceFace = $"/api/v1/faces/{referenceFaceId.ToString().ToLower()}" };

                var response = await _httpService.PostAsync<ReferenceFaceApi, ScoreResponse>($"/identity/api/v1/faces/{probeFaceId}/similarity", request);
                _logService.Log($"The combination of ProbeFaceId: {probeFaceId} and ReferenceFaceId: {referenceFaceId},has this score result: {response.Score}");
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
            var defaultUser = "SysAdmin";

            var query = await _context.Users
                .FirstOrDefaultAsync(x => x.IdNumber == user.IdNumber && !x.Deleted);

            if (query != null)
            {
                string message = $"Cannot register a new user on this computer, already an existing user." +
                    $"Failed user details FirstName: {user.FirstName}, LastName: {user.LastName}";
                _logService.Log(message);

                return new RegisterUserResponse
                {
                    Message = "Cannot register a new user on this computer, already an existing user.",
                };
            }

            var userEntity = new User
            {
                IdNumber = user.IdNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                CreatedDate = DateTime.Now,
                CreatedBy = defaultUser,
                TransactionBy = defaultUser,
                TransactionDate = DateTime.Now,
                Deleted = false,
            };

            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            _logService.Log($"\"Succefully register the user with id\": {userEntity.Id}");

            return new RegisterUserResponse { UserId = userEntity.Id, Message = "Succefully register the user" };
        }

        public async Task<VerificationResponse> VerifyUser(VerificationRequest request)
        {
            var query = await _context.Users
                 .Where(x => x.ComputerMotherboardSerialNumber == request.ComputerMotherboardSerialNumber && !x.Deleted)
                 .FirstOrDefaultAsync();

            var response = new VerificationResponse();

            if (query == null)
            {
                response.UserExist = false;
                return response;
            }

            response.UserExist = true;
            response.ReferenceFaceId = query.InnovatricsFaceId;
            return response;
        }

    }
}
