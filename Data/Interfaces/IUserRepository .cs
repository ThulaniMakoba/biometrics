using biometricService.Data.Entities;

namespace biometricService.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindUserByEdnaId(int ednaId);
        Task<User> FindUserByIdNumber(string idNumber);
        Task<int> LatestEdnId();
    }
}
