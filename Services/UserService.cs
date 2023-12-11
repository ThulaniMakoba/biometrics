using biometricService.Data;
using biometricService.Data.Entities;
using biometricService.Interfaces;
using biometricService.Models;
using System.Text;

namespace biometricService.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CreateCustomerResponse> CreateInnovatricsCustomer()
        {
            var client = _httpClientFactory.CreateClient("InnovatricsApi");
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/identity/api/v1/customers", content);

            var responseContent = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();

            return responseContent ?? new CreateCustomerResponse();
        }

        public Task<UserModel> GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RegisterUser(UserRegisterRequest user)
        {
            var entity = new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                InnovatricsId = user.InnovatricsId
            };
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }
    }
}
