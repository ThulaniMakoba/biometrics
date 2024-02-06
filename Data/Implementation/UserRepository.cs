using biometricService.Data.Entities;
using biometricService.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace biometricService.Data.Implementation
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> FindUserByEdnaId(int ednaId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.eDNAId == ednaId && !u.Deleted);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && !u.Deleted);
        }

        public async Task<User> FindUserByIdNumber(string idNumber)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdNumber == idNumber && !u.Deleted);
        }

        public async Task<int> LatestEdnId()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(x => x.eDNAId)
                .Select(x => x.eDNAId)
                .FirstOrDefaultAsync();
        }
    }
}
