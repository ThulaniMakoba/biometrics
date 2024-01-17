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

        const string defaultUser = "SysAdmin";

        public UserService(AppDbContext context, IHttpService httpService, ILogService logService)
        {
            _context = context;
            _httpService = httpService;
            _logService = logService;
        }

        public async Task<UserModel> ProbeReferenceFace(ProbeFaceRequest request)
        {
            try
            {
                var response = await _httpService.PostAsync<ReferenceFaceRequest, CreateReferenceFaceResponse>("/identity/api/v1/faces", new ReferenceFaceRequest
                {
                    image = request.image,
                    detection = request.detection,
                });

                var probeReferenceFaceResult = await ProbeFaceToReferenceFace(response.id, request.ReferenceFaceId);
                var userDetails = new UserModel();

                if (probeReferenceFaceResult.IsSuccess)
                    userDetails = await GetUserByFaceId(request.ReferenceFaceId);

                userDetails.IsSuccess = probeReferenceFaceResult.IsSuccess;

                return userDetails;
            }
            catch (Exception e)
            {
                return new UserModel { ErrorMessage = e.Message };
            }

        }

        private async Task<UserModel> GetUserByFaceId(Guid referenceFaceId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.InnovatricsFaceId == referenceFaceId);

            if (user == null)
                return new UserModel();

            return new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
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
                ComputerMotherboardSerialNumber = Guid.NewGuid().ToString(),
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

        public async Task UpdateUserWithReferenceFace(UpdateUserFaceDataRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UpdatedDate = DateTime.Now;
            user.InnovatricsFaceId = request.FaceReferenceId;
            user.ComputerMotherboardSerialNumber = request.ComputerSerialNumber;
            user.UpdatedBy = defaultUser;

            var faceData = new FaceData()
            {
                UserId = user.Id,
                FaceReferenceId = request.FaceReferenceId,
                FaceBase64 = request.FaceImageBase64,
                CreatedDate = DateTime.Now,
                CreatedBy = defaultUser,
                TransactionBy = defaultUser,
                TransactionDate = DateTime.Now,
            };

            _context.FaceData.Add(faceData);
            await _context.SaveChangesAsync();
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
