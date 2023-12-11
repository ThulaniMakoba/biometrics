using biometricService.Data.Entities;
using biometricService.Models;

namespace biometricService.Interfaces
{
    public interface IUserService
    {
        Task<int> RegisterUser(UserRegisterRequest user);
        Task<UserModel> GetUser(int id);
        Task<CreateCustomerResponse> CreateInnovatricsCustomer();
    }
}
