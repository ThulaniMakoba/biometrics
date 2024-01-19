using biometricService.Data.Entities;

namespace biometricService.Data.Interfaces
{
    public interface IFaceDataRepository : IRepository<FaceData>
    {
        Task<FaceData> FindByUserId(int userId);
    }
}
