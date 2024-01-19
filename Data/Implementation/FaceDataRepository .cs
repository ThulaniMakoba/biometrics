using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace biometricService.Data.Implementation
{
    public class FaceDataRepository : Repository<FaceData>, IFaceDataRepository
    {
        private readonly AppDbContext _context;
        public FaceDataRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FaceData> FindByUserId(int userId)
        {
            return await _context.FaceData
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync(f => f.UserId == userId);
        }
    }
}
